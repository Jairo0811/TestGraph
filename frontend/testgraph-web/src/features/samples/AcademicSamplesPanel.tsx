import { Button, Chip, Paper, Stack, Typography } from '@mui/material'
import type { AcademicSampleSummaryDto } from '../../api/types'

export interface AcademicSamplesPanelProps {
  samples: AcademicSampleSummaryDto[]
  loadingId?: string
  onLoad: (id: string) => void
}

export default function AcademicSamplesPanel({ samples, loadingId, onLoad }: AcademicSamplesPanelProps) {
  return (
    <Stack spacing={2}>
      <div>
        <Typography variant="h5" fontWeight={800}>
          Academic Samples
        </Typography>
        <Typography color="text.secondary">
          Los cinco ejercicios originales de ISO-300 se preservan como referencia y escenarios de validación.
        </Typography>
      </div>

      <Stack direction={{ xs: 'column', md: 'row' }} gap={1.5} flexWrap="wrap">
        {samples.map((sample) => (
          <Paper key={sample.id} variant="outlined" sx={{ p: 2, flex: '1 1 300px' }}>
            <Stack spacing={1}>
              <Typography fontWeight={800}>{sample.name}</Typography>
              <Typography variant="body2" color="text.secondary">
                {sample.originalAuthor}
              </Typography>
              <Typography variant="body2" color="text.secondary">
                {sample.description}
              </Typography>
              <Stack direction="row" gap={1} flexWrap="wrap">
                <Chip
                  size="small"
                  label={sample.analyzerReady ? 'TGPL V1 ready' : 'Preserved · syntax pending'}
                  color={sample.analyzerReady ? 'success' : 'default'}
                  variant="outlined"
                />
                {sample.expectedCyclomaticComplexity ? (
                  <Chip size="small" label={`V(G) ${sample.expectedCyclomaticComplexity}`} variant="outlined" />
                ) : null}
              </Stack>
              {sample.limitation ? (
                <Typography variant="caption" color="text.secondary">
                  {sample.limitation}
                </Typography>
              ) : null}
              <Button
                size="small"
                variant="outlined"
                disabled={!sample.analyzerReady || loadingId === sample.id}
                onClick={() => onLoad(sample.id)}
              >
                {loadingId === sample.id ? 'Cargando…' : 'Cargar en editor'}
              </Button>
            </Stack>
          </Paper>
        ))}
      </Stack>
    </Stack>
  )
}
