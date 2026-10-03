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
import { useMutation } from '@tanstack/react-query'
import { designTestCases } from '../../api/client'
import type { BasisPathDto, TestCaseDraftDto } from '../../api/types'

type DraftForm = {
  name: string
  inputs: string
  expectedResult: string
  technique: string
  linkedPathNumber: string
}

const techniques = [
  'Manual',
  'BoundaryValue',
  'EquivalencePartition',
  'BranchCoverage',
  'PathCoverage',
  'InvalidInput',
]

function parseInputs(raw: string) {
  const inputs: Record<string, string> = {}
  raw.split(/\r?\n/).forEach((line) => {
    const index = line.indexOf('=')
    if (index <= 0) return
    const key = line.slice(0, index).trim()
    const value = line.slice(index + 1).trim()
    if (key) inputs[key] = value
  })
  return inputs
}

export interface TestCaseDesignerPanelProps {
  sourceCode: string
  paths: BasisPathDto[]
}

export default function TestCaseDesignerPanel({ sourceCode, paths }: TestCaseDesignerPanelProps) {
  const [draft, setDraft] = useState<DraftForm>({
    name: '',
    inputs: '',
    expectedResult: '',
    technique: 'Manual',
    linkedPathNumber: '',
  })
  const [drafts, setDrafts] = useState<TestCaseDraftDto[]>([])

  const designMutation = useMutation({
    mutationFn: () => designTestCases(sourceCode, drafts),
  })

  const addCase = () => {
    if (!draft.name.trim() || !draft.expectedResult.trim()) return

    setDrafts((current) => [
      ...current,
      {
        name: draft.name.trim(),
        inputs: parseInputs(draft.inputs),
        expectedResult: draft.expectedResult.trim(),
        technique: draft.technique,
        linkedPathNumber: draft.linkedPathNumber ? Number(draft.linkedPathNumber) : undefined,
      },
    ])

    setDraft({
      name: '',
      inputs: '',
      expectedResult: '',
      technique: 'Manual',
      linkedPathNumber: '',
    })
    designMutation.reset()
  }

  return (
    <Stack spacing={2}>
      <div>
        <Typography variant="h5" fontWeight={800}>
          Test Case Designer
        </Typography>
        <Typography color="text.secondary">
          Diseña casos estructurales y vincúlalos opcionalmente a un basis path real.
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
            value={draft.expectedResult}
            onChange={(event) => setDraft({ ...draft, expectedResult: event.target.value })}
          />
          <Stack direction={{ xs: 'column', md: 'row' }} gap={1.5}>
            <TextField
              select
              label="Técnica"
              value={draft.technique}
              onChange={(event) => setDraft({ ...draft, technique: event.target.value })}
              sx={{ flex: 1 }}
            >
              {techniques.map((technique) => (
                <MenuItem key={technique} value={technique}>
                  {technique}
                </MenuItem>
              ))}
            </TextField>
            <TextField
              select
              label="Basis path"
              value={draft.linkedPathNumber}
              onChange={(event) => setDraft({ ...draft, linkedPathNumber: event.target.value })}
              sx={{ flex: 1 }}
            >
              <MenuItem value="">Sin vínculo</MenuItem>
              {paths.map((path) => (
                <MenuItem key={path.number} value={String(path.number)}>
                  Path {path.number} · {path.display}
                </MenuItem>
              ))}
            </TextField>
          </Stack>
          <Button variant="contained" onClick={addCase}>
            Agregar caso
          </Button>
        </Stack>
      </Paper>

      <Stack spacing={1}>
        {drafts.map((testCase, index) => (
          <Paper key={`${testCase.name}-${index}`} variant="outlined" sx={{ p: 2 }}>
            <Stack direction={{ xs: 'column', md: 'row' }} justifyContent="space-between" gap={1}>
              <div>
                <Typography fontWeight={800}>
                  TC-{String(index + 1).padStart(2, '0')} · {testCase.name}
                </Typography>
                <Typography variant="body2" color="text.secondary">
                  {Object.entries(testCase.inputs).map(([key, value]) => `${key}=${value}`).join(' · ') || 'Sin entradas documentadas'}
                </Typography>
                <Typography variant="body2" mt={1}>
                  Esperado: {testCase.expectedResult}
                </Typography>
              </div>
              <Stack direction="row" gap={1} alignItems="flex-start">
                <Chip label={testCase.technique} size="small" />
                {testCase.linkedPathNumber ? <Chip label={`Path ${testCase.linkedPathNumber}`} size="small" variant="outlined" /> : null}
              </Stack>
            </Stack>
          </Paper>
        ))}
      </Stack>

      {drafts.length > 0 ? (
        <Button variant="outlined" disabled={designMutation.isPending} onClick={() => designMutation.mutate()}>
          {designMutation.isPending ? 'Validando…' : 'Validar casos contra el CFG'}
        </Button>
      ) : null}

      {designMutation.error ? <Alert severity="error">{designMutation.error.message}</Alert> : null}
      {designMutation.data ? (
        <Alert severity={designMutation.data.isValid ? 'success' : 'warning'}>
          {designMutation.data.isValid
            ? `${designMutation.data.testCases.length} caso(s) validados estructuralmente.`
            : designMutation.data.diagnostics.map((diagnostic) => diagnostic.message).join(' · ')}
        </Alert>
      ) : null}
    </Stack>
  )
}
