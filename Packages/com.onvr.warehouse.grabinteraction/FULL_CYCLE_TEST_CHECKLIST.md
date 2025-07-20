# Full Cycle Test Checklist - onVR Grab Interaction Package

## 🎯 Test Objective
Validate that the onVR Grab Interaction package works correctly when installed from GitHub into a fresh Unity project.

## 📋 Pre-Test Setup

### 1. **Prepare GitHub Repository**
- [ ] Create new GitHub repository: `onvr-warehouse-grabinteraction`
- [ ] Upload the complete package folder
- [ ] Ensure all files are included:
  - `package.json`
  - `README.md`
  - `CHANGELOG.md`
  - `LICENSE.md`
  - `Runtime/` folder
  - `Editor/` folder
  - `Samples~/` folder
- [ ] Test repository is publicly accessible

### 2. **Prepare Test Environment**
- [ ] Install Unity 2021.3 LTS or later
- [ ] Create a new Unity project
- [ ] Ensure no existing packages that might conflict

## 🚀 Installation Test

### 3. **Package Installation**
- [ ] Open Package Manager (Window > Package Manager)
- [ ] Click **+** button → "Add package from git URL"
- [ ] Enter GitHub URL: `https://github.com/[username]/onvr-warehouse-grabinteraction.git`
- [ ] Click "Add"
- [ ] Verify package appears in Package Manager
- [ ] Check no installation errors in console

### 4. **Dependency Verification**
- [ ] Verify XR Interaction Toolkit 2.3.0+ is installed
- [ ] Verify Input System 1.4.0+ is installed
- [ ] Verify XR Core Utils 2.2.0+ is installed
- [ ] Check all dependencies are compatible

## 🛠️ Setup Wizard Test

### 5. **Setup Wizard Access**
- [ ] Navigate to `onVR > Grab Interaction > Setup Wizard`
- [ ] Verify wizard window opens
- [ ] Check all UI elements are visible and functional

### 6. **Platform Detection Test**
- [ ] Check "Detected Platform" shows correct platform
- [ ] Test "Refresh Platform Detection" button
- [ ] Verify platform changes when switching build target:
  - Android → Mobile
  - WebGL → Web
  - Standalone → Desktop
- [ ] Test platform detection in different Unity versions

### 7. **Setup Wizard Functionality**
- [ ] Test "Complete Setup" button
- [ ] Verify InteractionManager is created
- [ ] Check all platform handlers are added
- [ ] Test "Setup Manager Only" button
- [ ] Test "Add Sample Objects" button
- [ ] Test "Convert Selected Objects to Grabbable" button

## 🎮 Functionality Test

### 8. **Core Components Test**
- [ ] Verify `InteractionManager` component exists
- [ ] Check `GrabbableObject` component can be added
- [ ] Test `PlatformDetector` returns correct platform
- [ ] Verify all platform handlers are present

### 9. **Sample Scene Test**
- [ ] Open sample scene (if available)
- [ ] Verify scene loads without errors
- [ ] Check sample objects are present
- [ ] Test grab interactions work
- [ ] Verify visual feedback (highlighting, outlines)

### 10. **Manual Setup Test**
- [ ] Create empty scene
- [ ] Add `TestSceneSetup` component to empty GameObject
- [ ] Run "Setup Test Scene" from context menu
- [ ] Verify all components are created correctly
- [ ] Test grab interactions on created objects

## 🔧 Editor Tools Test

### 11. **Menu Items Test**
- [ ] Test `onVR > Grab Interaction > Add to Selected Objects`
- [ ] Test `onVR > Grab Interaction > Create Interaction Manager`
- [ ] Test `GameObject > onVR > Grabbable Object`
- [ ] Verify all menu items work correctly

### 12. **Custom Inspector Test**
- [ ] Select a GameObject with `GrabbableObject` component
- [ ] Verify custom inspector displays correctly
- [ ] Test all inspector fields are functional
- [ ] Check platform-specific settings work

### 13. **Debug Tools Test**
- [ ] Add `GrabInteractionDebugUI` component
- [ ] Verify debug information displays
- [ ] Test debug logging functionality
- [ ] Check debug UI doesn't interfere with normal operation

## 🎯 Platform-Specific Tests

### 14. **VR Platform Test** (if VR hardware available)
- [ ] Set build target to Android (Quest) or Standalone (PC VR)
- [ ] Verify VR platform is detected
- [ ] Test VR interaction handlers
- [ ] Check XR Integration Toolkit compatibility
- [ ] Test haptic feedback (if supported)

### 15. **Mobile Platform Test**
- [ ] Set build target to Android or iOS
- [ ] Verify Mobile platform is detected
- [ ] Test touch input handling
- [ ] Check gaze interaction functionality
- [ ] Verify mobile-specific optimizations

### 16. **Web Platform Test**
- [ ] Set build target to WebGL
- [ ] Verify Web platform is detected
- [ ] Test mouse input handling
- [ ] Check keyboard shortcuts
- [ ] Verify WebGL compatibility

### 17. **Desktop Platform Test**
- [ ] Set build target to Standalone
- [ ] Verify Desktop platform is detected
- [ ] Test mouse and keyboard input
- [ ] Check desktop-specific features

## 🔗 Integration Tests

### 18. **Articy Integration Test** (if Articy available)
- [ ] Add `ArticyIntegration` component to grabbable object
- [ ] Verify Articy fields are configurable
- [ ] Test Articy object ID assignment
- [ ] Check location ID functionality
- [ ] Verify flow trigger settings

### 19. **Cross-Package Communication Test**
- [ ] Test event system works correctly
- [ ] Verify UnityEvents fire properly
- [ ] Check C# events work as expected
- [ ] Test event cleanup on object destruction

## 📊 Performance Tests

### 20. **Memory Usage Test**
- [ ] Monitor memory usage during package installation
- [ ] Check memory usage with multiple grabbable objects
- [ ] Verify no memory leaks during runtime
- [ ] Test garbage collection behavior

### 21. **Performance Test**
- [ ] Test with 10+ grabbable objects
- [ ] Verify frame rate remains stable
- [ ] Check CPU usage is reasonable
- [ ] Test performance on target platforms

## 🐛 Error Handling Tests

### 22. **Error Scenarios Test**
- [ ] Test behavior when InteractionManager is missing
- [ ] Verify graceful handling of missing colliders
- [ ] Test behavior with invalid platform settings
- [ ] Check error messages are helpful

### 23. **Edge Cases Test**
- [ ] Test with objects without renderers
- [ ] Verify behavior with disabled GameObjects
- [ ] Test with objects outside camera view
- [ ] Check behavior with destroyed objects

## 📚 Documentation Tests

### 24. **Documentation Verification**
- [ ] Verify README.md is accessible and complete
- [ ] Check installation instructions are accurate
- [ ] Test all code examples work
- [ ] Verify troubleshooting section is helpful

### 25. **API Documentation Test**
- [ ] Check all public APIs are documented
- [ ] Verify code comments are clear
- [ ] Test IntelliSense works correctly
- [ ] Check documentation matches implementation

## 🔄 Regression Tests

### 26. **Reinstallation Test**
- [ ] Remove package completely
- [ ] Reinstall from GitHub
- [ ] Verify all functionality still works
- [ ] Check no leftover files or references

### 27. **Version Compatibility Test**
- [ ] Test with Unity 2021.3 LTS
- [ ] Test with Unity 2022.3 LTS
- [ ] Test with Unity 2023.x (if available)
- [ ] Verify compatibility across versions

## 📝 Test Results Documentation

### 28. **Test Report**
- [ ] Document all test results
- [ ] Note any issues or bugs found
- [ ] Record performance metrics
- [ ] Document platform-specific findings

### 29. **Issue Tracking**
- [ ] Create GitHub issues for any bugs found
- [ ] Document steps to reproduce issues
- [ ] Prioritize issues by severity
- [ ] Plan fixes for critical issues

## ✅ Success Criteria

The full cycle test is successful if:

1. **Installation**: Package installs without errors from GitHub
2. **Setup**: Setup wizard works correctly and creates functional scene
3. **Functionality**: All grab interactions work on target platforms
4. **Performance**: No significant performance impact
5. **Documentation**: All documentation is accurate and helpful
6. **Compatibility**: Works across different Unity versions and platforms

## 🚨 Critical Issues (Must Fix)

- [ ] Package fails to install from GitHub
- [ ] Setup wizard doesn't work
- [ ] Grab interactions don't function
- [ ] Platform detection fails
- [ ] Dependencies not installed correctly
- [ ] Memory leaks or performance issues

## 🔧 Minor Issues (Should Fix)

- [ ] UI layout issues in setup wizard
- [ ] Missing documentation
- [ ] Minor performance optimizations
- [ ] Additional error handling
- [ ] Enhanced debug information

## 📋 Test Execution Log

**Test Date**: _______________
**Unity Version**: _______________
**Tester**: _______________
**GitHub Repository**: _______________

### Test Results Summary
- [ ] All critical tests passed
- [ ] All minor issues documented
- [ ] Performance benchmarks met
- [ ] Documentation verified
- [ ] Ready for production use

### Notes
```
[Add any additional notes, observations, or recommendations here]
```

---

**Test completed by**: _______________
**Date**: _______________
**Signature**: _______________ 