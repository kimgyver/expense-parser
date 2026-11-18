import { afterEach, describe, expect, it, vi } from 'vitest'
import { ClientErrorCode, PARSE_ENDPOINT } from '../config'
import { parseExpenseText } from './parse'

describe('parseExpenseText', () => {
  afterEach(() => {
    vi.unstubAllGlobals()
    vi.restoreAllMocks()
  })

  it('maps a successful response to ok data', async () => {
    const payload = {
      vendor: 'Seaside Steakhouse',
      costCentre: 'DEV632',
      totalIncludingTax: 35000,
      salesTax: 4565.22,
      totalExcludingTax: 30434.78,
    }

    vi.stubGlobal(
      'fetch',
      vi.fn().mockResolvedValue({
        ok: true,
        json: async () => payload,
      }),
    )

    const result = await parseExpenseText('<total>35000</total>')

    expect(fetch).toHaveBeenCalledWith(
      PARSE_ENDPOINT,
      expect.objectContaining({
        method: 'POST',
        body: JSON.stringify({ text: '<total>35000</total>' }),
      }),
    )
    expect(result).toEqual({ ok: true, data: payload })
  })

  it('maps a 400 body to a structured error', async () => {
    vi.stubGlobal(
      'fetch',
      vi.fn().mockResolvedValue({
        ok: false,
        json: async () => ({
          code: 'MISSING_TOTAL',
          message: 'Missing required <total> tag.',
          correlationId: 'abc-123',
        }),
      }),
    )

    const result = await parseExpenseText('<vendor>x</vendor>')

    expect(result).toEqual({
      ok: false,
      error: {
        code: 'MISSING_TOTAL',
        message: 'Missing required <total> tag.',
        correlationId: 'abc-123',
      },
    })
  })

  it('falls back when error fields are missing', async () => {
    vi.stubGlobal(
      'fetch',
      vi.fn().mockResolvedValue({
        ok: false,
        json: async () => ({}),
      }),
    )

    const result = await parseExpenseText('bad')

    expect(result).toEqual({
      ok: false,
      error: {
        code: ClientErrorCode.Unknown,
        message: 'Request failed',
        correlationId: undefined,
      },
    })
  })
})
