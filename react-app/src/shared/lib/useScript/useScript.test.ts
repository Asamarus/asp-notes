import { renderHook } from '@test-utils'
import useScript, { UseScriptStatus } from './useScript'

describe('useScript hook', () => {
  let scriptSrc: string
  let scriptStatus: UseScriptStatus

  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  let querySelectorSpy: any

  beforeEach(() => {
    scriptSrc = 'https://example.com/script.js'
    scriptStatus = 'idle'
    querySelectorSpy = vi.spyOn(document, 'querySelector')
    querySelectorSpy.mockImplementation(() => ({
      getAttribute: vi.fn().mockReturnValue(scriptStatus),
      addEventListener: vi.fn(),
      removeEventListener: vi.fn(),
    }))
  })

  it('should return the script status on initial render', () => {
    const { result } = renderHook(() => useScript(scriptSrc))

    // Check initial script status
    expect(result.current).toEqual('idle')
  })

  it('should return "idle" when "shouldPreventLoad" option is true', () => {
    const { result } = renderHook(() => useScript(scriptSrc, { shouldPreventLoad: true }))

    // Check script status
    expect(result.current).toEqual('idle')
  })

  it('should call "remove" when "removeOnUnmount" option is true', () => {
    const removeMock = vi.fn()
    querySelectorSpy.mockImplementation(() => ({
      getAttribute: vi.fn().mockReturnValue('ready'),
      addEventListener: vi.fn(),
      removeEventListener: vi.fn(),
      remove: removeMock,
    }))

    const { unmount } = renderHook(() => useScript(scriptSrc, { removeOnUnmount: true }))

    unmount()

    // Check if the script element was removed
    expect(removeMock).toHaveBeenCalled()
  })
})
