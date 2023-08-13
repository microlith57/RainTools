using Monocle;
using Microsoft.Xna.Framework;
using System;
using System.Threading;

namespace Celeste.Mod.RainTools {
    public class AsyncLoader : Component {

        private Session session;
        private LevelLoader loader;

        public bool Loaded => loader.Loaded;
        public bool ImmediateSwap = false;

        public Action<Level, Level> OnLoad = (a, b) => { };

        private static WeakReference<Thread> thread;

        public AsyncLoader(AreaKey area, string roomName) : base(true, false) {
            session = new(area) {
                FirstLevel = false,
                Level = roomName,
                StartedFromBeginning = false
            };

            loader = new(session, Vector2.Zero) {
                PlayerIntroTypeOverride = Player.IntroTypes.None
            };
            thread = LevelLoader.LastLoadingThread;
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

        public void ForceLoadSync() {
            if (thread.TryGetTarget(out var loadingThread)) {
                loadingThread.Join();
            }
        }

    }
}
