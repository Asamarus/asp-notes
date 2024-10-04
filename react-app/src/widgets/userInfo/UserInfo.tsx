import { Menu, ActionIcon, useComputedColorScheme } from '@mantine/core'
import { openChangePasswordModal } from '@/widgets/changePasswordModal'
import useAppSelector from '@/shared/lib/useAppSelector'
import { logout } from '@/entities/user'

import { MdAccountCircle, MdLogout, MdVpnKey } from 'react-icons/md'

export interface UserInfoProps {
  /** Display type */
  displayType: 'notesPage' | 'settingsPage'
}

function UserInfo({ displayType }: UserInfoProps) {
  const colorScheme = useComputedColorScheme('light')
  const user = useAppSelector((state) => state.userData.user)

  let iconColor = 'gray'
  if (displayType === 'notesPage') {
    iconColor = colorScheme === 'dark' ? 'gray' : '#fff'
  }

  return (
    <Menu
      shadow="md"
      width={200}
    >
      <Menu.Target>
        <ActionIcon
          color={iconColor}
          aria-label="User account"
          size={32}
        >
          <MdAccountCircle size={32} />
        </ActionIcon>
      </Menu.Target>
      <Menu.Dropdown>
        {user && (
          <Menu.Item
            leftSection={<MdAccountCircle size={20} />}
            style={{ cursor: 'default' }}
          >
            {user.email}
          </Menu.Item>
        )}

        <Menu.Divider />

        <Menu.Item
          leftSection={<MdVpnKey size={20} />}
          onClick={openChangePasswordModal}
        >
          Change password
        </Menu.Item>
        <Menu.Item
          leftSection={<MdLogout size={20} />}
          onClick={logout}
        >
          Logout
        </Menu.Item>
      </Menu.Dropdown>
    </Menu>
  )
}

export default UserInfo
