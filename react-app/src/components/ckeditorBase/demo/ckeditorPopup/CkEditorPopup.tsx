import classes from './CkEditorPopup.module.css'

import { Button } from '@mantine/core'
import { useFullscreen } from '@mantine/hooks'
import CKEditorBase from '@/components/ckeditorBase'

function CkEditorPopup() {
  const { ref, toggle, fullscreen } = useFullscreen()
  return (
    <div className={classes.root}>
      <div className={classes.header}>
        <div>This is dynamic header</div>

        <Button
          onClick={toggle}
          color={fullscreen ? 'red' : 'blue'}
        >
          {fullscreen ? 'Exit Fullscreen' : 'View Image Fullscreen'}
        </Button>
      </div>
      <div
        ref={ref}
        className={classes.content}
      >
        <CKEditorBase />
      </div>
      <div className={classes.footer}>
        <div>This is dynamic footer</div>
      </div>
    </div>
  )
}

export default CkEditorPopup
