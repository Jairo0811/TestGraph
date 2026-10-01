export type FlowNodeKind = 'Entry' | 'Exit' | 'Statement' | 'Decision' | 'Merge'

export type FlowEdgeKind = 'Normal' | 'True' | 'False' | 'Back'

export interface FlowNodeDto {
  id: number
  kind: FlowNodeKind
  label: string
  sourceLine?: number
}

export interface FlowEdgeDto {
  sourceId: number
  targetId: number
  kind: FlowEdgeKind
  label?: string
}

export interface ControlFlowGraphDto {
  nodes: FlowNodeDto[]
  edges: FlowEdgeDto[]
  entryNodeId: number
  exitNodeId: number
}
