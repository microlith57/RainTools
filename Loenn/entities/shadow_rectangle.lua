local utils = require("utils")

local shadow_rect = {}

shadow_rect.name = "RainTools/ShadowRectangle"
shadow_rect.depth = -1100000
shadow_rect.fillColor = {0, 0, 0, 0.4}
shadow_rect.borderColor = {0, 0, 0, 0.8}
shadow_rect.placements = {
  name = "shadowRect",
  data = {
    width = 8,
    height = 8,
    offset = 0,
    length = 400,
    letsInLight = false,
    alpha = 1
  }
}

return shadow_rect
