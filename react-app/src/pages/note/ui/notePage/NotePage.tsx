import { useNavigate, useParams } from 'react-router-dom'

import { NoteLoader } from '@/entities/note'
import Navbar from '../navbar'
import Note from '@/widgets/note'

import styles from './NotePage.module.css'

function NotePage() {
  const { id } = useParams<{ id: string }>()
  const navigate = useNavigate()
  const noteId = id ? parseInt(id, 10) : null

  if (noteId === null || isNaN(noteId)) {
    return <>Invalid Note ID</>
  }
  return (
    <NoteLoader id={noteId}>
      <Navbar id={noteId} />
      <div className={styles['wrapper']}>
        <Note
          id={noteId}
          onClose={() => {
            navigate('/notes')
          }}
          displayType="page"
        />
      </div>
    </NoteLoader>
  )
}

export default NotePage
