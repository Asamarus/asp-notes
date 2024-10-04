import useAppSelector from '@/shared/lib/useAppSelector'

import { ActionIcon, Tooltip } from '@mantine/core'
import { MdPublic } from 'react-icons/md'

import commonStyles from '@/shared/ui/commonStyles.module.css'
import styles from './Sources.module.css'

export interface SourcesProps {
  /** Note's id */
  id: number
  onClick: () => void
}
function Sources({ id, onClick }: SourcesProps) {
  const numberOfSources = useAppSelector((state) => state.notes.collection[id]?.sources.length ?? 0)

  if (numberOfSources === 0) {
    return <div />
  }
  return (
    <div className={styles['wrapper']}>
      <Tooltip label={'View sources'}>
        <ActionIcon
          size={26}
          onClick={onClick}
        >
          <MdPublic
            className={commonStyles['action-icon']}
            size={26}
          />
        </ActionIcon>
      </Tooltip>
      <Tooltip label={'View sources'}>
        <span
          className={commonStyles['clickable-text']}
          onClick={onClick}
        >
          {numberOfSources}
        </span>
      </Tooltip>
    </div>
  )
}

export default Sources
