using UnityEngine;

namespace Otd2.SortingLayerSetter.Views
{
    public class CanvasCameraSortingLayerSetter : CanvasSortingLayerSetter
    {
        protected override void SetSortingLayer()
        {
            _canvas.worldCamera = Camera.main;
            _canvas.renderMode = RenderMode.ScreenSpaceCamera;
            base.SetSortingLayer();
        }
    }
}
