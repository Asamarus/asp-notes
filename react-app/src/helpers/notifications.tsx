import { TbExclamationMark, TbCircleCheckFilled } from 'react-icons/tb'
import { notifications } from '@mantine/notifications'
import type { NotificationData } from '@mantine/notifications'
export function showError(msg: string, options: Partial<NotificationData> = {}) {
  notifications.show({
    message: msg,
    icon: <TbExclamationMark size={20} />,
    color: 'red',
    autoClose: 10000,
    ...options,
  })
}

export function showSuccess(msg: string, options: Partial<NotificationData> = {}) {
  notifications.show({
    message: msg,
    icon: <TbCircleCheckFilled size={20} />,
    color: 'green',
    autoClose: 3000,
    ...options,
  })
}
