import useAppSelector from '@/shared/lib/useAppSelector'
import { useComputedColorScheme } from '@mantine/core'
import parse, { HTMLReactParserOptions, Element } from 'html-react-parser'
import { tomorrow, oneLight } from 'react-syntax-highlighter/dist/cjs/styles/prism'
import get from 'lodash/get'

import { Box } from '@mantine/core'
import { Prism } from 'react-syntax-highlighter'

import styles from './View.module.css'

export interface ViewProps {
  /** Note's id */
  id: number
}

const options: HTMLReactParserOptions = {
  replace: (domNode) => {
    if (domNode instanceof Element && domNode.attribs) {
      if (domNode.name === 'pre') {
        const language = get(domNode, 'children.0.attribs.class', null)
        const code = get(domNode, 'children.0.children.0.data', null)

        if (language !== null && code !== null && typeof language === 'string') {
          return (
            <SyntaxHighlighter
              language={language.substring(9)}
              code={code}
            />
          )
        }
      }
    }
  },
}

function SyntaxHighlighter({ language, code }: { language: string; code: string }) {
  const colorScheme = useComputedColorScheme('light')

  return (
    <Box mb={20}>
      <Prism
        language={language}
        style={colorScheme === 'dark' ? tomorrow : oneLight}
        className={styles['code']}
      >
        {code}
      </Prism>
    </Box>
  )
}

function View({ id }: ViewProps) {
  const title = useAppSelector((state) => state.notes.collection[id]?.title ?? '')
  const content = useAppSelector((state) => state.notes.collection[id]?.content ?? '')
  return (
    <>
      <div className={styles['title']}>{title}</div>
      <div className={styles['content']}>{parse(content, options)}</div>
    </>
  )
}

export default View
