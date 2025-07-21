# onVR Warehouse - Grab Interaction System

A modular grab interaction system for VR, Web, and Mobile platforms with **Articy integration** for narrative-driven interactions.

## ✨ Features

- **Cross-Platform Support**: VR, Web, and Mobile
- **Multiple Interaction Types**: Direct hand grabbing, remote grabbing, tap/click, gaze-pick
- **Articy Integration**: Connect grabbable objects to Articy flow templates for narrative interactions
- **Modular Architecture**: Easy to extend and customize
- **Event-Driven**: Rich event system for integration with your game logic

## 📦 Installation

### Via Unity Package Manager (Git URL)

1. Open Unity Package Manager (`Window > Package Manager`)
2. Click the `+` button and select `Add package from git URL`
3. Enter: `https://github.com/Raptor2012/onVR_Warehouse_package_test.git?path=/Packages/com.onvr.warehouse.grabinteraction`

### Via manifest.json

Add this to your `Packages/manifest.json`:

```json
{
  "dependencies": {
    "com.onvr.warehouse.grabinteraction": "https://github.com/Raptor2012/onVR_Warehouse_package_test.git?path=/Packages/com.onvr.warehouse.grabinteraction"
  }
}
```

## 🔧 Requirements

- Unity 2021.3 or later
- XR Interaction Toolkit 2.3.0+
- Input System 1.4.0+
- XR Core Utils 2.2.0+

## 🚀 Quick Start

### Basic Grabbable Object

1. Add a `GrabbableObject` component to any GameObject
2. Configure interaction settings in the inspector
3. Subscribe to grab events in your scripts:

```csharp
using OnVR.Warehouse.GrabInteraction.Core;

public class MyScript : MonoBehaviour
{
    void Start()
    {
        var grabbable = GetComponent<GrabbableObject>();
        grabbable.GrabStarted += OnGrabStarted;
        grabbable.GrabEnded += OnGrabEnded;
    }
    
    private void OnGrabStarted(GrabbableObject obj) 
    {
        Debug.Log("Object grabbed!");
    }
    
    private void OnGrabEnded(GrabbableObject obj) 
    {
        Debug.Log("Object released!");
    }
}
```

### Articy Integration

1. Add an `ArticyIntegration` component alongside `GrabbableObject`
2. Assign your Articy template reference
3. Configure flow triggering options:

```csharp
using OnVR.Warehouse.GrabInteraction.Integration;

public class MyArticyHandler : MonoBehaviour
{
    void Start()
    {
        var articyIntegration = GetComponent<ArticyIntegration>();
        articyIntegration.ArticyFlowTriggered += OnFlowTriggered;
        
        // Manually trigger a flow
        articyIntegration.PlayFlow(myFlowFragment);
    }
    
    private void OnFlowTriggered(ArticyRef flowRef)
    {
        Debug.Log($"Articy flow triggered: {flowRef}");
    }
}
```

## 📚 Core Components

### GrabbableObject
The main component that makes objects grabbable across different platforms.

### ArticyIntegration  
Connects grabbable objects to Articy flow templates for narrative interactions.

### InteractionManager
Centralized manager for handling different interaction types and input methods.

## 🎮 Supported Platforms

- **VR**: Hand tracking, controller grabbing
- **Desktop**: Mouse click and drag
- **Mobile**: Touch interactions
- **Web**: WebXR and fallback mouse controls

## 📖 Documentation

For detailed documentation and advanced usage, visit: [Documentation Link]

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Submit a pull request

## 📄 License

MIT License - see LICENSE file for details

## 🆘 Support

- **Issues**: [GitHub Issues](https://github.com/Raptor2012/onVR_Warehouse_package_test/issues)
- **Email**: info@onxt.de
- **Website**: https://onxt.de

---

Made with ❤️ by [onliveline.nxt GmbH](https://onxt.de)