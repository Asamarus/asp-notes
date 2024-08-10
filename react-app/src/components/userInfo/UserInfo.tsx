import { Menu, ActionIcon } from '@mantine/core'
import { openChangePasswordModal } from '@/modals'
import { accountsActions } from '@/actions'
import { useAccountsStore } from '@/store'

import { MdAccountCircle, MdLogout, MdVpnKey } from 'react-icons/md'

function UserInfo() {
  const user = useAccountsStore((state) => state.user)
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
          onClick={accountsActions.logout}
        >
          Logout
        </Menu.Item>
      </Menu.Dropdown>
    </Menu>
  )
}

export default UserInfo
