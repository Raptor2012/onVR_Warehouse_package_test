using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;

namespace OnVR.Warehouse.GrabInteraction.Editor
{
    /// <summary>
    /// Template generator for creating new onVR Warehouse packages
    /// </summary>
    public class PackageTemplateGenerator : EditorWindow
    {
        private string _packageName = "";
        private string _featureName = "";
        private string _displayName = "";
        private string _description = "";
        private string _targetPath = "";
        private bool _includeArticyIntegration = true;
        private bool _includeDebugUI = true;
        private bool _includeSamples = true;

        [MenuItem("onVR/Package Template/Create New Package")]
        public static void ShowWindow()
        {
            var window = GetWindow<PackageTemplateGenerator>("Package Template Generator");
            window.minSize = new Vector2(500, 600);
            window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.Space();
            var headerStyle = new GUIStyle(EditorStyles.largeLabel)
            {
                fontSize = 16,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            EditorGUILayout.LabelField("onVR Warehouse Package Generator", headerStyle);
            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Package Information", EditorStyles.boldLabel);
            
            _packageName = EditorGUILayout.TextField("Package Name (e.g., teleportation)", _packageName);
            _featureName = EditorGUILayout.TextField("Feature Name (e.g., Teleportation)", _featureName);
            _displayName = EditorGUILayout.TextField("Display Name", _displayName);
            _description = EditorGUILayout.TextArea("Description", _description, GUILayout.Height(60));
            
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Target Location", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();
            _targetPath = EditorGUILayout.TextField("Target Path", _targetPath);
            if (GUILayout.Button("Browse", GUILayout.Width(60)))
            {
                var path = EditorUtility.OpenFolderPanel("Select Package Directory", "Assets", "");
                if (!string.IsNullOrEmpty(path))
                {
                    _targetPath = path;
                }
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Package Options", EditorStyles.boldLabel);
            
            _includeArticyIntegration = EditorGUILayout.Toggle("Include Articy Integration", _includeArticyIntegration);
            _includeDebugUI = EditorGUILayout.Toggle("Include Debug UI", _includeDebugUI);
            _includeSamples = EditorGUILayout.Toggle("Include Sample Content", _includeSamples);
            
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            // Validation
            bool isValid = ValidateInput();
            GUI.enabled = isValid;

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Generate Package", EditorStyles.boldLabel);
            
            if (GUILayout.Button("Generate Package", GUILayout.Height(30)))
            {
                GeneratePackage();
            }
            
            GUI.enabled = true;
            
            if (!isValid)
            {
                EditorGUILayout.HelpBox("Please fill in all required fields and select a valid target path.", MessageType.Warning);
            }
            
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            // Help
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Help", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "This will create a new package following the onVR Warehouse template architecture. " +
                "The generated package will include all necessary files and structure for cross-platform compatibility.",
                MessageType.Info);
            EditorGUILayout.EndVertical();
        }

        private bool ValidateInput()
        {
            return !string.IsNullOrEmpty(_packageName) &&
                   !string.IsNullOrEmpty(_featureName) &&
                   !string.IsNullOrEmpty(_displayName) &&
                   !string.IsNullOrEmpty(_description) &&
                   !string.IsNullOrEmpty(_targetPath) &&
                   Directory.Exists(_targetPath);
        }

        private void GeneratePackage()
        {
            try
            {
                string packagePath = Path.Combine(_targetPath, $"com.onvr.warehouse.{_packageName.ToLower()}");
                
                if (Directory.Exists(packagePath))
                {
                    if (!EditorUtility.DisplayDialog("Package Exists", 
                        $"Package directory already exists at {packagePath}. Do you want to overwrite it?", 
                        "Yes", "No"))
                    {
                        return;
                    }
                    Directory.Delete(packagePath, true);
                }

                Directory.CreateDirectory(packagePath);
                
                // Generate package structure
                GeneratePackageJson(packagePath);
                GenerateReadme(packagePath);
                GenerateChangelog(packagePath);
                GenerateLicense(packagePath);
                GenerateRuntimeStructure(packagePath);
                GenerateEditorStructure(packagePath);
                
                if (_includeSamples)
                {
                    GenerateSamplesStructure(packagePath);
                }

                AssetDatabase.Refresh();
                
                EditorUtility.DisplayDialog("Package Generated", 
                    $"Package '{_displayName}' has been successfully generated at:\n{packagePath}", "OK");
                
                // Open the generated package in Project window
                var packageFolder = AssetDatabase.LoadAssetAtPath<Object>(packagePath.Replace(Application.dataPath, "Assets"));
                if (packageFolder != null)
                {
                    Selection.activeObject = packageFolder;
                    EditorGUIUtility.PingObject(packageFolder);
                }
            }
            catch (System.Exception e)
            {
                EditorUtility.DisplayDialog("Generation Failed", 
                    $"Failed to generate package: {e.Message}", "OK");
                Debug.LogError($"Package generation failed: {e}");
            }
        }

        private void GeneratePackageJson(string packagePath)
        {
            var packageJson = new StringBuilder();
            packageJson.AppendLine("{");
            packageJson.AppendLine($"  \"name\": \"com.onvr.warehouse.{_packageName.ToLower()}\",");
            packageJson.AppendLine("  \"version\": \"1.0.0\",");
            packageJson.AppendLine($"  \"displayName\": \"{_displayName}\",");
            packageJson.AppendLine($"  \"description\": \"{_description}\",");
            packageJson.AppendLine("  \"unity\": \"2021.3\",");
            packageJson.AppendLine("  \"dependencies\": {");
            packageJson.AppendLine("    \"com.unity.xr.interaction.toolkit\": \"2.3.0\",");
            packageJson.AppendLine("    \"com.onvr.warehouse.core\": \"1.0.0\"");
            packageJson.AppendLine("  },");
            packageJson.AppendLine("  \"keywords\": [");
            packageJson.AppendLine("    \"VR\",");
            packageJson.AppendLine("    \"Interaction\",");
            packageJson.AppendLine($"    \"{_featureName}\",");
            packageJson.AppendLine("    \"Mobile\",");
            packageJson.AppendLine("    \"Web\",");
            packageJson.AppendLine("    \"XR\",");
            packageJson.AppendLine("    \"Modular\"");
            packageJson.AppendLine("  ],");
            packageJson.AppendLine("  \"author\": {");
            packageJson.AppendLine("    \"name\": \"onliveline.nxt GmbH\",");
            packageJson.AppendLine("    \"email\": \"info@onxt.de\",");
            packageJson.AppendLine("    \"url\": \"https://onxt.de\"");
            packageJson.AppendLine("  },");
            packageJson.AppendLine("  \"type\": \"library\",");
            packageJson.AppendLine("  \"hideInEditor\": false");
            packageJson.AppendLine("}");

            File.WriteAllText(Path.Combine(packagePath, "package.json"), packageJson.ToString());
        }

        private void GenerateReadme(string packagePath)
        {
            var readme = new StringBuilder();
            readme.AppendLine($"# {_displayName}");
            readme.AppendLine();
            readme.AppendLine(_description);
            readme.AppendLine();
            readme.AppendLine("## 🎯 Key Features");
            readme.AppendLine();
            readme.AppendLine("- **🌐 Cross-Platform Support**: VR, Web, Mobile, and Desktop");
            readme.AppendLine("- **🔧 Modular Architecture**: Easy to integrate, customize, and extend");
            readme.AppendLine("- **🎮 Multiple Interaction Methods**: Platform-specific interactions");
            readme.AppendLine("- **⚡ Performance Optimized**: Efficient platform detection and handling");
            readme.AppendLine();
            readme.AppendLine("## 📦 Installation");
            readme.AppendLine();
            readme.AppendLine("### Package Manager (Recommended)");
            readme.AppendLine("1. Open Package Manager in Unity");
            readme.AppendLine("2. Add package from git URL: `https://github.com/onliveline-nxt/[package-name].git`");
            readme.AppendLine("3. Dependencies will be installed automatically");
            readme.AppendLine();
            readme.AppendLine("## ⚡ Quick Start");
            readme.AppendLine();
            readme.AppendLine("### Basic Setup");
            readme.AppendLine("```csharp");
            readme.AppendLine($"// 1. Add to any GameObject");
            readme.AppendLine($"var {_featureName.ToLower()}Object = myObject.AddComponent<{_featureName}Object>();");
            readme.AppendLine();
            readme.AppendLine("// 2. Configure for your platform");
            readme.AppendLine($"{_featureName.ToLower()}Object.SupportedPlatforms = InteractionPlatform.All;");
            readme.AppendLine("```");
            readme.AppendLine();
            readme.AppendLine("## 📋 Requirements");
            readme.AppendLine();
            readme.AppendLine("- **Unity**: 2021.3 LTS or later");
            readme.AppendLine("- **XR Interaction Toolkit**: 2.3.0+");
            readme.AppendLine("- **Platform SDKs**: Based on target platform");

            File.WriteAllText(Path.Combine(packagePath, "README.md"), readme.ToString());
        }

        private void GenerateChangelog(string packagePath)
        {
            var changelog = new StringBuilder();
            changelog.AppendLine("# Changelog");
            changelog.AppendLine();
            changelog.AppendLine("All notable changes to this package will be documented in this file.");
            changelog.AppendLine();
            changelog.AppendLine("The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),");
            changelog.AppendLine("and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).");
            changelog.AppendLine();
            changelog.AppendLine("## [1.0.0] - 2025-01-XX");
            changelog.AppendLine();
            changelog.AppendLine("### Added");
            changelog.AppendLine($"- Initial release of {_displayName}");
            changelog.AppendLine("- Cross-platform support (VR, Mobile, Web, Desktop)");
            changelog.AppendLine("- Platform-specific interaction handlers");
            changelog.AppendLine("- Articy integration support");
            changelog.AppendLine("- Setup wizard and editor tools");

            File.WriteAllText(Path.Combine(packagePath, "CHANGELOG.md"), changelog.ToString());
        }

        private void GenerateLicense(string packagePath)
        {
            var license = new StringBuilder();
            license.AppendLine("© 2025 onliveline.nxt GmbH. All rights reserved.");
            license.AppendLine();
            license.AppendLine("This software is licensed under a proprietary license.");
            license.AppendLine();
            license.AppendLine("For commercial licensing inquiries: licensing@onliveline.nxt");

            File.WriteAllText(Path.Combine(packagePath, "LICENSE.md"), license.ToString());
        }

        private void GenerateRuntimeStructure(string packagePath)
        {
            string runtimePath = Path.Combine(packagePath, "Runtime");
            Directory.CreateDirectory(runtimePath);

            // Assembly definition
            var asmdef = new StringBuilder();
            asmdef.AppendLine("{");
            asmdef.AppendLine($"  \"name\": \"com.onvr.warehouse.{_packageName.ToLower()}\",");
            asmdef.AppendLine($"  \"rootNamespace\": \"OnVR.Warehouse.{_featureName}\",");
            asmdef.AppendLine("  \"references\": [");
            asmdef.AppendLine("    \"Unity.XR.Interaction.Toolkit\",");
            asmdef.AppendLine("    \"Unity.InputSystem\",");
            asmdef.AppendLine("    \"Unity.XR.CoreUtils\",");
            asmdef.AppendLine("    \"com.onvr.warehouse.core\"");
            asmdef.AppendLine("  ],");
            asmdef.AppendLine("  \"includePlatforms\": [],");
            asmdef.AppendLine("  \"excludePlatforms\": [],");
            asmdef.AppendLine("  \"allowUnsafeCode\": false,");
            asmdef.AppendLine("  \"overrideReferences\": false,");
            asmdef.AppendLine("  \"precompiledReferences\": [],");
            asmdef.AppendLine("  \"autoReferenced\": true,");
            asmdef.AppendLine("  \"defineConstraints\": [],");
            asmdef.AppendLine("  \"versionDefines\": [],");
            asmdef.AppendLine("  \"noEngineReferences\": false");
            asmdef.AppendLine("}");

            File.WriteAllText(Path.Combine(runtimePath, $"com.onvr.warehouse.{_packageName.ToLower()}.asmdef"), asmdef.ToString());

            // Scripts structure
            string scriptsPath = Path.Combine(runtimePath, "Scripts");
            Directory.CreateDirectory(scriptsPath);
            Directory.CreateDirectory(Path.Combine(scriptsPath, "Core"));
            Directory.CreateDirectory(Path.Combine(scriptsPath, "Platforms"));
            Directory.CreateDirectory(Path.Combine(scriptsPath, "Integration"));

            // Generate core scripts
            GenerateCoreScripts(scriptsPath);
            GeneratePlatformScripts(scriptsPath);
            
            if (_includeArticyIntegration)
            {
                GenerateArticyIntegration(scriptsPath);
            }
            
            if (_includeDebugUI)
            {
                GenerateDebugUI(runtimePath);
            }
        }

        private void GenerateCoreScripts(string scriptsPath)
        {
            string corePath = Path.Combine(scriptsPath, "Core");
            
            // Types file
            var typesContent = GenerateTypesFile();
            File.WriteAllText(Path.Combine(corePath, $"{_featureName}Types.cs"), typesContent);
            
            // Manager file
            var managerContent = GenerateManagerFile();
            File.WriteAllText(Path.Combine(corePath, $"{_featureName}Manager.cs"), managerContent);
            
            // Object file
            var objectContent = GenerateObjectFile();
            File.WriteAllText(Path.Combine(corePath, $"{_featureName}Object.cs"), objectContent);
        }

        private string GenerateTypesFile()
        {
            var content = new StringBuilder();
            content.AppendLine("using System;");
            content.AppendLine();
            content.AppendLine($"namespace OnVR.Warehouse.{_featureName}.Core");
            content.AppendLine("{");
            content.AppendLine($"    /// <summary>");
            content.AppendLine($"    /// Types of {_featureName.ToLower()} interactions available");
            content.AppendLine($"    /// </summary>");
            content.AppendLine($"    public enum {_featureName}InteractionType");
            content.AppendLine("    {");
            content.AppendLine("        DirectHand,      // VR direct interaction");
            content.AppendLine($"        Remote{_featureName},      // VR remote interaction");
            content.AppendLine("        TapClick,        // Mobile/Web tap or click");
            content.AppendLine($"        Gaze{_featureName},        // Gaze-based interaction");
            content.AppendLine($"        {_featureName}Drag         // Drag interaction");
            content.AppendLine("    }");
            content.AppendLine();
            content.AppendLine($"    /// <summary>");
            content.AppendLine($"    /// Current state of a {_featureName.ToLower()} object");
            content.AppendLine($"    /// </summary>");
            content.AppendLine($"    public enum {_featureName}State");
            content.AppendLine("    {");
            content.AppendLine("        Idle,");
            content.AppendLine("        Active,");
            content.AppendLine($"        {_featureName}ing,");
            content.AppendLine("        Completed");
            content.AppendLine("    }");
            content.AppendLine("}");

            return content.ToString();
        }

        private string GenerateManagerFile()
        {
            var content = new StringBuilder();
            content.AppendLine("using System.Collections.Generic;");
            content.AppendLine("using UnityEngine;");
            content.AppendLine($"using OnVR.Warehouse.{_featureName}.Core;");
            content.AppendLine($"using OnVR.Warehouse.{_featureName}.Platforms;");
            content.AppendLine();
            content.AppendLine($"namespace OnVR.Warehouse.{_featureName}");
            content.AppendLine("{");
            content.AppendLine($"    /// <summary>");
            content.AppendLine($"    /// Central manager that handles all {_featureName.ToLower()} interactions across different platforms");
            content.AppendLine($"    /// </summary>");
            content.AppendLine($"    [AddComponentMenu(\"onVR/{_featureName}/{_featureName} Manager\")]");
            content.AppendLine($"    public class {_featureName}Manager : MonoBehaviour");
            content.AppendLine("    {");
            content.AppendLine("        [Header(\"Manager Settings\")]");
            content.AppendLine("        [SerializeField] private bool _autoInitialize = true;");
            content.AppendLine("        [SerializeField] private bool _debugMode = false;");
            content.AppendLine();
            content.AppendLine("        [Header(\"Platform Handlers\")]");
            content.AppendLine($"        [SerializeField] private VR{_featureName}Handler _vrHandler;");
            content.AppendLine($"        [SerializeField] private Mobile{_featureName}Handler _mobileHandler;");
            content.AppendLine($"        [SerializeField] private Web{_featureName}Handler _webHandler;");
            content.AppendLine();
            content.AppendLine("        // Singleton instance");
            content.AppendLine($"        private static {_featureName}Manager _instance;");
            content.AppendLine($"        public static {_featureName}Manager Instance");
            content.AppendLine("        {");
            content.AppendLine("            get");
            content.AppendLine("            {");
            content.AppendLine($"                if (_instance == null)");
            content.AppendLine("                {");
            content.AppendLine($"                    _instance = FindObjectOfType<{_featureName}Manager>();");
            content.AppendLine($"                    if (_instance == null)");
            content.AppendLine("                    {");
            content.AppendLine($"                        var go = new GameObject(\"{_featureName}Manager\");");
            content.AppendLine($"                        _instance = go.AddComponent<{_featureName}Manager>();");
            content.AppendLine("                    }");
            content.AppendLine("                }");
            content.AppendLine($"                return _instance;");
            content.AppendLine("            }");
            content.AppendLine("        }");
            content.AppendLine();
            content.AppendLine("        // Implementation continues...");
            content.AppendLine("        // TODO: Implement manager functionality");
            content.AppendLine("    }");
            content.AppendLine("}");

            return content.ToString();
        }

        private string GenerateObjectFile()
        {
            var content = new StringBuilder();
            content.AppendLine("using System;");
            content.AppendLine("using UnityEngine;");
            content.AppendLine("using UnityEngine.Events;");
            content.AppendLine($"using OnVR.Warehouse.{_featureName}.Core;");
            content.AppendLine();
            content.AppendLine($"namespace OnVR.Warehouse.{_featureName}");
            content.AppendLine("{");
            content.AppendLine($"    /// <summary>");
            content.AppendLine($"    /// Core component that makes any GameObject {_featureName.ToLower()}-capable across different platforms");
            content.AppendLine($"    /// </summary>");
            content.AppendLine($"    [AddComponentMenu(\"onVR/{_featureName}/{_featureName} Object\")]");
            content.AppendLine($"    public class {_featureName}Object : MonoBehaviour");
            content.AppendLine("    {");
            content.AppendLine($"        [Header(\"{_featureName} Settings\")]");
            content.AppendLine($"        [SerializeField] private bool _is{_featureName}Enabled = true;");
            content.AppendLine("        [SerializeField] private InteractionPlatform _supportedPlatforms = InteractionPlatform.All;");
            content.AppendLine($"        [SerializeField] private {_featureName}InteractionType[] _allowedInteractionTypes = new {_featureName}InteractionType[0];");
            content.AppendLine();
            content.AppendLine("        [Header(\"Events\")]");
            content.AppendLine($"        public UnityEvent On{_featureName}Start;");
            content.AppendLine($"        public UnityEvent On{_featureName}End;");
            content.AppendLine();
            content.AppendLine("        // Events");
            content.AppendLine($"        public event Action<{_featureName}Object> {_featureName}Started;");
            content.AppendLine($"        public event Action<{_featureName}Object> {_featureName}Ended;");
            content.AppendLine();
            content.AppendLine("        // Implementation continues...");
            content.AppendLine("        // TODO: Implement object functionality");
            content.AppendLine("    }");
            content.AppendLine("}");

            return content.ToString();
        }

        private void GeneratePlatformScripts(string scriptsPath)
        {
            string platformsPath = Path.Combine(scriptsPath, "Platforms");
            
            // Generate platform handler templates
            var vrHandler = GeneratePlatformHandler("VR");
            File.WriteAllText(Path.Combine(platformsPath, $"VR{_featureName}Handler.cs"), vrHandler);
            
            var mobileHandler = GeneratePlatformHandler("Mobile");
            File.WriteAllText(Path.Combine(platformsPath, $"Mobile{_featureName}Handler.cs"), mobileHandler);
            
            var webHandler = GeneratePlatformHandler("Web");
            File.WriteAllText(Path.Combine(platformsPath, $"Web{_featureName}Handler.cs"), webHandler);
        }

        private string GeneratePlatformHandler(string platform)
        {
            var content = new StringBuilder();
            content.AppendLine("using UnityEngine;");
            content.AppendLine($"using OnVR.Warehouse.{_featureName}.Core;");
            content.AppendLine();
            content.AppendLine($"namespace OnVR.Warehouse.{_featureName}.Platforms");
            content.AppendLine("{");
            content.AppendLine($"    /// <summary>");
            content.AppendLine($"    /// {platform} platform handler for {_featureName.ToLower()} interactions");
            content.AppendLine($"    /// </summary>");
            content.AppendLine($"    public class {platform}{_featureName}Handler : MonoBehaviour, I{_featureName}Handler");
            content.AppendLine("    {");
            content.AppendLine($"        private {_featureName}Manager _manager;");
            content.AppendLine();
            content.AppendLine($"        public void Initialize({_featureName}Manager manager)");
            content.AppendLine("        {");
            content.AppendLine($"            _manager = manager;");
            content.AppendLine("        }");
            content.AppendLine();
            content.AppendLine("        public void Enable()");
            content.AppendLine("        {");
            content.AppendLine("            // TODO: Enable {platform} specific functionality");
            content.AppendLine("        }");
            content.AppendLine();
            content.AppendLine("        public void Disable()");
            content.AppendLine("        {");
            content.AppendLine("            // TODO: Disable {platform} specific functionality");
            content.AppendLine("        }");
            content.AppendLine();
            content.AppendLine($"        public void OnObjectRegistered({_featureName}Object obj)");
            content.AppendLine("        {");
            content.AppendLine("            // TODO: Handle object registration");
            content.AppendLine("        }");
            content.AppendLine();
            content.AppendLine($"        public void OnObjectUnregistered({_featureName}Object obj)");
            content.AppendLine("        {");
            content.AppendLine("            // TODO: Handle object unregistration");
            content.AppendLine("        }");
            content.AppendLine();
            content.AppendLine("        public void Update()");
            content.AppendLine("        {");
            content.AppendLine("            // TODO: Handle {platform} specific updates");
            content.AppendLine("        }");
            content.AppendLine("    }");
            content.AppendLine("}");

            return content.ToString();
        }

        private void GenerateArticyIntegration(string scriptsPath)
        {
            string integrationPath = Path.Combine(scriptsPath, "Integration");
            
            var content = new StringBuilder();
            content.AppendLine("using UnityEngine;");
            content.AppendLine($"using OnVR.Warehouse.{_featureName};");
            content.AppendLine();
            content.AppendLine($"namespace OnVR.Warehouse.{_featureName}.Integration");
            content.AppendLine("{");
            content.AppendLine($"    /// <summary>");
            content.AppendLine($"    /// Articy integration for {_featureName.ToLower()} objects");
            content.AppendLine($"    /// </summary>");
            content.AppendLine($"    public class Articy{_featureName}Integration : MonoBehaviour");
            content.AppendLine("    {");
            content.AppendLine("        [Header(\"Articy Settings\")]");
            content.AppendLine("        [SerializeField] private string _articyObjectId;");
            content.AppendLine("        [SerializeField] private string _locationId;");
            content.AppendLine($"        [SerializeField] private bool _triggerFlowOn{_featureName} = true;");
            content.AppendLine($"        [SerializeField] private string _{_featureName.ToLower()}FlowFragment;");
            content.AppendLine();
            content.AppendLine("        // Implementation continues...");
            content.AppendLine("        // TODO: Implement Articy integration");
            content.AppendLine("    }");
            content.AppendLine("}");

            File.WriteAllText(Path.Combine(integrationPath, $"Articy{_featureName}Integration.cs"), content.ToString());
        }

        private void GenerateDebugUI(string runtimePath)
        {
            var content = new StringBuilder();
            content.AppendLine("using UnityEngine;");
            content.AppendLine($"using OnVR.Warehouse.{_featureName};");
            content.AppendLine();
            content.AppendLine($"namespace OnVR.Warehouse.{_featureName}");
            content.AppendLine("{");
            content.AppendLine($"    /// <summary>");
            content.AppendLine($"    /// Debug UI for {_featureName.ToLower()} interactions");
            content.AppendLine($"    /// </summary>");
            content.AppendLine($"    public class {_featureName}DebugUI : MonoBehaviour");
            content.AppendLine("    {");
            content.AppendLine("        [SerializeField] private bool _showDebugInfo = true;");
            content.AppendLine("        [SerializeField] private bool _logEvents = true;");
            content.AppendLine();
            content.AppendLine("        // Implementation continues...");
            content.AppendLine("        // TODO: Implement debug UI functionality");
            content.AppendLine("    }");
            content.AppendLine("}");

            File.WriteAllText(Path.Combine(runtimePath, $"{_featureName}DebugUI.cs"), content.ToString());
        }

        private void GenerateEditorStructure(string packagePath)
        {
            string editorPath = Path.Combine(packagePath, "Editor");
            Directory.CreateDirectory(editorPath);

            // Assembly definition
            var asmdef = new StringBuilder();
            asmdef.AppendLine("{");
            asmdef.AppendLine($"  \"name\": \"com.onvr.warehouse.{_packageName.ToLower()}.editor\",");
            asmdef.AppendLine($"  \"rootNamespace\": \"OnVR.Warehouse.{_featureName}.Editor\",");
            asmdef.AppendLine("  \"references\": [");
            asmdef.AppendLine("    \"Unity.XR.Interaction.Toolkit\",");
            asmdef.AppendLine("    \"Unity.InputSystem\",");
            asmdef.AppendLine("    \"Unity.XR.CoreUtils\",");
            asmdef.AppendLine($"    \"com.onvr.warehouse.{_packageName.ToLower()}\"");
            asmdef.AppendLine("  ],");
            asmdef.AppendLine("  \"includePlatforms\": [");
            asmdef.AppendLine("    \"Editor\"");
            asmdef.AppendLine("  ],");
            asmdef.AppendLine("  \"excludePlatforms\": [],");
            asmdef.AppendLine("  \"allowUnsafeCode\": false,");
            asmdef.AppendLine("  \"overrideReferences\": false,");
            asmdef.AppendLine("  \"precompiledReferences\": [],");
            asmdef.AppendLine("  \"autoReferenced\": true,");
            asmdef.AppendLine("  \"defineConstraints\": [],");
            asmdef.AppendLine("  \"versionDefines\": [],");
            asmdef.AppendLine("  \"noEngineReferences\": false");
            asmdef.AppendLine("}");

            File.WriteAllText(Path.Combine(editorPath, $"com.onvr.warehouse.{_packageName.ToLower()}.editor.asmdef"), asmdef.ToString());

            // Setup wizard
            var setupWizard = GenerateSetupWizard();
            File.WriteAllText(Path.Combine(editorPath, $"{_featureName}SetupWizard.cs"), setupWizard);

            // Object editor
            var objectEditor = GenerateObjectEditor();
            File.WriteAllText(Path.Combine(editorPath, $"{_featureName}ObjectEditor.cs"), objectEditor);
        }

        private string GenerateSetupWizard()
        {
            var content = new StringBuilder();
            content.AppendLine("using UnityEngine;");
            content.AppendLine("using UnityEditor;");
            content.AppendLine($"using OnVR.Warehouse.{_featureName};");
            content.AppendLine("using OnVR.Warehouse.GrabInteraction.Core;");
            content.AppendLine();
            content.AppendLine($"namespace OnVR.Warehouse.{_featureName}.Editor");
            content.AppendLine("{");
            content.AppendLine($"    /// <summary>");
            content.AppendLine($"    /// Setup wizard for {_displayName}");
            content.AppendLine($"    /// </summary>");
            content.AppendLine($"    public class {_featureName}SetupWizard : EditorWindow");
            content.AppendLine("    {");
            content.AppendLine("        private Vector2 _scrollPosition;");
            content.AppendLine("        private InteractionPlatform _targetPlatform = InteractionPlatform.All;");
            content.AppendLine("        private bool _autoSetupManager = true;");
            content.AppendLine("        private bool _addSampleObjects = true;");
            content.AppendLine();
            content.AppendLine($"        [MenuItem(\"onVR/{_featureName}/Setup Wizard\")]");
            content.AppendLine("        public static void ShowWindow()");
            content.AppendLine("        {");
            content.AppendLine($"            var window = GetWindow<{_featureName}SetupWizard>(\"{_featureName} Setup\");");
            content.AppendLine("            window.minSize = new Vector2(400, 500);");
            content.AppendLine("            window.Show();");
            content.AppendLine("        }");
            content.AppendLine();
            content.AppendLine("        private void OnGUI()");
            content.AppendLine("        {");
            content.AppendLine("            // TODO: Implement setup wizard GUI");
            content.AppendLine("            EditorGUILayout.LabelField(\"Setup Wizard - TODO\");");
            content.AppendLine("        }");
            content.AppendLine("    }");
            content.AppendLine("}");

            return content.ToString();
        }

        private string GenerateObjectEditor()
        {
            var content = new StringBuilder();
            content.AppendLine("using UnityEngine;");
            content.AppendLine("using UnityEditor;");
            content.AppendLine($"using OnVR.Warehouse.{_featureName};");
            content.AppendLine();
            content.AppendLine($"namespace OnVR.Warehouse.{_featureName}.Editor");
            content.AppendLine("{");
            content.AppendLine($"    /// <summary>");
            content.AppendLine($"    /// Custom editor for {_featureName}Object");
            content.AppendLine($"    /// </summary>");
            content.AppendLine($"    [CustomEditor(typeof({_featureName}Object))]");
            content.AppendLine($"    public class {_featureName}ObjectEditor : Editor");
            content.AppendLine("    {");
            content.AppendLine("        public override void OnInspectorGUI()");
            content.AppendLine("        {");
            content.AppendLine("            // TODO: Implement custom inspector");
            content.AppendLine("            DrawDefaultInspector();");
            content.AppendLine("        }");
            content.AppendLine("    }");
            content.AppendLine("}");

            return content.ToString();
        }

        private void GenerateSamplesStructure(string packagePath)
        {
            string samplesPath = Path.Combine(packagePath, "Samples~");
            Directory.CreateDirectory(samplesPath);
            
            string basicSamplePath = Path.Combine(samplesPath, $"Basic{_featureName}");
            Directory.CreateDirectory(basicSamplePath);
            
            // Sample README
            var sampleReadme = new StringBuilder();
            sampleReadme.AppendLine($"# Basic {_featureName} Sample");
            sampleReadme.AppendLine();
            sampleReadme.AppendLine($"This sample demonstrates basic {_featureName.ToLower()} functionality.");
            sampleReadme.AppendLine();
            sampleReadme.AppendLine("## Features");
            sampleReadme.AppendLine();
            sampleReadme.AppendLine("- Basic setup and configuration");
            sampleReadme.AppendLine("- Platform-specific examples");
            sampleReadme.AppendLine("- Event handling demonstration");
            sampleReadme.AppendLine();
            sampleReadme.AppendLine("## Usage");
            sampleReadme.AppendLine();
            sampleReadme.AppendLine("1. Open the sample scene");
            sampleReadme.AppendLine("2. Test on your target platform");
            sampleReadme.AppendLine("3. Examine the scripts for implementation examples");

            File.WriteAllText(Path.Combine(basicSamplePath, "README.md"), sampleReadme.ToString());
            
            Directory.CreateDirectory(Path.Combine(basicSamplePath, "Scenes"));
            Directory.CreateDirectory(Path.Combine(basicSamplePath, "Scripts"));
        }
    }
} 