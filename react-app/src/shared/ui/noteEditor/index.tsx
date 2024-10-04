import { lazy } from 'react'
import ComponentLoader from '@/shared/ui/componentLoader'

import type { NoteEditorProps } from './NoteEditor'

const NoteEditor = lazy(() => import('./NoteEditor'))

function NoteEditorLoader(props: NoteEditorProps) {
  return (
    <ComponentLoader>
      <NoteEditor {...props} />
    </ComponentLoader>
  )
}

export default NoteEditorLoader
