local ambient_light = {}

ambient_light.name = "RainTools/AmbientLight"
ambient_light.canForeground = true
ambient_light.canBackground = false
ambient_light.defaultData = {
  lightColor = "ffffff",
}
ambient_light.fieldInformation = {
  lightColor = {fieldType = "color"}
}

return ambient_light
