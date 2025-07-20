using System.Collections.Generic;
using UnityEngine;
using OnVR.Warehouse.GrabInteraction.Core;
using OnVR.Warehouse.GrabInteraction.Platforms;

namespace OnVR.Warehouse.GrabInteraction
{
    /// <summary>
    /// Central manager that handles all grab interactions across different platforms
    /// </summary>
    [AddComponentMenu("onVR/Grab Interaction/Interaction Manager")]
    public class InteractionManager : MonoBehaviour
    {
        [Header("Manager Settings")]
        [SerializeField] private bool _autoInitialize = true;
        [SerializeField] private bool _debugMode = false;

        [Header("Platform Handlers")]
        [SerializeField] private VRInteractionHandler _vrHandler;
        [SerializeField] private MobileInteractionHandler _mobileHandler;
        [SerializeField] private WebInteractionHandler _webHandler;

        // Singleton instance
        private static InteractionManager _instance;
        public static InteractionManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<InteractionManager>();
                    if (_instance == null)
                    {
                        var go = new GameObject("InteractionManager");
                        _instance = go.AddComponent<InteractionManager>();
                    }
                }
                return _instance;
            }
        }

        // Registered objects and handlers
        private readonly List<GrabbableObject> _registeredObjects = new List<GrabbableObject>();
        private readonly Dictionary<InteractionPlatform, IGrabInteractionHandler> _handlers = new Dictionary<InteractionPlatform, IGrabInteractionHandler>();

        // Current state
        private IGrabInteractionHandler _activeHandler;
        private InteractionPlatform _currentPlatform;

        private void Awake()
        {
            // Ensure singleton
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);

            if (_autoInitialize)
            {
                Initialize();
            }
        }

        private void Start()
        {
            // Detect platform and set up handlers
            _currentPlatform = PlatformDetector.CurrentPlatform;
            SetupPlatformHandlers();
            ActivateHandler();
        }

        /// <summary>
        /// Manually initialize the interaction manager
        /// </summary>
        public void Initialize()
        {
            if (_debugMode)
                Debug.Log($"InteractionManager: Initializing for platform {PlatformDetector.CurrentPlatform}");

            _currentPlatform = PlatformDetector.CurrentPlatform;
            SetupPlatformHandlers();
            ActivateHandler();
        }

        /// <summary>
        /// Register a grabbable object with the manager
        /// </summary>
        public void RegisterGrabbableObject(GrabbableObject grabbable)
        {
            if (!_registeredObjects.Contains(grabbable))
            {
                _registeredObjects.Add(grabbable);
                
                // Notify active handler
                _activeHandler?.OnObjectRegistered(grabbable);

                if (_debugMode)
                    Debug.Log($"InteractionManager: Registered {grabbable.name}");
            }
        }

        /// <summary>
        /// Unregister a grabbable object from the manager
        /// </summary>
        public void UnregisterGrabbableObject(GrabbableObject grabbable)
        {
            if (_registeredObjects.Contains(grabbable))
            {
                _registeredObjects.Remove(grabbable);
                
                // Notify active handler
                _activeHandler?.OnObjectUnregistered(grabbable);

                if (_debugMode)
                    Debug.Log($"InteractionManager: Unregistered {grabbable.name}");
            }
        }

        /// <summary>
        /// Get all registered grabbable objects
        /// </summary>
        public List<GrabbableObject> GetRegisteredObjects()
        {
            return new List<GrabbableObject>(_registeredObjects);
        }

        /// <summary>
        /// Get grabbable objects within range of a position
        /// </summary>
        public List<GrabbableObject> GetGrabbableObjectsInRange(Vector3 position, float range)
        {
            var objectsInRange = new List<GrabbableObject>();
            
            foreach (var obj in _registeredObjects)
            {
                if (obj != null && obj.IsWithinGrabRange(position) && obj.GetDistanceFromPoint(position) <= range)
                {
                    objectsInRange.Add(obj);
                }
            }

            return objectsInRange;
        }

        /// <summary>
        /// Find the closest grabbable object to a position
        /// </summary>
        public GrabbableObject FindClosestGrabbableObject(Vector3 position, float maxRange = float.MaxValue)
        {
            GrabbableObject closest = null;
            float closestDistance = maxRange;

            foreach (var obj in _registeredObjects)
            {
                if (obj == null || !obj.IsGrabbable)
                    continue;

                float distance = obj.GetDistanceFromPoint(position);
                if (distance < closestDistance)
                {
                    closest = obj;
                    closestDistance = distance;
                }
            }

            return closest;
        }

        /// <summary>
        /// Switch to a different platform handler (useful for testing)
        /// </summary>
        public void SwitchPlatform(InteractionPlatform platform)
        {
            if (_handlers.ContainsKey(platform))
            {
                _activeHandler?.Disable();
                _currentPlatform = platform;
                ActivateHandler();
            }
        }

        private void SetupPlatformHandlers()
        {
            // Clear existing handlers
            _handlers.Clear();

            // Setup VR handler
            if (_vrHandler != null)
            {
                _vrHandler.Initialize(this);
                _handlers[InteractionPlatform.VR] = _vrHandler;
            }
            else if (_currentPlatform == InteractionPlatform.VR)
            {
                _vrHandler = gameObject.AddComponent<VRInteractionHandler>();
                _vrHandler.Initialize(this);
                _handlers[InteractionPlatform.VR] = _vrHandler;
            }

            // Setup Mobile handler
            if (_mobileHandler != null)
            {
                _mobileHandler.Initialize(this);
                _handlers[InteractionPlatform.Mobile] = _mobileHandler;
            }
            else if (_currentPlatform == InteractionPlatform.Mobile)
            {
                _mobileHandler = gameObject.AddComponent<MobileInteractionHandler>();
                _mobileHandler.Initialize(this);
                _handlers[InteractionPlatform.Mobile] = _mobileHandler;
            }

            // Setup Web handler
            if (_webHandler != null)
            {
                _webHandler.Initialize(this);
                _handlers[InteractionPlatform.Web] = _webHandler;
            }
            else if (_currentPlatform == InteractionPlatform.Web)
            {
                _webHandler = gameObject.AddComponent<WebInteractionHandler>();
                _webHandler.Initialize(this);
                _handlers[InteractionPlatform.Web] = _webHandler;
            }

            // Desktop uses web handler
            if (_currentPlatform == InteractionPlatform.Desktop && _handlers.ContainsKey(InteractionPlatform.Web))
            {
                _handlers[InteractionPlatform.Desktop] = _handlers[InteractionPlatform.Web];
            }
        }

        private void ActivateHandler()
        {
            // Disable current handler
            _activeHandler?.Disable();

            // Activate new handler
            if (_handlers.ContainsKey(_currentPlatform))
            {
                _activeHandler = _handlers[_currentPlatform];
                _activeHandler.Enable();

                if (_debugMode)
                    Debug.Log($"InteractionManager: Activated {_currentPlatform} handler");
            }
            else
            {
                Debug.LogWarning($"InteractionManager: No handler found for platform {_currentPlatform}");
            }
        }

        private void Update()
        {
            _activeHandler?.UpdateHandler();
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            // Auto-assign handlers if not set
            if (_vrHandler == null)
                _vrHandler = GetComponent<VRInteractionHandler>();
            if (_mobileHandler == null)
                _mobileHandler = GetComponent<MobileInteractionHandler>();
            if (_webHandler == null)
                _webHandler = GetComponent<WebInteractionHandler>();
        }
#endif
    }
}
