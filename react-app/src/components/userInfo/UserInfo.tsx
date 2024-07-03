import { Menu, ActionIcon } from '@mantine/core'
import { MdAccountCircle, MdLogout, MdVpnKey } from 'react-icons/md'
import { useContext } from 'react'
import { AccountsContext } from '@/providers/accountsProvider'
import { openChangePasswordModal } from '@/modals'

function UserInfo() {
  const { user, logout } = useContext(AccountsContext)

  return (
    <Menu
      shadow="md"
      width={200}
    >
      <Menu.Target>
        <ActionIcon
          variant="subtle"
          color="gray"
          aria-label="User account"
          size={30}
        >
          <MdAccountCircle size={30} />
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
