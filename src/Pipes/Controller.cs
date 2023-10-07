using Microsoft.Xna.Framework;
using Monocle;
using System.Collections.Generic;
using System.Linq;

namespace Celeste.Mod.RainTools.Pipes {

    // [CustomEntity("RainTools/PipeController")]
    [Tracked]
    public class Controller : Entity {

        public IEnumerable<Pipe> Pipes => Components.Where(p => p is Pipe).Cast<Pipe>();
        private Dictionary<Vector2, IPart> discontiuities = new();

        #region constructors

        public Controller() {
            Depth = -Depths.Top;
            Visible = false;
        }

        public Controller(EntityData data) : this() => SetData(data);

        private void SetData(EntityData data) {
            // todo
        }

        public static Controller Load(Level level, LevelData levelData, Vector2 offset, EntityData data) {
            var existing = level.Tracker.GetEntity<Controller>();

            if (existing == null) {
                Controller instance = new(data);
                level.Tracker.Entities[typeof(Controller)].Add(instance);
                return instance;
            }

            existing.SetData(data);
            return null;
        }

        public static Controller AddIfAbsent(Level level) {
            var instance = level.Tracker.GetEntity<Controller>();

            if (instance == null) {
                level.Add(instance = new());
                level.Tracker.Entities[typeof(Controller)].Add(instance);
            }

            return instance;
        }

        #endregion

        #region add to level

        public void Add(Endpoint endpoint) {
            var pos = endpoint.Position;

            if (discontiuities.TryGetValue(pos, out var adj)) {
                discontiuities.Remove(pos);

                var pipe = adj.Pipe;
                bool addToEnd = false;

                if (adj == pipe.Parts[^1])
                    addToEnd = true;

                pipe.Add(endpoint, addToEnd);
            } else {
                Add(new Pipe(endpoint));
                discontiuities.Add(pos, endpoint);
            }
        }

        public void Add(Segment segment) {
            var start = segment.Position;
            var end = segment.EndPosition;

            bool startConnected = false, endConnected = false;
            bool addToEndOfStart = false, addToEndOfEnd = false;

            if (start == end) {
                Logger.Log(LogLevel.Verbose, nameof(RainToolsModule),
                           $"segment from ({start}) to ({end}) failed to join a pipe: loop");
                Add(new Pipe(segment));
                return;
            }

            if (discontiuities.TryGetValue(start, out var adj_a)) {
                discontiuities.Remove(start);
                startConnected = true;

                var pipe = adj_a.Pipe;

                if (adj_a == pipe.Parts[^1])
                    addToEndOfStart = true;

                pipe.Add(segment, addToEndOfStart);
            } else {
                discontiuities.Add(start, segment);
            }

            if (discontiuities.TryGetValue(end, out var adj_b)) {
                discontiuities.Remove(end);
                endConnected = true;

                var pipe = adj_b.Pipe;
                if (segment.Pipe == pipe) {
                    Logger.Log(LogLevel.Verbose, nameof(RainToolsModule),
                               $"segment from ({start}) to ({end}) formed a loop");
                    return;
                }

                if (adj_b == pipe.Parts[^1])
                    addToEndOfEnd = true;

                if (startConnected) {
                    segment.Pipe.AddFrom(pipe, addToEndOfStart, addToEndOfEnd);
                    Remove(pipe);
                } else {
                    pipe.Add(segment, addToEndOfEnd);
                }
            } else {
                discontiuities.Add(end, segment);
            }

            if (!startConnected && !endConnected)
                Add(new Pipe(segment));
        }

        #endregion

    }
}