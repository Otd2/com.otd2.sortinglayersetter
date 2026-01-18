using UnityEngine;
using Otd2.SortingLayerSetter.Attributes;

namespace Otd2.SortingLayerSetter.Views
{
    public class SortingLayerSetterBase : MonoBehaviour
    {
        [SortingLayer]
        public int SortingLayerId = 0;

        [SortingSubLayer]
        public string SubLayerName = "";

        public int sortOrder;

        private void Start()
        {
            Initialize();
            SetSortingLayer();
            Invoke(nameof(SetSortingLayer), 0.1f);
        }

        protected virtual void SetSortingLayer()
        {
            //in case of unwanted behavior, we can disable the component
            if (enabled == false)
                return;
            
            if (SortingLayerId == 0)
            {
                Debug.LogWarning($"SortingLayerId is not set for {gameObject.name} and type {GetType().Name}.");
            }
        }

        protected virtual void Initialize()
        {
        }
    }
}
