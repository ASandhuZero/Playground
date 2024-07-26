# Evaluations, Analysis, Discussion (high priority)

To evaluate how well our rule extraction approach worked, we did a set of tests, beginning with the most obvious--comparing generated rule set to designed rule sets--to more complex testing, such as attempting to extract rules from commercial tile maps.
We document our findings, problems that occur, and possible explanations for when our approach fails and how to make it succeed for generative tasks.
For our tests, we are lucky that the WFC repository contains example tile sets and rule sets. 
We used the Summer tile set and associated rule set to ensure reproducibility of our experiments.
As for why the Summer set was chosen for its top-down, 2D aesthetic, making it the example set with the most similarity to a tile set that would appear in a commercial game.

The first test we ran was focused on exploring if our approach generated rules that are good approximants of designed rules.
First an image is generated with the Simpletiled model, using the Summer rule set, ensuring that we had a "ground truth" rule set to compare against.
The newly generated image was then used as input to the Overlapping model. 
The rules were then extracted during the Overlapping model's execution, as described in the technical description.
The generated rule set was compared to the original Summer rule set. 
For every rule that existed in both rule sets, a count increased.
Once the rules have been compared, the total matching count was then used to calculate the percentage of rules that existed in both rule sets.
We named this percentage as "coverage" as the value indicated how well the generated rule set "covered" the rules within the original tile set.

For the Summer rule set that we tested on, after one run of a sufficiently large enough tilemap (40x40), the generated rules would often reach a 90% coverage rate.
To capture more of the original rules, we ran through the rule extraction process multiple times, saving the generated rules.
Then we compiled all of the generated rules, pruning to have only a unique set of rules. 
The compiled rule set was compared to the original rule set.
Currently, with this compiled approach, the Summer rule set, after six total runs of generating 40x40 tilemaps, the generated rule set achieves a 100% rule coverage rate.
We find this result reasonable that our approach does capture design rules, at least when working in small tile sets with readily available tile information, such as the tile's possible rotations and the tile's probablity. 
Figure N shows the output image.

We continued exploring our rule extraction approach with commercial tilemaps, in which there are no rule sets or tile sets provided. 
We chose tilemaps from both Dragon Quest IV{cite} and Legend Of Zelda A Link To The Past{cite}. 
From Dragon Quest IV, we chose Alefgard, the overworld map, and from A Link To The Past, two maps were chosen, Lake Hylia and Ice Island.
All three of these maps are far larger inputs than what we tested with beforehand.
Since there was no ground truth rule set we could use, our goal with this test was to explore if larger images could be generated, their rules extracted and used within the Simpletiled model. 
Our biggest concern was that without any tile rotation information, the generated rules 
The output was also quite garbled, tiles were being connected to others where this should not be the case.
 
We believe there are three possible causes for this garbled output, and those three are: tile rotations, Simpletiled inferred rules, and tile weights.
The first observation we made is that rotations can cause problems, as the overlapping model will not care for the rotations, the simpletiled model will. 
When we do constrain the Simpletiled model to only using the left/right pair and a single up/down inferred rule (this loosening of constraint was needed as the generation process kept failing otherwise), we noticed a strange tessellating pattern that emerged within the images. 
As an aside, currently there is no way to designate up/down rule pairs within WFC, leaving designers unable to explicitly state up/down rules.
These additional rules, we believe, cause the strange results that appear in figure X.
Yet, our best guess is that in order for our approach to work, the tile rotations must be known about beforehand, figure Z shows output of a summer tilemap generated with and without tile rotation information.
When ablating either the tile weights, the rotations, or editing the extracted rules, nothing achieved a better result than a tile set that contained designed information for each tile, as shown when using the Summer tile set information for generation with generated rules.

Another theory we have for why the extracted rules performed so poorly is due to the increase in extracted tiles.
The Summer tile set contains only 40 unique tiles, while our extracted tile sets were often in the hundreds.
Our best hypothesis is that would be possible for two identical grass tiles both had an additional tile object on top, say a player or some environmental prop, then these two tiles would be counted as distinct tiles, rather than matched to the already seen grass tile.
Due to this extreme increase in both generated tiles and generated rules we believe the coverage of these tilemaps were much lower but due to the Overlapping model's addition of strange artifacts.
We decided against using the generated images as more input into this system (such as what was done before to generate the total compiled Summer set).

Before concluding the evaluation and results section, we want to take a brief aside to focus on other generative techniques that we believe designers and developers may go to first before attempting to craft their own rules, which we believe are Large Language Models (LLMs).
We want to first inspect these approaches as they are some of the most accessible tools for designers who may not want to write their own rules and supply models such as copilot or Gemini the rule sets that come with WFC as prompts are far easier to use in comparison to generating rules.
As of writing this paper, our testing of these approaches do not give the results that WFC gives.

As for our procedure when generating the Gemini rule set, we prompted Gemini by supplying the Summer neighbor rules that we have been working with, requesting it to generate a similar set of neighbor rules, which it then generated. 
We took the rules and ran the Simpletiled model 20x20 size, hoping the size would not restrict the rule set so much. 
We then compare these rules against the ground truth rule that we used, hoping for a similarity score. 
As of writing this paper, we have not had a successful run with an unmodified rule set generated by Gemini.

As for the DALLE generated images, we supplied Copilot (which uses DALLE-3 at the time of this writing) a generated tile map image, requesting a similar output (all prompts are contained within the appendix).
While DALLE did generate tilemaps of the same color and theme, the generated images were not top-down tile maps but instead other types of game maps, such as an isometric tilemap or even table-top inspired. 
While we think the output of DALLE could be used in other ways, such as ideation tasks such as brainstorming, the images do not meet the requirement of generating a tilemap that resembles the input image in the same capacity as WFC. 
The DALLE images have been supplied in the appendix.

We admit now that these comparisons are not fair, as Gemini and Copilot are general models that aren't highly specialized in the specific task of tilemap generation, as WFC is.
Yet, we still include this exploration of other generative algorithms, as prompt-based generative tools are the tools that designers will most likely reach for before trying more specialized algorithms like WFC.

