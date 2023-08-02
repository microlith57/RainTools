local utils = require("utils")
local drawableSprite = require("structs.drawable_sprite")
local drawableLine = require("structs.drawable_line")

local cycle_controller = {}

cycle_controller.name = "RainTools/CycleController"
cycle_controller.depth = -1
cycle_controller.placements = {
  name = "controller",
  data = {
    cycleTag = "",
    progressionOffset = 0,
    secondsPerCycle = 0,
    transitionUpdate = true,
    frozenUpdate = true,
    -- pauseUpdate = false, -- todo add this later once it doesn't look bad
    flag = "",
  }
}
cycle_controller.fieldOrder = {
  "x", "y",
  "cycleTag",
  "progressionOffset", "secondsPerCycle",
  "transitionUpdate", "frozenUpdate", -- "pauseUpdate",
  "flag"
}

function cycle_controller.sprite(room, entity)
  local baseSprite = drawableSprite.fromTexture("RainTools/cycle_controller", entity)
  local errorSprite

  if (entity.cycleTag or "") == "" then
    errorSprite = drawableSprite.fromTexture("RainTools/empty_controller", entity)
    errorSprite.color = {1, 0, 0, 1}
  end

  return {baseSprite, errorSprite}
end

return cycle_controller
