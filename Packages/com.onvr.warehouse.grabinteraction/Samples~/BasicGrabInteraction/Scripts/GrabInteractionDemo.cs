using UnityEngine;
using UnityEngine.UI;
using OnVR.Warehouse.GrabInteraction;
using OnVR.Warehouse.GrabInteraction.Core;

namespace OnVR.Warehouse.GrabInteraction.Samples
{
    /// <summary>
    /// Simple demo script that shows interaction events and platform information
    /// </summary>
    public class GrabInteractionDemo : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Text _platformLabel;
        [SerializeField] private Text _eventLog;
        [SerializeField] private int _maxLogEntries = 10;

        [Header("Demo Objects")]
        [SerializeField] private GrabbableObject[] _demoObjects;

        private System.Collections.Generic.List<string> _logEntries = new System.Collections.Generic.List<string>();

        private void Start()
        {
            SetupUI();
            SetupDemoObjects();
        }

        private void SetupUI()
        {
            if (_platformLabel != null)
            {
                _platformLabel.text = $"Platform: {PlatformDetector.CurrentPlatform}";
                
                var preferredTypes = PlatformDetector.GetPreferredInteractionTypes();
                _platformLabel.text += $"\nSupported: {string.Join(", ", preferredTypes)}";
            }

            LogEvent("Demo started");
        }

        private void SetupDemoObjects()
        {
            // Find all grabbable objects if not assigned
            if (_demoObjects == null || _demoObjects.Length == 0)
            {
                _demoObjects = FindObjectsOfType<GrabbableObject>();
            }

            // Subscribe to events
            foreach (var obj in _demoObjects)
            {
                if (obj != null)
                {
                    obj.GrabStarted += OnObjectGrabbed;
                    obj.GrabEnded += OnObjectReleased;
                    obj.Highlighted += OnObjectHighlighted;
                    obj.Unhighlighted += OnObjectUnhighlighted;
                }
            }
        }

        private void OnObjectGrabbed(GrabbableObject grabbedObject)
        {
            LogEvent($"GRABBED: {grabbedObject.name}");
        }

        private void OnObjectReleased(GrabbableObject releasedObject)
        {
            LogEvent($"RELEASED: {releasedObject.name}");
        }

        private void OnObjectHighlighted(GrabbableObject highlightedObject)
        {
            LogEvent($"HIGHLIGHTED: {highlightedObject.name}");
        }

        private void OnObjectUnhighlighted(GrabbableObject unhighlightedObject)
        {
            LogEvent($"UNHIGHLIGHTED: {unhighlightedObject.name}");
        }

        private void LogEvent(string message)
        {
            string timestampedMessage = $"[{System.DateTime.Now:HH:mm:ss}] {message}";
            _logEntries.Add(timestampedMessage);

            // Keep only the last N entries
            if (_logEntries.Count > _maxLogEntries)
            {
                _logEntries.RemoveAt(0);
            }

            // Update UI
            if (_eventLog != null)
            {
                _eventLog.text = string.Join("\n", _logEntries);
            }

            Debug.Log($"GrabDemo: {timestampedMessage}");
        }

        private void Update()
        {
            // Show some debug info
            if (Input.GetKeyDown(KeyCode.F1))
            {
                ShowDebugInfo();
            }

            if (Input.GetKeyDown(KeyCode.F2))
            {
                ClearLog();
            }
        }

        private void ShowDebugInfo()
        {
            LogEvent("=== DEBUG INFO ===");
            LogEvent($"Registered Objects: {InteractionManager.Instance.GetRegisteredObjects().Count}");
            LogEvent($"Platform: {PlatformDetector.CurrentPlatform}");
            
            var activeObjects = 0;
            foreach (var obj in _demoObjects)
            {
                if (obj != null && obj.IsGrabbable)
                    activeObjects++;
            }
            LogEvent($"Active Demo Objects: {activeObjects}");
        }

        private void ClearLog()
        {
            _logEntries.Clear();
            if (_eventLog != null)
            {
                _eventLog.text = "";
            }
        }

        private void OnDestroy()
        {
            // Unsubscribe from events
            foreach (var obj in _demoObjects)
            {
                if (obj != null)
                {
                    obj.GrabStarted -= OnObjectGrabbed;
                    obj.GrabEnded -= OnObjectReleased;
                    obj.Highlighted -= OnObjectHighlighted;
                    obj.Unhighlighted -= OnObjectUnhighlighted;
                }
            }
        }

        // Public methods for UI buttons
        public void ToggleDemoObject(int index)
        {
            if (index >= 0 && index < _demoObjects.Length && _demoObjects[index] != null)
            {
                var obj = _demoObjects[index];
                obj.IsGrabbable = !obj.IsGrabbable;
                LogEvent($"Toggled {obj.name}: {(obj.IsGrabbable ? "Grabbable" : "Not Grabbable")}");
            }
        }

        public void RefreshPlatformDetection()
        {
            PlatformDetector.RefreshPlatformDetection();
            SetupUI();
            LogEvent("Platform detection refreshed");
        }
    }
}
