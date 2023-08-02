local ambient_light = {}

ambient_light.name = "RainTools/AmbientLight"
ambient_light.canForeground = true
ambient_light.canBackground = false
ambient_light.defaultData = {
  lightColor = "ffffff",
  alpha = 1.0
}
ambient_light.fieldInformation = {
  lightColor = {fieldType = "color"}
}
ambient_light.fieldOrder = {
  "lightColor", "alpha"
}

return ambient_light
