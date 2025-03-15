import { useForm } from '@mantine/form'
import { sectionsApi } from '@/entities/section'
import useFetch from '@/shared/lib/useFetch'
import { closeSectionFormModal } from '.'
import setSections from '../model/setSections'
import { showSuccess } from '@/shared/lib/notifications'

import { Fieldset, ColorInput, TextInput, Button, Group } from '@mantine/core'

import type { Section } from '@/entities/section'

export interface SectionFormModalProps {
  /** Section */
  section?: Section
}

const nameValidationRegex = /^[a-z0-9_]+$/
const nameValidation = (value: string) =>
  !value
    ? 'Name is required!'
    : !nameValidationRegex.test(value)
    ? 'Name must contain only lowercase Latin letters, numbers, underscores and no whitespaces!'
    : null

function SectionFormModal({ section }: SectionFormModalProps) {
  const { request: createSectionRequest, isLoading: isCreateSectionLoading } = useFetch(
    sectionsApi.createSection,
  )
  const { request: updateSectionRequest, isLoading: isUpdateSectionLoading } = useFetch(
    sectionsApi.updateSection,
  )

  const isLoading = section ? isUpdateSectionLoading : isCreateSectionLoading

  const form = useForm({
    mode: 'uncontrolled',
    initialValues: {
      name: section?.name || '',
      displayName: section?.displayName || '',
      color: section?.color || '',
    },
    validate: {
      name: nameValidation,
      displayName: (value) => (!value ? 'Display name is required!' : null),
      color: (value) => (!value ? 'Color is required!' : null),
    },
  })

  return (
    <form
      onSubmit={form.onSubmit((values) => {
        if (section) {
          updateSectionRequest(
            {
              id: section.id,
              request: {
                displayName: values.displayName,
                color: values.color,
              },
            },
            ({ data }) => {
              if (data) {
                const payload = data ?? []

                setSections(payload)
                closeSectionFormModal()
                showSuccess('Section is updated!')
              }
            },
          )
        } else {
          createSectionRequest(
            {
              name: values.name,
              displayName: values.displayName,
              color: values.color,
            },
            ({ data }) => {
              if (data) {
                const payload = data ?? []

                setSections(payload)
                closeSectionFormModal()
                showSuccess('Section is created!')
              }
            },
          )
        }
      })}
    >
      <Fieldset
        disabled={isLoading}
        variant="unstyled"
      >
        <TextInput
          withAsterisk
          label="Name"
          disabled={!!section}
          key={form.key('name')}
          {...form.getInputProps('name')}
        />
        <TextInput
          withAsterisk
          label="Display name"
          key={form.key('displayName')}
          {...form.getInputProps('displayName')}
        />
        <ColorInput
          withAsterisk
          label="Color"
          key={form.key('color')}
          {...form.getInputProps('color')}
        />
        <Group
          justify="flex-end"
          mt="md"
        >
          <Button
            type="submit"
            loading={isLoading}
          >
            {section ? 'Save' : 'Add'}
          </Button>
        </Group>
      </Fieldset>
    </form>
  )
}

export default SectionFormModal
