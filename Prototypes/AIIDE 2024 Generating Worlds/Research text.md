Research Text
==========================================================================
# Figures needed

tile to pixel mapping
Figure which shows the process of how we generate rules and then test those rules

# Abstract (done)

WaveFunctionCollapse (WFC), is a Procedural Content Generation (PCG) technique that learns pixel associations from the given input to generate similar classes of output. By learning pixel associations, WFC lets users bypass the tedious and time consuming step of decomposing their domain knowledge of a design area into a form that a WFC can understand. Yet, the designers lose fine control over the generative space that developing their own rules gives them. This paper explores automatic rule generation through WFC by exposing the pattern-based learning used by WFC to generate its pixel associations. By generating rules in the same representation as human designers, we hope this approach would ease the hard authorial task of crafting a bespoke rule set for WFC and address the authorial burden problem while prompting transparency. To better understand the internal association generation, we will explore the algorithm within this paper, focusing mostly on the pattern learning aspect of WFC. Focusing on these two aspects of the algorithm enables us to understand how to generate rules automatically and also test their validity. Finally, we supply a preliminary set of evaluation techniques to ensure that the generated rules capture the patterns within the input image, and can generate images through the rules that are similar to the original input image. By generating rules we hope to better understand the first steps towards creating a mixed initiative level tool centered on WFC that both allows users to give a series of training images, but also allow for the flexibility of adding, removing, and editing the generated rules for more user control of the generative process.

# Introduction

Because designers often have internalized their process, it can be hard to pull out their own design knowledge and write rules about how levels should be generated. 

... we need to rework this entire section, but don't worry about it right now, this should be the last section.

Currently the argument that I should make is a mix of authorial burden, much like the argument made in the Hierarchical WaveFunctionCollapse paper and enabling new avenue of research and inquiry through having a method for transfer learning. [paper](https://ojs.aaai.org/index.php/AIIDE/article/view/27498/27271)  
But also I think we are making an argument that this allows us a new avenue of learning, much like the PCGML papers.

Don't talk about reskinning if you don't like it?
I don't love this simple reskin take... but I think that there might be some good framing that we can use for later points.
While this seems like a simple reskin, we looking at a way of making these rulesets more general, ensuring that different types of tilesets can use the same ruleset.


Thus, in this paper, we contribute a novel way to do transfer learning through learning rulesets to allow for both a more explainable AI (make sure that there is a citation for this argument) {CITE}.

We achieve this novel transfer learning through using WaveFunctionCollapse and its learning functionality, but also the other model it has.
Our hope is that this approach to transfer learning will help create new ways of designing with these generative AI techniques.


Within this paper, we look at how to extract the rules from WFC, as well as evaluating them through a series of simple tests to show that our rule extraction methodology works. We also explore other avenues of quick generation pipelines such as using LLMs to generate rules and images based off a ground truth tilemap that we use for a comparison.
We are exploring WFC and its capabilities for automatic rule generation as a first step towards creating a mixed initiative system with a WFC-like algorithm at its core for designers.

# Related Work (high priority)

This section should focus on all of the WFC work that has happened recently, also mentioning something about the Model Synthesis work. Then after that talk about the various systems that have been built with WFC, like TileTerror, AestheticBot, Sturgeon and how WFC has lead to PCGML and other approaches like PCGML

- WaveFunctionCollapse 
    TileTerror
    Sturgeon
    the pcgml works that are out there
    ... other wfc papers lmao 
    and model synthesis

If anything, we will need papers about automatic rule generation with procedural systems.

# Technical Description (high priority)

Give a brief overview of the technical description.
Mention all additional code we wrote in this section
But give a high level overview that we can use to compact this section.
--- A guideline ---


To outline the technical description, we will be discussing four major parts. 
The first will be a high level explanation of WFC and its two models, followed then by a deeper explanation of how patterns are formed within WFC and how WFC infers rules.
Then we will explain the issues we faced with WFC's learning model (overlapping) as well as how we overcame the issue through encoding tilemaps into pixel bitmaps for input into the WFC algorithm.
Finally, we explain how we extract rules from the generated patterns within WFC.


## An explanation of WFC's rules based model and pattern based model.

While Karth and co give an detailed explanation of WFC within their work, we will to give a brief explanation of the two models that WFC has available to users, the overlapping and simpletiled models. 

(ensure that you explain the pixel association just in here)
(also make sure you describe the rules as neighbor rules, which capture the adjacency relationships between tiles)
The overlapping model of WFC focuses on learning patterns from an input image. 
The overlapping model does this by creating an NxN filter (N can be user defined); the filter "slides" over the input image, one pixel at a time to create pixel associations, which will then later be used in the generation process.
To clarify the process, if the original image contained a checkerboard patterning of red and black pixels, then the filter (assuming N is equal to two) will note that black is left of red and red if left of black.
The adjacency of the pixels creates the pattern, which WFC learns and uses for the generation process.
It should be noted that WFC will create an association between all of the pixels within the NxN pattern. 
So with the checkerboard example, the pixels on the diagonal will have an association as well the top and bottom rows. 
Effectively the pattern will be a fully connected association, at least to the best of our knowledge
The overlapping model does not require either rules or a tile/pixel set for generation, as the model will learn everything from the inputted image.

As for the other version of WFC, which is known as the simpletiled model, this model relies on rules for its generation process, skipping the learning step that the overlapping takes, requiring the user to supply their own rule set and tile set for generation.
These rules are fed in as input, alongside a set of tiles or pixels of whatever image patterns the user wishes to use.
The model will take the rules as a basis for further rule inference--more on this later--and the model will generate a new image that is adherent to rules.  
When reading the rules from a source, the simpletiled model will also infer additional rules, such as up/right rules and right/left rules. 
These additional rules will become important later in this paper.
Our best guess as to why the additional inference occurs is to ensure a large enough rule set for the generation step.
Additionally, during this rule generation phase, the tiles themselves are rotated, allowing for one tile to become a subset of additional tiles, once again increasing the possible solution space.

Both models share the constraint solving aspect, which we did not experiment with. The only modification done to the constraint solving was done to the simpletiled model, which began to fail more often as we attempted to generate larger output (around a 40x40 output with the Summer ruleset). More on the reason why this occur later in this paper.

(Mention that this is the downscaling portion)
As for our interests, we focused on the pixel association part of the overlapping model. 
The pixel association part, as we discovered, is where the algorithm stores all local relational information.
Originally we believed it would be rather straightforward to extract rules from the pixel association, but from our experience, the overlapping model works only with pixel maps rather than tilemaps. 
When given a tilemap, where each tile is an NxN pixel matrix, WFC would still create pixel associations, sliding the filter over by one pixel rather than by the size of the NxN pixel matrices that make up tiles. 
To deal with this issue, we devised a way to downscale tilemaps into a pixel bitmaps that could then be used as input into the overlapping model. 
Our approach was rather simple, before the overlapping model executed, the input image would be processed. 
A filter would slide over the image, sliding by a user defined tile size of pixels, extracting the tile (which would be a subregion within the image) and the newly extracted tile would be compared against a set of seen tiles.
If the tile hasn't been seen before, save the tile and add it to the unique tiles.
If it has been seen, continue without saving but increase the count score of that tile for later frequency/weight generation.

Once the entire image has been processed, each unique tile would get a weight that represented their frequency within the original image (this would be computed by the count score mentioned earlier). 
The list of unique tiles would then be mapped to a pixel representation for each unique tile.
Our tile-to-pixel mapping is rather straightforward. 
We set three variables (red, green, and blue) to zero, which are used to generate the pixel color. 
The tile list is iterated through and a pixel is assigned to that tile, then red is incremented. 
This continues until there are no more tiles, or red has reached 255 in which case the green channel will increase until reaching the max value of 255, and then finally the blue channel will be incremented.
The appendix contains a figure showing this mapping.

After the pixel mapping is done, the downscaling algorithm will continue by generating a new empty bitmap that is the size of the original image width and height but divided by the tile size specified by the user.
The downscaling algorithm then process the image once more, matching the image's tile with the unique tiles that were extracted from the first processing step. 
The pixel value of that tile will then be put into the pixel bitmap. 
This process will continue until the entire image has been processed and the pixel bitmap has a pixel in every position. 
The bitmap is then inputted into the overlapping model where the model will then learn the associations between pixels, effectively learning the tile associations as well. 

Once the overlapping model has processed the image, learning all of the associations, we add additional code that iterates through the patterns, generating left/right rules from the pixel values, which are then converted to the mapped tile value after a processing step.
This is done for all left/right pairs within the pair. 
In all our testing, the pattern size is set to 2x2, as to ensure we are learning the tile adjacency information that is needed to generate the neighbor rules that the simpletiled model requires. 
As for why we extract all possible left/right associations within the pattern, even though most will be redudant (as the overlapping model will the bottom pixels as top pixels eventually), we believe that the overlapping model does not capture the patterns on the outermost edges of the image, to ensure there is no indexing errors.
Thus we capture the edge patterns as well.
The rules are then saved for later use.

# Evaluations, Analysis, Discussion (high priority)

Mention a hypothesis at this point, so that way readers know what to expect.
Before testing, we believe that there is a relationship between tile set size and the total generated rule coverage (effectively how many generated rules did we expect to match with the ground truth rules we generated from), this is simply from having more tiles to learn from.

## how we evaluate the rules through the similarity test

For the testing to see if our generated rules were approximant representations of hand crafted rules that would be given to WFC.
A rather straight forward way we test this is to first generate an image using the rules version of WFC, then feed in this synthesized image into the overlapping model, which then learns the associations.
We extract the rules through the associations as outlined in the technical description, and then compare the extracted rules against the original ruleset that generated the input image. 
Figure N shows this pipeline.
To compare, we iterate through the generated rule set, comparing each rule against the original set, if there is a matching rule, then we increment a matching rule count. After all the rules have been compared, we simply take the total matching count and divide the total with the crafted rule count to get the percentage of original rules that the generated rules also has.
We also do the same percentage calculation for the generated rules, seeing how many additional inferred rules that the generated rule set has.

Overall we see that the generated rules will contain over half of the rules from the original set. 
As for why there are additional rules that do not exist in the original rule set, we believe this is due to the rule inference that the rules based version of WFC does. 
As in, for each left right pairing, there will then be an addition up down and various permutations of those rules.
(Maybe give an example here so that way everyone can understand, and also showcase the output of the algorithm when these aren't these inferred rules?)
We believe these inferred rules lead the pixel association model to learn these relationships as well, which then in turn are extracted as rules through our rule extraction process.

We ran the testing procedure 100 times, generating both new maps from the simpletiled model each round for the overlapping model to have a larger variety to learn from. 
The rules were then extracted and saved on disk for later compilation. 

## results and analysis of the rule similarity


Mention about the inferred rules and how the matching set increased, and then do so for each of the different type of rules... 


Maybe we make the case for the up down pairs in the analysis to explain why the output is so bad. That way we can hedge our bets with that description.
If that is possible, seeing as the how the up down relationships might require a different type of output or rewriting of the simpletiledmodel.

As we assumed, tilemaps that didn't capture their entire tileset on average did not perform well when rule matching between the ground truth rules and the generated rules. 
NOTE: There should be testing here done with the summer set vs the other sets that have subsets.



--- we might possibly generate a compiled rule set, to see if we can extract all the rules and if there is a limit to the inferred rules.
Further testing on the generative rules by compiling a critical mass of rules, finding if and when they hit a point of no more additions from the generative output. 

Mention how tile amounts and the complexity of the tilesets will lead to more failure rates... Assuming that is the case.
We should also show the pattern amount, to discuss how there needs to be a sufficient amount of patterns to generate a sufficient amount of rules (there is a really good graph here, I think about the pattern amount with the tile number plotted with the rule set similarity value? 
Or at least something to showcase that there appears to be some kind of relationship between the needed size of image based on number of tiles for a good rule coverage)

we do get full coverage on the summer rule set... which is interesting

WFC begins to fail around 40x40 but you have an explanation that depends on Merrel's explanation.

Which then implies that there is a sweet spot for WFC generation, or that while WFC is an interesting algorithm, there are other algorithms out there. Mention how there is a sweet spot of generation when it comes to WFC.

When talking about the compiled rule set, mention that this is a response to when NxN is low and all the additional rules help with the generative process (if they do, actually) 
There is a line of reasoning/explanation that can be given about the poor performance of WFC's output due to the self-similarity issue, which is that once a certain amount of non closing loops exist within an image, then WFC will begin to fail (which we can show an example of with the overworld generation process)

make sure to talk about the tile frequencies and why they are important, showing an example of this would be fantastic as well, but at some point all we are doing is showing the output of WFC, so make sure that is clear.

When it comes to evaluating levels, consider the whole complexity metric of dungeons through the dungeon and dragon metric where there is a line through the dungeon and each break from that line showcases more of a branching factor or something like that. While I think that dungeon generation is out of scope for this project, let's try to generate some legend of zelda dungeons and see what we get.

Something we did not expect to be an issue is how the simpletiled model will use subsets for generation, revealing a weakness in our approach, which is that we are unable to detect sub patterns within the generated artifact. 
But we believe that this is outside of the scope of this paper as WFC might not be the best algorithm for this type of generative rule process.

## generating rules with gemini and generating images with copilot. (I think this section should come after we explain how we are testing the ruleset

Before we conclude the results section, we want to take a brief aside to focus on other generative techniques that we believe designers and developers may go to first before attempting to craft their own rules, which we believe are Large Language Models (LLMs). 
We want to first inspect these approaches as they are some of the most accessible tools for designers who may not want to write their own rules and supply models such as copilot or Gemini the rule sets that come with WFC and hope for output that works, or the designer might instead bypass WFC and other generative algorithms of its class, like Model Synthesis, for DALLE or Midjourney.
As of writing this paper, our testing of these approaches does not work to the level of accuracy we were hoping for. 

As for our procedure when generating the Gemini rule set, we prompted Gemini by supplying the Summer neighbor rules that we have been working with, requesting it to generate a similar set of neighbor rules, which it then generated. 
We took the rules and ran the Simpletiled model 20x20 size, hoping the size would not restrict the rule set so much. 
We then compare these rules against the ground truth rule that we used, hoping for a similarity score. 
As of writing this paper, we have not had a successful run with an unmodified rule set generated by Gemini.
(if we have space, put the rules in the appendix).

Explain how the tilemap images were generated.
As for the DALLE generated images, we supplied Copilot (which uses DALLE-3 at the time of this writing) a generated tile map image, requesting a similar output, but all the generated images were not of the same class of image for top-down tile map but instead other types of game maps, such as an isometric tilemap or even table-top inspired. 
While we think this is fine for the creative process of generating different ideas, it does not meet the requirement of generating a tilemap that resembles the input image in the way that is often sought after when using procedural algorithms, such as WFC.
(if we have space, add the images in the appendix.)

We will be the first to admit that this comparison might not seem wholly fair, as Gemini and Copilot are general models that aren't highly specialized in the same manner as WFC. Yet, we still include these tests as we think it is a reasonable comparison as these are current tools that designers will most likely reach for when trying to generate more content for their projects or games.
We do, however, believe that there possible inclusion of these larger models for some aspect of the generative process, more of which will be in the future work section.


# Future work (low priority)

The issue with the other outputs is that we need to look at the subsets as well.
As you have been collecting more output from the other rule sets, there is a clear showcase that the rotations and subsets matter a lot, and those things aren't something you can detect, so you need to make sure that is clear and you have some working solution for that.
So mention why you think the problem exists, but only test against the rule sets that do have subsets.

Mention about how the next steps will also include learning more about the rule generation and how this work will lead to creating a new algorithm that does effectively what WFC does but with the stuff that Model Synthesis does as well because we want to create an image generating algorithm for tilemaps. (Mention something about self-similar tilemaps and closed loops.)

- implementing subset detection within images, as in trying to find if there is an area within the image that has a higher density of certain tiles, which can then allow for generating rules that capture that subset.
- using this technique to create a mixed initiative tool with WFC at its core.
- Introducing larger structure patterns alongside the tile generation within WFC.
- looking at annotating the generated rules to allow for additional annotations, like the work done in TileTerror.

- we could also discuss a little about using the rules to learn about design decisions, similar to what Cook and co do in this paper. https://repository.falmouth.ac.uk/2945/1/bare_conf.pdf (NOTE: that this a low priority idea?)

If we can use this system to detect patterns, then we can let the developer of the tilemap know that there are these frequent patterns in there tilemaps, interesting future work idea
Effectively this research project is working towards the idea of "autocomplete" for tilemap generation. 
As in there should be some tool or software that a tilemap designer can boot up and then feed in their own tilemaps to get suggestions on any partially designed maps.
While our original hope was using something like the Melan diagram to showcase that the dungeon is complex, but the key insight of the Melan diagram is that it shows complexity through a branching factor--one which we plan to use to showcase the complexity of our dungeon.
But for the sake of this paper, we are not doing any dungeon generation, but instead looking at a specific room.
We hope to take what we learned here and apply it to dungeon generation.

We noticed that within the base implementation of WFC, larger image sizes begin to fail, such as a 50x50 tilemap generation for the Summer tile set, we are not sure why this is the issue, so far one of the few sources that talks about the strange increased failure rate of WFC at larger image sizes is Merrel's comparison between WFC and his Model Synthesis work. https://paulmerrell.org/wp-content/uploads/2021/07/comparison.pdf 
This paper explains why WFC starts failing because it doesn't modify the blocks

# Conclusion (low priority)

The main takeaways? 
It is possible to extract associations from WFC.
WFC might not be the best suited algorithm for our goals, but there is a starting point, one which we can talk about in the future work.


Finally we conclude by summarize the technical and evaluation details which are listed below. 

    As a guide for this section, we will first lay down a high level overview, which is as follows: 
    a brief explanation of the two WFC models,
    - an deeper explanation of WFC's pattern generation,
    - a small aside to explain how we generate tilemaps with WFC through creating pixel bitmap representations,
    - an explanation of the rule extraction and tile extraction,
    - an explanation of using those newly extracted rules for generation and how the rules based version of WFC infers rules.

In this paper we present, to the best of our knowledge, a novel method of rule extraction through using WFC. 
We show how these extracted rules compare to original rules and the expected outcomes of using these extracted rules, we also show how other more accessible rule generation approaches do not seem to give the same kind of output, as with the rules generated by large language models.
Finally we evaluate the work done, showing why there are additional rules within the model, how WFC will infer rules...

