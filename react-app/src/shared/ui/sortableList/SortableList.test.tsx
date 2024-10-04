import { render } from '@test-utils'
import SortableListDemo from './demo/SortableListDemo'

describe('SortableList', () => {
  test('renders without crashing', () => {
    render(<SortableListDemo />)
  })
})
