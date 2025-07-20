using UnityEngine;
using UnityEngine.SceneManagement;


public class PortalTeleport : MonoBehaviour
{
    public string targetSceneName; // The name of the scene to teleport to
   // public string targetSpotNameForEntry;
    
    public void TeleportToScene()
    {
        SceneManager.LoadScene(targetSceneName);
    }
}
