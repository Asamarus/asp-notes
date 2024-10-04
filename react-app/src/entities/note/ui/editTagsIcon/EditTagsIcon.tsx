import { ActionIcon, Tooltip } from '@mantine/core'
import { MdLabel } from 'react-icons/md'

import commonStyles from '@/shared/ui/commonStyles.module.css'

export interface EditTagsIconProps {
  onClick: () => void
}
function EditTagsIcon({ onClick }: EditTagsIconProps) {
  return (
    <Tooltip label={'Edit tags'}>
      <ActionIcon
        size={26}
        onClick={() => {
          onClick()
        }}
      >
        <MdLabel
          className={commonStyles['action-icon']}
          size={26}
        />
      </ActionIcon>
    </Tooltip>
  )
}

export default EditTagsIcon
