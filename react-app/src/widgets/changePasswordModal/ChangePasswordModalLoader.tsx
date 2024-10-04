import { lazy } from 'react'
import ComponentLoader from '@/shared/ui/componentLoader'

const ChangePasswordModal = lazy(() => import('./ChangePasswordModal'))

function ChangePasswordModalLoader() {
  return (
    <ComponentLoader>
      <ChangePasswordModal />
    </ComponentLoader>
  )
}

export default ChangePasswordModalLoader
