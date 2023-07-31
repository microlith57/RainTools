local enums = require("consts.celeste_enums")

local lightFade = {}

lightFade.name = "RainTools/AltColorgradeFade"
lightFade.fieldInformation = {
  positionMode = {
    options = enums.trigger_position_modes,
    editable = false
  },
}
lightFade.placements = {
  name = "trigger",
  data = {
    positionMode = "NoEffect",
    alphaFrom = 1,
    alphaTo = 1,
    tag = "sun"
  }
}

return lightFade
