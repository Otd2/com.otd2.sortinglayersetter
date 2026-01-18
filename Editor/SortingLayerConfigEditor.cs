using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Otd2.SortingLayerSetter.Config;
using Otd2.SortingLayerSetter.Model;

namespace Otd2.SortingLayerSetter.Editor
{
    [CustomEditor(typeof(SortingLayerConfig))]
    public class SortingLayerConfigEditor : UnityEditor.Editor
    {
        private readonly string[] _defaultSortingLayers = new string[]
        {
            "Background",
            "Game",
            "GameVfx",
            "UI",
            "UiVfx",
            "UiOverlay",
            "Popup",
            "PopupVfx",
            "Overlay",
            "SystemMessage"
        };
        
        public override void OnInspectorGUI()
        {
            SortingLayerConfig sortingLayerConfig = (SortingLayerConfig)target;

            // Draw the default Inspector
            DrawDefaultInspector();
            
            // Display a warning if there are no sorting layers
            if(sortingLayerConfig.SortingSubLayers == null || sortingLayerConfig.SortingSubLayers.Count == 0)
            {
                EditorGUILayout.HelpBox(
                    "⚠ WARNING: This will modify the Tag Manager in Project Settings.\n\n" +
                    "All existing sorting layers will be removed and replaced with the default ones.",
                    MessageType.Warning);

                // Set default sorting layers button
                if (GUILayout.Button("Set Default Sorting Layers"))
                {
                    bool confirm = EditorUtility.DisplayDialog(
                        "Overwrite Sorting Layers?",
                        "This action will overwrite all existing sorting layers in the Project Settings.\n\n" +
                        "Are you sure you want to continue?",
                        "Yes", "Cancel");

                    if (confirm)
                    {
                        SetDefaultSortingLayers();
                        EditorUtility.SetDirty(sortingLayerConfig);
                    }
                }
            }
            else
            {
                // Validate sorting orders button
                if (GUILayout.Button("Validate Sorting Orders"))
                {
                    bool isValid = ValidateSortingOrders();

                    if (isValid)
                    {
                        EditorUtility.DisplayDialog("Validation Passed", "No sorting order conflicts found!", "OK");
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("Validation Failed", "Conflicting sorting order values found. Check the console for details.", "OK");
                    }
                }

            }
            
            // Fetch all sorting layers from project settings
            if (GUILayout.Button("Fetch All Sorting Layers"))
            {
                FetchAllSortingLayersInProjectSettings();
                EditorUtility.SetDirty(sortingLayerConfig);
            }
        }
        
        /// <summary>
        /// Gets all sorting layers from the project settings and merge them with the existing sorting layers in the config.
        /// </summary>
        private void FetchAllSortingLayersInProjectSettings()
        {
            List<SortingLayerWithSubLayers> sortingSubLayers = ((SortingLayerConfig)target).SortingSubLayers;
            //get all sorting layers from project settings
            foreach (SortingLayer sortingLayer in SortingLayer.layers)
            {
                //if sorting layer already exists in list, skip
                if(sortingSubLayers.Any(sortingLayerWithSubLayers => sortingLayerWithSubLayers.SortingLayerName == sortingLayer.name))
                    continue;
                
                //add sorting layer to list
                sortingSubLayers.Add(new SortingLayerWithSubLayers()
                {
                    SortingLayerId = sortingLayer.id,
                    SortingLayerName = sortingLayer.name,
                    SubLayers = new List<SortingSubLayer>()
                });
            }
            
            //remove sorting layers that are not in project settings sorting layers
            sortingSubLayers.RemoveAll(soringLayerModel => SortingLayer.layers.All(sortingLayer => sortingLayer.name != soringLayerModel.SortingLayerName));
        }

        /// <summary>
        /// Sets the default sorting layers to the layers in the project settings.
        /// </summary>
        private void SetDefaultSortingLayers()
        {
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty sortingLayersProp = tagManager.FindProperty("m_SortingLayers");

            sortingLayersProp.ClearArray(); // Clear existing sorting layers

            for (int i = 0; i < _defaultSortingLayers.Length; i++)
            {
                sortingLayersProp.InsertArrayElementAtIndex(i);
                SerializedProperty layer = sortingLayersProp.GetArrayElementAtIndex(i);
                layer.FindPropertyRelative("uniqueID").intValue = Random.Range(0, int.MaxValue);
                layer.FindPropertyRelative("name").stringValue = _defaultSortingLayers[i];
            }

            tagManager.ApplyModifiedProperties();
            
            FetchAllSortingLayersInProjectSettings();
        }

        /// <summary>
        /// Checks all of the sorting orders in the config and logs any conflicts.
        /// </summary>
        /// <returns></returns>
        private bool ValidateSortingOrders()
        {
            bool isValid = true;
            List<SortingLayerWithSubLayers> sortingSubLayers = ((SortingLayerConfig)target).SortingSubLayers;
            foreach (var sortingLayer in sortingSubLayers)
            {
                sortingLayer.SubLayers.Sort((sublayer1, sublayer2) => sublayer1.MinSortingOrder.CompareTo(sublayer2.MinSortingOrder));

                for (int i = 0; i < sortingLayer.SubLayers.Count - 1; i++)
                {
                    SortingSubLayer current = sortingLayer.SubLayers[i];
                    SortingSubLayer next = sortingLayer.SubLayers[i + 1];

                    if (current.MaxSortingOrder >= next.MinSortingOrder)
                    {
                        Debug.LogError($"Sorting order conflict in {sortingLayer.SortingLayerName}: " +
                                       $"'{current.SubLayerName}' ({current.MinSortingOrder}-{current.MaxSortingOrder}) " +
                                       $"overlaps with '{next.SubLayerName}' ({next.MinSortingOrder}-{next.MaxSortingOrder}).");
                        isValid = false;
                    }
                }
            }

            if (isValid)
            {
                Debug.Log("Sorting order validation passed. No conflicts found.");
            }
            
            return isValid;
        }
    }
}
