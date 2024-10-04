import useAppSelector from '@/shared/lib/useAppSelector'
import useCurrentColor from '@/shared/lib/useCurrentColor'
import store from '@/shared/lib/store'
import { events } from '@/shared/config'
import { dispatchCustomEvent } from '@/shared/lib/useCustomEventListener'
import { setFilters } from '@/entities/note'
import { openBooksModal } from '@/widgets/booksModal'
import { openTagsModal } from '@/widgets/tagsModal'
import { openCalendarModal } from '@/widgets/calendarModal'

import { Button, Switch, Fieldset } from '@mantine/core'
import { MdBook, MdLabel, MdCalendarMonth } from 'react-icons/md'

function NotesFilters() {
  const inRandomOrder = useAppSelector((state) => state.notes.filters.inRandomOrder)
  const withoutBook = useAppSelector((state) => state.notes.filters.withoutBook)
  const withoutTags = useAppSelector((state) => state.notes.filters.withoutTags)
  const color = useCurrentColor()

  return (
    <Fieldset legend="Filters">
      <Button
        color={color}
        fullWidth
        mb={10}
        leftSection={<MdBook size={20} />}
        onClick={() => {
          dispatchCustomEvent(events.drawer.close)
          openBooksModal()
        }}
      >
        Filter by book
      </Button>
      <Button
        color={color}
        fullWidth
        mb={10}
        leftSection={<MdLabel size={20} />}
        onClick={() => {
          dispatchCustomEvent(events.drawer.close)
          openTagsModal()
        }}
      >
        Filter by tags
      </Button>
      <Button
        color={color}
        fullWidth
        mb={10}
        leftSection={<MdCalendarMonth size={20} />}
        onClick={() => {
          dispatchCustomEvent(events.drawer.close)
          openCalendarModal()
        }}
      >
        Filter by date
      </Button>
      <Switch
        size="md"
        mb={10}
        color={color}
        label="In radom order"
        checked={inRandomOrder}
        onChange={() => {
          store.dispatch(setFilters({ inRandomOrder: !inRandomOrder }))
          dispatchCustomEvent(events.notesList.search)
          dispatchCustomEvent(events.drawer.close)
        }}
      />
      <Switch
        size="md"
        mb={10}
        color={color}
        label="Without book"
        checked={withoutBook}
        onChange={() => {
          store.dispatch(setFilters({ withoutBook: !withoutBook }))
          dispatchCustomEvent(events.notesList.search)
          dispatchCustomEvent(events.drawer.close)
        }}
      />
      <Switch
        size="md"
        color={color}
        label="Without tags"
        checked={withoutTags}
        onChange={() => {
          store.dispatch(setFilters({ withoutTags: !withoutTags }))
          dispatchCustomEvent(events.notesList.search)
          dispatchCustomEvent(events.drawer.close)
        }}
      />
    </Fieldset>
  )
}

export default NotesFilters
