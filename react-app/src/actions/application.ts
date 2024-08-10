import createFetch from '@/utils/createFetch'
import { applicationService } from '@/services'
import { useApplicationStore } from '@/store'

import type { AllNotesSection } from '@/store/application'

const getInitialDataRequest = createFetch(
  applicationService.getInitialData,
  useApplicationStore.getState().setIsLoading,
)

function getInitialData() {
  getInitialDataRequest(({ data }) => {
    if (data) {
      const allNotesSection: AllNotesSection = {
        name: data?.allNotesSection?.name ?? '',
        displayName: data?.allNotesSection?.displayName ?? '',
        color: data?.allNotesSection?.color ?? '',
      }

      useApplicationStore.getState().setAllNotesSection(allNotesSection)
    }
  })
}

export default { getInitialData }
