using System.Linq;
using Otd2.SortingLayerSetter.Config;
using Otd2.SortingLayerSetter.Model;

namespace Otd2.SortingLayerSetter.Utils
{
    public static class SortingLayerUtils
    {
        public static int GetSortingOrderWithSublayer(int sortingLayerId, string sortingSublayerName, int order)
        {
            SortingLayerConfig sortingLayerConfig = SortingLayerConfig.Instance;
            
            if (sortingLayerConfig == null || sortingLayerConfig.SortingSubLayers == null)
                return order;
                
            SortingLayerWithSubLayers assignedLayer = sortingLayerConfig.SortingSubLayers.Find(sortingLayer => sortingLayer.SortingLayerId == sortingLayerId);

            // If subLayerName is not assigned, use the default sorting layer
            SortingSubLayer assignedSublayer = null;
            if (string.IsNullOrEmpty(sortingSublayerName) == false && assignedLayer != null)
            {
                assignedSublayer = assignedLayer.SubLayers.FirstOrDefault(sortingSubLayer => sortingSubLayer.SubLayerName == sortingSublayerName);
            }

            return assignedSublayer?.MinSortingOrder + order ?? order;
        }
    }
}
