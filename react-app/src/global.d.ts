declare global {
  type RootState = import('./app/model/store').RootState
  type AppDispatch = import('./app/model/store').AppDispatch

  interface Window {
    __PRELOADED_STATE__: unknown
  }
}

export {}
