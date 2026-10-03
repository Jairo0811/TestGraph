import { Chip, Paper, Stack, Typography } from '@mui/material'
import type { ComplexityResultDto } from '../../api/types'

export default function ComplexityPanel({ result }: { result: ComplexityResultDto }) {
  return (
    <Paper variant="outlined" sx={{ p: 2.5 }}>
      <Stack spacing={2}>
        <Stack direction={{ xs: 'column', md: 'row' }} justifyContent="space-between" gap={2}>
          <div>
            <Typography variant="h5" fontWeight={800}>Cyclomatic Complexity</Typography>
            <Typography color="text.secondary">
              Las tres formulaciones estructurales se calculan desde el CFG real.
            </Typography>
          </div>
          <Stack direction="row" gap={1} alignItems="center">
            <Chip label={`V(G) = ${result.value}`} color="primary" sx={{ fontWeight: 800 }} />
            <Chip label={result.level} variant="outlined" />
          </Stack>
        </Stack>

        <Stack direction={{ xs: 'column', md: 'row' }} gap={1.5}>
          <Paper variant="outlined" sx={{ p: 2, flex: 1 }}>
            <Typography variant="overline" color="text.secondary">E - N + 2</Typography>
            <Typography variant="h5" fontWeight={800}>{result.formulas.edgeNode}</Typography>
            <Typography variant="body2" color="text.secondary">
              {result.edges} aristas · {result.nodes} nodos
            </Typography>
          </Paper>
          <Paper variant="outlined" sx={{ p: 2, flex: 1 }}>
            <Typography variant="overline" color="text.secondary">P + 1</Typography>
            <Typography variant="h5" fontWeight={800}>{result.formulas.predicate}</Typography>
            <Typography variant="body2" color="text.secondary">
              {result.predicates} nodos predicado
            </Typography>
          </Paper>
          <Paper variant="outlined" sx={{ p: 2, flex: 1 }}>
            <Typography variant="overline" color="text.secondary">Regiones</Typography>
            <Typography variant="h5" fontWeight={800}>{result.formulas.region}</Typography>
            <Typography variant="body2" color="text.secondary">
              {result.formulas.agree ? 'Las formulaciones coinciden' : 'Revisar consistencia del grafo'}
            </Typography>
          </Paper>
        </Stack>
      </Stack>
    </Paper>
  )
}
