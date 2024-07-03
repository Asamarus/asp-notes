import { AccountsContext } from '@/providers/accountsProvider'
import { PasswordInput, Fieldset, Group, Button } from '@mantine/core'
import { useForm } from 'react-hook-form'
import type { SubmitHandler } from 'react-hook-form'
import { useContext } from 'react'
import { TbLock } from 'react-icons/tb'

type Inputs = {
  currentPassword: string
  newPassword: string
  passwordRepeat: string
}

const minPasswordLengthValidation = (value: string) =>
  value.length >= 6 || 'Minimum password length is 6'

function ChangePasswordModal() {
  const { changePassword, isChangePasswordLoading } = useContext(AccountsContext)
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
  const onSubmit: SubmitHandler<Inputs> = (data) => {
    changePassword(data.currentPassword, data.newPassword, data.passwordRepeat)
  }

  return (
    <form onSubmit={handleSubmit(onSubmit)}>
      <Fieldset
        disabled={isChangePasswordLoading}
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
            loading={isChangePasswordLoading}
          >
            Update password
          </Button>
        </Group>
      </Fieldset>
    </form>
  )
}

export default ChangePasswordModal
