import useAppSelector from '@/shared/lib/useAppSelector'
import extractRootDomain from '../../model/extractRootDomain'

import { Anchor, BackgroundImage, Stack } from '@mantine/core'

import styles from './SourcesListModal.module.css'

export interface SourcesListModalProps {
  noteId: number
}

function SourcesListModal({ noteId }: SourcesListModalProps) {
  const sources = useAppSelector((state) => state.notes.collection[noteId]?.sources ?? [])

  return (
    <Stack gap={15}>
      {sources.map((source) => (
        <Anchor
          key={source.id}
          href={source.link}
          target="_blank"
          variant="text"
          className={styles['link']}
        >
          {source.showImage && source.image && (
            <div className={styles['image-wrapper']}>
              <BackgroundImage
                src={source.image}
                className={styles['image']}
              />
            </div>
          )}
          <div className={styles['info']}>
            <div className={styles['title']}>{source.title}</div>
            <div className={styles['domain']}>{extractRootDomain(source.link)}</div>
          </div>
        </Anchor>
      ))}
    </Stack>
  )
}

export default SourcesListModal
