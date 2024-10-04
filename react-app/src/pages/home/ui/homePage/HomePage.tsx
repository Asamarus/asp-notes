import { useEffect } from 'react'
import { useNavigate } from 'react-router-dom'
import store from '@/shared/lib/store'

import Loading from '@/shared/ui/loading'

export function HomePage() {
  const navigate = useNavigate()
  useEffect(() => {
    const sections = store.getState().sections.list

    if (sections.length > 0) {
      navigate('/notes')
    } else {
      navigate('/settings')
    }
  }, [navigate])

  return <Loading full />
}

export default HomePage
