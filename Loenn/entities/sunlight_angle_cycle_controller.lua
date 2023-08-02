local utils = require("utils")
local drawableSprite = require("structs.drawable_sprite")
local drawableLine = require("structs.drawable_line")

local sunlight_angle_cycle_controller = {}

sunlight_angle_cycle_controller.name = "RainTools/SunlightAngleCycleController"
sunlight_angle_cycle_controller.depth = 0
sunlight_angle_cycle_controller.placements = {
  name = "controller",
  data = {
    cycleTag = "",
    styleTag = "",
    angleOffset = 0,
    angleMultiplier = 1
  }
}
sunlight_angle_cycle_controller.fieldOrder = {
  "x", "y",
  "cycleTag", "styleTag",
  "angleOffset", "angleMultiplier"
}

function sunlight_angle_cycle_controller.sprite(room, entity)
  local baseSprite = drawableSprite.fromTexture("RainTools/cycle_controller_base", entity)
  local frontSprite = drawableSprite.fromTexture("RainTools/cycle_controller_sunlight_angle", entity)
  local errorSprite

  if ((entity.cycleTag or "") == "") or ((entity.styleTag or "") == "") then
    errorSprite = drawableSprite.fromTexture("RainTools/empty_controller", entity)
    errorSprite.color = {1, 0, 0, 1}
  end

  return {baseSprite, frontSprite, errorSprite}
end

return sunlight_angle_cycle_controller
