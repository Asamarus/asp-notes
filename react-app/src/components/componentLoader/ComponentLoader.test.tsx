import { render, screen } from '@test-utils'
import ComponentLoader from './ComponentLoader'

describe('ComponentLoader', () => {
  it('renders without crashing', () => {
    render(<ComponentLoader />)
  })

  it('renders children correctly', () => {
    render(
      <ComponentLoader>
        <div>Test Child</div>
      </ComponentLoader>,
    )
    const childElement = screen.getByText('Test Child')
    expect(childElement).toBeInTheDocument()
  })
})
