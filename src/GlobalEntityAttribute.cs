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
            if (dyndata.TryGet<List<Tuple<EntityData, Vector2, bool>>>("raintools_global_entities", out var savedEntities)) {
                savedEntities.Clear();
            } else {
                savedEntities = new();
                dyndata.Set("raintools_global_entities", savedEntities);
            }

            var names = entityNames;
            foreach (var levelData in mapData.Levels) {
                bool foundController = false;
                bool controllerSetsGlobalTag = false;

                levelData.Entities.RemoveAll(e => {
                    if (e.Name == "RainTools/GlobalEntityController") {
                        foundController = true;
                        controllerSetsGlobalTag = e.Bool("setGlobalTag", true);
                        return true;
                    }

                    if (names.Contains(e.Name)) {
                        savedEntities.Add(new(e, levelData.Position, false));
                        return true;
                    }

                    return false;
                });

                if (foundController) {
                    levelData.Entities.RemoveAll(e => {
                        if (!Level.EntityLoaders.ContainsKey(e.Name))
                            return false;

                        savedEntities.Add(new(e, levelData.Position, controllerSetsGlobalTag));
                        return true;
                    });

                    // todo change id for consistency with non-global triggers
                    levelData.Triggers.RemoveAll(e => {
                        if (!Level.EntityLoaders.ContainsKey(e.Name))
                            return false;

                        savedEntities.Add(new(e, levelData.Position, controllerSetsGlobalTag));
                        return true;
                    });
                }
            }
        }

        private static void onLoad(Level level) {
            var mapData = level.Session.MapData;
            var dyndata = DynamicData.For(mapData);

            if (dyndata.TryGet<List<Tuple<EntityData, Vector2, bool>>>("raintools_global_entities", out var savedEntities)) {
                foreach (var saved in savedEntities) {
                    var loader = Level.EntityLoaders[saved.Item1.Name];
                    var instance = loader.Invoke(level, saved.Item1.Level, saved.Item2, saved.Item1);

                    if (instance is null)
                        continue;

                    if (saved.Item3)
                        instance.Tag |= Tags.Global;

                    level.Add(instance);
                }
            }
        }
    }
}
