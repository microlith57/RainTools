using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System.Linq;
using Celeste.Mod.Backdrops;
using System;

namespace Celeste.Mod.RainTools.Backdrops {
    [CustomBackdrop("RainTools/Gradient")]
    public class Gradient : Backdrop {

        private const float DIAGONAL = 370f;

        public Color[] Colors = new Color[] { };
        public float Angle;
        public float GradientLength;
        public bool ExtendEnds = false;

        public Vector2 CameraOffset;

        private VertexPositionColor[] gradient;

        public Gradient(Color[] colors,
                        float alpha,
                        float angleRadians = 0f,
                        float gradientLength = 1f,
                        bool extendEnds = false) : base() {

            UseSpritebatch = false;

            Colors = colors;
            Color = Color.White * alpha;
            Angle = angleRadians;
            GradientLength = gradientLength;
            ExtendEnds = extendEnds;

        }

        public Gradient(BinaryPacker.Element data) : this(
            data.Attr("colors", "7bbedf,0c56c2")
                .Split(',')
                .Select(c => Calc.HexToColorWithAlpha(c.Trim()))
                .ToArray(),
            data.AttrFloat("alpha", 1),
            data.AttrFloat("angleDegrees", 0) * Calc.DegToRad,
            data.AttrFloat("gradientLength", 180),
            data.AttrBool("extendEnds", false)
        ) {
        }

        public void SetColors(BlendedCircularInterpolator<Color[]>.Blend blend) {
            if (blend.a.Length == 0 || blend.b.Length == 0)
                throw new IndexOutOfRangeException("too few colours to blend between!");

            int stops;

            if (blend.a.Length == 1)
                stops = blend.b.Length;
            else if (blend.b.Length == 1)
                stops = blend.a.Length;
            else
                stops = Utils.LCM(blend.a.Length - 1, blend.b.Length - 1) + 1;

            Array.Resize(ref Colors, stops);

            if (stops == 0) {
                return;
            } else if (stops == 1) {
                Color from = Utils.ColorArrayLerp(0.5f * (blend.a.Length - 1), blend.a);
                Color to = Utils.ColorArrayLerp(0.5f * (blend.b.Length - 1), blend.b);
                Colors[0] = Color.Lerp(from, to, blend.fac);
                return;
            }

            for (int i = 0; i < stops; i++) {
                var fac = i / ((float) (stops - 1f));

                Color from = Utils.ColorArrayLerp(fac * (blend.a.Length - 1), blend.a);
                Color to = Utils.ColorArrayLerp(fac * (blend.b.Length - 1), blend.b);

                Colors[i] = Color.Lerp(from, to, blend.fac);
            }
        }

        public override void Render(Scene scene) {
            if (Colors.Length == 0)
                return;

            var colors = Colors.Length > 1 ? Colors.ToArray() : new Color[] { Colors[0], Colors[0] };
            for (int i = 0; i < colors.Length; i++) {
                colors[i].R = (byte) (colors[i].R * (Color.R / 255f));
                colors[i].G = (byte) (colors[i].G * (Color.G / 255f));
                colors[i].B = (byte) (colors[i].B * (Color.B / 255f));
                colors[i].A = (byte) (colors[i].A * (Color.A / 255f));
            }

            Vector2 cam = ((scene as Level).Camera.Position + CameraOffset).Floor();
            Vector2 pos = (Position - cam * Scroll).Floor();

            var tris = SetVertices(pos, colors);

            Vector2 viewport = new Vector2(Engine.Graphics.GraphicsDevice.Viewport.Width, Engine.Graphics.GraphicsDevice.Viewport.Height);
            var mat = Matrix.CreateScale(1f / viewport.X * 2f, (0f - 1f / viewport.Y) * 2f, 1f)
                    * Matrix.CreateTranslation(-1f, 1f, 0f);

            var effect = GFX.FxPrimitive;
            var blendState = FrostHelper.API.API.GetBackdropBlendState(this) ?? BlendState.AlphaBlend;

            Engine.Instance.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            Engine.Instance.GraphicsDevice.BlendState = blendState;
            effect.Parameters["World"].SetValue(mat);

            foreach (EffectPass pass in effect.CurrentTechnique.Passes) {
                pass.Apply();
                Engine.Instance.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, gradient, 0, tris);
            }
        }

        private int SetVertices(Vector2 pos, Color[] colors) {
            var tris = (colors.Length - 1) * 2;

            if (ExtendEnds)
                tris += 4;

            var verts = tris + 2;
            Array.Resize(ref gradient, verts);

            float stepSize = GradientLength / (colors.Length - 1);

            var along = Vector2.UnitY.Rotate(Angle);

            var screenCenter = new Vector2(320, 180) / 2;
            var offset = pos - screenCenter;
            var corrected = screenCenter + offset.Proj(along);

            var start = corrected - along * GradientLength / 2f;
            var span = along.Rotate((float) Math.PI / 2f) * DIAGONAL / 2f;

            int v = 0;

            if (ExtendEnds) {
                var midpoint = start - along * 1024;
                gradient[v].Color = colors[0];
                gradient[v++].Position = new(midpoint - span, 0);
                gradient[v].Color = colors[0];
                gradient[v++].Position = new(midpoint + span, 0);
            }

            for (int i = 0; i < colors.Length; i++) {
                var midpoint = start + along * stepSize * i;
                gradient[v].Color = colors[i];
                gradient[v++].Position = new(midpoint - span, 0);
                gradient[v].Color = colors[i];
                gradient[v++].Position = new(midpoint + span, 0);
            }

            if (ExtendEnds) {
                var midpoint = start + along * (GradientLength + 1024);
                gradient[v].Color = colors[^1];
                gradient[v++].Position = new(midpoint - span, 0);
                gradient[v].Color = colors[^1];
                gradient[v++].Position = new(midpoint + span, 0);
            }

            return tris;
        }

    }
}