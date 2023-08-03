local utils = require("utils")

---

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
    alpha = 1,
    concave = false
  }
}

---

local shadow_rect_advanced = {}

shadow_rect_advanced.name = "RainTools/ShadowRectangleLinearColors"
shadow_rect_advanced.depth = -1100000
shadow_rect_advanced.fillColor = {0, 0, 0, 0.4}
shadow_rect_advanced.borderColor = {0, 0, 0, 0.8}
shadow_rect_advanced.placements = {
  name = "shadowRect",
  data = {
    width = 8,
    height = 8,
    offset = 0,
    length = 400,
    topLeftColor = "000000",
    topRightColor = "000000",
    bottomLeftColor = "000000",
    bottomRightColor = "000000",
    topLeftAlpha = 1,
    topRightAlpha = 1,
    bottomLeftAlpha = 1,
    bottomRightAlpha = 1,
    concave = false
  }
}

---

return {shadow_rect, shadow_rect_advanced}
