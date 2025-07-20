# onVR Warehouse Package Template Architecture

## Overview
This document defines the standard architecture and guidelines for all onVR Warehouse packages. The **Grab Interaction** package serves as the reference implementation that all other packages should follow.

## 🏗 Package Structure Template

```
com.onvr.warehouse.[feature]/
├── package.json                 # Package metadata and dependencies
├── README.md                    # User documentation
├── CHANGELOG.md                 # Version history
├── LICENSE.md                   # License information
├── PACKAGE_TEMPLATE_ARCHITECTURE.md  # This template (for reference packages)
├── Runtime/
│   ├── com.onvr.warehouse.[feature].asmdef
│   ├── Scripts/
│   │   ├── Core/               # Core functionality
│   │   │   ├── [Feature]Manager.cs
│   │   │   ├── [Feature]Object.cs
│   │   │   ├── [Feature]Types.cs
│   │   │   └── PlatformDetector.cs
│   │   ├── Platforms/          # Platform-specific implementations
│   │   │   ├── VR[Feature]Handler.cs
│   │   │   ├── Mobile[Feature]Handler.cs
│   │   │   └── Web[Feature]Handler.cs
│   │   └── Integration/        # External integrations
│   │       └── Articy[Feature]Integration.cs
│   └── [Feature]DebugUI.cs     # Debug utilities
├── Editor/
│   ├── com.onvr.warehouse.[feature].editor.asmdef
│   ├── [Feature]SetupWizard.cs
│   └── [Feature]ObjectEditor.cs
└── Samples~/
    └── Basic[Feature]/
        ├── README.md
        ├── Scenes/
        └── Scripts/
```

## 📦 Package.json Template

```json
{
  "name": "com.onvr.warehouse.[feature]",
  "version": "1.0.0",
  "displayName": "onVR Warehouse - [Feature Name]",
  "description": "[Brief description of the feature and its capabilities]",
  "unity": "2021.3",
  "dependencies": {
    "com.unity.xr.interaction.toolkit": "2.3.0",
    "com.onvr.warehouse.core": "1.0.0"
  },
  "keywords": [
    "VR",
    "Interaction",
    "[Feature]",
    "Mobile",
    "Web",
    "XR",
    "Modular"
  ],
  "author": {
    "name": "onliveline.nxt GmbH",
    "email": "info@onxt.de",
    "url": "https://onxt.de"
  },
  "type": "library",
  "hideInEditor": false
}
```

## 🎯 Core Architecture Principles

### 1. **Manager Pattern**
Every package must implement a singleton manager that:
- Handles platform detection and switching
- Manages registered objects/components
- Provides centralized configuration
- Implements the `I[Feature]Manager` interface

```csharp
public interface I[Feature]Manager
{
    void Initialize();
    void Register[Feature]Object([Feature]Object obj);
    void Unregister[Feature]Object([Feature]Object obj);
    List<[Feature]Object> GetRegisteredObjects();
    void SwitchPlatform(InteractionPlatform platform);
}
```

### 2. **Platform Handler Pattern**
Each platform must have a dedicated handler implementing:
```csharp
public interface I[Feature]Handler
{
    void Initialize([Feature]Manager manager);
    void Enable();
    void Disable();
    void OnObjectRegistered([Feature]Object obj);
    void OnObjectUnregistered([Feature]Object obj);
    void Update();
}
```

### 3. **Object Component Pattern**
Interactive objects must implement:
```csharp
public class [Feature]Object : MonoBehaviour
{
    // Configuration
    [SerializeField] private InteractionPlatform _supportedPlatforms;
    [SerializeField] private [Feature]InteractionType[] _allowedTypes;
    
    // Events
    public UnityEvent On[Feature]Start;
    public UnityEvent On[Feature]End;
    public event Action<[Feature]Object> [Feature]Started;
    public event Action<[Feature]Object> [Feature]Ended;
    
    // Core methods
    public bool Try[Feature](Transform source, [Feature]InteractionType type);
    public void End[Feature]();
    public bool Can[Feature]([Feature]InteractionType type);
}
```

### 4. **Type System**
Standard enums for all packages:
```csharp
[Flags]
public enum InteractionPlatform
{
    None = 0,
    VR = 1 << 0,
    Mobile = 1 << 1,
    Web = 1 << 2,
    Desktop = 1 << 3,
    All = VR | Mobile | Web | Desktop
}

public enum [Feature]InteractionType
{
    DirectHand,      // VR direct interaction
    Remote[Feature], // VR remote interaction
    TapClick,        // Mobile/Web tap or click
    Gaze[Feature],   // Gaze-based interaction
    [Feature]Drag     // Drag interaction
}

public enum [Feature]State
{
    Idle,
    Active,
    [Feature]ing,
    Completed
}
```

## 🔧 Implementation Guidelines

### 1. **Namespace Convention**
```csharp
namespace OnVR.Warehouse.[Feature]
{
    // Core classes
    namespace OnVR.Warehouse.[Feature].Core
    {
        // Types and interfaces
    }
    
    namespace OnVR.Warehouse.[Feature].Platforms
    {
        // Platform handlers
    }
    
    namespace OnVR.Warehouse.[Feature].Integration
    {
        // External integrations
    }
}
```

### 2. **Assembly Definition**
```json
{
    "name": "com.onvr.warehouse.[feature]",
    "rootNamespace": "OnVR.Warehouse.[Feature]",
    "references": [
        "Unity.XR.Interaction.Toolkit",
        "Unity.InputSystem",
        "Unity.XR.CoreUtils",
        "com.onvr.warehouse.core"
    ],
    "autoReferenced": true
}
```

### 3. **Editor Tools**
Every package must include:
- **Setup Wizard**: `onVR/[Feature]/Setup Wizard`
- **Custom Inspector**: Enhanced component editors
- **Menu Items**: Quick creation and setup options

### 4. **Debug Support**
```csharp
public class [Feature]DebugUI : MonoBehaviour
{
    [SerializeField] private bool _showDebugInfo = true;
    [SerializeField] private bool _logEvents = true;
    
    // Debug visualization and logging
}
```

## 🔗 Integration Guidelines

### 1. **Articy Integration**
All packages must support Articy integration:
```csharp
public class Articy[Feature]Integration : MonoBehaviour
{
    [SerializeField] private string _articyObjectId;
    [SerializeField] private string _locationId;
    [SerializeField] private bool _triggerFlowOn[Feature] = true;
    [SerializeField] private string _[feature]FlowFragment;
}
```

### 2. **Cross-Package Communication**
Use events and interfaces for inter-package communication:
```csharp
public static class [Feature]Events
{
    public static event Action<[Feature]Object> On[Feature]Started;
    public static event Action<[Feature]Object> On[Feature]Ended;
    public static event Action<[Feature]Object> On[Feature]StateChanged;
}
```

### 3. **Configuration System**
```csharp
[CreateAssetMenu(fileName = "[Feature]Config", menuName = "onVR/[Feature]/Configuration")]
public class [Feature]Configuration : ScriptableObject
{
    [Header("Platform Settings")]
    public InteractionPlatform defaultPlatforms = InteractionPlatform.All;
    
    [Header("Interaction Settings")]
    public float max[Feature]Distance = 10f;
    public bool enableRemote[Feature] = true;
    
    [Header("Visual Feedback")]
    public Color highlightColor = Color.yellow;
    public bool useOutlineEffect = true;
}
```

## 📋 Package Development Checklist

### Pre-Development
- [ ] Define feature scope and requirements
- [ ] Identify platform-specific behaviors
- [ ] Plan Articy integration points
- [ ] Design event system for cross-package communication

### Development
- [ ] Create package structure following template
- [ ] Implement core manager and types
- [ ] Create platform handlers (VR, Mobile, Web)
- [ ] Implement object components
- [ ] Add Articy integration
- [ ] Create editor tools and setup wizard
- [ ] Add debug utilities

### Testing
- [ ] Test on all target platforms
- [ ] Verify Articy integration
- [ ] Test cross-package communication
- [ ] Validate performance and memory usage
- [ ] Test with sample content

### Documentation
- [ ] Create comprehensive README
- [ ] Document API and usage examples
- [ ] Create integration guides
- [ ] Add troubleshooting section
- [ ] Update changelog

## 🎮 Work Package Mapping

Based on the estimation document, here's how packages should be structured:

### Core Interaction Packages
1. **Grab Interaction** ✅ (Reference Implementation)
2. **Teleportation** (Free & Guided)
3. **Locomotion** (Free, Rail-based, Continuous)
4. **Place/Throw** (Object placement and physics)
5. **Manipulate** (Object manipulation)
6. **Combine** (Object combination)

### UI/UX Packages
7. **Decision Menus** (Panel & Portal Hub)
8. **Smart Highlighting** (Visual cues)
9. **Immuno-Dex** (Wearable UI)

### System Packages
10. **Mission System** (Task tracking)
11. **AI Dialogue** (NPC interactions)
12. **Articy Integration** (Core import system)

### Utility Packages
13. **GitHub Crawler** (Asset discovery)
14. **PDF Export** (Documentation generation)
15. **RTCU Importer** (User content)

## 🚀 Next Steps

1. **Review this template** with your team
2. **Refine the grab interaction package** to fully match this template
3. **Create package creation scripts** to automate the setup
4. **Assign work packages** to developers with this template as reference
5. **Establish code review process** to ensure template compliance

This architecture ensures all packages will work together seamlessly while maintaining consistency and quality across the entire onVR Warehouse system. 