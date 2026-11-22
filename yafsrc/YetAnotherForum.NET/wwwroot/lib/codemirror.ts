import { EditorView, basicSetup } from 'codemirror'
import { Compartment, Prec } from '@codemirror/state';
import { keymap } from '@codemirror/view';
import { sql } from '@codemirror/lang-sql'

// Theme
import { oneDark } from '@codemirror/theme-one-dark';

var editor = document.getElementById('Editor') as HTMLTextAreaElement;

const themeConfig = new Compartment();

// Ctrl+Enter (Cmd+Enter) runs the query: the form is sent with the button data-run-query
const runQuery = Prec.highest(keymap.of([{
	key: 'Mod-Enter',
	run: () => {
		const button = editor?.form?.querySelector<HTMLButtonElement>('[data-run-query]');

		if (!button) {
			return false;
		}

		editor.form?.requestSubmit(button);

		return true;
	}
}]));

if (editor && document.documentElement.dataset.bsTheme) {
	if (document.documentElement.dataset.bsTheme === 'dark') {
		editorFromTextArea(editor, [basicSetup, sql(), runQuery, themeConfig.of(oneDark)]);
	} else {
		editorFromTextArea(editor, [basicSetup, sql(), runQuery]);
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