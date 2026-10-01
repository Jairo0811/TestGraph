import { Box, Chip, Container, Paper, Stack, Typography } from '@mui/material'
import AccountTreeOutlinedIcon from '@mui/icons-material/AccountTreeOutlined'
import ControlFlowGraphViewer from './features/control-flow/ControlFlowGraphViewer'
import { scholarshipGraph } from './features/control-flow/sampleGraph'
import AdjacencyMatrixPanel from './features/matrix/AdjacencyMatrixPanel'
import TestCaseDesignerPanel from './features/testing/TestCaseDesignerPanel'

const modules = [
  'TGPL Lexer',
  'TGPL Parser + AST',
  'Control Flow Graph Engine',
  'CFG Visualization',
]

export default function App() {
  return (
    <Container maxWidth="xl" sx={{ py: { xs: 4, md: 6 } }}>
      <Stack spacing={4}>
        <Stack
          direction={{ xs: 'column', md: 'row' }}
          justifyContent="space-between"
          alignItems={{ md: 'flex-end' }}
          gap={2}
        >
          <Box>
            <Typography variant="overline" color="primary.main" fontWeight={800}>
              Visual White-Box Testing Analyzer
            </Typography>
            <Typography variant="h2" component="h1" fontWeight={900} gutterBottom>
              TestGraph
            </Typography>
            <Typography variant="h5" color="text.secondary" maxWidth={760}>
              Visualize logic. Discover paths. Design better tests.
            </Typography>
          </Box>

          <Chip
            icon={<AccountTreeOutlinedIcon />}
            label="Phase 5 · CFG Visualization"
            color="primary"
            variant="outlined"
            sx={{ fontWeight: 700 }}
          />
        </Stack>

        <Paper
          variant="outlined"
          sx={{
            p: { xs: 2, md: 3 },
            background:
              'linear-gradient(145deg, rgba(14, 30, 52, .95), rgba(8, 17, 31, .95))',
          }}
        >
          <Stack spacing={2}>
            <Typography variant="h6" fontWeight={800}>
              Scholarship Calculator · Academic Sample
            </Typography>
            <Typography color="text.secondary" maxWidth={900}>
              El flujo de control ya puede explorarse visualmente. El grafo usa IDs
              deterministas, tipos semánticos de nodos y aristas, y conserva la
              relación con las líneas del pseudocódigo.
            </Typography>
            <Stack direction="row" gap={1} flexWrap="wrap">
              {modules.map((module) => (
                <Chip key={module} label={module} size="small" variant="outlined" />
              ))}
            </Stack>
          </Stack>
        </Paper>

        <ControlFlowGraphViewer graph={scholarshipGraph} />
        <AdjacencyMatrixPanel graph={scholarshipGraph} />
        <TestCaseDesignerPanel />
      </Stack>
    </Container>
  )
}
