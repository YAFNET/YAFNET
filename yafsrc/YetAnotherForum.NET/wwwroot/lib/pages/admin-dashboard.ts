import { Chart, BarController, BarElement, PieController, PolarAreaController, ArcElement, LinearScale, RadarController, RadialLinearScale, CategoryScale, Title, Legend, Tooltip } from 'chart.js';
import { StatsData } from './interfaces/StatsData';

Chart.register(BarController, BarElement, PieController, PolarAreaController, ArcElement, LinearScale, RadarController, RadialLinearScale, CategoryScale, Title, Legend, Tooltip);

const canvasBrowsers = document.getElementById('chart-browsers') as HTMLCanvasElement;
const canvasPlatforms = document.getElementById('chart-platforms') as HTMLCanvasElement;
const canvasCountries = document.getElementById('chart-countries') as HTMLCanvasElement;
const canvasRegistrations = document.getElementById('chart-registrations') as HTMLCanvasElement;

const labelsBrowsers = new Array();
const labelsPlatforms = new Array();
const labelsCountries = new Array();
const labelsRegistrations = new Array();

const dataBrowsers = new Array();
const dataPlatforms = new Array();
const dataCountries = new Array();
const dataRegistrations = new Array();

const style = getComputedStyle(document.body);

const colors = [
	style.getPropertyValue('--bs-primary'),
	style.getPropertyValue('--bs-secondary'),
	style.getPropertyValue('--bs-success'),
	style.getPropertyValue('--bs-info'),
	style.getPropertyValue('--bs-warning'),
	style.getPropertyValue('--bs-danger'),
	style.getPropertyValue('--bs-light'),
	style.getPropertyValue('--bs-dark')
];

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

if (canvasBrowsers && canvasPlatforms && canvasRegistrations) {
	var url = canvasBrowsers.dataset.url!;

	fetch(url,
		{
			method: 'GET',
			headers: {
				'RequestVerificationToken': (document.querySelector('input[name="__RequestVerificationToken"]') as
					HTMLInputElement).value
			}
		}).then(res => res.json()).then(data => {

		data.browsers.forEach((stats: StatsData) => {
			labelsBrowsers.push(stats.label);
			dataBrowsers.push(stats.data);
		});

		data.platforms.forEach((stats: StatsData) => {
			labelsPlatforms.push(stats.label);
			dataPlatforms.push(stats.data);
		});

		data.countries.forEach((stats: StatsData) => {
			labelsCountries.push(stats.label);
			dataCountries.push(stats.data);
		});

		data.registrations.forEach((stats: StatsData) => {
			labelsRegistrations.push(stats.label);
			dataRegistrations.push(stats.data);
		});

		whenVisible(canvasBrowsers,
			() => {
				const ctxBrowsers = canvasBrowsers.getContext('2d') as any;

				new Chart(ctxBrowsers,
					{
						type: 'polarArea',
						data: {
							labels: labelsBrowsers,
							datasets: [
								{
									label: canvasBrowsers.dataset.label,
									data: dataBrowsers,
									backgroundColor: colors
								}
							]
						},
						options: {
							animation: arcAnimation,
							resizeDelay: 1500,
							plugins: {
								title: {
									display: true,
									text: canvasBrowsers.dataset.title
								}
							}
						}
					});
			});

		whenVisible(canvasPlatforms,
			() => {
				const ctxPlatforms = canvasPlatforms.getContext('2d') as any;

				new Chart(ctxPlatforms,
					{
						type: 'pie',
						data: {
							labels: labelsPlatforms,
							datasets: [
								{
									label: canvasPlatforms.dataset.label,
									data: dataPlatforms,
									backgroundColor: colors
								}
							]
						},
						options: {
							animation: arcAnimation,
							resizeDelay: 1500,
							plugins: {
								title: {
									display: true,
									text: canvasPlatforms.dataset.title
								}
							}
						}
					});
			});

		if (canvasCountries) {
			whenVisible(canvasCountries,
				() => {
					const ctxCountries = canvasCountries.getContext('2d') as any;

					new Chart(ctxCountries,
						{
							type: 'bar',
							data: {
								labels: labelsCountries,
								datasets: [
									{
										label: canvasCountries.dataset.label,
										data: dataCountries,
										backgroundColor: colors
									}
								]
							},
							options: {
								animation: barAnimation,
								resizeDelay: 1500,
								plugins: {
									title: {
										display: true,
										text: canvasCountries.dataset.title
									}
								}
							}
						});
				});
		}

		whenVisible(canvasRegistrations,
			() => {
				const ctxRegistrations = canvasRegistrations.getContext('2d') as any;

				new Chart(ctxRegistrations,
					{
						type: 'bar',
						data: {
							labels: labelsRegistrations,
							datasets: [
								{
									label: canvasRegistrations.dataset.label,
									data: dataRegistrations,
									backgroundColor: colors
								}
							]
						},
						options: {
							animation: barAnimation,
							resizeDelay: 1500,
							plugins: {
								title: {
									display: true,
									text: canvasRegistrations.dataset.title
								}
							}
						}
					});
			});
	});
}

function animateCount(el: Element, target: number, duration: number) {
	const start = 0;
	const startTime = performance.now();
	function update(now: number) {
		const elapsed = now - startTime;
		const progress = Math.min(elapsed / duration, 1);
		const eased = 1 - Math.pow(1 - progress, 3);
		const current = Math.floor(start + (target - start) * eased);
		el.textContent = current.toLocaleString();
		if (progress < 1) requestAnimationFrame(update);
		else el.textContent = target.toLocaleString();
	}
	requestAnimationFrame(update);
}


document.querySelectorAll<HTMLElement>('.stats[data-count-target]').forEach(el => {
	const target = parseInt(el.dataset.countTarget!, 10);
	if (target > 0) el.textContent = '0';
});

const observer = new IntersectionObserver((entries, obs) => {
	entries.forEach(entry => {
		if (entry.isIntersecting) {
			const el = entry.target as HTMLElement;
			const target = parseInt(el.dataset.countTarget!, 10);
			if (target > 0) animateCount(el, target, 1200);
			obs.unobserve(el);
		}
	});
}, { threshold: 0.3 });

document.querySelectorAll<HTMLElement>('.stats[data-count-target]').forEach(el => {
	const target = parseInt(el.dataset.countTarget!, 10);
	if (target > 0) {
		observer.observe(el);
	}
});
