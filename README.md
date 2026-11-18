# Expense Email Parser

Parse marked-up email text, validate tags, and derive GST from a tax-inclusive `<total>`.

## Stack

- **Backend:** .NET 8 Web API (`ExpenseParser.Api` + `ExpenseParser.Core`)
- **Frontend:** React + TypeScript (Vite)
- **Tests:** xUnit (Core pipeline) + Vitest (UI)

## Quick start

### API (port 5080)

```bash
cd backend
dotnet run --project ExpenseParser.Api
```

Swagger: http://localhost:5080/swagger  
Health: http://localhost:5080/health

### UI (port 5173)

```bash
cd frontend
npm install
npm run dev
```

Vite proxies `/api` → `http://localhost:5080`.

### Tests

```bash
cd backend
dotnet test
```

Covers the assignment email sample, GST math, missing/invalid `<total>`, unbalanced tags, empty input, and default `cost_centre` → `UNKNOWN` (8 tests).

```bash
cd frontend
npm test
```

Vitest covers API result mapping (`parseExpenseText`) and parse-flow state (success / domain error / network / stale).

## API contract

`POST /api/v1/parse`

Request:

```json
{ "text": "email body with markup..." }
```

Success `200` (`ExpenseParseResult`):

```json
{
  "vendor": "Seaside Steakhouse",
  "description": "...",
  "date": "27 April 2022",
  "costCentre": "DEV632",
  "paymentMethod": "personal card",
  "totalIncludingTax": 35000,
  "salesTax": 4565.22,
  "totalExcludingTax": 30434.78
}
```

Failure `400` (`ParseErrorResponse`):

```json
{
  "code": "MISSING_TOTAL",
  "message": "Missing required <total> tag.",
  "correlationId": "..."
}
```

Also available: `GET /health` → `{ "status": "ok" }`.

### Error codes

| Code | Meaning |
|------|---------|
| `EMPTY_TEXT` | Empty input |
| `TEXT_TOO_LONG` | Over 100,000 characters |
| `UNBALANCED_TAG` | Opening/closing tag mismatch → reject entire message |
| `MISSING_TOTAL` | No `<total>` → reject entire message |
| `INVALID_TOTAL` | `<total>` present but not a number (e.g. `abc`, `35,000a`) |

Missing `<cost_centre>` does **not** reject; output uses `UNKNOWN`.

## Design decisions

### Pipeline (extract → validate → enrich)

1. **Extract** markup fields and enforce balanced tags  
2. **Validate** required `<total>`, default `cost_centre`  
3. **Enrich** sales tax from tax-inclusive total  

Keeps parsing rules testable without HTTP.

### Why `POST` (not `GET`)

Parsing a large email body is a processing action, not a simple resource fetch. `POST` keeps the payload in the request body, avoiding URL length limits and browser history.

### Tax assumption

- NZ **GST 15%** (exposed in the API as `salesTax`; rate in `TaxDefaults.SalesTaxRate`)
- `<total>` is **tax-inclusive**
- `totalExcludingTax = round(total / 1.15, 2)`
- `salesTax = round(total - totalExcludingTax, 2)`

### Supported tags

| Kind | Tags |
|------|------|
| Inside an `<expense>…</expense>` block in the email | `cost_centre`, `total`, `payment_method` |
| Elsewhere in the email body (standalone tags) | `vendor`, `description`, `date` |

### Also included

- API versioning path `/api/v1`
- CORS for local Vite
- `X-Correlation-Id` middleware
- Structured error codes
- Sample loaders + stale-input hint (`isStale`) in the UI
- CodeMirror 6 input editor with XML/markup highlighting
- JSON viewer for results
- Parse flow state via React Context (`ParseFlowProvider`) so Input/Result stay presentational
- Vitest unit tests for API client mapping and parse-flow state

### Intentionally omitted

- Auth (JWT / API key) — would sit at API gateway in production
- Rate limiting / DDoS controls
- Database / persistence (stateless parse)
- Idempotency keys (same input → same output; no side effects)

### Possible next steps

- Move policy knobs (`SalesTaxRate`, `MaxTextLength`, CORS origins) to `appsettings` + `IOptions<T>`
- Global exception middleware for unexpected failures (500 + correlation id)
- Broader Core tests (e.g. orphan closing tags) and a thin API integration test
- Stronger markup parsing if rules grow (e.g. dedicated parser, or XML only for the `<expense>` block—not the whole email)
- Request abort / in-flight handling on the UI for slower networks

## Project layout

```text
expense-parser/
├── README.md
├── backend/
│   ├── ExpenseParser.Api/
│   │   ├── Controllers/         # Parse, Health
│   │   ├── Contracts/           # ParseRequest, ParseErrorResponse
│   │   ├── Middleware/          # X-Correlation-Id
│   │   └── Extensions/
│   ├── ExpenseParser.Core/
│   │   ├── Constants/           # MarkupTags, ParseErrorCode, Defaults
│   │   ├── Pipeline/            # MarkupExtractor → FieldValidator → TaxCalculator
│   │   ├── Models/              # RawExpenseFields, ExpenseParseResult, …
│   │   └── Services/            # IExpenseParseService
│   └── ExpenseParser.Tests/
└── frontend/
    └── src/
        ├── api/                 # parse client + types (+ unit tests)
        ├── components/          # InputPanel, ResultPanel, MarkupEditor
        ├── context/             # ParseFlowProvider / useParseFlow (+ unit tests)
        ├── test/                # Vitest setup
        ├── config.ts            # endpoint, debounce, client error codes
        └── samples.ts
```

