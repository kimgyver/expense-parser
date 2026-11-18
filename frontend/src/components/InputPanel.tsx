import { useParseFlow } from '../context/ParseFlowContext'
import { MarkupEditor } from './MarkupEditor'

export const InputPanel = () => {
  const { text, setText, status, loadSample, submit, clear } = useParseFlow()

  return (
    <section className="panel">
      <div className="panel-heading">
        <h2>Input</h2>
      </div>

      <div className="sample-row">
        <span className="sample-label">Samples</span>
        <button type="button" className="ghost" onClick={() => loadSample('success')}>
          Success
        </button>
        <button type="button" className="ghost" onClick={() => loadSample('missingTotal')}>
          No total
        </button>
        <button type="button" className="ghost" onClick={() => loadSample('unbalancedTag')}>
          Unbalanced
        </button>
      </div>

      <MarkupEditor
        value={text}
        onChange={setText}
        placeholder="Paste email text with markup tags…"
      />

      <div className="actions">
        <button
          type="button"
          className="primary"
          onClick={submit}
          disabled={status === 'loading' || text.trim().length === 0}
        >
          {status === 'loading' ? 'Parsing…' : 'Submit'}
        </button>
        <button type="button" className="secondary" onClick={clear}>
          Clear
        </button>
      </div>
    </section>
  )
}
