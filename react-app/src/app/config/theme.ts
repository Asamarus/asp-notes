import { createTheme, ActionIcon, Textarea, Tooltip } from '@mantine/core'
import commonStyles from '@/shared/ui/commonStyles.module.css'

export const theme = createTheme({
  /** Put your Mantine theme override here */
  cursorType: 'pointer',
  primaryColor: 'blue',
  activeClassName: commonStyles['active-element'],
  components: {
    ActionIcon: ActionIcon.extend({
      defaultProps: {
        variant: 'subtle',
        color: 'gray',
      },
    }),
    Textarea: Textarea.extend({
      defaultProps: {
        autosize: true,
      },
    }),
    Tooltip: Tooltip.extend({
      defaultProps: {
        openDelay: 1000,
      },
    }),
    ModalHeader: {
      defaultProps: {
        color: 'blue',
      },
    },
  },
})
