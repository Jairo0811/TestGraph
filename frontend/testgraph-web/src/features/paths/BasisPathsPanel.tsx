import { Chip, Paper, Stack, Typography } from '@mui/material'
import type { PathsResultDto } from '../../api/types'

export default function BasisPathsPanel({ result }: { result: PathsResultDto }) {
  return (
    <Stack spacing={2}>
      <Stack direction={{ xs: 'column', md: 'row' }} justifyContent="space-between" gap={2}>
        <div>
          <Typography variant="h5" fontWeight={800}>Basis Paths</Typography>
          <Typography color="text.secondary">
            Conjunto determinístico de caminos linealmente independientes.
          </Typography>
        </div>
        <Stack direction="row" gap={1}>
          <Chip label={`${result.pathCount} paths`} color="primary" variant="outlined" />
          <Chip label={`V(G) ${result.cyclomaticComplexity}`} />
          <Chip label={result.complete ? 'Completo' : 'Incompleto'} color={result.complete ? 'success' : 'warning'} />
        </Stack>
      </Stack>

      <Stack spacing={1}>
        {result.items.map((path) => (
          <Paper key={path.number} variant="outlined" sx={{ p: 2 }}>
            <Stack direction={{ xs: 'column', md: 'row' }} justifyContent="space-between" gap={1}>
              <Typography fontWeight={800}>Path {path.number}</Typography>
              <Typography variant="body2" color="text.secondary" sx={{ wordBreak: 'break-word' }}>
                {path.display}
              </Typography>
            </Stack>
          </Paper>
        ))}
      </Stack>
    </Stack>
  )
}
