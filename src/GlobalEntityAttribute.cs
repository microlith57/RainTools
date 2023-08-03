using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using MonoMod.Utils;

namespace Celeste.Mod.RainTools {
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class GlobalEntityAttribute : Attribute {
        private static string[] _entityNames;
        private static string[] entityNames {
            get {
                if (_entityNames != null)
                    return _entityNames;

                _entityNames = Tracker.TrackedEntityTypes
                    .SelectMany((pair) => {
                        if (pair.Key.IsDefined(typeof(GlobalEntityAttribute))) {
                            var attrs = pair.Key
                                .GetCustomAttributes<CustomEntityAttribute>()
                                .SelectMany((a) =>
                                    a.IDs.SelectMany((id) => {
                                        var parts = id.Split('=');
                                        if (parts.Length > 0)
                                            return new string[] { parts[0] };
                                        return new string[] { };
                                    })
                                );
                            return attrs.ToList();
                        }
                        return new List<string>();
                    }).ToArray();

                return _entityNames;
            }
        }

        internal static void Load() {
            On.Celeste.MapData.Load += onMapDataLoad;
            Everest.Events.LevelLoader.OnLoadingThread += onLoad;
        }

        internal static void Unload() {
            On.Celeste.MapData.Load -= onMapDataLoad;
            Everest.Events.LevelLoader.OnLoadingThread -= onLoad;
        }

        private static void onMapDataLoad(On.Celeste.MapData.orig_Load orig, MapData mapData) {
            orig(mapData);

            var dyndata = DynamicData.For(mapData);

            if (dyndata.TryGet<List<Tuple<EntityData, Vector2>>>("raintools_global_entities", out var entities)) {
                entities.Clear();
            } else {
                entities = new();
                dyndata.Set("raintools_global_entities", entities);
            }

            var names = entityNames;
            foreach (var levelData in mapData.Levels) {
                levelData.Entities.RemoveAll((e) => {
                    if (!names.Contains(e.Name))
                        return false;

                    entities.Add(new(e, levelData.Position));
                    return true;
                });
            }
        }

        private static void onLoad(Level level) {
            var mapData = level.Session.MapData;
            var dyndata = DynamicData.For(mapData);

            if (dyndata.TryGet<List<Tuple<EntityData, Vector2>>>("raintools_global_entities", out var entities)) {
                foreach (var entity in entities) {
                    var loader = Level.EntityLoaders[entity.Item1.Name];
                    var instance = loader.Invoke(level, entity.Item1.Level, entity.Item2, entity.Item1);
                    if (instance is not null)
                        level.Add(instance);
                }
            }
        }
    }
}
