import { Menu, ActionIcon } from '@mantine/core'
import {
  MdMoreVert,
  MdOutlineInsertDriveFile,
  MdModeEdit,
  MdPublic,
  MdBook,
  MdLabel,
  MdFolder,
  MdDelete,
  MdOpenInNew,
} from 'react-icons/md'

import commonStyles from '@/shared/ui/commonStyles.module.css'

export interface ContextMenuProps {
  onViewClick: () => void
  onEditClick: () => void
  onOpenPopupClick: () => void
  onEditSourcesClick: () => void
  onChangeBookClick: () => void
  onChangeSectionClick: () => void
  onEditTagsClick: () => void
  onDeleteClick: () => void
  isNotSaved?: boolean
}

const iconSize = 18

function ContextMenu({
  onViewClick,
  onEditClick,
  onOpenPopupClick,
  onEditSourcesClick,
  onChangeBookClick,
  onEditTagsClick,
  onChangeSectionClick,
  onDeleteClick,
  isNotSaved = false,
}: ContextMenuProps) {
  return (
    <Menu
      shadow="md"
      width={200}
    >
      <Menu.Target>
        <ActionIcon size={24}>
          <MdMoreVert
            className={commonStyles['action-icon']}
            size={24}
          />
        </ActionIcon>
      </Menu.Target>

      <Menu.Dropdown>
        <Menu.Item
          leftSection={<MdOutlineInsertDriveFile size={iconSize} />}
          onClick={onViewClick}
          disabled={isNotSaved}
        >
          View
        </Menu.Item>
        <Menu.Item
          leftSection={<MdModeEdit size={iconSize} />}
          onClick={onEditClick}
        >
          Edit
        </Menu.Item>
        <Menu.Item
          leftSection={<MdOpenInNew size={iconSize} />}
          onClick={onOpenPopupClick}
        >
          Popup
        </Menu.Item>
        <Menu.Item
          leftSection={<MdPublic size={iconSize} />}
          onClick={onEditSourcesClick}
        >
          Edit Sources
        </Menu.Item>
        <Menu.Item
          leftSection={<MdBook size={iconSize} />}
          onClick={onChangeBookClick}
        >
          Change book
        </Menu.Item>
        <Menu.Item
          leftSection={<MdLabel size={iconSize} />}
          onClick={onEditTagsClick}
        >
          Edit tags
        </Menu.Item>
        <Menu.Item
          leftSection={<MdFolder size={iconSize} />}
          onClick={onChangeSectionClick}
        >
          Change section
        </Menu.Item>
        <Menu.Divider />
        <Menu.Item
          color="red"
          leftSection={<MdDelete size={iconSize} />}
          onClick={onDeleteClick}
          disabled={isNotSaved}
        >
          Delete
        </Menu.Item>
      </Menu.Dropdown>
    </Menu>
  )
}

export default ContextMenu
