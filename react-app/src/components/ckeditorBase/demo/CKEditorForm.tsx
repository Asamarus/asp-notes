import CKEditorBase from '@/components/ckeditorBase'
import { TextInput, Button, Group, Box, Input } from '@mantine/core'
import { useEffect } from 'react'
import { useForm } from 'react-hook-form'

export interface CKEditorFormProps {
  /** The content of the component */
  children?: React.ReactNode
}
function CKEditorForm() {
  const { register, handleSubmit, setValue, trigger } = useForm({
    defaultValues: {
      title: 'Title',
      content: 'Some content',
    },
  })

  useEffect(() => {
    register('content')
  })

  const onSubmit = (values: unknown) => console.log(values)

  return (
    <Box
      maw={600}
      mx="auto"
    >
      <form onSubmit={handleSubmit(onSubmit)}>
        <TextInput
          withAsterisk
          label="Title"
          placeholder="your@email.com"
          {...register('title')}
        />
        <Input.Wrapper label="Content">
          <CKEditorBase
            placeholder="Note content"
            onChange={(newContent) => {
              setValue('content', newContent)
              trigger('content')
            }}
            value="Some content"
          />
        </Input.Wrapper>
        <Group
          justify="flex-end"
          mt="md"
        >
          <Button type="submit">Submit</Button>
        </Group>
      </form>
    </Box>
  )
}

export default CKEditorForm
