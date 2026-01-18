using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Otd2.SortingLayerSetter.Attributes;
using Otd2.SortingLayerSetter.Config;

namespace Otd2.SortingLayerSetter.Editor
{
    [CustomPropertyDrawer(typeof(SortingSubLayerAttribute))]
    public class SortingSubLayerPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String) // Assuming the sub-layer name is a string
            {
                Debug.LogError("SortingSubLayer property should be a string (the sub-layer name)");
            }
            else
            {
                // Get the target object of the SerializedProperty
                object targetObject = property.serializedObject.targetObject;

                // Get the type of the target object
                Type targetType = targetObject.GetType();

                // Iterate through all fields of the target object
                foreach (FieldInfo field in targetType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    // Check if the field has the SortingLayerWithSublayer attribute
                    if (field.GetCustomAttribute<SortingLayerAttribute>() != null)
                    {
                        // Get the SerializedProperty for this field
                        SerializedProperty serializedProperty = property.serializedObject.FindProperty(field.Name);

                        if (serializedProperty != null)
                        {
                            string[] subLayers = GetSubLayerNames(serializedProperty.intValue); // Fetch the SubLayers based on the selected SortingLayer name

                            if (subLayers is { Length: > 0 })
                            {
                                // Display a dropdown for the sub-layers
                                int selectedIndex = Mathf.Max(0, System.Array.IndexOf(subLayers, property.stringValue));
                                selectedIndex = EditorGUI.Popup(position, label.text, selectedIndex, subLayers);

                                property.stringValue = subLayers[selectedIndex]; // Set the selected sub-layer value
                            }
                            else
                            {
                                property.stringValue = ""; // Reset the sub-layer value
                            }
                        }
                    }
                }
            }
        }
        
        // Helper method to fetch SubLayers based on the selected SortingLayer name
        private string[] GetSubLayerNames(int layerId)
        {
            var config = SortingLayerConfig.Instance;
            if (config == null || config.SortingSubLayers == null)
                return null;
                
            return config.SortingSubLayers.FirstOrDefault(sortingLayerWithSubLayers => sortingLayerWithSubLayers.SortingLayerId == layerId)?.SubLayers?.Select(sortingSubLayer => sortingSubLayer.SubLayerName).ToArray();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
    }
}
