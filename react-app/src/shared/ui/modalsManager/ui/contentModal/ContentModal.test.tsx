import { render, screen } from '@testing-library/react'
import ContentModal from './ContentModal'

describe('ContentModal', () => {
  test('renders children correctly', () => {
    const testText = 'Test child'
    render(<ContentModal>{testText}</ContentModal>)

    expect(screen.getByText(testText)).toBeInTheDocument()
  })
})
