import { useApplicationStore } from '@/store'

import { Title, Text, Paper } from '@mantine/core'

function DashboardPage() {
  const allNotesSection = useApplicationStore((state) => state.allNotesSection)

  return (
    <Paper
      shadow="xs"
      p="xl"
    >
      <Title mb={20}>Dashboard</Title>
      <Text>Paper is the most basic ui component</Text>
      {allNotesSection && (
        <>
          <Text>allNotesSection.name: {allNotesSection.name}</Text>
          <Text>allNotesSection.displayName: {allNotesSection.displayName}</Text>
          <Text>allNotesSection.color: {allNotesSection.color}</Text>
        </>
      )}
    </Paper>
  )
}

export default DashboardPage
