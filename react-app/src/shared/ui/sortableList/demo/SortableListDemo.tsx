import { useState } from 'react'

import SortableList from '@/shared/ui/sortableList'
import { MdDragIndicator } from 'react-icons/md'

import styles from './SortableListDemo.module.css'

function SortableListDemo() {
  const [items, setItems] = useState([{ id: 1 }, { id: 2 }, { id: 3 }, { id: 4 }])

  return (
    <>
      <SortableList
        items={items}
        onSortEnd={(newItems) => setItems(newItems)}
        renderItem={(item, dragHandleProps) => (
          <div className={styles['wrapper']}>
            <div className={styles['item']}>
              <div {...dragHandleProps}>
                <MdDragIndicator
                  size={25}
                  className={styles['drag-icon']}
                />
              </div>
              <div className={styles['content']}>
                <span> Item #{item.id}</span>
              </div>
            </div>
          </div>
        )}
      />
    </>
  )
}

export default SortableListDemo
