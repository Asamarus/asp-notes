import { useContext } from 'react'
import { ApplicationContext } from '@/providers/applicationProvider'

import { Title, Text, Paper } from '@mantine/core'

function DashboardPage() {
  const { initialData } = useContext(ApplicationContext)

  return (
    <Paper
      shadow="xs"
      p="xl"
    >
      <Title mb={20}>Dashboard</Title>
      <Text>Paper is the most basic ui component</Text>
      {initialData && (
        <>
          <Text>initialData.title: {initialData.title}</Text>
          <Text>initialData.someData: {initialData.someData}</Text>
        </>
      )}
    </Paper>
  )
}

export default DashboardPage
