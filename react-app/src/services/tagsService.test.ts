import {
    getTagsList,
    autocompleteTags,
} from './tagsService'
import { tagsMocks } from '@/mocks/api/data'

describe('tagsService', () => {

    it('getTagsList request test', async () => {
        const { data } = await getTagsList(tagsMocks.getTagsListRequestMock)
        expect(data).toEqual(tagsMocks.getTagsListResponseMock)
    })

    it('autocompleteTags request test', async () => {
        const { data } = await autocompleteTags(tagsMocks.autocompleteTagsRequestMock)
        expect(data).toEqual(tagsMocks.autocompleteTagsResponseMock)
    })
})