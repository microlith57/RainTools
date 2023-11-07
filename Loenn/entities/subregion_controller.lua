local utils = require("utils")
local drawableSprite = require("structs.drawable_sprite")
local drawableLine = require("structs.drawable_line")

local subregion_controller = {}

subregion_controller.name = "RainTools/SubregionController"
subregion_controller.depth = 0
subregion_controller.placements = {
  name = "controller",
  data = {
    cycleTag = "",
    onlyIn = "",
    exclude = "",
    regionDialogKey = "",
    subregionDialogKey = "",
    subregionID = "",
    showMode = "OncePerSession",
    triggerMode = "EnterRoom"
  }
}

subregion_controller.fieldInformation = {
  showMode = {
    options = {"Always", "OncePerSession", "OncePerFile"},
    editable = false
  },
  triggerMode = {
    options = {"EnterRoom", "TriggerDetect", "TriggerOnly"},
    editable = false
  }
}

subregion_controller.fieldOrder = {
  "x", "y",
  "cycleTag", "subregionID",
  "regionDialogKey", "subregionDialogKey",
  "onlyIn", "exclude",
  "showMode", "triggerMode"
}

function subregion_controller.sprite(room, entity)
  local baseSprite = drawableSprite.fromTexture("RainTools/subregion_controller", entity)
  local errorSprite

  if (entity.cycleTag or "") == "" or (entity.regionDialogKey or "") == "" or (entity.subregionDialogKey or "") == "" or (entity.subregionID or "") == "" then
    errorSprite = drawableSprite.fromTexture("RainTools/empty_controller", entity)
    errorSprite.color = {1, 0, 0, 1}
  end

  return {baseSprite, errorSprite}
end

return subregion_controller
