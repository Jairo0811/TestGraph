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
      <Stack
        direction={{ xs: 'column', md: 'row' }}
        justifyContent="space-between"
        alignItems={{ md: 'flex-end' }}
        gap={2}
      >
        <div>
          <Typography variant="h5" fontWeight={900}>
            Basis Paths
          </Typography>
          <Typography color="text.secondary">
            Selecciona un camino para resaltarlo sobre el Control Flow Graph.
          </Typography>
        </div>
        <Stack direction="row" gap={0.75} flexWrap="wrap" useFlexGap>
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
                p: { xs: 1.5, sm: 2 },
                borderRadius: 2.5,
                borderColor: selected ? 'primary.main' : 'rgba(148, 163, 184, .18)',
                bgcolor: selected ? 'rgba(34, 211, 238, .06)' : undefined,
              }}
            >
              <Stack
                direction={{ xs: 'column', md: 'row' }}
                justifyContent="space-between"
                alignItems={{ md: 'center' }}
                gap={1.25}
              >
                <div>
                  <Typography fontWeight={900}>Path {path.number}</Typography>
                  <Typography variant="body2" color="text.secondary" sx={{ overflowWrap: 'anywhere' }}>
                    {path.display}
                  </Typography>
                </div>
                <Button
                  size="small"
                  variant={selected ? 'contained' : 'outlined'}
                  onClick={() => onSelectPath(selected ? undefined : path.number)}
                  sx={{ width: { xs: '100%', md: 'auto' }, flexShrink: 0 }}
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
