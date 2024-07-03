import { lazy } from 'react'
import ComponentLoader from '@/components/componentLoader'

const LoginModal = lazy(() => import('./LoginModal'))

function LoginModalLoader() {
  return (
    <ComponentLoader>
      <LoginModal />
    </ComponentLoader>
  )
}

export default LoginModalLoader
