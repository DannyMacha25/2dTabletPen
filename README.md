# Introduction
2D Interface that allows for drawing on surfaces with mouse or tablet input. Currently features
rgb sliders for color change, a pen tool, eraser tool, and color picker tool.

# How To Setup
Clone the git into a project's assets folder. Open up sample scene for reference

You must have an instance of the *Input Handler* and the *UI* prefab in your scene.
The *Input Handler* has a few references you need to align.
- For `Color Input`, drag the object *RGB* from *UI* into it.
- For `Size Text`, drag the *Size Text* object from *UI/Pen Size*
- For `Tool Panels`, drag *Pen*, *Eraser*, and *Color Picker* from *UI/Tools* in that order.
There should be three elements in the list. Remember you can look at the `SampleScene` for reference.

To make a drawable surface, create an object with a meshrenderer. Place the `Whiteboard.cs` script 
on the object. Then place the *Whiteboard* tag on the object. The object is now a drawable surface!

# Controls
- WASD to move.
- Space/Shift to ascend/descend.
- left/right Brackets to change tool size.
- 1/2/3 or clicking on the tools will change your current tool.
- Mouse/Pen touch to draw on a surface.
- U to undo

## Panels
Drawable panels can be spawned with the f key. Once a panel is spawned, holding the left control allows you to interact with the panel. The following commands require the left control key to be held

- X to delete the panel
- Left/right to rotate the panel
- Mouse/Pen to move the panel

## Preparing a Model for Drawing
### Uv Projection
1. Download and open [Blender](https://www.blender.org/download/)
2. Open a new scene in Blender and delete everything in it.
3. Import your model into the scene
4. Select the model, go to the green triangle on the bottom right, go to 'UV Maps' and created a new UV map with the '+' button
5. Select the new UV Map
6. Go to the UV Editing window, click on the view with your model, click 'A' on your 
keyboard to select the entire model.
7. Go to the UV tab where it says "View, Select..." and go to "Smart UV Project"
8. Click "OK"

After that, a new UV has been created!
### Texture Baking
If you hate my text tutorial, try this [video](https://www.youtube.com/watch?v=VS4rqkqmg7Y&ab_channel=AverageGodotEnjoyer)!
1. Now go to the Shading window
2. In the bottom menu, right click , "Add" -> "Texture" -> "Image Texture"
3. On the Image Texture window, click New Texture
4. Name it your desired name and change the width and height to the width and height that matches the size of your model's other textures. Uncheck alpha if unecessary (if you're unsure, check it off)
5. Ctrl-C to copy that component
6. Open up the dropdown menu above the shader window (it should say Slot 1), and on every slot copy and paste that texture component into the material. Make sure the texture stays selected.
7. Go to "Render Properties" on the right (the little camera icon) then change the render engine to "Cycles"
8. Go to the Bake tab and change the type to "Diffuse"
9. Uncheck "Direct" and "Indirect" under "Contributions"
10. BAKE! (This might take a minute :3)
11. Go to the materials tab of the model
12. Delete all of the materials
13. Make a new material
14. In the Shading window, add another image texture
15. Select the new texture, then connect the color line to the base color of the material
16. Make sure the new UV map is enabled and export!

