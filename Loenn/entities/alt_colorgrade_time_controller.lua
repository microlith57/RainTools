local mods = require("mods")
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
    tag = "",
    depth = 0,
    colorgrade = "none",
    alpha = 1,
    colorgradeEase = "Linear",
    alphaEase = "Linear",
    mode = "Both"
  }
}
alt_colorgrade_time_controller.fieldInformation = {
  mode = {
    options = {"ColorgradeOnly", "AlphaOnly", "Both"},
    editable = false
  },
  colorgradeEase = {
    options = easings,
    editable = true
  },
  alphaEase = {
    options = easings,
    editable = true
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
