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
}

export default function ControlFlowGraphViewer({ graph }: ControlFlowGraphViewerProps) {
  const [selectedNodeId, setSelectedNodeId] = useState<string | null>(null)

  const elements = useMemo(() => toReactFlowElements(graph), [graph])

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
    selected: node.id === selectedNodeId,
  }))

  const edges: Edge[] = elements.edges.map((edge) => {
    const kind = (edge.data?.kind ?? 'Normal') as FlowEdgeKind
    const highlighted = selectedNodeId !== null && connectedEdgeIds.has(edge.id)

    return {
      ...edge,
      markerEnd: {
        type: MarkerType.ArrowClosed,
        color: highlighted ? '#22d3ee' : edgeColor[kind],
      },
      style: {
        stroke: highlighted ? '#22d3ee' : edgeColor[kind],
        strokeWidth: highlighted ? 3 : 1.8,
        opacity: selectedNodeId && !highlighted ? 0.28 : 1,
      },
      labelStyle: {
        fill: edgeColor[kind],
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
      <Stack direction={{ xs: 'column', md: 'row' }} justifyContent="space-between" gap={2}>
        <Box>
          <Typography variant="h5" fontWeight={800}>
            Control Flow Graph
          </Typography>
          <Typography color="text.secondary">
            Selecciona un nodo para resaltar sus conexiones directas.
          </Typography>
        </Box>
        <Stack direction="row" gap={1} flexWrap="wrap">
          <Chip size="small" label={`${graph.nodes.length} nodos`} />
          <Chip size="small" label={`${graph.edges.length} aristas`} />
          <Chip size="small" label={`${graph.nodes.filter((node) => node.kind === 'Decision').length} decisiones`} />
        </Stack>
      </Stack>

      <Paper
        variant="outlined"
        sx={{
          height: { xs: 620, md: 760 },
          overflow: 'hidden',
          borderColor: 'rgba(148,163,184,.2)',
          bgcolor: '#07111f',
        }}
      >
        <ReactFlow
          nodes={nodes}
          edges={edges}
          nodeTypes={nodeTypes}
          fitView
          fitViewOptions={{ padding: 0.18 }}
          minZoom={0.25}
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
          <Stack direction="row" gap={1} flexWrap="wrap">
            <Chip size="small" label="Entrada / Salida" sx={{ borderColor: '#22d3ee' }} variant="outlined" />
            <Chip size="small" label="Sentencia" sx={{ borderColor: '#3b82f6' }} variant="outlined" />
            <Chip size="small" label="Decisión" sx={{ borderColor: '#f59e0b' }} variant="outlined" />
            <Chip size="small" label="Merge" sx={{ borderColor: '#8b5cf6' }} variant="outlined" />
            <Chip size="small" label="Sí" sx={{ borderColor: '#22c55e' }} variant="outlined" />
            <Chip size="small" label="No" sx={{ borderColor: '#ef4444' }} variant="outlined" />
          </Stack>
        </Paper>

        <Paper variant="outlined" sx={{ p: 2, minWidth: { md: 300 } }}>
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
          ) : (
            <Typography variant="body2" color="text.secondary" mt={1}>
              Ninguno. Haz clic sobre un nodo del grafo.
            </Typography>
          )}
        </Paper>
      </Stack>
    </Stack>
  )
}
