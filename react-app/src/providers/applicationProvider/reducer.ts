import { State, InitialData } from './types'

export enum ActionType {
  SetInitialData = 'SetInitialData',
}

interface SetInitialDataAction {
  type: ActionType.SetInitialData
  payload: InitialData
}

export default function userReducer(state: State, action: SetInitialDataAction): State {
  switch (action.type) {
    case ActionType.SetInitialData: {
      return {
        ...state,
        initialData: action.payload,
      }
    }

    default: {
      return state
    }
  }
}
