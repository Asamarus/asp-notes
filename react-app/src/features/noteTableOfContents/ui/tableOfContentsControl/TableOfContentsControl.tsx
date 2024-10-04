import { useEffect, useState } from 'react'
import { useComputedColorScheme } from '@mantine/core'

import { CloseButton, Group, TextInput, Text, ScrollArea } from '@mantine/core'
import { MdSearch } from 'react-icons/md'

import styles from './TableOfContents.module.css'

type Heading = {
  level: 'h1' | 'h2' | 'h3' | 'h4'
  text: string
  element: HTMLElement
}

export interface TableOfContentsControlProps {
  onClose: () => void
  /** Note's id */
  id: number
}

const scrollToHeading = (element: HTMLElement) => {
  element.scrollIntoView({ behavior: 'smooth' })
}

function TableOfContentsControl({ id, onClose }: TableOfContentsControlProps) {
  const colorScheme = useComputedColorScheme('light')
  const [value, setValue] = useState('')
  const [headings, setHeadings] = useState<Heading[]>([])

  useEffect(() => {
    const noteContent = document.querySelector('div[data-name="note-content"]')
    if (noteContent) {
      const headingElements = noteContent.querySelectorAll('h1, h2, h3, h4')
      const newHeadings: Heading[] = Array.from(headingElements).map((el) => ({
        level: el.tagName.toLowerCase() as 'h1' | 'h2' | 'h3' | 'h4',
        text: el.textContent || '',
        element: el as HTMLElement,
      }))
      setHeadings(newHeadings)
    }
  }, [id])
  return (
    <>
      <Group
        justify="space-between"
        mb={5}
      >
        <Text size="md">Table of contents</Text>
        <CloseButton
          onClick={onClose}
          aria-label="Close table of contents"
        />
      </Group>
      <TextInput
        data-autofocus
        size="xs"
        placeholder="Filter headings"
        value={value}
        onChange={(event) => {
          setValue(event.currentTarget.value)
        }}
        mb={5}
        leftSection={
          <MdSearch
            size={20}
            color={colorScheme === 'dark' ? '#e9ecef' : '#000'}
          />
        }
        rightSection={
          value.length > 0 ? (
            <CloseButton
              size="sm"
              onMouseDown={(event) => event.preventDefault()}
              onClick={() => setValue('')}
              aria-label="Clear value"
            />
          ) : undefined
        }
      />
      <ScrollArea
        h={150}
        className={styles['headings-wrapper']}
      >
        {headings
          .filter((heading) => heading.text.toLowerCase().includes(value.toLowerCase()))
          .map((heading, index) => (
            <Text
              className={styles['heading']}
              key={index}
              onClick={() => {
                scrollToHeading(heading.element)
                onClose()
              }}
            >
              {heading.text}
            </Text>
          ))}
      </ScrollArea>
    </>
  )
}

export default TableOfContentsControl
