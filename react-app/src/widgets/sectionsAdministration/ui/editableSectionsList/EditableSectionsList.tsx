import useFetch from '@/shared/lib/useFetch'
import useAppSelector from '@/shared/lib/useAppSelector'
import { sectionsApi, reorderSections } from '@/entities/section'
import { openConfirmationModal } from '@/shared/ui/modalsManager'
import createFetch from '@/shared/lib/createFetch'
import noop from '@/shared/lib/noop'
import store from '@/shared/lib/store'
import { openAddSectionFormModal, openEditSectionFormModal } from '../sectionFormModal'
import setSections from '../model/setSections'
import { showSuccess } from '@/shared/lib/notifications'

import { Button, LoadingOverlay } from '@mantine/core'
import SortableList from '@/shared/ui/sortableList'
import EditableItem from '@/shared/ui/editableItem'
import { MdAdd } from 'react-icons/md'

import type { Section } from '@/entities/section'

import styles from './EditableSectionsList.module.css'

const reorderSectionsRequest = createFetch(sectionsApi.reorderSections, noop, {
  debounce: true,
})

const handleSortEnd = (sections: Section[]) => {
  const ids = sections.map((section) => section.id)

  store.dispatch(reorderSections(ids))
  reorderSectionsRequest({ ids })
  showSuccess('Sections are reordered!')
}

function EditableSectionsList() {
  const sections = useAppSelector((state) => state.sections.list)
  const { request: deleteSectionRequest, isLoading: isDeleteSectionLoading } = useFetch(
    sectionsApi.deleteSection,
  )

  return (
    <>
      <Button
        onClick={openAddSectionFormModal}
        mb={22}
      >
        <MdAdd size={22} />
        Add new section
      </Button>

      <div className={styles['wrapper']}>
        <LoadingOverlay visible={isDeleteSectionLoading} />
        <SortableList
          items={sections}
          onSortEnd={handleSortEnd}
          renderItem={(section, dragHandleProps) => (
            <EditableItem
              dragHandleProps={dragHandleProps}
              onEditClick={() => {
                openEditSectionFormModal(section)
              }}
              onDeleteClick={() => {
                openConfirmationModal({
                  title: 'Delete section',
                  message: 'Are you sure you want to delete this section?',
                  onConfirm: () => {
                    deleteSectionRequest(section.id, ({ data }) => {
                      if (data) {
                        const payload = data ?? []

                        setSections(payload)
                        showSuccess('Section is deleted!')
                      }
                    })
                  },
                })
              }}
            >
              <div className={styles['content']}>
                <div
                  className={styles['color']}
                  style={{ backgroundColor: section.color }}
                ></div>
                <div className={styles['title']}>{section.displayName}</div>
              </div>
            </EditableItem>
          )}
        />
      </div>
    </>
  )
}

export default EditableSectionsList
