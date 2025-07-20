# GitHub Setup Guide - Full Cycle Test

## 🚀 Quick Setup for GitHub Testing

### Step 1: Create GitHub Repository

1. **Go to GitHub.com** and sign in
2. **Create New Repository**:
   - Repository name: `onvr-warehouse-grabinteraction`
   - Description: `onVR Warehouse - Grab Interaction Package`
   - Make it **Public**
   - Don't initialize with README (we have our own)
3. **Copy the repository URL**: `https://github.com/[your-username]/onvr-warehouse-grabinteraction.git`

### Step 2: Upload Package Files

#### Option A: Using GitHub Web Interface
1. Go to your new repository
2. Click **"Add file"** → **"Upload files"**
3. Drag and drop the entire package folder contents:
   ```
   ├── package.json
   ├── README.md
   ├── CHANGELOG.md
   ├── LICENSE.md
   ├── PACKAGE_TEMPLATE_ARCHITECTURE.md
   ├── DEVELOPMENT_GUIDE.md
   ├── FULL_CYCLE_TEST_CHECKLIST.md
   ├── GITHUB_SETUP_GUIDE.md
   ├── Runtime/
   │   ├── com.onvr.warehouse.grabinteraction.asmdef
   │   ├── Scripts/
   │   │   ├── Core/
   │   │   ├── Platforms/
   │   │   └── Integration/
   │   └── [other runtime files]
   ├── Editor/
   │   ├── com.onvr.warehouse.grabinteraction.editor.asmdef
   │   └── [editor scripts]
   └── Samples~/
       └── BasicGrabInteraction/
   ```
4. Add commit message: `"Initial package upload"`
5. Click **"Commit changes"**

#### Option B: Using Git Command Line
```bash
# Clone the repository
git clone https://github.com/[your-username]/onvr-warehouse-grabinteraction.git
cd onvr-warehouse-grabinteraction

# Copy package files
cp -r "d:\OnlivelineProject\onVR Warehouse\Packages\com.onvr.warehouse.grabinteraction\*" .

# Add all files
git add .

# Commit
git commit -m "Initial package upload"

# Push
git push origin main
```

### Step 3: Verify Repository Structure

Your repository should look like this:
```
onvr-warehouse-grabinteraction/
├── package.json
├── README.md
├── CHANGELOG.md
├── LICENSE.md
├── PACKAGE_TEMPLATE_ARCHITECTURE.md
├── DEVELOPMENT_GUIDE.md
├── FULL_CYCLE_TEST_CHECKLIST.md
├── GITHUB_SETUP_GUIDE.md
├── Runtime/
│   ├── com.onvr.warehouse.grabinteraction.asmdef
│   ├── Scripts/
│   │   ├── Core/
│   │   │   ├── GrabbableObject.cs
│   │   │   ├── InteractionManager.cs
│   │   │   ├── InteractionTypes.cs
│   │   │   └── PlatformDetector.cs
│   │   ├── Platforms/
│   │   │   ├── VRInteractionHandler.cs
│   │   │   ├── MobileInteractionHandler.cs
│   │   │   └── WebInteractionHandler.cs
│   │   └── Integration/
│   │       └── ArticyIntegration.cs
│   ├── GrabInteractionDebugUI.cs
│   ├── TestGrabbableCube.cs
│   └── TestSceneSetup.cs
├── Editor/
│   ├── com.onvr.warehouse.grabinteraction.editor.asmdef
│   ├── GrabInteractionSetupWizard.cs
│   ├── GrabbableObjectEditor.cs
│   └── PackageTemplateGenerator.cs
└── Samples~/
    └── BasicGrabInteraction/
        ├── README.md
        └── Scripts/
            └── TestSceneSetup.cs
```

## 🧪 Full Cycle Test Instructions

### Step 4: Create Test Unity Project

1. **Open Unity Hub**
2. **Create New Project**:
   - Template: 3D (URP) or 3D Core
   - Unity Version: 2021.3 LTS or later
   - Project Name: `onVR-GrabInteraction-Test`
3. **Wait for project to open**

### Step 5: Install Package from GitHub

1. **Open Package Manager**: Window > Package Manager
2. **Add Package from Git URL**:
   - Click **+** button in top-left
   - Select **"Add package from git URL"**
   - Enter: `https://github.com/[your-username]/onvr-warehouse-grabinteraction.git`
   - Click **"Add"**
3. **Wait for installation** (may take a few minutes)
4. **Check console** for any errors

### Step 6: Test Package Installation

1. **Verify Package Appears**:
   - In Package Manager, look for "onVR Warehouse - Grab Interaction"
   - Check version shows "1.0.0"
   - Verify dependencies are installed

2. **Check Dependencies**:
   - XR Interaction Toolkit 2.3.0+
   - Input System 1.4.0+
   - XR Core Utils 2.2.0+

### Step 7: Test Setup Wizard

1. **Open Setup Wizard**: `onVR > Grab Interaction > Setup Wizard`
2. **Test Platform Detection**:
   - Check "Detected Platform" shows correct platform
   - Click "Refresh Platform Detection"
   - Verify platform changes when switching build target

3. **Test Setup Functions**:
   - Click "Complete Setup"
   - Verify InteractionManager is created
   - Check all platform handlers are added

### Step 8: Test Core Functionality

1. **Create Test Scene**:
   - Create empty scene
   - Add `TestSceneSetup` component to empty GameObject
   - Right-click component → "Setup Test Scene"

2. **Verify Components Created**:
   - InteractionManager in scene
   - Sample grabbable objects (cube, sphere, cylinder)
   - Proper lighting and ground plane

3. **Test Grab Interactions**:
   - Enter Play mode
   - Try to grab objects (click/tap/touch depending on platform)
   - Verify visual feedback (highlighting, outlines)
   - Check console for debug information

### Step 9: Test Platform-Specific Features

#### For Desktop/Web:
- Click objects to grab
- Drag to move objects
- Check keyboard shortcuts (G for grab, R for release)

#### For Mobile:
- Tap objects to grab
- Touch and drag to move
- Test gaze interaction if available

#### For VR (if hardware available):
- Set build target to Android (Quest) or Standalone (PC VR)
- Test hand interactions
- Verify haptic feedback

### Step 10: Test Editor Tools

1. **Menu Items**:
   - Test `onVR > Grab Interaction > Add to Selected Objects`
   - Test `onVR > Grab Interaction > Create Interaction Manager`
   - Test `GameObject > onVR > Grabbable Object`

2. **Custom Inspector**:
   - Select a grabbable object
   - Verify custom inspector displays
   - Test all configuration options

## ✅ Success Criteria

The test is successful if:

- [ ] Package installs without errors
- [ ] Setup wizard works correctly
- [ ] Platform detection shows correct platform
- [ ] Sample objects can be created and grabbed
- [ ] Visual feedback works (highlighting, outlines)
- [ ] No console errors or warnings
- [ ] All dependencies installed correctly
- [ ] Editor tools function properly

## 🐛 Common Issues & Solutions

### Issue: Package fails to install
**Solution**: Check GitHub URL is correct and repository is public

### Issue: Dependencies not found
**Solution**: Manually install XR Interaction Toolkit, Input System, XR Core Utils

### Issue: Setup wizard doesn't open
**Solution**: Check console for compilation errors, ensure all scripts compile

### Issue: Platform detection always shows Desktop
**Solution**: Check Unity build target, use "Refresh Platform Detection" button

### Issue: Objects not grabbable
**Solution**: Ensure InteractionManager exists, check colliders on objects

## 📝 Test Report Template

```
Test Date: [Date]
Unity Version: [Version]
GitHub Repository: [URL]
Tester: [Name]

Installation: ✅/❌
Setup Wizard: ✅/❌
Platform Detection: ✅/❌
Grab Functionality: ✅/❌
Visual Feedback: ✅/❌
Editor Tools: ✅/❌
Dependencies: ✅/❌

Issues Found:
- [List any issues]

Performance:
- Memory Usage: [MB]
- Frame Rate: [FPS]
- CPU Usage: [%]

Recommendations:
- [Any recommendations for improvement]

Overall Result: ✅ PASS / ❌ FAIL
```

## 🚀 Next Steps After Successful Test

1. **Document Results**: Fill out the test report
2. **Fix Issues**: Address any problems found
3. **Update Repository**: Push fixes to GitHub
4. **Share with Team**: Use as reference for other packages
5. **Plan Next Package**: Use template for teleportation package

---

**Good luck with the full cycle test!** 🎯 