import useAppSelector from '@/shared/lib/useAppSelector'
import clsx from 'clsx'

import styles from './Info.module.css'

export interface InfoProps {
  /** Note's id */
  id: number

  flexItem?: boolean
}
function Info({ id, flexItem = false }: InfoProps) {
  const createdAt = useAppSelector((state) => state.notes.collection[id]?.createdAt)
  return (
    <span
      className={clsx(styles['wrapper'], { [styles['wrapper-flex']]: flexItem })}
    >{`#${id} ${createdAt}`}</span>
  )
}

export default Info
