import { useForm } from 'react-hook-form'
import { accountsActions } from '@/actions'
import { useAccountsStore } from '@/store'
import isEmail from '@/helpers/isEmail'

import {
  Group,
  Title,
  TextInput,
  PasswordInput,
  Fieldset,
  Button,
  useMantineTheme,
  useMantineColorScheme,
} from '@mantine/core'
import { TbAt, TbLock } from 'react-icons/tb'
import { MdAccountCircle, MdLogin } from 'react-icons/md'

type Inputs = {
  email: string
  password: string
}

function LoginModal() {
  const { isLoginLoading } = useAccountsStore((state) => state)

  const { colorScheme } = useMantineColorScheme()
  const theme = useMantineTheme()

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<Inputs>({
    defaultValues: {
      email: '',
      password: '',
    },
  })

  return (
    <>
      <Group
        justify="center"
        gap="xs"
      >
        <MdAccountCircle
          color={colorScheme === 'dark' ? theme.colors.dark[0] : theme.colors.gray[7]}
          size={30}
        />
        <Title
          order={3}
          style={{ color: colorScheme === 'dark' ? theme.colors.dark[0] : theme.colors.gray[7] }}
        >
          Login
        </Title>
      </Group>
      <form
        onSubmit={handleSubmit((data) => {
          accountsActions.login(data.email, data.password)
        })}
      >
        <Fieldset
          disabled={isLoginLoading}
          variant="unstyled"
        >
          <TextInput
            withAsterisk
            label="Email"
            leftSection={<TbAt size={16} />}
            {...register('email', {
              required: 'Email is required',
              validate: (value) => isEmail(value) || 'Invalid email',
            })}
            error={errors.email && errors.email.message}
          />
          <PasswordInput
            withAsterisk
            label="Password"
            leftSection={<TbLock size={16} />}
            {...register('password', {
              required: 'Current password is required',
              validate: (value) => value.length >= 6 || 'Minimum password length is 6',
            })}
            error={errors.password && errors.password.message}
          />

          <Button
            mt="md"
            type="submit"
            fullWidth
            loading={isLoginLoading}
            color="green"
            leftSection={<MdLogin size={16} />}
          >
            Login
          </Button>
        </Fieldset>
      </form>
    </>
  )
}

export default LoginModal
