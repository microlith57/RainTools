local mods = require("mods")
local utils = require("utils")
local drawableSprite = require("structs.drawable_sprite")
local drawableLine = require("structs.drawable_line")

local easings = mods.requireFromPlugin("libraries.easings")

local gradient_cycle_controller = {}

gradient_cycle_controller.name = "RainTools/GradientCycleController"
gradient_cycle_controller.depth = 0
gradient_cycle_controller.nodeLimits = {1, 1}
gradient_cycle_controller.nodeVisibility = "always"
gradient_cycle_controller.placements = {
  name = "controller",
  data = {
    cycleTag = "",
    styleTag = "",
    colors = "7bbedf,0c56c2",
    ease = "Linear"
  }
}
gradient_cycle_controller.fieldInformation = {
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
gradient_cycle_controller.fieldOrder = {
  "x", "y",
  "cycleTag", "styleTag",
  "colors", "ease"
}

local function colors(entity)
  local parts = string.split(entity.colors or "", ",")()
  local first = parts[1] or "aa0000"
  local last = parts[#parts] or "aa0000"

  local success1, r1, g1, b1, a1 = utils.parseHexColor(first)
  local success2, r2, g2, b2, a2 = utils.parseHexColor(last)

  local res1 = success1 and {r1, g1, b1, a1} or {0.66, 0, 0, 1}
  local res2 = success2 and {r2, g2, b2, a2} or {0.66, 0, 0, 1}

  return res1, res2
end

function gradient_cycle_controller.sprite(room, entity)
  local fillSprite = drawableSprite.fromTexture("RainTools/controller_fill", entity)
  local gradientSprite = drawableSprite.fromTexture("RainTools/controller_gradient", entity)
  local baseSprite = drawableSprite.fromTexture("RainTools/cycle_controller_base", entity)
  local errorSprite
  baseSprite.color = nil

  if ((entity.cycleTag or "") == "") or ((entity.styleTag or "") == "") then
    errorSprite = drawableSprite.fromTexture("RainTools/empty_controller", entity)
    errorSprite.color = {1, 0, 0, 1}
  end

  local col1, col2 = colors(entity)
  fillSprite.color = col1
  gradientSprite.color = col2

  return {fillSprite, gradientSprite, baseSprite, errorSprite}
end

function gradient_cycle_controller.nodeSprite(room, entity, node, index)
  return drawableLine.fromPoints({entity.x, entity.y, node.x, node.y}, {1, 1, 1, 0.4}):getDrawableSprite()
end

function gradient_cycle_controller.nodeRectangle(room, entity, node)
  return utils.rectangle(node.x - 4, node.y - 4, 8, 8)
end

return gradient_cycle_controller
