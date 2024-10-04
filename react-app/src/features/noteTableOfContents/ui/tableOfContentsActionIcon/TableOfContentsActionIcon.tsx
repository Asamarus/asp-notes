import { useDisclosure, useClickOutside } from '@mantine/hooks'

import { ActionIcon, Tooltip, Popover } from '@mantine/core'
import { MdFormatListBulleted } from 'react-icons/md'
import TableOfContentsControl from '../tableOfContentsControl'

import commonStyles from '@/shared/ui/commonStyles.module.css'

export interface TableOfContentsActionIconProps {
  /** Note's id */
  id: number
}
function TableOfContentsActionIcon({ id }: TableOfContentsActionIconProps) {
  const [opened, handlers] = useDisclosure(false)
  const ref = useClickOutside(handlers.close)

  return (
    <Popover
      width={250}
      position="bottom-start"
      withArrow
      withinPortal={false}
      opened={opened}
      keepMounted
      trapFocus
    >
      <Popover.Target>
        <Tooltip label={'Table of contents'}>
          <ActionIcon
            size={26}
            onClick={handlers.toggle}
          >
            <MdFormatListBulleted
              className={commonStyles['action-icon']}
              size={26}
            />
          </ActionIcon>
        </Tooltip>
      </Popover.Target>

      <Popover.Dropdown ref={ref}>
        <TableOfContentsControl
          id={id}
          onClose={handlers.close}
        />
      </Popover.Dropdown>
    </Popover>
  )
}

export default TableOfContentsActionIcon
