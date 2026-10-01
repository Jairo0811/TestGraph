import type { Edge, Node } from '@xyflow/react'
import type { ControlFlowGraphDto, FlowNodeKind } from './types'

export type CfgNodeData = {
  label: string
  kind: FlowNodeKind
  sourceLine?: number
}

const nodeWidth = 190
const rowGap = 130
const columnGap = 250

const manualPositions: Record<number, { x: number; y: number }> = {
  1: { x: 360, y: 0 },
  2: { x: 360, y: rowGap },
  3: { x: 360, y: rowGap * 2 },
  4: { x: 360, y: rowGap * 3 },
  5: { x: 150, y: rowGap * 4 },
  10: { x: 650, y: rowGap * 4 },
  6: { x: 40, y: rowGap * 5 },
  7: { x: 270, y: rowGap * 5 },
  8: { x: 160, y: rowGap * 6 },
  9: { x: 380, y: rowGap * 6 },
  11: { x: 360, y: rowGap * 7 },
  12: { x: 360, y: rowGap * 8 },
  13: { x: 360, y: rowGap * 9 },
}

export function toReactFlowElements(graph: ControlFlowGraphDto): {
  nodes: Node<CfgNodeData>[]
  edges: Edge[]
} {
  const nodes = graph.nodes.map((node, index) => ({
    id: String(node.id),
    type: 'cfg',
    position: manualPositions[node.id] ?? {
      x: (index % 3) * columnGap,
      y: Math.floor(index / 3) * rowGap,
    },
    data: {
      label: node.label,
      kind: node.kind,
      sourceLine: node.sourceLine,
    },
    width: nodeWidth,
  } satisfies Node<CfgNodeData>))

  const edges = graph.edges.map((edge, index) => ({
    id: `e-${edge.sourceId}-${edge.targetId}-${index}`,
    source: String(edge.sourceId),
    target: String(edge.targetId),
    label: edge.label,
    type: edge.kind === 'Back' ? 'smoothstep' : 'default',
    animated: edge.kind === 'Back',
    data: { kind: edge.kind },
  } satisfies Edge))

  return { nodes, edges }
}
