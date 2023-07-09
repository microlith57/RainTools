using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste.Mod.RainTools {
    [Tracked(true)]
    public abstract class ShadowCaster : Entity {
        public readonly int MaxTriCount;

        public ShadowCaster(Vector2 position, int maxTriCount) : base(position) {
            MaxTriCount = maxTriCount;
        }

        public abstract void UpdateVerts(ShadowRenderer.State state);
    }
}
