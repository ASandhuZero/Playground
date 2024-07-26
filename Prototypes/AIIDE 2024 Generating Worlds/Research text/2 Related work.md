# Related Work (high priority)

Inspired by other techniques such as Model Synthesis by Merrel and Wave Function Collapse, the quantum physics phenomenon, WaveFunctionCollapse has been held a interest with the research community with its pattern associations and constraint solving.
The first to bring the attention of WFC's preculiar way it generates output was Karth and Smith.
It has also found much traction in commercial games (such as Bad North and Caves of Qud) and tools. 
Due to its popularity as a PCG technique, WFC has quite the expanse of research, such as PCGML, an approach that incorporates Machine Learning in PCG. 
Much of this is from WFC's use of statistical sampling and constraint solving (which as of this writing, it appears the canonical algorithm is Arc Consistency 4).
We also wish to take an aside and reference Merrel's Model Synthesis work, a key inspiration for WFC, and Merrel's insight on both algorithms has led us better understanding some of the generative issues WFC has.

As for what work has been done with WFC, starting first is Karth and Smith's work on showing the implicit Machine Learning aspects of WFC and how to add discriminant learning to the algorithm alongside a mixed-initiative approach to "train" the model through interaction.
They showcased the possibility of introducing negative constraints into WFC.
Other works with using WFC and ML 

Mention the VQ-VAE work somewhere

Other approaches have looked at adding additional design information and inference, such as Sturgeon--which focused on TODO:
Other avenues of this design focused thrust include Sandhu's work on adding additional design reasoning elements into a modified version of WFC, then continuing with the on at looking at larger structure generation for a within WFC.
Other approaches, such as Hierarchical WaveFunctionCollapse, look to add structure by including high-level patterns to the generation process.
Additionally other structural approaches, such as You Only Randomize Once [CITIE] looked at expanding WFC into a more generic constraint solver which can then leverage techniques such as pre-rolling. 


Other works also include creating a mixed-initiative tool with WFC at its core, such as Karth and Smith's work mentioned earlier, but also other works such MiWFC, another mixed-initiative system that focuses on WFC.
Additionally work has been done in terms of improving the search heuristic.(bigger resemeblance)
As stated earlier, WFC appears to be a testbed for PCG techniques and progress, but after surveying the field, we believe, to the best of our knowledge, there has not been any work done focusing on extracting neighbor rules through WFC in the method that we present in this paper.
