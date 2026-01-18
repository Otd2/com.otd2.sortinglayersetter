using System.Collections.Generic;

namespace Otd2.SortingLayerSetter.Model
{
    [System.Serializable]
    public class SortingLayerWithSubLayers
    {
        public string SortingLayerName;
        public int SortingLayerId;
        public List<SortingSubLayer> SubLayers;
    }
}
