import { Paper, Stack, Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Typography } from '@mui/material'
import type { ControlFlowGraphDto } from '../control-flow/types'

export interface AdjacencyMatrixPanelProps {
  graph: ControlFlowGraphDto
}

export default function AdjacencyMatrixPanel({ graph }: AdjacencyMatrixPanelProps) {
  const nodeIds = [...graph.nodes].map((node) => node.id).sort((a, b) => a - b)
  const indexes = new Map(nodeIds.map((id, index) => [id, index]))
  const matrix = nodeIds.map(() => nodeIds.map(() => 0))

  graph.edges.forEach((edge) => {
    const row = indexes.get(edge.sourceId)
    const column = indexes.get(edge.targetId)

    if (row !== undefined && column !== undefined) {
      matrix[row][column] += 1
    }
  })

  return (
    <Stack spacing={2}>
      <div>
        <Typography variant="h5" fontWeight={800}>
          Matriz de adyacencia
        </Typography>
        <Typography color="text.secondary">
          Cada fila representa el nodo origen y cada columna el nodo destino.
        </Typography>
      </div>

      <TableContainer component={Paper} variant="outlined" sx={{ maxHeight: 520 }}>
        <Table stickyHeader size="small" aria-label="Matriz de adyacencia del CFG">
          <TableHead>
            <TableRow>
              <TableCell sx={{ fontWeight: 800 }}>Nodo</TableCell>
              {nodeIds.map((id) => (
                <TableCell key={id} align="center" sx={{ fontWeight: 800 }}>
                  {id}
                </TableCell>
              ))}
            </TableRow>
          </TableHead>
          <TableBody>
            {matrix.map((row, rowIndex) => (
              <TableRow key={nodeIds[rowIndex]} hover>
                <TableCell component="th" scope="row" sx={{ fontWeight: 800 }}>
                  {nodeIds[rowIndex]}
                </TableCell>
                {row.map((value, columnIndex) => (
                  <TableCell
                    key={nodeIds[columnIndex]}
                    align="center"
                    sx={{
                      color: value > 0 ? 'primary.main' : 'text.secondary',
                      fontWeight: value > 0 ? 800 : 400,
                    }}
                  >
                    {value}
                  </TableCell>
                ))}
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>
    </Stack>
  )
}
