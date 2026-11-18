import { ClientErrorCode, PARSE_ENDPOINT } from '../config'

export type ExpenseParseResult = {
  vendor?: string | null
  description?: string | null
  date?: string | null
  costCentre: string
  paymentMethod?: string | null
  totalIncludingTax: number
  salesTax: number
  totalExcludingTax: number
}

export type ParseErrorResponse = {
  code: string
  message: string
  correlationId?: string
}

export type ParseApiResult =
  | { ok: true; data: ExpenseParseResult }
  | { ok: false; error: ParseErrorResponse }

export async function parseExpenseText(text: string): Promise<ParseApiResult> {
  const response = await fetch(PARSE_ENDPOINT, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({ text }),
  })

  const body = await response.json()

  if (!response.ok) {
    return {
      ok: false,
      error: {
        code: body.code ?? ClientErrorCode.Unknown,
        message: body.message ?? 'Request failed',
        correlationId: body.correlationId,
      },
    }
  }

  return { ok: true, data: body as ExpenseParseResult }
}
