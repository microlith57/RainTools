local ambient_light = {}

ambient_light.name = "RainTools/AltColorgrade"
ambient_light.canForeground = true
ambient_light.canBackground = false
ambient_light.defaultData = {
  color = "ffffff",
  alpha = 1.0,
  colorgradeA = "none",
  colorgradeB = "none",
  blendFactor = 0.0
}
ambient_light.fieldInformation = {
  color = {fieldType = "color"}
}

return ambient_light
