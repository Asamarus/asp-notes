import { ModalsState } from './types'

export enum ActionType {
  Add = 'Add',
  Open = 'Open',
  Replace = 'Replace',
  Close = 'Close',
  Remove = 'Remove',
}

interface AddAction {
  type: ActionType.Add
  payload: string
}

interface OpenAction {
  type: ActionType.Open
  payload: string
}

interface ReplaceAction {
  type: ActionType.Replace
  payload: [string, string]
}

interface CloseAction {
  type: ActionType.Close
  payload: string
}

interface RemoveAction {
  type: ActionType.Remove
  payload: string
}

export default function modalsReducer(
  state: ModalsState,
  action: AddAction | OpenAction | ReplaceAction | CloseAction | RemoveAction,
): ModalsState {
  switch (action.type) {
    case ActionType.Add: {
      return {
        modals: [...state.modals, action.payload],
        modalState: { ...state.modalState, ...{ [action.payload]: 'added' } },
      }
    }

    case ActionType.Open: {
      return {
        modals: state.modals,
        modalState: { ...state.modalState, ...{ [action.payload]: 'opened' } },
      }
    }

    case ActionType.Replace: {
      const [oldId, newId] = action.payload
      // eslint-disable-next-line @typescript-eslint/no-unused-vars
      const { [oldId]: value, ...modalState } = state.modalState
      return {
        modals: [...state.modals.filter((m) => m !== oldId), newId],
        modalState: { ...modalState, ...{ [newId]: 'opened' } },
      }
    }

    case ActionType.Close: {
      return {
        modals: state.modals,
        modalState: { ...state.modalState, ...{ [action.payload]: 'closed' } },
      }
    }

    case ActionType.Remove: {
      // eslint-disable-next-line @typescript-eslint/no-unused-vars
      const { [action.payload]: value, ...modalState } = state.modalState
      return {
        modals: state.modals.filter((m) => m !== action.payload),
        modalState: { ...modalState },
      }
    }

    default: {
      return state
    }
  }
}
