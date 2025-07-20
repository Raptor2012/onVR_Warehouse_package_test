using UnityEngine;

namespace OnVR.Warehouse.GrabInteraction.Test
{
    /// <summary>
    /// Test script to demonstrate grabbable cube functionality
    /// This script shows how to set up and customize a grabbable object
    /// </summary>
    public class TestGrabbableCube : MonoBehaviour
    {
        [Header("Test Cube Settings")]
        [SerializeField] private Material highlightMaterial;
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color grabbedColor = Color.green;
        
        private GrabbableObject grabbableComponent;
        private Renderer cubeRenderer;
        private Material originalMaterial;

        private void Start()
        {
            SetupGrabbableCube();
            SetupEventListeners();
        }

        private void SetupGrabbableCube()
        {
            // Get or add the GrabbableObject component
            grabbableComponent = GetComponent<GrabbableObject>();
            if (grabbableComponent == null)
            {
                grabbableComponent = gameObject.AddComponent<GrabbableObject>();
            }

            // Get the renderer
            cubeRenderer = GetComponent<Renderer>();
            if (cubeRenderer != null)
            {
                originalMaterial = cubeRenderer.material;
                
                // Set initial color
                cubeRenderer.material.color = normalColor;
            }

            // Configure the grabbable settings
            grabbableComponent.IsGrabbable = true;
            
            Debug.Log($"Test Cube '{name}' is ready for interaction!");
            Debug.Log($"Current platform: {Core.PlatformDetector.CurrentPlatform}");
            Debug.Log($"Supported interaction types: {string.Join(", ", Core.PlatformDetector.GetPreferredInteractionTypes())}");
        }

        private void SetupEventListeners()
        {
            if (grabbableComponent == null) return;

            // Unity Events (set up in inspector)
            grabbableComponent.OnGrabStart.AddListener(() => 
            {
                Debug.Log($"Cube '{name}' was grabbed!");
                if (cubeRenderer != null)
                    cubeRenderer.material.color = grabbedColor;
            });

            grabbableComponent.OnGrabEnd.AddListener(() => 
            {
                Debug.Log($"Cube '{name}' was released!");
                if (cubeRenderer != null)
                    cubeRenderer.material.color = normalColor;
            });

            grabbableComponent.OnHighlight.AddListener(() => 
            {
                Debug.Log($"Cube '{name}' is highlighted!");
            });

            grabbableComponent.OnUnhighlight.AddListener(() => 
            {
                Debug.Log($"Cube '{name}' highlight removed!");
            });

            // C# Events (alternative way to listen to events)
            grabbableComponent.GrabStarted += OnGrabStarted;
            grabbableComponent.GrabEnded += OnGrabEnded;
            grabbableComponent.Highlighted += OnHighlighted;
            grabbableComponent.Unhighlighted += OnUnhighlighted;
        }

        private void OnGrabStarted(GrabbableObject grabbable)
        {
            Debug.Log($"C# Event: {grabbable.name} grab started by {grabbable.CurrentGrabber?.name}");
        }

        private void OnGrabEnded(GrabbableObject grabbable)
        {
            Debug.Log($"C# Event: {grabbable.name} grab ended");
        }

        private void OnHighlighted(GrabbableObject grabbable)
        {
            Debug.Log($"C# Event: {grabbable.name} highlighted");
        }

        private void OnUnhighlighted(GrabbableObject grabbable)
        {
            Debug.Log($"C# Event: {grabbable.name} unhighlighted");
        }

        private void OnDestroy()
        {
            // Clean up event listeners
            if (grabbableComponent != null)
            {
                grabbableComponent.GrabStarted -= OnGrabStarted;
                grabbableComponent.GrabEnded -= OnGrabEnded;
                grabbableComponent.Highlighted -= OnHighlighted;
                grabbableComponent.Unhighlighted -= OnUnhighlighted;
            }
        }

        // Test methods you can call from buttons or inspector
        [ContextMenu("Test Grab")]
        public void TestGrab()
        {
            if (grabbableComponent != null && grabbableComponent.CurrentState != Core.GrabState.Grabbed)
            {
                grabbableComponent.TryGrab(Camera.main.transform, Core.GrabInteractionType.TapClick);
            }
        }

        [ContextMenu("Test Release")]
        public void TestRelease()
        {
            if (grabbableComponent != null)
            {
                grabbableComponent.Release();
            }
        }

        [ContextMenu("Test Highlight")]
        public void TestHighlight()
        {
            if (grabbableComponent != null)
            {
                grabbableComponent.Highlight();
            }
        }

        [ContextMenu("Test Unhighlight")]
        public void TestUnhighlight()
        {
            if (grabbableComponent != null)
            {
                grabbableComponent.Unhighlight();
            }
        }
    }
}
