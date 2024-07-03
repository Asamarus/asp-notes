import { Title, Paper } from '@mantine/core'

function Page() {
  return (
    <Paper
      shadow="xs"
      p="xl"
    >
      <Title mb={20}>Page</Title>

      <div>Top</div>
      <div style={{ height: 1000 }} />
      <div>Bottom</div>
    </Paper>
  )
}

export default Page
