using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Celeste.Mod.RainTools.Pipes {

    // [CustomEntity("RainTools/PipeController")]
    [Tracked]
    public class Controller : Entity {

        public IEnumerable<Pipe> Pipes => Components.Where(p => p is Pipe).Cast<Pipe>();
        private Dictionary<Vector2, IPart> discontiuities = new();

        public Controller() {
            Depth = -Depths.Top;
            Visible = false;
        }

        public Controller(EntityData data) : this() => SetData(data);

        private void SetData(EntityData data) {
            // todo
        }

        public static Controller Load(Level level, LevelData levelData, Vector2 offset, EntityData data) {
            var instance = level.Tracker.GetEntity<Controller>();

            if (instance != null) {
                instance.SetData(data);
                return null;
            }

            var dyndata = DynamicData.For(level);

            instance = dyndata.Get<Controller>("raintools_pipe_controller");

            if (instance != null) {
                instance.SetData(data);
                return null;
            }

            instance = new Controller(data);
            dyndata.Set("raintools_pipe_controller", instance);
            return instance;
        }

        public static Controller AddIfAbsent(Level level) {
            var instance = level.Tracker.GetEntity<Controller>();
            if (instance != null)
                return instance;

            var dyndata = DynamicData.For(level);

            instance = dyndata.Get<Controller>("raintools_pipe_controller");
            if (instance != null)
                return instance;

            level.Add(instance = new());
            dyndata.Set("raintools_pipe_controller", instance);

            return instance;
        }

        public void AddPart(Endpoint endpoint) {
            var pos = endpoint.AbsPosition;

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

        public void AddPart(Edge edge) {
            var start = edge.AbsPosition;
            var end = edge.EndPosition;

            bool startConnected = false, endConnected = false;
            bool addToEndOfStart = false, addToEndOfEnd = false;

            if (start == end) {
                Logger.Log(LogLevel.Verbose, nameof(RainToolsModule),
                           $"edge from ({start}) to ({end}) failed to join a pipe: loop");
                Add(new Pipe(edge));
                return;
            }

            if (discontiuities.TryGetValue(start, out var adj_a)) {
                discontiuities.Remove(start);
                startConnected = true;

                var pipe = adj_a.Pipe;

                if (adj_a == pipe.Parts[^1])
                    addToEndOfStart = true;

                pipe.Add(edge, addToEndOfStart);
            } else {
                discontiuities.Add(start, edge);
            }

            if (discontiuities.TryGetValue(end, out var adj_b)) {
                discontiuities.Remove(end);
                endConnected = true;

                var pipe = adj_b.Pipe;
                if (edge.Pipe == pipe) {
                    Logger.Log(LogLevel.Verbose, nameof(RainToolsModule),
                               $"edge from ({start}) to ({end}) formed a loop");
                    return;
                }

                if (adj_b == pipe.Parts[^1])
                    addToEndOfEnd = true;

                if (startConnected) {
                    edge.Pipe.AddFrom(pipe, addToEndOfStart, addToEndOfEnd);
                    Remove(pipe);
                } else {
                    pipe.Add(edge, addToEndOfEnd);
                }
            } else {
                discontiuities.Add(end, edge);
            }

            if (!startConnected && !endConnected)
                Add(new Pipe(edge));
        }

    }
}