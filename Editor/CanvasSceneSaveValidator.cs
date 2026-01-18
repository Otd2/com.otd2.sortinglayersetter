using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Otd2.SortingLayerSetter.Config;
using Otd2.SortingLayerSetter.Model;
using Otd2.SortingLayerSetter.Views;

namespace Otd2.SortingLayerSetter.Editor
{
    [InitializeOnLoad]
    public class CanvasSceneSaveValidator
    {
        static CanvasSceneSaveValidator()
        {
            EditorSceneManager.sceneSaving += OnSceneSaving;
            EditorSceneManager.sceneClosing += OnSceneClosing;
        }

        private static void OnSceneClosing(Scene scene, bool removingscene)
        {
            ValidateCanvasSortingLayerSetter(scene);
        }

        private static void OnSceneSaving(Scene scene, string path)
        {
            ValidateCanvasSortingLayerSetter(scene);
        }
        
        private static void ValidateCanvasSortingLayerSetter(Scene scene)
        {
            // Find all GameObjects with the component you want to validate
            foreach (SortingLayerSetterBase canvasSortingLayerObjectInScene in Object.FindObjectsOfType<SortingLayerSetterBase>(true))
            {
                if(IsValid(canvasSortingLayerObjectInScene) == false)
                {
                    EditorUtility.DisplayDialog("Canvas sorting orders are incorrect", $"Something went wrong with SortingLayerSetterBase validation in Scene : {scene.name}. \nCheck Console for more information.", "OK");
                    
                    // Select the GameObject in the Hierarchy
                    Selection.activeGameObject = canvasSortingLayerObjectInScene.gameObject;

                    // Focus on the gameObject in Scene View
                    SceneView.FrameLastActiveSceneView();
                    break;
                }
            }
        }
        
        /// <summary>
        /// This validation is triggered when the scene is saved or closed.
        /// </summary>
        /// <returns></returns>
        private static bool IsValid(SortingLayerSetterBase canvasSortingLayerSetter)
        {
            // If the scene is playing, no need to validate
            if(EditorApplication.isPlaying || EditorApplication.isCompiling)
            {
                return true;
            }
            
            SortingLayerConfig sortingLayerConfig = SortingLayerConfig.Instance;
            if (sortingLayerConfig == null)
            {
                return true; // Skip validation if config is not found
            }
            
            int sortingLayerId = canvasSortingLayerSetter.SortingLayerId;
            string subLayerName = canvasSortingLayerSetter.SubLayerName;
            int sortOrder = canvasSortingLayerSetter.sortOrder;
            
            List<SortingLayerWithSubLayers> sortingLayerWithSubLayersList = sortingLayerConfig.SortingSubLayers;
            
            if (sortingLayerWithSubLayersList == null)
            {
                return true;
            }
            
            // Check if the sorting layer is assigned correctly
            bool isSortingIdAssigned = sortingLayerWithSubLayersList.Any(sortingLayers => sortingLayers.SortingLayerId == sortingLayerId);
            if (isSortingIdAssigned == false)
            {
                Debug.LogError("Sorting Layer is not assigned correctly.");
                return false;
            }
            
            // Check if the sorting sublayer is assigned correctly
            bool isSortingSubLayerAssigned = string.IsNullOrEmpty(subLayerName) == false;
            SortingLayerWithSubLayers assignedLayer = sortingLayerWithSubLayersList.Find(sortingLayer => sortingLayer.SortingLayerId == sortingLayerId);
            bool hasSubLayers = assignedLayer.SubLayers is { Count: > 0 };

            // If subLayer is not assigned and has no sublayers in config, no need to check the sublayer and sorting order
            if(isSortingSubLayerAssigned == false && hasSubLayers == false)
            {
                return true;
            }
            
            // If subLayer is assigned but the sublayer list is empty, return false
            if(isSortingSubLayerAssigned && hasSubLayers == false)
            {
                Debug.LogError("Sorting SubLayer is not assigned correctly.");
                return false;
            }
            
            if(isSortingSubLayerAssigned == false && hasSubLayers)
            {
                Debug.LogError("Sorting SubLayer must be assigned when the Sorting Layer has sublayers.");
                return false;
            }
            
            // Check if the sublayer is assigned correctly
            var isSublayerIsCorrect = assignedLayer.SubLayers.Any(subLayer => subLayer.SubLayerName == subLayerName);
            if (isSublayerIsCorrect == false)
            {
                Debug.LogError("Sorting SubLayer is not valid");
                return false;
            }
            
            // Check if the sorting order is in range
            SortingSubLayer assignedSublayer = assignedLayer.SubLayers.FirstOrDefault(subLayer => subLayer.SubLayerName == subLayerName);
            int maxRange = assignedSublayer.MaxSortingOrder - assignedSublayer.MinSortingOrder;
            bool sortingOrderIsCorrect = 0 <= sortOrder && sortOrder <= maxRange;
            if (sortingOrderIsCorrect == false)
            {
                Debug.LogError("Sorting Order is not in range.");
                return false;
            }

            return true;
        }
    }
}
