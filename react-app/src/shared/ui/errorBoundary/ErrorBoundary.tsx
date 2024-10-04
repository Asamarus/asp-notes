import React, { ErrorInfo } from 'react'

import { Title, Text, Button, Container, Group } from '@mantine/core'

import styles from './ErrorBoundary.module.css'

interface ErrorBoundaryProps {
  children: React.ReactNode
}

interface ErrorBoundaryState {
  hasError: boolean
  error: Error | null
  errorInfo: ErrorInfo | null
}

class ErrorBoundary extends React.Component<ErrorBoundaryProps, ErrorBoundaryState> {
  constructor(props: ErrorBoundaryProps) {
    super(props)
    this.state = {
      hasError: false,
      error: null,
      errorInfo: null,
    }
  }

  componentDidCatch(error: Error, errorInfo: ErrorInfo) {
    this.setState({
      hasError: true,
      error,
      errorInfo,
    })
    console.error(error, errorInfo)
  }

  render() {
    if (this.state.hasError) {
      return (
        <div className={styles['wrapper']}>
          <Container>
            <div className={styles['label']}>500</div>
            <Title className={styles['title']}>Something went wrong.</Title>
            <Text
              size="lg"
              ta="center"
              className={styles['description']}
            >
              Error occurred while rendering this page. Please try to refresh the page or contact
              support.
            </Text>
            <Group justify="center">
              <Button
                variant="white"
                size="md"
                onClick={() => window.location.reload()}
              >
                Refresh the page
              </Button>
            </Group>
          </Container>
        </div>
      )
    }

    return this.props.children
  }
}

export default ErrorBoundary
