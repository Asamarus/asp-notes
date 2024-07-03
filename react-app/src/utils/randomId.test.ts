import randomId from './randomId'

describe('randomId', () => {
  it('should return a string', () => {
    const result = randomId()
    expect(typeof result).toBe('string')
  })

  it('should start with "modules-"', () => {
    const result = randomId()
    expect(result.startsWith('modules-')).toBe(true)
  })
})
