import { Button, Paper, Stack, Typography } from '@mui/material'
import DownloadOutlinedIcon from '@mui/icons-material/DownloadOutlined'
import ImageOutlinedIcon from '@mui/icons-material/ImageOutlined'
import { toPng } from 'html-to-image'
import type { ControlFlowGraphDto } from '../control-flow/types'

export interface ExportPanelProps {
  graph: ControlFlowGraphDto
}

function downloadText(filename: string, mime: string, content: string) {
  const blob = new Blob([content], { type: mime })
  const url = URL.createObjectURL(blob)
  const anchor = document.createElement('a')
  anchor.href = url
  anchor.download = filename
  anchor.click()
  URL.revokeObjectURL(url)
}

export default function ExportPanel({ graph }: ExportPanelProps) {
  const exportJson = () => {
    downloadText('testgraph-analysis.json', 'application/json', JSON.stringify(graph, null, 2))
  }

  const exportCsv = () => {
    const ids = [...graph.nodes].map((node) => node.id).sort((a, b) => a - b)
    const index = new Map(ids.map((id, position) => [id, position]))
    const matrix = ids.map(() => ids.map(() => 0))

    graph.edges.forEach((edge) => {
      const row = index.get(edge.sourceId)
      const column = index.get(edge.targetId)
      if (row !== undefined && column !== undefined) {
        matrix[row][column] += 1
      }
    })

    const csvRows = [',' + ids.join(',')]
    matrix.forEach((row, rowIndex) => {
      csvRows.push(String(ids[rowIndex]) + ',' + row.join(','))
    })

    downloadText('testgraph-matrix.csv', 'text/csv', csvRows.join('\n'))
  }

  const exportPng = async () => {
    const element = document.querySelector<HTMLElement>('[data-testid="cfg-canvas"]')
    if (!element) return

    const dataUrl = await toPng(element, {
      backgroundColor: '#07111f',
      pixelRatio: 2,
    })

    const anchor = document.createElement('a')
    anchor.href = dataUrl
    anchor.download = 'testgraph-cfg.png'
    anchor.click()
  }

  return (
    <Paper variant="outlined" sx={{ p: 2.5 }}>
      <Stack spacing={2}>
        <div>
          <Typography variant="h5" fontWeight={800}>
            Exports & Reports
          </Typography>
          <Typography color="text.secondary">
            Exporta el análisis estructural en formatos portables.
          </Typography>
        </div>

        <Stack direction={{ xs: 'column', sm: 'row' }} gap={1}>
          <Button startIcon={<DownloadOutlinedIcon />} variant="outlined" onClick={exportJson}>
            JSON
          </Button>
          <Button startIcon={<DownloadOutlinedIcon />} variant="outlined" onClick={exportCsv}>
            CSV Matrix
          </Button>
          <Button startIcon={<ImageOutlinedIcon />} variant="contained" onClick={exportPng}>
            CFG PNG
          </Button>
        </Stack>
      </Stack>
    </Paper>
  )
}
