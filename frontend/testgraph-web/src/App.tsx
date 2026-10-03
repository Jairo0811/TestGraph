import { Box, Chip, Container, Stack, Typography } from '@mui/material'
import AccountTreeOutlinedIcon from '@mui/icons-material/AccountTreeOutlined'
import AnalysisWorkspace from './features/workspace/AnalysisWorkspace'

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
            label="v1.0.1 · Integrated V1"
            color="primary"
            variant="outlined"
            sx={{ fontWeight: 700 }}
          />
        </Stack>

        <AnalysisWorkspace />
      </Stack>
    </Container>
  )
}
