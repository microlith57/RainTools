local enums = require("consts.celeste_enums")

local lightFade = {}

lightFade.name = "RainTools/StylegroundFade"
lightFade.fieldInformation = {
  positionMode = {
    options = enums.trigger_position_modes,
    editable = false
  },
  mode = {
    options = {"ColorOnly", "AlphaOnly", "Both"},
    editable = false
  },
  colorFrom = {fieldType = "color"},
  colorTo = {fieldType = "color"}
}
lightFade.placements = {
  name = "trigger",
  data = {
    positionMode = "NoEffect",
    mode = "Both",
    alphaFrom = 1,
    alphaTo = 1,
    colorFrom = "ffffff",
    colorTo = "ffffff",
    tag = ""
  }
}

-- todo extended trigger text

return lightFade
