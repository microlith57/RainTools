using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using MonoMod.Utils;

namespace Celeste.Mod.RainTools {
    [Tracked(true)]
    public abstract class ShadowCaster : Entity {
        public readonly int MaxTriCount;

        public ShadowCaster(Vector2 position, int maxTriCount) : base(position) {
            MaxTriCount = maxTriCount;
            Tag |= Tags.Persistent;
        }

        public abstract void UpdateVerts(DirectionalLightingRenderer state);

        #region hooks

        private static string[] _shadowCasterNames;
        private static string[] shadowCasterNames {
            get {
                if (_shadowCasterNames != null)
                    return _shadowCasterNames;

                _shadowCasterNames = Tracker.TrackedEntityTypes
                    .SelectMany((pair) => {
                        if (pair.Value.Contains(typeof(ShadowCaster))) {
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

                return _shadowCasterNames;
            }
        }

        private struct ShadowData {
            public EntityData Data;
            public Vector2 Offset;

            public ShadowData(EntityData data, Vector2 offset) {
                this.Data = data;
                this.Offset = offset;
            }
        }

        internal static void Load() {
            Everest.Events.LevelLoader.OnLoadingThread += onLoad;
            On.Celeste.MapData.Load += onMapDataLoad;
        }

        internal static void Unload() {
            Everest.Events.LevelLoader.OnLoadingThread -= onLoad;
            On.Celeste.MapData.Load -= onMapDataLoad;
        }

        private static void onMapDataLoad(On.Celeste.MapData.orig_Load orig, MapData mapData) {
            orig(mapData);

            var dyndata = DynamicData.For(mapData);

            List<ShadowData> shadowData;
            if (dyndata.TryGet("raintools_shadowdata", out shadowData)) {
                shadowData.Clear();
            } else {
                shadowData = new();
                dyndata.Set("raintools_shadowdata", shadowData);
            }

            var names = shadowCasterNames;
            foreach (var levelData in mapData.Levels) {
                levelData.Entities.RemoveAll((e) => {
                    if (!names.Contains(e.Name))
                        return false;

                    shadowData.Add(new ShadowData(e, levelData.Position));
                    return true;
                });
            }
        }

        private static void onLoad(Level level) {
            var mapData = level.Session.MapData;
            var dyndata = DynamicData.For(mapData);

            if (dyndata.TryGet<List<ShadowData>>("raintools_shadowdata", out var shadowData)) {
                foreach (var shadow in shadowData) {
                    var loader = Level.EntityLoaders[shadow.Data.Name];
                    var entity = loader.Invoke(level, shadow.Data.Level, shadow.Offset, shadow.Data);
                    level.Add(entity);
                }
            }
        }

        #endregion
    }
}
