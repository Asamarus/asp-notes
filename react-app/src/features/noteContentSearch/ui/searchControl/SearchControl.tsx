import { useEffect, useRef, useState } from 'react'
import { useComputedColorScheme } from '@mantine/core'
import useCurrentColor from '@/shared/lib/useCurrentColor'
import Mark from 'mark.js'
import store from '@/shared/lib/store'
import useIsMounted from '@/shared/lib/useIsMounted'

import { ActionIcon, Tooltip, CloseButton, Group, TextInput, Text, Checkbox } from '@mantine/core'
import { MdSearch, MdKeyboardArrowUp, MdKeyboardArrowDown } from 'react-icons/md'

import commonStyles from '@/shared/ui/commonStyles.module.css'
import styles from './SearchControl.module.css'

export interface SearchControlProps {
  onClose: () => void
  /** Note's id */
  id: number
}

const groupMarks = (marks: NodeListOf<HTMLElement>): HTMLElement[] => {
  const result: HTMLElement[] = []
  let currentGroup: HTMLElement[] = []
  const threshold = 10

  marks.forEach((mark) => {
    const markTop = mark.getBoundingClientRect().top

    if (currentGroup.length === 0) {
      currentGroup.push(mark)
    } else {
      const lastMarkTop = currentGroup[currentGroup.length - 1].getBoundingClientRect().top
      if (Math.abs(markTop - lastMarkTop) <= threshold) {
        currentGroup.push(mark)
      } else {
        result.push(currentGroup[0])
        currentGroup = [mark]
      }
    }
  })

  if (currentGroup.length > 0) {
    result.push(currentGroup[0])
  }

  return result
}

function SearchControl({ id, onClose }: SearchControlProps) {
  const isMounted = useIsMounted()
  const [searchWholePhrase, setSearchWholePhrase] = useState(false)
  const colorScheme = useComputedColorScheme('light')
  const [value, setValue] = useState('')
  const markInstance = useRef<Mark | null>(null)
  const currentColor = useCurrentColor()
  const [currentIndex, setCurrentIndex] = useState(0)
  const [totalResults, setTotalResults] = useState(0)
  const highlightedResults = useRef<HTMLElement[]>([])

  useEffect(() => {
    const noteContent = document.querySelector('div[data-name="note-content"]') as HTMLElement
    if (noteContent) {
      try {
        markInstance.current = new Mark(noteContent)
      } catch (error) {
        console.error('Error creating Mark instance:', error)
      }
    }
    const foundWholePhrase = store.getState().notes.metadata.foundWholePhrase
    const keywords = store.getState().notes.metadata.keywords
    const searchTerm = store.getState().notes.filters.searchTerm

    setSearchWholePhrase(foundWholePhrase)
    setValue(foundWholePhrase ? searchTerm : keywords.join(' '))
  }, [id])

  useEffect(() => {
    if (markInstance.current) {
      try {
        markInstance.current.unmark({
          done: () => {
            markInstance.current?.mark(value, {
              separateWordSearch: !searchWholePhrase,
              className: styles['mark'],
              acrossElements: true,
              done: () => {
                try {
                  if (!isMounted()) {
                    return
                  }
                  const marks = document.querySelectorAll('div[data-name="note-content"] mark')
                  highlightedResults.current = groupMarks(marks as NodeListOf<HTMLElement>)
                  setTotalResults(highlightedResults.current.length)
                  setCurrentIndex(0)
                  scrollToResult(0)
                } catch (error) {
                  console.error('Error grouping marks:', error)
                }
              },
            })
          },
        })
      } catch (error) {
        console.error('Error marking:', error)
      }
    }
  }, [value, searchWholePhrase, isMounted])

  const scrollToResult = (index: number) => {
    if (
      highlightedResults.current.length > 0 &&
      index >= 0 &&
      index < highlightedResults.current.length
    ) {
      highlightedResults.current[index].scrollIntoView({ behavior: 'smooth' })
    }
  }

  const hasResults = totalResults > 0

  const handleNext = () => {
    if (hasResults) {
      setCurrentIndex((prevIndex) => {
        const newIndex = (prevIndex + 1) % totalResults
        scrollToResult(newIndex)
        return newIndex
      })
    }
  }

  const handlePrevious = () => {
    if (hasResults) {
      setCurrentIndex((prevIndex) => {
        const newIndex = (prevIndex - 1 + totalResults) % totalResults
        scrollToResult(newIndex)
        return newIndex
      })
    }
  }

  return (
    <>
      <Group
        justify="space-between"
        mb={5}
      >
        <Text size="md">Search note content</Text>
        <CloseButton
          onClick={onClose}
          aria-label="Close search"
        />
      </Group>
      <Group
        justify="space-between"
        wrap="nowrap"
        gap={5}
        mb={5}
      >
        <TextInput
          data-autofocus
          size="xs"
          placeholder="Search..."
          value={value}
          onChange={(event) => {
            setValue(event.currentTarget.value)
          }}
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
        <Text>
          {currentIndex >= 0 && totalResults > 0 ? currentIndex + 1 : currentIndex}/{totalResults}
        </Text>
        <Tooltip label={'Previous'}>
          <ActionIcon
            size={26}
            onClick={handlePrevious}
            disabled={!hasResults}
          >
            <MdKeyboardArrowUp
              className={commonStyles['action-icon']}
              size={26}
            />
          </ActionIcon>
        </Tooltip>
        <Tooltip label={'Next'}>
          <ActionIcon
            size={26}
            onClick={handleNext}
            disabled={!hasResults}
          >
            <MdKeyboardArrowDown
              className={commonStyles['action-icon']}
              size={26}
            />
          </ActionIcon>
        </Tooltip>
      </Group>
      <Checkbox
        label="Search whole phrase"
        color={currentColor}
        checked={searchWholePhrase}
        onChange={(event) => setSearchWholePhrase(event.currentTarget.checked)}
      />
    </>
  )
}

export default SearchControl
