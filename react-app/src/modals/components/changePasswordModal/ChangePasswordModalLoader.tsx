import { lazy } from 'react'
import ComponentLoader from '@/components/componentLoader'

const ChangePasswordModal = lazy(() => import('./ChangePasswordModal'))

function ChangePasswordModalLoader() {
  return (
    <ComponentLoader>
      <ChangePasswordModal />
    </ComponentLoader>
  )
}

export default ChangePasswordModalLoader
