# Procedural content generation and custom mesh scripts
Included in this repo are the asset files and project settings of my procedural content projects in unity. Scripts and other asset files can be found under respective folders under assets. Most of these projects were made using untiy version 2022.3.43 (f1) and the built-in 3D rendering pipeline. To demo any of these projects, download unity 2022.3.43(f1) [here](https://unity.com/releases/editor/archive) and import the assets and project settings.

Here is a quick overview of each project and its contents

## CubeSphere

This project was done following catlikecoding's mesh tutorial series. Each scene in this project implements a new different feature described below. Check out each scene to observe each feature.

1. Composite Cube Scene.
   - Create a custom cube using 6 different grid faces with loops custom setting the vertice positions, uv, and tangents.
2. Cube Sphere Scene
   - Custom cube sphere made by custom calculating vertex positions, normal vectors, and UV coordinates. Also includes optimizations like ensuring no duplicate vertices and splitting the mesh so it can be properly colored with different materials (more draw calls but worth it for fewer vertices here). 
3. Cube Scene
   - Creates a scene showcasing the creation of custom rounded cubes of different sizes and experiments with unity rigid body simulation.
4. Mesh Deformation Scene
   - Implements simple mesh deformation simulation by implementing a spring force to deform and return to the original shape of the cube sphere. The deformation force scales with size and can be tested in the game window by clicking the mesh with the left mouse button.
  
## Procedural Terrain Generator

In this project I create a procedural, tiling, terrain generator. The features of this project include:
- Custom terrain height generation using Perlin noise.
- Generates infinite tiles based on user position
- Optimizes terrain chunks by implementing frustum and distance culling.
- Uses random numbers to generate tree prefabs and randomly places them on the terrain surface.
To do on this project: fix terrain seam issues to share vertices and properly calculate normals. Add random height-based texturing to the surface.

## Procedural Torus Mesh

Creates custom torus mesh with customizable radius. also includes random seeds to color and procedurally generate donuts inside a spherical area using rejection sampling.

## Transformation Matrices

Custom creates rotation, scaling, and positon transformation functions using the homogenous coordinate system. Then later, also provides projection to 2D space using an orthographic camera and a perspective camera (with variable focal length).

## Procedural Creature Generator

This project was inspired by Will Wright's creature creator in his revolutionary game Spore (check out his presentation of it [here](https://www.youtube.com/watch?v=8PXiNNXUUF8)). In this project, I focused on creating high-quality curved surfaces and using those to randomly create different creature shapes using random numbers. The key features of this project include:

1. Creates Custom Catmullrom curve tubular mesh. This is done by manually defining control points, computing Frenet Frames based on these control points then using that to interpolate control points and create a mesh.
2. Uses random seeds to randomize creature size, leg count and size, wing prefab selection, and color.

## Procedural Flock

In this project I follow Craig Reynolds' behavior rules of interaction described [here](https://www.cs.toronto.edu/~dt/siggraph97-course/cwr87/) to create a procedurally generated flock simulation. For the flock, I make a simple prefab to represent the boids/flock agents. Then I used weighted forces to balance the following:

**Behaviors**
- Flock centering - agents head towards the average center position of neighbors
- Avoidance - use flock agent 2d collision radius to avoid collisions with each other
- Velocity matching - match the average direction and speed of neighboring flock members
- Wandering - randomness added to give more realistic motion
- Stay in the world - prevent from going outside world bounds.
