import { lazy } from 'react'
import ComponentLoader from '@/shared/ui/componentLoader'

const LoginModal = lazy(() => import('./LoginModal'))

function LoginModalLoader() {
  return (
    <ComponentLoader>
      <LoginModal />
    </ComponentLoader>
  )
}

export default LoginModalLoader
