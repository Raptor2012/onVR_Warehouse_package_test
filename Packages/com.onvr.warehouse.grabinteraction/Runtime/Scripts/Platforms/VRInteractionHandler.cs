using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using OnVR.Warehouse.GrabInteraction.Core;

namespace OnVR.Warehouse.GrabInteraction.Platforms
{
    /// <summary>
    /// Handles VR-specific grab interactions including direct hand and remote grab
    /// </summary>
    [AddComponentMenu("onVR/Grab Interaction/VR Handler")]
    public class VRInteractionHandler : MonoBehaviour, IGrabInteractionHandler
    {
        [Header("VR Settings")]
        [SerializeField] private bool _enableDirectGrab = true;
        [SerializeField] private bool _enableRemoteGrab = true;
        [SerializeField] private float _remoteGrabRange = 10f;
        [SerializeField] private LayerMask _grabLayerMask = -1;

        [Header("Haptic Feedback")]
        [SerializeField] private bool _enableHaptics = true;
        [SerializeField] private float _grabHapticIntensity = 0.5f;
        [SerializeField] private float _grabHapticDuration = 0.1f;

        [Header("Remote Grab Visual")]
        [SerializeField] private LineRenderer _remoteGrabLine;
        [SerializeField] private Material _remoteGrabLineMaterial;
        [SerializeField] private Color _remoteGrabLineColor = Color.blue;

        // Interface implementation
        public bool IsEnabled { get; private set; }
        public InteractionPlatform SupportedPlatform => InteractionPlatform.VR;

        // Private fields
        private InteractionManager _manager;
        private readonly Dictionary<UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor, GrabbableObject> _activeGrabs = new Dictionary<UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor, GrabbableObject>();
        private UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor[] _interactors;
        private Camera _vrCamera;

        public void Initialize(InteractionManager manager)
        {
            _manager = manager;
            SetupVRComponents();
        }

        public void Enable()
        {
            IsEnabled = true;
            if (_remoteGrabLine != null)
                _remoteGrabLine.enabled = false;
        }

        public void Disable()
        {
            IsEnabled = false;
            ReleaseAllGrabs();
            if (_remoteGrabLine != null)
                _remoteGrabLine.enabled = false;
        }

        public void UpdateHandler()
        {
            if (!IsEnabled) return;

            UpdateDirectGrabInteractions();
            if (_enableRemoteGrab)
                UpdateRemoteGrabInteractions();
        }

        public void OnObjectRegistered(GrabbableObject grabbable)
        {
            SetupGrabbableForVR(grabbable);
        }

        public void OnObjectUnregistered(GrabbableObject grabbable)
        {
            // Clean up any active grabs
            var grabsToRemove = new List<UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor>();
            foreach (var kvp in _activeGrabs)
            {
                if (kvp.Value == grabbable)
                {
                    grabsToRemove.Add(kvp.Key);
                }
            }

            foreach (var interactor in grabsToRemove)
            {
                _activeGrabs.Remove(interactor);
            }
        }

        private void SetupVRComponents()
        {
            // Find VR camera
            _vrCamera = Camera.main;
            if (_vrCamera == null)
            {
                var vrCameras = FindObjectsOfType<Camera>();
                foreach (var cam in vrCameras)
                {
                    if (cam.gameObject.name.ToLower().Contains("vr") || 
                        cam.gameObject.name.ToLower().Contains("head") ||
                        cam.gameObject.name.ToLower().Contains("main"))
                    {
                        _vrCamera = cam;
                        break;
                    }
                }
            }

            // Find XR interactors
            _interactors = FindObjectsOfType<UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor>();

            // Setup remote grab line if not assigned
            if (_remoteGrabLine == null && _enableRemoteGrab)
            {
                SetupRemoteGrabLine();
            }
        }

        private void SetupRemoteGrabLine()
        {
            var lineObject = new GameObject("RemoteGrabLine");
            lineObject.transform.SetParent(transform);
            _remoteGrabLine = lineObject.AddComponent<LineRenderer>();
            
            if (_remoteGrabLineMaterial != null)
                _remoteGrabLine.material = _remoteGrabLineMaterial;
            
            _remoteGrabLine.endColor = _remoteGrabLineColor;
            _remoteGrabLine.startWidth = 0.01f;
            _remoteGrabLine.endWidth = 0.01f;
            _remoteGrabLine.enabled = false;
        }

        private void UpdateDirectGrabInteractions()
        {
            if (!_enableDirectGrab || _interactors == null) return;

            foreach (var interactor in _interactors)
            {
                if (interactor == null) continue;

                // Check for new grabs
                if (interactor.isSelectActive && !_activeGrabs.ContainsKey(interactor))
                {
                    var target = GetGrabbableAtInteractor(interactor);
                    if (target != null && target.CanBeGrabbed(GrabInteractionType.DirectHand))
                    {
                        if (target.TryGrab(interactor.transform, GrabInteractionType.DirectHand))
                        {
                            _activeGrabs[interactor] = target;
                            PlayHapticFeedback(interactor);
                        }
                    }
                }
                // Check for grab releases
                else if (!interactor.isSelectActive && _activeGrabs.ContainsKey(interactor))
                {
                    var grabbedObject = _activeGrabs[interactor];
                    grabbedObject.Release();
                    _activeGrabs.Remove(interactor);
                }
                // Update grab position
                else if (_activeGrabs.ContainsKey(interactor))
                {
                    var grabbedObject = _activeGrabs[interactor];
                    UpdateGrabbedObjectPosition(grabbedObject, interactor.transform);
                }
            }
        }

        private void UpdateRemoteGrabInteractions()
        {
            // Simple ray-based remote grab implementation
            foreach (var interactor in _interactors)
            {
                if (interactor == null || _activeGrabs.ContainsKey(interactor)) continue;

                // Cast ray for remote objects
                if (Physics.Raycast(interactor.transform.position, interactor.transform.forward, 
                    out RaycastHit hit, _remoteGrabRange, _grabLayerMask))
                {
                    var grabbable = hit.collider.GetComponent<GrabbableObject>();
                    if (grabbable != null && grabbable.CanBeGrabbed(GrabInteractionType.RemoteGrab))
                    {
                        // Show remote grab line
                        if (_remoteGrabLine != null)
                        {
                            _remoteGrabLine.enabled = true;
                            _remoteGrabLine.SetPosition(0, interactor.transform.position);
                            _remoteGrabLine.SetPosition(1, hit.point);
                        }

                        grabbable.Highlight();

                        // Check for grab input
                        if (interactor.isSelectActive)
                        {
                            if (grabbable.TryGrab(interactor.transform, GrabInteractionType.RemoteGrab))
                            {
                                _activeGrabs[interactor] = grabbable;
                                PlayHapticFeedback(interactor);
                                if (_remoteGrabLine != null)
                                    _remoteGrabLine.enabled = false;
                            }
                        }
                    }
                }
                else
                {
                    // Hide remote grab line
                    if (_remoteGrabLine != null && _remoteGrabLine.enabled)
                    {
                        _remoteGrabLine.enabled = false;
                    }
                }
            }
        }

        private GrabbableObject GetGrabbableAtInteractor(UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor interactor)
        {
            // Check direct collision with interactor
            var colliders = Physics.OverlapSphere(interactor.transform.position, 0.1f, _grabLayerMask);
            foreach (var col in colliders)
            {
                var grabbable = col.GetComponent<GrabbableObject>();
                if (grabbable != null)
                    return grabbable;
            }

            // Check selected interactable
            if (interactor.firstInteractableSelected != null)
            {
                return interactor.firstInteractableSelected.transform.GetComponent<GrabbableObject>();
            }

            return null;
        }

        private void UpdateGrabbedObjectPosition(GrabbableObject grabbedObject, Transform grabberTransform)
        {
            if (grabbedObject == null || grabberTransform == null) return;

            // Simple position follow for grabbed objects
            grabbedObject.transform.position = Vector3.Lerp(
                grabbedObject.transform.position,
                grabberTransform.position,
                Time.deltaTime * 10f
            );
        }

        private void PlayHapticFeedback(UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor interactor)
        {
            if (!_enableHaptics) return;

            // Try to get haptic device
            var controller = interactor.GetComponent<XRBaseController>();
            if (controller != null)
            {
                controller.SendHapticImpulse(_grabHapticIntensity, _grabHapticDuration);
            }
        }

        private void SetupGrabbableForVR(GrabbableObject grabbable)
        {
            // Add XR Grab Interactable if not present
            if (grabbable.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>() == null)
            {
                var xrInteractable = grabbable.gameObject.AddComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
                
                // Configure for our system
                xrInteractable.selectMode = UnityEngine.XR.Interaction.Toolkit.Interactables.InteractableSelectMode.Single;
                xrInteractable.movementType = UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable.MovementType.VelocityTracking;
            }
        }

        private void ReleaseAllGrabs()
        {
            foreach (var kvp in _activeGrabs)
            {
                kvp.Value?.Release();
            }
            _activeGrabs.Clear();
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (_interactors != null)
            {
                Gizmos.color = Color.blue;
                foreach (var interactor in _interactors)
                {
                    if (interactor != null)
                    {
                        Gizmos.DrawRay(interactor.transform.position, 
                                     interactor.transform.forward * _remoteGrabRange);
                    }
                }
            }
        }
#endif
    }
}
