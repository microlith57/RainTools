using Microsoft.Xna.Framework;

namespace Celeste.Mod.RainTools.Pipes {
    internal interface IPart {
        Pipe Pipe { get; set; }
        Vector2 EndPosition { get; }

        float Length { get; }
    }
}