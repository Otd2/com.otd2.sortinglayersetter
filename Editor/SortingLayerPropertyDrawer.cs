using UnityEditor;
using UnityEngine;
using Otd2.SortingLayerSetter.Attributes;

namespace Otd2.SortingLayerSetter.Editor
{
    [CustomPropertyDrawer(typeof(SortingLayerAttribute))]
    public class SortingLayerPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.Integer)
            {
                Debug.LogError("Sorting Layer property should be an integer (the layer ID)");
            }
            else
            {
                EditorGUI.BeginProperty(position, label, property);

                // Get the current sorting layer ID
                int currentLayerID = property.intValue;

                // Get the names and IDs of all sorting layers
                string[] sortingLayerNames = GetSortingLayerNames();
                int[] sortingLayerIDs = GetSortingLayerIDs();

                // Find the index of the current sorting layer ID
                int currentIndex = System.Array.IndexOf(sortingLayerIDs, currentLayerID);

                // Draw the popup and get the selected index
                int selectedIndex = EditorGUI.Popup(position, label.text, currentIndex, sortingLayerNames);

                // Update the property value if the selection changed
                if (selectedIndex >= 0 && selectedIndex < sortingLayerIDs.Length)
                {
                    property.intValue = sortingLayerIDs[selectedIndex];
                }

                EditorGUI.EndProperty();
            }
        }

        private string[] GetSortingLayerNames()
        {
            int layerCount = SortingLayer.layers.Length;
            string[] layerNames = new string[layerCount];
            for (int i = 0; i < layerCount; i++)
            {
                layerNames[i] = SortingLayer.layers[i].name;
            }
            return layerNames;
        }

        private int[] GetSortingLayerIDs()
        {
            int layerCount = SortingLayer.layers.Length;
            int[] layerIDs = new int[layerCount];
            for (int i = 0; i < layerCount; i++)
            {
                layerIDs[i] = SortingLayer.layers[i].id;
            }
            return layerIDs;
        }
    }
}
