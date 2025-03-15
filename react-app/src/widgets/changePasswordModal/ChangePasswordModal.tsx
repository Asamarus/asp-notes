import { useForm } from '@mantine/form'
import { usersApi } from '@/entities/user'
import useFetch from '@/shared/lib/useFetch'
import { closeChangePasswordModal } from '.'
import { showSuccess } from '@/shared/lib/notifications'

import { PasswordInput, Fieldset, Group, Button } from '@mantine/core'
import { TbLock } from 'react-icons/tb'

const minPasswordLengthValidation = (value: string) =>
  value.length < 6 ? 'Minimum password length is 6' : null

function ChangePasswordModal() {
  const { request: changePasswordRequest, isLoading } = useFetch(usersApi.changePassword)

  const form = useForm({
    mode: 'uncontrolled',
    initialValues: {
      currentPassword: '',
      newPassword: '',
      passwordRepeat: '',
    },
    validate: {
      currentPassword: minPasswordLengthValidation,
      newPassword: minPasswordLengthValidation,
      passwordRepeat: (value, values) =>
        value.length < 6
          ? 'Minimum password length is 6'
          : value !== values.newPassword
          ? 'Passwords do not match'
          : null,
    },
  })

  return (
    <form
      onSubmit={form.onSubmit((values) => {
        changePasswordRequest(
          {
            currentPassword: values.currentPassword,
            newPassword: values.newPassword,
            passwordRepeat: values.passwordRepeat,
          },
          ({ error }) => {
            if (!error) {
              closeChangePasswordModal()
              showSuccess('Password is updated!')
            }
          },
        )
      })}
    >
      <Fieldset
        disabled={isLoading}
        variant="unstyled"
      >
        <PasswordInput
          withAsterisk
          label="Current password"
          leftSection={<TbLock size={16} />}
          key={form.key('currentPassword')}
          {...form.getInputProps('currentPassword')}
        />
        <PasswordInput
          withAsterisk
          label="New password"
          leftSection={<TbLock size={16} />}
          key={form.key('newPassword')}
          {...form.getInputProps('newPassword')}
        />
        <PasswordInput
          withAsterisk
          label="New password confirmation"
          leftSection={<TbLock size={16} />}
          key={form.key('passwordRepeat')}
          {...form.getInputProps('passwordRepeat')}
        />
        <Group
          justify="flex-end"
          mt="md"
        >
          <Button
            type="submit"
            loading={isLoading}
          >
            Update password
          </Button>
        </Group>
      </Fieldset>
    </form>
  )
}

export default ChangePasswordModal
