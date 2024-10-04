import { addNoteSource, updateNoteSource, removeNoteSource, reorderNoteSources } from './sourcesApi'
import * as sourcesMocks from './sourcesApiMockData'

describe('sourcesApi', () => {
  it('addNoteSource request test', async () => {
    const { data } = await addNoteSource(sourcesMocks.addNoteSourceRequestMock)
    expect(data).toEqual(sourcesMocks.addNoteSourceResponseMock)
  })

  it('updateNoteSource request test', async () => {
    const { data } = await updateNoteSource(sourcesMocks.updateNoteSourceRequestMock)
    expect(data).toEqual(sourcesMocks.updateNoteSourceResponseMock)
  })

  it('removeNoteSource request test', async () => {
    const { data } = await removeNoteSource(sourcesMocks.removeNoteSourceRequestMock)
    expect(data).toEqual(sourcesMocks.removeNoteSourceResponseMock)
  })

  it('reorderNoteSources request test', async () => {
    const { data } = await reorderNoteSources(sourcesMocks.reorderNoteSourcesRequestMock)
    expect(data).toEqual(sourcesMocks.reorderNoteSourcesResponseMock)
  })
})
