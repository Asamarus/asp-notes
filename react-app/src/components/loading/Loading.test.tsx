import { render, screen } from '@test-utils'
import Loading from './Loading'

import styles from './Loading.module.css'

describe('Loading', () => {
  test('renders without crashing', () => {
    render(<Loading />)
  })

  test('applies custom class name', () => {
    render(<Loading className="custom-class" />)
    expect(screen.getByLabelText('Loading')).toHaveClass('custom-class')
  })

  test('applies custom styles', () => {
    render(<Loading style={{ color: 'red' }} />)
    expect(screen.getByLabelText('Loading')).toHaveStyle('color: rgb(255, 0, 0)')
  })

  it('applies full class when full prop is true', () => {
    render(<Loading full />)
    const loadingElement = screen.getByLabelText('Loading')
    expect(loadingElement).toHaveClass(styles.full)
  })

  it('applies inline class when inline prop is true', () => {
    render(<Loading inline />)
    const loadingElement = screen.getByLabelText('Loading')
    expect(loadingElement).toHaveClass(styles.inline)
  })
})
