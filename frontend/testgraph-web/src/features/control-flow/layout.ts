import type { Edge, Node } from '@xyflow/react'
import type { ControlFlowGraphDto, FlowNodeKind } from './types'

export type CfgNodeData = {
  label: string
  kind: FlowNodeKind
  sourceLine?: number
}

const nodeWidth = 190
const rowGap = 140
const columnGap = 240

function calculatePositions(graph: ControlFlowGraphDto) {
  const levelById = new Map<number, number>([[graph.entryNodeId, 0]])
  const queue = [graph.entryNodeId]

  while (queue.length > 0) {
    const current = queue.shift()!
    const currentLevel = levelById.get(current) ?? 0

    graph.edges
      .filter((edge) => edge.sourceId === current && edge.kind !== 'Back')
      .sort((a, b) => a.targetId - b.targetId)
      .forEach((edge) => {
        if (!levelById.has(edge.targetId)) {
          levelById.set(edge.targetId, currentLevel + 1)
          queue.push(edge.targetId)
        }
      })
  }

  const maxLevel = Math.max(0, ...levelById.values())
  graph.nodes.forEach((node) => {
    if (!levelById.has(node.id)) levelById.set(node.id, maxLevel + 1)
  })

  const groups = new Map<number, number[]>()
  graph.nodes.forEach((node) => {
    const level = levelById.get(node.id) ?? 0
    const group = groups.get(level) ?? []
    group.push(node.id)
    groups.set(level, group)
  })

  const positions = new Map<number, { x: number; y: number }>()
  Array.from(groups.entries())
    .sort(([a], [b]) => a - b)
    .forEach(([level, ids]) => {
      ids.sort((a, b) => a - b)
      const width = (ids.length - 1) * columnGap
      ids.forEach((id, index) => {
        positions.set(id, {
          x: 520 - width / 2 + index * columnGap,
          y: level * rowGap,
        })
      })
    })

  return positions
}

export function toReactFlowElements(graph: ControlFlowGraphDto): {
  nodes: Node<CfgNodeData>[]
  edges: Edge[]
} {
  const positions = calculatePositions(graph)

  const nodes = graph.nodes.map((node) => ({
    id: String(node.id),
    type: 'cfg',
    position: positions.get(node.id) ?? { x: 0, y: 0 },
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
    type: 'smoothstep',
    animated: edge.kind === 'Back',
    data: { kind: edge.kind },
  } satisfies Edge))

  return { nodes, edges }
}
