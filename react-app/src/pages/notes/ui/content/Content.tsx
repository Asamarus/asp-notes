import NotesList from '@/widgets/notesList'

import styles from './Content.module.css'

function Content() {
  return (
    <div className={styles['wrapper']}>
      <NotesList />
    </div>
  )
}

export default Content
