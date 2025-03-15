import type { components } from '@/shared/api'

export const getApplicationDataResponseMock: components['schemas']['ApplicationDataResponse'] = {
  allNotesSection: {
    name: 'all',
    displayName: 'Notes',
    color: '#1e88e5',
  },
  sections: [
    {
      id: 1,
      name: 'it',
      displayName: 'IT',
      color: '#f9a825',
    },
    {
      id: 2,
      name: 'dot_net',
      displayName: '.Net',
      color: '#607d8b',
    },
    {
      id: 3,
      name: 'front_end',
      displayName: 'Front-end',
      color: '#4caf50',
    },
    {
      id: 4,
      name: 'database',
      displayName: 'Database',
      color: '#44bdb9',
    },
  ],
}
