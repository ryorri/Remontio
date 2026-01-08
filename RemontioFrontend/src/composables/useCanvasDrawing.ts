import { ref, nextTick } from 'vue'
import { pixelToMeter } from '@/helpers/distanceFormatter'
import type { PointDTO } from '@/backend/BackendBase'

export interface DistanceLabelPosition {
  index: number
  x: number
  y: number
  width: number
  height: number
}

export function useCanvasDrawing() {
  const points = ref<PointDTO[]>([])
  const canvas = ref<HTMLCanvasElement | null>(null)
  const polygonClosed = ref(false)
  const mousePosition = ref<{ x: number; y: number } | null>(null)
  const editingDistanceIndex = ref<number | null>(null)
  const editDistanceValue = ref<string>('')
  const distanceLabelPositions = ref<DistanceLabelPosition[]>([])
  const canvasInputPosition = ref<{ x: number; y: number } | null>(null)
  const canvasDistanceInput = ref<HTMLInputElement | null>(null)

  const pixelsToCm = (pixels: number): number => {
    return pixels * pixelToMeter * 100
  }

  const calculateDistance = (p1: PointDTO, p2: PointDTO): number => {
    const dx = (p2.x || 0) - (p1.x || 0)
    const dy = (p2.y || 0) - (p1.y || 0)
    const distanceInPixels = Math.sqrt(dx * dx + dy * dy)
    return pixelsToCm(distanceInPixels)
  }

  const calculatePolygonArea = (): number => {
    if (points.value.length < 3) return 0

    let area = 0
    const pts = points.value

    for (let i = 0; i < pts.length; i++) {
      const j = (i + 1) % pts.length
      const pointI = pts[i]
      const pointJ = pts[j]
      if (pointI && pointJ) {
        area += (pointI.x || 0) * (pointJ.y || 0)
        area -= (pointJ.x || 0) * (pointI.y || 0)
      }
    }

    return Math.abs(area / 2) * pixelToMeter * pixelToMeter
  }

  const drawCanvas = () => {
    if (!canvas.value) return

    const ctx = canvas.value.getContext('2d')
    if (!ctx) return

    ctx.clearRect(0, 0, canvas.value.width, canvas.value.height)

    const gridSpacing = 30
    ctx.strokeStyle = '#e0e0e0'
    ctx.lineWidth = 1
    for (let i = 0; i < canvas.value.width; i += gridSpacing) {
      ctx.beginPath()
      ctx.moveTo(i, 0)
      ctx.lineTo(i, canvas.value.height)
      ctx.stroke()
    }
    for (let i = 0; i < canvas.value.height; i += gridSpacing) {
      ctx.beginPath()
      ctx.moveTo(0, i)
      ctx.lineTo(canvas.value.width, i)
      ctx.stroke()
    }

    if (points.value.length === 0) return

    const originX = parseFloat(canvas.value.dataset.originX || '0')
    const originY = parseFloat(canvas.value.dataset.originY || '0')

    ctx.strokeStyle = '#4A90E2'
    ctx.fillStyle = 'rgba(74, 144, 226, 0.2)'
    ctx.lineWidth = 2

    ctx.beginPath()
    const firstPoint = points.value[0]
    if (!firstPoint) return
    ctx.moveTo(originX + (firstPoint.x || 0), originY + (firstPoint.y || 0))
    for (let i = 1; i < points.value.length; i++) {
      const point = points.value[i]
      if (point) {
        ctx.lineTo(originX + (point.x || 0), originY + (point.y || 0))
      }
    }
    if (points.value.length > 2) {
      ctx.closePath()
      ctx.fill()
    }
    ctx.stroke()

    points.value.forEach((point: PointDTO, index: number) => {
      const canvasX = originX + (point.x || 0)
      const canvasY = originY + (point.y || 0)

      ctx.fillStyle = '#4A90E2'
      ctx.beginPath()
      ctx.arc(canvasX, canvasY, 5, 0, 2 * Math.PI)
      ctx.fill()

      ctx.fillStyle = '#000'
      ctx.font = '12px Arial'
      ctx.fillText(`${index + 1}`, canvasX + 10, canvasY - 10)
    })

    distanceLabelPositions.value = []
    ctx.fillStyle = '#E74C3C'
    ctx.font = 'bold 13px Arial'
    for (let i = 0; i < points.value.length; i++) {
      const currentPoint = points.value[i]
      const nextPoint = points.value[(i + 1) % points.value.length]

      if (currentPoint && nextPoint && (i < points.value.length - 1 || points.value.length > 2)) {
        const distance = calculateDistance(currentPoint, nextPoint)
        const midX = originX + ((currentPoint.x || 0) + (nextPoint.x || 0)) / 2
        const midY = originY + ((currentPoint.y || 0) + (nextPoint.y || 0)) / 2

        const text = `${distance.toFixed(1)} cm`
        const metrics = ctx.measureText(text)
        const labelX = midX - metrics.width / 2 - 3
        const labelY = midY - 10
        const labelWidth = metrics.width + 6
        const labelHeight = 16

        distanceLabelPositions.value.push({
          index: i,
          x: labelX,
          y: labelY,
          width: labelWidth,
          height: labelHeight,
        })

        if (editingDistanceIndex.value === i) {
          ctx.fillStyle = 'rgba(74, 144, 226, 0.9)'
        } else {
          ctx.fillStyle = 'rgba(255, 255, 255, 0.8)'
        }
        ctx.fillRect(labelX, labelY, labelWidth, labelHeight)

        ctx.strokeStyle = editingDistanceIndex.value === i ? '#4A90E2' : '#E74C3C'
        ctx.lineWidth = 1
        ctx.strokeRect(labelX, labelY, labelWidth, labelHeight)

        ctx.fillStyle = editingDistanceIndex.value === i ? '#fff' : '#E74C3C'
        ctx.fillText(text, midX - metrics.width / 2, midY + 3)
      }
    }

    if (mousePosition.value && points.value.length > 0 && !polygonClosed.value) {
      const lastPoint = points.value[points.value.length - 1]
      if (lastPoint) {
        const lastCanvasX = originX + (lastPoint.x || 0)
        const lastCanvasY = originY + (lastPoint.y || 0)

        const relativeMouseX = mousePosition.value.x - originX
        const relativeMouseY = mousePosition.value.y - originY

        ctx.strokeStyle = '#999'
        ctx.lineWidth = 1
        ctx.setLineDash([5, 5])
        ctx.beginPath()
        ctx.moveTo(lastCanvasX, lastCanvasY)
        ctx.lineTo(mousePosition.value.x, mousePosition.value.y)
        ctx.stroke()
        ctx.setLineDash([])

        const previewDistance = calculateDistance(lastPoint, {
          x: relativeMouseX,
          y: relativeMouseY,
        })
        const midX = (lastCanvasX + mousePosition.value.x) / 2
        const midY = (lastCanvasY + mousePosition.value.y) / 2

        ctx.fillStyle = 'rgba(255, 255, 255, 0.9)'
        const previewText = `${previewDistance.toFixed(1)} cm`
        const previewMetrics = ctx.measureText(previewText)
        ctx.fillRect(midX - previewMetrics.width / 2 - 3, midY - 10, previewMetrics.width + 6, 16)

        ctx.fillStyle = '#666'
        ctx.font = 'bold 13px Arial'
        ctx.fillText(previewText, midX - previewMetrics.width / 2, midY + 3)
      }
    }
  }

  const addPointToCanvas = (event: MouseEvent) => {
    if (!canvas.value) return

    const rect = canvas.value.getBoundingClientRect()
    const clickX = event.clientX - rect.left
    const clickY = event.clientY - rect.top

    for (const label of distanceLabelPositions.value) {
      if (
        clickX >= label.x &&
        clickX <= label.x + label.width &&
        clickY >= label.y &&
        clickY <= label.y + label.height
      ) {
        startDistanceEdit(label.index)
        return
      }
    }

    if (polygonClosed.value) return

    if (points.value.length === 0) {
      points.value.push({ x: 0, y: 0 })
      polygonClosed.value = false
      if (!canvas.value.dataset.originX) {
        canvas.value.dataset.originX = clickX.toString()
        canvas.value.dataset.originY = clickY.toString()
      }
    } else {
      const originX = parseFloat(canvas.value.dataset.originX || '0')
      const originY = parseFloat(canvas.value.dataset.originY || '0')

      if (points.value.length >= 3) {
        const distanceToFirst = Math.sqrt(
          Math.pow(clickX - originX, 2) + Math.pow(clickY - originY, 2),
        )

        if (distanceToFirst < 10) {
          polygonClosed.value = true
          drawCanvas()
          return
        }
      }

      const relativeX = clickX - originX
      const relativeY = clickY - originY
      points.value.push({ x: relativeX, y: relativeY })
    }

    drawCanvas()
  }

  const clearCanvasPoints = () => {
    points.value = []
    polygonClosed.value = false
    if (canvas.value) {
      delete canvas.value.dataset.originX
      delete canvas.value.dataset.originY
    }
    mousePosition.value = null
    drawCanvas()
  }

  const undoLastPoint = () => {
    if (points.value.length > 0) {
      points.value.pop()
      polygonClosed.value = false

      if (points.value.length === 0 && canvas.value) {
        delete canvas.value.dataset.originX
        delete canvas.value.dataset.originY
        mousePosition.value = null
      }

      drawCanvas()
    }
  }

  const updateMousePosition = (event: MouseEvent) => {
    if (!canvas.value || points.value.length === 0) {
      mousePosition.value = null
      return
    }

    const rect = canvas.value.getBoundingClientRect()
    const x = event.clientX - rect.left
    const y = event.clientY - rect.top

    let isOverLabel = false
    for (const label of distanceLabelPositions.value) {
      if (
        x >= label.x &&
        x <= label.x + label.width &&
        y >= label.y &&
        y <= label.y + label.height
      ) {
        isOverLabel = true
        canvas.value.style.cursor = 'pointer'
        break
      }
    }

    if (!isOverLabel) {
      canvas.value.style.cursor = polygonClosed.value ? 'default' : 'crosshair'
    }

    mousePosition.value = { x, y }
    drawCanvas()
  }

  const startDistanceEdit = (index: number) => {
    const point1 = points.value[index]
    const point2 = points.value[(index + 1) % points.value.length]
    if (point1 && point2 && canvas.value) {
      editingDistanceIndex.value = index
      editDistanceValue.value = calculateDistance(point1, point2).toFixed(1)

      const originX = parseFloat(canvas.value.dataset.originX || '0')
      const originY = parseFloat(canvas.value.dataset.originY || '0')
      const midX = originX + ((point1.x || 0) + (point2.x || 0)) / 2
      const midY = originY + ((point1.y || 0) + (point2.y || 0)) / 2

      canvasInputPosition.value = {
        x: midX - 40,
        y: midY - 12,
      }

      nextTick(() => {
        canvasDistanceInput.value?.focus()
        canvasDistanceInput.value?.select()
      })

      drawCanvas()
    }
  }

  const cancelDistanceEdit = () => {
    editingDistanceIndex.value = null
    editDistanceValue.value = ''
    canvasInputPosition.value = null
    drawCanvas()
  }

  const applyDistanceEdit = (index: number) => {
    const newDistance = parseFloat(editDistanceValue.value)
    if (isNaN(newDistance) || newDistance <= 0) {
      cancelDistanceEdit()
      return
    }

    const point1 = points.value[index]
    const nextIndex = (index + 1) % points.value.length
    const point2 = points.value[nextIndex]
    if (!point1 || !point2) return

    const dx = (point2.x || 0) - (point1.x || 0)
    const dy = (point2.y || 0) - (point1.y || 0)
    const angle = Math.atan2(dy, dx)

    const newDistancePx = newDistance / 100 / pixelToMeter

    const newX = (point1.x || 0) + Math.cos(angle) * newDistancePx
    const newY = (point1.y || 0) + Math.sin(angle) * newDistancePx

    if (index === points.value.length - 1 && points.value.length > 2) {
      const offsetX = newX - (point2.x || 0)
      const offsetY = newY - (point2.y || 0)

      const firstPoint = points.value[0]
      if (firstPoint) {
        points.value[0] = {
          x: (firstPoint.x || 0) + offsetX,
          y: (firstPoint.y || 0) + offsetY,
        }
      }
    } else {
      const offsetX = newX - (point2.x || 0)
      const offsetY = newY - (point2.y || 0)

      for (let i = nextIndex; i < points.value.length; i++) {
        const point = points.value[i]
        if (point) {
          points.value[i] = {
            x: (point.x || 0) + offsetX,
            y: (point.y || 0) + offsetY,
          }
        }
      }
    }

    cancelDistanceEdit()
    drawCanvas()
  }

  return {
    points,
    canvas,
    polygonClosed,
    mousePosition,
    editingDistanceIndex,
    editDistanceValue,
    distanceLabelPositions,
    canvasInputPosition,
    canvasDistanceInput,
    calculateDistance,
    calculatePolygonArea,
    drawCanvas,
    addPointToCanvas,
    clearCanvasPoints,
    undoLastPoint,
    updateMousePosition,
    startDistanceEdit,
    cancelDistanceEdit,
    applyDistanceEdit,
  }
}
