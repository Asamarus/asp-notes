import { configureStore } from '@reduxjs/toolkit'
import { sectionsSlice } from '@/entities/section'
import { notesSlice } from '@/entities/note'
import { userSlice } from '@/entities/user'

const store = configureStore({
  reducer: {
    sections: sectionsSlice.reducer,
    notes: notesSlice.reducer,
    userData: userSlice.reducer,
  },
})

export type RootState = ReturnType<typeof store.getState>
export type AppDispatch = typeof store.dispatch
export default store
