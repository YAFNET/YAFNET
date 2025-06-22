import IMentionItem from './IMentionItem';
export default interface IMentionOptions {
	lookup?: string;
	id?: string;
	selector?: string;
	element?: HTMLElement | null;
	symbol?: string;
	items?: IMentionItem[];
	item_template?: string;
	onclick?: ((data: DOMStringMap) => void) | undefined;
	url?: string;
}