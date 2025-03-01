using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Game.Editor
{
    public class LevelCleanup
    {
        [MenuItem("Level/PrefabifyGround")]
        private static void PrefabifyGround()
        {
            GameObject root = GameObject.Find("Level");
            if (root == null)
            {
                Debug.LogError("Couldn't find level root.");
                return;
            }

            int count = 0;
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Level/Ground/Ground.prefab");
            for (int i = 0; i < root.transform.childCount; i++)
            {
                GameObject c = root.transform.GetChild(i).gameObject;
                if (c.gameObject.name == "Ground" || c.gameObject.name.StartsWith("Right") ||
                    c.gameObject.name.StartsWith("Left") || c.gameObject.name.StartsWith("Top"))
                {
                    count++;
                    var go = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                    go.transform.SetParent(root.transform);
                    go.transform.position = c.transform.position;
                    go.transform.rotation = c.transform.rotation;
                    go.transform.localScale = c.transform.localScale;
                    GameObject.DestroyImmediate(c);
                }
            }
            
            Debug.Log($"Prefabified {count} instances.");
        }
        
        [MenuItem("Level/PrefabifySpawners")]
        private static void PrefabifySpawners()
        {
            GameObject root = GameObject.Find("Level");
            if (root == null)
            {
                Debug.LogError("Couldn't find level root.");
                return;
            }

            int count = 0;
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Level/Spawner.prefab");
            for (int i = 0; i < root.transform.childCount; i++)
            {
                GameObject c = root.transform.GetChild(i).gameObject;
                if (c.name.StartsWith("Spawner"))
                {
                    count++;
                    var go = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                    go.transform.SetParent(root.transform);
                    go.transform.position = c.transform.position;
                    GameObject.DestroyImmediate(c);
                }
            }
            
            Debug.Log($"Prefabified {count} instances.");
        }

        [MenuItem("Level/PrefabifyLedges")]
        private static void PrefabifyLedges()
        {
            GameObject root = GameObject.Find("Level");
            if (root == null)
            {
                Debug.LogError("Couldn't find level root.");
                return;
            }

            var platforms = new List<GameObject>();
            for (int i = 0; i < root.transform.childCount; i++)
            {
                GameObject c = root.transform.GetChild(i).gameObject;
                if (c.name.StartsWith("Ledge") && !c.name.Contains("Clone"))
                {
                    platforms.Add(c);
                }
            }

            var platformCounts = new Dictionary<string, int>();
            foreach (var p in platforms)
            {
                string type = $"{p.transform.localScale.x},{p.transform.localScale.y}";
                if (platformCounts.ContainsKey(type))
                {
                    platformCounts[type] += 1;
                }
                else
                {
                    platformCounts[type] = 1;
                }
            }

            Debug.Log("Platform types:");
            foreach (var k in platformCounts.Keys)
            {
                Debug.Log($"Type {k}: {platformCounts[k]}");
            }
            
            
            // try replacing prefabs
            var prefabsDict = new Dictionary<string, GameObject>();
            prefabsDict["1,0.25"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Level/Platforms/Ledge-1.prefab");
            prefabsDict["1.5,0.25"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Level/Platforms/Ledge-1p5.prefab");
            prefabsDict["2,0.25"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Level/Platforms/Ledge-2.prefab");
            prefabsDict["3,0.25"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Level/Platforms/Ledge-3.prefab");
            prefabsDict["4,0.25"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Level/Platforms/Ledge-4.prefab");
            prefabsDict["5,0.25"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Level/Platforms/Ledge-5.prefab");
            prefabsDict["10,0.25"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Level/Platforms/Ledge-10.prefab");
            prefabsDict["27,0.25"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Level/Platforms/Ledge-27.prefab");
            foreach (var p in platforms)
            {
                string type = $"{p.transform.localScale.x},{p.transform.localScale.y}";
                if (prefabsDict.ContainsKey(type))
                {
                    var go = (GameObject)PrefabUtility.InstantiatePrefab(prefabsDict[type]);
                    go.transform.SetParent(root.transform);
                    go.transform.position = p.transform.position;
                    GameObject.DestroyImmediate(p);
                }
            }
        }
    }
}