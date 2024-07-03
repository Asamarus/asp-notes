import { Button, Group, useMantineTheme } from '@mantine/core'
import { Link } from 'react-router-dom'
import { MdLogoDev } from 'react-icons/md'

function Logo() {
  const theme = useMantineTheme()
  return (
    <Button
      variant="transparent"
      component={Link}
      color="black"
      to="/"
    >
      <Group>
        <MdLogoDev
          size={40}
          color={theme.colors.blue[9]}
        />
        ProjectTemplate
      </Group>
    </Button>
  )
}

export default Logo
