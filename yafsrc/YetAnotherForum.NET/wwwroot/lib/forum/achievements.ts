import * as bootstrap from 'bootstrap';

/**
 * An unlocked achievement (see AchievementInfo, the json of api/Achievement/Unseen).
 */
interface AchievementInfo {
	id: number;
	name: string;
	description: string;
	icon: string;
	points: number;
	rarityName: string;
	rarityClass: string;
	percent: string;
	featured: boolean;
}

/**
 * The anti forgery token of the page (the dialog has its own).
 */
function requestToken(): string {
	return (document.querySelector('input[name="__RequestVerificationToken"]') as HTMLInputElement | null)?.value ?? '';
}

function post(url: string, keepalive = false): Promise<Response> {
	return fetch(url, {
		method: 'POST',
		headers: { 'RequestVerificationToken': requestToken(), 'Accept': 'application/json' },
		credentials: 'same-origin',
		keepalive: keepalive
	});
}

function format(template: string, ...values: (string | number)[]): string {
	return values.reduce<string>((text, value, index) => text.replace(`{${index}}`, String(value)), template);
}

/**
 * Shows or hides an achievement in the profile and with the posts, returns the error message if the user shows
 * the most achievements already.
 */
async function setFeatured(baseUrl: string, id: number, featured: boolean): Promise<string | null> {
	try {
		const response = await post(`${baseUrl}/Feature/${id}/${featured}`);

		if (response.ok) {
			return null;
		}

		const result = await response.json().catch(() => ({}));

		return result.message ?? response.statusText;
	} catch {
		return 'Error';
	}
}

/**
 * The "achievement unlocked" dialog: the trophy pops in with rays, glow and sparks.
 */
class AchievementDialog {
	private readonly modal: HTMLElement;
	private readonly baseUrl: string;
	private queue: AchievementInfo[] = [];
	private index = 0;
	private loading = false;

	constructor(modal: HTMLElement) {
		this.modal = modal;
		this.baseUrl = modal.dataset.url ?? '/api/Achievement';

		this.element('AchievementUnlockedNext').addEventListener('click', () => this.next());
		this.element('AchievementUnlockedFeature').addEventListener('click', () => this.toggleFeatured());

		// the achievements the user has not looked at are seen too, they are on the page of the achievements
		this.element('AchievementUnlockedAll').addEventListener('click', () => this.markAllSeen());

		// closing the dialog dismisses all new achievements (the counter tells there are more)
		modal.addEventListener('hidden.bs.modal', () => {
			this.markAllSeen();
			this.element('AchievementUnlockedStage').replaceChildren();
		});
	}

	/**
	 * Loads the new achievements and shows them (nothing if the dialog is open).
	 */
	async load(): Promise<void> {
		if (this.loading || this.queue.length > 0) {
			return;
		}

		this.loading = true;

		try {
			const response = await fetch(`${this.baseUrl}/Unseen`, {
				headers: { 'Accept': 'application/json' },
				credentials: 'same-origin'
			});

			if (!response.ok) {
				return;
			}

			const achievements = await response.json() as AchievementInfo[];

			if (achievements.length === 0) {
				return;
			}

			this.queue = achievements;
			this.index = 0;
			this.render();

			bootstrap.Modal.getOrCreateInstance(this.modal).show();
		} catch {
			// no dialog without a connection, the achievements are shown on the next page
		} finally {
			this.loading = false;
		}
	}

	private element(id: string): HTMLElement {
		return this.modal.querySelector<HTMLElement>(`#${id}`)!;
	}

	private get current(): AchievementInfo {
		return this.queue[this.index];
	}

	private next(): void {
		if (this.index < this.queue.length - 1) {
			// the user has seen this one
			this.markSeen(this.current.id);

			this.index++;
			this.render();
		}
	}

	private render(): void {
		const achievement = this.current;
		const data = this.modal.dataset;

		// the colors of the rarity
		const content = this.modal.querySelector<HTMLElement>('.modal-content')!;
		content.className = `modal-content ${achievement.rarityClass}`;

		this.renderStage(achievement);

		this.element('AchievementUnlockedName').textContent = achievement.name;

		const description = this.element('AchievementUnlockedDescription');
		description.textContent = achievement.description;
		description.classList.toggle('d-none', !achievement.description);

		this.element('AchievementUnlockedPoints').textContent =
			format((achievement.points === 1 ? data.textPoint : data.textPoints) ?? '+{0}', achievement.points);
		this.element('AchievementUnlockedRarity').textContent = achievement.rarityName;
		this.element('AchievementUnlockedPercent').textContent = format(data.textPercent ?? '{0}', achievement.percent);

		this.renderFeatured();

		this.element('AchievementUnlockedMessage').classList.add('d-none');

		// the counter and the next button when there are more new achievements
		const pager = this.element('AchievementUnlockedPager');
		pager.classList.toggle('d-none', this.queue.length < 2);
		this.element('AchievementUnlockedCounter').textContent =
			format(data.textCounter ?? '{0} / {1}', this.index + 1, this.queue.length);
		(this.element('AchievementUnlockedNext') as HTMLButtonElement).disabled = this.index >= this.queue.length - 1;

		// an achievement is seen when the user acts (next, close, see all): the dialog can be shown on a page that is
		// just left (e.g. the live message of the post that is sent), the next page shows it again
	}

	/**
	 * Builds the trophy with the rays, the glow and the sparks again (the animations start again).
	 */
	private renderStage(achievement: AchievementInfo): void {
		const stage = this.element('AchievementUnlockedStage');

		const rays = document.createElement('div');
		rays.className = 'achievement-rays';

		const glow = document.createElement('div');
		glow.className = 'achievement-glow';

		const trophy = document.createElement('span');
		trophy.className = `achievement-trophy achievement-trophy-lg ${achievement.rarityClass}`;
		trophy.setAttribute('aria-hidden', 'true');

		const icon = document.createElement('i');
		icon.className = `fas fa-${achievement.icon}`;
		trophy.appendChild(icon);

		const sparks = document.createElement('div');
		sparks.className = 'achievement-sparks';

		const count = 16;

		for (let i = 0; i < count; i++) {
			const spark = document.createElement('span');
			const angle = (Math.PI * 2 * i) / count + Math.random() * 0.4;
			const distance = 5.5 + Math.random() * 2.5;

			spark.className = 'achievement-spark';
			spark.style.setProperty('--spark-x', `${(Math.cos(angle) * distance).toFixed(2)}rem`);
			spark.style.setProperty('--spark-y', `${(Math.sin(angle) * distance).toFixed(2)}rem`);
			spark.style.animationDelay = `${(0.35 + Math.random() * 0.25).toFixed(2)}s`;

			sparks.appendChild(spark);
		}

		stage.replaceChildren(rays, glow, trophy, sparks);
	}

	private renderFeatured(): void {
		const button = this.element('AchievementUnlockedFeature');
		const featured = this.current.featured;

		button.classList.toggle('active', featured);
		button.setAttribute('aria-pressed', String(featured));
		button.querySelector('i')!.className = `fas fa-${featured ? 'check' : 'thumbtack'} me-1`;
		button.querySelector('span')!.textContent =
			(featured ? this.modal.dataset.textFeatured : this.modal.dataset.textFeature) ?? '';
	}

	private async toggleFeatured(): Promise<void> {
		const achievement = this.current;
		const message = this.element('AchievementUnlockedMessage');

		const error = await setFeatured(this.baseUrl, achievement.id, !achievement.featured);

		if (error) {
			message.textContent = error;
			message.classList.remove('d-none');
			return;
		}

		message.classList.add('d-none');
		achievement.featured = !achievement.featured;
		this.renderFeatured();
	}

	private markSeen(id: number): void {
		post(`${this.baseUrl}/Seen/${id}`).catch(() => { /* shown again on the next page */ });
	}

	private markAllSeen(): void {
		// the ones before the current one are marked by "next" already
		this.queue.slice(this.index).forEach(achievement => {
			post(`${this.baseUrl}/Seen/${achievement.id}`, true).catch(() => { /* shown again on the next page */ });
		});

		this.queue = [];
		this.index = 0;
	}
}

const _global = (window /* browser */ || global /* node */) as any;

document.addEventListener('DOMContentLoaded', () => {
	const modal = document.getElementById('AchievementUnlockedModal');

	if (modal) {
		const dialog = new AchievementDialog(modal);

		// the notification hub tells about achievements that are unlocked while the page is open
		_global.yafAchievementUnlocked = () => dialog.load();

		if (modal.dataset.unseen === 'true') {
			dialog.load();
		}
	}

	// the switches of the unlocked achievements (my achievements): show the achievement in the profile
	document.querySelectorAll<HTMLInputElement>('input[data-achievement-feature]').forEach(input => {
		input.addEventListener('change', async () => {
			const id = parseInt(input.dataset.achievementFeature ?? '0', 10);
			const baseUrl = modal?.dataset.url ?? '/api/Achievement';

			input.disabled = true;

			const error = await setFeatured(baseUrl, id, input.checked);

			input.disabled = false;

			if (error) {
				input.checked = !input.checked;

				const Notify = _global.Notify;

				if (Notify) {
					const _ = new Notify({ message: error, icon: 'fa fa-exclamation-triangle' },
						{ type: 'warning', element: 'body', placement: { from: 'top', align: 'center' }, delay: 5000 });
				}

				return;
			}

			// the showcase of the page shows the trophy
			const showcase = document.getElementById('AchievementShowcase');

			if (!showcase) {
				return;
			}

			showcase.querySelector(`[data-achievement-showcase="${id}"]`)?.remove();

			if (input.checked) {
				const card = input.closest('.achievement-card');
				const trophy = card?.querySelector('.achievement-trophy')?.cloneNode(true) as HTMLElement | undefined;

				if (trophy) {
					trophy.classList.add('achievement-trophy-sm');

					const item = document.createElement('span');
					item.dataset.achievementShowcase = String(id);
					item.title = card?.querySelector('.achievement-name')?.textContent ?? '';
					item.appendChild(trophy);

					showcase.appendChild(item);
				}
			}
		});
	});
});
