import type { components } from '@/shared/api'

const sectionsMocks = [
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
]

export const getSectionsListResponseMock: components['schemas']['SectionsResponse'] = {
  sections: sectionsMocks,
}

export const createSectionRequestMock: components['schemas']['CreateSectionRequest'] = {
  name: 'newSection',
  displayName: 'New Section',
  color: '#000000',
}

export const createSectionResponseMock: components['schemas']['SectionsResponse'] = {
  message: 'Section is created successfully!',
  showNotification: true,
  sections: sectionsMocks,
}

export const updateSectionRequestMock: components['schemas']['UpdateSectionRequest'] = {
  id: 1,
  displayName: 'Updated Section 1',
  color: '#000000',
}

export const updateSectionResponseMock: components['schemas']['SectionsResponse'] = {
  message: 'Section is updated successfully!',
  showNotification: true,
  sections: sectionsMocks,
}

export const deleteSectionRequestMock: components['schemas']['DeleteSectionRequest'] = {
  id: 1,
}

export const deleteSectionResponseMock: components['schemas']['SectionsResponse'] = {
  message: 'Section is deleted successfully!',
  showNotification: true,
  sections: sectionsMocks,
}

export const reorderSectionsRequestMock: components['schemas']['ReorderSectionsRequest'] = {
  ids: [3, 2, 1],
}

export const reorderSectionsResponseMock: components['schemas']['SectionsResponse'] = {
  message: 'Sections are reordered successfully!',
  showNotification: true,
  sections: sectionsMocks,
}
