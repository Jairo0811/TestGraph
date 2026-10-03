import { Chip, Paper, Stack, Typography } from '@mui/material'
import type { ComplexityResultDto } from '../../api/types'

export default function ComplexityPanel({ result }: { result: ComplexityResultDto }) {
  return (
    <Paper
      variant="outlined"
      sx={{
        p: { xs: 2, sm: 2.5 },
        borderRadius: { xs: 2.5, md: 3 },
        borderColor: 'rgba(148, 163, 184, .18)',
      }}
    >
      <Stack spacing={2}>
        <Stack
          direction={{ xs: 'column', md: 'row' }}
          justifyContent="space-between"
          alignItems={{ md: 'center' }}
          gap={2}
        >
          <div>
            <Typography variant="h5" fontWeight={900}>
              Cyclomatic Complexity
            </Typography>
            <Typography color="text.secondary">
              Las tres formulaciones estructurales se calculan desde el CFG real.
            </Typography>
          </div>
          <Stack direction="row" gap={0.75} flexWrap="wrap" useFlexGap>
            <Chip label={`V(G) = ${result.value}`} color="primary" sx={{ fontWeight: 900 }} />
            <Chip label={result.level} variant="outlined" />
          </Stack>
        </Stack>

        <Stack direction="row" gap={1.5} flexWrap="wrap" useFlexGap>
          <Paper
            variant="outlined"
            sx={{ p: 2, flex: '1 1 180px', minWidth: 0, bgcolor: 'rgba(34, 211, 238, .025)' }}
          >
            <Typography variant="overline" color="text.secondary">
              E - N + 2
            </Typography>
            <Typography variant="h5" fontWeight={900} color="primary.main">
              {result.formulas.edgeNode}
            </Typography>
            <Typography variant="body2" color="text.secondary">
              {result.edges} aristas · {result.nodes} nodos
            </Typography>
          </Paper>
          <Paper
            variant="outlined"
            sx={{ p: 2, flex: '1 1 180px', minWidth: 0, bgcolor: 'rgba(59, 130, 246, .025)' }}
          >
            <Typography variant="overline" color="text.secondary">
              P + 1
            </Typography>
            <Typography variant="h5" fontWeight={900} color="primary.main">
              {result.formulas.predicate}
            </Typography>
            <Typography variant="body2" color="text.secondary">
              {result.predicates} nodos predicado
            </Typography>
          </Paper>
          <Paper
            variant="outlined"
            sx={{ p: 2, flex: '1 1 180px', minWidth: 0, bgcolor: 'rgba(139, 92, 246, .025)' }}
          >
            <Typography variant="overline" color="text.secondary">
              Regiones
            </Typography>
            <Typography variant="h5" fontWeight={900} color="primary.main">
              {result.formulas.region}
            </Typography>
            <Typography variant="body2" color="text.secondary">
              {result.formulas.agree ? 'Las formulaciones coinciden' : 'Revisar consistencia del grafo'}
            </Typography>
          </Paper>
        </Stack>
      </Stack>
    </Paper>
  )
}
