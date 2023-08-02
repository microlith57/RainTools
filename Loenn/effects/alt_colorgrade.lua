local alt_colorgrade = {}

alt_colorgrade.name = "RainTools/AltColorgrade"
alt_colorgrade.canForeground = true
alt_colorgrade.canBackground = false
alt_colorgrade.defaultData = {
  color = "ffffff",
  alpha = 1.0,
  colorgradeA = "none",
  colorgradeB = "none",
  blendFactor = 0.0
}
alt_colorgrade.fieldInformation = {
  color = {fieldType = "color"}
}
alt_colorgrade.fieldOrder = {
  "color", "alpha",
  "colorgradeA", "colorgradeB",
  "blendFactor"
}

return alt_colorgrade
