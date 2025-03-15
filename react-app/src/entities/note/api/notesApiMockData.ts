import type { components, paths } from '@/shared/api'

const testNote: components['schemas']['NotesItemResponse'] = {
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

export const searchNotesRequestMock: paths['/api/notes']['get']['parameters']['query'] = {
  Section: 'front_end',
  SearchTerm: 'note',
  Page: 1,
  Book: 'book1',
  Tags: ['tag1', 'tag2'],
  InRandomOrder: false,
  WithoutBook: false,
  WithoutTags: false,
  FromDate: '2021-01-01',
  ToDate: '2021-12-31',
}

export const searchNotesResponseMock: components['schemas']['PaginatedResponseOfNotesItemResponse'] =
  {
    total: 4,
    count: 4,
    lastPage: 1,
    canLoadMore: false,
    page: 1,
    searchTerm: '',
    keywords: [],
    foundWholePhrase: false,
    data: [
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
  }

export const autocompleteNotesRequestMock: paths['/api/notes/autocomplete']['get']['parameters']['query'] =
  {
    SearchTerm: 'note',
    Section: 'section1',
    Book: 'book1',
  }

export const autocompleteNotesResponseMock: components['schemas']['NotesAutocompleteResponse'] = {
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

export const getNoteRequestMock = 1

export const getNoteResponseMock = testNote

export const createNoteRequestMock: components['schemas']['NotesCreateRequest'] = {
  section: 'section1',
  book: 'book1',
}

export const createNoteResponseMock = testNote

export const updateNoteRequestMock: {
  id: number
  request: components['schemas']['NotesPatchRequest']
} = {
  id: 1,
  request: {
    title: 'Updated Note 1',
    content: '<p>Updated Note1 content</p>',
    book: 'book2',
    tags: ['tag3', 'tag4'],
    section: 'section2',
  },
}

export const updateNoteResponseMock = testNote

export const deleteNoteRequestMock = 1

export const getCalendarDaysRequestMock: paths['/api/notes/calendar']['get']['parameters']['query'] =
  {
    Month: 1,
    Year: 2021,
    Section: 'section1',
  }

export const getCalendarDaysResponseMock: components['schemas']['NotesCalendarDaysResponseItem'][] =
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
