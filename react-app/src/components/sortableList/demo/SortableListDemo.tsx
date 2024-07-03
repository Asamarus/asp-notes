import SortableList from '@/components/sortableList'
import { useState } from 'react'

import { MdDragIndicator } from 'react-icons/md'

import classes from './SortableListDemo.module.css'

function SortableListDemo() {
  const [items, setItems] = useState(['1', '2', '3'])

  return (
    <>
      <SortableList
        items={items}
        onSortEnd={(newItems) => setItems(newItems)}
        renderItem={(id, dragHandleProps) => (
          <div className={classes.root}>
            <div className={classes.item}>
              <div {...dragHandleProps}>
                <MdDragIndicator
                  size={25}
                  className={classes.dragIcon}
                />
              </div>
              <div className={classes.content}>
                <span> Item #{id}</span>
              </div>
            </div>
          </div>
        )}
      />
    </>
  )
}

export default SortableListDemo
