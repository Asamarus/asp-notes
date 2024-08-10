import { Loader } from '@mantine/core'
import clsx from 'clsx'
import type { LoaderProps } from '@mantine/core'

import styles from './Loading.module.css'

export interface LoadingProps {
  className?: string
  style?: React.CSSProperties
  full?: boolean
  inline?: boolean
  /** Loader props */
  loaderProps?: LoaderProps
}
function Loading({ className, style, loaderProps, full, inline }: LoadingProps) {
  const wrapperClassName = clsx(
    styles['root'],
    {
      [styles['full']]: full,
      [styles['inline']]: inline,
    },
    className,
  )
  return (
    <div
      className={wrapperClassName}
      style={style}
      aria-label="Loading"
    >
      <Loader
        size={40}
        {...loaderProps}
      />
    </div>
  )
}

export default Loading
