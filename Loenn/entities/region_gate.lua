local region_gate = {}

local AreaModes = {"SameAsCurrentMap", "Normal", "BSide", "CSide"}

region_gate.name = "RainTools/RegionGate"
region_gate.texture = "RainTools/empty_controller"
region_gate.placements = {
  name = "gate",
  data = {
    leftSID = "",
    leftSide = "SameAsCurrentMap",
    rightSID = "",
    rightSide = "SameAsCurrentMap"
  }
}
region_gate.fieldInformation = {
  leftSide = {options = AreaModes, editable = false},
  rightSide = {options = AreaModes, editable = false}
}

return region_gate
