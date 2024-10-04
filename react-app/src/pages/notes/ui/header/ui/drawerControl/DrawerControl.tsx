import { events } from '@/shared/config'
import { dispatchCustomEvent } from '@/shared/lib/useCustomEventListener'

import { ActionIcon, useComputedColorScheme } from '@mantine/core'
import { MdMenu } from 'react-icons/md'

function DrawerControl() {
  const colorScheme = useComputedColorScheme('light')
  return (
    <ActionIcon
      size={50}
      variant="transparent"
      onClick={() => {
        dispatchCustomEvent(events.drawer.open)
      }}
      color={colorScheme === 'dark' ? 'gray' : '#fff'}
    >
      <MdMenu size={40} />
    </ActionIcon>
  )
}

export default DrawerControl
