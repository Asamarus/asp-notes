import isEmpty from './isEmpty'

describe('isEmpty', () => {
  it('should return true for null', () => {
    expect(isEmpty(null)).toBe(true)
  })

  it('should return true for undefined', () => {
    expect(isEmpty(undefined)).toBe(true)
  })

  it('should return true for an empty string', () => {
    expect(isEmpty('')).toBe(true)
  })

  it('should return true for the number 0', () => {
    expect(isEmpty(0)).toBe(true)
  })

  it('should return true for false', () => {
    expect(isEmpty(false)).toBe(true)
  })

  it('should return true for an empty object', () => {
    expect(isEmpty({})).toBe(true)
  })

  it('should return false for a non-empty string', () => {
    expect(isEmpty('hello')).toBe(false)
  })

  it('should return false for a non-zero number', () => {
    expect(isEmpty(42)).toBe(false)
  })

  it('should return false for true', () => {
    expect(isEmpty(true)).toBe(false)
  })

  it('should return false for a non-empty object', () => {
    expect(isEmpty({ a: 1 })).toBe(false)
  })
})
