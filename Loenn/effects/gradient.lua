local gradient = {}

gradient.name = "RainTools/Gradient"
gradient.defaultData = {
  x = 160, y = 90,
  scrollX = 0.0, scrollY = 0.0,
  colors = "7bbedf,0c56c2",
  alpha = 1.0,
  angleDegrees = 0.0,
  gradientLength = 180,
  extendEnds = false
}
gradient.fieldOrder = {
  "x", "y", "scrollX", "scrollY",
  "colors", "alpha",
  "angleDegrees", "gradientLength",
  "extendEnds"
}

return gradient
