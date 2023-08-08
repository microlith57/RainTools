local utils = require("utils")
local drawableSprite = require("structs.drawable_sprite")
local drawableLine = require("structs.drawable_line")

local global_controller = {}

global_controller.name = "RainTools/GlobalEntityController"
global_controller.texture = "RainTools/global_controller"
global_controller.placements = {
  name = "controller",
  data = {
    setGlobalTag = true
  }
}

return global_controller
