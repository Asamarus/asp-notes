import { render } from '@test-utils'
import ResponsiveMasonry from './ResponsiveMasonry'

const getGridProps = () => {
  return {
    gutter: 10,
    minWidth: 200,
  }
}

function TestResponsiveMasonry() {
  const divArray = Array.from({ length: 50 }, (_, i) => i)
  const heights = [100, 200, 300]

  return (
    <ResponsiveMasonry getGridProps={getGridProps}>
      {divArray.map((_, index) => (
        <div
          key={index}
          style={{
            border: '1px solid black',
            padding: '10px',
            height: heights[Math.floor(Math.random() * heights.length)],
          }}
        >
          {index}
        </div>
      ))}
    </ResponsiveMasonry>
  )
}

describe('ResponsiveMasonry', () => {
  test('renders without crashing', () => {
    render(<TestResponsiveMasonry />)
  })
})
