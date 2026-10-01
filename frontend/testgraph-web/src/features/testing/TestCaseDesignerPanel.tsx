import { useState } from 'react'
import {
  Button,
  MenuItem,
  Paper,
  Stack,
  TextField,
  Typography,
  Chip,
} from '@mui/material'

type DraftCase = {
  name: string
  inputs: string
  expected: string
  technique: string
}

const techniques = [
  'Manual',
  'BoundaryValue',
  'EquivalencePartition',
  'BranchCoverage',
  'PathCoverage',
  'InvalidInput',
]

export default function TestCaseDesignerPanel() {
  const [draft, setDraft] = useState<DraftCase>({
    name: '',
    inputs: '',
    expected: '',
    technique: 'Manual',
  })
  const [cases, setCases] = useState<DraftCase[]>([])

  const addCase = () => {
    if (!draft.name.trim() || !draft.expected.trim()) return
    setCases((current) => [...current, draft])
    setDraft({ name: '', inputs: '', expected: '', technique: 'Manual' })
  }

  return (
    <Stack spacing={2}>
      <div>
        <Typography variant="h5" fontWeight={800}>
          Test Case Designer
        </Typography>
        <Typography color="text.secondary">
          Diseña casos de prueba estructurales y clasifícalos por técnica.
        </Typography>
      </div>

      <Paper variant="outlined" sx={{ p: 2.5 }}>
        <Stack spacing={2}>
          <TextField
            label="Nombre"
            value={draft.name}
            onChange={(event) => setDraft({ ...draft, name: event.target.value })}
          />
          <TextField
            label="Entradas"
            placeholder={'edad=20\npromedio=9.5'}
            multiline
            minRows={3}
            value={draft.inputs}
            onChange={(event) => setDraft({ ...draft, inputs: event.target.value })}
          />
          <TextField
            label="Resultado esperado"
            value={draft.expected}
            onChange={(event) => setDraft({ ...draft, expected: event.target.value })}
          />
          <TextField
            select
            label="Técnica"
            value={draft.technique}
            onChange={(event) => setDraft({ ...draft, technique: event.target.value })}
          >
            {techniques.map((technique) => (
              <MenuItem key={technique} value={technique}>
                {technique}
              </MenuItem>
            ))}
          </TextField>
          <Button variant="contained" onClick={addCase}>
            Agregar caso
          </Button>
        </Stack>
      </Paper>

      <Stack spacing={1}>
        {cases.map((testCase, index) => (
          <Paper key={`${testCase.name}-${index}`} variant="outlined" sx={{ p: 2 }}>
            <Stack direction={{ xs: 'column', md: 'row' }} justifyContent="space-between" gap={1}>
              <div>
                <Typography fontWeight={800}>
                  TC-{String(index + 1).padStart(2, '0')} · {testCase.name}
                </Typography>
                <Typography variant="body2" color="text.secondary" sx={{ whiteSpace: 'pre-line' }}>
                  {testCase.inputs || 'Sin entradas documentadas'}
                </Typography>
                <Typography variant="body2" mt={1}>
                  Esperado: {testCase.expected}
                </Typography>
              </div>
              <Chip label={testCase.technique} size="small" />
            </Stack>
          </Paper>
        ))}
      </Stack>
    </Stack>
  )
}
