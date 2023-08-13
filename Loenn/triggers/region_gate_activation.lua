local region_gate_activation = {}

region_gate_activation.name = "RainTools/RegionGateActivationZone"
region_gate_activation.placements = {
  name = "trigger",
  data = {side = "Left"}
}
region_gate_activation.fieldInformation = {
  side = {
    options = {"Left", "Right"},
    editable = false,
    activationDelay = 1.0
  }
}
region_gate_activation.fieldInformation = {
  activationDelay = {minimumValue = 0.0, allowEmpty = false}
}

return region_gate_activation
