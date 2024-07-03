import { render, act } from '@testing-library/react'
import { dispatchStorageEvent } from '@test-utils'
import useCrossTabEventListener from './useCrossTabEventListener'

describe('useCrossTabEventListener hook', () => {
  it('calls the listener with the correct payload when the custom event is dispatched', () => {
    const listener = vi.fn()
    const eventName = 'customEvent'

    function TestComponent() {
      useCrossTabEventListener(listener)
      return null
    }

    render(<TestComponent />)

    const payload = 'test payload'
    dispatchStorageEvent(eventName, payload)

    expect(listener).toHaveBeenCalledWith({
      eventName,
      payload,
    })
  })

  it('removes the event listener when the component is unmounted', () => {
    const listener = vi.fn()
    const eventName = 'customEvent'

    function TestComponent() {
      useCrossTabEventListener(listener)
      return null
    }

    const { unmount } = render(<TestComponent />)
    unmount()

    act(() => {
      const payload = 'test payload'
      dispatchStorageEvent(eventName, payload)
    })

    expect(listener).not.toHaveBeenCalled()
  })
})
