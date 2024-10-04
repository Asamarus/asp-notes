import useAppSelector from '@/shared/lib/useAppSelector'

import { Tooltip, ScrollArea } from '@mantine/core'

import commonStyles from '@/shared/ui/commonStyles.module.css'

export interface BookProps {
  /** Note's id */
  id: number

  onClick: (book: string) => void

  maxWidth?: number
}
function Book({ id, onClick, maxWidth = 100 }: BookProps) {
  const book = useAppSelector((state) => state.notes.collection[id]?.book)

  if (!book) return null

  return (
    <Tooltip label="View notes with this book">
      <ScrollArea
        scrollbarSize={5}
        maw={maxWidth}
      >
        <span
          className={commonStyles['clickable-text']}
          onClick={() => {
            onClick(book)
          }}
        >
          {book}
        </span>
      </ScrollArea>
    </Tooltip>
  )
}

export default Book
