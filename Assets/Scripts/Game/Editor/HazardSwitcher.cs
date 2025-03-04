using System.Collections.Generic;
using Baddy;
using UnityEditor;
using UnityEngine;

namespace Game.Editor
{
    public class HazardSwitcher
    {

        [MenuItem("Hazard/ToggleBreakable %g")]
        private static void ToggleBreakable()
        {
            GameObject currentHazard = (GameObject)Selection.activeObject;
            if (currentHazard == null)
            {
                Debug.LogError("Unable to load.");
            }

            bool isBreakable = currentHazard.GetComponent<DamageOnHit>();

            if (!isBreakable)
            {
                if (!currentHazard.GetComponent<BreakOnHit>())
                {
                    Debug.LogError("Selected object is not a hazard or breakable hazard.");
                }
            }

            GameObject prefab;
            if (isBreakable)
            {
                prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Level/Hazards/BreakableHazard Variant.prefab");
            }
            else
            {
                prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Level/Hazards/Hazard.prefab");
            }

            var go = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            go.transform.SetParent(currentHazard.transform.parent);
            go.transform.position = currentHazard.transform.position;
            go.transform.rotation = currentHazard.transform.rotation;
            go.transform.localScale = currentHazard.transform.localScale;
            GameObject.DestroyImmediate(currentHazard);
            Selection.activeObject = go;
        }
    }
}