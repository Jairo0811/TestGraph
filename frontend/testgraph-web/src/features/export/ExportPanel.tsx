import { useState } from 'react'
import { Alert, Button, Paper, Stack, Typography } from '@mui/material'
import DownloadOutlinedIcon from '@mui/icons-material/DownloadOutlined'
import ImageOutlinedIcon from '@mui/icons-material/ImageOutlined'
import { toPng } from 'html-to-image'

async function downloadFromApi(path: string, filename: string, sourceCode: string) {
  const response = await fetch(path, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ sourceCode }),
  })

  if (!response.ok) {
    let message = `${response.status} ${response.statusText}`
    try {
      const payload = (await response.json()) as { error?: string }
      if (payload.error) message = payload.error
    } catch {
      // Keep HTTP status.
    }
    throw new Error(message)
  }

  const blob = await response.blob()
  const url = URL.createObjectURL(blob)
  const anchor = document.createElement('a')
  anchor.href = url
  anchor.download = filename
  anchor.click()
  URL.revokeObjectURL(url)
}

export default function ExportPanel({ sourceCode }: { sourceCode: string }) {
  const [error, setError] = useState<string>()

  const runDownload = async (path: string, filename: string) => {
    setError(undefined)
    try {
      await downloadFromApi(path, filename, sourceCode)
    } catch (exception) {
      setError(exception instanceof Error ? exception.message : 'No se pudo completar la exportación.')
    }
  }

  const exportPng = async () => {
    setError(undefined)
    try {
      const element = document.querySelector<HTMLElement>('[data-testid="cfg-canvas"]')
      if (!element) throw new Error('El CFG todavía no está visible.')

      const dataUrl = await toPng(element, {
        backgroundColor: '#07111f',
        pixelRatio: 2,
      })

      const anchor = document.createElement('a')
      anchor.href = dataUrl
      anchor.download = 'testgraph-cfg.png'
      anchor.click()
    } catch (exception) {
      setError(exception instanceof Error ? exception.message : 'No se pudo exportar el CFG.')
    }
  }

  return (
    <Paper variant="outlined" sx={{ p: 2.5 }}>
      <Stack spacing={2}>
        <div>
          <Typography variant="h5" fontWeight={800}>
            Exports & Reports
          </Typography>
          <Typography color="text.secondary">
            Los formatos de datos se generan desde el mismo pipeline determinístico del backend.
          </Typography>
        </div>

        <Stack direction={{ xs: 'column', sm: 'row' }} gap={1} flexWrap="wrap">
          <Button startIcon={<DownloadOutlinedIcon />} variant="outlined" onClick={() => runDownload('/api/analysis/export/json', 'testgraph-analysis.json')}>
            JSON
          </Button>
          <Button startIcon={<DownloadOutlinedIcon />} variant="outlined" onClick={() => runDownload('/api/analysis/export/matrix.csv', 'testgraph-matrix.csv')}>
            CSV Matrix
          </Button>
          <Button startIcon={<DownloadOutlinedIcon />} variant="outlined" onClick={() => runDownload('/api/analysis/report/markdown', 'testgraph-report.md')}>
            Markdown Report
          </Button>
          <Button startIcon={<ImageOutlinedIcon />} variant="contained" onClick={exportPng}>
            CFG PNG
          </Button>
        </Stack>

        {error ? <Alert severity="error">{error}</Alert> : null}
      </Stack>
    </Paper>
  )
}
