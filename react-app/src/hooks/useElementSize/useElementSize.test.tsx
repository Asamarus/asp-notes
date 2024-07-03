import { render, act, screen } from '@testing-library/react'
import useElementSize from './useElementSize'

interface TestComponentProps {
  newWidth?: number
  newHeight?: number
}

describe('useElementSize hook', () => {
  it('updates the width and height correctly when the window is resized', async () => {
    function TestComponent({ newWidth = 100, newHeight = 100 }: TestComponentProps) {
      const { ref, width, height } = useElementSize()

      return (
        <div
          ref={ref}
          style={{ width: newWidth, height: newHeight }}
        >
          {width}x{height}
        </div>
      )
    }

    // Mock offsetWidth and offsetHeight to return a fixed size
    Object.defineProperty(HTMLElement.prototype, 'offsetWidth', { configurable: true, value: 100 })
    Object.defineProperty(HTMLElement.prototype, 'offsetHeight', { configurable: true, value: 100 })

    const { rerender } = render(<TestComponent />)

    // Initially, the size should be 100x100
    expect(screen.getByText('100x100')).toBeInTheDocument()

    // Re-render the component with a different size
    rerender(
      <TestComponent
        newWidth={200}
        newHeight={200}
      />,
    )

    // Change the mock size
    Object.defineProperty(HTMLElement.prototype, 'offsetWidth', { configurable: true, value: 200 })
    Object.defineProperty(HTMLElement.prototype, 'offsetHeight', { configurable: true, value: 200 })

    // Manually dispatch the resize event
    act(() => {
      window.dispatchEvent(new Event('resize'))
    })

    // Now, the size should be 200x200
    expect(screen.getByText('200x200')).toBeInTheDocument()
  })
})
