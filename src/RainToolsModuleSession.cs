using Monocle;
using System;

namespace Celeste.Mod.RainTools {
    public class RainToolsModuleSession : EverestModuleSession {
        public float Time = 0f;
        public float SunAngle => Calc.WrapAngle((Time - 0.25f) * (float)(2f * Math.PI));
    }
}
