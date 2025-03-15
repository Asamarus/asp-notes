import { useForm } from '@mantine/form'
import { usersApi } from '@/entities/user'
import useFetch from '@/shared/lib/useFetch'
import isEmail from '@/shared/lib/isEmail'
import { closeLoginModal } from '.'
import { dispatchCrossTabEvent } from '@/shared/lib/useCrossTabEventListener'
import { dispatchCustomEvent } from '@/shared/lib/useCustomEventListener'
import { events } from '@/shared/config'

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

function LoginModal() {
  const { request: loginRequest, isLoading } = useFetch(usersApi.login)

  const { colorScheme } = useMantineColorScheme()
  const theme = useMantineTheme()

  const form = useForm({
    mode: 'uncontrolled',
    initialValues: {
      email: '',
      password: '',
    },
    validate: {
      email: (value) => (!isEmail(value) ? 'Invalid email' : null),
      password: (value) => (value.length < 6 ? 'Minimum password length is 6' : null),
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
        onSubmit={form.onSubmit((data) => {
          loginRequest({ email: data.email, password: data.password }, ({ data }) => {
            if (data) {
              const user = {
                id: data.id ?? '',
                email: data.email ?? '',
              }

              closeLoginModal()
              dispatchCustomEvent(events.user.loggedIn, { user })
              dispatchCrossTabEvent(events.user.loggedIn, { user })
            }
          })
        })}
      >
        <Fieldset
          disabled={isLoading}
          variant="unstyled"
        >
          <TextInput
            withAsterisk
            label="Email"
            leftSection={<TbAt size={16} />}
            key={form.key('email')}
            {...form.getInputProps('email')}
          />
          <PasswordInput
            withAsterisk
            label="Password"
            leftSection={<TbLock size={16} />}
            key={form.key('password')}
            {...form.getInputProps('password')}
          />

          <Button
            mt="md"
            type="submit"
            fullWidth
            loading={isLoading}
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
