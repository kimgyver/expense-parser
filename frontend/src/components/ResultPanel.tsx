import JsonView from '@uiw/react-json-view'
import { useParseFlow } from '../context/ParseFlowContext'

export const ResultPanel = () => {
  const { status, result, error, isStale } = useParseFlow()

  return (
    <section className="panel">
      <div className="panel-heading">
        <h2>Result</h2>
      </div>

      {isStale && (status === 'success' || status === 'error') && (
        <div className="hint" role="status">
          Input changed — Submit again to refresh results.
        </div>
      )}

      {status === 'idle' && <p className="placeholder">No result yet.</p>}

      {status === 'loading' && <p className="placeholder">Parsing…</p>}

      {status === 'success' && result && (
        <>
          <p className="success-label">Parsed successfully</p>
          <div className="json-wrap">
            <JsonView
              value={result}
              collapsed={false}
              displayDataTypes={false}
              displayObjectSize={false}
              enableClipboard={false}
              shortenTextAfterLength={0}
            />
          </div>
        </>
      )}

      {status === 'error' && error && (
        <div className="error-box" role="alert">
          <strong>Error: {error.message}</strong>
          <div>Code: {error.code}</div>
          {error.correlationId && <div>CorrelationId: {error.correlationId}</div>}
        </div>
      )}
    </section>
  )
}
