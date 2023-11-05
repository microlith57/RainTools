using System.Collections.Generic;

namespace Celeste.Mod.RainTools {
    public class RainToolsModuleSaveData : EverestModuleSaveData {

        public HashSet<string> VisitedSubregionIDs { get; set; } = new();

    }
}
