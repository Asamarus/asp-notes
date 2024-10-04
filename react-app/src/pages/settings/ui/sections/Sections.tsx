import useAppSelector from '@/shared/lib/useAppSelector'

import { Title, Paper, Alert } from '@mantine/core'
import { MdWarning } from 'react-icons/md'
import SectionsAdministration from '@/widgets/sectionsAdministration'

function Sections() {
  const sections = useAppSelector((state) => state.sections.list)

  return (
    <Paper
      shadow="xs"
      p="xl"
    >
      <Title mb={20}>Sections</Title>
      {sections.length === 0 && (
        <Alert
          variant="light"
          color="red"
          title="No sections found"
          mb={20}
          icon={<MdWarning size={30} />}
        >
          To add a new note you need to create at least one section!
        </Alert>
      )}

      <SectionsAdministration />
    </Paper>
  )
}

export default Sections
