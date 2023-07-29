using Microsoft.Xna.Framework;
using Monocle;
using Celeste.Mod.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using MonoMod.Utils;

namespace Celeste.Mod.RainTools {
    [Tracked(true)]
    [GlobalEntity]
    [CustomEntity("RainTools/SunlightAngleTimeController")]
    public class SunlightAngleTimeController : Entity {
        public string StylegroundTag;

        public SunlightAngleTimeController(EntityData data, Vector2 offset) : base() {
            StylegroundTag = data.Attr("tag");
        }

        public override void Update() {
            base.Update();

            float sunAngle = RainToolsModule.Session.SunAngle;

            foreach (var fg in (Scene as Level).Foreground.GetEach<Sunlight>(StylegroundTag)) {
                fg.Angle = sunAngle;
            }
        }
    }
}
