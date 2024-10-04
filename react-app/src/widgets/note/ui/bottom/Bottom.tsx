import { openTagsModal } from '@/widgets/tagsModal'
import { useNavigate } from 'react-router-dom'
import rison from 'rison'
import store from '@/shared/lib/store'
import { setFilters } from '@/entities/note'
import { events } from '@/shared/config'
import { dispatchCustomEvent } from '@/shared/lib/useCustomEventListener'
import { EditTagsIcon, Tags } from '@/entities/note'
import { ScrollArea } from '@mantine/core'

import type { NoteProps } from '../../Note'

import styles from './Bottom.module.css'

type BottomProps = Pick<NoteProps, 'id' | 'displayType' | 'onClose'>

function Bottom({ id, displayType, onClose }: BottomProps) {
  const navigate = useNavigate()
  return (
    <div className={styles['wrapper']}>
      <EditTagsIcon
        onClick={() => {
          openTagsModal(id)
        }}
      />
      <div className={styles['spacer']} />
      <ScrollArea scrollbarSize={5}>
        <Tags
          id={id}
          onClick={(tag) => {
            if (displayType === 'page') {
              const searchParams = new URLSearchParams()
              searchParams.append(
                'list',
                rison.encode({
                  tags: [tag],
                }),
              )
              const section = store.getState().notes.collection[id]?.section ?? ''
              navigate(`/notes/${section}?${searchParams.toString()}`)
            } else if (displayType === 'modal') {
              store.dispatch(setFilters({ tags: [tag] }))
              dispatchCustomEvent(events.notesList.search)
              onClose()
            }
          }}
        />
      </ScrollArea>
    </div>
  )
}

export default Bottom
