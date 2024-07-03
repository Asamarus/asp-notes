export interface State {
  initialData: InitialData | null
}

export interface Context extends State {}

export interface InitialData {
  title: string
  someData: string
}
