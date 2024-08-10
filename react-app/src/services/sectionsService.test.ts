import {
    getSectionsList,
    createSection,
    updateSection,
    deleteSection,
    reorderSections,
} from './sectionsService'
import { sectionsMocks } from '@/mocks/api/data'

describe('sectionsService', () => {

    it('getSectionsList request test', async () => {
        const { data } = await getSectionsList()
        expect(data).toEqual(sectionsMocks.getSectionsListResponseMock)
    })

    it('createSection request test', async () => {
        const { data } = await createSection(sectionsMocks.createSectionRequestMock)
        expect(data).toEqual(sectionsMocks.createSectionResponseMock)
    })

    it('updateSection request test', async () => {
        const { data } = await updateSection(sectionsMocks.updateSectionRequestMock)
        expect(data).toEqual(sectionsMocks.updateSectionResponseMock)
    })

    it('deleteSection request test', async () => {
        const { data } = await deleteSection(sectionsMocks.deleteSectionRequestMock)
        expect(data).toEqual(sectionsMocks.deleteSectionResponseMock)
    })

    it('reorderSections request test', async () => {
        const { data } = await reorderSections(sectionsMocks.reorderSectionsRequestMock)
        expect(data).toEqual(sectionsMocks.reorderSectionsResponseMock)
    })
})