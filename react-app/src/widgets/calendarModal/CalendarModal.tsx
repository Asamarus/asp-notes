import { useState, useEffect } from 'react'
import useAppSelector from '@/shared/lib/useAppSelector'
import dayjs from 'dayjs'
import store from '@/shared/lib/store'
import { events } from '@/shared/config'
import { dispatchCustomEvent } from '@/shared/lib/useCustomEventListener'
import { closeCalendarModal } from '.'
import useFetch from '@/shared/lib/useFetch'
import { notesApi, setFilters } from '@/entities/note'
import getCurrentSection from '@/shared/model/getCurrentSection'
import useCurrentColor from '@/shared/lib/useCurrentColor'

import { Center, LoadingOverlay } from '@mantine/core'
import { Calendar } from '@mantine/dates'

import styles from './CalendarModal.module.css'

function handleDateSelect(date: string) {
  store.dispatch(setFilters({ fromDate: date, toDate: date }))
  dispatchCustomEvent(events.notesList.search)
  closeCalendarModal()
}

function CalendarModal() {
  const { request, isLoading } = useFetch(notesApi.getCalendarDays)
  const fromDate = useAppSelector((state) => state.notes.filters.fromDate)
  const [selectedDate, setSelectedDate] = useState(
    fromDate ? dayjs(fromDate).toDate() : dayjs().toDate(),
  )
  const [noteDays, setNoteDays] = useState<number[]>([])
  const color = useCurrentColor()

  const selectedMonth = dayjs(selectedDate).month()
  const selectedYear = dayjs(selectedDate).year()

  useEffect(() => {
    request(
      { Month: selectedMonth + 1, Year: selectedYear, Section: getCurrentSection() },
      ({ data }) => {
        if (data) {
          setNoteDays(data.map((day) => dayjs(day.date).date()))
        }
      },
    )
  }, [selectedMonth, selectedYear, request])

  return (
    <Center pos="relative">
      <LoadingOverlay visible={isLoading} />
      <Calendar
        date={selectedDate}
        onDateChange={setSelectedDate}
        excludeDate={(date) => {
          const day = date.getDate()

          const dateMonth = dayjs(date).month()
          const dateYear = dayjs(date).year()

          if (dateMonth !== selectedMonth || dateYear !== selectedYear) {
            return true
          }

          return !noteDays.includes(day)
        }}
        renderDay={(date) => {
          const day = date.getDate()

          const dateMonth = dayjs(date).month()
          const dateYear = dayjs(date).year()

          if (dateMonth !== selectedMonth || dateYear !== selectedYear) {
            return <div />
          }

          if (noteDays.includes(day)) {
            return (
              <div
                className={styles['day']}
                style={{
                  background: color,
                }}
                onClick={() => handleDateSelect(dayjs(date).format('YYYY-MM-DD'))}
              >
                {day}
              </div>
            )
          }

          return day
        }}
      />
    </Center>
  )
}

export default CalendarModal
