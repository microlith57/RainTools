using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System.Linq;
using Celeste.Mod.Backdrops;
using System;

namespace Celeste.Mod.RainTools.Backdrops {
    internal interface IHasAngle {
        float GetAngle();
        void SetAngle(float angle);
    }
}