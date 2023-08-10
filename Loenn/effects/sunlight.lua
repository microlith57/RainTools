local sunlight = {}

sunlight.name = "RainTools/Sunlight"
sunlight.canForeground = true
sunlight.canBackground = false
sunlight.defaultData = {
  lightColor = "ffffff",
  alpha = 1.0,
  angleDegrees = 0.0,
  blur1 = 2.0,
  blur2 = 1.0
}
sunlight.fieldInformation = {
  lightColor = {fieldType = "color"}
}
sunlight.fieldOrder = {
  "lightColor", "alpha",
  "angleDegrees",
  "blur1", "blur2"
}

return sunlight
