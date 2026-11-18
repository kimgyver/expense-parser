import { act, renderHook } from '@testing-library/react'
import { afterEach, describe, expect, it, vi } from 'vitest'
import { ClientErrorCode, STALE_DEBOUNCE_MS } from '../config'
import { parseExpenseText } from '../api/parse'
import { ParseFlowProvider, useParseFlow } from './ParseFlowContext'

vi.mock('../api/parse', async (importOriginal) => {
  const actual = await importOriginal<typeof import('../api/parse')>()
  return {
    ...actual,
    parseExpenseText: vi.fn(),
  }
})

const mockedParse = vi.mocked(parseExpenseText)

function renderParseFlow() {
  return renderHook(() => useParseFlow(), {
    wrapper: ({ children }) => <ParseFlowProvider>{children}</ParseFlowProvider>,
  })
}

describe('ParseFlowContext', () => {
  afterEach(() => {
    vi.clearAllMocks()
    vi.useRealTimers()
  })

  it('sets success state when parse returns ok', async () => {
    mockedParse.mockResolvedValue({
      ok: true,
      data: {
        costCentre: 'DEV632',
        totalIncludingTax: 100,
        salesTax: 13.04,
        totalExcludingTax: 86.96,
      },
    })

    const { result } = renderParseFlow()

    act(() => {
      result.current.setText('<total>100</total>')
    })

    await act(async () => {
      await result.current.submit()
    })

    expect(result.current.status).toBe('success')
    expect(result.current.result?.costCentre).toBe('DEV632')
    expect(result.current.error).toBeNull()
    expect(result.current.isStale).toBe(false)
  })

  it('sets error state when parse returns a domain error', async () => {
    mockedParse.mockResolvedValue({
      ok: false,
      error: {
        code: 'MISSING_TOTAL',
        message: 'Missing required <total> tag.',
      },
    })

    const { result } = renderParseFlow()

    act(() => {
      result.current.setText('<vendor>x</vendor>')
    })

    await act(async () => {
      await result.current.submit()
    })

    expect(result.current.status).toBe('error')
    expect(result.current.result).toBeNull()
    expect(result.current.error?.code).toBe('MISSING_TOTAL')
  })

  it('sets network error when parse throws', async () => {
    mockedParse.mockRejectedValue(new Error('offline'))

    const { result } = renderParseFlow()

    await act(async () => {
      await result.current.submit()
    })

    expect(result.current.status).toBe('error')
    expect(result.current.error?.code).toBe(ClientErrorCode.Network)
  })

  it('marks result stale after text changes past debounce', async () => {
    vi.useFakeTimers()
    mockedParse.mockResolvedValue({
      ok: true,
      data: {
        costCentre: 'UNKNOWN',
        totalIncludingTax: 10,
        salesTax: 1.3,
        totalExcludingTax: 8.7,
      },
    })

    const { result } = renderParseFlow()

    act(() => {
      result.current.setText('<total>10</total>')
    })

    await act(async () => {
      await result.current.submit()
    })

    expect(result.current.isStale).toBe(false)

    act(() => {
      result.current.setText('<total>11</total>')
    })

    act(() => {
      vi.advanceTimersByTime(STALE_DEBOUNCE_MS)
    })

    expect(result.current.isStale).toBe(true)
  })
})
