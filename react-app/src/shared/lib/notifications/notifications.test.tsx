import { cleanup, render, screen, act } from '@test-utils'
import { notifications } from '@mantine/notifications'
import { showError, showSuccess } from './notifications'

import { TbExclamationMark, TbCircleCheckFilled } from 'react-icons/tb'

describe('notifications', () => {
  beforeEach(() => {
    cleanup()
  })

  it('should call notifications.show with the correct parameters', async () => {
    const spy = vi.spyOn(notifications, 'show')

    await act(async () => {
      showError('Test error message')
    })

    expect(spy).toHaveBeenCalledWith({
      message: 'Test error message',
      icon: <TbExclamationMark size={20} />,
      color: 'red',
      autoClose: 10000,
    })

    await act(async () => {
      showSuccess('Test success message')
    })

    expect(spy).toHaveBeenCalledWith({
      message: 'Test success message',
      icon: <TbCircleCheckFilled size={20} />,
      color: 'green',
      autoClose: 3000,
    })

    spy.mockRestore()
  })

  it('should override options correctly', async () => {
    const spy = vi.spyOn(notifications, 'show')

    await act(async () => {
      showError('Test error message', { color: 'blue', autoClose: 5000 })
    })

    expect(spy).toHaveBeenCalledWith({
      message: 'Test error message',
      icon: <TbExclamationMark size={20} />,
      color: 'blue',
      autoClose: 5000,
    })

    await act(async () => {
      showSuccess('Test success message', { color: 'yellow', autoClose: 2000 })
    })

    expect(spy).toHaveBeenCalledWith({
      message: 'Test success message',
      icon: <TbCircleCheckFilled size={20} />,
      color: 'yellow',
      autoClose: 2000,
    })

    spy.mockRestore()
  })

  it('should display message', async () => {
    render(<div />)

    await act(async () => {
      showError('Test error message')
    })

    expect(screen.getAllByText('Test error message')[0]).toBeInTheDocument()

    await act(async () => {
      showSuccess('Test success message')
    })

    expect(screen.getAllByText('Test success message')[0]).toBeInTheDocument()
  })
})
