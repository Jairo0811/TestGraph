import { Box, Button, Chip, Paper, Stack, Typography } from '@mui/material'
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
        <Typography variant="h5" fontWeight={900}>
          Academic Samples
        </Typography>
        <Typography color="text.secondary">
          Los cinco ejercicios originales de ISO-300 se preservan como referencia y escenarios de validación.
        </Typography>
      </div>

      <Box
        sx={{
          display: 'grid',
          gridTemplateColumns: {
            xs: '1fr',
            sm: 'repeat(2, minmax(0, 1fr))',
            lg: 'repeat(3, minmax(0, 1fr))',
          },
          gap: 1.5,
        }}
      >
        {samples.map((sample) => (
          <Paper
            key={sample.id}
            variant="outlined"
            sx={{
              p: 2,
              minWidth: 0,
              height: '100%',
              borderRadius: 2.5,
              borderColor: 'rgba(148, 163, 184, .18)',
              transition: 'transform .18s ease, border-color .18s ease, background-color .18s ease',
              '&:hover': {
                transform: 'translateY(-2px)',
                borderColor: 'rgba(34, 211, 238, .3)',
                bgcolor: 'rgba(34, 211, 238, .025)',
              },
            }}
          >
            <Stack spacing={1} height="100%">
              <Typography fontWeight={900}>{sample.name}</Typography>
              <Typography variant="body2" color="text.secondary">
                {sample.originalAuthor}
              </Typography>
              <Typography variant="body2" color="text.secondary" sx={{ flexGrow: 1 }}>
                {sample.description}
              </Typography>
              <Stack direction="row" gap={0.75} flexWrap="wrap" useFlexGap>
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
                fullWidth
              >
                {loadingId === sample.id ? 'Cargando…' : 'Cargar en editor'}
              </Button>
            </Stack>
          </Paper>
        ))}
      </Box>
    </Stack>
  )
}
