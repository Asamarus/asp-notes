import { useForm, Controller } from 'react-hook-form'
import { sectionsApi } from '@/entities/section'
import useFetch from '@/shared/lib/useFetch'
import { closeSectionFormModal } from '.'
import setSections from '../model/setSections'

import { Fieldset, ColorInput, TextInput, Button, Group } from '@mantine/core'

import type { Section } from '@/entities/section'

export interface SectionFormModalProps {
  /** Section */
  section?: Section
}

type Inputs = {
  name: string
  displayName: string
  color: string
}
const nameValidationRegex = /^[a-z0-9_]+$/
const nameValidation = (value: string) =>
  nameValidationRegex.test(value) ||
  'Name must contain only lowercase Latin letters, numbers, underscores and no whitespaces!'

function SectionFormModal({ section }: SectionFormModalProps) {
  const { request: createSectionRequest, isLoading: isCreateSectionLoading } = useFetch(
    sectionsApi.createSection,
  )
  const { request: updateSectionRequest, isLoading: isUpdateSectionLoading } = useFetch(
    sectionsApi.updateSection,
  )

  const isLoading = section ? isUpdateSectionLoading : isCreateSectionLoading

  const {
    register,
    handleSubmit,
    control,
    formState: { errors },
  } = useForm<Inputs>({
    defaultValues: {
      name: section?.name || '',
      displayName: section?.displayName || '',
      color: section?.color || '',
    },
  })

  return (
    <form
      onSubmit={handleSubmit((data) => {
        if (section) {
          updateSectionRequest(
            {
              id: section.id,
              displayName: data.displayName,
              color: data.color,
            },
            ({ data }) => {
              if (data) {
                const payload = data?.sections ?? []

                setSections(payload)
                closeSectionFormModal()
              }
            },
          )
        } else {
          createSectionRequest(
            {
              name: data.name,
              displayName: data.displayName,
              color: data.color,
            },
            ({ data }) => {
              if (data) {
                const payload = data?.sections ?? []

                setSections(payload)
                closeSectionFormModal()
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
          {...register('name', {
            required: 'Name is required',
            validate: nameValidation,
          })}
          error={errors.name && errors.name.message}
        />
        <TextInput
          withAsterisk
          label="Display name"
          {...register('displayName', {
            required: 'Display name is required',
          })}
          error={errors.displayName && errors.displayName.message}
        />
        <Controller
          name="color"
          control={control}
          rules={{
            required: 'Color is required',
          }}
          render={({ field }) => (
            <ColorInput
              withAsterisk
              label="Color"
              {...field}
              error={errors.color && errors.color.message}
            />
          )}
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
