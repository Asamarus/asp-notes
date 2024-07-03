import { act, waitFor, renderHook } from '@testing-library/react'
import useFetch from './useFetch'

type GetDataFunction = (arg?: unknown) => Promise<{ data: unknown; error: unknown }>

describe('useFetch', () => {
  it('should call the async function with the provided arguments', async () => {
    const getData: GetDataFunction = vi.fn().mockResolvedValue({ data: null, error: null })
    const { result } = renderHook(() => useFetch(getData))

    act(() => {
      result.current.request('arg1', () => {})
    })

    await waitFor(() => expect(result.current.isLoading).toBe(false))

    expect(getData).toHaveBeenCalledWith('arg1')
  })

  it('should call the onResult callback with the result of the async function', async () => {
    const getData: GetDataFunction = vi.fn().mockResolvedValue({ data: 'data', error: null })
    const onResult = vi.fn()
    const { result } = renderHook(() => useFetch(getData))

    await act(async () => {
      result.current.request('arg1', onResult)
    })

    expect(onResult).toHaveBeenCalledWith({ data: 'data', error: null })
  })

  it('should not call the onResult callback if the request is not the latest', async () => {
    const getData: GetDataFunction = vi
      .fn()
      .mockImplementation(
        (data: string) => new Promise((resolve) => resolve({ data: data, error: null })),
      )
    const onResult = vi.fn()
    const { result } = renderHook(() => useFetch(getData, { concurrent: true }))

    await act(async () => {
      result.current.request('data1', onResult)
      result.current.request('data2', onResult)
      result.current.request('data3', onResult)
    })

    await waitFor(() => expect(result.current.isLoading).toBe(false))

    expect(onResult).toHaveBeenCalledTimes(1)
    expect(onResult).toHaveBeenCalledWith({ data: 'data3', error: null })
  })

  it('should debounce requests if debounce is true', async () => {
    vi.useFakeTimers()
    const getData: GetDataFunction = vi.fn().mockResolvedValue({ data: null, error: null })
    const { result } = renderHook(() => useFetch(getData, { debounce: true, debounceTimeout: 100 }))

    act(() => {
      result.current.request('arg1', () => {})
      result.current.request('arg2', () => {})
      vi.advanceTimersByTime(100)
      vi.useRealTimers()
    })

    await waitFor(() => expect(result.current.isLoading).toBe(false))

    expect(getData).toHaveBeenCalledTimes(1)
    expect(getData).toHaveBeenCalledWith('arg2')
  })

  it('should not debounce requests if debounce is false', async () => {
    const getData: GetDataFunction = vi.fn().mockResolvedValue({ data: null, error: null })
    const { result } = renderHook(() => useFetch(getData, { debounce: false, concurrent: true }))

    await act(async () => {
      result.current.request('arg1', () => {})
      result.current.request('arg2', () => {})
    })

    expect(getData).toHaveBeenCalledTimes(2)
  })

  it('correctly updates isLoading', async () => {
    const getData: GetDataFunction = vi.fn().mockResolvedValue({ data: null, error: null })
    const { result } = renderHook(() => useFetch(getData))

    expect(result.current.isLoading).toBe(false)

    act(() => {
      result.current.request(() => {})
    })

    await waitFor(() => result.current.isLoading === true)

    await waitFor(() => result.current.isLoading === false)

    expect(result.current.isLoading).toBe(false)
  })

  it('uses initialIsLoading', async () => {
    const getData: GetDataFunction = vi.fn().mockResolvedValue({ data: null, error: null })
    const { result } = renderHook(() => useFetch(getData, { initialIsLoading: true }))

    expect(result.current.isLoading).toBe(true)

    act(() => {
      result.current.request(() => {})
    })

    expect(result.current.isLoading).toBe(true)

    await waitFor(() => result.current.isLoading === false)

    expect(result.current.isLoading).toBe(false)
  })
})
