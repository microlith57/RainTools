using System.Collections.Generic;

namespace Celeste.Mod.RainTools {
    public class RainToolsModuleSession : EverestModuleSession {

        public Dictionary<string, float> CycleProgressions { get; set; } = new();

        public HashSet<string> VisitedSubregionIDs { get; set; } = new();

        public string CurrentSubregionID { get; set; }

    }
}
