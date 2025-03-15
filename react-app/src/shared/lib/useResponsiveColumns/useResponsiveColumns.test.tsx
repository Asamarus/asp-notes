import { render, renderHook, act } from '@testing-library/react'
import useResponsiveColumns from '@/shared/lib/useResponsiveColumns'

describe('useResponsiveColumns hook', () => {
  it('should return correct values', () => {
    const fn = vi.fn().mockReturnValue({
      gutter: 10,
      minWidth: 100,
      forceMinWidth: false,
    })

    const { result } = renderHook(() => useResponsiveColumns(fn))

    const { container } = render(<div ref={result.current.ref} />)

    // Initial values
    expect(result.current.columnWidth).toBe('')
    expect(result.current.columnHeight).toBe(0)
    expect(result.current.gutter).toBe(0)

    const div = container.firstChild

    // Mock the offsetWidth and offsetHeight to simulate a resize
    Object.defineProperty(div, 'offsetWidth', { configurable: true, value: 500 })
    Object.defineProperty(div, 'offsetHeight', { configurable: true, value: 500 })

    // Trigger resize event
    act(() => {
      global.dispatchEvent(new Event('resize'))
    })

    // Check updated values
    expect(result.current.columnWidth).not.toBe('')
    expect(result.current.columnHeight).not.toBe(0)
    expect(result.current.gutter).toBe(10)
  })
})
