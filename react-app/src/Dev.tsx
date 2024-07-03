import BrowserApiMockProvider from '@/mocks/api/BrowserApiMockProvider'
import { accountsService } from './services'

function DevWrapper() {
  return (
    <BrowserApiMockProvider>
      <Dev />
    </BrowserApiMockProvider>
  )
}

function Dev() {
  return (
    <>
      <button
        onClick={() => {
          accountsService.getUser().then(({ data }) => {
            console.log('userData', data)
          })
        }}
      >
        Test
      </button>
      Dev
    </>
  )
}

export default DevWrapper
