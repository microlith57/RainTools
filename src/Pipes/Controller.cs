using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Celeste.Mod.RainTools.Pipes {
    [CustomEntity("RainTools/PipeController")]
    public class Controller : Entity {

        public List<Pipe> Pipes;

        private Dictionary<Vector2, Tuple<IPart, IPart>> graph;

        #region constructors

        public Controller() {
            Depth = -Depths.Top;
            Visible = false;
        }

        public Controller(EntityData data) : this() => SetData(data);

        public static Controller AddIfAbsent(Level level) {
            var instance = level.Tracker.GetEntity<Controller>();

            if (instance == null)
                level.Add(instance = new());

            return instance;
        }

        public static Controller Load(Level level, LevelData levelData, Vector2 offset, EntityData data) {
            var existing = level.Tracker.GetEntity<Controller>();

            if (existing == null)
                return new Controller(data);

            existing.SetData(data);
            return null;
        }

        private void SetData(EntityData data) {
            // todo
        }

        #endregion

        #region add to level

        public void Add(Endpoint endpoint) {
            var pos = endpoint.Position;

            if (!graph.ContainsKey(pos))
                graph[pos] = new(endpoint, null);
            else if (graph[pos].Item2 != null)
                if (graph[pos].Item1 is not Endpoint)
                    graph[pos] = new(graph[pos].Item1, endpoint);
                else
                    throw new Exception("oh no");
            else
                throw new Exception("oh no");
        }

        public void Add(Segment segment) {
            var start = segment.Position;
            var end = segment.EndPosition;

            if (start == end)
                throw new Exception("oh no");

            if (!graph.ContainsKey(start))
                graph[start] = new(segment, null);
            else if (graph[start].Item2 == null)
                graph[start] = new(graph[start].Item1, segment);
            else
                throw new Exception("oh no");

            if (!graph.ContainsKey(end))
                graph[end] = new(segment, null);
            else if (graph[end].Item2 == null)
                graph[end] = new(graph[end].Item1, segment);
            else
                throw new Exception("oh no");
        }

        #endregion

        #region awake

        public override void Awake(Scene scene) {
            base.Awake(scene);

            var endpoints = scene.Tracker.GetEntities<Endpoint>().Cast<Endpoint>().ToList();
            var segments = scene.Tracker.GetEntities<Segment>().Cast<Segment>().ToList();

            Pipes = [];

            while (endpoints.Count > 0) {
                var start = endpoints[^1];
                var pipe = new Pipe() { Start = start };
                start.Pipe = pipe;
                endpoints.RemoveAt(endpoints.Count - 1);

                IPart part = pipe.Start;
                while (true) {
                    var tup = graph[part.EndPosition];
                    var next = (tup.Item2 == part) ? tup.Item1 : tup.Item2;
                    if (next == part || next == null)
                        throw new NotImplementedException("oh no");

                    next.Pipe = pipe;

                    if (next is Segment seg) {
                        pipe.Segments.Add(seg);
                        seg.StartDistance = pipe.TotalLength;
                        segments.Remove(seg);
                    } else if (next is Endpoint end) {
                        pipe.End = end;
                        endpoints.Remove(end);
                        break;
                    } else
                        throw new NotImplementedException("should be unreachable!");

                    pipe.TotalLength += next.Length;
                    part = next;
                }

                Add(pipe);
            }
        }

        #endregion

    }
}