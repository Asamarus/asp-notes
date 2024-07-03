import { render, act } from '@testing-library/react'
import useCustomEventListener, { dispatch } from './useCustomEventListener'

describe('useCustomEventListener hook', () => {
  it('calls the listener with the correct payload when the custom event is dispatched', () => {
    const listener = vi.fn()
    const eventName = 'customEvent'

    function TestComponent() {
      useCustomEventListener(eventName, listener)
      return null
    }

    render(<TestComponent />)

    const payload = 'test payload'
    dispatch(eventName, payload)

    expect(listener).toHaveBeenCalledWith(payload)
  })

  it('removes the event listener when the component is unmounted', () => {
    const listener = vi.fn()
    const eventName = 'customEvent'

    function TestComponent() {
      useCustomEventListener(eventName, listener)
      return null
    }

    const { unmount } = render(<TestComponent />)
    unmount()

    act(() => {
      const payload = 'test payload'
      dispatch(eventName, payload)
    })

    expect(listener).not.toHaveBeenCalled()
  })
})
