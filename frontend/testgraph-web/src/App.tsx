import AccountTreeOutlinedIcon from '@mui/icons-material/AccountTreeOutlined'
import { Box, Chip, Container, Paper, Stack, Typography } from '@mui/material'
import AnalysisWorkspace from './features/workspace/AnalysisWorkspace'

export default function App() {
  return (
    <Box sx={{ minHeight: '100vh', overflowX: 'hidden' }}>
      <Container
        maxWidth="xl"
        sx={{
          px: { xs: 2, sm: 3, md: 4 },
          py: { xs: 2.5, sm: 4, md: 6 },
        }}
      >
        <Stack spacing={{ xs: 3, md: 4 }}>
          <Paper
            component="header"
            variant="outlined"
            sx={{
              position: 'relative',
              overflow: 'hidden',
              p: { xs: 2.5, sm: 3.5, md: 4.5 },
              borderRadius: { xs: 3, md: 4 },
              borderColor: 'rgba(34, 211, 238, .2)',
              background:
                'linear-gradient(135deg, rgba(12, 28, 49, .96) 0%, rgba(9, 21, 38, .94) 58%, rgba(15, 35, 61, .92) 100%)',
              boxShadow: '0 24px 80px rgba(0, 0, 0, .2)',
              '&::before': {
                content: '""',
                position: 'absolute',
                width: { xs: 220, md: 420 },
                height: { xs: 220, md: 420 },
                borderRadius: '50%',
                right: { xs: -120, md: -130 },
                top: { xs: -130, md: -220 },
                background: 'radial-gradient(circle, rgba(34,211,238,.16), transparent 68%)',
                pointerEvents: 'none',
              },
            }}
          >
            <Stack
              direction={{ xs: 'column', md: 'row' }}
              justifyContent="space-between"
              alignItems={{ xs: 'flex-start', md: 'center' }}
              gap={{ xs: 3, md: 5 }}
              position="relative"
              zIndex={1}
            >
              <Stack spacing={{ xs: 1.25, sm: 1.75 }} sx={{ minWidth: 0, flex: 1 }}>
                <Typography
                  variant="overline"
                  color="primary.main"
                  fontWeight={900}
                  sx={{ letterSpacing: { xs: '.08em', sm: '.12em' } }}
                >
                  Visual White-Box Testing Analyzer
                </Typography>

                <Box
                  component="img"
                  src="/testgraph-logo.png"
                  alt="TestGraph"
                  sx={{
                    display: 'block',
                    width: { xs: 'min(100%, 320px)', sm: 420, md: 520 },
                    maxWidth: '100%',
                    height: 'auto',
                    filter: 'drop-shadow(0 12px 28px rgba(0, 153, 255, .14))',
                  }}
                />

                <Typography
                  variant="h5"
                  color="text.secondary"
                  sx={{
                    maxWidth: 760,
                    fontSize: { xs: '1.05rem', sm: '1.25rem', md: '1.45rem' },
                    lineHeight: 1.45,
                  }}
                >
                  Visualize logic. Discover paths. Design better tests.
                </Typography>
              </Stack>

              <Stack
                spacing={1}
                alignItems={{ xs: 'flex-start', md: 'flex-end' }}
                sx={{ flexShrink: 0 }}
              >
                <Chip
                  icon={<AccountTreeOutlinedIcon />}
                  label="v1.0.1 · Integrated V1"
                  color="primary"
                  variant="outlined"
                  sx={{
                    fontWeight: 800,
                    bgcolor: 'rgba(34, 211, 238, .05)',
                    '& .MuiChip-icon': { color: 'primary.main' },
                  }}
                />
                <Typography variant="caption" color="text.secondary">
                  Deterministic CFG · Complexity · Paths · Tests
                </Typography>
              </Stack>
            </Stack>
          </Paper>

          <AnalysisWorkspace />

          <Box component="footer" sx={{ py: 1, textAlign: 'center' }}>
            <Typography variant="caption" color="text.secondary">
              TestGraph · UNAPEC ISO-300 · v1.0.1
            </Typography>
          </Box>
        </Stack>
      </Container>
    </Box>
  )
}
