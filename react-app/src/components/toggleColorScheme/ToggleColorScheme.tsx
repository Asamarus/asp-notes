import { Tooltip, ActionIcon, useMantineColorScheme } from '@mantine/core'
import { MdDarkMode, MdLightMode } from 'react-icons/md'

function ToggleColorScheme() {
  const { colorScheme, setColorScheme } = useMantineColorScheme()

  const dark = colorScheme === 'dark'
  return (
    <Tooltip label={`Change theme to ${dark ? 'light' : 'dark'}`}>
      <ActionIcon
        size={30}
        color={dark ? 'yellow' : 'gray'}
        onClick={() => {
          setColorScheme(dark ? 'light' : 'dark')
        }}
      >
        {dark ? <MdLightMode size={30} /> : <MdDarkMode size={30} />}
      </ActionIcon>
    </Tooltip>
  )
}

export default ToggleColorScheme
