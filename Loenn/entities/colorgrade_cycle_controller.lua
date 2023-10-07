local mods = require("mods")
local utils = require("utils")
local drawableSprite = require("structs.drawable_sprite")
local drawableLine = require("structs.drawable_line")

local easings = mods.requireFromPlugin("libraries.easings")

local colorgrade_cycle_controller = {}

colorgrade_cycle_controller.name = "RainTools/ColorgradeCycleController"
colorgrade_cycle_controller.depth = 0
colorgrade_cycle_controller.nodeLimits = {1, 1}
colorgrade_cycle_controller.nodeVisibility = "always"
colorgrade_cycle_controller.placements = {
  name = "controller",
  data = {
    cycleTag = "",
    colorgrade = "none",
    colorgradeEase = "Linear",
    flag = ""
  }
}
colorgrade_cycle_controller.fieldInformation = {
  colorgradeEase = {
    options = easings,
    editable = true
  }
}
colorgrade_cycle_controller.fieldOrder = {
  "x", "y",
  "cycleTag",
  "colorgrade", "colorgradeEase",
  "flag"
}

function colorgrade_cycle_controller.sprite(room, entity)
  local baseSprite = drawableSprite.fromTexture("RainTools/cycle_controller_base", entity)
  local frontSprite = drawableSprite.fromTexture("RainTools/cycle_controller_colorgrade", entity)
  local errorSprite

  if (entity.cycleTag or "") == "" then
    errorSprite = drawableSprite.fromTexture("RainTools/empty_controller", entity)
    errorSprite.color = {1, 0, 0, 1}
  end

  return {baseSprite, frontSprite, errorSprite}
end

function colorgrade_cycle_controller.nodeSprite(room, entity, node, index)
  return drawableLine.fromPoints({entity.x, entity.y, node.x, node.y}, {1, 1, 1, 0.4}):getDrawableSprite()
end

function colorgrade_cycle_controller.nodeRectangle(room, entity, node)
  return utils.rectangle(node.x - 4, node.y - 4, 8, 8)
end

return colorgrade_cycle_controller
