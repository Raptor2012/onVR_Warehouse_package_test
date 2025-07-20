using System.Collections.Generic;
using Articy.Unity;
using Articy.Unity.Interfaces;
using Articy.Unity.Utils;
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Articy.Vanflyta; 
using Unity.VisualScripting;
using System.Text.RegularExpressions;

[RequireComponent(typeof(ArticyFlowPlayer))]
public class onVRArticyFlowController : MonoBehaviour, IArticyFlowPlayerCallbacks
{
    private ArticyFlowPlayer flowPlayer;

    [SerializeField]
    private AudioSource dialogueAudioSource;
    
    [SerializeField] private GameObject ovrCameraRig; // Assign your OVR camera rig here

    void Start()
    {
        flowPlayer = GetComponent<ArticyFlowPlayer>();
		Debug.Assert(flowPlayer != null, "ArticyDebugFlowPlayer needs the ArticyFlowPlayer component!.");
    }
    public void OnBranchesUpdated(IList<Branch> aBranches)
    {
        // Implement as needed
    }

    public void OnFlowPlayerPaused(IFlowObject aObject)
    {
        StartCoroutine(HandleFlowObject(aObject));
    }

    private IEnumerator HandleFlowObject(IFlowObject aObject)
    {
        //1. Handle instructions/comments via Expression
        Instruction articyObj = aObject as Instruction;
        if (articyObj != null)
        {   
            Debug.Log($"Processing instruction: {articyObj.TechnicalName}");

            // Get the description text
            string description = articyObj.Text; // Try .Text instead of .DisplayText
            Debug.Log($"Instruction Description: {description}");
            if (description.StartsWith("JumpToHotspot"))
            {
                // Extract the value inside the single quotes
                var match = Regex.Match(description, @"JumpToHotspot\('([^']+)'\)");
                string spotName = match.Success ? match.Groups[1].Value : "";
                Debug.Log(spotName + " this is the spot name");
                TeleportToSpot(spotName);
                yield return null; // Wait a frame for teleport
                flowPlayer.Play();
                yield break;
            }
            
        }

        // 2. Handle dialogue with VO using LocalizeWithVO
        DialogueFragment articyDialogueObj = aObject as DialogueFragment;
        if (articyDialogueObj != null)
        {
            string localizationKey = articyDialogueObj.TechnicalName;
            string localizedText = articyDialogueObj.Text;

            AudioClip voClip = articyDialogueObj.Text.LoadVOAssetAsAudioClip();
            if (voClip != null)
            {
                dialogueAudioSource.clip = voClip;
                dialogueAudioSource.Play();
                yield return new WaitWhile(() => dialogueAudioSource.isPlaying);
            }
            flowPlayer.Play();
            yield break;
        }

        // Default: just continue
        flowPlayer.Play();
    }

    private void TeleportToSpot(string spotName)
    {
        var spot = GameObject.Find(spotName);
        if (spot != null && ovrCameraRig != null)
        {
            ovrCameraRig.transform.position = spot.transform.position;
            ovrCameraRig.transform.rotation = spot.transform.rotation;
        }
        else
        {
            Debug.LogWarning($"Spot '{spotName}' or OVR Camera Rig not found!");
        }
    }
}
