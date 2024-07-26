# Technical Description (high priority)


We outline our technical description here.
First we begin with a high level explanation of WFC and its two models (overlapping and simpletiled).
Then we continue on by explaining how the overlapping model generates patterns from images, and how the simpletiled model infers rules from a rule set, both of which are used during the image generation process.
We then explain an issue we faced with the overlapping model and how we overcame this problem through mapping tilemaps to a pixel bitmap representation, which ensured that locality was preserved.
Then we explain how we hooked into the overlapping model to extract rules from the pattern generating process.

To begin, WFC can be broken down into two phases, the pattern/rule generation phase followed by the constraint solving phase.
The first phase of the algorithm will have WFC generate rules and a constraint graph based on either an input image or user defined dimensions.
The second phase which uses those rules to solve a constraint graph
(mention something about the two parts that WFC is, a rule generation approach followed by a constraint solving approach)

Starting first with the learning model of WFC, also known as the Overlapping model, this model focuses learning patterns from an input image.
The overlapping model learns by creating an NxN filter (N can be user defined or set to a small value such as 2 or 3).
The filter "slides" over the input image, one pixel position at a time to create pixel associations, acting as a kernel from Computer Vision techniques, but instead of performing a convolution on the image, the pixel values are copied and their association is saved.
The association itself captures the adjacent pixels pairs, which is later used in WFC's generation process.
This process continues until the entire image has been processed.
The Overlapping model then moves on to the image generation step, which will be explained after explaining the Simpletiled model as both models share the same process for generating images.

The Simpletiled model focuses on using design crafted rules for its generation process, skipping the learning step altogether.
Design rules are input to the model--the rules themselves are encoded as left/right neighbor associations.
As for the rules, before any generation occurs, the Simpletiled model uses the inputted rules to infer additional rules, all based on rotating the two neighbors. 
The inferred rules will often include information such as new left/right, right/left, and up/down pairings.
Additionally, all tiles that are given a unique rotation, will have new images generated based on that rotation. 

Finally, both models share the same type of constraint solving approach, which we did not experiment with.
As for how it works, we point to Karth's paper for a more indepth explanation, but a basic overview is that a matrix is first generated with each cell containing all possible choices (this in effect is the "superposition" matrix that this algorithm gets its name from).
The first cell will be picked and a definite value will be randomly chosen.
The choice will be propagated through the matrix, removing all incompatible adjacent neighbors.
Once all incompatible choices are removed from the matrix, a new cell will be chosen--this time the choice will be based on a heuristic. 
A choice within that cell will be picked and the propagation phase will happen once more.
This process of choice and propagation will continue until the matrix is in a stable state, where there is a value in all cells, or an unstable state where some cells have no values, which indicates the generation phase has failed.

The only difference between the two approaches is that a tileset must be provided from the Simpletiled model, as it does not generate its rules from an input image, like the Overlapping model. 
We do wish to mention that the only modification to the generation process was ensuring the output of both models was indeed a stable image, as both began to give false positives as the requested image sizes grew larger.
This phenomenon has been documented by Merrel when comparing WFC to his work, Model Synthesis, which we will expand more on in the analysis section.

As for our interests, we focused on the pixel association part of the overlapping model. 
The pixel association part, as we discovered, is where the algorithm stores all local relational information.
Originally we believed it would be rather straightforward to extract rules from the pixel association, but from our experience, the overlapping model works only with pixel maps rather than tilemaps. 
When given a tilemap, where each tile is an NxN pixel matrix, WFC would still create pixel associations, sliding the filter over by one pixel rather than by the size of the NxN pixel matrices that make up tiles. 
The end result is a garbled image, which is shown in Figure M.

To address the pixel garbling without modfying WFC, we devised a way to downscale tilemaps into a pixel bitmaps that could then be used as input into the overlapping model. 
Before the overlapping model executed, the input image would be processed. 
A filter would slide over the image, sliding by a user defined tile size of pixels, extracting the tile (which would be a subregion within the image) and the newly extracted tile would be compared against a set of seen tiles.
If the tile hasn't been seen before, save the tile and add it to the unique tiles.
If it has been seen, continue without saving but increase the count score of that tile for later frequency/weight generation.
In addition to downscaling the image, we also extracted tiles from the image, creating an extracted tile set for later use.

Once the entire image has been processed, each unique tile would get a weight that represented their frequency within the original image (this would be computed by the count score mentioned earlier). 
The list of unique tiles would then be mapped to a pixel representation for each unique tile.
For generating a unique pixel per tile, we set three variables (red, green, and blue, each representing their respective color channels) to zero; red is incremented until it hits 255, then green will increment by one with red resetting to zero. This will repeat until green also reaches 255, in which case blue will increment by one and both red and green will reset to zero.
The tile list is iterated through and a pixel value is assigned to that tile, then red is incremented. 

After the pixel mapping is done, the downscaling algorithm continues by generating a new empty bitmap with the size of the original input image width and height but divided by the tile size specified by the user.
If the original image size is 128x128 and the tiles are 16x16, then the new pixel bitmap will be 8x8, as 128 divided 16 is 8.
The downscaling algorithm then process the image once more, matching the image's tile with the unique tiles that were extracted from the first processing step. 
The pixel value of that tile will then be put into the pixel bitmap. 
This process will continue until the entire image has been processed and the pixel bitmap has a pixel in every position. 
The bitmap is then inputted into the overlapping model where the model will then learn the associations between pixels, effectively learning the tile associations as well. 

TODO: Please make sure to mention how we are mapping tiles back to their tile set if we know that information, as it is crucial for the results section.
Once the overlapping model has processed the image, learning all of the associations, we add additional code that iterates through the patterns, generating left/right rules from the pixel values, which are then converted to the mapped tile value after a processing step.
This is done for all left/right pairs within the pair. 
In all our testing, the pattern size is set to 2x2, as to ensure we are learning the tile adjacency information that is needed to generate the neighbor rules that the simpletiled model requires. 
As for why we extract all possible left/right associations within the pattern, is to ensure full pixel association extraction. 
These left/right pairs are mapped from the pixel value back to the original tile value, thus giving the basis for the generated left/right ruleset.
The rules are then saved for later use alongside a newly generated image that should resemble the original tilemap.
It is possible to generate the rule set without the accompanying image, but we found the generated image useful for helping better understand the rules that are generated.
