import type { components } from '@/misc/openapi'

const sectionsMocks = [
  {
    id: 1,
    name: 'section1',
    displayName: 'Section 1',
    color: '#000000',
  },
  {
    id: 2,
    name: 'section2',
    displayName: 'Section 2',
    color: '#000000',
  },
  {
    id: 3,
    name: 'section3',
    displayName: 'Section 3',
    color: '#000000',
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
  sections: sectionsMocks,
}

export const updateSectionRequestMock: components['schemas']['UpdateSectionRequest'] = {
  id: 1,
  displayName: 'Updated Section 1',
  color: '#000000',
}

export const updateSectionResponseMock: components['schemas']['SectionsResponse'] = {
  message: 'Section is updated successfully!',
  sections: sectionsMocks,
}

export const deleteSectionRequestMock: components['schemas']['DeleteSectionRequest'] = {
  id: 1,
}

export const deleteSectionResponseMock: components['schemas']['SectionsResponse'] = {
  message: 'Section is deleted successfully!',
  sections: sectionsMocks,
}

export const reorderSectionsRequestMock: components['schemas']['ReorderSectionsRequest'] = {
  ids: [3, 2, 1],
}

export const reorderSectionsResponseMock: components['schemas']['SectionsResponse'] = {
  message: 'Sections are reordered successfully!',
  sections: sectionsMocks,
}
