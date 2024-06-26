Research Text
==========================================================================

Future work would be updating and repairing the rulesets but we can do that later

<RESEACH CONTINUE>
read this paper and gain any good insights about the type of transfer learning you are doing.
https://www.semanticscholar.org/reader/18d026ec5d0eebd17ee2c762da89540c0b3d7bde

So, for tomorrow:
1. figure out what type of transfer learning you're doing and plug into those evaluations 
2. if for some reason there aren't evaluations for the transfer learning you are attempting, note that. 
3. You need to pull out those associations from WFC and get them into a file that you can understand. 
4. After you generate that file, create an annotated mapping of the tileset you have. URGENT: 





What are we solving? What is the problem that we can point to and say "this is possible solution for this problem"
TODO: This requires us to read some research papers. I don't think this is an authorial burden paper... But I also don't think this is a new replayability paper either... 

We should acknowledge about using this is that we are creating a total system that uses WFC as it's generator.

# Abstract

Big thing is I am looking at a tilemap and employing the pixel learning that model synthesis has and is implemented in WFC. My contribution isn't the algorithm itself in somuch that is the exploration of the algorithm and then in its application to tilemap and tilemap constraint learning.


WaveFunctionCollapse, a Procedural Content Generation (PCG) technique that learns pixel associations from the given input to generate similar classes of output. Users, thus, bypass the tedious step of decomposing their domain knowledge of a design area into a form that a PCG system can use, but often the designer loses fine control over the generative space that rules give them–thus possibly leading to frustrations with the generative process or artifact. This paper explores automatic rule generation through WFC by exposing the pattern-based learning used by WFC to generate rules automatically. By generating rules, we hope this approach would ease the hard authorial task of crafting a bespoke rule set for WFC and address the authorial burden problem. To better understand the internal association generation, we will explore the algorithm within this paper, focusing mostly on the locality pattern reconciliation aspect of WFC, while also better understanding the inferred rules aspect of a rules-based version of WFC (that comes together with the learning variant). Focusing on these two aspects of the algorithm enables us to understand how to generate rules automatically and also test their validity. Finally, we supply a preliminary set of evaluation techniques to ensure that the generated rules capture the patterns within the input image, and can generate images through the rules that are similar to the original input image. By automatically generating these rules, we hope to better understand the first steps towards creating a mixed initiative level tool centered on WFC that both allows users to give a series of training images, but also allow for the flexibility of adding, removing, and editing the generated rules for more user control of the generative process.

# Introduction

Currently the argument that I should make is a mix of authorial burden, much like the argument made in the Hierarchical WaveFunctionCollapse paper and enabling new avenue of research and inquiry through having a method for transfer learning. [paper](https://ojs.aaai.org/index.php/AIIDE/article/view/27498/27271)  
But also I think we are making an argument that this allows us a new avenue of learning, much like the PCGML papers.
---




While this seems like a simple reskin, we looking at a way of making these rulesets more general, ensuring that different types of tilesets can use the same ruleset.
This is not a simple reskin, but instead an abstraction layer that allows for more reusability.

To do this, we look at adding a feature layer to the tilesets and tilemap, much like the work done in TileTerror. 
This feature map will be used to generate the ruleset. 
It should be mentioned that this feature map is effectively an extension of the symmetry rules (the features will be other important concepts such as if a tile is walkable, the type of tile, and other extensions of the symmetry rules.)
o
---> Move the below text to technical description because it does not seem to be useful here
To learn

Then we are using WFC to learn the tile associations that will then be used to generate a ruleset focused on the annotations themselves. 
After the association map has been generated from the original tilemap, that map is then used to create a new ruleset that is targeted to another annotated tileset.
The new ruleset is then given alongside the tileset back to WFC to generate a new tilemap that uses the original seed tilemap as it's design patterns.

Because designers often have internalized their process, we believe that by not having designers need to learn another tool, but instead feed the tilemaps into the system themselves, we create a more friendly tool that can help speed up the designers workflow.
<---

Thus, in this paper, we contribute a novel way to do transfer learning through learning rulesets to allow for both a more explainable AI (make sure that there is a citation for this argument) {CITE}.
We achieve this novel transfer learning through using WaveFunctionCollapse and its learning functionality, but also the other model it has.
Our hope is that this approach to transfer learning will help create new ways of designing with these generative AI techniques.


Can WFC generate tile spaces and tile representations in the same way that wfc learns pixel spaces. 

# Related Work

All of the WFC work that has come before and also some transfer learning because after all, we need to show some level of knowledge of the space... At least enough of it so the readers can assume that we know what transfer learning is.

1. Transfer Learning
2. WaveFunctionCollapse
3. Level generation approaches

# Technical Description

Don't forget that you run the simpletiled model first, then from there you run the overlapping code, that means that you are running both models. You then, after generating the ruleset from overlapping, you then run simpletiled once more and check the output.

This section focuses on how we implemented transfer learning through WFC.
checksum and check for collision?
How are you going to care for rotation and orientation
Figure out some hashing pixel function for rotation and orientation.

This is misc text, but you might want to mention that tiles themselves are pixel arrays and can be represented in that way... I don't think this is important, but it's worthwhile to mention so that way others can recreate our research if need be.

## Creating associations 

- Use WFC to generate 
Rather than trying to do some silly convolution, we should just do a tile indexing so that way it's faster for to implement, but this isn't a research task.
So the way that it can work is effectively turn the tileset into a pixel set, and if there are any conflicts, the sort out the conflicts before giving it to the overlapping model; that way we already know that there can't be any issues with the tilemap itself.

## Annotation mapping 
NOTE: Instead of saying annotations, we may want to use features as that gets us more in line with the rhetoric that ML people know.

Effectively in two halves, the first is the learning of rulesets and then the application of those rulesets.

NOTE:Something that a reviewer might ask about the goodness of the tileset itself, so make a mention about how we went about choosing our original tileset.

What questions should be answered?

1. Is this transfer learning?
2. If so, how is the learning evaluated in a way that is appropriate for transfer learning?
3. What do we gain from this research? How does transfer learning within WFC help us?
4. How do you *know* this works? Where's the proof? What's your methodology? What's your evaluation? 
    - Just as a reminder, you're going to have to prove that the output is both playable, just like every other WFC map out there, but also that your approach has generated a vaild set of rules. 
5. This seems like reusing the same rules with a different skin... How is this novel?
- This is actually such an important question to answer. Which I think that you answer by learning the annotations rather than the tile itself. 
- I think a good answer for this right now would be to say this, "while it seems that all that is occurring is a simple reskinning of tiles, it is not. We take the associations between patterns that WFC learns, remap them back to the original tileset, create an annotation map from the tileset and then use that *annotation* map as the partial ruleset for generation with the simpletiledmodel 
NOTE: Write down or create a diagram of the dummy data that you think you'll have. Look below for what I mean.

input into the system is a tilemap that has already been generated, that tilemap must have an annotation map as well, that way this process can work

tiles = {
    a : {
        symmetry : T,
        other information that matters
    }
}

generated ruleset = {
    corner_tile = {
        must also be : [walkable],
        allowed : [fire tile, power tile, puzzle tile, other types of tiles, walkable]
    }
}




1. Annotate a tileset with additional information (such as rotation and walkable information) 
2. Automagically generate annotated tilemaps for WFC 
3. Learn annotation association by feeding the tilemap into WFC
4. Use those annotated association to generate rulesets
5. Use the generated rulesets to generate tilemaps from a *different* tileset. 

# Evaluations

Creating an hybrid approach where you input two tilemaps and then have both of those maps contribute to the same ruleset, and then see what the output is for that new tilemap

Online corrections while generating, so if it becoming more dissimiliar, then you can adjust the probability of the generative space to get closer to being similar to the original tilemap input




One of your evaluations should be a similarity test between an input ruleset and then a generated ruleset and see how close they are two each other.

A possible evaluation will be to look at the tile frequencies between the original tilemap and the new ones. 
And by frequencies, I want to compare the frequency of certain types of tiles to see how often they show up in the learned version.
If we go down this approach, then the tileset must be simple enough to show this win, but also complex enough as well.

There is a two stage evaluation that needs to be done, the first is inspecting the level that is generated is all well and good.

1. We must ensure that the levels are playable, so have some A* agent ensuring that the level is playable because the original WaveFunctionCollapse work has no way of guaranteeing that the level that has been generated is playable.
2. Once a playable level has been generated, then do some comparison between the tilemaps and see if there are any similarities between the feature maps. The maps should be broken down into feature vectors because this makes sense to me?

Maybe do some kind of longest substring within the feature vectors so that way we can pull out the most probable patterns.

I think there is a really interesting idea about trying to discover the emergent patterns within WFC for design reasons. 
This reminds me of the partial work that you've done with TileTerror, but a little more robust as in the partials can be a learned thing
(What a great paper idea, learning larger patterns and using those patterns within the level generator)

(How are we going to compare two different feature vectors for similarities)

When it comes to evaluating levels, consider the whole complexity metric of dungeons through the dungeon and dragon metric where there is a line through the dungeon and each break from that line showcases more of a branching factor or something like that. 
Dungeon generation is a different paper, I think.

# Future work

While our original hope was using something like the Melan diagram to showcase that the dungeon is complex, but the key insight of the Melan diagram is that it shows complexity through a branching factor--one which we plan to use to showcase the complexity of our dungeon.
But for the sake of this paper, we are not doing any dungeon generation, but instead looking at a specific room.
We hope to take what we learned here and apply it to dungeon generation.

# Conclusion

So the take away for this paper is that we are attempting to see how much transfer learning that we can get out of this first attempt of transfer learning with WFC.

# Interesting ideas

The rules are written in a left right pattern, but because of the rotations, there most likely a new implicit left right pattern being generated, so green 0 and water 1 will also generate a water 1 and a green 0

Tiles are sparsely represented and give a high level explanation why the output ruleset
When the patterns are being generated through the overlapping, check to see if the pattern generated is also within the original ruleset. If it is not then you can show a new rule origin point and the context that generated it.

Look at the model synthesis work and see what it has to say about this.

There is an issue that should be obvious, which is tha the simple tiled model will take the rules that exist and create an up and down relationship betweeen these rules and then use those as well. So really, the patterns that we should be keeping track are just the left and right associations... But then the issue comes when the patterns are made, they look at the output and there most likely is a case that they are getting the up and down rules as well. 
So I guess we are also mining those as well and should explain how that stays within the artifact and we are learning the implicit left and right rules as well.


make sure to compare the output against some large language model, such as Gemini or chatgpt, to show that your rulesets contain more information from the original. I think this is going to be needed so that way I can go ahead and get that out of the bat. You can use the Max Kerminski work to show that you prompted it well enough. But Gemini already failed, so we can just get that test out of the way.

this paper seems most likely that it will be of use. https://repository.falmouth.ac.uk/2945/1/bare_conf.pdf

Also explain that there won't be a one to one mapping because the patterns cause some issues, but that is fine as you want to explore the original WFC code.

10x10 seem to have a high level of contradictions when there are a lot of pixels.

One of the more interesting arguments about WFC, maybe this comes in the future work or something like that, is that it's supposed to work with small example sizes with what appears to be a relatively small pattern size... But we want to increase the learning set, I think.
I am almost positive that you're going to have to figure out how to stitch the rules together so that way we can create more complex small NxN tilemaps

If we can use this system to detect patterns, then we can let the developer of the tilemap know that there are these frequent patterns in there tilemaps

Effectively this research project is working towards the idea of "autocomplete" for tilemap generation. 
As in there should be some tool or software that a tilemap designer can boot up and then feed in their own tilemaps to get suggestions on any partially designed maps.

Interestingly enough there seems to be something about tile symmetries done by a mathematician Wang. Maybe that could be used for an evaluation or looking at tilemap complexity?

With the feature set that we generate, it would be interesting to give the *weight* of the tiles themselves, that way the developer can see what tiles are most used in their tilemaps 

Any type of metric that is adherent to the original tileset and tilemap should be denoted that it can also be done on the transfer learnt map as well.

Question to answer, does WaveFunctionCollapse have the capacity to learn from *multiple* tilemaps? It should, honestly. 
