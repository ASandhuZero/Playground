# Conclusion (low priority)

The main takeaways? 
It is possible to extract associations from WFC.
WFC might not be the best suited algorithm for our goals, but there is a starting point, one which we can talk about in the future work.

In this paper we explored automatic rule generation and extraction through WaveFunctionCollapse.
Our procedure required WFC as it can both learn pixel associations and also generate with a rule set. 
By using WFC's Overlapping model, we developed a technique that extracted rules and tiles an input image that is given to the Overlapping mode.
The extraction process focused on mapping tiles to pixel representations, generating a downscaled pixel bitmap of a tilemap, using the pixel bitmap as input into the Overlapping mode where the model would then learn the associations between pixels.
Once the associations were generated, they would be iterated through once more, where the pixel values would then be used as the basis for a left/right neighbor pair, with the pixel values as the keys. 
Finally the pixel values would be remapped to their tile counterparts, thus leaving a gneerated left/right tile neighbor rule.
All pixel associations were processed through this method, creating a list of extracted rules.

To test if our extracted rule sets were valid, we devised a test by using the Simpletiled model and the Summer rule set to generate a new image that would then be used as input to the Overlapping model, where a generated rule set would be extracted.
We then compared the two rule sets against each other, counting up each time both sets shared a rule. 
The total count was then used to calculate a coverage score, where our rule set performed well, showcasing that it did capture the original rule set and thus inferred the design knowledge that those rules contained.
Overall our generated rule sets achieved high coverage percentages, even though additional rules were generated. 
We summerized these rules were the rules that the Simpletiled model infers when generating a new image.

To stress test our approach, we used different commercial game tile maps as input, the Alefgard overworld map from Dragon Quest IV and the Lake Hylia map from Legend of Zelda: A Link To The Past.
Our results show that without knowing about the tile sets ahead of time, with information such as their rotations and weights, then our approach will be not produce understandable tile maps due to the Simpletiled's inferred rules. 
Yet, when having the tile information present within the algorithm, the generated rules work well, which we show cased using the Summer tile set.
Overall, we believe our approach has promise for being integral in a future mixed initiative tool that has a WFC/Model Synthesis-like algorithm at its core.
