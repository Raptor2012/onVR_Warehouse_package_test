using System;
using UnityEngine;
using UnityEngine.Events;
using OnVR.Warehouse.GrabInteraction.Core;

namespace OnVR.Warehouse.GrabInteraction
{
    /// <summary>
    /// Core component that makes any GameObject grabbable across different platforms
    /// </summary>
    [AddComponentMenu("onVR/Grab Interaction/Grabbable Object")]
    public class GrabbableObject : MonoBehaviour
    {
        [Header("Grabbable Settings")]
        [SerializeField] private bool _isGrabbable = true;
        [SerializeField] private InteractionPlatform _supportedPlatforms = InteractionPlatform.All;
        [SerializeField] private GrabInteractionType[] _allowedInteractionTypes = new GrabInteractionType[0];

        [Header("Visual Feedback")]
        [SerializeField] private Material _highlightMaterial;
        [SerializeField] private Color _highlightColor = Color.yellow;
        [SerializeField] private bool _useOutlineEffect = true;

        [Header("Physics Settings")]
        [SerializeField] private bool _usePhysics = true;
        [SerializeField] private bool _freezeRotationWhenGrabbed = false;
        [SerializeField] private float _grabForce = 100f;

        [Header("Distance Settings")]
        [SerializeField] private float _maxGrabDistance = 10f;
        [SerializeField] private bool _enableRemoteGrab = true;

        [Header("Events")]
        public UnityEvent OnGrabStart;
        public UnityEvent OnGrabEnd;
        public UnityEvent OnHighlight;
        public UnityEvent OnUnhighlight;

        // Private fields
        private GrabState _currentState = GrabState.Idle;
        private Renderer _renderer;
        private Material _originalMaterial;
        private Rigidbody _rigidbody;
        private bool _wasKinematic;
        private Transform _grabber;

        // Properties
        public bool IsGrabbable 
        { 
            get => _isGrabbable; 
            set => _isGrabbable = value; 
        }

        public GrabState CurrentState => _currentState;
        public Transform CurrentGrabber => _grabber;

        // Events
        public event Action<GrabbableObject> GrabStarted;
        public event Action<GrabbableObject> GrabEnded;
        public event Action<GrabbableObject> Highlighted;
        public event Action<GrabbableObject> Unhighlighted;

        private void Awake()
        {
            InitializeComponents();
            SetupDefaultInteractionTypes();
        }

        private void Start()
        {
            // Register with the interaction manager
            InteractionManager.Instance.RegisterGrabbableObject(this);
        }

        private void OnDestroy()
        {
            // Unregister from the interaction manager
            if (InteractionManager.Instance != null)
            {
                InteractionManager.Instance.UnregisterGrabbableObject(this);
            }
        }

        private void InitializeComponents()
        {
            _renderer = GetComponentInChildren<Renderer>();
            _rigidbody = GetComponent<Rigidbody>();

            if (_renderer != null)
            {
                _originalMaterial = _renderer.material;
            }

            // Add rigidbody if not present and physics is enabled
            if (_usePhysics && _rigidbody == null)
            {
                _rigidbody = gameObject.AddComponent<Rigidbody>();
            }

            // Ensure collider exists
            if (GetComponent<Collider>() == null && GetComponentInChildren<Collider>() == null)
            {
                var boxCollider = gameObject.AddComponent<BoxCollider>();
                Debug.LogWarning($"GrabbableObject '{name}' had no collider. Added BoxCollider automatically.");
            }
        }

        private void SetupDefaultInteractionTypes()
        {
            if (_allowedInteractionTypes.Length == 0)
            {
                _allowedInteractionTypes = PlatformDetector.GetPreferredInteractionTypes();
            }
        }

        /// <summary>
        /// Attempts to grab this object
        /// </summary>
        public bool TryGrab(Transform grabber, GrabInteractionType interactionType)
        {
            if (!CanBeGrabbed(interactionType))
                return false;

            StartGrab(grabber, interactionType);
            return true;
        }

        /// <summary>
        /// Releases the grab on this object
        /// </summary>
        public void Release()
        {
            if (_currentState != GrabState.Grabbed)
                return;

            EndGrab();
        }

        /// <summary>
        /// Highlights the object (e.g., when hovered)
        /// </summary>
        public void Highlight()
        {
            if (_currentState == GrabState.Highlighted || _currentState == GrabState.Grabbed)
                return;

            _currentState = GrabState.Highlighted;
            ApplyHighlightEffect();
            
            OnHighlight?.Invoke();
            Highlighted?.Invoke(this);
        }

        /// <summary>
        /// Removes highlight from the object
        /// </summary>
        public void Unhighlight()
        {
            if (_currentState != GrabState.Highlighted)
                return;

            _currentState = GrabState.Idle;
            RemoveHighlightEffect();
            
            OnUnhighlight?.Invoke();
            Unhighlighted?.Invoke(this);
        }

        /// <summary>
        /// Checks if this object can be grabbed with the specified interaction type
        /// </summary>
        public bool CanBeGrabbed(GrabInteractionType interactionType)
        {
            if (!_isGrabbable)
                return false;

            if (!_supportedPlatforms.HasFlag(PlatformDetector.CurrentPlatform))
                return false;

            if (_allowedInteractionTypes.Length > 0)
            {
                bool typeAllowed = false;
                foreach (var allowedType in _allowedInteractionTypes)
                {
                    if (allowedType == interactionType)
                    {
                        typeAllowed = true;
                        break;
                    }
                }
                if (!typeAllowed)
                    return false;
            }

            return _currentState != GrabState.Grabbed;
        }

        private void StartGrab(Transform grabber, GrabInteractionType interactionType)
        {
            _currentState = GrabState.Grabbed;
            _grabber = grabber;

            // Handle physics
            if (_rigidbody != null)
            {
                _wasKinematic = _rigidbody.isKinematic;
                if (interactionType == GrabInteractionType.DirectHand)
                {
                    _rigidbody.isKinematic = true;
                }
                
                if (_freezeRotationWhenGrabbed)
                {
                    _rigidbody.freezeRotation = true;
                }
            }

            RemoveHighlightEffect();
            
            OnGrabStart?.Invoke();
            GrabStarted?.Invoke(this);
        }

        private void EndGrab()
        {
            _currentState = GrabState.Released;
            var previousGrabber = _grabber;
            _grabber = null;

            // Restore physics
            if (_rigidbody != null)
            {
                _rigidbody.isKinematic = _wasKinematic;
                if (_freezeRotationWhenGrabbed)
                {
                    _rigidbody.freezeRotation = false;
                }
            }

            OnGrabEnd?.Invoke();
            GrabEnded?.Invoke(this);

            // Return to idle state after a frame
            Invoke(nameof(ReturnToIdle), 0.1f);
        }

        private void ReturnToIdle()
        {
            _currentState = GrabState.Idle;
        }

        private void ApplyHighlightEffect()
        {
            if (_renderer == null)
                return;

            if (_highlightMaterial != null)
            {
                _renderer.material = _highlightMaterial;
            }
            else if (_useOutlineEffect)
            {
                // Simple color tint for highlight
                var material = _renderer.material;
                material.color = _highlightColor;
            }
        }

        private void RemoveHighlightEffect()
        {
            if (_renderer == null || _originalMaterial == null)
                return;

            _renderer.material = _originalMaterial;
        }

        /// <summary>
        /// Gets the distance from a point to this grabbable object
        /// </summary>
        public float GetDistanceFromPoint(Vector3 point)
        {
            var collider = GetComponent<Collider>() ?? GetComponentInChildren<Collider>();
            if (collider != null)
            {
                return Vector3.Distance(point, collider.ClosestPoint(point));
            }
            return Vector3.Distance(point, transform.position);
        }

        /// <summary>
        /// Checks if this object is within grab range of a point
        /// </summary>
        public bool IsWithinGrabRange(Vector3 point)
        {
            return GetDistanceFromPoint(point) <= _maxGrabDistance;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            // Draw grab range
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, _maxGrabDistance);
        }
#endif
    }
}
