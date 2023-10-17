local mods = require("mods")
local utils = require("utils")
local drawableSprite = require("structs.drawable_sprite")
local drawableLine = require("structs.drawable_line")
local drawableRectangle = require("structs.drawable_rectangle")

---

local pipe_segment = {
  name = "RainTools/PipeSegment",
  depth = -1100000,
  color = {1, 1, 1, 0.8},
  nodeLimits = {1, -1},
  nodeVisibility = "never",
  placements = {
    {
      name = "fg",
      data = {
        depth = -10600
      }
    },
    {
      name = "bg",
      data = {
        depth = 8500
      }
    }
  },
  rectangle = function(room, entity)
    return utils.rectangle(entity.x + 2, entity.y + 2, 4, 4)
  end,
  nodeRectangle = function(room, entity, node)
    return utils.rectangle(node.x + 2, node.y + 2, 4, 4)
  end,
  sprite = function(room, entity)
    -- local sprites = {}

    local points = {entity.x + 4, entity.y + 4}
    for _, node in ipairs(entity.nodes) do
      table.insert(points, node.x + 4)
      table.insert(points, node.y + 4)
    end
    return drawableLine.fromPoints(points, {1, 1, 1, 0.4}):getDrawableSprite()
    -- table.insert(sprites, )

    -- local startRect = utils.rectangle(entity.x + 2, entity.y + 2, 4, 4)
    -- local endRect = utils.rectangle(entity.nodes[#entity.nodes].x + 2, entity.nodes[#entity.nodes].y + 2, 4, 4)
    -- table.insert(sprites, drawableRectangle.fromRectangle("bordered", startRect, {1, 1, 1, 0.4}, {0, 0, 0, 0}))
    -- table.insert(sprites, drawableRectangle.fromRectangle("bordered", endRect, {1, 1, 1, 0.4}, {0, 0, 0, 0}))

    -- return sprites
  end
}

-- local pipe_segment_curve = {
--   name = "RainTools/PipeSegmentCurve",
--   color = {1, 1, 1, 0.8},
--   nodeLimits = {2, 2},
--   nodeLineRenderType = "line",
--   nodeVisibility = "always",
--   placements = {
--     {
--       name = "fg",
--       depth = -10600
--     },
--     {
--       name = "bg",
--       depth = 8500
--     }
--   }
-- }

local pipe_entrance = {
  name = "RainTools/PipeEntrance",
  depth = -1100000,
  placements = {
    {
      name = "left",
      data = {
        direction = "left",
        directory = "objects/RainTools/shortcut_pipes/"
      }
    },
    {
      name = "right",
      data = {
        direction = "right",
        directory = "objects/RainTools/shortcut_pipes/"
      }
    },
    {
      name = "up",
      data = {
        direction = "up",
        directory = "objects/RainTools/shortcut_pipes/"
      }
    },
    {
      name = "down",
      data = {
        direction = "down",
        directory = "objects/RainTools/shortcut_pipes/"
      }
    }
  },
  rectangle = function(room, entity)
    return utils.rectangle(entity.x, entity.y, 8, 8)
  end,
  sprite = function(room, entity)
    local sprite = drawableSprite.fromTexture(entity.directory .. entity.direction, entity)

    sprite.x = entity.x + 4
    sprite.y = entity.y + 4

    return sprite
  end
}

---

return {pipe_segment, pipe_entrance}
