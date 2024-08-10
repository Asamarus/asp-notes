import createFetch from '@/utils/createFetch'
import { notesService } from '@/services'
import { useCalendarStore, useSectionsStore } from '@/store'

import type { components } from '@/misc/openapi'
import type { CalendarDay } from '@/store/calendar'

function getCalendarDayFromResponse(
  day: components['schemas']['NoteCalendarDaysResponseItem'],
): CalendarDay {
  return {
    count: day.count ?? 0,
    date: day.date ?? '',
  }
}

function setIsLoading(isLoading: boolean) {
  if (useCalendarStore.getState().isMounted) {
    useCalendarStore.getState().setIsLoading(isLoading)
  }
}

const getCalendarDaysRequest = createFetch(notesService.getCalendarDays, setIsLoading, {
  concurrent: true,
})

function getCalendarDays({ month, year }: { month: number; year: number }) {
  getCalendarDaysRequest(
    {
      month,
      year,
      section: useSectionsStore.getState().currentSection?.name,
    },
    ({ data }) => {
      if (data && useCalendarStore.getState().isMounted) {
        const calendarDays = data?.map(getCalendarDayFromResponse) ?? []

        useCalendarStore.getState().setDays(calendarDays)
      }
    },
  )
}

export default { getCalendarDays }
