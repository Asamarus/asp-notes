import { createSlice, PayloadAction } from '@reduxjs/toolkit'

import type { User } from './types'

type State = {
  user: User | null
}

const initialState: State = {
  user: null,
}

export const userSlice = createSlice({
  name: 'userData',
  initialState,
  reducers: {
    setUser: (state, action: PayloadAction<User | null>) => {
      state.user = action.payload
    },
  },
})

export const { setUser } = userSlice.actions
