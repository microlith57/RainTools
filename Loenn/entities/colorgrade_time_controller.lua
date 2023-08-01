local mods = require("mods")
local utils = require("utils")
local drawableSprite = require("structs.drawable_sprite")
local drawableLine = require("structs.drawable_line")

local colorgrade_time_controller = {}

colorgrade_time_controller.name = "RainTools/ColorgradeTimeController"
colorgrade_time_controller.depth = 0
colorgrade_time_controller.nodeLimits = {1, 1}
colorgrade_time_controller.nodeVisibility = "always"
colorgrade_time_controller.placements = {
  name = "controller",
  data = {
    colorgrade = "none",
    colorgradeEase = "Linear"
  }
}
colorgrade_time_controller.fieldInformation = {
  colorgradeEase = {
    options = easings,
    editable = true
  }
}

function colorgrade_time_controller.sprite(room, entity)
  local baseSprite = drawableSprite.fromTexture("RainTools/time_controller_base", entity)
  local frontSprite = drawableSprite.fromTexture("RainTools/time_controller_colorgrade", entity)

  return {baseSprite, frontSprite}
end

function colorgrade_time_controller.nodeSprite(room, entity, node, index)
  return drawableLine.fromPoints({entity.x, entity.y, node.x, node.y}, {1, 1, 1, 0.4}):getDrawableSprite()
end

function colorgrade_time_controller.nodeRectangle(room, entity, node)
  return utils.rectangle(node.x - 4, node.y - 4, 8, 8)
end

return colorgrade_time_controller
