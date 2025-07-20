# onVR Warehouse Development Guide

## 🎯 Overview
This guide provides step-by-step instructions for developing new onVR Warehouse packages following the established template architecture. The **Grab Interaction** package serves as the reference implementation.

## 🚀 Quick Start for New Developers

### 1. **Setup Your Development Environment**
```bash
# Clone the repository
git clone https://github.com/onliveline-nxt/onvr-warehouse.git
cd onvr-warehouse

# Open in Unity 2021.3 LTS or later
# Install required packages via Package Manager:
# - XR Interaction Toolkit 2.3.0+
# - Input System 1.4.0+
# - XR Core Utils 2.2.0+
```

### 2. **Create a New Package**
1. Open Unity Editor
2. Go to `onVR > Package Template > Create New Package`
3. Fill in the package details:
   - **Package Name**: `teleportation` (lowercase, no spaces)
   - **Feature Name**: `Teleportation` (PascalCase)
   - **Display Name**: `onVR Warehouse - Teleportation`
   - **Description**: Brief description of functionality
4. Select target path and options
5. Click "Generate Package"

### 3. **Implement Core Functionality**
Follow the generated template and implement:
- Core manager and types
- Platform handlers (VR, Mobile, Web)
- Object components
- Articy integration
- Editor tools

## 📋 Package Development Workflow

### Phase 1: Planning (1-2 days)
- [ ] **Define Requirements**: What does this package do?
- [ ] **Platform Analysis**: How does it work on VR/Mobile/Web?
- [ ] **Articy Integration**: What Articy elements does it connect to?
- [ ] **Cross-Package Dependencies**: What other packages does it need?

### Phase 2: Core Implementation (3-5 days)
- [ ] **Create Package Structure**: Use the template generator
- [ ] **Implement Core Types**: Enums, interfaces, base classes
- [ ] **Build Manager**: Singleton pattern with platform detection
- [ ] **Create Object Components**: Main interactive elements

### Phase 3: Platform Handlers (2-3 days per platform)
- [ ] **VR Handler**: XR Interaction Toolkit integration
- [ ] **Mobile Handler**: Touch and gaze input
- [ ] **Web Handler**: Mouse and keyboard input
- [ ] **Cross-Platform Testing**: Verify all platforms work

### Phase 4: Integration & Polish (2-3 days)
- [ ] **Articy Integration**: Connect to Articy workflow
- [ ] **Editor Tools**: Setup wizard and custom inspectors
- [ ] **Debug Tools**: Debug UI and logging
- [ ] **Sample Content**: Example scenes and scripts

### Phase 5: Testing & Documentation (1-2 days)
- [ ] **Platform Testing**: Test on all target platforms
- [ ] **Performance Testing**: Memory and performance validation
- [ ] **Documentation**: README, API docs, examples
- [ ] **Code Review**: Team review and feedback

## 🏗 Architecture Deep Dive

### Core Components Every Package Must Have

#### 1. **Manager Pattern**
```csharp
public class [Feature]Manager : MonoBehaviour
{
    // Singleton pattern
    public static [Feature]Manager Instance { get; private set; }
    
    // Platform detection
    private InteractionPlatform _currentPlatform;
    
    // Object registration
    private List<[Feature]Object> _registeredObjects;
    
    // Platform handlers
    private Dictionary<InteractionPlatform, I[Feature]Handler> _handlers;
    
    // Core methods
    public void Initialize();
    public void Register[Feature]Object([Feature]Object obj);
    public void SwitchPlatform(InteractionPlatform platform);
}
```

#### 2. **Platform Handler Interface**
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

#### 3. **Object Component Pattern**
```csharp
public class [Feature]Object : MonoBehaviour
{
    // Configuration
    [SerializeField] private InteractionPlatform _supportedPlatforms;
    [SerializeField] private [Feature]InteractionType[] _allowedTypes;
    
    // State
    private [Feature]State _currentState;
    
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

## 🔧 Implementation Guidelines

### 1. **Namespace Convention**
```csharp
namespace OnVR.Warehouse.[Feature]
{
    // Main package namespace
}

namespace OnVR.Warehouse.[Feature].Core
{
    // Core types, interfaces, enums
}

namespace OnVR.Warehouse.[Feature].Platforms
{
    // Platform-specific handlers
}

namespace OnVR.Warehouse.[Feature].Integration
{
    // External integrations (Articy, etc.)
}
```

### 2. **Event System**
Use UnityEvents for inspector connections and C# events for code:
```csharp
// Inspector-friendly events
public UnityEvent On[Feature]Start;
public UnityEvent On[Feature]End;

// Code-friendly events
public event Action<[Feature]Object> [Feature]Started;
public event Action<[Feature]Object> [Feature]Ended;
```

### 3. **Platform Detection**
Always use the centralized platform detection:
```csharp
var platform = PlatformDetector.CurrentPlatform;
if (platform.HasFlag(InteractionPlatform.VR))
{
    // VR-specific logic
}
```

### 4. **Articy Integration**
Every package should support Articy integration:
```csharp
public class Articy[Feature]Integration : MonoBehaviour
{
    [SerializeField] private string _articyObjectId;
    [SerializeField] private string _locationId;
    [SerializeField] private bool _triggerFlowOn[Feature] = true;
    [SerializeField] private string _[feature]FlowFragment;
}
```

## 🎮 Platform-Specific Implementation

### VR Implementation
```csharp
public class VR[Feature]Handler : MonoBehaviour, I[Feature]Handler
{
    private XRInteractionManager _interactionManager;
    private List<IXRInteractor> _interactors;
    
    public void Initialize([Feature]Manager manager)
    {
        _interactionManager = FindObjectOfType<XRInteractionManager>();
        SetupInteractors();
    }
    
    private void SetupInteractors()
    {
        // Setup XR interactors for your feature
    }
}
```

### Mobile Implementation
```csharp
public class Mobile[Feature]Handler : MonoBehaviour, I[Feature]Handler
{
    private Camera _mainCamera;
    private InputAction _touchAction;
    
    public void Initialize([Feature]Manager manager)
    {
        _mainCamera = Camera.main;
        SetupInputActions();
    }
    
    private void SetupInputActions()
    {
        // Setup touch and gaze input
    }
}
```

### Web Implementation
```csharp
public class Web[Feature]Handler : MonoBehaviour, I[Feature]Handler
{
    private Camera _mainCamera;
    private InputAction _clickAction;
    
    public void Initialize([Feature]Manager manager)
    {
        _mainCamera = Camera.main;
        SetupInputActions();
    }
    
    private void SetupInputActions()
    {
        // Setup mouse and keyboard input
    }
}
```

## 🔗 Cross-Package Communication

### 1. **Event-Based Communication**
```csharp
// In your package
public static class [Feature]Events
{
    public static event Action<[Feature]Object> On[Feature]Started;
    public static event Action<[Feature]Object> On[Feature]Ended;
}

// In another package
void Start()
{
    [Feature]Events.On[Feature]Started += Handle[Feature]Started;
}
```

### 2. **Interface-Based Communication**
```csharp
public interface I[Feature]Provider
{
    bool Is[Feature]Active { get; }
    [Feature]Object Current[Feature]Object { get; }
}
```

### 3. **Configuration Sharing**
```csharp
[CreateAssetMenu(fileName = "[Feature]Config", menuName = "onVR/[Feature]/Configuration")]
public class [Feature]Configuration : ScriptableObject
{
    // Shared configuration
}
```

## 🛠 Editor Tools Development

### 1. **Setup Wizard**
```csharp
[MenuItem("onVR/[Feature]/Setup Wizard")]
public static void ShowWindow()
{
    var window = GetWindow<[Feature]SetupWizard>("[Feature] Setup");
    window.Show();
}
```

### 2. **Custom Inspector**
```csharp
[CustomEditor(typeof([Feature]Object))]
public class [Feature]ObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Custom inspector logic
    }
}
```

### 3. **Menu Items**
```csharp
[MenuItem("onVR/[Feature]/Add to Selected Objects")]
public static void Add[Feature]ToSelected()
{
    // Add component to selected objects
}
```

## 🧪 Testing Guidelines

### 1. **Unit Testing**
```csharp
[Test]
public void [Feature]Object_Can[Feature]_ReturnsTrue_WhenEnabled()
{
    // Arrange
    var obj = new GameObject().AddComponent<[Feature]Object>();
    obj.Is[Feature]Enabled = true;
    
    // Act
    bool result = obj.Can[Feature]([Feature]InteractionType.TapClick);
    
    // Assert
    Assert.IsTrue(result);
}
```

### 2. **Platform Testing**
- Test on VR headset (Quest, PC VR)
- Test on mobile device (Android, iOS)
- Test on web browser (Chrome, Firefox, Safari)
- Test on desktop (Windows, Mac, Linux)

### 3. **Performance Testing**
```csharp
// Monitor memory usage
var memoryBefore = GC.GetTotalMemory(false);
// Your feature code
var memoryAfter = GC.GetTotalMemory(false);
Debug.Log($"Memory used: {memoryAfter - memoryBefore} bytes");
```

## 📚 Documentation Requirements

### 1. **README.md Structure**
```markdown
# [Feature Name]

## Overview
Brief description of the feature

## Key Features
- Feature 1
- Feature 2

## Installation
Package Manager instructions

## Quick Start
Basic usage examples

## API Reference
Key classes and methods

## Examples
Code examples and samples

## Troubleshooting
Common issues and solutions
```

### 2. **Code Documentation**
```csharp
/// <summary>
/// Core component that makes any GameObject [feature]-capable
/// </summary>
/// <remarks>
/// This component handles [feature] interactions across all supported platforms.
/// It automatically detects the current platform and uses the appropriate handler.
/// </remarks>
/// <example>
/// <code>
/// var obj = gameObject.AddComponent<[Feature]Object>();
/// obj.SupportedPlatforms = InteractionPlatform.All;
/// </code>
/// </example>
public class [Feature]Object : MonoBehaviour
{
    // Implementation
}
```

## 🚀 Deployment Checklist

### Before Release
- [ ] All platforms tested and working
- [ ] Performance benchmarks met
- [ ] Memory usage optimized
- [ ] Documentation complete
- [ ] Sample content included
- [ ] Code review completed
- [ ] Changelog updated
- [ ] Version number updated

### Release Process
1. **Create Release Branch**: `git checkout -b release/v1.0.0`
2. **Update Version**: Update package.json version
3. **Update Changelog**: Add release notes
4. **Final Testing**: Test on all platforms
5. **Create Tag**: `git tag v1.0.0`
6. **Merge to Main**: `git checkout main && git merge release/v1.0.0`
7. **Push**: `git push origin main --tags`

## 🎯 Work Package Assignment Guide

### For Team Leads
1. **Assign Packages**: Based on developer expertise
2. **Set Milestones**: 2-week sprints recommended
3. **Review Progress**: Weekly check-ins
4. **Coordinate Dependencies**: Ensure packages work together

### For Developers
1. **Follow Template**: Use the package generator
2. **Implement Incrementally**: Core → Platforms → Integration → Polish
3. **Test Continuously**: Test on all platforms during development
4. **Document as You Go**: Keep documentation updated
5. **Ask for Help**: Use team chat for questions

### Package Dependencies
```
Core Packages (Build First):
├── com.onvr.warehouse.core
├── com.onvr.warehouse.grabinteraction ✅
└── com.onvr.warehouse.teleportation

Interaction Packages:
├── com.onvr.warehouse.place
├── com.onvr.warehouse.throw
├── com.onvr.warehouse.manipulate
└── com.onvr.warehouse.combine

UI Packages:
├── com.onvr.warehouse.decisionmenus
├── com.onvr.warehouse.smarthighlighting
└── com.onvr.warehouse.immunodex

System Packages:
├── com.onvr.warehouse.missionsystem
├── com.onvr.warehouse.aidialogue
└── com.onvr.warehouse.articyintegration
```

## 🆘 Getting Help

### Resources
- **Template Architecture**: `PACKAGE_TEMPLATE_ARCHITECTURE.md`
- **Reference Implementation**: Grab Interaction package
- **Team Chat**: Discord/Slack for questions
- **Code Reviews**: GitHub pull requests

### Common Issues
1. **Platform Detection**: Always use `PlatformDetector.CurrentPlatform`
2. **Event Handling**: Use both UnityEvents and C# events
3. **Memory Management**: Clean up event subscriptions in OnDestroy
4. **Performance**: Profile on target platforms regularly

### Support Channels
- **Technical Questions**: Team chat
- **Architecture Questions**: Lead developer
- **Platform Issues**: Platform-specific experts
- **Articy Integration**: Articy specialist

---

**Remember**: Consistency is key! Follow the template architecture closely to ensure all packages work together seamlessly. 