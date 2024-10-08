import { STORAGE_EVENT } from '@/shared/lib/useCrossTabEventListener'
import { fireEvent } from '@test-utils'

export function dispatchStorageEvent(eventName: string, payload: unknown): void {
  fireEvent(
    window,
    new StorageEvent('storage', {
      key: STORAGE_EVENT,
      storageArea: window.localStorage,
      newValue: JSON.stringify({ eventName, payload }),
    }),
  )
}
