import { ActionIcon } from '@mantine/core'
import { MdDragIndicator, MdEdit, MdDelete } from 'react-icons/md'

import type { DraggableProvidedDragHandleProps } from '@hello-pangea/dnd'

import styles from './EditableItem.module.css'

export interface EditableItemProps {
  /** The content of the component */
  children?: React.ReactNode

  /** Drag handler props */
  dragHandleProps?: DraggableProvidedDragHandleProps | null

  /** OnEditClick */
  onEditClick: () => void

  /** Drag handler props */
  onDeleteClick: () => void
}
function EditableItem({
  dragHandleProps,
  children,
  onEditClick,
  onDeleteClick,
}: EditableItemProps) {
  return (
    <div className={styles['wrapper']}>
      <div className={styles['item']}>
        <div {...dragHandleProps}>
          <MdDragIndicator
            size={24}
            className={styles['drag-icon']}
          />
        </div>
        <div className={styles['content']}>{children}</div>
        <ActionIcon
          size={26}
          onClick={onEditClick}
        >
          <MdEdit
            size={26}
            className={styles['icon']}
          />
        </ActionIcon>
        <ActionIcon
          size={26}
          onClick={onDeleteClick}
        >
          <MdDelete
            size={26}
            className={styles['icon']}
          />
        </ActionIcon>
      </div>
    </div>
  )
}

export default EditableItem
