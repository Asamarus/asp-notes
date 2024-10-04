import { renderHook, act } from '@test-utils'
import useViewportSize from './useViewportSize'

describe('useViewportSize hook', () => {
  it('should return the window size on initial render and update it when the window size or orientation changes', () => {
    //Mock window size
    Object.defineProperty(window, 'innerWidth', {
      writable: true,
      configurable: true,
      value: 1024,
    })

    Object.defineProperty(window, 'innerHeight', {
      writable: true,
      configurable: true,
      value: 768,
    })

    const { result } = renderHook(() => useViewportSize())

    // Check initial window size
    expect(result.current).toEqual({ width: 1024, height: 768 })

    // Change window size
    act(() => {
      window.innerWidth = 800
      window.innerHeight = 600
      window.dispatchEvent(new Event('resize'))
    })

    // Check updated window size
    expect(result.current).toEqual({ width: 800, height: 600 })

    //Change orientation
    act(() => {
      window.innerWidth = 600
      window.innerHeight = 800
      window.dispatchEvent(new Event('orientationchange'))
    })

    // Check updated window size
    expect(result.current).toEqual({ width: 600, height: 800 })
  })
})
