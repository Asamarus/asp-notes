import { lazy } from 'react'
import { Routes, Route } from 'react-router-dom'
import ComponentLoader from '@/components/componentLoader'

const HomePage = lazy(() => import('@/pages/home'))
const DashboardPage = lazy(() => import('@/pages/dashboard'))
const Page = lazy(() => import('@/pages/page'))
const NotFound = lazy(() => import('@/pages/notFound'))

function Content() {
  return (
    <Routes>
      <Route
        path="/"
        element={
          <ComponentLoader full>
            <HomePage />
          </ComponentLoader>
        }
      />
      <Route
        path="/dashboard"
        element={
          <ComponentLoader full>
            <DashboardPage />
          </ComponentLoader>
        }
      />
      <Route
        path="/page"
        element={
          <ComponentLoader full>
            <Page />
          </ComponentLoader>
        }
      />
      <Route
        path="*"
        element={
          <ComponentLoader full>
            <NotFound />
          </ComponentLoader>
        }
      />
    </Routes>
  )
}

export default Content
