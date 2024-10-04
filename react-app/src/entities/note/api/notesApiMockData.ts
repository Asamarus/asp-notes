import type { components } from '@/shared/api'

const testNote = {
  id: 1,
  createdAt: '2021-01-01 10:25:16',
  updatedAt: '2021-01-01 11:11:25',
  title: 'Note 1',
  section: 'dot_net',
  content: '<p>Note1 content</p>',
  preview: 'Note1 content',
  tags: ['tag1', 'tag2', 'tag3', 'tag4', 'tag5', 'tag6'],
  book: 'book1',
  sources: [
    {
      id: '1',
      title: 'Source 1',
      link: 'https://example.com',
      description: 'Source 1 description',
      image: 'https://developer.mozilla.org/mdn-social-share.cd6c4a5a.png',
      showImage: true,
    },
    {
      id: '2',
      title: 'Source 2',
      link: 'https://example.com',
      description: 'Source 2 description',
      showImage: false,
    },
    {
      id: '3',
      title: 'Source 3',
      link: 'https://example.com',
      description: 'Source 3 description',
      showImage: false,
    },
  ],
}

export const searchNotesRequestMock: components['schemas']['SearchNotesRequest'] = {
  section: 'front_end',
  searchTerm: 'note',
  page: 1,
  book: 'book1',
  tags: ['tag1', 'tag2'],
  inRandomOrder: false,
  withoutBook: false,
  withoutTags: false,
  fromDate: '2021-01-01',
  toDate: '2021-12-31',
}

export const searchNotesResponseMock: components['schemas']['SearchNotesResponse'] = {
  notes: [
    testNote,
    {
      id: 2,
      createdAt: '2021-01-01 10:25:16',
      updatedAt: '2021-01-01 11:11:25',
      title: '',
      section: 'it',
      content: '<p>Note 2 content</p>',
      preview: 'Note 2 content',
      tags: [],
      book: 'book1',
      sources: [],
    },
    {
      id: 3,
      createdAt: '2021-01-01 10:25:16',
      updatedAt: '2021-01-01 11:11:25',
      title: 'Note 3',
      section: 'it',
      content: '<p>Note 3 content</p>',
      preview: 'Note 3 content',
      tags: ['tag1', 'tag2'],
      book: '',
      sources: [],
    },
    {
      id: 4,
      createdAt: '2021-01-01 10:25:16',
      updatedAt: '2021-01-01 11:11:25',
      title: 'Note 4',
      section: 'it',
      content: '<p>Note 4 content</p>',
      preview: 'Note 4 content',
      tags: ['tag1', 'tag2'],
      book: 'book1',
      sources: [],
    },
  ],
  total: 4,
  count: 4,
  lastPage: 1,
  canLoadMore: false,
  page: 1,
  searchTerm: '',
  keywords: [],
  foundWholePhrase: false,
}

export const autocompleteNotesRequestMock: components['schemas']['AutocompleteNotesRequest'] = {
  searchTerm: 'note',
  section: 'section1',
  book: 'book1',
}

export const autocompleteNotesResponseMock: components['schemas']['AutocompleteNotesResponse'] = {
  notes: [
    {
      id: 1,
      title: 'Note 1',
    },
    {
      id: 2,
      title: 'Note 2',
    },
    {
      id: 3,
      title: 'Note 3',
    },
    {
      id: 4,
      title: 'Note 4',
    },
  ],
  books: ['book1', 'book2'],
  tags: ['tag1', 'tag2'],
}

export const getNoteRequestMock: components['schemas']['GetNoteRequest'] = {
  id: 1,
}

export const getNoteResponseMock: components['schemas']['NoteResponse'] = {
  note: testNote,
}

export const createNoteRequestMock: components['schemas']['CreateNoteRequest'] = {
  section: 'section1',
  book: 'book1',
}

export const createNoteResponseMock: components['schemas']['NoteResponse'] = {
  message: 'Note is created!',
  showNotification: true,
  note: testNote,
}

export const updateNoteRequestMock: components['schemas']['UpdateNoteRequest'] = {
  id: 1,
  title: 'Updated Note 1',
  content: '<p>Updated Note1 content</p>',
}

export const updateNoteResponseMock: components['schemas']['NoteResponse'] = {
  message: 'Note is saved!',
  showNotification: true,
  note: testNote,
}

export const updateNoteBookRequestMock: components['schemas']['UpdateNoteBookRequest'] = {
  id: 1,
  book: 'book2',
}

export const updateNoteBookResponseMock: components['schemas']['NoteResponse'] = {
  message: 'Note book is updated!',
  showNotification: true,
  note: testNote,
}

export const updateNoteTagsRequestMock: components['schemas']['UpdateNoteTagsRequest'] = {
  id: 1,
  tags: ['tag3', 'tag4'],
}

export const updateNoteTagsResponseMock: components['schemas']['NoteResponse'] = {
  message: 'Note tags are updated!',
  showNotification: true,
  note: testNote,
}

export const updateNoteSectionRequestMock: components['schemas']['UpdateNoteSectionRequest'] = {
  id: 1,
  section: 'section2',
}

export const updateNoteSectionResponseMock: components['schemas']['NoteResponse'] = {
  message: 'Note is moved to another section!',
  showNotification: true,
  note: testNote,
}

export const deleteNoteRequestMock: components['schemas']['DeleteNoteRequest'] = {
  id: 1,
}

export const deleteNoteResponseMock: components['schemas']['SuccessResponse'] = {
  message: 'Note is deleted!',
  showNotification: true,
}

export const getCalendarDaysRequestMock: components['schemas']['GetNoteCalendarDaysRequest'] = {
  month: 1,
  year: 2021,
  section: 'section1',
}

export const getCalendarDaysResponseMock: components['schemas']['NoteCalendarDaysResponseItem'][] =
  [
    {
      count: 11,
      date: '2021-01-11',
    },
    {
      count: 14,
      date: '2021-01-12',
    },
    {
      count: 2,
      date: '2021-01-14',
    },
  ]
