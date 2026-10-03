import { Button, Chip, Paper, Stack, Typography } from '@mui/material'
import type { PathsResultDto } from '../../api/types'

export interface BasisPathsPanelProps {
  result: PathsResultDto
  selectedPathNumber?: number
  onSelectPath: (pathNumber?: number) => void
}

export default function BasisPathsPanel({ result, selectedPathNumber, onSelectPath }: BasisPathsPanelProps) {
  return (
    <Stack spacing={2}>
      <Stack direction={{ xs: 'column', md: 'row' }} justifyContent="space-between" gap={2}>
        <div>
          <Typography variant="h5" fontWeight={800}>Basis Paths</Typography>
          <Typography color="text.secondary">
            Selecciona un camino para resaltarlo sobre el Control Flow Graph.
          </Typography>
        </div>
        <Stack direction="row" gap={1}>
          <Chip label={`${result.pathCount} paths`} color="primary" variant="outlined" />
          <Chip label={`V(G) ${result.cyclomaticComplexity}`} />
          <Chip label={result.complete ? 'Completo' : 'Incompleto'} color={result.complete ? 'success' : 'warning'} />
        </Stack>
      </Stack>

      <Stack spacing={1}>
        {result.items.map((path) => {
          const selected = selectedPathNumber === path.number
          return (
            <Paper
              key={path.number}
              variant="outlined"
              sx={{
                p: 2,
                borderColor: selected ? 'primary.main' : undefined,
                bgcolor: selected ? 'rgba(34, 211, 238, .06)' : undefined,
              }}
            >
              <Stack direction={{ xs: 'column', md: 'row' }} justifyContent="space-between" alignItems={{ md: 'center' }} gap={1}>
                <div>
                  <Typography fontWeight={800}>Path {path.number}</Typography>
                  <Typography variant="body2" color="text.secondary" sx={{ wordBreak: 'break-word' }}>
                    {path.display}
                  </Typography>
                </div>
                <Button
                  size="small"
                  variant={selected ? 'contained' : 'outlined'}
                  onClick={() => onSelectPath(selected ? undefined : path.number)}
                >
                  {selected ? 'Quitar resaltado' : 'Resaltar'}
                </Button>
              </Stack>
            </Paper>
          )
        })}
      </Stack>
    </Stack>
  )
}
