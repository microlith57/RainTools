using System;
using Monocle;

namespace Celeste.Mod.RainTools.ShadowCasters {
    [Tracked(true)]
    public class CustomLight : Component {
        public Action OnRenderLight;

        public CustomLight(Action onRenderLight)
            : base(active: false, visible: true) {
            OnRenderLight = onRenderLight;
        }
    }
}
