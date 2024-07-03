import useResponsiveColumns from '@/hooks/useResponsiveColumns'
import type {
  useResponsiveColumnsCallbackParams,
  useResponsiveColumnsCallbackReturnType,
} from '@/hooks/useResponsiveColumns'
import React, { useEffect, useRef, useState } from 'react'
import { flushSync } from 'react-dom'
import { css } from '@emotion/css'

export interface ResponsiveMasonryProps {
  /** Responsive grid params */
  getGridProps: (
    params: useResponsiveColumnsCallbackParams,
  ) => useResponsiveColumnsCallbackReturnType

  /** The content of the component */
  children?: React.ReactNode
}

const parseToNumber = (val: string) => {
  return Number(val.replace('px', ''))
}

function ResponsiveMasonry({ children, getGridProps }: ResponsiveMasonryProps) {
  const { ref: masonryRef, columnWidth, gutter } = useResponsiveColumns(getGridProps)

  const [maxColumnHeight, setMaxColumnHeight] = useState(0)
  const [numberOfLineBreaks, setNumberOfLineBreaks] = useState(0)

  const handleResize = (masonryChildren: ResizeObserverEntry[]) => {
    window.requestAnimationFrame(() => {
      if (!masonryRef.current || !masonryChildren || masonryChildren.length === 0) {
        return
      }
      const masonry = masonryRef.current
      const masonryFirstChild = masonryRef.current?.firstChild as Element
      const parentWidth = masonry.clientWidth
      const firstChildWidth = masonryFirstChild['clientWidth']

      if (parentWidth === 0 || firstChildWidth === 0) {
        return
      }

      const firstChildComputedStyle = window.getComputedStyle(masonryFirstChild)
      const firstChildMarginLeft = parseToNumber(firstChildComputedStyle.marginLeft)
      const firstChildMarginRight = parseToNumber(firstChildComputedStyle.marginRight)

      const currentNumberOfColumns = Math.round(
        parentWidth / (firstChildWidth + firstChildMarginLeft + firstChildMarginRight),
      )

      const columnHeights = new Array(currentNumberOfColumns).fill(0)
      let skip = false
      masonry.childNodes.forEach((element: ChildNode) => {
        const child = element as HTMLElement
        if (child.nodeType !== Node.ELEMENT_NODE || child.dataset.class === 'line-break' || skip) {
          return
        }
        const childComputedStyle = window.getComputedStyle(child)
        const childMarginTop = parseToNumber(childComputedStyle.marginTop)
        const childMarginBottom = parseToNumber(childComputedStyle.marginBottom)
        // if any one of children isn't rendered yet, masonry's height shouldn't be computed yet
        const childHeight = parseToNumber(childComputedStyle.height)
          ? Math.ceil(parseToNumber(childComputedStyle.height)) + childMarginTop + childMarginBottom
          : 0
        if (childHeight === 0) {
          skip = true
          return
        }
        // if there is a nested image that isn't rendered yet, masonry's height shouldn't be computed yet
        for (let i = 0; i < child.childNodes.length; i += 1) {
          const nestedChild = child.childNodes[i] as Element

          if (nestedChild.tagName === 'IMG' && nestedChild.clientHeight === 0) {
            skip = true
            break
          }
        }
        if (!skip) {
          // find the current shortest column (where the current item will be placed)
          const currentMinColumnIndex = columnHeights.indexOf(Math.min(...columnHeights))
          columnHeights[currentMinColumnIndex] += childHeight
          const order = currentMinColumnIndex + 1

          child.style.order = `${order}`
        }
      })
      if (!skip) {
        // In React 18, state updates in a ResizeObserver's callback are happening after the paint which causes flickering
        // when doing some visual updates in it. Using flushSync ensures that the dom will be painted after the states updates happen
        // Related issue - https://github.com/facebook/react/issues/24331
        flushSync(() => {
          setMaxColumnHeight(Math.max(...columnHeights))
          setNumberOfLineBreaks(currentNumberOfColumns > 0 ? currentNumberOfColumns - 1 : 0)
        })
      }
    })
  }

  const observer = useRef(
    typeof ResizeObserver === 'undefined' ? undefined : new ResizeObserver(handleResize),
  )

  useEffect(() => {
    const resizeObserver = observer.current
    // IE and old browsers are not supported
    if (resizeObserver === undefined) {
      return undefined
    }

    if (masonryRef.current) {
      masonryRef.current.childNodes.forEach((childNode: ChildNode) => {
        if (childNode instanceof Element) {
          resizeObserver.observe(childNode)
        }
      })
    }
    return () => {
      resizeObserver?.disconnect()
    }
  }, [columnWidth, gutter, children, masonryRef])

  //  columns are likely to have different heights and hence can start to merge;
  //  a line break at the end of each column prevents columns from merging
  const lineBreaks = new Array(numberOfLineBreaks).fill('').map((_, index) => (
    <span
      key={index}
      data-class="line-break"
      style={{
        flexBasis: '100%',
        width: 0,
        margin: 0,
        padding: 0,
        order: index + 1,
      }}
    />
  ))

  return (
    <div
      ref={masonryRef}
      className={css({
        display: 'flex',
        flexFlow: 'column wrap',
        alignContent: 'flex-start',
        boxSizing: 'border-box',
        overflow: 'hidden',
        ...(columnWidth !== '100%' && { margin: `0 -${gutter / 2}px` }),
        '& > *': {
          boxSizing: 'border-box',
          ...(columnWidth !== '100%' && { margin: `${gutter / 2}px` }),
          ...(columnWidth === '100%' && { marginBottom: `${gutter}px` }),
          width: columnWidth,
        },
      })}
      style={{
        ...(maxColumnHeight > 0 && {
          height: maxColumnHeight,
        }),
      }}
    >
      {columnWidth !== '' && children}
      {lineBreaks}
    </div>
  )
}

export default ResponsiveMasonry
