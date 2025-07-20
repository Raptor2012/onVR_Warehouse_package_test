using UnityEngine;
using UnityEditor;
using OnVR.Warehouse.GrabInteraction;

namespace OnVR.Warehouse.GrabInteraction.Editor
{
    /// <summary>
    /// Custom editor for GrabbableObject component
    /// </summary>
    [CustomEditor(typeof(GrabbableObject))]
    public class GrabbableObjectEditor : UnityEditor.Editor
    {
        private SerializedProperty _isGrabbable;
        private SerializedProperty _supportedPlatforms;
        private SerializedProperty _allowedInteractionTypes;
        
        private SerializedProperty _highlightMaterial;
        private SerializedProperty _highlightColor;
        private SerializedProperty _useOutlineEffect;
        
        private SerializedProperty _usePhysics;
        private SerializedProperty _freezeRotationWhenGrabbed;
        private SerializedProperty _grabForce;
        
        private SerializedProperty _maxGrabDistance;
        private SerializedProperty _enableRemoteGrab;

        private void OnEnable()
        {
            _isGrabbable = serializedObject.FindProperty("_isGrabbable");
            _supportedPlatforms = serializedObject.FindProperty("_supportedPlatforms");
            _allowedInteractionTypes = serializedObject.FindProperty("_allowedInteractionTypes");
            
            _highlightMaterial = serializedObject.FindProperty("_highlightMaterial");
            _highlightColor = serializedObject.FindProperty("_highlightColor");
            _useOutlineEffect = serializedObject.FindProperty("_useOutlineEffect");
            
            _usePhysics = serializedObject.FindProperty("_usePhysics");
            _freezeRotationWhenGrabbed = serializedObject.FindProperty("_freezeRotationWhenGrabbed");
            _grabForce = serializedObject.FindProperty("_grabForce");
            
            _maxGrabDistance = serializedObject.FindProperty("_maxGrabDistance");
            _enableRemoteGrab = serializedObject.FindProperty("_enableRemoteGrab");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var grabbableObject = (GrabbableObject)target;

            // Header
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("onVR Grab Interaction", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            // Current state info
            if (Application.isPlaying)
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField("Runtime Info", EditorStyles.boldLabel);
                EditorGUILayout.LabelField($"Current State: {grabbableObject.CurrentState}");
                if (grabbableObject.CurrentGrabber != null)
                {
                    EditorGUILayout.LabelField($"Grabbed by: {grabbableObject.CurrentGrabber.name}");
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
            }

            // Basic settings
            EditorGUILayout.PropertyField(_isGrabbable);
            EditorGUILayout.PropertyField(_supportedPlatforms);
            EditorGUILayout.PropertyField(_allowedInteractionTypes, true);

            EditorGUILayout.Space();

            // Visual feedback
            EditorGUILayout.LabelField("Visual Feedback", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_highlightMaterial);
            if (_highlightMaterial.objectReferenceValue == null)
            {
                EditorGUILayout.PropertyField(_highlightColor);
                EditorGUILayout.PropertyField(_useOutlineEffect);
            }

            EditorGUILayout.Space();

            // Physics settings
            EditorGUILayout.LabelField("Physics Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_usePhysics);
            if (_usePhysics.boolValue)
            {
                EditorGUILayout.PropertyField(_freezeRotationWhenGrabbed);
                EditorGUILayout.PropertyField(_grabForce);
            }

            EditorGUILayout.Space();

            // Distance settings
            EditorGUILayout.LabelField("Distance Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_maxGrabDistance);
            EditorGUILayout.PropertyField(_enableRemoteGrab);

            EditorGUILayout.Space();

            // Validation and helper buttons
            EditorGUILayout.LabelField("Validation", EditorStyles.boldLabel);
            
            // Check for required components
            var collider = grabbableObject.GetComponent<Collider>() ?? grabbableObject.GetComponentInChildren<Collider>();
            if (collider == null)
            {
                EditorGUILayout.HelpBox("No Collider found! Grab interaction requires a Collider component.", MessageType.Warning);
                if (GUILayout.Button("Add Box Collider"))
                {
                    grabbableObject.gameObject.AddComponent<BoxCollider>();
                }
            }

            var rigidbody = grabbableObject.GetComponent<Rigidbody>();
            if (_usePhysics.boolValue && rigidbody == null)
            {
                EditorGUILayout.HelpBox("Physics is enabled but no Rigidbody found. A Rigidbody will be added automatically at runtime.", MessageType.Info);
                if (GUILayout.Button("Add Rigidbody Now"))
                {
                    grabbableObject.gameObject.AddComponent<Rigidbody>();
                }
            }

            // Quick setup buttons
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Quick Setup", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Setup for VR"))
            {
                SetupForVR(grabbableObject);
            }
            if (GUILayout.Button("Setup for Mobile"))
            {
                SetupForMobile(grabbableObject);
            }
            EditorGUILayout.EndHorizontal();
            
            if (GUILayout.Button("Setup for All Platforms"))
            {
                SetupForAllPlatforms(grabbableObject);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void SetupForVR(GrabbableObject grabbable)
        {
            _supportedPlatforms.enumValueIndex = (int)Core.InteractionPlatform.VR;
            // Set VR-specific interaction types
            _allowedInteractionTypes.ClearArray();
            _allowedInteractionTypes.arraySize = 2;
            _allowedInteractionTypes.GetArrayElementAtIndex(0).enumValueIndex = (int)Core.GrabInteractionType.DirectHand;
            _allowedInteractionTypes.GetArrayElementAtIndex(1).enumValueIndex = (int)Core.GrabInteractionType.RemoteGrab;
            
            _usePhysics.boolValue = true;
            _enableRemoteGrab.boolValue = true;
            serializedObject.ApplyModifiedProperties();
        }

        private void SetupForMobile(GrabbableObject grabbable)
        {
            _supportedPlatforms.enumValueIndex = (int)Core.InteractionPlatform.Mobile;
            // Set mobile-specific interaction types
            _allowedInteractionTypes.ClearArray();
            _allowedInteractionTypes.arraySize = 3;
            _allowedInteractionTypes.GetArrayElementAtIndex(0).enumValueIndex = (int)Core.GrabInteractionType.TapClick;
            _allowedInteractionTypes.GetArrayElementAtIndex(1).enumValueIndex = (int)Core.GrabInteractionType.GazePick;
            _allowedInteractionTypes.GetArrayElementAtIndex(2).enumValueIndex = (int)Core.GrabInteractionType.Drag;
            
            _usePhysics.boolValue = false;
            _enableRemoteGrab.boolValue = false;
            serializedObject.ApplyModifiedProperties();
        }

        private void SetupForAllPlatforms(GrabbableObject grabbable)
        {
            _supportedPlatforms.enumValueIndex = (int)Core.InteractionPlatform.All;
            // Clear interaction types to use defaults
            _allowedInteractionTypes.ClearArray();
            
            _usePhysics.boolValue = true;
            _enableRemoteGrab.boolValue = true;
            serializedObject.ApplyModifiedProperties();
        }
    }
}
