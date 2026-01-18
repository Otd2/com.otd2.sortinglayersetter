using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Otd2.SortingLayerSetter.Views;

namespace Otd2.SortingLayerSetter.Editor
{
    public class SortingLayerEditorUtils
    {
        [MenuItem("Tools/OTD2/Sorting Layer/Find Missing CanvasSortingLayerSetter")]
        public static void FindMissingCanvasSortingLayerSetter()
        {
            // Find all prefabs in the project
            string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab");
            List<string> missingCanvasPaths = new List<string>();

            foreach (string guid in prefabGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                if (prefab == null)
                    continue;

                // Search all Canvas components including inactive children
                Canvas[] canvases = prefab.GetComponentsInChildren<Canvas>(true);

                foreach (Canvas canvas in canvases)
                {
                    // Check if CanvasSortingLayerSetter is missing
                    if (canvas.GetComponent<SortingLayerSetterBase>() == null)
                    {
                        string fullPath = GetFullPath(canvas.transform, prefab.transform);
                        missingCanvasPaths.Add($"{path} -> {fullPath}");
                    }
                }
            }

            if (missingCanvasPaths.Count == 0)
            {
                Debug.Log("✅ All canvases have CanvasSortingLayerSetter.");
            }
            else
            {
                Debug.LogWarning($"❌ Found {missingCanvasPaths.Count} canvases missing CanvasSortingLayerSetter:");
                foreach (string msg in missingCanvasPaths)
                {
                    Debug.Log(msg);
                }
            }
        }

        // Helper method to get the full path of the canvas within the prefab hierarchy
        private static string GetFullPath(Transform current, Transform root)
        {
            List<string> pathParts = new List<string>();
            while (current != null && current != root)
            {
                pathParts.Insert(0, current.name);
                current = current.parent;
            }

            pathParts.Insert(0, root.name);
            return string.Join("/", pathParts);
        }

        [MenuItem("Tools/OTD2/Sorting Layer/Check All Sorting Layer Setters Are Set Correctly")]
        public static void CheckAllSortingLayerSettersAreSetCorrectly()
        {
            string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab");
            List<string> defaultSetterPaths = new List<string>();

            foreach (string guid in prefabGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab == null) continue;

                SortingLayerSetterBase[] setters = prefab.GetComponentsInChildren<SortingLayerSetterBase>(true);

                foreach (SortingLayerSetterBase setter in setters)
                {
                    if (setter.SortingLayerId == 0)
                    {
                        string fullPath = GetFullPath(setter.transform, prefab.transform);
                        defaultSetterPaths.Add($"{path} -> {fullPath}");
                    }
                }
            }

            if (defaultSetterPaths.Count == 0)
            {
                Debug.Log("✅ No SortingLayerSetter is set to 'Default' layer.");
            }
            else
            {
                Debug.LogWarning($"⚠️ Found {defaultSetterPaths.Count} SortingLayerSetters using 'Default' layer:");
                foreach (var msg in defaultSetterPaths)
                {
                    Debug.Log(msg);
                }
            }
        }

        [MenuItem("Tools/OTD2/Sorting Layer/Fix Unknown Sorting Layer Setters")]
        public static void AutoFixSpriteRenderersWithUnknownLayers()
        {
            string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab");

            // Get valid sorting layers in the project
            HashSet<string> validSortingLayers = new HashSet<string>();
            foreach (var layer in SortingLayer.layers)
            {
                validSortingLayers.Add(layer.name);
            }

            int fixedCount = 0;
            int skippedCount = 0;

            foreach (string guid in prefabGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefabRoot = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefabRoot == null) continue;

                // Instantiate the prefab temporarily for editing
                GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefabRoot);
                if (instance == null) continue;

                bool hasChanges = false;

                SpriteRenderer[] renderers = instance.GetComponentsInChildren<SpriteRenderer>(true);
                foreach (SpriteRenderer renderer in renderers)
                {
                    if (!validSortingLayers.Contains(renderer.sortingLayerName))
                    {
                        var setter = renderer.GetComponent<SortingLayerSetterBase>();
                        if (setter != null)
                        {
                            string ex = renderer.sortingLayerName;
                            renderer.sortingLayerName = SortingLayer.IDToName(setter.SortingLayerId);
                            string now = renderer.sortingLayerName;
                            renderer.sortingOrder = setter.sortOrder;
                            Debug.Log($"✔️ Fixed: {path} -> {GetFullPath(renderer.transform, instance.transform)} (EX: {ex} -- NOW: {now})");
                            hasChanges = true;
                            fixedCount++;
                        }
                        else
                        {
                            Debug.LogWarning($"⚠️ Skipped: {path} -> {GetFullPath(renderer.transform, instance.transform)} (Unknown layer, no setter or sublayerName)");
                            skippedCount++;
                        }
                    }
                }

                // If we made any changes, apply them back to the prefab
                if (hasChanges)
                {
                    PrefabUtility.SaveAsPrefabAsset(instance, path);
                }

                GameObject.DestroyImmediate(instance);
            }

            Debug.Log($"✅ Auto-fix complete. Fixed: {fixedCount}, Skipped: {skippedCount}");
        }
    }
}
