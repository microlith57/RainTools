using Monocle;

namespace Celeste.Mod.RainTools.Backdrops {
    internal interface IDisplacementStyleground {
        bool DisplacementVisible { get; }
        void RenderDisplacement(Scene scene);
    }
}
