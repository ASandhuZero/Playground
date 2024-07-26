README.md
==========================================================================
## Random thoughts 

I think that there still needs to be some debugging layer or debugging logger so we can better understand what is going on within the functions.

You're still using WFC as a constraint solver, but you're building a system around it. 
This system that you're going to give to AIIDE isn't just WFC but also a larger system that has:


The big thing about your system is that you are moving away from just treating WFC as a constraint solver but instead treating the annotation mapping itself as the first class thing, you are creating a layer of abstraction through using annotations, which you can cite your own work about.

I also think that at some point this system could turn into a recommender system that is helping people generate their own tilemaps? But that would require a port of WFC into JS... which we have already done before... Maybe that's a summer project.
I think that is a future question, honestly. As in a future work that should be mentioned later on in the paper.

As in, the C# code should also have a learning model that takes in annotated tilesets, then annotates the 
corresponding tilemaps to create an annotation layer. 
That annotation layer should then be passed into WFC. 

# Features

- [ ] Input image file through gui
- [ ] Watch the generation process occur.
- [ ] Quick playable project

# Current TODO

I guess the easiest way would be to create convolution layer that will transpose the tilemap into a pixel bitmap that we can send through the overlapping model.

- [ ] Create a way for WFC to learn from a set of images.

- [ ] Figure out how the overlapping model generates the association
- [ ] Save these associations in some data format for later use
- [ ] Use the associations to create new rule sets that can then be fed into WFC

- [ ] At some point in the near future, refactor your code.
- [ ] Uh, json.lua shouldn't be at the root folder... But here we are.
- [ ] WFC only generates a graph, which it then gives back to the server.
- [ ] Love game decodes the incoming data and generates Love2D understandable tilemap data structure, then gives to Love2D.
- [ ] Love2D takes the tilemap data and renders a tilemap from the tilemap data.

## Data flow

TODO: make a data flow diagram.
Right now the idea is outlined below:

- A user will start a new instance of a love2d game.

- When the game is started, the game will request a tilemap from the lua server that the game exists on.

- The lua server will send a request to a C# server via TCP/IP; 

- The C# server will first look at the request to see if it valid.
If it is valid then parse the request to WFC

- WFC runs until it can find a solution for the graph problem, or times out

- The graph is sent back to the lua server. This graph is sent over in some encoding that both systems can understand.

- The lua server constructs the tilemap json that is finally given to love.

- Love2D renders the pipeline. 

## Backlog

TODO: Right now, you need to write out the research paper and methodologies you want to use to evaluate your generator. You clearly want a generator, but in order to publish it, you need to give yourself a roadmap and design what the generator is used for. 
You personally want to make a game which is all swell and 

- [ ] Use WFC to generate tilemap from a downloaded tilemap
it seems that you need to make your own tilemap, which should be fine. 
Make sure to add an annotation 

- [ ] Create a suite of testing software to make sure that everything is working or at least check to see if everything is working

- [ ] use Tiled representation so it's more generic and anyone who is familiar with tiled can then interface with the tool that you make.

- [ ] Sixlabors is a graphics engine within C#? (Lots of questions about that.) Regardless though, removing Sixlabors seems to be the idea as WFC shouldn't generate images--just output graphs. TODO: Remove Sixlabors?

## Completed tasks

- [x] C# TCPlistener works and is sending dummy data to other servers.
- [x] Lua server up and running, which sends data to the C# server.
- [x] Love2D game starts.

- [x] Figure out how to display tiles in love2D.
- [x] Once the game has started, the game should request a tilemap.
