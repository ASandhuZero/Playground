# Introduction


Designing and creating assets for games is often a time consuming and intensive process for designers.
Often called the authorial burden, researchers have attempted to offload much of this burden to algorithmic approaches for generation, often called Procedural Content Generation (PCG), which often require the user to design rules or procedures for generating more game assets. 
Yet, just like with authorial burden, often with PCG, designers will be required to encode their design knowledge to get the algorithm to work. 
This can be a great effort as it will require much technical understanding for the user if they wish to express themselves in the algorithm, they must encode design knowledge into rigid data structures.

To address this issue, researchers have created GUI tools that let the designers work with an interface rather than the code itself.
These tools are called Mixed Initiative tools, which makes it easier for users to use PCG algorithms, at the cost of the designer losing some expressiveness with the algorithm, as there is now an interface they must use.
The issue arises, however, that often these mixed initiative tools still require some level of knowledge to use the tool, which can deter designers to other.
Easier GUIs and prompt-based approaches like DALLE or Chat-GPT become an easier to use alternative for the designers.
But the designers will often lose out on expressiveness prompt-based approaches as they cannot fine tune the model, like they can fine tune a rule set for a PCG system.

Thus we wish to address the tension between harder-to-use specified tools and techniques and the easier-to-use prompt based techniques by exploring automatic design knowledge extraction.
As for our contribution, we explore if it is possible to automatically generate rules with WFC, the outcomes of using an approach like WFC. 
To explore this space, we investigated WaveFunctionCollapse (WFC), an algorithm that has interested researchers due to its image generation through constraint solving and learning aspects. WFC contains two different models, one for learning constraints from images and another that requires encoded design information to generate new images. 
By inspecting both of these models, we discovered that it is possible to extract learned patterns and convert them into rules, which we then tested with through the rules-based WFC model. 
We also provide a way to evaluate the extracted rules through a testing generated rules against a ground truth rule set.  We also explore other avenues of quick generation pipelines such as using Google's Gemini and DALLE for both rule set generation and image generation. Finally, we test our approach with commercial tilemaps, and discuss where our technique succeeds and where our technique fails and what it fails.
