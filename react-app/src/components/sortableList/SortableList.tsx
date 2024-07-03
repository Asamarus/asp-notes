import React, { useEffect, useRef } from 'react'
import { createPortal } from 'react-dom'
import {
  DragDropContext,
  Droppable,
  Draggable,
  DraggableProvidedDragHandleProps,
} from '@hello-pangea/dnd'
import type { DraggableStyle } from '@hello-pangea/dnd'

type Item = string

const reorder = (list: Item[], startIndex: number, endIndex: number): Item[] => {
  const result = Array.from(list)
  const [removed] = result.splice(startIndex, 1)
  result.splice(endIndex, 0, removed)

  return result
}

function lockAxis(style: DraggableStyle | undefined): React.CSSProperties | undefined {
  if (style?.transform) {
    const axisLockY = `translate(0px, ${style.transform.split(',').pop()}`

    return {
      ...style,
      transform: axisLockY,
    }
  }
  return style
}

export interface SortableListProps {
  /** List items */
  items: Item[]
  /** On sort end */
  onSortEnd: (ids: Item[]) => void
  /** Render item */
  renderItem: (
    id: Item,
    dragHandleProps: DraggableProvidedDragHandleProps | null | undefined,
  ) => React.ReactNode
}

function SortableList({ items, onSortEnd, renderItem }: SortableListProps) {
  const portalRef = useRef<HTMLDivElement | null>(null)

  useEffect(() => {
    const portalDiv = document.createElement('div')
    document.body.appendChild(portalDiv)
    portalRef.current = portalDiv

    return () => {
      document.body.removeChild(portalDiv)
    }
  }, [])

  return (
    <DragDropContext
      onDragEnd={({ destination, source }) => {
        // dropped outside the list
        if (!destination) {
          return
        }

        const newItems = reorder(items, source.index, destination.index)
        onSortEnd(newItems)
      }}
    >
      <Droppable
        droppableId="dnd-list"
        direction="vertical"
      >
        {(droppableProvided) => (
          <div
            {...droppableProvided.droppableProps}
            ref={droppableProvided.innerRef}
          >
            {items.map((id, index) => (
              <Draggable
                key={`${id}`}
                index={index}
                draggableId={`${id}`}
              >
                {(draggableProvided, draggableSnapshot) => {
                  const itemNode = (
                    <div
                      ref={draggableProvided.innerRef}
                      {...draggableProvided.draggableProps}
                      style={lockAxis(draggableProvided.draggableProps.style)}
                    >
                      {renderItem(id, draggableProvided.dragHandleProps)}
                    </div>
                  )

                  if (draggableSnapshot.isDragging && portalRef.current) {
                    return createPortal(itemNode, portalRef.current)
                  }

                  return itemNode
                }}
              </Draggable>
            ))}
            {droppableProvided.placeholder}
          </div>
        )}
      </Droppable>
    </DragDropContext>
  )
}

export default SortableList
