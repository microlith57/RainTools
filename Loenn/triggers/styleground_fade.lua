local mods = require("mods")
local enums = require("consts.celeste_enums")

local easings = mods.requireFromPlugin("libraries.easings")

local lightFade = {}

lightFade.name = "RainTools/StylegroundFade"
lightFade.placements = {
  name = "trigger",
  data = {
    positionMode = "NoEffect",
    styleTag = "",
    colorFrom = "ffffff",
    colorTo = "ffffff",
    alphaFrom = 1,
    alphaTo = 1,
    colorEase = "Linear",
    alphaEase = "Linear",
    mode = "ColorAndAlpha"
  }
}
lightFade.fieldInformation = {
  positionMode = {
    options = enums.trigger_position_modes,
    editable = false
  },
  mode = {
    options = {"ColorTimesPrevA", "ColorTimesAlpha", "ColorOnly", "AlphaOnly", "ColorAndAlpha"},
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
lightFade._lonnExt_extendedText = function(trigger)
  return string.format("%s, %s, %s", trigger.tag or "no tag!", trigger.positionMode or "NoEffect", trigger.mode or "Both")
end

return lightFade
