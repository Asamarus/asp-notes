import createFetch from '@/utils/createFetch'
import { sectionsService } from '@/services'
import { useSectionsStore } from '@/store'
import events from '@/events'
import { dispatch as dispatchCrossTabEvent } from '@/hooks/useCrossTabEventListener'

import type { components } from '@/misc/openapi'
import type { Section } from '@/store/sections'

function getSectionFromResponse(section: components['schemas']['SectionItemResponse']): Section {
  return {
    id: section.id ?? 0,
    name: section.name ?? '',
    displayName: section.displayName ?? '',
    color: section.color ?? '',
  }
}

const getSectionsListRequest = createFetch(sectionsService.getSectionsList, (isLoading) => {
  useSectionsStore.getState().setIsLoading('isSectionsListLoading', isLoading)
})

const createSectionRequest = createFetch(sectionsService.createSection, (isLoading) => {
  useSectionsStore.getState().setIsLoading('isAddNewSectionLoading', isLoading)
})

const updateSectionRequest = createFetch(sectionsService.updateSection, (isLoading) => {
  useSectionsStore.getState().setIsLoading('isUpdateSectionLoading', isLoading)
})

const deleteSectionRequest = createFetch(sectionsService.deleteSection, (isLoading) => {
  useSectionsStore.getState().setIsLoading('isDeleteSectionLoading', isLoading)
})

const reorderSectionsRequest = createFetch(
  sectionsService.reorderSections,
  (isLoading) => {
    useSectionsStore.getState().setIsLoading('isReorderSectionsLoading', isLoading)
  },
  {
    debounce: true,
  },
)

function setSections(sections: components['schemas']['SectionItemResponse'][]) {
  const payload: Section[] = []

  sections?.forEach((section) => {
    payload.push(getSectionFromResponse(section))
  })

  useSectionsStore.getState().setSections(payload)
  dispatchCrossTabEvent(events.sections.updated, payload)
}

function getSections() {
  getSectionsListRequest(({ data }) => {
    if (data) {
      const payload = data?.sections ?? []

      setSections(payload)

      if (!useSectionsStore.getState().sectionsLoaded) {
        useSectionsStore.getState().setSectionsLoaded(true)
      }
    }
  })
}

function createSection(requestData: components['schemas']['CreateSectionRequest']) {
  createSectionRequest(requestData, ({ data }) => {
    if (data) {
      const payload = data?.sections ?? []

      setSections(payload)
    }
  })
}

function updateSection(requestData: components['schemas']['UpdateSectionRequest']) {
  updateSectionRequest(requestData, ({ data }) => {
    if (data) {
      const payload = data?.sections ?? []

      setSections(payload)
    }
  })
}

function deleteSection(requestData: components['schemas']['DeleteSectionRequest']) {
  deleteSectionRequest(requestData, ({ data }) => {
    if (data) {
      const payload = data?.sections ?? []

      setSections(payload)
    }
  })
}

function reorderSections(requestData: components['schemas']['ReorderSectionsRequest']) {
  useSectionsStore.getState().reorderSections(requestData.ids)
  reorderSectionsRequest(requestData)
}

function setCurrentSection(section: string | null) {
  if (section === null) {
    useSectionsStore.getState().setCurrentSection(null)
    return
  }

  const currentSection =
    useSectionsStore.getState().sections.find((s) => s.name === section) ?? null

  if (currentSection) {
    useSectionsStore.getState().setCurrentSection(currentSection)
  }
}

export default {
  getSections,
  createSection,
  updateSection,
  deleteSection,
  reorderSections,
  setCurrentSection,
}
