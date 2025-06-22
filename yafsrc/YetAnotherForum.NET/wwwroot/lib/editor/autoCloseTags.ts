/**
 * Auto Close BBCode tags
 *
 * Original author...
 * @author itsmikita@gmail.com
 */

export default class AutoCloseTags {
	private readonly textarea: HTMLTextAreaElement;
	private readonly autoClosingTags: string[];

	constructor(textarea: HTMLTextAreaElement) {
		this.textarea = textarea;

		this.autoClosingTags = [
			'b', 'i', 'u', 'h', 'code', 'img', 'quote', 'left', 'center', 'right',
			'indent', 'list', 'color', 'size', 'albumimg', 'attach', 'youtube', 'vimeo',
			'instagram', 'twitter', 'facebook', 'googlewidget', 'spoiler', 'userlink', 'googlemaps',
			'hide', 'group-hide', 'hide-thanks', 'hide-reply-thanks', 'hide-reply', 'hide-posts',
			'dailymotion', 'audio', 'media'
		];

		this.enableAutoCloseTags();
	}

	private enableAutoCloseTags(): void {
		this.textarea.addEventListener('keydown', (event: KeyboardEvent) => {
			const keyCode = event.key;

			if (keyCode === ']') {
				const position = this.textarea.selectionStart;
				const before = this.textarea.value.substring(0, position);
				const after = this.textarea.value.substring(this.textarea.selectionEnd);

				let tagName: string | undefined;

				try {
					// @ts-ignore: Object is possibly 'null'.
					tagName = before.match(/\[([^\]]+)$/)[1].match(/^([a-z1-6]+)/)[1];
				} catch(e) {
					return;
				}

				if (!tagName || !this.autoClosingTags.includes(tagName)) {
					return;
				}

				const closeTag = `[/${tagName}]`;
				this.textarea.value = before + closeTag + after;
				this.textarea.selectionStart = this.textarea.selectionEnd = position;
				this.textarea.focus();
			}
		});
	}
}
