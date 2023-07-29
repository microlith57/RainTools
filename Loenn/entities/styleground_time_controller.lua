local utils = require("utils")
local drawableSprite = require("structs.drawable_sprite")
local drawableLine = require("structs.drawable_line")

local styleground_time_controller = {}

styleground_time_controller.name = "RainTools/StylegroundTimeController"
styleground_time_controller.depth = 0
styleground_time_controller.nodeLimits = {1, 1}
styleground_time_controller.nodeVisibility = "always"
styleground_time_controller.placements = {
  name = "controller",
  data = {
    tag = "sun",
    color = "ffffff",
    alpha = 1.0
  }
}
styleground_time_controller.fieldInformation = {
  color = {
    fieldType = "color"
  }
}

function styleground_time_controller.sprite(room, entity)
  local baseSprite = drawableSprite.fromTexture("RainTools/time_controller_base", entity)
  local frontSprite = drawableSprite.fromTexture("RainTools/time_controller_styleground", entity)
  baseSprite.color = nil

  return {baseSprite, frontSprite}
end

function styleground_time_controller.nodeSprite(room, entity, node, index)
  return drawableLine.fromPoints({entity.x, entity.y, node.x, node.y}, {1, 1, 1, 0.4}):getDrawableSprite()
end

function styleground_time_controller.nodeRectangle(room, entity, node)
  return utils.rectangle(node.x - 4, node.y - 4, 8, 8)
end

return styleground_time_controller
