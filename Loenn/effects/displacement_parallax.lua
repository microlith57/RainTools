local displacement_parallax = {}

displacement_parallax.name = "RainTools/DisplacementParallax"
displacement_parallax.canForeground = true
displacement_parallax.canBackground = false
displacement_parallax.defaultData = {
  x = 0,
  y = 0,
  texture = "bgs/microlith57/RainTools/heat00",
  scrollx = 1,
  scrolly = 1,
  speedx = 0,
  speedy = 0,
  -- color = "ffffff",
  alpha = 1,
  flipx = false,
  flipy = false,
  loopx = true,
  loopy = true,
  instantIn = false,
  instantOut = false,
  fadeIn = false,
  wind = 0
}
-- color = {fieldType = "color"}
displacement_parallax.fieldInformation = {}

return displacement_parallax
