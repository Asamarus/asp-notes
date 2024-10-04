import openAsPopup from './openAsPopup'

describe('openAsPopup', () => {
  // Save the original window.open function
  const originalWindowOpen = window.open

  beforeEach(() => {
    // Mock window.open before each test
    window.open = vi.fn()
  })

  afterEach(() => {
    // Restore the original window.open function after each test
    window.open = originalWindowOpen
  })

  it('should open a new window with the specified URL and default options', () => {
    openAsPopup('https://example.com')

    expect(window.open).toHaveBeenCalledWith(
      'https://example.com',
      '_blank',
      expect.stringContaining('width=600'),
    )
  })

  it('should open a new window with the specified URL and custom options', () => {
    openAsPopup('https://example.com', { name: 'test', width: 600, height: 800, focus: false })

    expect(window.open).toHaveBeenCalledWith(
      'https://example.com',
      'test',
      expect.stringContaining('width=600'),
    )
  })
})
