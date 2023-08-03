using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.RainTools {
    [Tracked(true)]
    [GlobalEntity]
    public abstract class ShadowCaster : Entity {

        public readonly int MaxTriCount;

        public ShadowCaster(Vector2 position, int maxTriCount) : base(position) {
            MaxTriCount = maxTriCount;
            Tag |= Tags.Persistent;
        }

        public abstract void UpdateVerts(DirectionalLightingRenderer state);

    }
}
