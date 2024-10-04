import { lazy } from 'react'
import { Routes, Route, Navigate } from 'react-router-dom'
import ComponentLoader from '@/shared/ui/componentLoader'

const HomePage = lazy(() => import('@/pages/home'))
const NotesPage = lazy(() => import('@/pages/notes'))
const NotePage = lazy(() => import('@/pages/note'))
const SettingsPage = lazy(() =>
  import('@/pages/settings').then((module) => ({ default: module.SettingsPage })),
)
const Sections = lazy(() =>
  import('@/pages/settings/').then((module) => ({ default: module.Sections })),
)
const NotFound = lazy(() => import('@/pages/notFound'))

function Pages() {
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
        path="/notes/:section?"
        element={
          <ComponentLoader full>
            <NotesPage />
          </ComponentLoader>
        }
      />
      <Route
        path="/note/:id"
        element={
          <ComponentLoader full>
            <NotePage />
          </ComponentLoader>
        }
      />
      <Route
        path="/settings"
        element={
          <ComponentLoader full>
            <SettingsPage />
          </ComponentLoader>
        }
      >
        <Route
          index
          element={
            <Navigate
              to="/settings/sections"
              replace
            />
          }
        />
        <Route
          path="sections"
          element={
            <ComponentLoader full>
              <Sections />
            </ComponentLoader>
          }
        />
      </Route>
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

export default Pages
