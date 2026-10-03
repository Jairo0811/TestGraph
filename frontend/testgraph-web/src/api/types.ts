import type { ControlFlowGraphDto } from '../features/control-flow/types'

export interface ComplexityResultDto {
  value: number
  nodes: number
  edges: number
  predicates: number
  regions: number
  connectedComponents: number
  formulas: {
    edgeNode: number
    predicate: number
    region: number
    agree: boolean
  }
  level: string
}

export interface BasisPathDto {
  number: number
  nodeIds: number[]
  edgeIndexes: number[]
  display: string
}

export interface PathsResultDto {
  cyclomaticComplexity: number
  pathCount: number
  candidatePathCount: number
  complete: boolean
  items: BasisPathDto[]
}

export interface MatrixResultDto {
  nodeIds: number[]
  rows: number[][]
  size: number
}

export interface BoundarySuggestionDto {
  variable: string
  operator: string
  threshold: number
  values: number[]
  sourceLine?: number
}

export interface SuggestedTestCaseDto {
  number: number
  name: string
  inputs: Record<string, string>
  expectedResult: string
  technique: string
  rationale: string
  sourceLine?: number
}

export interface AssistedSuggestionsDto {
  boundaries: BoundarySuggestionDto[]
  testCases: SuggestedTestCaseDto[]
}

export interface AnalysisResultDto {
  graph: ControlFlowGraphDto
  complexity: ComplexityResultDto
  paths: PathsResultDto
  matrix: MatrixResultDto
  suggestions: AssistedSuggestionsDto
}

export interface TestCaseDraftDto {
  name: string
  inputs: Record<string, string>
  expectedResult: string
  technique: string
  linkedPathNumber?: number
}

export interface StructuralTestCaseDto {
  number: number
  name: string
  inputs: Record<string, string>
  expectedResult: string
  technique: string | number
  linkedPathNumber?: number
  coveredNodeIds: number[]
  coveredEdgeIndexes: number[]
}

export interface TestCaseDiagnosticDto {
  draftIndex: number
  code: string
  message: string
}

export interface TestCaseDesignResultDto {
  testCases: StructuralTestCaseDto[]
  diagnostics: TestCaseDiagnosticDto[]
  isValid: boolean
}

export interface AcademicSampleSummaryDto {
  id: string
  name: string
  originalAuthor: string
  description: string
  expectedCyclomaticComplexity?: number
  analyzerReady: boolean
  limitation?: string
}

export interface AcademicSampleDto extends AcademicSampleSummaryDto {
  sourceCode: string
}

export interface AuthUserDto {
  id: string
  email: string
}

export interface AuthResponseDto {
  accessToken: string
  expiresAt: string
  user: AuthUserDto
}

export interface ProjectSummaryDto {
  id: string
  name: string
  description?: string
  createdAt: string
  updatedAt: string
  analysisCount: number
}

export interface SavedAnalysisDto {
  id: string
  projectId: string
  cyclomaticComplexity: number
  nodes: number
  edges: number
  paths: number
}
