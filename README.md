# Procedural content generation and custom mesh scripts
Included in this repo are the asset files and project settings of my procedural content projects in unity. Scripts and other asset files can be found under respective folders under assets. Most of these projects were made using untiy version 2022.3.43 (f1) and the built-in 3D rendering pipeline. To demo any of these projects, download unity 2022.3.43(f1) here https://unity.com/releases/editor/archive and import the assets and project settings.

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
  - Implements simple mesh deformation simulation by implementing a spring force to deform and return to the original shape of the cube sphere. The deformation force scales with size and can be tested in the game window by clicking the mesh with left mouse button.
