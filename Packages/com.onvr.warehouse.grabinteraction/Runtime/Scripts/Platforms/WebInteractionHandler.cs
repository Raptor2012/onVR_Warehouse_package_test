using UnityEngine;
using OnVR.Warehouse.GrabInteraction.Core;

namespace OnVR.Warehouse.GrabInteraction.Platforms
{
    /// <summary>
    /// Handles web-specific grab interactions including mouse click and gaze-pick
    /// </summary>
    [AddComponentMenu("onVR/Grab Interaction/Web Handler")]
    public class WebInteractionHandler : MonoBehaviour, IGrabInteractionHandler
    {
        [Header("Web Settings")]
        [SerializeField] private bool _enableClickInteraction = true;
        [SerializeField] private bool _enableGazePick = false; // Usually disabled on web
        [SerializeField] private bool _enableDragInteraction = true;
        [SerializeField] private LayerMask _interactionLayerMask = -1;

        [Header("Mouse Settings")]
        [SerializeField] private float _clickTimeThreshold = 0.3f;
        [SerializeField] private float _dragThreshold = 5f;
        [SerializeField] private float _maxClickDistance = 100f;

        [Header("Keyboard Shortcuts")]
        [SerializeField] private KeyCode _grabKey = KeyCode.G;
        [SerializeField] private KeyCode _releaseKey = KeyCode.R;

        // Interface implementation
        public bool IsEnabled { get; private set; }
        public InteractionPlatform SupportedPlatform => InteractionPlatform.Web;

        // Private fields
        private InteractionManager _manager;
        private Camera _camera;
        private GrabbableObject _currentGrabbedObject;
        private GrabbableObject _currentHighlightedObject;
        
        // Mouse tracking
        private Vector3 _mouseStartPosition;
        private float _mouseDownTime;
        private bool _isDragging;
        private Vector3 _dragOffset;

        public void Initialize(InteractionManager manager)
        {
            _manager = manager;
            SetupWebComponents();
        }

        public void Enable()
        {
            IsEnabled = true;
        }

        public void Disable()
        {
            IsEnabled = false;
            ReleaseCurrentGrab();
            ClearHighlight();
        }

        public void UpdateHandler()
        {
            if (!IsEnabled) return;

            UpdateMouseInteractions();
            UpdateKeyboardShortcuts();
            UpdateObjectHighlighting();
        }

        public void OnObjectRegistered(GrabbableObject grabbable)
        {
            // Web objects don't need special setup
        }

        public void OnObjectUnregistered(GrabbableObject grabbable)
        {
            if (_currentGrabbedObject == grabbable)
                _currentGrabbedObject = null;
            if (_currentHighlightedObject == grabbable)
                _currentHighlightedObject = null;
        }

        private void SetupWebComponents()
        {
            // Find main camera
            _camera = Camera.main;
            if (_camera == null)
            {
                _camera = FindObjectOfType<Camera>();
            }
        }

        private void UpdateMouseInteractions()
        {
            if (Input.GetMouseButtonDown(0))
            {
                HandleMouseDown();
            }
            else if (Input.GetMouseButton(0))
            {
                HandleMouseHold();
            }
            else if (Input.GetMouseButtonUp(0))
            {
                HandleMouseUp();
            }
        }

        private void HandleMouseDown()
        {
            _mouseStartPosition = Input.mousePosition;
            _mouseDownTime = Time.time;
            _isDragging = false;

            // Raycast to find grabbable object
            var grabbable = GetGrabbableAtMousePosition();
            if (grabbable != null)
            {
                grabbable.Highlight();
                _currentHighlightedObject = grabbable;
            }
        }

        private void HandleMouseHold()
        {
            if (!_isDragging && _enableDragInteraction)
            {
                float dragDistance = Vector3.Distance(Input.mousePosition, _mouseStartPosition);
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
                            CalculateDragOffset();
                        }
                    }
                }
            }

            // Update dragged object position
            if (_isDragging && _currentGrabbedObject != null)
            {
                UpdateDraggedObjectPosition();
            }
        }

        private void HandleMouseUp()
        {
            float clickDuration = Time.time - _mouseDownTime;
            
            if (_isDragging)
            {
                // End drag interaction
                if (_currentGrabbedObject != null)
                {
                    _currentGrabbedObject.Release();
                    _currentGrabbedObject = null;
                }
            }
            else if (clickDuration <= _clickTimeThreshold && _enableClickInteraction)
            {
                // Handle click interaction
                var grabbable = GetGrabbableAtMousePosition();
                if (grabbable != null && grabbable.CanBeGrabbed(GrabInteractionType.TapClick))
                {
                    // Simple click - grab and immediately release (like selection)
                    if (grabbable.TryGrab(_camera.transform, GrabInteractionType.TapClick))
                    {
                        grabbable.Release();
                    }
                }
            }

            // Clear highlight
            ClearHighlight();
            _isDragging = false;
        }

        private void UpdateKeyboardShortcuts()
        {
            // Manual grab with keyboard
            if (Input.GetKeyDown(_grabKey))
            {
                if (_currentGrabbedObject == null)
                {
                    var grabbable = GetGrabbableAtMousePosition();
                    if (grabbable != null && grabbable.CanBeGrabbed(GrabInteractionType.TapClick))
                    {
                        if (grabbable.TryGrab(_camera.transform, GrabInteractionType.TapClick))
                        {
                            _currentGrabbedObject = grabbable;
                            CalculateDragOffset();
                        }
                    }
                }
            }

            // Manual release with keyboard
            if (Input.GetKeyDown(_releaseKey))
            {
                if (_currentGrabbedObject != null)
                {
                    _currentGrabbedObject.Release();
                    _currentGrabbedObject = null;
                }
            }

            // Update grabbed object position when using keyboard control
            if (_currentGrabbedObject != null && !_isDragging)
            {
                UpdateDraggedObjectPosition();
            }
        }

        private void UpdateObjectHighlighting()
        {
            // Update highlighting for object under mouse cursor
            if (!_isDragging && _currentGrabbedObject == null)
            {
                var grabbable = GetGrabbableAtMousePosition();
                
                if (grabbable != _currentHighlightedObject)
                {
                    ClearHighlight();
                    
                    if (grabbable != null)
                    {
                        grabbable.Highlight();
                        _currentHighlightedObject = grabbable;
                    }
                }
            }
        }

        private GrabbableObject GetGrabbableAtMousePosition()
        {
            if (_camera == null) return null;

            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, _maxClickDistance, _interactionLayerMask))
            {
                return hit.collider.GetComponent<GrabbableObject>();
            }

            return null;
        }

        private void CalculateDragOffset()
        {
            if (_currentGrabbedObject == null || _camera == null) return;

            // Calculate offset between mouse ray and object position
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            Vector3 objectScreenPos = _camera.WorldToScreenPoint(_currentGrabbedObject.transform.position);
            Vector3 mouseWorldPos = ray.GetPoint(Vector3.Distance(_camera.transform.position, _currentGrabbedObject.transform.position));
            
            _dragOffset = _currentGrabbedObject.transform.position - mouseWorldPos;
        }

        private void UpdateDraggedObjectPosition()
        {
            if (_currentGrabbedObject == null || _camera == null) return;

            // Convert mouse position to world position
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            float distanceToObject = Vector3.Distance(_camera.transform.position, _currentGrabbedObject.transform.position);
            
            Vector3 worldPosition = ray.GetPoint(distanceToObject) + _dragOffset;
            _currentGrabbedObject.transform.position = Vector3.Lerp(
                _currentGrabbedObject.transform.position,
                worldPosition,
                Time.deltaTime * 8f
            );
        }

        private void ClearHighlight()
        {
            if (_currentHighlightedObject != null)
            {
                _currentHighlightedObject.Unhighlight();
                _currentHighlightedObject = null;
            }
        }

        private void ReleaseCurrentGrab()
        {
            if (_currentGrabbedObject != null)
            {
                _currentGrabbedObject.Release();
                _currentGrabbedObject = null;
            }
        }

        // Web-specific helper methods
        public void SetCursor(Texture2D cursorTexture, Vector2 hotspot)
        {
            Cursor.SetCursor(cursorTexture, hotspot, CursorMode.Auto);
        }

        public void ResetCursor()
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (_camera != null)
            {
                // Draw mouse ray
                Gizmos.color = Color.blue;
                Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
                Gizmos.DrawRay(ray.origin, ray.direction * _maxClickDistance);
            }
        }
#endif
    }
}
