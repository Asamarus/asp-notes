import useAppSelector from '@/shared/lib/useAppSelector'

import { Highlight } from '@mantine/core'

import styles from './Preview.module.css'

export interface PreviewProps {
  /** Note's id */
  id: number
}
function Preview({ id }: PreviewProps) {
  const title = useAppSelector((state) => state.notes.collection[id]?.title ?? '')
  const preview = useAppSelector((state) => state.notes.collection[id]?.preview ?? '')
  const keywords = useAppSelector((state) => state.notes.metadata.keywords)
  const searchTerm = useAppSelector((state) => state.notes.filters.searchTerm)
  const foundWholePhrase = useAppSelector((state) => state.notes.metadata.foundWholePhrase)

  if (keywords.length > 0) {
    return (
      <>
        <Highlight
          className={styles['title']}
          highlight={foundWholePhrase ? searchTerm : keywords}
        >
          {title}
        </Highlight>
        <Highlight
          className={styles['content']}
          highlight={foundWholePhrase ? searchTerm : keywords}
        >
          {preview}
        </Highlight>
      </>
    )
  }

  return (
    <>
      <div className={styles['title']}>{title}</div>
      <div className={styles['content']}>{preview}</div>
    </>
  )
}

export default Preview
