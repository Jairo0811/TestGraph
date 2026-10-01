import { Chip, Paper, Stack, Typography } from '@mui/material'

const samples = [
  ['Matrix Minimum Even', 'Angel Emmanuel Gonzalez Acosta', false],
  ['Scholarship Calculator', 'Francis Jairo Matias Rosario', true],
  ['Numbers Ending in Four', 'Robinson Junior Novo Lopez', true],
  ['Find Number 24', 'Christian Rainel Menendez Hiciano', false],
  ['Discount Calculator', 'Diego Jose Montero Almonte', true],
] as const

export default function AcademicSamplesPanel() {
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
        {samples.map(([name, author, ready]) => (
          <Paper key={name} variant="outlined" sx={{ p: 2, flex: '1 1 300px' }}>
            <Stack spacing={1}>
              <Typography fontWeight={800}>{name}</Typography>
              <Typography variant="body2" color="text.secondary">
                {author}
              </Typography>
              <Chip
                size="small"
                label={ready ? 'TGPL V1 ready' : 'Preserved · matrix syntax pending'}
                color={ready ? 'success' : 'default'}
                variant="outlined"
                sx={{ alignSelf: 'flex-start' }}
              />
            </Stack>
          </Paper>
        ))}
      </Stack>
    </Stack>
  )
}
