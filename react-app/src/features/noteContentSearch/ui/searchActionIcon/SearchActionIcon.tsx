import { useDisclosure, useClickOutside } from '@mantine/hooks'

import { ActionIcon, Tooltip, Popover } from '@mantine/core'
import { MdSearch } from 'react-icons/md'
import SearchControl from '../searchControl'

import commonStyles from '@/shared/ui/commonStyles.module.css'

export interface SearchActionIconProps {
  /** Note's id */
  id: number
}
function SearchActionIcon({ id }: SearchActionIconProps) {
  const [opened, handlers] = useDisclosure(false)
  const ref = useClickOutside(handlers.close)

  return (
    <Popover
      width={300}
      position="bottom-start"
      withArrow
      withinPortal={false}
      opened={opened}
      keepMounted
      trapFocus
    >
      <Popover.Target>
        <Tooltip label={'Search note content'}>
          <ActionIcon
            size={26}
            onClick={handlers.toggle}
          >
            <MdSearch
              className={commonStyles['action-icon']}
              size={26}
            />
          </ActionIcon>
        </Tooltip>
      </Popover.Target>
      <Popover.Dropdown ref={ref}>
        <SearchControl
          id={id}
          onClose={handlers.close}
        />
      </Popover.Dropdown>
    </Popover>
  )
}

export default SearchActionIcon
