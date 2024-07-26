# Future work and Discussion (low priority)


Overall, our work done with rule extraction gave us the insight that we were looking for. 
WFC may not be the algorithm for our mixed-initiative algorithm. 
While WFC succeeds well on smaller pixel images and does well with rule sets and tile sets, we are unsure how to proceed forward with WFC when it comes to our rule extraction work. 
Our hesitation stems from the Simpletiled model's inferred rules, and how it is difficult for users to generate their own up/down pairings.
Additionally, while the inferred rules do increase the generative space, we wish to return to the earlier observation made on WFC failing more often as the requested image size increased. 

To remind the reader, the observation we made earlier is as requested output image size increases, WFC begins to fail more frequently by returning an unstable matrix, rather than a stable matrix that can be converted into an image. 
An unstable matrix will contain cells that have not been "observed" (a valid option has been chosen) or there are no possible choices, which indicates the generation process has failed. 
The failure rates increased as we reached 50x50 tile maps for the Summer tile set, which has been our best performing tile and rule set, with a near 100% generation percentage. 
As for why the failure rate begins to increase, Merrel describes WFC's inability to cope with larger image sizes as a result of WFC not modifying the generative space--which Merrel points out in his comparison work between WFC and Model Synthesis.
https://paulmerrell.org/wp-content/uploads/2021/07/comparison.pdf 
WFC's failure with larger image sizes is most likely due to the algorithm unable to modify the image that it is generating, thus learning to higher contradiction states. 

This--and the inability to explicitly state up/down neighbor pairs--leads us to the algorithmic drawing board, as we believe a hybrid algorithm that takes Model Synthesis's modification procedures and WFC's association learning would be the best next step for developing our mixed-initiative tool.
It is important because then it can repair where ever the model is failing, like what was happening constantly when generating larger images.
As for additions to our future algorithm, we would like to expand on the pixel downscaling and upscaling approach we used for generating images with the Overlapping model.
Our reasoning for this is in hopes that as we effectively "convolute" the image, upscaling even the pixel representation, we can capture pattern of patterns, effectively learning inferred hierarchy. 
If this approach fails, then we will be looking into incorporating other research that focuses on including design into WFC.
Another approach we are considering is inferring design knowledge from rules, akin to the work done by Cook. https://repository.falmouth.ac.uk/2945/1/bare_conf.pdf 





