using Monocle;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Graphics;

namespace Celeste.Mod.RainTools {
    [Tracked(true)]
    [GlobalEntity]
    [CustomEntity("RainTools/HeatController")]
    public class HeatController : Entity {

        public Vector2 BaseSpeed, DistortSpeed;
        public MTexture BaseTexture, DistortTexture;
        public float WindMultiplier;
        public float BaseAmount;

        private VirtualRenderTarget target;

        public HeatController(EntityData data, Vector2 offset) : base(data.Position + offset) {
            Depth = -Depths.Top;

            Add(new BeforeRenderHook(BeforeRender));
            Add(new DisplacementRenderHook(RenderDisplacement));

            target = new VirtualRenderTarget("raintools_heat_texture", 320, 180, 0, false, false);

            BaseSpeed = new(data.Float("baseSpeedX"), data.Float("baseSpeedY"));
            BaseTexture = GFX.Game[data.Attr("baseTexture", "raintools_heat")];

            DistortSpeed = new(data.Float("distortSpeedX"), data.Float("distortSpeedY"));
            DistortTexture = GFX.Game[data.Attr("distortTexture", "raintools_heat")];
        }

        public override void Added(Scene scene) {
            base.Added(scene);

            if (scene.Tracker.GetEntity<HeatController>() != null)
                RemoveSelf();
        }

        private void BeforeRender() {
            Engine.Graphics.GraphicsDevice.SetRenderTarget(GameplayBuffers.TempA);
            Engine.Graphics.GraphicsDevice.Clear(Color.Transparent);


            Engine.Graphics.GraphicsDevice.SetRenderTarget(GameplayBuffers.TempB);
            Engine.Graphics.GraphicsDevice.Clear(Color.Transparent);

            Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, GFX.FxTexture);
            Draw.SpriteBatch.End();

            Engine.Graphics.GraphicsDevice.SetRenderTarget(target);
            Engine.Graphics.GraphicsDevice.Clear(Color.Transparent);

            var effect = GFX.FxDistort;
            effect.CurrentTechnique = effect.Techniques["Displace"];
            Engine.Graphics.GraphicsDevice.Textures[1] = GameplayBuffers.TempB;

            Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, effect);
            Draw.SpriteBatch.Draw(GameplayBuffers.TempA, Vector2.Zero, Color.White);
            Draw.SpriteBatch.End();
        }

        private void RenderDisplacement() {
            Draw.SpriteBatch.Draw(target, Vector2.Zero, Color.White);
        }

    }

    public class HeatRenderHook : Component {
        public Action RenderHeat;

        public HeatRenderHook(Action render)
            : base(active: false, visible: true) {
            RenderHeat = render;
        }
    }
}
