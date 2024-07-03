import { render, screen, userEvent } from '@test-utils'

import ToggleColorScheme from './ToggleColorScheme'

describe('ToggleColorScheme', () => {
  it('changes tooltip title on click', async () => {
    render(<ToggleColorScheme />)
    const button = screen.getByRole('button')
    expect(document.documentElement).toHaveAttribute('data-mantine-color-scheme', 'light')

    await userEvent.click(button)
    expect(document.documentElement).toHaveAttribute('data-mantine-color-scheme', 'dark')

    await userEvent.click(button)
    expect(document.documentElement).toHaveAttribute('data-mantine-color-scheme', 'light')
  })
})
