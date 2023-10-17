using Microsoft.Xna.Framework;

namespace Celeste.Mod.RainTools.Pipes {
    public interface IPart {
        Pipe Pipe { get; set; }
        Vector2 Position { get; }
        Vector2 EndPosition { get; }

        float Offset { get; set; }
        float Length { get; }
    }
}