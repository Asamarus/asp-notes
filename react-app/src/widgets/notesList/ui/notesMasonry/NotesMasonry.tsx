import { memo } from 'react'
import useAppSelector from '@/shared/lib/useAppSelector'

import ResponsiveMasonry from '@/shared/ui/responsiveMasonry'
import NotesListItem from '../notesListItem'

const getGridProps = () => {
  return {
    gutter: 10,
    minWidth: 200,
  }
}

function NotesMasonry() {
  const ids = useAppSelector((state) => state.notes.ids)

  return (
    <ResponsiveMasonry getGridProps={getGridProps}>
      {ids.map((id, index) => (
        <NotesListItem
          key={`${id}-${index}`}
          id={id}
        />
      ))}
    </ResponsiveMasonry>
  )
}

export default memo(NotesMasonry)
