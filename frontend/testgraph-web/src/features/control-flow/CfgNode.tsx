import { memo } from 'react'
import { Handle, Position, type NodeProps } from '@xyflow/react'
import { Box, Typography } from '@mui/material'
import type { CfgNodeData } from './layout'

const accentByKind: Record<CfgNodeData['kind'], string> = {
  Entry: '#22d3ee',
  Exit: '#22d3ee',
  Statement: '#60a5fa',
  Decision: '#f59e0b',
  Merge: '#a78bfa',
}

function CfgNode({ data, selected }: NodeProps) {
  const nodeData = data as CfgNodeData
  const accent = accentByKind[nodeData.kind]

  return (
    <Box
      sx={{
        minWidth: 170,
        maxWidth: 220,
        border: '1px solid',
        borderColor: selected ? accent : 'rgba(148,163,184,.35)',
        borderRadius:
          nodeData.kind === 'Decision' ? 2 : nodeData.kind === 'Entry' || nodeData.kind === 'Exit' ? 5 : 2.5,
        bgcolor: selected ? 'rgba(15, 23, 42, .98)' : 'rgba(15, 23, 42, .92)',
        boxShadow: selected ? `0 0 0 2px ${accent}33, 0 10px 30px rgba(0,0,0,.35)` : '0 8px 24px rgba(0,0,0,.25)',
        px: 2,
        py: 1.4,
        position: 'relative',
      }}
    >
      <Handle type="target" position={Position.Top} style={{ background: accent }} />
      <Typography
        variant="caption"
        sx={{ display: 'block', color: accent, fontWeight: 800, letterSpacing: '.08em', textTransform: 'uppercase' }}
      >
        {nodeData.kind}
      </Typography>
      <Typography variant="body2" sx={{ fontWeight: 700, mt: 0.4 }}>
        {nodeData.label}
      </Typography>
      {nodeData.sourceLine && (
        <Typography variant="caption" color="text.secondary">
          Línea {nodeData.sourceLine}
        </Typography>
      )}
      <Handle type="source" position={Position.Bottom} style={{ background: accent }} />
    </Box>
  )
}

export default memo(CfgNode)
