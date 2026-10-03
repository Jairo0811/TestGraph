import { Paper, Stack, Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Typography } from '@mui/material'
import type { MatrixResultDto } from '../../api/types'

export default function AdjacencyMatrixPanel({ matrix }: { matrix: MatrixResultDto }) {
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
              {matrix.nodeIds.map((id) => (
                <TableCell key={id} align="center" sx={{ fontWeight: 800 }}>
                  {id}
                </TableCell>
              ))}
            </TableRow>
          </TableHead>
          <TableBody>
            {matrix.rows.map((row, rowIndex) => (
              <TableRow key={matrix.nodeIds[rowIndex]} hover>
                <TableCell component="th" scope="row" sx={{ fontWeight: 800 }}>
                  {matrix.nodeIds[rowIndex]}
                </TableCell>
                {row.map((value, columnIndex) => (
                  <TableCell
                    key={matrix.nodeIds[columnIndex]}
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
