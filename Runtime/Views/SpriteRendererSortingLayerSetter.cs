using UnityEngine;
using Otd2.SortingLayerSetter.Utils;

namespace Otd2.SortingLayerSetter.Views
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteRendererSortingLayerSetter : SortingLayerSetterBase
    {
        private SpriteRenderer _spriteRenderer;

        protected override void Initialize()
        {
            base.Initialize();
            _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        }

        protected override void SetSortingLayer()
        {
            base.SetSortingLayer();
            if (_spriteRenderer == null)
            {
                Debug.LogError($"SpriteRenderer component is missing on the GameObject -> {gameObject.name}. Please add it or set isSpriteRenderer to false.");
                return;
            }

            _spriteRenderer.sortingLayerName = SortingLayer.IDToName(SortingLayerId);
            _spriteRenderer.sortingOrder = SortingLayerUtils.GetSortingOrderWithSublayer(SortingLayerId, SubLayerName, sortOrder);
        }
    }
}
