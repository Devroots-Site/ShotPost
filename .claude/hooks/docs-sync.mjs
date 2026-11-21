// Claude-Code-Hook: haelt die Dokumentation unter docs/ aktuell.
//
//   PostToolUse (Edit|Write): merkt sich je Sitzung, welche Code-Dateien Claude geaendert hat
//                             und ob danach auch docs/ angefasst wurde.
//   Stop:                     wurde Code geaendert, aber nicht die Doku, bekommt Claude einmalig
//                             den Auftrag, die passenden Seiten nachzuziehen.
//
// Eingebunden in .claude/settings.json. Die Zuordnung Code -> Doku-Seite steht in CLAUDE.md.

import { existsSync, mkdirSync, readFileSync, rmSync, writeFileSync } from 'node:fs'
import { tmpdir } from 'node:os'
import { dirname, join, relative, resolve } from 'node:path'
import { fileURLToPath } from 'node:url'

const projectDir = resolve(dirname(fileURLToPath(import.meta.url)), '..', '..')

const CODE_EXTENSIONS = ['.cs', '.axaml', '.csproj', '.sql']
const CODE_FILES = ['docker-compose.yml', 'global.json', 'dotnet-tools.json', '.editorconfig']
const CODE_PREFIXES = ['.github/']
const IGNORED_PREFIXES = ['.claude/', 'bin/', 'obj/', '.idea/', '.git/']

function isDocRelevant(path) {
  if (IGNORED_PREFIXES.some((p) => path.startsWith(p))) return false
  return (
    CODE_EXTENSIONS.some((e) => path.endsWith(e)) ||
    CODE_FILES.includes(path) ||
    CODE_PREFIXES.some((p) => path.startsWith(p))
  )
}

function readInput() {
  try {
    return JSON.parse(readFileSync(0, 'utf8'))
  } catch {
    return {}
  }
}

const input = readInput()
const sessionId = String(input.session_id ?? 'default').replace(/[^\w-]/g, '_')
const stateFile = join(tmpdir(), 'shotpost-docs-sync', `${sessionId}.json`)

function loadState() {
  try {
    return JSON.parse(readFileSync(stateFile, 'utf8'))
  } catch {
    return { code: [], docs: false }
  }
}

function saveState(state) {
  mkdirSync(dirname(stateFile), { recursive: true })
  writeFileSync(stateFile, JSON.stringify(state))
}

if (input.hook_event_name === 'PostToolUse') {
  const file = input.tool_input?.file_path
  if (typeof file !== 'string') process.exit(0)

  const path = relative(projectDir, resolve(projectDir, file)).replaceAll('\\', '/')
  if (path.startsWith('..')) process.exit(0) // ausserhalb des Projekts

  const state = loadState()
  if (path.startsWith('docs/')) {
    state.docs = true
  } else if (isDocRelevant(path)) {
    if (!state.code.includes(path)) state.code.push(path)
    state.docs = false // Doku muss NACH der letzten Code-Aenderung angefasst werden
  } else {
    process.exit(0)
  }
  saveState(state)
  process.exit(0)
}

if (input.hook_event_name === 'Stop') {
  const state = loadState()
  if (existsSync(stateFile)) rmSync(stateFile) // nur einmal je Aenderungsrunde erinnern

  // stop_hook_active: Claude laeuft bereits wegen dieses Hooks weiter -> nicht erneut blockieren.
  if (input.stop_hook_active || state.code.length === 0 || state.docs) process.exit(0)

  const reason = [
    'Dokumentation pruefen: In dieser Runde wurden Code-Dateien geaendert, aber nichts unter docs/:',
    ...state.code.map((f) => `  - ${f}`),
    '',
    'Gleiche die betroffenen Seiten unter docs/ mit dem neuen Stand ab (Zuordnung: Abschnitt',
    '"Dokumentation" in CLAUDE.md) und aktualisiere sie knapp. Neue Seiten zusaetzlich in',
    'docs/.vitepress/config.mts eintragen. Beschreibt die Doku das geaenderte Verhalten gar nicht',
    '(z.B. reine Umformatierung oder internes Refactoring), aendere nichts und sage das in einem Satz.',
  ].join('\n')

  process.stdout.write(JSON.stringify({ decision: 'block', reason }))
}
