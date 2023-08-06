local mods = require("mods")
local utils = require("utils")
local drawableSprite = require("structs.drawable_sprite")
local drawableLine = require("structs.drawable_line")

local easings = mods.requireFromPlugin("libraries.easings")

local cloudscape_cycle_controller = {}

cloudscape_cycle_controller.name = "RainTools/CloudscapeCycleController"
cloudscape_cycle_controller.depth = 0
cloudscape_cycle_controller.nodeLimits = {1, 1}
cloudscape_cycle_controller.nodeVisibility = "always"
cloudscape_cycle_controller.placements = {
  name = "controller",
  data = {
    cycleTag = "",
    styleTag = "",
    ringColors = "6d8ada,aea0c1,d9cbbc",
    ringEase = "Linear",
    backgroundColor = "4f9af7",
    backgroundEase = "Linear"
  }
}
cloudscape_cycle_controller.fieldInformation = {
  backgroundColor = {fieldType = "color"},
  ringEase = {
    options = easings,
    editable = true
  },
  backgroundEase = {
    options = easings,
    editable = true
  }
}
cloudscape_cycle_controller.fieldOrder = {
  "x", "y",
  "cycleTag", "styleTag",
  "ringColors", "ringEase",
  "backgroundColor", "backgroundEase"
}

function cloudscape_cycle_controller.sprite(room, entity)
  local baseSprite = drawableSprite.fromTexture("RainTools/cycle_controller_base", entity)
  local frontSprite = drawableSprite.fromTexture("RainTools/cycle_controller_cloudscape", entity)
  local errorSprite
  baseSprite.color = nil

  local success, r, g, b = utils.parseHexColor(entity.backgroundColor)
  if success then
    frontSprite.color = {r, g, b, 1}
  end

  if ((entity.cycleTag or "") == "") or ((entity.styleTag or "") == "") then
    errorSprite = drawableSprite.fromTexture("RainTools/empty_controller", entity)
    errorSprite.color = {1, 0, 0, 1}
  end

  return {baseSprite, frontSprite, errorSprite}
end

function cloudscape_cycle_controller.nodeSprite(room, entity, node, index)
  return drawableLine.fromPoints({entity.x, entity.y, node.x, node.y}, {1, 1, 1, 0.4}):getDrawableSprite()
end

function cloudscape_cycle_controller.nodeRectangle(room, entity, node)
  return utils.rectangle(node.x - 4, node.y - 4, 8, 8)
end

return cloudscape_cycle_controller
