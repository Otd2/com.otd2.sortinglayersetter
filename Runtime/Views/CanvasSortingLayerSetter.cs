using UnityEngine;
using Otd2.SortingLayerSetter.Utils;

namespace Otd2.SortingLayerSetter.Views
{
    [RequireComponent(typeof(Canvas))]
    public class CanvasSortingLayerSetter : SortingLayerSetterBase
    {
        protected Canvas _canvas;

        protected override void Initialize()
        {
            _canvas = gameObject.GetComponent<Canvas>();
        }

        protected override void SetSortingLayer()
        {
            base.SetSortingLayer();
            _canvas.overrideSorting = true;
            _canvas.sortingLayerName = SortingLayer.IDToName(SortingLayerId);
            _canvas.sortingOrder = SortingLayerUtils.GetSortingOrderWithSublayer(SortingLayerId, SubLayerName, sortOrder);
        }
    }
}
