using UnityEngine;
using UnityEngine.XR;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace OnVR.Warehouse.GrabInteraction.Core
{
    /// <summary>
    /// Utility class for detecting the current platform and available interaction methods
    /// </summary>
    public static class PlatformDetector
    {
        private static InteractionPlatform? _cachedPlatform;

        /// <summary>
        /// Gets the current platform based on runtime environment
        /// </summary>
        public static InteractionPlatform CurrentPlatform
        {
            get
            {
                if (_cachedPlatform.HasValue)
                    return _cachedPlatform.Value;

                _cachedPlatform = DetectPlatform();
                return _cachedPlatform.Value;
            }
        }

        /// <summary>
        /// Forces re-detection of the platform (useful for testing)
        /// </summary>
        public static void RefreshPlatformDetection()
        {
            _cachedPlatform = null;
        }

        /// <summary>
        /// Checks if the current platform supports a specific interaction type
        /// </summary>
        public static bool SupportsInteractionType(GrabInteractionType interactionType)
        {
            var platform = CurrentPlatform;

            return interactionType switch
            {
                GrabInteractionType.DirectHand => platform.HasFlag(InteractionPlatform.VR),
                GrabInteractionType.RemoteGrab => platform.HasFlag(InteractionPlatform.VR),
                GrabInteractionType.TapClick => platform.HasFlag(InteractionPlatform.Mobile) || 
                                               platform.HasFlag(InteractionPlatform.Web) || 
                                               platform.HasFlag(InteractionPlatform.Desktop),
                GrabInteractionType.GazePick => platform.HasFlag(InteractionPlatform.Mobile) || 
                                              platform.HasFlag(InteractionPlatform.Web) ||
                                              platform.HasFlag(InteractionPlatform.VR),
                GrabInteractionType.Drag => platform.HasFlag(InteractionPlatform.Mobile) || 
                                          platform.HasFlag(InteractionPlatform.Web),
                _ => false
            };
        }

        private static InteractionPlatform DetectPlatform()
        {
            // Check for VR first
            if (XRSettings.enabled && XRSettings.loadedDeviceName != "None")
            {
                return InteractionPlatform.VR;
            }

            // Check for mobile platforms
            if (Application.isMobilePlatform)
            {
                return InteractionPlatform.Mobile;
            }

            // Check for web platform
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                return InteractionPlatform.Web;
            }

            // In editor, try to detect target platform from build settings
            #if UNITY_EDITOR
            var buildTarget = UnityEditor.EditorUserBuildSettings.activeBuildTarget;
            switch (buildTarget)
            {
                case UnityEditor.BuildTarget.Android:
                case UnityEditor.BuildTarget.iOS:
                    return InteractionPlatform.Mobile;
                case UnityEditor.BuildTarget.WebGL:
                    return InteractionPlatform.Web;
                case UnityEditor.BuildTarget.StandaloneWindows:
                case UnityEditor.BuildTarget.StandaloneOSX:
                case UnityEditor.BuildTarget.StandaloneLinux64:
                    return InteractionPlatform.Desktop;
                default:
                    return InteractionPlatform.Desktop;
            }
            #else
            // Default to desktop
            return InteractionPlatform.Desktop;
            #endif
        }

        /// <summary>
        /// Gets the preferred interaction types for the current platform
        /// </summary>
        public static GrabInteractionType[] GetPreferredInteractionTypes()
        {
            return CurrentPlatform switch
            {
                InteractionPlatform.VR => new[] { GrabInteractionType.DirectHand, GrabInteractionType.RemoteGrab, GrabInteractionType.GazePick },
                InteractionPlatform.Mobile => new[] { GrabInteractionType.TapClick, GrabInteractionType.GazePick, GrabInteractionType.Drag },
                InteractionPlatform.Web => new[] { GrabInteractionType.TapClick, GrabInteractionType.GazePick, GrabInteractionType.Drag },
                InteractionPlatform.Desktop => new[] { GrabInteractionType.TapClick, GrabInteractionType.GazePick },
                _ => new[] { GrabInteractionType.TapClick }
            };
        }
    }
}
