import { useState, useEffect } from 'react'
//https://ckeditor.com/docs/ckeditor5/latest/getting-started/installation/react/react.html
import { CKEditor } from '@ckeditor/ckeditor5-react'
import { useComputedColorScheme } from '@mantine/core'

import {
  ClassicEditor,
  AutoLink,
  Bold,
  Code,
  CodeBlock,
  Essentials,
  Heading,
  Italic,
  Link,
  List,
  Paragraph,
  SourceEditing,
  Table,
  TableCaption,
  TableCellProperties,
  TableColumnResize,
  TableProperties,
  TableToolbar,
  Undo,
} from 'ckeditor5'
import { darkColorScheme, lightColorScheme } from './theme'
import FullscreenPlugin from './plugins/fullScreen'
import TabularDataPlugin from './plugins/tabularData'

export { Plugin, ButtonView, Dialog, View } from 'ckeditor5'

import type { EditorConfig, EventInfo, Editor } from 'ckeditor5'

import 'ckeditor5/ckeditor5.css'
import './NoteEditor.css'

export interface NoteEditorProps {
  /** Disabled state */
  disabled?: boolean

  /** Fired when editor content is changed */
  onChange?: (event: EventInfo, editor: Editor) => void

  /** Fired when editor is focused */
  onFocus?: (event: EventInfo, editor: Editor) => void

  /** Fired when editor loses focus */
  onBlur?: (event: EventInfo, editor: Editor) => void

  /** Editor value */
  value?: string
}

const editorConfig: EditorConfig = {
  licenseKey: 'GPL',
  toolbar: {
    items: [
      'fullscreen',
      'heading',
      '|',
      'codeBlock',
      'bold',
      'italic',
      'link',
      'bulletedList',
      'numberedList',
      'sourceEditing',
      '|',
      'code',
      'tabularData',
      'insertTable',
      'undo',
      'redo',
    ],
    shouldNotGroupWhenFull: true,
  },
  plugins: [
    AutoLink,
    Bold,
    Code,
    CodeBlock,
    Essentials,
    Heading,
    Italic,
    Link,
    List,
    Paragraph,
    SourceEditing,
    Table,
    TableCaption,
    TableCellProperties,
    TableColumnResize,
    TableProperties,
    TableToolbar,
    Undo,
    FullscreenPlugin,
    TabularDataPlugin,
  ],
  heading: {
    options: [
      {
        model: 'paragraph',
        title: 'Paragraph',
        class: 'ck-heading_paragraph',
      },
      {
        model: 'heading1',
        view: 'h1',
        title: 'Heading 1',
        class: 'ck-heading_heading1',
      },
      {
        model: 'heading2',
        view: 'h2',
        title: 'Heading 2',
        class: 'ck-heading_heading2',
      },
      {
        model: 'heading3',
        view: 'h3',
        title: 'Heading 3',
        class: 'ck-heading_heading3',
      },
      {
        model: 'heading4',
        view: 'h4',
        title: 'Heading 4',
        class: 'ck-heading_heading4',
      },
    ],
  },
  link: {
    addTargetToExternalLinks: true,
    defaultProtocol: 'https://',
    decorators: {
      toggleDownloadable: {
        mode: 'manual',
        label: 'Downloadable',
        attributes: {
          download: 'file',
        },
      },
    },
  },
  placeholder: 'Note content',
  table: {
    contentToolbar: [
      'tableColumn',
      'tableRow',
      'mergeTableCells',
      'tableProperties',
      'tableCellProperties',
    ],
  },
  codeBlock: {
    languages: [
      { language: 'plain-text', label: 'Plain text' },
      { language: 'csharp', label: 'C#' },
      { language: 'tsx', label: '.tsx' },
      { language: 'jsx', label: '.jsx' },
      { language: 'typescript', label: 'TypeScript' },
      { language: 'javascript', label: 'JavaScript' },
      { language: 'css', label: 'CSS' },
      { language: 'html', label: 'HTML' },
      { language: 'xml', label: 'XML' },
      { language: 'python', label: 'Python' },
      { language: 'sql', label: 'SQL' },
      { language: 'json', label: 'JSON' },
      { language: 'less', label: 'LESS' },
      { language: 'sass', label: 'Sass' },
      { language: 'java', label: 'Java' },
      { language: 'regex', label: 'Regex' },
      { language: 'bash', label: 'Bash' },
      { language: 'diff', label: 'Diff' },
      { language: 'git', label: 'git' },
    ],
  },
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

function NoteEditor({ onBlur, onChange, onFocus, value, disabled }: NoteEditorProps) {
  const [isLayoutReady, setIsLayoutReady] = useState(false)
  const colorScheme = useComputedColorScheme('light')

  useEffect(() => {
    setIsLayoutReady(true)

    return () => setIsLayoutReady(false)
  }, [])

  useEffect(() => {
    setColorScheme(colorScheme)
  }, [colorScheme])

  return (
    <div className="note-editor-wrapper">
      <div className="note-editor">
        <div className="note-editor-content">
          {isLayoutReady && (
            <CKEditor
              editor={ClassicEditor}
              config={editorConfig}
              disabled={disabled}
              onBlur={onBlur}
              onChange={onChange}
              onFocus={onFocus}
              data={value}
            />
          )}
        </div>
      </div>
    </div>
  )
}

export default NoteEditor
