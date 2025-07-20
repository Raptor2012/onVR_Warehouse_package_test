# Changelog

All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.1.0] - 2025-01-XX

### Added
- **Articy Integration**: Complete integration with Articy Unity importer
  - `ArticyIntegration` component for connecting grabbable objects to Articy flow templates
  - Support for triggering Articy flows on grab/release events
  - Location-based object management via Articy
  - Template-driven grabbable property control
- **ArticyIntegrationUtilities**: Static utility class for batch operations
  - Find objects by location or template type
  - Set grabbable state for multiple objects
  - Trigger flows across object groups
- **Enhanced Event System**: 
  - `ArticyFlowTriggered` event for flow execution notifications
  - `ArticyPropertyChanged` event for template property updates

### Changed
- Updated package description to include Articy integration features
- Added "Articy" and "Narrative" keywords for better discoverability
- Bumped version to 1.1.0 to reflect new major features

### Fixed
- Resolved compilation errors with Articy Unity importer integration
- Fixed ArticyRef to Branch conversion issues in flow triggering
- Improved pattern matching compatibility with ArticyObject types

## [1.0.0] - 2025-01-XX

### Added
- Initial release of the onVR Warehouse Grab Interaction System
- **Core Components**:
  - `GrabbableObject`: Main component for making objects interactive
  - `InteractionManager`: Centralized management system
  - Platform-specific interaction handlers
- **Cross-Platform Support**:
  - VR: Hand tracking and controller-based grabbing
  - Mobile: Touch and gaze-pick interactions  
  - Desktop: Mouse click and drag
  - Web: WebXR and fallback mouse controls
- **Modular Architecture**: Easy to extend and customize
- **Event-Driven System**: Rich events for grab start/end, highlight, etc.
- **Performance Optimized**: Efficient platform detection and interaction handling
- **XR Interaction Toolkit Integration**: Full compatibility with Unity's XR system

### Dependencies
- Unity XR Interaction Toolkit 2.3.0+
- Unity Input System 1.4.0+
- Unity XR Core Utils 2.2.0+

---

## Version Schema

- **Major** (X.y.z): Breaking changes or major new features
- **Minor** (x.Y.z): New features, backward compatible
- **Patch** (x.y.Z): Bug fixes, backward compatible
