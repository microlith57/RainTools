local utils = require("utils")
local drawableSprite = require("structs.drawable_sprite")
local drawableLine = require("structs.drawable_line")

local alt_colorgrade_time_controller = {}

alt_colorgrade_time_controller.name = "RainTools/AltColorgradeTimeController"
alt_colorgrade_time_controller.depth = 0
alt_colorgrade_time_controller.nodeLimits = {1, 1}
alt_colorgrade_time_controller.nodeVisibility = "always"
alt_colorgrade_time_controller.placements = {
  name = "controller",
  data = {
    colorgrade = "none",
    tag = "",
    depth = 0,
    alpha = 1
  }
}

function alt_colorgrade_time_controller.sprite(room, entity)
  local baseSprite = drawableSprite.fromTexture("RainTools/time_controller_base", entity)
  local frontSprite = drawableSprite.fromTexture("RainTools/time_controller_colorgrade_alt", entity)

  return {baseSprite, frontSprite}
end

function alt_colorgrade_time_controller.nodeSprite(room, entity, node, index)
  return drawableLine.fromPoints({entity.x, entity.y, node.x, node.y}, {1, 1, 1, 0.4}):getDrawableSprite()
end

function alt_colorgrade_time_controller.nodeRectangle(room, entity, node)
  return utils.rectangle(node.x - 4, node.y - 4, 8, 8)
end

return alt_colorgrade_time_controller
