import CodeMirror from '@uiw/react-codemirror'
import { xml } from '@codemirror/lang-xml'
import { EditorView } from '@codemirror/view'
import './MarkupEditor.css'

type MarkupEditorProps = {
  value: string
  onChange: (value: string) => void
  placeholder?: string
}

export const MarkupEditor = ({ value, onChange, placeholder }: MarkupEditorProps) => {
  return (
    <div className="editor-shell">
      <CodeMirror
        value={value}
        height="100%"
        placeholder={placeholder}
        extensions={[xml(), EditorView.lineWrapping]}
        onChange={onChange}
        basicSetup={{
          lineNumbers: false,
          foldGutter: false,
          highlightActiveLine: true,
          highlightSelectionMatches: true,
        }}
      />
    </div>
  )
}
