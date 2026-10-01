<div align="center">

# TestGraph

<img src="https://img.shields.io/badge/UNAPEC-ISO--300-003B70?style=for-the-badge" alt="UNAPEC ISO-300" />
<img src="https://img.shields.io/badge/Estado-v1.0.0--rc.1-2563EB-2563EB?style=for-the-badge" alt="Estado: v1.0.0-rc.1" />
<img src="https://img.shields.io/badge/Tipo-Portafolio%20%7C%20Open%20Source-6F42C1?style=for-the-badge" alt="Proyecto de portafolio y open source" />

<br/><br/>

<a href="https://github.com/Jairo0811/TestGraph/actions/workflows/ci.yml">
  <img src="https://github.com/Jairo0811/TestGraph/actions/workflows/ci.yml/badge.svg" alt="CI" />
</a>

<br/><br/>

**Visualize logic. Discover paths. Design better tests.**

*From algorithms to better software.*

</div>

## 📌 Descripción

**TestGraph** es una plataforma visual de **pruebas de caja blanca y análisis de flujo de control**. Su objetivo es transformar pseudocódigo estructurado en grafos de flujo de control interactivos, calcular complejidad ciclomática, identificar caminos linealmente independientes, generar matrices de adyacencia y apoyar el diseño de casos de prueba estructurales.

El proyecto evoluciona un trabajo académico de 2024 hacia una herramienta moderna de ingeniería de software con análisis determinístico y una arquitectura preparada para crecer por fases.

> 🎓 **Origen académico:** TestGraph nace a partir del proyecto final de **Fundamentos de Ingeniería de Software (ISO-300)** de la **Universidad APEC (UNAPEC)**, realizado durante el período **Septiembre - Diciembre 2024**.

---

## 🎓 Información académica

| Información | Detalle |
|---|---|
| 📖 Asignatura | **Fundamentos de Ingeniería de Software (ISO-300)** |
| 👨‍🏫 Profesor | **Leandro Eduardo Fondeur Gil** |
| 🏫 Institución | **Universidad APEC (UNAPEC)** |
| 📅 Período académico | **Septiembre - Diciembre 2024** |
| 📁 Tipo de entrega | **Proyecto Final** |
| 👥 Grupo | **#4** |
| 📅 Entrega original | **5 de diciembre de 2024** |

### 👥 Equipo académico original

| 👤 Integrante | 🆔 Matrícula |
|---|---|
| Francis Jairo Matias Rosario | A00115261 |
| Diego Jose Montero Almonte | A00115699 |
| Robinson Junior Novo Lopez | A00115885 |
| Angel Emmanuel Gonzalez Acosta | A00116360 |
| Christian Rainel Menendez Hiciano | A00116551 |

La versión moderna de **TestGraph** conserva los ejercicios académicos como escenarios de validación, pero la plataforma actual constituye una evolución técnica independiente orientada a portafolio.

---

## 🧭 Continuidad académica

Existe una continuidad verificable por profesor con [**IngSoft Studio**](https://github.com/Jairo0811/IngSoft-Studio). **Leandro Eduardo Fondeur Gil** impartió previamente **Introducción a la Ingeniería en Software (SOF-015)** en el **Instituto Tecnológico de Las Américas (ITLA)** durante **2017-C3**, y posteriormente **Fundamentos de Ingeniería de Software (ISO-300)** en **UNAPEC** durante **Septiembre - Diciembre 2024**.

| Orden | Institución | Asignatura | Proyecto | Período |
|---:|---|---|---|---|
| 1 | ITLA | Introducción a la Ingeniería en Software (SOF-015) | [**IngSoft Studio**](https://github.com/Jairo0811/IngSoft-Studio) | 2017-C3 |
| 2 | UNAPEC | Fundamentos de Ingeniería de Software (ISO-300) | **TestGraph** | Septiembre - Diciembre 2024 |

La relación es **docente y formativa**; ambos proyectos son aplicaciones independientes y no existe dependencia técnica entre ellos.

---

## 🎯 Visión

El flujo académico original requería realizar manualmente:

1. lectura y comprensión de pseudocódigo;
2. identificación de nodos, aristas, regiones y nodos predicado;
3. construcción del grafo de flujo de control;
4. cálculo de complejidad ciclomática;
5. identificación del conjunto base de caminos independientes;
6. construcción de la matriz del grafo;
7. diseño de casos de prueba efectivos.

TestGraph transforma ese flujo en:

```text
Structured Pseudocode
        ↓
      Lexer
        ↓
      Parser
        ↓
       AST
        ↓
Control Flow Graph
        ↓
┌──────────────────────────────┐
│ Cyclomatic Complexity        │
│ Independent Paths            │
│ Adjacency Matrix             │
│ Assisted Test Cases          │
│ Structural Coverage          │
└──────────────────────────────┘
```

---

## ✅ Alcance V1

- editor de pseudocódigo estructurado;
- lexer y parser para TGPL;
- Abstract Syntax Tree (AST);
- generación del Control Flow Graph (CFG);
- visualización interactiva del CFG;
- cálculo de complejidad ciclomática;
- análisis de basis paths;
- generación de matriz de adyacencia;
- diseño manual y asistido de casos de prueba;
- modelo de cobertura estructural;
- proyectos académicos de ejemplo;
- persistencia de proyectos;
- exportación a PNG, CSV y JSON.

---

## 🧠 TGPL — TestGraph Pseudocode Language

TestGraph V1 utilizará un lenguaje de pseudocódigo controlado en lugar de intentar interpretar pseudocódigo arbitrario en lenguaje natural.

```text
Entero edad
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
```

Construcciones iniciales:

- declaraciones: `Entero`, `Real`, `Logico`;
- entrada/salida: `Leer`, `Escribir`;
- asignaciones: `<-`;
- condiciones: `Si`, `Sino Si`, `Sino`, `Fin Si`;
- ciclos: `Mientras`, `Para`;
- operadores: `>`, `<`, `>=`, `<=`, `=`, `<>`, `Y`, `O`, `NO`.

---

## ⚙️ Motor de análisis

El núcleo de TestGraph debe permanecer determinístico:

```text
Source Code
    ↓
Lexer
    ↓
Tokens
    ↓
Parser
    ↓
AST
    ↓
CFG Builder
    ↓
ControlFlowGraph
    ↓
Complexity / Paths / Matrix / Tests
```

### Complejidad ciclomática

Para un grafo de flujo de control conectado se utilizarán las formulaciones equivalentes:

```text
V(G) = E - N + 2
V(G) = P + 1
V(G) = R
```

Donde:

- `E` = aristas;
- `N` = nodos;
- `P` = nodos predicado;
- `R` = regiones.

### Basis Path Analysis

El número de caminos base corresponde a la complejidad ciclomática:

```text
Basis Paths = V(G)
```

La interfaz permitirá seleccionar un camino y resaltarlo directamente sobre el CFG.

### Cobertura estructural

V1 modelará cobertura estructural sin instrumentación runtime:

- Node Coverage;
- Edge Coverage;
- Decision Coverage;
- Basis Path Coverage.

---

## 🧪 Ejercicios académicos preservados

Los ejercicios originales se conservarán como muestras y escenarios de validación del motor:

1. **Matrix Minimum Even** — identificar la columna con el menor número par en una matriz 5x3.
2. **Scholarship Calculator** — calcular una beca según edad y promedio académico.
3. **Numbers Ending in Four** — mostrar números terminados en `4` dentro de un rango.
4. **Find Number 24** — buscar el número `24` en una matriz 4x3.
5. **Discount Calculator** — calcular descuento según umbrales de precio.

Para Scholarship Calculator, el análisis académico original documentó:

```text
Nodes: 18
Edges: 23
Predicate Nodes: 6
Regions: 7
Cyclomatic Complexity: 7
```

Para Discount Calculator:

```text
price >= 200       → 15%
price < 100        → 10%
100 <= price < 200 → 12%
```

Complejidad ciclomática original: `3`.

---

## 🧱 Stack tecnológico objetivo

### 🎨 Frontend

<p>
  <img src="https://skillicons.dev/icons?i=react,ts,vite" alt="React, TypeScript y Vite" />
</p>

- React 19;
- TypeScript;
- Vite;
- React Router;
- TanStack Query;
- React Flow;
- Monaco Editor;
- MUI.

### ⚙️ Backend

<p>
  <img src="https://skillicons.dev/icons?i=dotnet,cs" alt=".NET y C#" />
</p>

- .NET 10;
- ASP.NET Core Web API;
- C#;
- Entity Framework Core.

### 🗄️ Datos y herramientas

<p>
  <img src="https://cdn.jsdelivr.net/gh/devicons/devicon/icons/microsoftsqlserver/microsoftsqlserver-plain.svg" alt="Microsoft SQL Server" width="52" height="52" />
  <img src="https://skillicons.dev/icons?i=git,github,githubactions" alt="Git, GitHub y GitHub Actions" />
</p>

- Microsoft SQL Server;
- Git / GitHub;
- GitHub Actions.

> El stack anterior corresponde a la arquitectura objetivo definida en **Fase 0**. La implementación de código comienza formalmente en **Fase 1**.

---

## 🏗️ Arquitectura objetivo

TestGraph utilizará **Modular Monolith + Clean Architecture**.

```text
TestGraph.sln

src/
├── TestGraph.Domain
├── TestGraph.Application
├── TestGraph.Analysis
├── TestGraph.Infrastructure
└── TestGraph.Api

tests/
├── TestGraph.Domain.Tests
├── TestGraph.Application.Tests
└── TestGraph.Analysis.Tests

web/
└── testgraph-web
```

`TestGraph.Analysis` será el principal diferenciador técnico:

```text
TestGraph.Analysis
├── Lexing
├── Parsing
├── Syntax
├── ControlFlow
├── Complexity
├── Paths
├── Matrix
└── Testing
```

---

## 🗃️ Modelo de dominio inicial

Entidades base definidas para la implementación:

- `Project`;
- `Analysis`;
- `GraphNode`;
- `GraphEdge`;
- `ExecutionPath`;
- `TestCase`.

La autenticación seguirá inicialmente el modelo:

```text
Guest Mode
+
Optional Account
```

Los invitados podrán analizar pseudocódigo y utilizar muestras; las cuentas añadirán persistencia, historial, exportaciones y almacenamiento asociado al usuario.

---

## 🔌 Dirección de API

Endpoints iniciales previstos:

```http
POST /api/analysis/parse
POST /api/analysis/control-flow
POST /api/analysis/complexity
POST /api/analysis/paths
POST /api/analysis/test-cases
```

Posteriormente el flujo podrá consolidarse en:

```http
POST /api/analysis
```

---

## 🤖 Filosofía de IA

La IA **no forma parte del núcleo matemático ni estructural** de TestGraph.

Deben permanecer determinísticos:

- generación del CFG;
- complejidad ciclomática;
- análisis de caminos;
- matrices de adyacencia.

La IA podrá incorporarse como apoyo para:

- explicar grafos y caminos;
- sugerir escenarios de prueba adicionales;
- explicar complejidad elevada;
- sugerir oportunidades de refactorización.

---

## 🚫 Fuera de alcance de V1

- análisis directo de C#;
- análisis de Java;
- análisis de JavaScript/TypeScript;
- análisis de Python;
- análisis automático de repositorios GitHub;
- integraciones CI/CD;
- instrumentación runtime;
- mutation testing;
- load testing;
- security testing;
- static analysis empresarial.

### Evolución futura

- **V2 — Real Source Code:** primer objetivo C# con Roslyn cuando aporte valor.
- **V3 — Repository Analysis:** análisis de métodos complejos y priorización de refactorización/pruebas.

---

## 🗺️ Roadmap

| Fase | Alcance | Estado |
|---:|---|:---:|
| 0 | Definición del producto y fundación técnica | ✅ |
| 1 | Solution Foundation | ✅ |
| 2 | TGPL Lexer | ✅ |
| 3 | TGPL Parser + AST | ✅ |
| 4 | Control Flow Graph Engine | ✅ |
| 5 | CFG Visualization | ✅ |
| 6 | Cyclomatic Complexity | ✅ |
| 7 | Basis Path Analysis | ✅ |
| 8 | Adjacency Matrix | ✅ |
| 9 | Test Case Designer | ✅ |
| 10 | Assisted Test Generation | ✅ |
| 11 | Projects + Persistence | ✅ |
| 12 | Authentication | ✅ |
| 13 | Academic Samples | ✅ |
| 14 | Exports & Reports | ✅ |
| 15 | QA + Hardening | ✅ |
| 16 | Release Candidate | 🔄 |

---

## 📊 Estado actual

**Fases 0 a 15 completadas. Fase 16 — Release Candidate (v1.0.0-rc.1) en revisión.**

TestGraph ya está formalmente definido como una plataforma visual de análisis de flujo de control y pruebas de caja blanca. El roadmap V1 está implementado hasta la **Fase 16 — Release Candidate**. El siguiente paso es integrar los PRs pendientes en orden, validar CI en `main` y promover `v1.0.0-rc.1` a una versión estable cuando no queden bloqueos.

---

<p align="center">
  <strong>TestGraph · Visualize logic. Discover paths. Design better tests.</strong><br/>
  Universidad APEC (UNAPEC) · Fundamentos de Ingeniería de Software (ISO-300)
</p>
