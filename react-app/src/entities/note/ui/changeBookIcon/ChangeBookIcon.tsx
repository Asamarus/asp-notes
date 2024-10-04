import { ActionIcon, Tooltip } from '@mantine/core'
import { MdBook } from 'react-icons/md'

import commonStyles from '@/shared/ui/commonStyles.module.css'

export interface ChangeBookIconProps {
  onClick: () => void
}
function ChangeBookIcon({ onClick }: ChangeBookIconProps) {
  return (
    <Tooltip label={'Change book'}>
      <ActionIcon
        size={26}
        onClick={() => {
          onClick()
        }}
      >
        <MdBook
          className={commonStyles['action-icon']}
          size={26}
        />
      </ActionIcon>
    </Tooltip>
  )
}

export default ChangeBookIcon
