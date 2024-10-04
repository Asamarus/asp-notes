import type { Section } from './types'
import type { components } from '@/shared/api'

function getSectionFromResponse(section: components['schemas']['SectionItemResponse']): Section {
  return {
    id: section.id ?? 0,
    name: section.name ?? '',
    displayName: section.displayName ?? '',
    color: section.color ?? '',
  }
}

export default getSectionFromResponse
