import React, { useRef, useEffect } from 'react'
import useScript from '@/hooks/useScript'
import { Loader, useComputedColorScheme } from '@mantine/core'
import styles from './CKEditorBase.module.css'

//https://cdn.ckeditor.com/
const _url = 'https://cdn.ckeditor.com/ckeditor5/41.3.1/classic/ckeditor.js'

const readOnlyLockId = 'modules-read-only-lock-id'

import { darkColorScheme, lightColorScheme } from './theme'

interface Editor {
  enableReadOnlyMode(lockId: string): void
  disableReadOnlyMode(lockId: string): void
  setData(value: string): void
  getData(): string
  model: {
    document: {
      on(event: string, callback: () => void): void
    }
  }
  editing: {
    view: {
      document: {
        on(event: string, callback: (event: React.FocusEvent) => void): void
      }
    }
  }
  destroy(): void
}

export interface CKEditorBaseProps {
  /** Editor url */
  editorUrl?: string

  /** Disabled state */
  disabled?: boolean

  /** Editor config */
  config?: Record<string, unknown>

  /** Placeholder */
  placeholder?: string

  /** Fired when editor content is changed */
  onChange?(value: string): void

  /** Fired when editor instance is ready */
  onReady?(editor: unknown): void

  /** Fired when editor is focused */
  onFocus?(event: React.FocusEvent): void

  /** Fired when editor loses focus */
  onBlur?(event: React.FocusEvent): void

  /** Editor value */
  value?: string
}

const defaultProps: Partial<CKEditorBaseProps> = {
  editorUrl: _url,
  disabled: false,
}

function initEditor(editor: Editor, props: CKEditorBaseProps) {
  const { disabled, onChange, onReady, onFocus, onBlur, value } = props

  if (disabled) {
    editor.enableReadOnlyMode(readOnlyLockId)
  }

  if (typeof value === 'string') {
    editor.setData(value)
  }

  const modelDocument = editor.model.document
  const viewDocument = editor.editing.view.document

  modelDocument.on('change:data', () => {
    if (typeof onChange === 'function') {
      const data = editor.getData()
      onChange(data.replace(/[\r]+/g, ''))
    }
  })

  viewDocument.on('focus', (event: React.FocusEvent) => {
    if (typeof onFocus === 'function') {
      onFocus(event)
    }
  })

  viewDocument.on('blur', (event: React.FocusEvent) => {
    if (typeof onBlur === 'function') {
      onBlur(event)
    }
  })

  setTimeout(() => {
    if (typeof onReady === 'function') {
      onReady(editor)
    }
  })
}

function setColorScheme(colorScheme: 'light' | 'dark') {
  const r = document.querySelector(':root') as HTMLElement

  if (colorScheme === 'light') {
    for (const key in lightColorScheme) {
      r.style.setProperty(key, lightColorScheme[key])
    }
  } else {
    for (const key in darkColorScheme) {
      r.style.setProperty(key, darkColorScheme[key])
    }
  }
}

function CKEditor({
  editorUrl = defaultProps.editorUrl,
  disabled = defaultProps.disabled,
  value,
  placeholder,
  config,
  ...props
}: CKEditorBaseProps) {
  const colorScheme = useComputedColorScheme('light')

  const status = useScript(editorUrl!, {
    removeOnUnmount: false,
  })

  const node = useRef(null)
  const editor = useRef<Editor | null>(null)

  useEffect(() => {
    if (typeof window['ClassicEditor' as keyof Window] !== 'undefined') {
      try {
        window['ClassicEditor' as keyof Window]
          .create(node.current, { ...config, ...{ placeholder } })
          .then((e: Editor) => {
            editor.current = e
            setColorScheme(colorScheme)
            initEditor(e, { disabled, value, ...props })
          })
          .catch((error: unknown) => {
            console.error(error)
          })
      } catch (error) {
        console.error(error)
      }
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [status])

  useEffect(() => {
    return () => {
      if (editor.current !== null && typeof editor.current.destroy === 'function') {
        editor.current.destroy()
      }
    }
  }, [])

  useEffect(() => {
    if (editor.current) {
      if (disabled) {
        editor.current?.enableReadOnlyMode(readOnlyLockId)
      } else {
        editor.current?.disableReadOnlyMode(readOnlyLockId)
      }
    }
  }, [disabled])

  useEffect(() => {
    if (editor.current) {
      editor.current.setData(value!)
    }
  }, [value])

  useEffect(() => {
    setColorScheme(colorScheme)
  }, [colorScheme])

  return (
    <>
      {status !== 'ready' && (
        <div className={styles.loader}>
          <Loader />
        </div>
      )}
      <div ref={node} />
    </>
  )
}

const MemoizedCKEditor = React.memo(CKEditor)
export default MemoizedCKEditor
