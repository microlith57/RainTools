using System;
using Monocle;

namespace Celeste.Mod.RainTools.ShadowCasters {
    [Tracked]
    public class CustomShadow : Component {
        public Action<DirectionalLightingRenderer> OnRenderShadow;

        public CustomShadow(Action<DirectionalLightingRenderer> onRenderShadow)
            : base(active: false, visible: true) {
            OnRenderShadow = onRenderShadow;
        }
    }
}
