import { openConfirmationModal } from '@/shared/ui/modalsManager'
import { notesApi, removeNote } from '@/entities/note'
import useFetch from '@/shared/lib/useFetch'
import { events } from '@/shared/config'
import { dispatchCrossTabEvent } from '@/shared/lib/useCrossTabEventListener'
import store from '@/shared/lib/store'
import { showSuccess } from '@/shared/lib/notifications'

import { Button } from '@mantine/core'
import { MdDelete } from 'react-icons/md'

export interface DeleteProps {
  /** Note's id */
  id: number
  onClose(): void
}
function Delete({ id, onClose }: DeleteProps) {
  const { request, isLoading } = useFetch(notesApi.deleteNote)
  return (
    <Button
      leftSection={<MdDelete size={20} />}
      color="red"
      loading={isLoading}
      onClick={() => {
        openConfirmationModal({
          title: `Delete #${id} note`,
          message: 'Are you sure you want to delete this note?',
          onConfirm: () => {
            request(id, ({ error }) => {
              if (!error) {
                onClose()
                store.dispatch(removeNote(id))
                dispatchCrossTabEvent(events.note.deleted, id)
                showSuccess('Note is deleted!')
              }
            })
          },
        })
      }}
    >
      Delete note
    </Button>
  )
}

export default Delete
