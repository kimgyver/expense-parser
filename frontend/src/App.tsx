import { InputPanel } from './components/InputPanel'
import { ResultPanel } from './components/ResultPanel'
import { ParseFlowProvider } from './context/ParseFlowContext'
import './App.css'

const App = () => {
  return (
    <ParseFlowProvider>
      <div className="page">
        <header className="header">
          <h1>Expense Email Parser</h1>
          <p>Paste marked-up email text. Submit parses via the API.</p>
        </header>

        <main className="layout">
          <InputPanel />
          <ResultPanel />
        </main>
      </div>
    </ParseFlowProvider>
  )
}

export default App
