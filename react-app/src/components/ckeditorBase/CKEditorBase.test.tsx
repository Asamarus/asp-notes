import { render } from '@test-utils'
import CKEditor from './CKEditorBase'

describe('CKEditor', () => {
  test('renders without crashing', () => {
    render(<CKEditor />)
  })
})
