local utils = require("utils")

---

local do_it_for_the_line = {}

do_it_for_the_line.name = "RainTools/ShadowLine"
do_it_for_the_line.depth = -1100000
do_it_for_the_line.color = {0, 0, 0, 0.8}
do_it_for_the_line.nodeLimits = {1, -1}
do_it_for_the_line.nodeLineRenderType = "line"
do_it_for_the_line.nodeVisibility = "always"
do_it_for_the_line.placements = {
  name = "shadowLine",
  data = {
    offset = 0,
    length = 400,
    letsInLight = false,
    alpha = 1
  }
}
function do_it_for_the_line.rectangle(room, entity)
  return utils.rectangle(entity.x - 4, entity.y - 4, 8, 8)
end

---

local do_it_for_the_line_advanced = {}

do_it_for_the_line_advanced.name = "RainTools/ShadowLineLinearColors"
do_it_for_the_line_advanced.depth = -1100000
do_it_for_the_line_advanced.color = {0, 0, 0, 0.8}
do_it_for_the_line_advanced.nodeLimits = {1, -1}
do_it_for_the_line_advanced.nodeLineRenderType = "line"
do_it_for_the_line_advanced.nodeVisibility = "always"
do_it_for_the_line_advanced.placements = {
  name = "shadowLine",
  data = {
    offset = 0,
    length = 400,
    colorA = "000000",
    colorB = "000000",
    alphaA = 1,
    alphaB = 1
  }
}
function do_it_for_the_line_advanced.rectangle(room, entity)
  return utils.rectangle(entity.x - 4, entity.y - 4, 8, 8)
end

---

return {do_it_for_the_line, do_it_for_the_line_advanced}
