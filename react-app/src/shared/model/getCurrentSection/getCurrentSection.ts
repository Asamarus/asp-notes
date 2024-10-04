import store from '@/shared/lib/store'

function getCurrentSection(noteId?: number) {
  let sectionName = store.getState().sections.current?.name
  if (typeof noteId === 'number' && noteId > 0) {
    sectionName = store.getState().notes.collection[noteId]?.section
  }

  const allNotesSectionName = store.getState().sections.allNotesSection.name

  if (sectionName && sectionName !== allNotesSectionName) {
    return sectionName
  }
  return undefined
}

export default getCurrentSection
