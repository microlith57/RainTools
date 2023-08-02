local mods = require("mods")
local utils = require("utils")
local drawableSprite = require("structs.drawable_sprite")
local drawableLine = require("structs.drawable_line")

local easings = mods.requireFromPlugin("libraries.easings")

local styleground_cycle_controller = {}

styleground_cycle_controller.name = "RainTools/StylegroundCycleController"
styleground_cycle_controller.depth = 0
styleground_cycle_controller.nodeLimits = {1, 1}
styleground_cycle_controller.nodeVisibility = "always"
styleground_cycle_controller.placements = {
  name = "controller",
  data = {
    cycleTag = "",
    styleTag = "",
    color = "ffffff",
    colorEase = "Linear",
    alpha = 1.0,
    alphaEase = "Linear",
    mode = "ColorAndAlpha"
  }
}
styleground_cycle_controller.fieldInformation = {
  mode = {
    options = {"ColorTimesPrevA", "ColorTimesAlpha", "ColorOnly", "AlphaOnly", "ColorAndAlpha"},
    editable = false
  },
  color = {fieldType = "color"},
  colorEase = {
    options = easings,
    editable = true
  },
  alphaEase = {
    options = easings,
    editable = true
  }
}
styleground_cycle_controller.fieldOrder = {
  "x", "y",
  "cycleTag", "styleTag",
  "color", "colorEase",
  "alpha", "alphaEase",
  "mode"
}

function styleground_cycle_controller.sprite(room, entity)
  local baseSprite = drawableSprite.fromTexture("RainTools/cycle_controller_base", entity)
  local frontSprite = drawableSprite.fromTexture("RainTools/cycle_controller_styleground", entity)
  local errorSprite
  baseSprite.color = nil

  if entity.mode == "AlphaOnly" then
    frontSprite.color = nil
  end

  if ((entity.cycleTag or "") == "") or ((entity.styleTag or "") == "") then
    errorSprite = drawableSprite.fromTexture("RainTools/empty_controller", entity)
    errorSprite.color = {1, 0, 0, 1}
  end

  return {baseSprite, frontSprite, errorSprite}
end

function styleground_cycle_controller.nodeSprite(room, entity, node, index)
  return drawableLine.fromPoints({entity.x, entity.y, node.x, node.y}, {1, 1, 1, 0.4}):getDrawableSprite()
end

function styleground_cycle_controller.nodeRectangle(room, entity, node)
  return utils.rectangle(node.x - 4, node.y - 4, 8, 8)
end

return styleground_cycle_controller
