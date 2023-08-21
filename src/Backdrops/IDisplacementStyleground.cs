using Monocle;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Celeste.Mod.RainTools.Backdrops {
    internal interface IDisplacementStyleground {
        bool DisplacementVisible { get; }
        void RenderDisplacement(Scene scene);
    }
}

namespace Celeste.Mod.RainTools.Hooks {
    internal static class DisplacementStyleground {

        internal static void Load() {
            IL.Celeste.DisplacementRenderer.BeforeRender += DisplacementRenderer_BeforeRender;
        }

        internal static void Unload() {
            IL.Celeste.DisplacementRenderer.BeforeRender -= DisplacementRenderer_BeforeRender;
        }

        private static void DisplacementRenderer_BeforeRender(ILContext context) {
            ILCursor cursor = new(context);

            cursor.GotoNext(MoveType.Before, i => i.MatchCallOrCallvirt(out var m)
                                               && m.FullName.EndsWith("Monocle.Draw::get_SpriteBatch()"));

            cursor.Emit(OpCodes.Ldarg_1);
            cursor.EmitDelegate((Scene scene) => {
                var effects = (scene as Level).Foreground.GetEach<Backdrops.IDisplacementStyleground>();
                if (!effects.Any())
                    return;

                Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Matrix.Identity);

                foreach (var effect in effects)
                    if (effect.DisplacementVisible)
                        effect.RenderDisplacement(scene);

                Draw.SpriteBatch.End();
            });
        }

    }
}
