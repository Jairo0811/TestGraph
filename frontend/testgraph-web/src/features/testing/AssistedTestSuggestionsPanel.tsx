import AutoAwesomeOutlinedIcon from '@mui/icons-material/AutoAwesomeOutlined'
import { Chip, Paper, Stack, Typography } from '@mui/material'

const suggestions = [
  { name: 'edad = 17', rationale: 'Valor inmediatamente inferior a edad > 18' },
  { name: 'edad = 18', rationale: 'Valor exacto de frontera para edad > 18' },
  { name: 'edad = 19', rationale: 'Valor inmediatamente superior a edad > 18' },
  { name: 'promedio = 8.99', rationale: 'Valor inmediatamente inferior a promedio >= 9' },
  { name: 'promedio = 9', rationale: 'Valor exacto de frontera para promedio >= 9' },
  { name: 'promedio = 9.01', rationale: 'Valor inmediatamente superior a promedio >= 9' },
]

export default function AssistedTestSuggestionsPanel() {
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
          Sugerencias determinísticas basadas en los límites detectados en las condiciones TGPL.
        </Typography>
      </div>

      <Stack direction={{ xs: 'column', md: 'row' }} gap={1.5} flexWrap="wrap">
        {suggestions.map((suggestion) => (
          <Paper key={suggestion.name} variant="outlined" sx={{ p: 2, flex: '1 1 300px' }}>
            <Stack spacing={1}>
              <Stack direction="row" justifyContent="space-between" gap={1}>
                <Typography fontWeight={800}>{suggestion.name}</Typography>
                <Chip size="small" label="Boundary Value" color="primary" variant="outlined" />
              </Stack>
              <Typography variant="body2" color="text.secondary">
                {suggestion.rationale}
              </Typography>
            </Stack>
          </Paper>
        ))}
      </Stack>
    </Stack>
  )
}
