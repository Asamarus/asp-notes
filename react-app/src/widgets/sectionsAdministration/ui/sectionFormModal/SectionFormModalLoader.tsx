import { lazy } from 'react'
import ComponentLoader from '@/shared/ui/componentLoader'

import type { Section } from '@/entities/section'

const SectionFormModal = lazy(() => import('./SectionFormModal'))

export interface SectionFormModalLoaderProps {
  /** Section */
  section?: Section
}

function SectionFormModalLoader({ section }: SectionFormModalLoaderProps) {
  return (
    <ComponentLoader>
      <SectionFormModal section={section} />
    </ComponentLoader>
  )
}

export default SectionFormModalLoader
