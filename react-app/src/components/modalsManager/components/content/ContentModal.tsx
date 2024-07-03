import React from 'react'

export interface ContentModalProps {
  children: React.ReactNode
}

function ContentModal({ children }: ContentModalProps) {
  return <>{children}</>
}

export default ContentModal
