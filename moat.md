# Moat's Openbrush Experimental Additions:

## Freepaint tool enhancements:

### Lazy Input
* Applies linear-interpolative smoothing to your brush stroke input

1. You can toggle Lazy Input by pressing the Undo button on the Brush controller
1. The button will display a rabbit icon overlay when disabled, and a turtle when enabled.
1. Trigger pressure will affect the rate at which the brush "catches up" to your controller position and rotation

* Carving Mode will constrain brush movement to the Brush Controller's planar orientation.

1. Carving Mode can be toggled by pressing the same button while holding the brush trigger
1. Carving Mode will display an upside-down turtle icon on the button overlay when enabled.
1. Steer the brush's movement by turning your wrist during a brush stroke.

### Wand Stroke Guide
* Use the wand to guide the brushstroke for greater control to create combinations of straight and curved lines

1. Activate the Wand Stroke Guide by holding the Wand's trigger before beginning your brush stroke.
1. Your brushstrokes will be constrained to move directly towards the Wand.
1. Move the wand to control the stroke's direction.
1. Move your brush around the Wand Stroke Guideline to control the tilt of the brush.
1. The Wand Stroke Guide will automatically turn off when neither trigger is pressed.

* Create dashed lines by tapping trigger while guiding a brush stroke.
* Turning on Lazy Input will use the brush trigger to control the speed at which the brushstroke follows the Wand Stroke Guide
* Lazy Input mode will also keep your brush stroke as a single, unbroken line until you release the Wand Stroke Guide

* The Revolver is an additional guide feature that is a part of the Wand Stroke Guide.
* It will constrain the brush stroke to move in circular path that orbits the Wand Stroke Guide line.
* The Revolver is useful for creating circular arcs, circles, spirals, tunnels, tubes and other shapes with a circular cross-section.

1. Turn it on by activating the Wand Stroke Guide and then pressing the Brush's Undo Button (Same button used for turning on Lazy Input)
1. This same button will set the Revolver's Radius, which is defined by the distance between the Brush Controller and the Wand Stroke Guide Line

* You can change the revolver's radius at any time even during a brush stroke. 

* The Revolver can be made to spin automatically by holding the Brush's trigger and then moving the joystick during a brushstroke.
* The direction and rate of spin is dependent on the direction you move the joystick during a brushstroke.
* During a brushstroke, You can stop the auto-spin by touching the joystick while leaving it centered.

## Batch Filter

* The Batch Filter is useful for selecting or deleting overlapping brush strokes from different brush types.

1. Switch to either the eraser or selection tools.
1. Hold down the Wand Trigger
1. Hold down the Brush Trigger to activate the tool
1. Carefully hit a brushstroke with your selection/eraser tool
1. As long as you continue to hold down the Brush Trigger, only brushstrokes made with that same brush will be affected by that tool.
1. Releasing the Brush Trigger will make your selection/eraser tool behave normally again.