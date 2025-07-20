using UnityEngine;
using UnityEditor;
using OnVR.Warehouse.GrabInteraction;
using OnVR.Warehouse.GrabInteraction.Core;

namespace OnVR.Warehouse.GrabInteraction.Editor
{
    /// <summary>
    /// Setup wizard for onVR Grab Interaction package
    /// </summary>
    public class GrabInteractionSetupWizard : EditorWindow
    {
        private Vector2 _scrollPosition;
        private InteractionPlatform _targetPlatform = InteractionPlatform.All;
        private bool _autoSetupManager = true;
        private bool _addSampleObjects = true;
        private bool _setupArticyIntegration = false;

        [MenuItem("onVR/Grab Interaction/Setup Wizard")]
        public static void ShowWindow()
        {
            var window = GetWindow<GrabInteractionSetupWizard>("Grab Interaction Setup");
            window.minSize = new Vector2(400, 500);
            window.Show();
        }

        private void OnGUI()
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            // Header
            EditorGUILayout.Space();
            var headerStyle = new GUIStyle(EditorStyles.largeLabel)
            {
                fontSize = 16,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            EditorGUILayout.LabelField("onVR Grab Interaction Setup", headerStyle);
            EditorGUILayout.Space();

            // Current platform detection
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Platform Detection", EditorStyles.boldLabel);
            var detectedPlatform = PlatformDetector.CurrentPlatform;
            EditorGUILayout.LabelField($"Detected Platform: {detectedPlatform}");
            
            if (GUILayout.Button("Refresh Platform Detection"))
            {
                PlatformDetector.RefreshPlatformDetection();
                Repaint();
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            // Setup options
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Setup Options", EditorStyles.boldLabel);
            
            _targetPlatform = (InteractionPlatform)EditorGUILayout.EnumFlagsField("Target Platform", _targetPlatform);
            _autoSetupManager = EditorGUILayout.Toggle("Auto Setup Interaction Manager", _autoSetupManager);
            _addSampleObjects = EditorGUILayout.Toggle("Add Sample Objects", _addSampleObjects);
            _setupArticyIntegration = EditorGUILayout.Toggle("Setup Articy Integration", _setupArticyIntegration);
            
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            // Current scene analysis
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Scene Analysis", EditorStyles.boldLabel);
            
            var existingManager = FindObjectOfType<InteractionManager>();
            if (existingManager != null)
            {
                EditorGUILayout.LabelField("✓ Interaction Manager found");
            }
            else
            {
                EditorGUILayout.LabelField("✗ No Interaction Manager found");
            }

            var grabbableObjects = FindObjectsOfType<GrabbableObject>();
            EditorGUILayout.LabelField($"Grabbable Objects: {grabbableObjects.Length}");

            var articyObjects = FindObjectsOfType<Integration.ArticyIntegration>();
            EditorGUILayout.LabelField($"Articy Integration Objects: {articyObjects.Length}");
            
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            // Setup buttons
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Setup Actions", EditorStyles.boldLabel);

            if (GUILayout.Button("Complete Setup", GUILayout.Height(30)))
            {
                PerformCompleteSetup();
            }

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Setup Manager Only"))
            {
                SetupInteractionManager();
            }
            if (GUILayout.Button("Add Sample Objects"))
            {
                AddSampleObjects();
            }
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Convert Selected Objects to Grabbable"))
            {
                ConvertSelectedObjectsToGrabbable();
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            // Help and documentation
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Help & Documentation", EditorStyles.boldLabel);
            
            if (GUILayout.Button("Open Documentation"))
            {
                Application.OpenURL("https://docs.onliveline.nxt/grab-interaction");
            }
            
            if (GUILayout.Button("Report Issues"))
            {
                Application.OpenURL("https://github.com/onliveline-nxt/grab-interaction/issues");
            }
            
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndScrollView();
        }

        private void PerformCompleteSetup()
        {
            if (EditorUtility.DisplayDialog("Complete Setup", 
                $"This will set up the Grab Interaction system for {_targetPlatform}. Continue?", 
                "Yes", "Cancel"))
            {
                if (_autoSetupManager)
                {
                    SetupInteractionManager();
                }

                if (_addSampleObjects)
                {
                    AddSampleObjects();
                }

                if (_setupArticyIntegration)
                {
                    SetupArticyIntegration();
                }

                EditorUtility.DisplayDialog("Setup Complete", 
                    "Grab Interaction system has been set up successfully!", "OK");
            }
        }

        private void SetupInteractionManager()
        {
            var existingManager = FindObjectOfType<InteractionManager>();
            if (existingManager == null)
            {
                var managerGO = new GameObject("InteractionManager");
                var manager = managerGO.AddComponent<InteractionManager>();
                
                // Add platform-specific handlers based on target platform
                if (_targetPlatform.HasFlag(InteractionPlatform.VR))
                {
                    managerGO.AddComponent<Platforms.VRInteractionHandler>();
                }
                if (_targetPlatform.HasFlag(InteractionPlatform.Mobile))
                {
                    managerGO.AddComponent<Platforms.MobileInteractionHandler>();
                }
                if (_targetPlatform.HasFlag(InteractionPlatform.Web) || _targetPlatform.HasFlag(InteractionPlatform.Desktop))
                {
                    managerGO.AddComponent<Platforms.WebInteractionHandler>();
                }

                Selection.activeGameObject = managerGO;
                Debug.Log("InteractionManager created and configured.");
            }
            else
            {
                Debug.Log("InteractionManager already exists in scene.");
            }
        }

        private void AddSampleObjects()
        {
            // Create a sample grabbable cube
            var cubeGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cubeGO.name = "SampleGrabbableCube";
            cubeGO.transform.position = Vector3.up;
            
            var grabbable = cubeGO.AddComponent<GrabbableObject>();
            cubeGO.AddComponent<Rigidbody>();
            
            // Create a sample grabbable sphere
            var sphereGO = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphereGO.name = "SampleGrabbableSphere";
            sphereGO.transform.position = Vector3.up + Vector3.right * 2;
            
            var sphereGrabbable = sphereGO.AddComponent<GrabbableObject>();
            sphereGO.AddComponent<Rigidbody>();
            
            // Create a parent object for organization
            var sampleParent = new GameObject("Sample Grabbable Objects");
            cubeGO.transform.SetParent(sampleParent.transform);
            sphereGO.transform.SetParent(sampleParent.transform);
            
            Selection.activeGameObject = sampleParent;
            Debug.Log("Sample grabbable objects created.");
        }

        private void ConvertSelectedObjectsToGrabbable()
        {
            var selectedObjects = Selection.gameObjects;
            if (selectedObjects.Length == 0)
            {
                EditorUtility.DisplayDialog("No Selection", "Please select objects to convert to grabbable.", "OK");
                return;
            }

            int convertedCount = 0;
            foreach (var obj in selectedObjects)
            {
                if (obj.GetComponent<GrabbableObject>() == null)
                {
                    obj.AddComponent<GrabbableObject>();
                    
                    // Ensure collider exists
                    if (obj.GetComponent<Collider>() == null && obj.GetComponentInChildren<Collider>() == null)
                    {
                        obj.AddComponent<BoxCollider>();
                    }
                    
                    convertedCount++;
                }
            }

            EditorUtility.DisplayDialog("Conversion Complete", 
                $"Converted {convertedCount} objects to grabbable.", "OK");
        }

        private void SetupArticyIntegration()
        {
            var selectedObjects = Selection.gameObjects;
            if (selectedObjects.Length == 0)
            {
                EditorUtility.DisplayDialog("No Selection", 
                    "Please select objects to add Articy integration to.", "OK");
                return;
            }

            foreach (var obj in selectedObjects)
            {
                if (obj.GetComponent<Integration.ArticyIntegration>() == null)
                {
                    // Ensure grabbable component exists
                    if (obj.GetComponent<GrabbableObject>() == null)
                    {
                        obj.AddComponent<GrabbableObject>();
                    }
                    
                    obj.AddComponent<Integration.ArticyIntegration>();
                }
            }

            Debug.Log("Articy integration added to selected objects.");
        }
    }

    /// <summary>
    /// Menu items for quick actions
    /// </summary>
    public static class GrabInteractionMenuItems
    {
        [MenuItem("onVR/Grab Interaction/Add to Selected Objects")]
        public static void AddGrabbableToSelected()
        {
            var selectedObjects = Selection.gameObjects;
            if (selectedObjects.Length == 0)
            {
                Debug.LogWarning("No objects selected!");
                return;
            }

            foreach (var obj in selectedObjects)
            {
                if (obj.GetComponent<GrabbableObject>() == null)
                {
                    obj.AddComponent<GrabbableObject>();
                    
                    // Ensure collider exists
                    if (obj.GetComponent<Collider>() == null && obj.GetComponentInChildren<Collider>() == null)
                    {
                        obj.AddComponent<BoxCollider>();
                    }
                }
            }

            Debug.Log($"Added GrabbableObject to {selectedObjects.Length} objects.");
        }

        [MenuItem("onVR/Grab Interaction/Create Interaction Manager")]
        public static void CreateInteractionManager()
        {
            var existingManager = Object.FindObjectOfType<InteractionManager>();
            if (existingManager != null)
            {
                Debug.LogWarning("InteractionManager already exists in scene!");
                Selection.activeGameObject = existingManager.gameObject;
                return;
            }

            var managerGO = new GameObject("InteractionManager");
            managerGO.AddComponent<InteractionManager>();
            Selection.activeGameObject = managerGO;
            
            Debug.Log("InteractionManager created.");
        }

        [MenuItem("GameObject/onVR/Grabbable Object", false, 10)]
        public static void CreateGrabbableObject(MenuCommand menuCommand)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = "GrabbableObject";
            go.AddComponent<GrabbableObject>();
            go.AddComponent<Rigidbody>();
            
            // Place in scene
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }
    }
}
