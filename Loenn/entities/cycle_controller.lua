local utils = require("utils")
local drawableSprite = require("structs.drawable_sprite")
local drawableLine = require("structs.drawable_line")

local cycle_controller = {}

cycle_controller.name = "RainTools/CycleController"
cycle_controller.texture = "RainTools/cycle_controller"
cycle_controller.depth = 0
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

return cycle_controller
