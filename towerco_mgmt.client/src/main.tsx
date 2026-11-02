import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import App from '@/app/App'
import PersonaProvider from '@/app/PersonaProvider'

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <PersonaProvider>
      <App />
    </PersonaProvider>
  </StrictMode>,
)
