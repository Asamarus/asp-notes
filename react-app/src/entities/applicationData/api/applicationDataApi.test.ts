import { getApplicationData } from './applicationDataApi'
import * as applicationDataMocks from './applicationDataApiMockData'

describe('applicationDataService', () => {
  it('getApplicationData request test', async () => {
    const { data } = await getApplicationData()
    expect(data).toEqual(applicationDataMocks.getApplicationDataResponseMock)
  })
})
