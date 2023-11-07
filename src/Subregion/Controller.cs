﻿using Celeste.Mod.Entities;
using Celeste.Mod.RainTools.Triggers;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.RainTools.Subregion {

    [CustomEntity($"RainTools/SubregionController")]
    [GlobalEntity]
    [Tracked]
    public class Controller : Entity {

        private enum ShowModes {
            Always,
            OncePerSession,
            OncePerFile
        };

        private enum TriggerModes {
            EnterRoom,
            TriggerDetect,
            TriggerOnly
        }

        private ShowModes ShowMode;

        private TriggerModes TriggerMode;

        private static bool justDied;

        private string CycleTag;

        private string Exclude;

        private string OnlyIn;

        private string RegionDialogKey;

        private string SubregionDialogKey;

        public string SubregionID;

        public Controller(EntityData data, Vector2 offset) : base() {
            Tag = Tags.Global | Tags.TransitionUpdate;

            #region Entity Data
            CycleTag = data.Attr("cycleTag", "").Trim();
            Exclude = data.Attr("exclude", "");
            OnlyIn = data.Attr("onlyIn", "");
            ShowMode = data.Enum("showMode", ShowModes.OncePerSession);
            RegionDialogKey = data.Attr("regionDialogKey", "");
            SubregionDialogKey = data.Attr("subregionDialogKey", "");
            SubregionID = data.Attr("subregionID", "default");
            TriggerMode = data.Enum("triggerMode", TriggerModes.EnterRoom);
            #endregion

            #region Components
            Add(new TransitionListener {
                OnOutBegin = () => {
                    HandleTransition(false);
                }
            });
            #endregion
        }

        /*
        public static void Load() {
            Everest.Events.Player.OnSpawn += Event_Player_OnSpawn;
        }

        public static void Unload() {
            Everest.Events.Player.OnSpawn -= Event_Player_OnSpawn;
        }
        */

        public void HandleTransition(bool fromTrigger) {
            // TODO: make this cleaner
            Level level = Scene as Level;
            string roomName = level.Session.LevelData.Name;

            // make sure the room is one of ours and its not trigger only
            if (!level.Session.MapData.ParseLevelsList(OnlyIn).Contains(roomName)
                || level.Session.MapData.ParseLevelsList(Exclude).Contains(roomName)
                || TriggerMode == TriggerModes.TriggerOnly && !fromTrigger && !justDied)
                return;

            // if in trigger detect mode, the room has a trigger, and this was not from a trigger, return
            if (TriggerMode == TriggerModes.TriggerDetect
                && level.Tracker.GetEntity<SubregionTextElementTrigger>() != null
                && !fromTrigger
                && !justDied)
                return;

            // return if we're in the same subregion
            if (RainToolsModule.Session.CurrentSubregionID == SubregionID && !justDied)
                return;

            // update our current subregion
            RainToolsModule.Session.CurrentSubregionID = SubregionID;
            
            // if it's once per session or once per file blah blah blah you get it
            if ((RainToolsModule.Session.VisitedSubregionIDs.Contains(SubregionID) && ShowMode == ShowModes.OncePerSession
                || RainToolsModule.SaveData.VisitedSubregionIDs.Contains(SubregionID) && ShowMode == ShowModes.OncePerFile)
                && !justDied)
                return;

            // update where we've been
            if (!RainToolsModule.Session.VisitedSubregionIDs.Contains(SubregionID)) {
                RainToolsModule.Session.VisitedSubregionIDs.Add(SubregionID);
            }

            if (!RainToolsModule.SaveData.VisitedSubregionIDs.Contains(SubregionID)) {
                RainToolsModule.SaveData.VisitedSubregionIDs.Add(SubregionID);
            }
            
            // could make duration, easeTimer, and delay controllable
            Activate(delay: (fromTrigger || justDied) ? 0f : 1.5f);
        }

        public TextElement Activate(float duration = 10f, float easeTime = .25f, float delay = 1.5f) {
            TextElement element;
            Scene.Add(element = new TextElement(CycleTag, RegionDialogKey, SubregionDialogKey, duration, easeTime, delay));
            return element;
        }

        /*
        private static void Event_Player_OnSpawn(Player player) {
            justDied = player.JustRespawned;
        }
        */
    }
}
