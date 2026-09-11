import { Box, Button, Chip, Container, Paper, Stack, Typography } from '@mui/material'

const modules = [
  'Control Flow Graph',
  'Cyclomatic Complexity',
  'Independent Paths',
  'Adjacency Matrix',
  'Test Cases',
]

export default function App() {
  return (
    <Container maxWidth="lg" sx={{ py: 8 }}>
      <Stack spacing={4}>
        <Box>
          <Typography variant="overline" color="primary.main">
            Visual White-Box Testing Analyzer
          </Typography>
          <Typography variant="h2" component="h1" fontWeight={800} gutterBottom>
            TestGraph
          </Typography>
          <Typography variant="h5" color="text.secondary" maxWidth={760}>
            Visualize logic. Discover paths. Design better tests.
          </Typography>
        </Box>

        <Paper variant="outlined" sx={{ p: 4 }}>
          <Stack spacing={3}>
            <Typography variant="h5" fontWeight={700}>
              Solution Foundation
            </Typography>
            <Typography color="text.secondary">
              The project shell is ready for TGPL parsing, control-flow analysis,
              path discovery, graph matrices, and structural test-case design.
            </Typography>
            <Stack direction="row" gap={1} flexWrap="wrap">
              {modules.map((module) => (
                <Chip key={module} label={module} variant="outlined" />
              ))}
            </Stack>
            <Box>
              <Button variant="contained" disabled>
                New Analysis — coming in Phase 2
              </Button>
            </Box>
          </Stack>
        </Paper>
      </Stack>
    </Container>
  )
}
