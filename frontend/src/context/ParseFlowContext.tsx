import {
  createContext,
  useContext,
  useEffect,
  useState,
  type ReactNode,
} from 'react'
import {
  parseExpenseText,
  type ExpenseParseResult,
  type ParseErrorResponse,
} from '../api/parse'
import { API_PORT, ClientErrorCode, STALE_DEBOUNCE_MS } from '../config'
import { samples, type SampleKey } from '../samples'

export type ParseStatus = 'idle' | 'loading' | 'success' | 'error'

type ParseFlowContextValue = {
  text: string
  setText: (value: string) => void
  status: ParseStatus
  result: ExpenseParseResult | null
  error: ParseErrorResponse | null
  isStale: boolean
  submit: () => Promise<void>
  clear: () => void
  loadSample: (key: SampleKey) => void
}

const ParseFlowContext = createContext<ParseFlowContextValue | null>(null)

function useParseFlowState(): ParseFlowContextValue {
  const [text, setText] = useState('')
  const [submittedText, setSubmittedText] = useState<string | null>(null)
  const [status, setStatus] = useState<ParseStatus>('idle')
  const [result, setResult] = useState<ExpenseParseResult | null>(null)
  const [error, setError] = useState<ParseErrorResponse | null>(null)
  const [isStale, setIsStale] = useState(false)

  useEffect(() => {
    if (submittedText === null) {
      setIsStale(false)
      return
    }

    const timer = window.setTimeout(() => {
      setIsStale(text !== submittedText)
    }, STALE_DEBOUNCE_MS)

    return () => window.clearTimeout(timer)
  }, [text, submittedText])

  async function submit() {
    setStatus('loading')
    setError(null)

    try {
      const apiResult = await parseExpenseText(text)
      setSubmittedText(text)
      setIsStale(false)

      if (apiResult.ok) {
        setResult(apiResult.data)
        setError(null)
        setStatus('success')
        return
      }

      setResult(null)
      setError(apiResult.error)
      setStatus('error')
    } catch {
      setResult(null)
      setError({
        code: ClientErrorCode.Network,
        message: `Could not reach the API. Is the backend running on port ${API_PORT}?`,
      })
      setStatus('error')
    }
  }

  function clear() {
    setText('')
    setSubmittedText(null)
    setResult(null)
    setError(null)
    setStatus('idle')
    setIsStale(false)
  }

  function loadSample(key: SampleKey) {
    setText(samples[key])
    setSubmittedText(null)
    setResult(null)
    setError(null)
    setStatus('idle')
    setIsStale(false)
  }

  return {
    text,
    setText,
    status,
    result,
    error,
    isStale,
    submit,
    clear,
    loadSample,
  }
}

export const ParseFlowProvider = ({ children }: { children: ReactNode }) => {
  const value = useParseFlowState()
  return (
    <ParseFlowContext.Provider value={value}>
      {children}
    </ParseFlowContext.Provider>
  )
}

export function useParseFlow() {
  const value = useContext(ParseFlowContext)
  if (!value) {
    throw new Error('useParseFlow must be used within ParseFlowProvider')
  }
  return value
}
