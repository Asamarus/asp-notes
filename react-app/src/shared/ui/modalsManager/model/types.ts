import type { ReactNode } from 'react'

import type { ModalProps } from '@mantine/core'

export type ConfirmLabels = Record<'confirm' | 'cancel', ReactNode>

export type ModalSettings = Partial<Omit<ModalProps, 'opened' | 'onClose'>> & {
  modalId?: string
  onCloseConfirm?(id: string): void
  onClosed?(): void
}

export type OpenModalParams = {
  modalId?: string
  name: string
  data?: Record<string, unknown>
  settings?: ModalSettings
}

export type ModalData = {
  name: string
  inUrl?: boolean
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  component: React.ComponentType<any>
  modalProps?: ModalSettings
}

export type ActiveModal = {
  id: string
  name: string
  inUrl: boolean
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  component: React.ComponentType<any>
  data: Record<string, unknown>
  settings: ModalSettings
  modalProps: ModalSettings
}

export type ModalsState = {
  modals: string[]
  modalState: Record<string, string>
}
