using Monocle;
using Microsoft.Xna.Framework;
using System;

namespace Celeste.Mod.RainTools {
    public class AsyncLoader : Component {

        private Session session;
        private LevelLoader loader;

        public bool Loaded => loader.Loaded;
        public bool ImmediateSwap = false;

        public Action<Level, Level> OnLoad = (a, b) => { };

        public AsyncLoader(AreaKey area, string roomName, Vector2 pos) : base(true, false) {
            session = new(area) {
                FirstLevel = false,
                Level = roomName,
                StartedFromBeginning = false
            };

            loader = new(session, pos) {
                PlayerIntroTypeOverride = Player.IntroTypes.None
            };
        }

        public override void Update() {
            var level = Scene as Level;

            base.Update();
            loader.Update();

            if (Loaded) {
                OnLoad(level, loader.Level);
                if (ImmediateSwap && Engine.Scene == Scene)
                    Engine.Scene = loader.Level;

                RemoveSelf();
            }
        }

    }
}
