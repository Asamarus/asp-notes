import openAsPopup from '@/shared/lib/openAsPopup'

import { Tooltip, ActionIcon, useComputedColorScheme } from '@mantine/core'
import { MdOpenInNew } from 'react-icons/md'

function OpenInNewWindow() {
  const colorScheme = useComputedColorScheme('light')

  return (
    <Tooltip label="Open current page in a new window">
      <ActionIcon
        size={32}
        color={colorScheme === 'dark' ? 'gray' : '#fff'}
        onClick={() => {
          openAsPopup(window.location.href)
        }}
      >
        <MdOpenInNew size={32} />
      </ActionIcon>
    </Tooltip>
  )
}

export default OpenInNewWindow
