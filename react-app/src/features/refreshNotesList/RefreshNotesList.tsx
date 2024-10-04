import { useComputedColorScheme } from '@mantine/core'
import { MdRefresh } from 'react-icons/md'
import { events } from '@/shared/config'
import { dispatchCustomEvent } from '@/shared/lib/useCustomEventListener'

import { Tooltip, ActionIcon } from '@mantine/core'

function RefreshNotesList() {
  const colorScheme = useComputedColorScheme('light')

  return (
    <Tooltip label="Refresh notes list">
      <ActionIcon
        size={32}
        color={colorScheme === 'dark' ? 'gray' : '#fff'}
        onClick={() => {
          dispatchCustomEvent(events.notesList.search)
        }}
      >
        <MdRefresh size={32} />
      </ActionIcon>
    </Tooltip>
  )
}

export default RefreshNotesList
