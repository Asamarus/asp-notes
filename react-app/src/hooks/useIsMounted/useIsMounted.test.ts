import { act, renderHook } from '@testing-library/react'
import useIsMounted from './useIsMounted'

describe('useIsMounted hook', () => {
  it('should return a function that returns true when the component is mounted and false when it is unmounted', () => {
    const { result, unmount } = renderHook(() => useIsMounted())

    // Initially, the component is mounted, so the function should return true
    expect(result.current()).toBe(true)

    // Unmount the component
    act(() => {
      unmount()
    })

    // Now, the component is unmounted, so the function should return false
    expect(result.current()).toBe(false)
  })
})
