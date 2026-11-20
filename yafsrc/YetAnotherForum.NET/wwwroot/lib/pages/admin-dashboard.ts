import { Chart, BarController, BarElement, PieController, PolarAreaController, ArcElement, LinearScale, RadarController, RadialLinearScale, CategoryScale, Title, Legend, Tooltip } from 'chart.js';
import { StatsData } from './interfaces/StatsData';

Chart.register(BarController, BarElement, PieController, PolarAreaController, ArcElement, LinearScale, RadarController, RadialLinearScale, CategoryScale, Title, Legend, Tooltip);

const canvasBrowsers = document.getElementById('chart-browsers') as HTMLCanvasElement;
const canvasPlatforms = document.getElementById('chart-platforms') as HTMLCanvasElement;
const canvasCountries = document.getElementById('chart-countries') as HTMLCanvasElement;
const canvasRegistrations = document.getElementById('chart-registrations') as HTMLCanvasElement;

const style = getComputedStyle(document.body);

const cssVar = (name: string) => style.getPropertyValue(name).trim();

// The theme colors, without light and dark (one of them is (nearly) the background color).
const colors = [
	cssVar('--bs-primary'),
	cssVar('--bs-success'),
	cssVar('--bs-info'),
	cssVar('--bs-warning'),
	cssVar('--bs-danger'),
	cssVar('--bs-secondary')
];

// Text and grid lines follow the theme (light and dark mode).
Chart.defaults.color = cssVar('--bs-secondary-color') || Chart.defaults.color;
Chart.defaults.borderColor = cssVar('--bs-border-color-translucent') || Chart.defaults.borderColor;
Chart.defaults.maintainAspectRatio = false;
Chart.defaults.resizeDelay = 200;

const reduceMotion = window.matchMedia('(prefers-reduced-motion: reduce)').matches;

// Sweep the slices open, scaling out from the centre.
const arcAnimation = reduceMotion
	? false as const
	: {
		duration: 1200,
		easing: 'easeOutQuart' as const,
		animateRotate: true,
		animateScale: true
	};

// Grow the bars one after the other, left to right.
const barAnimation = reduceMotion
	? false as const
	: {
		duration: 900,
		easing: 'easeOutQuart' as const,
		delay: (ctx: any) => ctx.type === 'data' && ctx.mode === 'default' ? ctx.dataIndex * 80 : 0
	};

// Hold the chart back until it is scrolled into view, so the animation is not
// spent while the canvas is still below the fold.
function whenVisible(el: Element, create: () => void) {
	if (reduceMotion || !('IntersectionObserver' in window)) {
		create();
		return;
	}

	const chartObserver = new IntersectionObserver((entries, obs) => {
		entries.forEach(entry => {
			if (entry.isIntersecting) {
				obs.unobserve(entry.target);
				create();
			}
		});
	}, { threshold: 0.25 });

	chartObserver.observe(el);
}

function arcChart(canvas: HTMLCanvasElement, type: 'pie' | 'polarArea', stats: StatsData[]) {
	whenVisible(canvas,
		() => {
			new Chart(canvas.getContext('2d') as any,
				{
					type: type,
					data: {
						labels: stats.map(s => s.label),
						datasets: [
							{
								label: canvas.dataset.label,
								data: stats.map(s => s.data),
								backgroundColor: colors,
								// the gaps between the slices in the color of the card (white by default)
								borderColor: getComputedStyle(canvas.closest('.card') ?? document.body).backgroundColor
							}
						]
					},
					options: {
						animation: arcAnimation,
						scales: type === 'polarArea'
							? { r: { ticks: { backdropColor: 'transparent' } } }
							: {},
						plugins: {
							legend: { position: 'bottom' }
						}
					}
				});
		});
}

function barChart(canvas: HTMLCanvasElement, stats: StatsData[]) {
	whenVisible(canvas,
		() => {
			new Chart(canvas.getContext('2d') as any,
				{
					type: 'bar',
					data: {
						labels: stats.map(s => s.label),
						datasets: [
							{
								label: canvas.dataset.label,
								data: stats.map(s => s.data),
								backgroundColor: colors[0],
								borderRadius: 4,
								maxBarThickness: 48
							}
						]
					},
					options: {
						animation: barAnimation,
						scales: {
							x: { grid: { display: false } },
							y: { beginAtZero: true, ticks: { precision: 0 } }
						},
						plugins: {
							legend: { display: false }
						}
					}
				});
		});
}

if (canvasBrowsers && canvasPlatforms && canvasRegistrations) {
	fetch(canvasRegistrations.dataset.url!,
		{
			method: 'GET',
			headers: {
				'RequestVerificationToken': (document.querySelector('input[name="__RequestVerificationToken"]') as
					HTMLInputElement).value
			}
		}).then(res => res.json()).then(data => {
		barChart(canvasRegistrations, data.registrations);
		arcChart(canvasBrowsers, 'polarArea', data.browsers);
		arcChart(canvasPlatforms, 'pie', data.platforms);

		if (canvasCountries) {
			barChart(canvasCountries, data.countries);
		}
	});
}

// Count the key figures up from 0, the number ends with the text formatted by the server (e.g. 1.234 in German).
function animateCount(el: HTMLElement, target: number, duration: number) {
	const text = el.dataset.countText!;
	const startTime = performance.now();

	function update(now: number) {
		const progress = Math.min((now - startTime) / duration, 1);
		const eased = 1 - Math.pow(1 - progress, 3);

		if (progress < 1) {
			el.textContent = Math.floor(target * eased).toLocaleString();
			requestAnimationFrame(update);
		} else {
			el.textContent = text;
		}
	}

	requestAnimationFrame(update);
}

const counters = Array.from(document.querySelectorAll<HTMLElement>('.stats[data-count-target]'))
	.filter(el => parseInt(el.dataset.countTarget!, 10) > 0);

if (!reduceMotion && 'IntersectionObserver' in window) {
	const observer = new IntersectionObserver((entries, obs) => {
		entries.forEach(entry => {
			if (entry.isIntersecting) {
				const el = entry.target as HTMLElement;
				animateCount(el, parseInt(el.dataset.countTarget!, 10), 1200);
				obs.unobserve(el);
			}
		});
	}, { threshold: 0.3 });

	counters.forEach(el => {
		el.dataset.countText = el.textContent!.trim();
		el.textContent = '0';
		observer.observe(el);
	});
}

// The time since a background task was started, updated every second. It is counted from the server time, so a
// wrong clock of the client doesn't change the durations.
const taskList = document.querySelector<HTMLElement>('.task-list[data-server-now]');

function formatDuration(ms: number) {
	const total = Math.max(Math.floor(ms / 1000), 0);
	const days = Math.floor(total / 86400);
	const pad = (n: number) => n.toString().padStart(2, '0');
	const time = `${pad(Math.floor(total % 86400 / 3600))}:${pad(Math.floor(total % 3600 / 60))}:${pad(total % 60)}`;

	return days > 0 ? `${days}.${time}` : time;
}

if (taskList) {
	const durations = taskList.querySelectorAll<HTMLElement>('.task-duration[data-started]');
	const serverNow = Number(taskList.dataset.serverNow);
	const loaded = Date.now();

	window.setInterval(() => {
		const now = serverNow + (Date.now() - loaded);

		durations.forEach(el => {
			el.textContent = formatDuration(now - Number(el.dataset.started));
		});
	}, 1000);
}
