import { useMemo, useState } from 'react'
import {
  Background,
  BackgroundVariant,
  Controls,
  MarkerType,
  MiniMap,
  ReactFlow,
  type Edge,
  type Node,
  type NodeTypes,
} from '@xyflow/react'
import '@xyflow/react/dist/style.css'
import { Box, Chip, Paper, Stack, Typography } from '@mui/material'
import CfgNode from './CfgNode'
import { toReactFlowElements, type CfgNodeData } from './layout'
import type { ControlFlowGraphDto, FlowEdgeKind, FlowNodeKind } from './types'

const nodeTypes: NodeTypes = { cfg: CfgNode }

const edgeColor: Record<FlowEdgeKind, string> = {
  Normal: '#64748b',
  True: '#22c55e',
  False: '#ef4444',
  Back: '#a78bfa',
}

const minimapColor: Record<FlowNodeKind, string> = {
  Entry: '#22d3ee',
  Exit: '#22d3ee',
  Statement: '#3b82f6',
  Decision: '#f59e0b',
  Merge: '#8b5cf6',
}

export interface ControlFlowGraphViewerProps {
  graph: ControlFlowGraphDto
  highlightedPathNodeIds?: number[]
}

export default function ControlFlowGraphViewer({ graph, highlightedPathNodeIds = [] }: ControlFlowGraphViewerProps) {
  const [selectedNodeId, setSelectedNodeId] = useState<string | null>(null)

  const elements = useMemo(() => toReactFlowElements(graph), [graph])
  const highlightedNodes = useMemo(
    () => new Set(highlightedPathNodeIds.map(String)),
    [highlightedPathNodeIds],
  )
  const highlightedPairs = useMemo(() => {
    const pairs = new Set<string>()
    for (let index = 0; index < highlightedPathNodeIds.length - 1; index++) {
      pairs.add(`${highlightedPathNodeIds[index]}:${highlightedPathNodeIds[index + 1]}`)
    }
    return pairs
  }, [highlightedPathNodeIds])

  const connectedEdgeIds = useMemo(() => {
    if (!selectedNodeId) return new Set<string>()

    return new Set(
      elements.edges
        .filter((edge) => edge.source === selectedNodeId || edge.target === selectedNodeId)
        .map((edge) => edge.id),
    )
  }, [elements.edges, selectedNodeId])

  const nodes: Node<CfgNodeData>[] = elements.nodes.map((node) => ({
    ...node,
    selected: node.id === selectedNodeId || (!selectedNodeId && highlightedNodes.has(node.id)),
  }))

  const edges: Edge[] = elements.edges.map((edge) => {
    const kind = (edge.data?.kind ?? 'Normal') as FlowEdgeKind
    const directHighlight = selectedNodeId !== null && connectedEdgeIds.has(edge.id)
    const pathHighlight = !selectedNodeId && highlightedPairs.has(`${edge.source}:${edge.target}`)
    const highlighted = directHighlight || pathHighlight
    const hasFocus = selectedNodeId !== null || highlightedPathNodeIds.length > 0

    return {
      ...edge,
      markerEnd: {
        type: MarkerType.ArrowClosed,
        color: highlighted ? '#22d3ee' : edgeColor[kind],
      },
      style: {
        stroke: highlighted ? '#22d3ee' : edgeColor[kind],
        strokeWidth: highlighted ? 3 : 1.8,
        opacity: hasFocus && !highlighted ? 0.25 : 1,
      },
      labelStyle: {
        fill: highlighted ? '#22d3ee' : edgeColor[kind],
        fontWeight: 700,
        fontSize: 12,
      },
    }
  })

  const selectedNode = selectedNodeId
    ? graph.nodes.find((node) => String(node.id) === selectedNodeId)
    : undefined

  return (
    <Stack spacing={2}>
      <Stack
        direction={{ xs: 'column', md: 'row' }}
        justifyContent="space-between"
        alignItems={{ md: 'flex-end' }}
        gap={2}
      >
        <Box>
          <Typography variant="h5" fontWeight={900}>
            Control Flow Graph
          </Typography>
          <Typography color="text.secondary">
            Selecciona un nodo o un basis path para resaltar el flujo correspondiente.
          </Typography>
        </Box>
        <Stack direction="row" gap={0.75} flexWrap="wrap" useFlexGap>
          <Chip size="small" label={`${graph.nodes.length} nodos`} />
          <Chip size="small" label={`${graph.edges.length} aristas`} />
          <Chip size="small" label={`${graph.nodes.filter((node) => node.kind === 'Decision').length} decisiones`} />
        </Stack>
      </Stack>

      <Paper
        data-testid="cfg-canvas"
        variant="outlined"
        sx={{
          height: { xs: 500, sm: 620, md: 760 },
          minHeight: 420,
          overflow: 'hidden',
          borderRadius: { xs: 2.5, md: 3 },
          borderColor: 'rgba(34, 211, 238, .16)',
          bgcolor: '#07111f',
          boxShadow: 'inset 0 0 60px rgba(0, 0, 0, .12)',
          '& .react-flow__minimap': {
            display: { xs: 'none', sm: 'block' },
          },
          '& .react-flow__controls': {
            transform: { xs: 'scale(.9)', sm: 'none' },
            transformOrigin: 'bottom left',
          },
        }}
      >
        <ReactFlow
          nodes={nodes}
          edges={edges}
          nodeTypes={nodeTypes}
          fitView
          fitViewOptions={{ padding: 0.18 }}
          minZoom={0.2}
          maxZoom={1.8}
          onNodeClick={(_, node) => setSelectedNodeId(node.id)}
          onPaneClick={() => setSelectedNodeId(null)}
          nodesDraggable
          nodesConnectable={false}
          elementsSelectable
          proOptions={{ hideAttribution: true }}
        >
          <Background color="#1e293b" gap={24} size={1} variant={BackgroundVariant.Dots} />
          <MiniMap
            pannable
            zoomable
            nodeColor={(node) => minimapColor[(node.data as CfgNodeData).kind]}
            maskColor="rgba(2, 6, 23, .68)"
          />
          <Controls showInteractive={false} />
        </ReactFlow>
      </Paper>

      <Stack direction={{ xs: 'column', md: 'row' }} gap={2}>
        <Paper variant="outlined" sx={{ p: 2, flex: 1 }}>
          <Typography variant="subtitle2" fontWeight={800} gutterBottom>
            Leyenda
          </Typography>
          <Stack direction="row" gap={0.75} flexWrap="wrap" useFlexGap>
            <Chip size="small" label="Entrada / Salida" sx={{ borderColor: '#22d3ee' }} variant="outlined" />
            <Chip size="small" label="Sentencia" sx={{ borderColor: '#3b82f6' }} variant="outlined" />
            <Chip size="small" label="Decisión" sx={{ borderColor: '#f59e0b' }} variant="outlined" />
            <Chip size="small" label="Merge" sx={{ borderColor: '#8b5cf6' }} variant="outlined" />
            <Chip size="small" label="Sí" sx={{ borderColor: '#22c55e' }} variant="outlined" />
            <Chip size="small" label="No" sx={{ borderColor: '#ef4444' }} variant="outlined" />
          </Stack>
        </Paper>

        <Paper variant="outlined" sx={{ p: 2, minWidth: { md: 300 }, flex: { xs: 1, md: '0 1 360px' } }}>
          <Typography variant="subtitle2" fontWeight={800}>
            Nodo seleccionado
          </Typography>
          {selectedNode ? (
            <Stack spacing={0.5} mt={1}>
              <Typography>{selectedNode.label}</Typography>
              <Typography variant="body2" color="text.secondary">
                #{selectedNode.id} · {selectedNode.kind}
                {selectedNode.sourceLine ? ` · línea ${selectedNode.sourceLine}` : ''}
              </Typography>
            </Stack>
          ) : highlightedPathNodeIds.length > 0 ? (
            <Typography variant="body2" color="text.secondary" mt={1} sx={{ overflowWrap: 'anywhere' }}>
              Basis path resaltado: {highlightedPathNodeIds.join(' → ')}
            </Typography>
          ) : (
            <Typography variant="body2" color="text.secondary" mt={1}>
              Ninguno. Haz clic sobre un nodo o selecciona un basis path.
            </Typography>
          )}
        </Paper>
      </Stack>
    </Stack>
  )
}
