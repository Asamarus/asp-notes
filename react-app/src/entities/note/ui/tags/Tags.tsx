import useAppSelector from '@/shared/lib/useAppSelector'

import { Tooltip } from '@mantine/core'

import commonStyles from '@/shared/ui/commonStyles.module.css'
import styles from './Tags.module.css'

export interface TagsProps {
  /** Note's id */
  id: number

  onClick: (tag: string) => void
}
function Tags({ id, onClick }: TagsProps) {
  const tags = useAppSelector((state) => state.notes.collection[id]?.tags ?? [])

  if (tags.length === 0) return null

  return (
    <div className={styles['wrapper']}>
      {tags.map((tag) => (
        <Tooltip
          key={tag}
          label="View notes with this tag"
        >
          <span
            className={commonStyles['clickable-text']}
            onClick={() => {
              onClick(tag)
            }}
          >
            #{tag}{' '}
          </span>
        </Tooltip>
      ))}
    </div>
  )
}

export default Tags
