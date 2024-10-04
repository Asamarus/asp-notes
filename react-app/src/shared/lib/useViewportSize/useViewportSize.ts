import { useCallback, useState, useEffect } from 'react'
import { useWindowEvent } from '@mantine/hooks'

const eventListerOptions = {
  passive: true,
}

function useViewportSize() {
  const [windowSize, setWindowSize] = useState({
    width: 0,
    height: 0,
  })

  const setSize = useCallback(() => {
    setWindowSize({
      width: window.innerWidth || 0,
      height: window.innerHeight || 0,
    })
  }, [])

  useWindowEvent('resize', setSize, eventListerOptions)
  useWindowEvent('orientationchange', setSize, eventListerOptions)
  useEffect(setSize, [setSize])

  return windowSize
}

export default useViewportSize
