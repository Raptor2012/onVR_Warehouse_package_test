# OnVR Warehouse Grab Interaction Package - Test Guide

## Quick Start Guide

### 1. Automatic Test Scene Setup

The easiest way to test the package is to use the automated test scene setup:

1. Create an empty GameObject in your scene
2. Add the `TestSceneSetup` component to it
3. The script will automatically create:
   - InteractionManager (singleton)
   - Multiple test cubes with GrabbableObject components
   - UI with instructions
   - Debug interface

### 2. Manual Setup

If you prefer to set up manually:

#### Step 1: Create the InteractionManager
- The InteractionManager is a singleton that auto-creates itself
- It will automatically detect your platform (VR, Mobile, Web, Desktop)
- It handles all interactions across different platforms

#### Step 2: Create a Grabbable Cube
1. Create a Cube (GameObject > 3D Object > Cube)
2. Add the `GrabbableObject` component
3. Ensure the cube has:
   - A Collider (automatically added by GrabbableObject if missing)
   - A Rigidbody (automatically added if physics is enabled)

#### Step 3: Configure Grabbable Settings
The GrabbableObject component has these key settings:

**Grabbable Settings:**
- `Is Grabbable`: Enable/disable grabbing
- `Supported Platforms`: Which platforms can interact (VR, Mobile, Web, Desktop)
- `Allowed Interaction Types`: Specific interaction types allowed

**Visual Feedback:**
- `Highlight Material`: Material to use when highlighted
- `Highlight Color`: Color tint when highlighted
- `Use Outline Effect`: Simple color-based highlighting

**Physics Settings:**
- `Use Physics`: Enable physics-based interaction
- `Freeze Rotation When Grabbed`: Lock rotation during grab
- `Grab Force`: Force applied during physics grabbing

**Distance Settings:**
- `Max Grab Distance`: Maximum distance for remote grabbing
- `Enable Remote Grab`: Allow grabbing from a distance

### 3. Interaction Types by Platform

**Desktop/Web (Mouse):**
- `TapClick`: Click to grab/release
- `Drag`: Click and drag to move objects
- `GazePick`: Look at objects to highlight (optional)

**Mobile (Touch):**
- `TapClick`: Tap to grab/release
- `Drag`: Touch and drag to move objects
- `GazePick`: Look at objects for auto-selection

**VR:**
- `DirectHand`: Direct controller/hand interaction
- `RemoteGrab`: Point and grab distant objects
- `GazePick`: Look-based selection

### 4. Events and Callbacks

The GrabbableObject provides both UnityEvents and C# events:

**UnityEvents (set in Inspector):**
- `OnGrabStart`: When object is grabbed
- `OnGrabEnd`: When object is released
- `OnHighlight`: When object is highlighted
- `OnUnhighlight`: When highlight is removed

**C# Events (subscribe in code):**
```csharp
grabbableObject.GrabStarted += OnGrabStarted;
grabbableObject.GrabEnded += OnGrabEnded;
grabbableObject.Highlighted += OnHighlighted;
grabbableObject.Unhighlighted += OnUnhighlighted;
```

### 5. Testing Your Setup

Use the debug UI components to test:

1. **TestGrabbableCube**: Add to any cube for automatic setup and logging
2. **GrabInteractionDebugUI**: Provides test buttons and state monitoring
3. **TestSceneSetup**: Complete automated test scene creation

### 6. Platform-Specific Instructions

**For Web/Desktop Testing:**
- Click on cubes to grab them
- Drag while holding mouse button to move
- Use keyboard shortcuts: G to grab, R to release
- Hover over objects to see highlight effects

**For Mobile Testing:**
- Tap on cubes to grab them
- Touch and drag to move objects
- Enable gaze-pick for hands-free interaction

**For VR Testing:**
- Use controller triggers to grab
- Point at distant objects for remote grab
- Gaze-based selection works with head movement

### 7. Customization

You can customize the interaction by:

1. **Modifying GrabbableObject settings** in the Inspector
2. **Creating custom interaction handlers** by implementing `IGrabInteractionHandler`
3. **Adding custom platform detection** by extending `PlatformDetector`
4. **Creating custom visual effects** for highlighting and feedback

### 8. Troubleshooting

**Common Issues:**

1. **Objects not grabbable:**
   - Check if GrabbableObject component is added
   - Verify IsGrabbable is true
   - Ensure supported platforms include current platform
   - Check if object has a collider

2. **No interaction response:**
   - Verify InteractionManager exists in scene
   - Check if correct platform handler is active
   - Look at console for debug messages

3. **Physics issues:**
   - Ensure Rigidbody is present if UsePhysics is enabled
   - Check mass and drag settings on Rigidbody
   - Verify collider settings

**Debug Tools:**
- Enable debug mode on InteractionManager
- Use the debug UI components
- Check console logs for interaction events
- Use Scene view to see grab range gizmos

### 9. Performance Considerations

- The system automatically registers/unregisters objects
- Platform detection is cached for performance
- Use appropriate interaction ranges to limit raycast checks
- Consider object pooling for many grabbable objects

### 10. Integration Notes

This package integrates with:
- Unity's XR system for VR detection
- Standard Unity Input system
- Unity Physics system
- Unity UI system for interface elements

The package is designed to work out-of-the-box with minimal setup while providing extensive customization options for advanced use cases.
