import AutoAwesomeOutlinedIcon from '@mui/icons-material/AutoAwesomeOutlined'
import { Chip, Paper, Stack, Typography } from '@mui/material'
import type { AssistedSuggestionsDto } from '../../api/types'

export default function AssistedTestSuggestionsPanel({ result }: { result: AssistedSuggestionsDto }) {
  return (
    <Stack spacing={2}>
      <div>
        <Stack direction="row" spacing={1} alignItems="center">
          <AutoAwesomeOutlinedIcon color="primary" />
          <Typography variant="h5" fontWeight={800}>
            Assisted Test Generation
          </Typography>
        </Stack>
        <Typography color="text.secondary">
          Sugerencias determinísticas generadas desde los límites detectados en el AST TGPL.
        </Typography>
      </div>

      {result.testCases.length === 0 ? (
        <Paper variant="outlined" sx={{ p: 2 }}>
          <Typography color="text.secondary">
            No se detectaron comparaciones numéricas con valores de frontera sugeribles.
          </Typography>
        </Paper>
      ) : (
        <Stack direction={{ xs: 'column', md: 'row' }} gap={1.5} flexWrap="wrap">
          {result.testCases.map((suggestion) => (
            <Paper key={`${suggestion.number}-${suggestion.name}`} variant="outlined" sx={{ p: 2, flex: '1 1 300px' }}>
              <Stack spacing={1}>
                <Stack direction="row" justifyContent="space-between" gap={1}>
                  <Typography fontWeight={800}>{suggestion.name}</Typography>
                  <Chip size="small" label={suggestion.technique} color="primary" variant="outlined" />
                </Stack>
                <Typography variant="body2" color="text.secondary">
                  {suggestion.rationale}
                </Typography>
                {suggestion.sourceLine ? (
                  <Typography variant="caption" color="text.secondary">
                    Línea {suggestion.sourceLine}
                  </Typography>
                ) : null}
              </Stack>
            </Paper>
          ))}
        </Stack>
      )}
    </Stack>
  )
}
