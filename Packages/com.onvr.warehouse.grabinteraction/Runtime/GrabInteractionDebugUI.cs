using UnityEngine;
using UnityEngine.UI;

namespace OnVR.Warehouse.GrabInteraction.Test
{
    /// <summary>
    /// Debug UI for testing grab interactions
    /// Provides buttons and information for testing the grab system
    /// </summary>
    public class GrabInteractionDebugUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Canvas debugCanvas;
        [SerializeField] private Transform buttonContainer;
        [SerializeField] private Text infoDisplay;
        [SerializeField] private Button testGrabButton;
        [SerializeField] private Button testReleaseButton;
        [SerializeField] private Button refreshPlatformButton;

        private GrabbableObject[] grabbableObjects;

        private void Start()
        {
            SetupDebugUI();
            RefreshGrabbableObjects();
        }

        private void SetupDebugUI()
        {
            if (debugCanvas == null)
            {
                CreateDebugCanvas();
            }

            CreateDebugButtons();
            UpdateInfoDisplay();
        }

        private void CreateDebugCanvas()
        {
            GameObject canvasGO = new GameObject("DebugCanvas");
            debugCanvas = canvasGO.AddComponent<Canvas>();
            debugCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            debugCanvas.sortingOrder = 100; // Ensure it's on top
            
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();

            // Create container for buttons
            GameObject containerGO = new GameObject("ButtonContainer");
            containerGO.transform.SetParent(debugCanvas.transform);
            buttonContainer = containerGO.transform;
            
            // Position container at top-right
            RectTransform containerRT = containerGO.AddComponent<RectTransform>();
            containerRT.anchorMin = new Vector2(1, 1);
            containerRT.anchorMax = new Vector2(1, 1);
            containerRT.pivot = new Vector2(1, 1);
            containerRT.anchoredPosition = new Vector2(-10, -10);
            containerRT.sizeDelta = new Vector2(200, 300);

            // Add vertical layout group
            var layoutGroup = containerGO.AddComponent<VerticalLayoutGroup>();
            layoutGroup.spacing = 5;
            layoutGroup.childForceExpandWidth = true;
            layoutGroup.childControlHeight = false;

            // Create info display
            CreateInfoDisplay();
        }

        private void CreateInfoDisplay()
        {
            GameObject infoGO = new GameObject("InfoDisplay");
            infoGO.transform.SetParent(debugCanvas.transform);
            
            infoDisplay = infoGO.AddComponent<Text>();
            infoDisplay.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            infoDisplay.fontSize = 10;
            infoDisplay.color = Color.white;
            
            // Add background
            var bg = infoGO.AddComponent<Image>();
            bg.color = new Color(0, 0, 0, 0.7f);
            
            // Position at bottom-right
            RectTransform rt = infoDisplay.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(1, 0);
            rt.anchorMax = new Vector2(1, 0);
            rt.pivot = new Vector2(1, 0);
            rt.anchoredPosition = new Vector2(-10, 10);
            rt.sizeDelta = new Vector2(300, 150);
        }

        private void CreateDebugButtons()
        {
            // Test Grab All button
            testGrabButton = CreateButton("Test Grab All", () => {
                foreach (var grabbable in grabbableObjects)
                {
                    if (grabbable != null && grabbable.CanBeGrabbed(Core.GrabInteractionType.TapClick))
                    {
                        grabbable.TryGrab(Camera.main.transform, Core.GrabInteractionType.TapClick);
                    }
                }
            });

            // Test Release All button
            testReleaseButton = CreateButton("Release All", () => {
                foreach (var grabbable in grabbableObjects)
                {
                    if (grabbable != null)
                    {
                        grabbable.Release();
                    }
                }
            });

            // Highlight All button
            CreateButton("Highlight All", () => {
                foreach (var grabbable in grabbableObjects)
                {
                    if (grabbable != null)
                    {
                        grabbable.Highlight();
                    }
                }
            });

            // Unhighlight All button
            CreateButton("Unhighlight All", () => {
                foreach (var grabbable in grabbableObjects)
                {
                    if (grabbable != null)
                    {
                        grabbable.Unhighlight();
                    }
                }
            });

            // Refresh Platform button
            refreshPlatformButton = CreateButton("Refresh Platform", () => {
                Core.PlatformDetector.RefreshPlatformDetection();
                UpdateInfoDisplay();
                Debug.Log($"Platform refreshed: {Core.PlatformDetector.CurrentPlatform}");
            });

            // Refresh Objects button
            CreateButton("Refresh Objects", RefreshGrabbableObjects);
        }

        private Button CreateButton(string text, System.Action onClick)
        {
            GameObject buttonGO = new GameObject($"Button_{text}");
            buttonGO.transform.SetParent(buttonContainer);
            
            Button button = buttonGO.AddComponent<Button>();
            Image buttonImage = buttonGO.AddComponent<Image>();
            buttonImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
            
            // Add text
            GameObject textGO = new GameObject("Text");
            textGO.transform.SetParent(buttonGO.transform);
            Text buttonText = textGO.AddComponent<Text>();
            buttonText.text = text;
            buttonText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            buttonText.fontSize = 12;
            buttonText.color = Color.white;
            buttonText.alignment = TextAnchor.MiddleCenter;
            
            // Set text rect
            RectTransform textRT = buttonText.GetComponent<RectTransform>();
            textRT.anchorMin = Vector2.zero;
            textRT.anchorMax = Vector2.one;
            textRT.offsetMin = Vector2.zero;
            textRT.offsetMax = Vector2.zero;
            
            // Set button size
            RectTransform buttonRT = button.GetComponent<RectTransform>();
            buttonRT.sizeDelta = new Vector2(180, 25);
            
            // Add click listener
            button.onClick.AddListener(() => onClick?.Invoke());
            
            return button;
        }

        private void RefreshGrabbableObjects()
        {
            grabbableObjects = FindObjectsOfType<GrabbableObject>();
            Debug.Log($"Found {grabbableObjects.Length} grabbable objects in scene");
            UpdateInfoDisplay();
        }

        private void UpdateInfoDisplay()
        {
            if (infoDisplay == null) return;

            var platform = Core.PlatformDetector.CurrentPlatform;
            var interactionTypes = Core.PlatformDetector.GetPreferredInteractionTypes();
            var manager = InteractionManager.Instance;

            string info = $"=== Grab Interaction Debug ===\n";
            info += $"Platform: {platform}\n";
            info += $"Interaction Types: {string.Join(", ", interactionTypes)}\n";
            info += $"Manager Active: {manager != null}\n";
            info += $"Objects Found: {grabbableObjects?.Length ?? 0}\n\n";

            if (grabbableObjects != null)
            {
                info += "Object States:\n";
                foreach (var obj in grabbableObjects)
                {
                    if (obj != null)
                    {
                        info += $"• {obj.name}: {obj.CurrentState}\n";
                    }
                }
            }

            infoDisplay.text = info;
        }

        private void Update()
        {
            UpdateInfoDisplay();
        }
    }
}
