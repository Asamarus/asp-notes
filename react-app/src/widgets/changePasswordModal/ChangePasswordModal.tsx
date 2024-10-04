import { useForm } from 'react-hook-form'
import { usersApi } from '@/entities/user'
import useFetch from '@/shared/lib/useFetch'
import { closeChangePasswordModal } from '.'

import { PasswordInput, Fieldset, Group, Button } from '@mantine/core'
import { TbLock } from 'react-icons/tb'

type Inputs = {
  currentPassword: string
  newPassword: string
  passwordRepeat: string
}

const minPasswordLengthValidation = (value: string) =>
  value.length >= 6 || 'Minimum password length is 6'

function ChangePasswordModal() {
  const { request: changePasswordRequest, isLoading } = useFetch(usersApi.changePassword)

  const {
    register,
    handleSubmit,
    getValues,
    formState: { errors },
  } = useForm<Inputs>({
    defaultValues: {
      currentPassword: '',
      newPassword: '',
      passwordRepeat: '',
    },
  })

  return (
    <form
      onSubmit={handleSubmit((data) => {
        changePasswordRequest(
          {
            currentPassword: data.currentPassword,
            newPassword: data.newPassword,
            passwordRepeat: data.passwordRepeat,
          },
          ({ data }) => {
            if (data) {
              closeChangePasswordModal()
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
          {...register('currentPassword', {
            required: 'Current password is required',
            validate: minPasswordLengthValidation,
          })}
          error={errors.currentPassword && errors.currentPassword.message}
        />
        <PasswordInput
          withAsterisk
          label="New password"
          leftSection={<TbLock size={16} />}
          {...register('newPassword', {
            required: 'New password is required',
            validate: minPasswordLengthValidation,
          })}
          error={errors.newPassword && errors.newPassword.message}
        />
        <PasswordInput
          withAsterisk
          label="New password confirmation"
          leftSection={<TbLock size={16} />}
          {...register('passwordRepeat', {
            required: 'Password confirmation is required',
            validate: (value) =>
              value.length < 6
                ? 'Minimum password length is 6'
                : value !== getValues('newPassword')
                ? 'Passwords do not match'
                : undefined,
          })}
          error={errors.passwordRepeat && errors.passwordRepeat.message}
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
