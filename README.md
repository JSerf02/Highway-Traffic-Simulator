# Highway Traffic Simulator
Jacob Serfaty's final project for *Media Art and Design Practice*

# What is this?

This project is a simple Unity scene that simulates the flow of traffic on the highway. It works by spawning in cars continuously and having them drive with different behaviors.

The script `Car.cs` contains the `CarMode`interface that defines different behaviors cars can have and also the `Car` class which determines how cars choose these behaviors.

The script `CarSpawner.cs` spawns in cars.

The script `GameManager.cs` speeds up the game in the beginning so there isn't an absurdly long period without any cars.

All other scripts define specific car behaviors, such as driving normally, driving faster, breaking for no reason, and merging.

# How to use

1. Clone this repository
2. Open this folder in Unity Hub to import the project
   - Make sure the version of the project is 2022.2.3f1
3. Open the project in the Unity editor
