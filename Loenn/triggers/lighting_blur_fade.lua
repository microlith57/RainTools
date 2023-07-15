local enums = require("consts.celeste_enums")

local lightFade = {}

lightFade.name = "RainTools/LightingBlurFade"
lightFade.fieldInformation = {
  positionMode = {
    options = enums.trigger_position_modes,
    editable = false
  }
}
lightFade.placements = {
  name = "lighting_blur_fade",
  data = {
    positionMode = "NoEffect",
    blur1From = 1,
    blur1To = 1,
    blur2From = 1,
    blur2To = 1,
    fadeBlur1 = true,
    fadeBlur2 = true,
    tag = "sun"
  }
}

return lightFade
