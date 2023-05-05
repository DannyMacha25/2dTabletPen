#Introduction
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