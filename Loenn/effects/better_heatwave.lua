local heatwave = {}

heatwave.name = "RainTools/BetterHeatwave"
heatwave.canForeground = true
heatwave.canBackground = false
heatwave.defaultData = {
  texture = "bgs/microlith57/RainTools/distortion/blob",
  blobCount = 100,
  minVelX = -1,
  minVelY = 1,
  maxVelX = -10,
  maxVelY = -20,
  minScaleVel = 0.05,
  maxScaleVel = 0.3,
  scaleAcceleration = -0.001,
  distortAlpha = 0.05
}

return heatwave
