const events = {
  user: {
    unAuthorized: 'user:unAuthorized',
    loggedIn: 'user:loggedIn',
  },
  sections: {
    updated: 'sections:updated',
  },
  notesList: {
    updateUrl: 'notesList:updateUrl',
  },
  note: {
    updated: 'note:updated',
    deleted: 'note:deleted',
  },
}

export default events
