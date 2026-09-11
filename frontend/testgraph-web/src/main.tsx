import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { CssBaseline, ThemeProvider, createTheme } from '@mui/material'
import App from './App'

const queryClient = new QueryClient()

const theme = createTheme({
  palette: {
    mode: 'dark',
    primary: { main: '#22d3ee' },
    secondary: { main: '#3b82f6' },
    background: {
      default: '#08111f',
      paper: '#101b2d',
    },
  },
  shape: { borderRadius: 12 },
})

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <QueryClientProvider client={queryClient}>
      <ThemeProvider theme={theme}>
        <CssBaseline />
        <App />
      </ThemeProvider>
    </QueryClientProvider>
  </StrictMode>,
)
