import { lazy } from 'react'
import ComponentLoader from '@/components/componentLoader'
import type { LoadingModalProps } from './LoadingModal'

const LoadingModal = lazy(() => import('./LoadingModal'))

function LoadingModalLoader(props: LoadingModalProps) {
  return (
    <ComponentLoader>
      <LoadingModal {...props} />
    </ComponentLoader>
  )
}

export default LoadingModalLoader
