import { useState, useEffect } from 'react'

import useElementSize from '@/shared/lib/useElementSize'
import useViewportSize from '@/shared/lib/useViewportSize'

export interface useResponsiveColumnsCallbackParams {
  containerWidth: number
  viewportWidth: number
}

export interface useResponsiveColumnsCallbackReturnType {
  gutter: number
  minWidth: number
  forceMinWidth?: boolean
}

function useResponsiveColumns(
  fn: (params: useResponsiveColumnsCallbackParams) => useResponsiveColumnsCallbackReturnType,
) {
  const { ref, width: containerWidth } = useElementSize()
  const { width: viewportWidth } = useViewportSize()

  const [columnWidth, setColumnWidth] = useState('')
  const [columnHeight, setColumnHeight] = useState(0)
  const [_gutter, setGutter] = useState(0)

  useEffect(() => {
    if (containerWidth === 0) return

    const { gutter, minWidth, forceMinWidth } = fn({
      containerWidth,
      viewportWidth: viewportWidth,
    })

    setGutter(gutter)

    if (containerWidth < (minWidth + gutter) * 2) {
      setColumnWidth('100%')
      setColumnHeight(containerWidth)
    } else if (forceMinWidth) {
      setColumnWidth(`${minWidth}px`)
      setColumnHeight(minWidth)
    } else {
      let columns = Math.floor(containerWidth / (minWidth + gutter))
      let gridItemWidth = containerWidth / columns

      if (gridItemWidth < minWidth) {
        gridItemWidth = minWidth
      }

      columns = Math.floor(containerWidth / gridItemWidth)

      if (gutter > 0) {
        setColumnWidth(`calc((100% / ${columns}) - ${gutter}px)`)
        setColumnHeight(containerWidth / columns - gutter)
      } else {
        setColumnWidth(`calc(100% / ${columns})`)
        setColumnHeight(containerWidth / columns)
      }
    }
  }, [fn, containerWidth, viewportWidth])

  return { ref, columnWidth, columnHeight, gutter: _gutter }
}

export default useResponsiveColumns
