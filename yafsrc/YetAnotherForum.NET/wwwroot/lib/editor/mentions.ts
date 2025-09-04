import IMentionOptions from './interfaces/IMentionOptions';
import IMentionItem from './interfaces/IMentionItem';

export default function Mentions(opts: IMentionOptions = {}) {
    opts = Object.assign({
        lookup: 'lookup',
        id: '',
        selector: '',
        element: null,
        symbol: '@',
        items: [],
        item_template:
            '<a class="dropdown-item" href="#"><img src="${item.avatar}" alt="avatar" class="me-2 img-thumbnail" style="max-width:20px;max-height:20px" /> ${item.name}</a>',
        onclick: undefined,
        url: ''
    }, opts);

    const $e: HTMLElement | null =
        (opts.id && document.getElementById(opts.id)) ||
        (opts.selector && document.querySelector(opts.selector)) as HTMLElement ||
        opts.element;

    if (!$e)
        return console.error('Invalid element selector', opts);

    const $lookup = document.createElement('div');
    $lookup.classList.value = `fixed-top autohide ${opts.lookup}`;
    $e.parentNode?.insertBefore($lookup, $e.nextSibling);

    $e.addEventListener('keydown', processKey);
    $e.addEventListener('keyup', showLookup);
    $e.addEventListener('click', hideLookup);

    let start = 0, end = 0, prevWord = '';

    function showLookup(event: KeyboardEvent) {
        if (event.code === 'Escape') {
            return hideLookup();
        }

        const sel = window.getSelection();
        const $parent = $lookup.parentNode?.querySelector('textarea') as HTMLTextAreaElement;

        if (!sel?.anchorNode || !$parent) return;

        const text = $parent.value || '';
        const curr = $parent.selectionStart;

        const getLength = (arr: RegExpMatchArray | null): number =>
            Array.isArray(arr) && arr.length > 0 ? arr[0].length : 0;

        start = curr - getLength(text.slice(0, curr).match(/[\S]+$/g));
        end = curr + getLength(text.slice(curr).match(/^[\S]+/g));

        const word = text.substring(start, end);

        if (!word || word[0] !== opts.symbol) {
            prevWord = '';
            return hideLookup();
        }

        if (word === prevWord) return;
        prevWord = word;

        const rect = $parent.getBoundingClientRect();
        $lookup.style.left = `${rect.left + 20}px`;
        $lookup.style.top = `${rect.top + 30}px`;

        const query = word.slice(1);

        if (query.length >= 3 && opts.url) {
            const token = (document.querySelector('input[name="__RequestVerificationToken"]') as HTMLInputElement)?.value;

            fetch(opts.url.replace('{q}', encodeURIComponent(query)), {
                method: 'GET',
                headers: {
                    Accept: 'application/json',
                    'Content-Type': 'application/json;charset=utf-8',
                    RequestVerificationToken: token
                }
            })
                .then(response => response.json())
                .then((data: IMentionItem[]) => opts.items = data);
        }

        const items = (opts.items || [])
            .filter(e => e.name.toLowerCase().includes(query.toLowerCase()))
            .map(item => eval(
                '`<li class="mention-li-nt ${opts.lookup}" data-name="${item.name}" data-id="${item.id}">' +
                opts.item_template +
                '</li>`'));

        if (!items.length) return hideLookup();

        $lookup.innerHTML = `<ul class="dropdown-menu dropdown-mentions show">${items.join('')}</ul>`;
        [...$lookup.firstElementChild!.children].forEach(el => {
            el.addEventListener('click', onClick);
            el.addEventListener('mouseenter', onHover);
        });
        $lookup.firstElementChild!.children[0].classList.add('active');

        if ($lookup.hasAttribute('hidden')) $lookup.removeAttribute('hidden');

        const bounding = $lookup.getBoundingClientRect();
        if (
            bounding.bottom > window.innerHeight ||
            bounding.right > window.innerWidth
        ) {
            $lookup.style.top = `${parseInt($lookup.style.top) - 10 - $lookup.clientHeight}px`;
        }
    }

    function hideLookup() {
        if (!$lookup.hasAttribute('hidden'))
            $lookup.setAttribute('hidden', 'true');
    }

    function onClick(event: Event) {
        const el = (event.target as HTMLElement).closest('.mention-li-nt') as HTMLElement;
        if (!el) return;

        opts.onclick?.(el.dataset);

        const $parent = $lookup.parentNode?.querySelector('textarea') as HTMLTextAreaElement;
        $parent.value = $parent.value.replace($parent.value.substring(start + 1, end), '');

        hideLookup();
    }

    function onHover(event: Event) {
        const target = (event.target as HTMLElement).closest('.mention-li-nt') as HTMLElement;
        if (!target || target.classList.contains('active')) return;

        [...$lookup.firstElementChild!.children]
            .forEach(el => el.classList.remove('active'));
        target.classList.add('active');
    }

    function processKey(event: KeyboardEvent) {
        const code = event.key;
        if (!['ArrowUp', 'ArrowDown', 'Enter'].includes(code) || $lookup.hasAttribute('hidden')) return;

        event.preventDefault();

        if (code === 'Enter') {
            const active = $lookup.querySelector('.active') as HTMLElement;
            active?.click();
            return;
        }

        const $children = [...$lookup.firstElementChild!.children] as HTMLElement[];
        const curr = $children.findIndex(el => el.classList.contains('active'));

        $children[curr].classList.remove('active');

        const nextIdx = (curr + (code === 'ArrowUp' ? -1 : 1) + $children.length) % $children.length;
        const $next = $children[nextIdx];
        $next.classList.add('active');
        $next.scrollIntoView({ block: 'nearest' });
    }
}
