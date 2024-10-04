const events = {
  user: {
    unAuthorized: 'user:unAuthorized',
    loggedIn: 'user:loggedIn',
  },
  sections: {
    updated: 'sections:updated',
  },
  notesList: {
    search: 'notesList:search',
  },
  notesSearch: {
    setValue: 'notesSearch:setValue',
  },
  note: {
    updated: 'note:updated',
    deleted: 'note:deleted',
  },
  drawer: {
    open: 'drawer:open',
    close: 'drawer:close',
  },
}

export default events
