import { Title, Text, Paper, Button } from '@mantine/core'

export function HomePage() {
  return (
    <Paper
      shadow="xs"
      p="xl"
    >
      <Title mb={20}>Home page</Title>

      <Text>This is home page</Text>

      <Button
        onClick={() => {
          console.log('Test')
        }}
      >
        Test
      </Button>
    </Paper>
  )
}

export default HomePage
