import type { Section } from '@/entities/section'

interface PreloadedState {
  sections: {
    current: Section | null
    list: Section[]
    allNotesSection: {
      name: string
      displayName: string
      color: string
    }
  }
}

function getPreloadedState(): PreloadedState {
  const result = {
    sections: {
      current: null,
      list: [] as Section[],
      allNotesSection: {
        name: '',
        displayName: '',
        color: '',
      },
    },
  } as PreloadedState

  if (typeof window !== 'undefined' && window['__PRELOADED_STATE__']) {
    const preloadedState = window['__PRELOADED_STATE__'] as Partial<PreloadedState>
    if (typeof preloadedState === 'object') {
      try {
        if (preloadedState.sections) {
          if (preloadedState.sections.current !== undefined) {
            result.sections.current = preloadedState.sections.current
          }
          if (preloadedState.sections.list !== undefined) {
            result.sections.list = preloadedState.sections.list
          }
          if (preloadedState.sections.allNotesSection !== undefined) {
            result.sections.allNotesSection = preloadedState.sections.allNotesSection
          }
        }
      } catch (error) {
        console.error('Failed to parse preloaded state:', error)
      }
    }
  }

  return result
}

export default getPreloadedState
