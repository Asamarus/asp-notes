import { Container, Title, Text, Button, Group } from '@mantine/core'
import { Link } from 'react-router-dom'
import { Illustration } from './Illustration'

import styles from './NotFoundPage.module.css'

const NotFoundPage = () => (
  <div className={styles['wrapper']}>
    <Container className={styles['container']}>
      <div className={styles['inner']}>
        <Illustration className={styles['image']} />
        <div className={styles['content']}>
          <Title className={styles['title']}>Nothing to see here</Title>
          <Text
            c="dimmed"
            size="lg"
            ta="center"
            className={styles['description']}
          >
            Page you are trying to open does not exist. You may have mistyped the address, or the
            page has been moved to another URL. If you think this is an error contact support.
          </Text>
          <Group justify="center">
            <Button
              component={Link}
              to="/"
              size="md"
              className={styles['button']}
            >
              Take me back to home page
            </Button>
          </Group>
        </div>
      </div>
    </Container>
  </div>
)

export default NotFoundPage
