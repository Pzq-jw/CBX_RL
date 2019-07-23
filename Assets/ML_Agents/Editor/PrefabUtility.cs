//Create a folder (right click in the Assets directory, click Create>New Folder) and name it “Editor” if one doesn’t exist already.
//Place this script in that folder

//This script creates a new menu and a new menu item in the Editor window
// Use the new menu item to create a Prefab at the given path. If a Prefab already exists it asks if you want to replace it
//Click on a GameObject in your Hierarchy, then go to Examples>Create Prefab to see it in action.

using UnityEngine;
using UnityEditor;

public class Example : EditorWindow
{
    static public float spawnPosOffsetX = 10;
    static public float spawnPosOffsetY = 0;
    static public float spawnPosOffsetZ = 10;
    static public int spawnNum = 24;
    static private Vector3 lastSpawnPos;
    static private GameObject prefab;


    [MenuItem("Examples/Instantiate Selected")]
    static void CreatePrefab()
    {

        lastSpawnPos = Vector3.zero;
        prefab = Selection.activeObject as GameObject;
        for(int i=0; i<spawnNum; i++)
        {
            GameObject clone = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            clone.gameObject.name = "ML_Area" + i.ToString();
            clone.transform.position = GetSpawnPos(lastSpawnPos);
        }
    }

    [MenuItem("Examples/Instantiate Selected", true)]

    static bool ValidateCreatePrefab()
    {
        GameObject go = Selection.activeObject as  GameObject;
        if (go == null)
            return false;

        return PrefabUtility.GetPrefabType(go) == PrefabType.Prefab || PrefabUtility.GetPrefabType(go) == PrefabType.ModelPrefab;
    }

    static Vector3 GetSpawnPos(Vector3 lastAreaPos)
    {
        Vector3 newSpawnPos = new Vector3(
                                lastAreaPos.x + spawnPosOffsetX,
                                lastAreaPos.y + spawnPosOffsetY,
                                lastAreaPos.z + spawnPosOffsetZ);
        lastSpawnPos = newSpawnPos;
        return newSpawnPos;

    }
}