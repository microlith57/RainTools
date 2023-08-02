local utils = require("utils")
local drawableSprite = require("structs.drawable_sprite")
local drawableLine = require("structs.drawable_line")

local flag_cycle_controller = {}

flag_cycle_controller.name = "RainTools/FlagCycleController"
flag_cycle_controller.depth = 0
flag_cycle_controller.nodeLimits = {1, 1}
flag_cycle_controller.nodeVisibility = "always"
flag_cycle_controller.placements = {
  name = "controller",
  data = {
    cycleTag = "",
    flag = "",
    stateOnEnter = true,
    stateOnLeave = false,
    arcAngle = 0.01
  }
}
flag_cycle_controller.fieldOrder = {
  "x", "y",
  "cycleTag",
  "flag",
  "stateOnEnter", "stateOnLeave",
  "arcAngle"
}

function flag_cycle_controller.sprite(room, entity)
  local baseSprite = drawableSprite.fromTexture("RainTools/cycle_controller_base", entity)
  local frontSprite = drawableSprite.fromTexture("RainTools/cycle_controller_flag", entity)
  local errorSprite

  if (entity.cycleTag or "") == "" then
    errorSprite = drawableSprite.fromTexture("RainTools/empty_controller", entity)
    errorSprite.color = {1, 0, 0, 1}
  end

  return {baseSprite, frontSprite, errorSprite}
end

function flag_cycle_controller.nodeSprite(room, entity, node, index)
  -- todo draw whole wedge
  return drawableLine.fromPoints({entity.x, entity.y, node.x, node.y}, {1, 1, 1, 0.4}):getDrawableSprite()
end

function flag_cycle_controller.nodeRectangle(room, entity, node)
  return utils.rectangle(node.x - 4, node.y - 4, 8, 8)
end

return flag_cycle_controller
