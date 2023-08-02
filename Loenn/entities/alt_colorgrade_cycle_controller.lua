local mods = require("mods")
local utils = require("utils")
local drawableSprite = require("structs.drawable_sprite")
local drawableLine = require("structs.drawable_line")

local alt_colorgrade_cycle_controller = {}

alt_colorgrade_cycle_controller.name = "RainTools/AltColorgradeCycleController"
alt_colorgrade_cycle_controller.depth = 0
alt_colorgrade_cycle_controller.nodeLimits = {1, 1}
alt_colorgrade_cycle_controller.nodeVisibility = "always"
alt_colorgrade_cycle_controller.placements = {
  name = "controller",
  data = {
    cycleTag = "",
    styleTag = "",
    colorgrade = "none",
    colorgradeEase = "Linear",
    alpha = 1,
    alphaEase = "Linear",
    mode = "Both"
  }
}
alt_colorgrade_cycle_controller.fieldInformation = {
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
alt_colorgrade_cycle_controller.fieldOrder = {
  "x", "y",
  "colorgrade", "colorgradeEase",
  "alpha", "alphaEase",
  "mode"
}

function alt_colorgrade_cycle_controller.sprite(room, entity)
  local baseSprite = drawableSprite.fromTexture("RainTools/cycle_controller_base", entity)
  local frontSprite = drawableSprite.fromTexture("RainTools/cycle_controller_colorgrade_alt", entity)

  return {baseSprite, frontSprite}
end

function alt_colorgrade_cycle_controller.nodeSprite(room, entity, node, index)
  return drawableLine.fromPoints({entity.x, entity.y, node.x, node.y}, {1, 1, 1, 0.4}):getDrawableSprite()
end

function alt_colorgrade_cycle_controller.nodeRectangle(room, entity, node)
  return utils.rectangle(node.x - 4, node.y - 4, 8, 8)
end

return alt_colorgrade_cycle_controller
