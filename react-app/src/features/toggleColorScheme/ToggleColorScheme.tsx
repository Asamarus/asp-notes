import { Tooltip, ActionIcon, useMantineColorScheme } from '@mantine/core'
import { MdDarkMode, MdLightMode } from 'react-icons/md'

export interface ToggleColorSchemeProps {
  /** Display type */
  displayType: 'notesPage' | 'settingsPage'
}

function ToggleColorScheme({ displayType }: ToggleColorSchemeProps) {
  const { colorScheme, setColorScheme } = useMantineColorScheme()

  const dark = colorScheme === 'dark'

  let iconColor = dark ? 'yellow' : 'gray'
  if (displayType === 'notesPage') {
    iconColor = dark ? 'yellow' : '#fff'
  }

  return (
    <Tooltip label={`Change theme to ${dark ? 'light' : 'dark'}`}>
      <ActionIcon
        size={32}
        color={iconColor}
        onClick={() => {
          setColorScheme(dark ? 'light' : 'dark')
        }}
      >
        {dark ? <MdLightMode size={32} /> : <MdDarkMode size={30} />}
      </ActionIcon>
    </Tooltip>
  )
}

export default ToggleColorScheme
