import React, { useReducer, useRef, useEffect, useCallback } from 'react'
import { useSearchParams, useLocation } from 'react-router-dom'
import { getDefaultZIndex } from '@mantine/core'
import randomId from '@/shared/lib/randomId'
import { useWindowEvent } from '@mantine/hooks'
import rison from 'rison'
import hash from 'object-hash'
import isEqual from 'lodash/isEqual'
import omit from 'lodash/omit'
import has from 'lodash/has'
import get from 'lodash/get'
import modalsReducer, { ActionType } from '../../model/reducer'
import { useModalsEvents } from '../../model/events'

import Modal from '../modal'

import type { ModalData, ModalSettings, OpenModalParams, ActiveModal } from '../../model/types'

export interface ModalsManagerProps {
  /** List of modals */
  modals: Record<string, ModalData>

  /** Shared Modal component props, applied for every modal */
  commonModalProps?: ModalSettings
}

const zIndex = getDefaultZIndex('modal')

function ModalsManager({ modals, commonModalProps }: ModalsManagerProps) {
  const activeModals = useRef<Record<string, ActiveModal>>({})
  const activeModalsStack = useRef<string[]>([])
  const activeUrlModals = useRef<Record<string, string>>({})

  const location = useLocation()
  const [urlParams, setSearchParams] = useSearchParams()

  const [state, dispatch] = useReducer(modalsReducer, {
    modals: [],
    modalState: {},
  })

  const handleOpenModal = useCallback(
    ({ modalId, name, data = {}, settings }: OpenModalParams) => {
      const modal = modals[name]

      if (!modal) {
        console.error('No modal with name:' + name)
        return
      }

      const id = modalId || randomId()

      if (activeModals.current[id]) {
        console.error(`Modal with id:${id} already added!`)
        return
      }

      activeModals.current[id] = {
        id,
        name: name,
        inUrl: modal.inUrl ?? false,
        component: modal.component,
        data: data,
        settings: settings ?? {},
        modalProps: modal.modalProps ?? {},
      }

      if (modal.inUrl && has(activeUrlModals.current, modal.name)) {
        const oldId = activeUrlModals.current[modal.name]
        activeUrlModals.current[modal.name] = id

        activeModals.current = omit(activeModals.current, oldId)

        activeModalsStack.current = [...activeModalsStack.current.filter((m) => m !== oldId), id]

        urlParams.set(modal.name, rison.encode(data))
        setSearchParams(urlParams)

        dispatch({
          type: ActionType.Replace,
          payload: [oldId, id],
        })

        return
      }

      if (modal.inUrl) {
        activeUrlModals.current[modal.name] = id
        urlParams.set(modal.name, rison.encode(data))
        setSearchParams(urlParams)
      }

      activeModalsStack.current = [...activeModalsStack.current, id]

      dispatch({
        type: ActionType.Add,
        payload: id,
      })
    },
    [modals, setSearchParams, urlParams],
  )

  const handleCloseModal = useCallback((id: string) => {
    if (!has(activeModals.current, id)) {
      console.error(`Modal with id:${id} already closed!`)
      return
    }
    dispatch({
      type: ActionType.Close,
      payload: id,
    })
  }, [])

  const handleKeyUp = (event: KeyboardEvent) => {
    if (event.key === 'Escape' && activeModalsStack.current.length > 0) {
      const modalId = activeModalsStack.current[activeModalsStack.current.length - 1]
      const modal = activeModals.current[modalId]

      if (
        modal &&
        get(modal, 'modalProps.closeOnEscape', true) &&
        get(modal, 'settings.closeOnEscape', true)
      ) {
        const onCloseConfirm = modal.modalProps.onCloseConfirm
        if (typeof onCloseConfirm === 'function') {
          onCloseConfirm(modal.id)
        } else {
          handleCloseModal(modal.id)
        }
      }
    }
  }

  useWindowEvent('keyup', handleKeyUp)

  useModalsEvents({
    openModal: handleOpenModal,
    closeModal: handleCloseModal,
  })

  useEffect(() => {
    const urlParamsEntries: Record<string, string> = {}

    for (const [key, value] of urlParams.entries()) {
      urlParamsEntries[key] = value
    }

    Object.keys(modals).forEach((key) => {
      const modal = modals[key]
      if (
        modal.inUrl &&
        !has(urlParamsEntries, modal.name) &&
        has(activeUrlModals.current, modal.name)
      ) {
        handleCloseModal(activeUrlModals.current[modal.name])
      }
    })

    Object.keys(modals).forEach((key) => {
      const modal = modals[key]
      if (modal.inUrl && has(urlParamsEntries, modal.name)) {
        const data = rison.decode(urlParamsEntries[modal.name]) as Record<string, unknown>
        const currentData = activeModals.current[activeUrlModals.current[modal.name]]?.data ?? {}
        if (!has(activeUrlModals.current, modal.name) || !isEqual(data, currentData)) {
          handleOpenModal({ name: modal.name, data: data })
        }
      }
    })
  }, [location.key, modals, handleCloseModal, handleOpenModal, urlParams])

  return (
    <>
      {state.modals.map((id, index) => {
        const modal = activeModals.current[id]

        if (!modal) {
          return
        }

        const { data, modalProps, settings, component } = modal

        const extraProps = {
          ...commonModalProps,
          ...modalProps,
          ...settings,
        }

        const props = {
          key: hash(omit(data, 'children')),
          ...data,
          onModalClose: () => {
            const { onCloseConfirm } = extraProps
            if (typeof onCloseConfirm === 'function') {
              onCloseConfirm(id)
            } else {
              handleCloseModal(id)
            }
          },
        }

        const modalState = state.modalState[id]

        return (
          <Modal
            key={id}
            {...omit(extraProps, ['onCloseConfirm', 'onClosed'])}
            zIndex={zIndex + index}
            opened={modalState === 'opened'}
            closeOnEscape={false}
            onMounted={() => {
              window.requestAnimationFrame(() => {
                dispatch({
                  type: ActionType.Open,
                  payload: id,
                })
              })
            }}
            onClose={() => {
              const { onCloseConfirm } = extraProps
              if (typeof onCloseConfirm === 'function') {
                onCloseConfirm(id)
              } else {
                handleCloseModal(id)
              }
            }}
            transitionProps={{
              onExited: () => {
                const { onClosed } = extraProps

                onClosed?.()

                activeModalsStack.current = activeModalsStack.current.filter((m) => m !== id)

                activeModals.current = omit(activeModals.current, id)

                if (modal.inUrl) {
                  activeUrlModals.current = omit(activeUrlModals.current, modal.name)

                  if (urlParams.has(modal.name)) {
                    urlParams.delete(modal.name)
                    setSearchParams(urlParams)
                  }
                }

                dispatch({
                  type: ActionType.Remove,
                  payload: id,
                })
              },
            }}
          >
            {React.createElement(component, props)}
          </Modal>
        )
      })}
    </>
  )
}

export default ModalsManager
