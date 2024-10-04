import { act, render, screen, userEvent, waitFor } from '@test-utils'
import { openModal, closeModal, defaultModals } from '../../index'
import { useLocation, useSearchParams } from 'react-router-dom'

import ModalsManager from './ModalsManager'

import type { ModalData, ModalSettings } from '../../model/types'
import type { Location } from 'react-router-dom'

function ModalWithData({ id }: { id: string }) {
  return <div>Current id is: {id}</div>
}

const modalWithData: ModalData = {
  name: 'modalWithData',
  component: ModalWithData,
}

function ModalWithUrl() {
  return <div>Modal with URL</div>
}

const onCloseConfirmMock = vi.fn()

const modalWithUrlData: ModalData = {
  name: 'modalWithUrl',
  inUrl: true,
  component: ModalWithUrl,
  modalProps: {
    onCloseConfirm: (id) => {
      onCloseConfirmMock(id)
      closeModal(id)
    },
  },
}

const modals = {
  ...defaultModals,
  ...{
    [modalWithUrlData.name]: modalWithUrlData,
    [modalWithData.name]: modalWithData,
  },
}

function ModalsManagerWrapper({
  commonModalProps,
}: {
  commonModalProps?: ModalSettings | undefined
}) {
  return (
    <ModalsManager
      modals={modals}
      commonModalProps={commonModalProps}
    />
  )
}

describe('ModalsManager', () => {
  it('opens and closes modal', async () => {
    render(
      <div>
        <ModalsManagerWrapper />
        <button
          onClick={() =>
            openModal({
              name: 'content',
              data: {
                children: <div>This is test modal</div>,
              },
            })
          }
        >
          Open test modal
        </button>
      </div>,
    )

    await userEvent.click(screen.getByRole('button', { name: 'Open test modal' }))

    //Modal is opened
    await waitFor(() => {
      expect(screen.getByText('This is test modal')).toBeInTheDocument()
    })

    await userEvent.keyboard('{Escape}')

    // Modal is closed
    await waitFor(() => {
      expect(screen.queryByText('This is test modal')).not.toBeInTheDocument()
    })
  })

  it('opens multiple modals', async () => {
    render(<ModalsManagerWrapper />)

    act(() => {
      openModal({
        name: 'content',
        data: {
          children: <div>This is test modal</div>,
        },
      })
    })

    expect(await screen.findByText('This is test modal')).toBeInTheDocument()

    act(() => {
      openModal({
        name: 'content',
        data: {
          children: <div>This is test modal</div>,
        },
      })
    })

    await waitFor(() => {
      expect(screen.getAllByText('This is test modal')).toHaveLength(2)
    })

    await userEvent.keyboard('{Escape}')

    //Only one modal is closed
    await waitFor(() => {
      expect(screen.getAllByText('This is test modal')).toHaveLength(1)
    })
  })

  it('opens a modal with URL, closes it on ESC press, and calls onCloseConfirmMock with a string argument', async () => {
    let location: Location
    const TestComponent = () => {
      location = useLocation()
      return <ModalsManagerWrapper />
    }

    render(<TestComponent />)

    act(() => {
      openModal({
        name: 'modalWithUrl',
      })
    })

    expect(await screen.findByText('Modal with URL')).toBeInTheDocument()
    expect(location!.search).toBe('?modalWithUrl=%28%29')

    await userEvent.keyboard('{Escape}')

    await waitFor(() => {
      expect(screen.queryByText('This is test modal')).not.toBeInTheDocument()
    })

    await waitFor(() => {
      expect(location!.search).toBe('')
    })

    expect(onCloseConfirmMock).toHaveBeenCalledWith(expect.any(String))
  })

  it('opens and closes a modal with URL on URL change', async () => {
    let _setSearchParams: (params: URLSearchParams) => void
    const TestComponent = () => {
      const [, setSearchParams] = useSearchParams()
      _setSearchParams = setSearchParams
      return <ModalsManagerWrapper />
    }

    render(<TestComponent />)

    act(() => {
      _setSearchParams(new URLSearchParams('modalWithUrl=%28%29'))
    })

    expect(await screen.findByText('Modal with URL')).toBeInTheDocument()

    act(() => {
      _setSearchParams(new URLSearchParams(''))
    })

    await waitFor(() => {
      expect(screen.queryByText('This is test modal')).not.toBeInTheDocument()
    })
  })

  it('updates modal on data change', async () => {
    render(<ModalsManagerWrapper />)

    act(() => {
      openModal({
        name: 'modalWithData',
        data: {
          id: '1',
        },
      })
    })

    expect(await screen.findByText('Current id is: 1')).toBeInTheDocument()

    act(() => {
      openModal({
        name: 'modalWithData',
        data: {
          id: '2',
        },
      })
    })

    expect(await screen.findByText('Current id is: 2')).toBeInTheDocument()
  })
})
