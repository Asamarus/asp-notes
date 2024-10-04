import { useDisclosure } from '@mantine/hooks'

import { AppShell, Burger, Group, ScrollArea, Button } from '@mantine/core'
import { Outlet } from 'react-router-dom'
import ToggleColorScheme from '@/features/toggleColorScheme'
import UserInfo from '@/widgets/userInfo'
import Menu from '../menu'
import { Link } from 'react-router-dom'
import { MdSettings } from 'react-icons/md'

function SettingsPage() {
  const [opened, { toggle, close }] = useDisclosure()
  return (
    <AppShell
      header={{ height: 50 }}
      navbar={{
        width: 300,
        breakpoint: 'sm',
        collapsed: { mobile: !opened },
      }}
      padding={20}
    >
      <AppShell.Header style={{ transitionProperty: 'transform, left' }}>
        <Group
          h="100%"
          px="xs"
          justify="space-between"
        >
          <Group>
            <Burger
              opened={opened}
              onClick={toggle}
              hiddenFrom="sm"
              size="sm"
            />
            <Button
              variant="transparent"
              component={Link}
              color="black"
              to="/settings"
            >
              <Group>
                <MdSettings size={30} />
                Settings
              </Group>
            </Button>
          </Group>
          <Group>
            <ToggleColorScheme displayType="settingsPage" />
            <UserInfo displayType="settingsPage" />
          </Group>
        </Group>
      </AppShell.Header>
      <AppShell.Navbar p={0}>
        <AppShell.Section component={ScrollArea}>
          <Menu close={close} />
        </AppShell.Section>
      </AppShell.Navbar>
      <AppShell.Main>
        <Outlet />
      </AppShell.Main>
    </AppShell>
  )
}

export default SettingsPage
