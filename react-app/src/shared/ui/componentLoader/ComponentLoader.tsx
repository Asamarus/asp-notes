import React, { Suspense } from 'react'
import Loading from '@/shared/ui/loading'

import type { LoadingProps } from '@/shared/ui/loading'

export interface ComponentLoaderProps extends LoadingProps {
  /** The content of the component */
  children?: React.ReactNode
}
function ComponentLoader(props: ComponentLoaderProps) {
  const { full, inline, children } = props

  return (
    <Suspense
      fallback={
        <Loading
          full={full}
          inline={inline}
        />
      }
    >
      {children}
    </Suspense>
  )
}

export default ComponentLoader
