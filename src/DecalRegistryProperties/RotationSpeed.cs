using Monocle;
using System.Xml;

namespace Celeste.Mod.RainTools.DecalRegistryProperties {
    public static class RotationSpeed {

        [Tracked]
        public class Rotator : Component {

            public float Speed;

            public Rotator(float speed) : base(true, false) => Speed = speed;
            public override void Update() => (Entity as Decal).Rotation += Speed * Engine.DeltaTime;

        }

        public static void HandleRotationSpeedDecal(Decal decal, XmlAttributeCollection attrs) {
            float speed = 0f;

            if (attrs["value"]?.Value is string s_val)
                speed = float.Parse(s_val).ToRad();

            decal.Add(new Rotator(speed));
        }

        internal static void Load() {
            DecalRegistry.AddPropertyHandler("raintools_rotation_speed", HandleRotationSpeedDecal);
        }

    }
}
