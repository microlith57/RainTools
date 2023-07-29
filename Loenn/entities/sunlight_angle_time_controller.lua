local utils = require("utils")
local drawableSprite = require("structs.drawable_sprite")
local drawableLine = require("structs.drawable_line")

local sunlight_angle_time_controller = {}

sunlight_angle_time_controller.name = "RainTools/SunlightAngleTimeController"
sunlight_angle_time_controller.depth = 0
sunlight_angle_time_controller.placements = {
  name = "controller",
  data = {
    tag = "sun",
  }
}

function sunlight_angle_time_controller.sprite(room, entity)
  local baseSprite = drawableSprite.fromTexture("RainTools/time_controller_base", entity)
  local frontSprite = drawableSprite.fromTexture("RainTools/time_controller_sunlight_angle", entity)

  return {baseSprite, frontSprite}
end

return sunlight_angle_time_controller
