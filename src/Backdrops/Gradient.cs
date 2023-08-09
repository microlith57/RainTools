using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System.Linq;
using Celeste.Mod.Backdrops;
using System;

namespace Celeste.Mod.RainTools {
    [CustomBackdrop("RainTools/Gradient")]
    public class GradientBackdrop : Backdrop {

        private const float DIAGONAL = 370f;

        public Color[] Colors = new Color[] { };
        public float Angle;
        public float Alpha;
        public float GradientLength;
        public bool ExtendEnds = false;

        public Vector2 CameraOffset;

        private VertexPositionColor[] gradient;

        public GradientBackdrop(Color[] colors,
                                float alpha,
                                float angleRadians = 0f,
                                float gradientLength = 1f,
                                bool extendEnds = false) : base() {

            UseSpritebatch = false;

            Colors = colors;
            Alpha = alpha;
            Angle = angleRadians;
            GradientLength = gradientLength;
            ExtendEnds = extendEnds;

        }

        public GradientBackdrop(BinaryPacker.Element data) : this(
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
            var stops = LCM(blend.a.Length, blend.b.Length);
            Array.Resize(ref Colors, stops);

            for (int i = 0; i < stops; i++) {
                var fac = i / (stops - 1);

                Color from = ColorArrayLerp(fac * (blend.a.Length - 1), blend.a);
                Color to = ColorArrayLerp(fac * (blend.b.Length - 1), blend.b);

                Colors[i] = Color.Lerp(from, to, blend.fac);
            }
        }

        public override void Render(Scene scene) {
            if (Colors.Length == 0)
                return;

            var colors = Colors.Length > 1 ? Colors : new Color[] { Colors[0], Colors[0] };

            Vector2 cam = ((scene as Level).Camera.Position + CameraOffset).Floor();
            Vector2 pos = (Position - cam * Scroll).Floor();

            var tris = SetVertices(pos, colors);

            Vector2 viewport = new Vector2(Engine.Graphics.GraphicsDevice.Viewport.Width, Engine.Graphics.GraphicsDevice.Viewport.Height);
            var mat = Matrix.Identity;
            mat *= Matrix.CreateScale(1f / viewport.X * 2f, (0f - 1f / viewport.Y) * 2f, 1f);
            mat *= Matrix.CreateTranslation(-1f, 1f, 0f);

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
            var proj = Vector2.Dot(offset, along) / Vector2.Dot(along, along) * along;
            var corrected = screenCenter + proj;

            var start = corrected - along * GradientLength / 2f;
            var span = along.Rotate((float) Math.PI / 2f) * DIAGONAL / 2f;

            int v = 0;

            if (ExtendEnds) {
                var midpoint = start - along * 1024;
                gradient[v].Color = Colors[0] * Alpha;
                gradient[v++].Position = new(midpoint - span, 0);
                gradient[v].Color = Colors[0] * Alpha;
                gradient[v++].Position = new(midpoint + span, 0);
            }

            for (int i = 0; i < colors.Length; i++) {
                var midpoint = start + along * stepSize * i;
                gradient[v].Color = Colors[i] * Alpha;
                gradient[v++].Position = new(midpoint - span, 0);
                gradient[v].Color = Colors[i] * Alpha;
                gradient[v++].Position = new(midpoint + span, 0);
            }

            if (ExtendEnds) {
                var midpoint = start + along * (GradientLength + 1024);
                gradient[v].Color = Colors[^1] * Alpha;
                gradient[v++].Position = new(midpoint - span, 0);
                gradient[v].Color = Colors[^1] * Alpha;
                gradient[v++].Position = new(midpoint + span, 0);
            }

            return tris;
        }

        // todo refactor into util class
        // todo credit communalhelper

        private static int GCF(int a, int b) {
            while (b != 0) {
                int temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }

        private static int LCM(int a, int b) {
            return (a / GCF(a, b)) * b;
        }

        public static Color ColorArrayLerp(float lerp, params Color[] colors) {
            float m = Mod(lerp, colors.Length);
            int fromIndex = (int) Math.Floor(m);
            int toIndex = Mod(fromIndex + 1, colors.Length);
            float clampedLerp = m - fromIndex;

            return Color.Lerp(colors[fromIndex], colors[toIndex], clampedLerp);
        }

        public static float Mod(float x, float m) {
            return ((x % m) + m) % m;
        }

        public static int Mod(int x, int m) {
            return ((x % m) + m) % m;
        }

    }
}