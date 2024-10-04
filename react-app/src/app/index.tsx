import ReactDOM from 'react-dom/client'

import App from './ui/App.tsx'

//This code is used to prevent 'react-remove-scroll' from blocking scroll wheel event in NoteEditor fullscreen mode
//https://github.com/theKashey/react-remove-scroll

// Function to stop propagation of the "wheel" event
const stopWheelPropagation = (event: WheelEvent) => {
  event.stopPropagation()
}

// Add the event listener with high priority
document.addEventListener('wheel', stopWheelPropagation, { capture: true })

ReactDOM.createRoot(document.getElementById('root')!).render(<App />)
