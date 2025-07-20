# Basic Grab Interaction Sample

This sample demonstrates the basic functionality of the onVR Grab Interaction system.

## Contents

- **SampleScene.unity**: A basic scene showing grabbable objects
- **Prefabs/**: Reusable grabbable object prefabs
- **Materials/**: Visual feedback materials
- **Scripts/**: Sample integration scripts

## Setup

1. Open the SampleScene
2. The scene includes an InteractionManager and several grabbable objects
3. Test interaction based on your platform:
   - **VR**: Use hand controllers for direct grab or remote grab
   - **Mobile**: Tap objects or use gaze-pick
   - **Web/Desktop**: Click and drag objects

## Objects in Scene

- **BasicCube**: Simple grabbable cube with physics
- **HighlightedSphere**: Sphere with custom highlight material
- **RemoteGrabCylinder**: Cylinder optimized for remote grabbing
- **ArticyIntegratedObject**: Object with Articy integration

## Customization

Feel free to modify the objects and settings to understand how the system works. Use the Setup Wizard (onVR > Grab Interaction > Setup Wizard) to configure the system for your specific needs.
