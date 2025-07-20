using System.Collections.Generic;
using UnityEngine;
using OnVR.Warehouse.GrabInteraction.Core;

namespace OnVR.Warehouse.GrabInteraction.Platforms
{
    /// <summary>
    /// Handles mobile-specific grab interactions including tap/click and gaze-pick
    /// </summary>
    [AddComponentMenu("onVR/Grab Interaction/Mobile Handler")]
    public class MobileInteractionHandler : MonoBehaviour, IGrabInteractionHandler
    {
        [Header("Mobile Settings")]
        [SerializeField] private bool _enableTapInteraction = true;
        [SerializeField] private bool _enableGazePick = true;
        [SerializeField] private bool _enableDragInteraction = true;
        [SerializeField] private LayerMask _interactionLayerMask = -1;

        [Header("Touch Settings")]
        [SerializeField] private float _tapTimeThreshold = 0.3f;
        [SerializeField] private float _dragThreshold = 10f;
        [SerializeField] private float _maxTouchDistance = 50f;

        [Header("Gaze Settings")]
        [SerializeField] private float _gazeTimeToSelect = 2f;
        [SerializeField] private float _gazeRayDistance = 100f;
        [SerializeField] private bool _showGazeReticle = true;
        [SerializeField] private GameObject _gazeReticlePrefab;

        // Interface implementation
        public bool IsEnabled { get; private set; }
        public InteractionPlatform SupportedPlatform => InteractionPlatform.Mobile;

        // Private fields
        private InteractionManager _manager;
        private Camera _camera;
        private GrabbableObject _currentGrabbedObject;
        private GrabbableObject _currentHighlightedObject;
        private GrabbableObject _currentGazedObject;
        
        // Touch tracking
        private Vector2 _touchStartPosition;
        private float _touchStartTime;
        private bool _isDragging;
        
        // Gaze tracking
        private float _gazeTimer;
        private GameObject _gazeReticle;

        public void Initialize(InteractionManager manager)
        {
            _manager = manager;
            SetupMobileComponents();
        }

        public void Enable()
        {
            IsEnabled = true;
            if (_gazeReticle != null)
                _gazeReticle.SetActive(_showGazeReticle);
        }

        public void Disable()
        {
            IsEnabled = false;
            ReleaseCurrentGrab();
            ClearHighlight();
            if (_gazeReticle != null)
                _gazeReticle.SetActive(false);
        }

        public void UpdateHandler()
        {
            if (!IsEnabled) return;

            if (_enableTapInteraction || _enableDragInteraction)
                UpdateTouchInteractions();
            
            if (_enableGazePick)
                UpdateGazeInteractions();
        }

        public void OnObjectRegistered(GrabbableObject grabbable)
        {
            // Mobile objects don't need special setup
        }

        public void OnObjectUnregistered(GrabbableObject grabbable)
        {
            if (_currentGrabbedObject == grabbable)
                _currentGrabbedObject = null;
            if (_currentHighlightedObject == grabbable)
                _currentHighlightedObject = null;
            if (_currentGazedObject == grabbable)
                _currentGazedObject = null;
        }

        private void SetupMobileComponents()
        {
            // Find main camera
            _camera = Camera.main;
            if (_camera == null)
            {
                _camera = FindObjectOfType<Camera>();
            }

            // Setup gaze reticle
            if (_enableGazePick && _showGazeReticle && _gazeReticlePrefab != null)
            {
                _gazeReticle = Instantiate(_gazeReticlePrefab);
                _gazeReticle.SetActive(false);
            }
        }

        private void UpdateTouchInteractions()
        {
#if UNITY_ANDROID || UNITY_IOS || UNITY_EDITOR
            // Handle touch input
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                HandleTouch(touch);
            }
#endif
            // Fallback to mouse for testing in editor
#if UNITY_EDITOR || UNITY_STANDALONE
            HandleMouseInput();
#endif
        }

        private void HandleTouch(Touch touch)
        {
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    HandleTouchStart(touch.position);
                    break;
                
                case TouchPhase.Moved:
                    HandleTouchMove(touch.position);
                    break;
                
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    HandleTouchEnd(touch.position);
                    break;
            }
        }

        private void HandleMouseInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                HandleTouchStart(Input.mousePosition);
            }
            else if (Input.GetMouseButton(0))
            {
                HandleTouchMove(Input.mousePosition);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                HandleTouchEnd(Input.mousePosition);
            }
        }

        private void HandleTouchStart(Vector2 screenPosition)
        {
            _touchStartPosition = screenPosition;
            _touchStartTime = Time.time;
            _isDragging = false;

            // Raycast to find grabbable object
            var grabbable = GetGrabbableAtScreenPosition(screenPosition);
            if (grabbable != null)
            {
                grabbable.Highlight();
                _currentHighlightedObject = grabbable;
            }
        }

        private void HandleTouchMove(Vector2 screenPosition)
        {
            if (!_isDragging && _enableDragInteraction)
            {
                float dragDistance = Vector2.Distance(screenPosition, _touchStartPosition);
                if (dragDistance > _dragThreshold)
                {
                    _isDragging = true;
                    
                    // Start drag interaction
                    if (_currentHighlightedObject != null && 
                        _currentHighlightedObject.CanBeGrabbed(GrabInteractionType.Drag))
                    {
                        if (_currentHighlightedObject.TryGrab(_camera.transform, GrabInteractionType.Drag))
                        {
                            _currentGrabbedObject = _currentHighlightedObject;
                        }
                    }
                }
            }

            // Update dragged object position
            if (_isDragging && _currentGrabbedObject != null)
            {
                UpdateDraggedObjectPosition(screenPosition);
            }
        }

        private void HandleTouchEnd(Vector2 screenPosition)
        {
            float touchDuration = Time.time - _touchStartTime;
            
            if (_isDragging)
            {
                // End drag interaction
                if (_currentGrabbedObject != null)
                {
                    _currentGrabbedObject.Release();
                    _currentGrabbedObject = null;
                }
            }
            else if (touchDuration <= _tapTimeThreshold && _enableTapInteraction)
            {
                // Handle tap interaction
                var grabbable = GetGrabbableAtScreenPosition(screenPosition);
                if (grabbable != null && grabbable.CanBeGrabbed(GrabInteractionType.TapClick))
                {
                    // Simple tap - grab and immediately release
                    if (grabbable.TryGrab(_camera.transform, GrabInteractionType.TapClick))
                    {
                        // For tap interaction, we might want to trigger an action rather than continuous grab
                        grabbable.Release();
                    }
                }
            }

            // Clear highlight
            ClearHighlight();
            _isDragging = false;
        }

        private void UpdateGazeInteractions()
        {
            if (_camera == null) return;

            // Raycast from center of screen
            Vector3 screenCenter = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0);
            Ray gazeRay = _camera.ScreenPointToRay(screenCenter);

            if (Physics.Raycast(gazeRay, out RaycastHit hit, _gazeRayDistance, _interactionLayerMask))
            {
                var grabbable = hit.collider.GetComponent<GrabbableObject>();
                
                if (grabbable != null && grabbable.CanBeGrabbed(GrabInteractionType.GazePick))
                {
                    if (_currentGazedObject != grabbable)
                    {
                        // New object gazed at
                        _currentGazedObject?.Unhighlight();
                        _currentGazedObject = grabbable;
                        _currentGazedObject.Highlight();
                        _gazeTimer = 0f;
                    }
                    else
                    {
                        // Continue gazing at same object
                        _gazeTimer += Time.deltaTime;
                        
                        if (_gazeTimer >= _gazeTimeToSelect)
                        {
                            // Gaze selection triggered
                            if (_currentGazedObject.TryGrab(_camera.transform, GrabInteractionType.GazePick))
                            {
                                // For gaze pick, immediately release (like a selection)
                                _currentGazedObject.Release();
                            }
                            _gazeTimer = 0f;
                        }
                    }

                    // Update gaze reticle
                    UpdateGazeReticle(hit.point, _gazeTimer / _gazeTimeToSelect);
                }
                else
                {
                    ClearGaze();
                }
            }
            else
            {
                ClearGaze();
            }
        }

        private GrabbableObject GetGrabbableAtScreenPosition(Vector2 screenPosition)
        {
            if (_camera == null) return null;

            Ray ray = _camera.ScreenPointToRay(screenPosition);
            if (Physics.Raycast(ray, out RaycastHit hit, _maxTouchDistance, _interactionLayerMask))
            {
                return hit.collider.GetComponent<GrabbableObject>();
            }

            return null;
        }

        private void UpdateDraggedObjectPosition(Vector2 screenPosition)
        {
            if (_currentGrabbedObject == null || _camera == null) return;

            // Convert screen position to world position
            Ray ray = _camera.ScreenPointToRay(screenPosition);
            float distanceToObject = Vector3.Distance(_camera.transform.position, _currentGrabbedObject.transform.position);
            
            Vector3 worldPosition = ray.GetPoint(distanceToObject);
            _currentGrabbedObject.transform.position = Vector3.Lerp(
                _currentGrabbedObject.transform.position,
                worldPosition,
                Time.deltaTime * 5f
            );
        }

        private void UpdateGazeReticle(Vector3 worldPosition, float progress)
        {
            if (_gazeReticle == null) return;

            _gazeReticle.SetActive(true);
            _gazeReticle.transform.position = worldPosition;
            _gazeReticle.transform.LookAt(_camera.transform);

            // Update reticle visual based on progress
            var renderer = _gazeReticle.GetComponent<Renderer>();
            if (renderer != null)
            {
                Color color = Color.Lerp(Color.white, Color.green, progress);
                renderer.material.color = color;
            }

            var scale = Mathf.Lerp(1f, 1.5f, progress);
            _gazeReticle.transform.localScale = Vector3.one * scale;
        }

        private void ClearHighlight()
        {
            if (_currentHighlightedObject != null)
            {
                _currentHighlightedObject.Unhighlight();
                _currentHighlightedObject = null;
            }
        }

        private void ClearGaze()
        {
            if (_currentGazedObject != null)
            {
                _currentGazedObject.Unhighlight();
                _currentGazedObject = null;
            }
            _gazeTimer = 0f;
            
            if (_gazeReticle != null)
                _gazeReticle.SetActive(false);
        }

        private void ReleaseCurrentGrab()
        {
            if (_currentGrabbedObject != null)
            {
                _currentGrabbedObject.Release();
                _currentGrabbedObject = null;
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (_camera != null)
            {
                // Draw gaze ray
                Gizmos.color = Color.green;
                Vector3 screenCenter = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0);
                Ray gazeRay = _camera.ScreenPointToRay(screenCenter);
                Gizmos.DrawRay(gazeRay.origin, gazeRay.direction * _gazeRayDistance);
            }
        }
#endif
    }
}
