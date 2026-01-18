using System.Collections.Generic;
using UnityEngine;
using Otd2.SortingLayerSetter.Model;

namespace Otd2.SortingLayerSetter.Config
{
    [CreateAssetMenu(fileName = "SortingLayerConfig", menuName = "OTD2/Sorting Layer/Config")]
    public class SortingLayerConfig : ScriptableObject
    {
        private const string ResourcePath = "SortingLayerConfig";
        private static SortingLayerConfig _instance;

        public static SortingLayerConfig Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<SortingLayerConfig>(ResourcePath);
#if UNITY_EDITOR
                    if (_instance == null)
                    {
                        Debug.LogWarning($"SortingLayerConfig not found at Resources/{ResourcePath}. Please create one using Create > OTD2 > Sorting Layer > Config and place it in a Resources folder.");
                    }
#endif
                }
                return _instance;
            }
        }

        public List<SortingLayerWithSubLayers> SortingSubLayers = new List<SortingLayerWithSubLayers>();
    }
}
