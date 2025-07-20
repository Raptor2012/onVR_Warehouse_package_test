using UnityEngine;
using OnVR.Warehouse.GrabInteraction;
using OnVR.Warehouse.GrabInteraction.Core;

namespace OnVR.Warehouse.GrabInteraction.Samples
{
    /// <summary>
    /// Automatically sets up a test scene for the grab interaction package
    /// </summary>
    public class TestSceneSetup : MonoBehaviour
    {
        [Header("Test Configuration")]
        [SerializeField] private bool _autoSetupOnStart = true;
        [SerializeField] private bool _createSampleObjects = true;
        [SerializeField] private bool _setupVRRig = true;
        [SerializeField] private bool _setupLighting = true;

        [Header("Sample Objects")]
        [SerializeField] private GameObject _cubePrefab;
        [SerializeField] private GameObject _spherePrefab;
        [SerializeField] private GameObject _cylinderPrefab;

        private void Start()
        {
            if (_autoSetupOnStart)
            {
                SetupTestScene();
            }
        }

        [ContextMenu("Setup Test Scene")]
        public void SetupTestScene()
        {
            Debug.Log("Setting up test scene for onVR Grab Interaction...");

            // Setup interaction manager
            SetupInteractionManager();

            // Setup VR rig if needed
            if (_setupVRRig)
            {
                SetupVRRig();
            }

            // Setup lighting
            if (_setupLighting)
            {
                SetupLighting();
            }

            // Create sample objects
            if (_createSampleObjects)
            {
                CreateSampleObjects();
            }

            // Log platform detection
            LogPlatformInfo();

            Debug.Log("Test scene setup complete!");
        }

        private void SetupInteractionManager()
        {
            var existingManager = FindObjectOfType<InteractionManager>();
            if (existingManager == null)
            {
                var managerGO = new GameObject("InteractionManager");
                var manager = managerGO.AddComponent<InteractionManager>();
                
                // Add platform handlers
                managerGO.AddComponent<Platforms.VRInteractionHandler>();
                managerGO.AddComponent<Platforms.MobileInteractionHandler>();
                managerGO.AddComponent<Platforms.WebInteractionHandler>();

                Debug.Log("Created InteractionManager with all platform handlers");
            }
            else
            {
                Debug.Log("InteractionManager already exists in scene");
            }
        }

        private void SetupVRRig()
        {
            // Check if we're in VR mode
            if (PlatformDetector.CurrentPlatform.HasFlag(InteractionPlatform.VR))
            {
                var xrRig = FindObjectOfType<UnityEngine.XR.Interaction.Toolkit.XRRig>();
                if (xrRig == null)
                {
                    Debug.LogWarning("No XR Rig found in scene. VR interactions may not work properly.");
                    Debug.Log("To fix this, add an XR Rig to your scene or enable XR in Project Settings.");
                }
                else
                {
                    Debug.Log("XR Rig found and ready for VR interactions");
                }
            }
        }

        private void SetupLighting()
        {
            // Ensure we have proper lighting
            var mainLight = FindObjectOfType<Light>();
            if (mainLight == null)
            {
                var lightGO = new GameObject("Directional Light");
                var light = lightGO.AddComponent<Light>();
                light.type = LightType.Directional;
                light.intensity = 1f;
                light.shadows = LightShadows.Soft;
                lightGO.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
                
                Debug.Log("Created directional light for proper scene lighting");
            }
        }

        private void CreateSampleObjects()
        {
            // Create a ground plane
            CreateGroundPlane();

            // Create sample grabbable objects
            CreateGrabbableCube(new Vector3(0, 1, 2));
            CreateGrabbableSphere(new Vector3(2, 1, 0));
            CreateGrabbableCylinder(new Vector3(-2, 1, 0));

            Debug.Log("Created sample grabbable objects");
        }

        private void CreateGroundPlane()
        {
            var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.position = Vector3.zero;
            ground.transform.localScale = Vector3.one * 5f;

            // Add a material
            var renderer = ground.GetComponent<Renderer>();
            var material = new Material(Shader.Find("Standard"));
            material.color = new Color(0.3f, 0.3f, 0.3f);
            renderer.material = material;
        }

        private void CreateGrabbableCube(Vector3 position)
        {
            GameObject cube;
            if (_cubePrefab != null)
            {
                cube = Instantiate(_cubePrefab, position, Quaternion.identity);
            }
            else
            {
                cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = position;
                
                // Add a material
                var renderer = cube.GetComponent<Renderer>();
                var material = new Material(Shader.Find("Standard"));
                material.color = Color.red;
                renderer.material = material;
            }

            cube.name = "Grabbable Cube";
            
            // Add grabbable component
            var grabbable = cube.AddComponent<GrabbableObject>();
            grabbable.SupportedPlatforms = InteractionPlatform.All;
            grabbable.HighlightColor = Color.yellow;
            grabbable.UseOutlineEffect = true;

            // Add rigidbody for physics
            var rigidbody = cube.GetComponent<Rigidbody>();
            if (rigidbody == null)
            {
                rigidbody = cube.AddComponent<Rigidbody>();
            }
            rigidbody.mass = 1f;
            rigidbody.drag = 0.5f;
            rigidbody.angularDrag = 0.5f;
        }

        private void CreateGrabbableSphere(Vector3 position)
        {
            GameObject sphere;
            if (_spherePrefab != null)
            {
                sphere = Instantiate(_spherePrefab, position, Quaternion.identity);
            }
            else
            {
                sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.transform.position = position;
                
                // Add a material
                var renderer = sphere.GetComponent<Renderer>();
                var material = new Material(Shader.Find("Standard"));
                material.color = Color.blue;
                renderer.material = material;
            }

            sphere.name = "Grabbable Sphere";
            
            // Add grabbable component
            var grabbable = sphere.AddComponent<GrabbableObject>();
            grabbable.SupportedPlatforms = InteractionPlatform.All;
            grabbable.HighlightColor = Color.cyan;
            grabbable.UseOutlineEffect = true;

            // Add rigidbody for physics
            var rigidbody = sphere.GetComponent<Rigidbody>();
            if (rigidbody == null)
            {
                rigidbody = sphere.AddComponent<Rigidbody>();
            }
            rigidbody.mass = 0.5f;
            rigidbody.drag = 0.5f;
            rigidbody.angularDrag = 0.5f;
        }

        private void CreateGrabbableCylinder(Vector3 position)
        {
            GameObject cylinder;
            if (_cylinderPrefab != null)
            {
                cylinder = Instantiate(_cylinderPrefab, position, Quaternion.identity);
            }
            else
            {
                cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                cylinder.transform.position = position;
                
                // Add a material
                var renderer = cylinder.GetComponent<Renderer>();
                var material = new Material(Shader.Find("Standard"));
                material.color = Color.green;
                renderer.material = material;
            }

            cylinder.name = "Grabbable Cylinder";
            
            // Add grabbable component
            var grabbable = cylinder.AddComponent<GrabbableObject>();
            grabbable.SupportedPlatforms = InteractionPlatform.All;
            grabbable.HighlightColor = Color.magenta;
            grabbable.UseOutlineEffect = true;

            // Add rigidbody for physics
            var rigidbody = cylinder.GetComponent<Rigidbody>();
            if (rigidbody == null)
            {
                rigidbody = cylinder.AddComponent<Rigidbody>();
            }
            rigidbody.mass = 1.5f;
            rigidbody.drag = 0.5f;
            rigidbody.angularDrag = 0.5f;
        }

        private void LogPlatformInfo()
        {
            var platform = PlatformDetector.CurrentPlatform;
            Debug.Log($"=== Platform Detection ===");
            Debug.Log($"Current Platform: {platform}");
            Debug.Log($"Unity Platform: {Application.platform}");
            Debug.Log($"Is Mobile: {Application.isMobilePlatform}");
            Debug.Log($"XR Enabled: {UnityEngine.XR.XRSettings.enabled}");
            Debug.Log($"XR Device: {UnityEngine.XR.XRSettings.loadedDeviceName}");
            
            #if UNITY_EDITOR
            var buildTarget = UnityEditor.EditorUserBuildSettings.activeBuildTarget;
            Debug.Log($"Build Target: {buildTarget}");
            #endif
            
            Debug.Log($"=========================");
        }

        [ContextMenu("Test Platform Detection")]
        public void TestPlatformDetection()
        {
            PlatformDetector.RefreshPlatformDetection();
            LogPlatformInfo();
        }

        [ContextMenu("Create Single Test Object")]
        public void CreateSingleTestObject()
        {
            CreateGrabbableCube(new Vector3(0, 1, 0));
        }

        [ContextMenu("Clear All Grabbable Objects")]
        public void ClearAllGrabbableObjects()
        {
            var grabbables = FindObjectsOfType<GrabbableObject>();
            foreach (var grabbable in grabbables)
            {
                if (Application.isPlaying)
                {
                    Destroy(grabbable.gameObject);
                }
                else
                {
                    DestroyImmediate(grabbable.gameObject);
                }
            }
            Debug.Log($"Cleared {grabbables.Length} grabbable objects");
        }
    }
} 