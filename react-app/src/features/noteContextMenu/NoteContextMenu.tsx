import openAsPopup from '@/shared/lib/openAsPopup'
import { openSourcesAdministrationModal } from '@/widgets/sources'
import { openChangeSectionModal } from '@/widgets/changeSectionModal'
import { openBooksModal } from '@/widgets/booksModal'
import { openTagsModal } from '@/widgets/tagsModal'

import { ContextMenu } from '@/entities/note'

export interface NoteContextMenuProps {
  /** Note's id */
  id: number
  changeNoteTab: (tab: 'view' | 'edit' | 'delete') => void
  isNotSaved?: boolean
}
function NoteContextMenu({ id, changeNoteTab, isNotSaved = false }: NoteContextMenuProps) {
  return (
    <ContextMenu
      onViewClick={() => {
        changeNoteTab('view')
      }}
      onEditClick={() => {
        changeNoteTab('edit')
      }}
      onOpenPopupClick={() => {
        openAsPopup(`/note/${id}`)
      }}
      onEditSourcesClick={() => {
        openSourcesAdministrationModal(id)
      }}
      onChangeBookClick={() => {
        openBooksModal(id)
      }}
      onChangeSectionClick={() => {
        openChangeSectionModal(id)
      }}
      onEditTagsClick={() => {
        openTagsModal(id)
      }}
      onDeleteClick={() => {
        changeNoteTab('delete')
      }}
      isNotSaved={isNotSaved}
    />
  )
}

export default NoteContextMenu
