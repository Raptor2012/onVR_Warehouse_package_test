using System;

namespace OnVR.Warehouse.GrabInteraction.Core
{
    /// <summary>
    /// Supported platforms for grab interaction
    /// </summary>
    [Flags]
    public enum InteractionPlatform
    {
        None = 0,
        VR = 1 << 0,
        Mobile = 1 << 1,
        Web = 1 << 2,
        Desktop = 1 << 3,
        All = VR | Mobile | Web | Desktop
    }

    /// <summary>
    /// Types of grab interactions available
    /// </summary>
    public enum GrabInteractionType
    {
        DirectHand,      // VR direct hand interaction
        RemoteGrab,      // VR remote grabbing
        TapClick,        // Mobile/Web tap or click
        GazePick,        // Gaze-based selection
        Drag             // Drag interaction for mobile/web
    }

    /// <summary>
    /// Current state of a grabbable object
    /// </summary>
    public enum GrabState
    {
        Idle,
        Highlighted,
        Grabbed,
        Released
    }
}
