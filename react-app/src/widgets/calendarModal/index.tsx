import { openModal, closeModal } from '@/shared/ui/modalsManager'

import CalendarModalLoader from './CalendarModalLoader'

const modalId = 'calendarModal'

function openCalendarModal() {
  openModal({
    modalId: modalId,
    name: 'content',
    settings: {
      title: 'Filter notes by date',
      closeOnClickOutside: false,
      size: 300,
    },
    data: {
      children: <CalendarModalLoader />,
    },
  })
}

function closeCalendarModal() {
  closeModal(modalId)
}

export { openCalendarModal, closeCalendarModal }
