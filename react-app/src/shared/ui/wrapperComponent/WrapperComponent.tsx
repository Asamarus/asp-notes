export interface WrapperComponentProps {
  /** The content of the component */
  children?: React.ReactNode
}
function WrapperComponent({ children }: WrapperComponentProps) {
  return children
}

export default WrapperComponent
