// Conversion constant: 600 pixels = 20 meters (2000 cm)
// 1 pixel = 3.33 cm = 0.0333 meters
export const pixelToMeter = 0.0333

export const cm2ToM2 = (cm2: number): number => {
  return cm2 * pixelToMeter * pixelToMeter
}

export const m2ToCm2 = (m2: number): number => {
  return m2 / (pixelToMeter * pixelToMeter)
}
