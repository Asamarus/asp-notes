import useAppSelector from '@/shared/lib/useAppSelector'

import styles from './Title.module.css'

function Title() {
  const displayName = useAppSelector((state) => state.sections.current?.displayName)

  return (
    <div className={styles['wrapper']}>
      <span className={styles['title']}>{displayName}</span>
    </div>
  )
}

export default Title
