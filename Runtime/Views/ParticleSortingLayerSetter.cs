using UnityEngine;
using Otd2.SortingLayerSetter.Utils;

namespace Otd2.SortingLayerSetter.Views
{
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticleSortingLayerSetter : SortingLayerSetterBase
    {
        private ParticleSystem _particleSystem;
        private Renderer _renderer;

        protected override void Initialize()
        {
            base.Initialize();
            _particleSystem = GetComponent<ParticleSystem>();
            _renderer = _particleSystem.GetComponent<Renderer>();
        }

        protected override void SetSortingLayer()
        {
            base.SetSortingLayer();
            _renderer.sortingLayerName = SortingLayer.IDToName(SortingLayerId);
            _renderer.sortingOrder = SortingLayerUtils.GetSortingOrderWithSublayer(SortingLayerId, SubLayerName, sortOrder);
        }
    }
}
