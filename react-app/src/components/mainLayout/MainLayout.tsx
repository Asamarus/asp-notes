import { AppShell, Burger, Group, ScrollArea } from '@mantine/core'
import { useDisclosure } from '@mantine/hooks'
import Logo from './components/logo'
import Menu from './components/menu'
import ToggleColorScheme from '@/components/toggleColorScheme'
import UserInfo from '../userInfo'

export interface MainLayoutProps {
  /** The content of the component */
  children?: React.ReactNode
}
function MainLayout({ children }: MainLayoutProps) {
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
            <Logo />
          </Group>
          <Group>
            <ToggleColorScheme />
            <UserInfo />
          </Group>
        </Group>
      </AppShell.Header>
      <AppShell.Navbar p={0}>
        <AppShell.Section component={ScrollArea}>
          <Menu close={close} />
        </AppShell.Section>
      </AppShell.Navbar>
      <AppShell.Main>{children}</AppShell.Main>
    </AppShell>
  )
}

export default MainLayout
