import isEmail from './isEmail'

describe('isEmail', () => {
  it('should return true for valid emails', () => {
    expect(isEmail('test@example.com')).toBe(true)
    expect(isEmail('john.doe@example.co.uk')).toBe(true)
  })

  it('should return false for invalid emails', () => {
    expect(isEmail('test@example')).toBe(false)
    expect(isEmail('test@.com')).toBe(false)
    expect(isEmail('test')).toBe(false)
  })

  it('should return false for empty or whitespace-only strings', () => {
    expect(isEmail('')).toBe(false)
    expect(isEmail(' ')).toBe(false)
  })
})
