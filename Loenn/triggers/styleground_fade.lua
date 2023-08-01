local mods = require("mods")
local enums = require("consts.celeste_enums")

local easings = mods.requireFromPlugin("libraries.easings")

local lightFade = {}

lightFade.name = "RainTools/StylegroundFade"
lightFade.placements = {
  name = "trigger",
  data = {
    positionMode = "NoEffect",
    tag = "",
    colorFrom = "ffffff",
    colorTo = "ffffff",
    alphaFrom = 1,
    alphaTo = 1,
    colorEase = "Linear",
    alphaEase = "Linear",
    mode = "Both"
  }
}
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
  colorTo = {fieldType = "color"},
  colorEase = {
    options = easings,
    editable = true
  },
  alphaEase = {
    options = easings,
    editable = true
  }
}

-- todo extended trigger text

return lightFade
