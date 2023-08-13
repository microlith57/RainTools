local utils = require("utils")
local drawableSprite = require("structs.drawable_sprite")
local drawableLine = require("structs.drawable_line")

local angle_cycle_controller = {}

angle_cycle_controller.name = "RainTools/StylegroundAngleCycleController"
angle_cycle_controller.depth = 0
angle_cycle_controller.placements = {
  name = "controller",
  data = {
    cycleTag = "",
    styleTag = "",
    angleMultiplier = 1,
    angleOffsetDegrees = 0,
    flag = ""
  }
}
angle_cycle_controller.fieldInformation = {
  angleMultiplier = {
    fieldType = "integer"
  }
}
angle_cycle_controller.fieldOrder = {
  "x", "y",
  "cycleTag", "styleTag",
  "angleMultiplier", "angleOffsetDegrees"
}

function angle_cycle_controller.sprite(room, entity)
  local baseSprite = drawableSprite.fromTexture("RainTools/cycle_controller_base", entity)
  local frontSprite = drawableSprite.fromTexture("RainTools/cycle_controller_sunlight_angle", entity)
  local errorSprite

  if ((entity.cycleTag or "") == "") or ((entity.styleTag or "") == "") then
    errorSprite = drawableSprite.fromTexture("RainTools/empty_controller", entity)
    errorSprite.color = {1, 0, 0, 1}
  end

  return {baseSprite, frontSprite, errorSprite}
end

return angle_cycle_controller
