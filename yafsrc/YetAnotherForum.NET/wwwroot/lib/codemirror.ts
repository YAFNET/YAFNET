import { EditorView, basicSetup } from 'codemirror'
import { Compartment } from '@codemirror/state';
import { sql } from '@codemirror/lang-sql'

// Theme
import { oneDark } from '@codemirror/theme-one-dark';

var editor = document.getElementById('Editor') as HTMLTextAreaElement;

const themeConfig = new Compartment();

if (editor && document.documentElement.dataset.bsTheme) {
	if (document.documentElement.dataset.bsTheme === 'dark') {
		editorFromTextArea(editor, [basicSetup, sql(), themeConfig.of(oneDark)]);
	} else {
		editorFromTextArea(editor, [basicSetup, sql()]);
	}
}

function editorFromTextArea(textarea: HTMLTextAreaElement, extensions: any[]) {
	const view = new EditorView({ doc: textarea.value, extensions });
	textarea.parentNode?.insertBefore(view.dom, textarea);
	textarea.style.display = 'none';
	if (textarea.form) textarea.form.addEventListener('submit', () => {
		textarea.value = view.state.doc.toString();
	});
	return view;
}