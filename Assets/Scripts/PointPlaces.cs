using Articy.Vanflyta; 
using Articy.Unity;
using UnityEditor;
using UnityEngine;  

public class PlacePoints : MonoBehaviour
{
    public ArticyRef Location;
    public ArticyRef FloorPlan;
    public GameObject teleportHotspotPrefab;
    public GameObject portalPrefab;

    [ContextMenu("Place All Points")]
    public void PlaceAllPoints()
    {
        Debug.Log("floor plan");
        LocationImage plan = FloorPlan.GetObject<LocationImage>();
        var translation = plan.Transform.Translation;
        Debug.Log(translation);
        Debug.Log(plan.CachedImageHeight);
        Debug.Log(plan.CachedImageWidth);
        float midX = translation.x + plan.CachedImageWidth / 2;
        float midY = translation.y + plan.CachedImageWidth / 2;
        Debug.Log(midX);
        Debug.Log(midY);

        Location location = Location.GetObject<Location>();
        foreach (var aObject in location.Children)
        {
            if (aObject.TechnicalName.StartsWith("Spt"))
            {
                Spot spot = aObject as Spot;
                Debug.Log(spot);
                var spotX = spot.Position.x;
                var spotY = spot.Position.y;
                Debug.Log("spotX,spotY: " + spotX.ToString() + "," + spotY.ToString());

                var spotXCenter = spotX - midX;
                var spotYCenter = spotY - midY;

                var spotXNormalized = spotXCenter / plan.CachedImageWidth;
                var spotYNormalized = -spotYCenter / plan.CachedImageHeight;
                Debug.Log(spotXNormalized);
                Debug.Log(spotYNormalized);

                var bounds = GetBounds();
                float unity_center_x = bounds.center.x;
                float unity_center_z = bounds.center.z;
                float worldX = unity_center_x + spotXNormalized * bounds.size.x;
                float worldZ = unity_center_z + spotYNormalized * bounds.size.z;
                GameObject prefab = teleportHotspotPrefab;
                GameObject spawnedObject = Instantiate(prefab, new Vector3(worldX, 0f, worldZ), Quaternion.identity);
                spawnedObject.name = spot.TechnicalName;
            }
            else if (aObject.TechnicalName.StartsWith("Lnk"))
            {
                Link link = aObject as Link;
                Debug.Log($"Processing link: {link.TechnicalName}");
                var linkX = link.Position.x;
                var linkY = link.Position.y;

                var linkXCenter = linkX - midX;
                var linkYCenter = linkY - midY;

                var linkXNormalized = linkXCenter / plan.CachedImageWidth;
                var linkYNormalized = -linkYCenter / plan.CachedImageHeight;

                var bounds = GetBounds();
                float unity_center_x = bounds.center.x;
                float unity_center_z = bounds.center.z;
                float worldX = unity_center_x + linkXNormalized * bounds.size.x;
                float worldZ = unity_center_z + linkYNormalized * bounds.size.z;

                GameObject entityPrefab = null;

                // Get the target entity from the link                
                if (link.Target != null)
                {
                    Entity targetEntity = link.Target as Entity;
                    if (targetEntity != null)
                    {
                        Debug.Log($"Found target entity: {targetEntity.DisplayName}");

                        // Check if the entity has any attachments (3D models)
                        if (targetEntity.Attachments != null && targetEntity.Attachments.Count > 0)
                        {
                            // Get the first attachment (the GLB model)
                            var assetRef = targetEntity.Attachments[0];
                            if (assetRef != null)
                            {
                                string modelName = targetEntity.DisplayName;
                                Debug.Log($"Looking for model: {modelName}");
                                if (entityPrefab == null)
                                {
#if UNITY_EDITOR
                                    // Search for the model asset anywhere in the project by name (without extension)
                                    string[] guids = UnityEditor.AssetDatabase.FindAssets($"{modelName} t:GameObject");
                                    if (guids.Length == 0)
                                    {
                                        // Try with underscores if spaces didn't work
                                        string formattedName = modelName.Replace(" ", "_");
                                        guids = UnityEditor.AssetDatabase.FindAssets($"{formattedName} t:GameObject");
                                    }
                                    if (guids.Length > 0)
                                    {
                                        string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
                                        entityPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                                        Debug.Log($"Loaded entity prefab from: {assetPath}");
                                    }
                                    else
                                    {
                                        Debug.LogWarning($"Could not find prefab for {modelName} anywhere in the project.");
                                    }
#endif
                                }
                                // Spawn the prefab or fallback to UpPrefab
                                if (entityPrefab != null)
                                {
                                    Debug.Log($"Spawning entity prefab at {worldX}, {worldZ}");
                                    GameObject spawnedObject = Instantiate(entityPrefab, new Vector3(worldX, 0f, worldZ), Quaternion.identity);
                                    spawnedObject.name = targetEntity.DisplayName;
                                }
                                else
                                {
                                    Debug.Log("No entity prefab found, using UpPrefab as fallback");
                                    GameObject spawnedObject = Instantiate(teleportHotspotPrefab, new Vector3(worldX, 0f, worldZ), Quaternion.identity);
                                    spawnedObject.name = targetEntity.DisplayName;
                                }
                            }
                        }
                    }

                    else
                    {
                        Spot targetSpot = link.Target as Spot;
                        if (targetSpot != null)
                        {
                            // Handle teleportation to the target spot
                            Instantiate(portalPrefab, new Vector3(worldX, 0f, worldZ), Quaternion.identity).name = link.TechnicalName;
                         //   
                            Debug.Log($"Linking portal to target spot: {targetSpot.TechnicalName}");

                        }
                    }


                }
            }

            // After handling spots and links
            foreach (var entity in location.Children)
            {
                // Check if this is an entity you want to spawn (adjust the condition as needed)
                // Replace 'Entity' with the actual Articy type for your entities if different
                if (entity is Entity articyEntity)
                {
                    // Get the prefab by name (assuming prefab name matches entity name)
                    string prefabName = articyEntity.DisplayName;
                    GameObject entityPrefab = Resources.Load<GameObject>(prefabName);
                    if (entityPrefab == null)
                    {
                        Debug.LogWarning($"Prefab for entity '{prefabName}' not found in Resources.");
                        continue;
                    }

                    // Use entity.Position.x/y for placement
                    var entityX = articyEntity.Position.x;
                    var entityY = articyEntity.Position.y;

                    var entityXCenter = entityX - midX;
                    var entityYCenter = entityY - midY;

                    var entityXNormalized = entityXCenter / plan.CachedImageWidth;
                    var entityYNormalized = -entityYCenter / plan.CachedImageHeight;

                    var bounds = GetBounds();
                    float unity_center_x = bounds.center.x;
                    float unity_center_z = bounds.center.z;
                    float worldX = unity_center_x + entityXNormalized * bounds.size.x;
                    float worldZ = unity_center_z + entityYNormalized * bounds.size.z;

                    GameObject spawnedObject = Instantiate(entityPrefab, new Vector3(worldX, 0f, worldZ), Quaternion.identity);
                    spawnedObject.name = articyEntity.TechnicalName;
                }
            }
        }
    }
    public Bounds GetBounds()
    {
        GameObject obj = gameObject;
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
            return new Bounds(obj.transform.position, Vector3.zero);

        Bounds bounds = renderers[0].bounds;
        foreach (Renderer r in renderers)
        {
            bounds.Encapsulate(r.bounds);
        }
        return bounds;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
