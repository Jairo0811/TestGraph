import { useState } from 'react'
import {
  Alert,
  Button,
  Chip,
  MenuItem,
  Paper,
  Stack,
  TextField,
  Typography,
} from '@mui/material'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import {
  clearStoredToken,
  createProject,
  getMe,
  getProjects,
  getStoredToken,
  login,
  register,
  saveAnalysis,
} from '../../api/client'
import type { TestCaseDraftDto } from '../../api/types'

export interface AccountProjectsPanelProps {
  sourceCode: string
  testCases: TestCaseDraftDto[]
}

export default function AccountProjectsPanel({ sourceCode, testCases }: AccountProjectsPanelProps) {
  const queryClient = useQueryClient()
  const [authenticated, setAuthenticated] = useState(Boolean(getStoredToken()))
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [projectName, setProjectName] = useState('')
  const [projectDescription, setProjectDescription] = useState('')
  const [projectId, setProjectId] = useState('')

  const meQuery = useQuery({
    queryKey: ['me'],
    queryFn: getMe,
    enabled: authenticated,
    retry: false,
  })

  const projectsQuery = useQuery({
    queryKey: ['projects'],
    queryFn: getProjects,
    enabled: authenticated,
    retry: false,
  })

  const authMutation = useMutation({
    mutationFn: ({ mode }: { mode: 'login' | 'register' }) =>
      mode === 'login' ? login(email, password) : register(email, password),
    onSuccess: async () => {
      setAuthenticated(true)
      setPassword('')
      await queryClient.invalidateQueries({ queryKey: ['me'] })
      await queryClient.invalidateQueries({ queryKey: ['projects'] })
    },
  })

  const createMutation = useMutation({
    mutationFn: () => createProject(projectName, projectDescription || undefined),
    onSuccess: async (project) => {
      setProjectId(project.id)
      setProjectName('')
      setProjectDescription('')
      await queryClient.invalidateQueries({ queryKey: ['projects'] })
    },
  })

  const saveMutation = useMutation({
    mutationFn: () => saveAnalysis(projectId, sourceCode, testCases),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['projects'] }),
  })

  const logout = () => {
    clearStoredToken()
    setAuthenticated(false)
    setProjectId('')
    queryClient.removeQueries({ queryKey: ['me'] })
    queryClient.removeQueries({ queryKey: ['projects'] })
  }

  return (
    <Paper variant="outlined" sx={{ p: 2.5 }}>
      <Stack spacing={2}>
        <div>
          <Typography variant="h5" fontWeight={800}>Account & Projects</Typography>
          <Typography color="text.secondary">
            El análisis funciona como invitado. Inicia sesión solo si deseas guardar proyectos e historial.
          </Typography>
        </div>

        {!authenticated ? (
          <Stack spacing={1.5}>
            <TextField label="Email" type="email" value={email} onChange={(event) => setEmail(event.target.value)} />
            <TextField label="Contraseña" type="password" value={password} onChange={(event) => setPassword(event.target.value)} />
            <Stack direction={{ xs: 'column', sm: 'row' }} gap={1}>
              <Button variant="contained" disabled={authMutation.isPending} onClick={() => authMutation.mutate({ mode: 'login' })}>
                Iniciar sesión
              </Button>
              <Button variant="outlined" disabled={authMutation.isPending} onClick={() => authMutation.mutate({ mode: 'register' })}>
                Crear cuenta
              </Button>
            </Stack>
            <Typography variant="caption" color="text.secondary">
              La contraseña debe tener al menos 8 caracteres.
            </Typography>
            {authMutation.error ? <Alert severity="error">{authMutation.error.message}</Alert> : null}
          </Stack>
        ) : (
          <Stack spacing={2}>
            <Stack direction={{ xs: 'column', sm: 'row' }} justifyContent="space-between" gap={1}>
              <Typography>
                Sesión: <strong>{meQuery.data?.email ?? 'cuenta autenticada'}</strong>
              </Typography>
              <Button size="small" onClick={logout}>Cerrar sesión</Button>
            </Stack>

            <Stack direction={{ xs: 'column', md: 'row' }} gap={1.5}>
              <TextField label="Nuevo proyecto" value={projectName} onChange={(event) => setProjectName(event.target.value)} sx={{ flex: 1 }} />
              <TextField label="Descripción" value={projectDescription} onChange={(event) => setProjectDescription(event.target.value)} sx={{ flex: 2 }} />
              <Button variant="outlined" disabled={!projectName.trim() || createMutation.isPending} onClick={() => createMutation.mutate()}>
                Crear
              </Button>
            </Stack>

            <Stack direction={{ xs: 'column', md: 'row' }} gap={1.5}>
              <TextField
                select
                label="Proyecto"
                value={projectId}
                onChange={(event) => setProjectId(event.target.value)}
                sx={{ flex: 1 }}
              >
                {(projectsQuery.data ?? []).map((project) => (
                  <MenuItem key={project.id} value={project.id}>
                    {project.name} · {project.analysisCount} análisis
                  </MenuItem>
                ))}
              </TextField>
              <Button variant="contained" disabled={!projectId || !sourceCode.trim() || saveMutation.isPending} onClick={() => saveMutation.mutate()}>
                Guardar análisis actual
              </Button>
            </Stack>

            <Stack direction="row" gap={1} flexWrap="wrap">
              <Chip size="small" variant="outlined" label={`${testCases.length} caso(s) de prueba incluidos`} />
            </Stack>

            {createMutation.error ? <Alert severity="error">{createMutation.error.message}</Alert> : null}
            {saveMutation.error ? <Alert severity="error">{saveMutation.error.message}</Alert> : null}
            {saveMutation.isSuccess ? <Alert severity="success">Análisis y casos de prueba guardados correctamente.</Alert> : null}
            {meQuery.error || projectsQuery.error ? (
              <Alert severity="warning">La sesión ya no es válida o la base de datos no está disponible. Cierra sesión e inténtalo de nuevo.</Alert>
            ) : null}
          </Stack>
        )}
      </Stack>
    </Paper>
  )
}
