import { useState } from 'react'
import PlayArrowOutlinedIcon from '@mui/icons-material/PlayArrowOutlined'
import RestartAltOutlinedIcon from '@mui/icons-material/RestartAltOutlined'
import { Alert, Button, Chip, Paper, Stack, TextField, Typography } from '@mui/material'
import { useMutation, useQuery } from '@tanstack/react-query'
import { analyzeSource, getSample, getSamples } from '../../api/client'
import type { AnalysisResultDto, TestCaseDraftDto } from '../../api/types'
import ControlFlowGraphViewer from '../control-flow/ControlFlowGraphViewer'
import ComplexityPanel from '../complexity/ComplexityPanel'
import BasisPathsPanel from '../paths/BasisPathsPanel'
import AdjacencyMatrixPanel from '../matrix/AdjacencyMatrixPanel'
import TestCaseDesignerPanel from '../testing/TestCaseDesignerPanel'
import AssistedTestSuggestionsPanel from '../testing/AssistedTestSuggestionsPanel'
import AcademicSamplesPanel from '../samples/AcademicSamplesPanel'
import ExportPanel from '../export/ExportPanel'
import AccountProjectsPanel from '../account/AccountProjectsPanel'

const defaultSource = `Entero edad
Real promedio
Real beca

Leer edad
Leer promedio

Si edad > 18 Entonces
    Si promedio >= 9 Entonces
        beca <- 2000
    Sino Si promedio >= 7.5 Entonces
        beca <- 1000
    Sino Si promedio >= 6 Entonces
        beca <- 500
    Sino
        beca <- 0
    Fin Si
Sino
    Si promedio >= 9 Entonces
        beca <- 3000
    Sino Si promedio >= 8 Entonces
        beca <- 2000
    Sino Si promedio >= 6 Entonces
        beca <- 100
    Sino
        beca <- 0
    Fin Si
Fin Si

Escribir beca`

export default function AnalysisWorkspace() {
  const [source, setSource] = useState(defaultSource)
  const [analysis, setAnalysis] = useState<AnalysisResultDto | null>(null)
  const [loadingSampleId, setLoadingSampleId] = useState<string>()
  const [selectedPathNumber, setSelectedPathNumber] = useState<number>()
  const [designedCases, setDesignedCases] = useState<TestCaseDraftDto[]>([])

  const samplesQuery = useQuery({
    queryKey: ['academic-samples'],
    queryFn: getSamples,
    retry: 1,
  })

  const analysisMutation = useMutation({
    mutationFn: analyzeSource,
    onSuccess: (result) => {
      setAnalysis(result)
      setSelectedPathNumber(undefined)
      setDesignedCases([])
    },
  })

  const clearDerivedState = () => {
    setAnalysis(null)
    setSelectedPathNumber(undefined)
    setDesignedCases([])
    analysisMutation.reset()
  }

  const updateSource = (next: string) => {
    setSource(next)
    clearDerivedState()
  }

  const runAnalysis = () => {
    analysisMutation.mutate(source)
  }

  const loadSample = async (id: string) => {
    setLoadingSampleId(id)
    try {
      const sample = await getSample(id)
      setSource(sample.sourceCode)
      clearDerivedState()
    } finally {
      setLoadingSampleId(undefined)
    }
  }

  const selectedPath = analysis?.paths.items.find((path) => path.number === selectedPathNumber)

  return (
    <Stack spacing={4}>
      <Paper variant="outlined" sx={{ p: { xs: 2, md: 3 } }}>
        <Stack spacing={2}>
          <Stack direction={{ xs: 'column', md: 'row' }} justifyContent="space-between" gap={2}>
            <div>
              <Typography variant="h5" fontWeight={800}>TGPL Analyzer</Typography>
              <Typography color="text.secondary">
                Edita pseudocódigo TGPL y ejecuta el pipeline real del backend.
              </Typography>
            </div>
            <Stack direction="row" gap={1} flexWrap="wrap">
              <Chip label="Lexer" size="small" variant="outlined" />
              <Chip label="Parser + AST" size="small" variant="outlined" />
              <Chip label="CFG" size="small" variant="outlined" />
              <Chip label="Complexity" size="small" variant="outlined" />
              <Chip label="Basis Paths" size="small" variant="outlined" />
            </Stack>
          </Stack>

          <TextField
            label="TGPL source"
            value={source}
            onChange={(event) => updateSource(event.target.value)}
            multiline
            minRows={18}
            maxRows={30}
            fullWidth
            slotProps={{
              input: {
                sx: {
                  fontFamily: 'Consolas, "Cascadia Code", monospace',
                  fontSize: 14,
                  lineHeight: 1.55,
                },
              },
            }}
          />

          <Stack direction={{ xs: 'column', sm: 'row' }} gap={1}>
            <Button
              variant="contained"
              startIcon={<PlayArrowOutlinedIcon />}
              disabled={analysisMutation.isPending || !source.trim()}
              onClick={runAnalysis}
            >
              {analysisMutation.isPending ? 'Analizando…' : 'Analizar'}
            </Button>
            <Button
              variant="outlined"
              startIcon={<RestartAltOutlinedIcon />}
              onClick={() => {
                setSource(defaultSource)
                clearDerivedState()
              }}
            >
              Restaurar Becas
            </Button>
          </Stack>

          {analysisMutation.error ? (
            <Alert severity="error">{analysisMutation.error.message}</Alert>
          ) : null}

          {!analysis && !analysisMutation.error ? (
            <Alert severity="info">
              Ejecuta el análisis para generar CFG, complejidad, basis paths, matriz y sugerencias desde el backend.
            </Alert>
          ) : null}
        </Stack>
      </Paper>

      {analysis ? (
        <>
          <ComplexityPanel result={analysis.complexity} />
          <ControlFlowGraphViewer graph={analysis.graph} highlightedPathNodeIds={selectedPath?.nodeIds} />
          <BasisPathsPanel
            result={analysis.paths}
            selectedPathNumber={selectedPathNumber}
            onSelectPath={setSelectedPathNumber}
          />
          <AdjacencyMatrixPanel matrix={analysis.matrix} />
          <TestCaseDesignerPanel
            sourceCode={source}
            paths={analysis.paths.items}
            onDraftsChange={setDesignedCases}
          />
          <AssistedTestSuggestionsPanel result={analysis.suggestions} />
          <ExportPanel sourceCode={source} />
        </>
      ) : null}

      <AccountProjectsPanel sourceCode={source} testCases={designedCases} />

      {samplesQuery.error ? (
        <Alert severity="warning">
          No se pudo cargar el catálogo académico. Verifica que TestGraph.Api esté ejecutándose en http://localhost:5152.
        </Alert>
      ) : null}

      <AcademicSamplesPanel
        samples={samplesQuery.data ?? []}
        loadingId={loadingSampleId}
        onLoad={loadSample}
      />
    </Stack>
  )
}
