﻿using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.RainTools.Subregion {

    [CustomEntity($"RainTools/SubregionController")]
    [GlobalEntity]
    [Tracked]
    public class Controller : Entity {

        private enum ShowModeOptions {
            Always,
            OncePerSession,
            OncePerFile
        };

        private bool TriggerOnly;

        private ShowModeOptions ShowMode;

        private string CycleTag;

        private string DialogKey;

        private string Exclude;

        private string OnlyIn;

        public string SubregionID;

        public Controller(EntityData data, Vector2 offset) : base() {
            Tag = Tags.Global | Tags.TransitionUpdate;

            #region Entity Data
            CycleTag = data.Attr("cycleTag", "").Trim();
            DialogKey = data.Attr("dialogKey", "");
            Exclude = data.Attr("exclude", "");
            OnlyIn = data.Attr("onlyIn", "");
            ShowMode = data.Enum("showMode", ShowModeOptions.OncePerSession);
            SubregionID = data.Attr("subregionID", "default");
            TriggerOnly = data.Bool("triggerOnly", false);
            #endregion

            #region Components
            Add(new TransitionListener {
                // check rooms
                OnOutBegin = () => {
                    HandleTransition(false);
                }
            });
            #endregion
        }

        public void HandleTransition(bool fromTrigger) {
            // TODO: make this cleaner
            Level level = Scene as Level;
            string roomName = level.Session.LevelData.Name;

            // make sure the room is one of ours and its not trigger only
            if (!level.Session.MapData.ParseLevelsList(OnlyIn).Contains(roomName)
                || level.Session.MapData.ParseLevelsList(Exclude).Contains(roomName)
                || TriggerOnly && !fromTrigger)
                return;

            // return if we're in the same subregion
            if (RainToolsModule.Session.CurrentSubregionID == SubregionID)
                return;

            // update our current subregion
            RainToolsModule.Session.CurrentSubregionID = SubregionID;
            
            // if it's once per session or once per file blah blah blah you get it
            if ((RainToolsModule.Session.VisitedSubregionIDs.Contains(SubregionID) && ShowMode == ShowModeOptions.OncePerSession)
                || (RainToolsModule.SaveData.VisitedSubregionIDs.Contains(SubregionID) && ShowMode == ShowModeOptions.OncePerFile))
                return;

            // update where we've been
            if (!RainToolsModule.Session.VisitedSubregionIDs.Contains(SubregionID)) {
                RainToolsModule.Session.VisitedSubregionIDs.Add(SubregionID);
            }

            if (!RainToolsModule.SaveData.VisitedSubregionIDs.Contains(SubregionID)) {
                RainToolsModule.SaveData.VisitedSubregionIDs.Add(SubregionID);
            }
            
            Activate(delay: fromTrigger ? 0f : 1.5f);
        }

        public TextElement Activate(float duration = 10f, float easeTime = .25f, float delay = 1.5f) {
            TextElement element;
            Scene.Add(element = new TextElement(CycleTag, DialogKey, duration, easeTime, delay));
            return element;
        }

    }
}
