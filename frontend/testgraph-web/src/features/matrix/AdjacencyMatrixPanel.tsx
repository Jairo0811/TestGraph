import { Paper, Stack, Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Typography } from '@mui/material'
import type { MatrixResultDto } from '../../api/types'

export default function AdjacencyMatrixPanel({ matrix }: { matrix: MatrixResultDto }) {
  const minimumWidth = Math.max(720, matrix.nodeIds.length * 64)

  return (
    <Stack spacing={2}>
      <div>
        <Typography variant="h5" fontWeight={900}>
          Matriz de adyacencia
        </Typography>
        <Typography color="text.secondary">
          Cada fila representa el nodo origen y cada columna el nodo destino.
        </Typography>
      </div>

      <TableContainer
        component={Paper}
        variant="outlined"
        sx={{
          maxHeight: { xs: 440, sm: 520 },
          maxWidth: '100%',
          overflowX: 'auto',
          borderRadius: { xs: 2.5, md: 3 },
          borderColor: 'rgba(148, 163, 184, .18)',
        }}
      >
        <Table stickyHeader size="small" aria-label="Matriz de adyacencia del CFG" sx={{ minWidth: minimumWidth }}>
          <TableHead>
            <TableRow>
              <TableCell
                sx={{
                  fontWeight: 900,
                  position: 'sticky',
                  left: 0,
                  zIndex: 4,
                  bgcolor: 'background.paper',
                }}
              >
                Nodo
              </TableCell>
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
                <TableCell
                  component="th"
                  scope="row"
                  sx={{
                    fontWeight: 900,
                    position: 'sticky',
                    left: 0,
                    zIndex: 2,
                    bgcolor: 'background.paper',
                  }}
                >
                  {matrix.nodeIds[rowIndex]}
                </TableCell>
                {row.map((value, columnIndex) => (
                  <TableCell
                    key={matrix.nodeIds[columnIndex]}
                    align="center"
                    sx={{
                      color: value > 0 ? 'primary.main' : 'text.secondary',
                      fontWeight: value > 0 ? 900 : 400,
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
