import type { State, User } from './types'

export enum ActionType {
  SetUser = 'SetUser',
}

interface SetUserAction {
  type: ActionType.SetUser
  payload: User
}

export default function userReducer(state: State, action: SetUserAction): State {
  switch (action.type) {
    case ActionType.SetUser: {
      return {
        ...state,
        user: action.payload,
      }
    }

    default: {
      return state
    }
  }
}
