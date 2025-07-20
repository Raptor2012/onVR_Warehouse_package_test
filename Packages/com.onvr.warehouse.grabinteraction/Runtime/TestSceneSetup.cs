using UnityEngine;
using UnityEngine.UI;

namespace OnVR.Warehouse.GrabInteraction.Test
{
    /// <summary>
    /// Test scene setup script that creates a complete test environment
    /// Run this script to automatically set up a test scene with grabbable cubes
    /// </summary>
    public class TestSceneSetup : MonoBehaviour
    {
        [Header("Test Scene Configuration")]
        [SerializeField] private bool autoSetupOnStart = true;
        [SerializeField] private int numberOfCubes = 3;
        [SerializeField] private Vector3 cubeSpacing = new Vector3(3f, 0f, 0f);
        [SerializeField] private Material[] cubeMaterials;

        [Header("UI")]
        [SerializeField] private Canvas uiCanvas;
        [SerializeField] private Text instructionText;
        [SerializeField] private Text statusText;

        private void Start()
        {
            if (autoSetupOnStart)
            {
                SetupTestScene();
            }
        }

        [ContextMenu("Setup Test Scene")]
        public void SetupTestScene()
        {
            Debug.Log("Setting up Grab Interaction Test Scene...");

            // Ensure InteractionManager exists
            SetupInteractionManager();

            // Create test cubes
            CreateTestCubes();

            // Setup UI
            SetupUI();

            // Setup camera
            SetupCamera();

            Debug.Log("Test scene setup complete!");
        }

        private void SetupInteractionManager()
        {
            // The InteractionManager will auto-create itself as a singleton
            // But we can ensure it exists and configure it
            var manager = InteractionManager.Instance;
            
            Debug.Log($"InteractionManager initialized for platform: {Core.PlatformDetector.CurrentPlatform}");
        }

        private void CreateTestCubes()
        {
            for (int i = 0; i < numberOfCubes; i++)
            {
                // Create cube
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.name = $"TestCube_{i + 1}";
                
                // Position cube
                cube.transform.position = transform.position + (cubeSpacing * i);
                
                // Add Rigidbody for physics
                Rigidbody rb = cube.GetComponent<Rigidbody>();
                if (rb == null)
                {
                    rb = cube.AddComponent<Rigidbody>();
                }
                
                // Add GrabbableObject component
                var grabbable = cube.AddComponent<GrabbableObject>();
                
                // Add test script
                var testScript = cube.AddComponent<TestGrabbableCube>();
                
                // Set material if available
                if (cubeMaterials != null && i < cubeMaterials.Length && cubeMaterials[i] != null)
                {
                    cube.GetComponent<Renderer>().material = cubeMaterials[i];
                }

                Debug.Log($"Created test cube: {cube.name} at position {cube.transform.position}");
            }
        }

        private void SetupUI()
        {
            if (uiCanvas == null)
            {
                // Create UI Canvas
                GameObject canvasGO = new GameObject("TestUI");
                uiCanvas = canvasGO.AddComponent<Canvas>();
                uiCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasGO.AddComponent<CanvasScaler>();
                canvasGO.AddComponent<GraphicRaycaster>();
            }

            // Create instruction text
            if (instructionText == null)
            {
                GameObject textGO = new GameObject("InstructionText");
                textGO.transform.SetParent(uiCanvas.transform);
                instructionText = textGO.AddComponent<Text>();
                instructionText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                instructionText.fontSize = 14;
                instructionText.color = Color.white;
                
                // Position at top-left
                RectTransform rt = instructionText.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(0, 1);
                rt.anchorMax = new Vector2(0, 1);
                rt.pivot = new Vector2(0, 1);
                rt.anchoredPosition = new Vector2(10, -10);
                rt.sizeDelta = new Vector2(400, 200);
            }

            // Create status text
            if (statusText == null)
            {
                GameObject statusGO = new GameObject("StatusText");
                statusGO.transform.SetParent(uiCanvas.transform);
                statusText = statusGO.AddComponent<Text>();
                statusText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                statusText.fontSize = 12;
                statusText.color = Color.yellow;
                
                // Position at bottom-left
                RectTransform rt = statusText.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(0, 0);
                rt.anchorMax = new Vector2(0, 0);
                rt.pivot = new Vector2(0, 0);
                rt.anchoredPosition = new Vector2(10, 10);
                rt.sizeDelta = new Vector2(400, 100);
            }

            UpdateUIText();
        }

        private void UpdateUIText()
        {
            var platform = Core.PlatformDetector.CurrentPlatform;
            var interactionTypes = Core.PlatformDetector.GetPreferredInteractionTypes();

            if (instructionText != null)
            {
                string instructions = $"Grab Interaction Test Scene\n\nPlatform: {platform}\n\nInstructions:\n";
                
                switch (platform)
                {
                    case Core.InteractionPlatform.Web:
                    case Core.InteractionPlatform.Desktop:
                        instructions += "• Click on cubes to grab them\n";
                        instructions += "• Drag while holding to move them\n";
                        instructions += "• Press G to grab, R to release\n";
                        break;
                    case Core.InteractionPlatform.Mobile:
                        instructions += "• Tap on cubes to grab them\n";
                        instructions += "• Drag your finger to move them\n";
                        instructions += "• Gaze at cubes for auto-select\n";
                        break;
                    case Core.InteractionPlatform.VR:
                        instructions += "• Point and grab with controllers\n";
                        instructions += "• Use remote grab for distant objects\n";
                        instructions += "• Gaze-based selection available\n";
                        break;
                }
                
                instructionText.text = instructions;
            }

            if (statusText != null)
            {
                statusText.text = $"Interaction Types: {string.Join(", ", interactionTypes)}\nReady for testing!";
            }
        }

        private void SetupCamera()
        {
            Camera mainCamera = Camera.main;
            if (mainCamera == null)
            {
                mainCamera = FindObjectOfType<Camera>();
            }

            if (mainCamera != null)
            {
                // Position camera to view the test cubes
                mainCamera.transform.position = new Vector3(0, 2, -5);
                mainCamera.transform.rotation = Quaternion.Euler(15, 0, 0);
            }
        }

        private void Update()
        {
            // Update status text with current state
            if (statusText != null)
            {
                var manager = InteractionManager.Instance;
                var platform = Core.PlatformDetector.CurrentPlatform;
                statusText.text = $"Platform: {platform}\nManager Active: {manager != null}\nTime: {Time.time:F1}s";
            }
        }
    }
}
