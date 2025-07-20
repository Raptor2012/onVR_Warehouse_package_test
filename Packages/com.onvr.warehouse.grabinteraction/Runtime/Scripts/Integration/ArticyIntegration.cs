using UnityEngine;
using OnVR.Warehouse.GrabInteraction.Core;
using System;
using Articy.Unity;

namespace OnVR.Warehouse.GrabInteraction.Integration
{
    /// <summary>
    /// Integration component for Articy flow templates and location-based interactions
    /// </summary>
    [AddComponentMenu("onVR/Grab Interaction/Articy Integration")]
    public class ArticyIntegration : MonoBehaviour
    {
            [Header("Articy Template")]
    [SerializeField] public ArticyRef LinkWithTemplate;
    [SerializeField] private string _locationId;
    [SerializeField] private bool _useArticyGrabbableProperty = true;
    [SerializeField] private bool _overrideGrabbableFromArticy = false;
        
        [Header("Flow Integration")]
        [SerializeField] private bool _triggerFlowOnGrab = false;
        [SerializeField] private ArticyRef _grabFlowFragment;
        [SerializeField] private bool _triggerFlowOnRelease = false;
        [SerializeField] private ArticyRef _releaseFlowFragment;
        
        [Header("Articy Flow Player")]
        [SerializeField] private ArticyFlowPlayer _flowPlayer;

        // Events for Articy integration
        public event Action<ArticyRef> ArticyFlowTriggered;
        public event Action<string, bool> ArticyPropertyChanged;
        
        // Flow player access
        public ArticyFlowPlayer FlowPlayer => _flowPlayer;

        // Private fields
        private GrabbableObject _grabbableObject;
        private bool _initialGrabbableState;

        private void Awake()
        {
            _grabbableObject = GetComponent<GrabbableObject>();
            if (_grabbableObject == null)
            {
                Debug.LogError($"ArticyIntegration on {name} requires a GrabbableObject component!");
                return;
            }

            _initialGrabbableState = _grabbableObject.IsGrabbable;
        }

        private void Start()
        {
            // Find or create ArticyFlowPlayer if not assigned
            if (_flowPlayer == null)
            {
                _flowPlayer = FindObjectOfType<ArticyFlowPlayer>();
                if (_flowPlayer == null)
                {
                    var flowPlayerGO = new GameObject("ArticyFlowPlayer");
                    _flowPlayer = flowPlayerGO.AddComponent<ArticyFlowPlayer>();
                }
            }

            // Subscribe to grabbable events
            if (_grabbableObject != null)
            {
                _grabbableObject.GrabStarted += OnObjectGrabbed;
                _grabbableObject.GrabEnded += OnObjectReleased;
            }

            // Initialize from Articy if enabled
            if (_useArticyGrabbableProperty)
            {
                InitializeFromArticy();
            }
        }

        private void OnDestroy()
        {
            // Unsubscribe from events
            if (_grabbableObject != null)
            {
                _grabbableObject.GrabStarted -= OnObjectGrabbed;
                _grabbableObject.GrabEnded -= OnObjectReleased;
            }
        }

        /// <summary>
        /// Initialize grabbable state from Articy template
        /// </summary>
        private void InitializeFromArticy()
        {
            if (LinkWithTemplate != null && LinkWithTemplate.GetObject() != null)
            {
                Debug.Log($"Initializing from Articy template: {LinkWithTemplate}");
                var template = LinkWithTemplate.GetObject();
                if (template != null)
                {
                    Debug.Log($"Template found: {template}");
                    
                    // Try to get grabbable property from template
                    bool articyGrabbable = GetArticyGrabbableProperty();
                    if (_overrideGrabbableFromArticy)
                    {
                        _grabbableObject.IsGrabbable = articyGrabbable;
                    }
                }
            }
        }

        /// <summary>
        /// Get the grabbable property from Articy template
        /// </summary>
        /// <returns>True if object should be grabbable according to Articy</returns>
        private bool GetArticyGrabbableProperty()
        {
            try
            {
                if (LinkWithTemplate != null && LinkWithTemplate.GetObject() != null)
                {
                    var template = LinkWithTemplate.GetObject();
                    if (template != null)
                    {
                        // Try to access grabbable property through reflection or template methods
                        // This is a placeholder until the specific template structure is known
                        Debug.Log($"Template type: {template.GetType().Name}");
                        
                        // For now, return the initial state
                        // TODO: Implement specific template property access when template structure is defined
                        return _initialGrabbableState;
                    }
                }
                
                // Fallback to initial state if template is not available
                return _initialGrabbableState;
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Failed to get Articy grabbable property: {ex.Message}");
                return _initialGrabbableState;
            }
        }

        /// <summary>
        /// Set the grabbable property in Articy template
        /// </summary>
        private void SetArticyGrabbableProperty(bool value)
        {
            try
            {
                if (LinkWithTemplate != null && LinkWithTemplate.GetObject() != null)
                {
                    var template = LinkWithTemplate.GetObject();
                    if (template != null)
                    {
                        // Try to set grabbable property through reflection or template methods
                        // This is a placeholder until the specific template structure is known
                        Debug.Log($"Setting grabbable property to {value} for template: {template.GetType().Name}");
                        
                        // TODO: Implement specific template property setting when template structure is defined
                        ArticyPropertyChanged?.Invoke("boolGrabbable", value);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Failed to set Articy grabbable property: {ex.Message}");
            }
        }

        /// <summary>
        /// Trigger an Articy flow fragment using the proper Articy flow player
        /// </summary>
        public void TriggerArticyFlow(ArticyRef flowFragment)
        {
            if (flowFragment == null || flowFragment.GetObject() == null) return;

            try
            {
                if (_flowPlayer != null)
                {
                    // Set the starting point and play the flow
                    _flowPlayer.StartOn = flowFragment.GetObject();
                    _flowPlayer.Play();
                    Debug.Log($"Triggering Articy flow: {flowFragment}");
                    ArticyFlowTriggered?.Invoke(flowFragment);
                }
                else
                {
                    Debug.LogWarning("No ArticyFlowPlayer found to trigger flow");
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Failed to trigger Articy flow '{flowFragment}': {ex.Message}");
            }
        }


        /// <summary>
        /// Called when the object is grabbed
        /// </summary>
        private void OnObjectGrabbed(GrabbableObject grabbedObject)
        {
            if (_triggerFlowOnGrab && _grabFlowFragment != null)
            {
                TriggerArticyFlow(_grabFlowFragment);
            }
        }

        /// <summary>
        /// Called when the object is released
        /// </summary>
        private void OnObjectReleased(GrabbableObject releasedObject)
        {
            if (_triggerFlowOnRelease && _releaseFlowFragment != null)
            {
                TriggerArticyFlow(_releaseFlowFragment);
            }
        }

        /// <summary>
        /// Manually set the grabbable state and sync with Articy
        /// </summary>
        public void SetGrabbable(bool grabbable)
        {
            if (_grabbableObject != null)
            {
                _grabbableObject.IsGrabbable = grabbable;
            }

            if (_useArticyGrabbableProperty)
            {
                SetArticyGrabbableProperty(grabbable);
            }
        }

        /// <summary>
        /// Update the object from Articy properties (call this when Articy data changes)
        /// </summary>
        public void RefreshFromArticy()
        {
            if (_useArticyGrabbableProperty)
            {
                InitializeFromArticy();
            }
        }

        /// <summary>
        /// Set the Articy template reference (useful for runtime assignment)
        /// </summary>
        public void SetArticyTemplate(ArticyRef templateRef)
        {
            LinkWithTemplate = templateRef;
            RefreshFromArticy();
        }

        /// <summary>
        /// Set the location ID (useful for location-based interactions)
        /// </summary>
        public void SetLocationId(string locationId)
        {
            _locationId = locationId;
        }

        /// <summary>
        /// Get the current Articy template reference
        /// </summary>
        public ArticyRef GetArticyTemplate()
        {
            return LinkWithTemplate;
        }

        /// <summary>
        /// Get the current location ID
        /// </summary>
        public string GetLocationId()
        {
            return _locationId;
        }

        /// <summary>
        /// Check if this object belongs to a specific location
        /// </summary>
        public bool IsInLocation(string locationId)
        {
            return !string.IsNullOrEmpty(_locationId) && _locationId.Equals(locationId, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Get the Articy template as a specific type
        /// </summary>
        public T GetTemplate<T>() where T : ArticyObject
        {
            if (LinkWithTemplate != null && LinkWithTemplate.GetObject() != null)
            {
                return LinkWithTemplate.GetObject<T>();
            }
            return null;
        }

        /// <summary>
        /// Check if the template has a specific feature
        /// </summary>
        public bool HasFeature<T>() where T : class
        {
            var template = GetTemplate<ArticyObject>();
            if (template != null)
            {
                // This would need to be implemented based on your specific template structure
                // For now, we'll check if the feature exists by trying to access it through reflection
                try
                {
                    var featureProperty = template.GetType().GetProperty(typeof(T).Name);
                    return featureProperty != null;
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// Manually trigger a specific flow fragment
        /// </summary>
        public void PlayFlow(ArticyRef flowFragment)
        {
            TriggerArticyFlow(flowFragment);
        }

        /// <summary>
        /// Get a feature from the template
        /// </summary>
        public T GetFeature<T>() where T : class
        {
            var template = GetTemplate<ArticyObject>();
            if (template != null)
            {
                try
                {
                    // Try to access the feature through reflection
                    var featureProperty = template.GetType().GetProperty(typeof(T).Name);
                    if (featureProperty != null)
                    {
                        return featureProperty.GetValue(template) as T;
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Failed to get feature {typeof(T).Name}: {ex.Message}");
                }
            }
            return null;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            // Ensure we have a GrabbableObject component
            if (_grabbableObject == null)
            {
                _grabbableObject = GetComponent<GrabbableObject>();
            }
        }
#endif
    }

    /// <summary>
    /// Helper class for Articy integration utilities
    /// </summary>
    public static class ArticyIntegrationUtilities
    {
        /// <summary>
        /// Find all objects in a location
        /// </summary>
        public static ArticyIntegration[] FindObjectsInLocation(string locationId)
        {
            var allArticyObjects = GameObject.FindObjectsOfType<ArticyIntegration>();
            var objectsInLocation = new System.Collections.Generic.List<ArticyIntegration>();

            foreach (var obj in allArticyObjects)
            {
                if (obj.IsInLocation(locationId))
                {
                    objectsInLocation.Add(obj);
                }
            }

            return objectsInLocation.ToArray();
        }

        /// <summary>
        /// Find all objects with a specific template type
        /// </summary>
        public static ArticyIntegration[] FindObjectsWithTemplate<T>() where T : ArticyObject
        {
            var allArticyObjects = GameObject.FindObjectsOfType<ArticyIntegration>();
            var objectsWithTemplate = new System.Collections.Generic.List<ArticyIntegration>();

            foreach (var obj in allArticyObjects)
            {
                if (obj.GetTemplate<T>() != null)
                {
                    objectsWithTemplate.Add(obj);
                }
            }

            return objectsWithTemplate.ToArray();
        }

        /// <summary>
        /// Set grabbable state for all objects in a location
        /// </summary>
        public static void SetLocationGrabbable(string locationId, bool grabbable)
        {
            var objectsInLocation = FindObjectsInLocation(locationId);
            foreach (var obj in objectsInLocation)
            {
                obj.SetGrabbable(grabbable);
            }
        }

        /// <summary>
        /// Set grabbable state for all objects with a specific template
        /// </summary>
        public static void SetTemplateGrabbable<T>(bool grabbable) where T : ArticyObject
        {
            var objectsWithTemplate = FindObjectsWithTemplate<T>();
            foreach (var obj in objectsWithTemplate)
            {
                obj.SetGrabbable(grabbable);
            }
        }

        /// <summary>
        /// Refresh all objects from Articy data
        /// </summary>
        public static void RefreshAllFromArticy()
        {
            var allArticyObjects = GameObject.FindObjectsOfType<ArticyIntegration>();
            foreach (var obj in allArticyObjects)
            {
                obj.RefreshFromArticy();
            }
        }

        /// <summary>
        /// Trigger a flow on all objects in a location
        /// </summary>
        public static void TriggerFlowInLocation(string locationId, ArticyRef flowFragment)
        {
            var objectsInLocation = FindObjectsInLocation(locationId);
            foreach (var obj in objectsInLocation)
            {
                if (obj.FlowPlayer != null && flowFragment != null)
                {
                    // Use the instance method to properly trigger the flow
                    try
                    {
                        // Delegate to the instance method which handles the proper flow triggering
                        obj.TriggerArticyFlow(flowFragment);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning($"Failed to trigger flow in location {locationId}: {ex.Message}");
                    }
                }
            }
        }
    }
}
