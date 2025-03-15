import useAppSelector from '@/shared/lib/useAppSelector'
import { openConfirmationModal } from '@/shared/ui/modalsManager'
import useFetch from '@/shared/lib/useFetch'
import createFetch from '@/shared/lib/createFetch'
import { sourcesApi } from '@/entities/source'
import { reorderNotesSources, setNoteSources } from '@/entities/note'
import { dispatchCrossTabEvent } from '@/shared/lib/useCrossTabEventListener'
import noop from '@/shared/lib/noop'
import store from '@/shared/lib/store'
import { events } from '@/shared/config'
import { openAddNewSourceFormModal } from './ui/addNewSourceFormModal'
import { openEditSourceFormModal } from './ui/editSourceFormModal'
import useCurrentColor from '@/shared/lib/useCurrentColor'
import { showSuccess } from '@/shared/lib/notifications'

import SortableList from '@/shared/ui/sortableList'
import { Button, LoadingOverlay, Anchor } from '@mantine/core'
import EditableItem from '@/shared/ui/editableItem'
import { MdAdd } from 'react-icons/md'

import type { NoteSource } from '@/entities/note'

import styles from './SourcesAdministrationModal.module.css'

export interface SourcesAdministrationModalProps {
  noteId: number
}

const reorderSourcesRequest = createFetch(sourcesApi.reorderNoteSources, noop, {
  debounce: true,
})

const handleSortEnd = (noteId: number, sources: NoteSource[]) => {
  const sourceIds = sources.map((source) => source.id)

  store.dispatch(reorderNotesSources({ id: noteId, sourceIds }))
  reorderSourcesRequest({ noteId, request: { sourceIds } }, ({ data }) => {
    if (data) {
      dispatchCrossTabEvent(events.note.updated, store.getState().notes.collection[noteId])
      showSuccess('Sources are reordered!')
    }
  })
}

function SourcesAdministrationModal({ noteId }: SourcesAdministrationModalProps) {
  const sources = useAppSelector((state) => state.notes.collection[noteId]?.sources ?? [])
  const color = useCurrentColor()
  const { request: removeNoteSourceRequest, isLoading: isDeleteSourceLoading } = useFetch(
    sourcesApi.removeNoteSource,
  )

  return (
    <>
      <Button
        color={color}
        onClick={() => {
          openAddNewSourceFormModal(noteId)
        }}
        mb={22}
      >
        <MdAdd size={22} />
        Add new source
      </Button>
      <div className={styles['wrapper']}>
        <LoadingOverlay visible={isDeleteSourceLoading} />
        <SortableList
          items={sources}
          onSortEnd={(sources) => {
            handleSortEnd(noteId, sources)
          }}
          renderItem={(source, dragHandleProps) => (
            <EditableItem
              dragHandleProps={dragHandleProps}
              onEditClick={() => {
                openEditSourceFormModal(noteId, source)
              }}
              onDeleteClick={() => {
                openConfirmationModal({
                  title: 'Delete sources',
                  message: 'Are you sure you want to delete this source?',
                  onConfirm: () => {
                    removeNoteSourceRequest({ noteId, sourceId: source.id }, ({ data }) => {
                      if (data) {
                        store.dispatch(setNoteSources({ id: noteId, sources: data }))
                        dispatchCrossTabEvent(
                          events.note.updated,
                          store.getState().notes.collection[noteId],
                        )
                        showSuccess('Source is deleted!')
                      }
                    })
                  },
                })
              }}
            >
              <div className={styles['content']}>
                <Anchor
                  className={styles['link']}
                  href={source.link}
                  target="_blank"
                >
                  <div>{source.title}</div>
                  <div>{source.link}</div>
                </Anchor>
              </div>
            </EditableItem>
          )}
        />
      </div>
    </>
  )
}

export default SourcesAdministrationModal
