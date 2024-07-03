import { lazy } from 'react'
import ComponentLoader from '@/components/componentLoader'
import type { ConfirmationModalProps } from './ConfirmationModal'

const ConfirmationModal = lazy(() => import('./ConfirmationModal'))

function ConfirmationModalLoader(props: ConfirmationModalProps) {
  return (
    <ComponentLoader>
      <ConfirmationModal {...props} />
    </ComponentLoader>
  )
}

export default ConfirmationModalLoader
