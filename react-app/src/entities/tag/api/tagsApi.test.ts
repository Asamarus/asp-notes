import { getTagsList } from './tagsApi'
import * as tagsMocks from './tagsApiMockData'

describe('tagsApi', () => {
  it('getTagsList request test', async () => {
    const { data } = await getTagsList(tagsMocks.getTagsListRequestMock)
    expect(data).toEqual(tagsMocks.getTagsListResponseMock)
  })
})
