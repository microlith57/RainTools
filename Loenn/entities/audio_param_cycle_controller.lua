local mods = require("mods")
local utils = require("utils")
local drawableSprite = require("structs.drawable_sprite")
local drawableLine = require("structs.drawable_line")

local easings = mods.requireFromPlugin("libraries.easings")

local audio_param_controller = {}

audio_param_controller.name = "RainTools/AudioParamCycleController"
audio_param_controller.depth = 0
audio_param_controller.nodeLimits = {1, 1}
audio_param_controller.nodeVisibility = "always"
audio_param_controller.placements = {
  name = "controller",
  data = {
    cycleTag = "",
    ambience = false,
    param = "fade",
    value = 0,
    ease = "Linear",
    flag = ""
  }
}
audio_param_controller.fieldInformation = {
  ease = {
    options = easings,
    editable = true
  }
}
audio_param_controller.fieldOrder = {
  "x", "y",
  "cycleTag",
  "ambience",
  "param", "value", "ease",
  "flag"
}

function audio_param_controller.sprite(room, entity)
  local baseSprite = drawableSprite.fromTexture("RainTools/cycle_controller_base", entity)
  local frontSprite = drawableSprite.fromTexture("RainTools/cycle_controller_audio", entity)
  local errorSprite

  if (entity.cycleTag or "") == "" or (entity.param or "fade") == "" then
    errorSprite = drawableSprite.fromTexture("RainTools/empty_controller", entity)
    errorSprite.color = {1, 0, 0, 1}
  end

  return {baseSprite, frontSprite, errorSprite}
end

function audio_param_controller.nodeSprite(room, entity, node, index)
  return drawableLine.fromPoints({entity.x, entity.y, node.x, node.y}, {1, 1, 1, 0.4}):getDrawableSprite()
end

function audio_param_controller.nodeRectangle(room, entity, node)
  return utils.rectangle(node.x - 4, node.y - 4, 8, 8)
end

return audio_param_controller
