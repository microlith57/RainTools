local sunlight = {}

sunlight.name = "RainTools/Sunlight"
sunlight.canForeground = true
sunlight.canBackground = false
sunlight.defaultData = {
  angle = 0.0,
  lightColor = "ffffff",
  intensity = 1.0,
  blur1 = 2.0,
  blur2 = 1.0
}
sunlight.fieldInformation = {
  lightColor = {fieldType = "color"}
}

return sunlight
