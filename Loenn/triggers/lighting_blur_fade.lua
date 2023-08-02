local enums = require("consts.celeste_enums")

local lighting_blur_fade = {}

lighting_blur_fade.name = "RainTools/LightingBlurFade"
lighting_blur_fade.placements = {
  name = "trigger",
  data = {
    styleTag = "",
    positionMode = "NoEffect",
    mode = "Both",
    blur1From = 1,
    blur1To = 1,
    blur2From = 1,
    blur2To = 1
  }
}
lighting_blur_fade.fieldInformation = {
  positionMode = {
    options = enums.trigger_position_modes,
    editable = false
  },
  mode = {
    options = {"OnlyBlur2", "OnlyBlur1", "Both"},
    editable = false
  }
}
lighting_blur_fade._lonnExt_extendedText = function(trigger)
  return string.format("%s, %s, %s", trigger.tag or "no tag!", trigger.positionMode or "NoEffect", trigger.mode or "Both")
end

return lighting_blur_fade
