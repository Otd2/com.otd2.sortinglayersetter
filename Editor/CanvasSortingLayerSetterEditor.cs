using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using Otd2.SortingLayerSetter.Config;
using Otd2.SortingLayerSetter.Model;
using Otd2.SortingLayerSetter.Views;

namespace Otd2.SortingLayerSetter.Editor
{
    [CustomEditor(typeof(SortingLayerSetterBase), true)]
    public class CanvasSortingLayerSetterEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            SortingLayerSetterBase canvasSortingLayerSetter = (SortingLayerSetterBase)target;

            SortingLayerConfig sortingLayerConfig = SortingLayerConfig.Instance;

            if (sortingLayerConfig == null)
            {
                EditorGUILayout.HelpBox("SortingLayerConfig not found. Please create one using Create > OTD2 > Sorting Layer > Config and place it in a Resources folder.", MessageType.Warning);
                DrawDefaultInspector();
                return;
            }

            bool isSortingIdAssigned = canvasSortingLayerSetter.SortingLayerId != 0;
            bool isSortingSubLayerAssigned = string.IsNullOrEmpty(canvasSortingLayerSetter.SubLayerName) == false;

            if (isSortingIdAssigned && isSortingSubLayerAssigned)
            {
                List<SortingSubLayer> sublayers = sortingLayerConfig.SortingSubLayers.FirstOrDefault(sortingSubLayers => sortingSubLayers.SortingLayerId == canvasSortingLayerSetter.SortingLayerId)?.SubLayers;

                if (sublayers == null)
                {
                    EditorGUILayout.HelpBox(
                        "Sorting SubLayer is not assigned. The default sub-layer will be used.",
                        MessageType.Warning);
                }
                else
                {
                    SortingSubLayer sublayer = sublayers.FirstOrDefault(sortingSubLayer => sortingSubLayer.SubLayerName == canvasSortingLayerSetter.SubLayerName);

                    if (sublayer == null)
                    {
                        EditorGUILayout.HelpBox(
                            "Sorting SubLayer is not assigned correctly.",
                            MessageType.Error);
                    }
                    else
                    {
                        int maxRange = sublayer.MaxSortingOrder - sublayer.MinSortingOrder;
                        if (canvasSortingLayerSetter.sortOrder > maxRange)
                        {
                            //Show sorting order in help box with red color
                            EditorGUILayout.HelpBox(
                                $"Sorting Order is out of range. Max: {maxRange}",
                                MessageType.Error);
                        }
                        else
                        {
                            EditorGUILayout.HelpBox(
                                $"Sorting Order is in range. Max: {maxRange}",
                                MessageType.Info);
                        }
                    }
                }
            }
            else if (isSortingIdAssigned == false && isSortingSubLayerAssigned == false)
            {
                EditorGUILayout.HelpBox("Sorting Layer is not assigned. The default layer will be used.", MessageType.Warning);
            }

            // Draw the default Inspector
            DrawDefaultInspector();
        }
    }
}
