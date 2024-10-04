import { useRef, useState } from 'react'

import { useIsomorphicEffect } from '@mantine/hooks'

interface Size {
  width: number
  height: number
}

function useElementSize<T extends HTMLElement = HTMLDivElement>() {
  const ref = useRef<T>(null)
  const [size, setSize] = useState<Size>({
    width: 0,
    height: 0,
  })

  useIsomorphicEffect(() => {
    const updateSize = () => {
      setSize({
        width: ref?.current?.offsetWidth || 0,
        height: ref?.current?.offsetHeight || 0,
      })
    }

    window.addEventListener('resize', updateSize)
    updateSize()

    return () => window.removeEventListener('resize', updateSize)
  }, [])

  return { ref, width: size.width, height: size.height }
}

export default useElementSize
