local enums = require("consts.celeste_enums")

local lightFade = {}

lightFade.name = "RainTools/LightingColorFade"
lightFade.fieldInformation = {
  positionMode = {
    options = enums.trigger_position_modes,
    editable = false
  },
  colorFrom = {
    fieldType = "color"
  },
  colorTo = {
    fieldType = "color"
  }
}
lightFade.placements = {
  name = "lighting_color_fade",
  data = {
    positionMode = "NoEffect",
    alphaFrom = 1,
    alphaTo = 1,
    colorFrom = "ffffff",
    colorTo = "ffffff",
    colorOnly = false,
    tag = "sun"
  }
}

return lightFade
