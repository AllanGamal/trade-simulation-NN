## Purpose

**Learning and Building Neural Networks**: To gain a deep understanding of how to build simple Convolutional Neural Networks (CNN) from scratch. 
**Simulation of Evolutionary Behavior**: To create a simulation where autonomous agents, called Bloblys, evolve their behavior over time through iterative processes involving genetic algorithms and neural networks. This allows us to observe how these creatures adapt and evolve in their environment. 
**GIF below takes a while to load**

![Blobly Simulation](images/simulation.gif)

# Project Overview: Blobly Simulation

This project is about simulating (in godot) blob-like creatures, "Bloblys", with neural networks. These Bloblys have various resources and skills and perform different actions to survive and reproduce. The project uses neural networks to determine the behavior of each Blobly and includes a system to visualize their performance over time using graphs.
The charts to the right shows the average, the average of the bottom 50% (red) and the top 50% (green).
For every generation there will be a "winner" with the highest fitness level. For the next round there will be a perfect copy of that winner (the big one), and the charts below the simulated world is the stats of the perfect copy of last round winner. 


## Survival Requirements

For Bloblys to survive, they must manage their resources and perform various actions in a specific sequence:

1. **Eating**: To eat, Bloblys need cooked fish.
2. **Cooking Fish**: To cook fish in the kitchen, Bloblys need raw fish and wood.
3. **Fishing**: To obtain raw fish, Bloblys need to fish at the lake, which requires fishing hooks.
4. **Crafting Fishing Hooks**: To make fishing hooks in the workshop, Bloblys need wood.
5. **Chopping Wood**: To get wood, Bloblys must chop trees into wood in the workshop.
6. **Chopping Trees**: To get trees, Bloblys must go to the lumberyard and chop trees.

For every round, every blobly only start with food and hunger to survive for 200 unites of time. If they run out of food and hunger goes to 0, they will die.


## Neural Networks

### Overview

The Blobly Simulation utilizes neural networks to dictate the behavior of Bloblys. Each Blobly is equipped with a neural network that processes inputs and outputs decisions that determine the Blobly's actions. These neural networks evolve over time through a genetic algorithm.


**Layers and Nodes**:The neural network consists of an input layer, one or more hidden layers, and an output layer. Each layer contains nodes connected to nodes in the next layer via weights.
**Activation Functions**: Each layer uses an activation function. I am using ReLU  for hidden layers and Softmax for the output layer.
**Weights and Biases**: Weights and biases are initialized randomly and adjusted through the genetic algorithm during the evolution process. Weights determine the strength of connections between nodes, while biases adjust the input to the activation function.
