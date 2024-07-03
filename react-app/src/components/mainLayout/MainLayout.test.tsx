import { render, screen } from '@test-utils'

import MainLayout from './MainLayout'

describe('MainLayout', () => {
  it('renders without crashing', () => {
    render(<MainLayout />)
    expect(screen.getByRole('banner')).toBeInTheDocument()
  })

  it('renders children correctly', () => {
    render(<MainLayout>Test Children</MainLayout>)
    expect(screen.getByText('Test Children')).toBeInTheDocument()
  })
})
