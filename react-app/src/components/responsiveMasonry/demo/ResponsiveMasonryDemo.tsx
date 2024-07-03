import ResponsiveMasonry from '../ResponsiveMasonry'
import type { useResponsiveColumnsCallbackParams } from '@/hooks/useResponsiveColumns'
const getGridProps = (params: useResponsiveColumnsCallbackParams) => {
  if (params.containerWidth < 700) {
    return {
      gutter: 10,
      minWidth: 200,
    }
  }
  if (params.viewportWidth >= 700 && params.containerWidth < 1000) {
    return {
      gutter: 5,
      minWidth: 400,
    }
  }
  return {
    gutter: 20,
    minWidth: 700,
  }
}

export default function TestResponsiveMasonry() {
  const divArray = Array.from({ length: 50 }, (_, i) => i)
  const heights = [100, 200, 300]

  return (
    <ResponsiveMasonry getGridProps={getGridProps}>
      {divArray.map((_, index) => (
        <div
          key={index}
          style={{
            border: '1px solid black',
            padding: '10px',
            height: heights[Math.floor(Math.random() * heights.length)],
          }}
        >
          {index}
        </div>
      ))}
    </ResponsiveMasonry>
  )
}
