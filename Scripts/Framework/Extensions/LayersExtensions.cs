using Framework.Generated;
using UnityEngine;

namespace Framework.Extensions
{
    public static class LayersExtensions
    {
        public static LayerMask ToLayerMask(this Layers layer) => 1 << (int)layer;
    }
}
