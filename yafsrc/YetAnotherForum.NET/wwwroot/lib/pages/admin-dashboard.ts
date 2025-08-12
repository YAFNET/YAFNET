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

		const ctxBrowsers = canvasBrowsers.getContext('2d') as any;

		const chartBrowsers = new Chart(ctxBrowsers,
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
					plugins: {
						title: {
							display: true,
							text: canvasBrowsers.dataset.title
						}
					}
				}
			});

		const ctxPlatforms = canvasPlatforms.getContext('2d') as any;

		const chartPlatforms = new Chart(ctxPlatforms,
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
					plugins: {
						title: {
							display: true,
							text: canvasPlatforms.dataset.title
						}
					}
				}
			});

		if (canvasCountries) {
			const ctxCountries = canvasCountries.getContext('2d') as any;

			const chartCountries = new Chart(ctxCountries,
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
						plugins: {
							title: {
								display: true,
								text: canvasCountries.dataset.title
							}
						}
					}
				});
		}


		const ctxRegistrations = canvasRegistrations.getContext('2d') as any;

		const chartRegistrations = new Chart(ctxRegistrations,
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
					plugins: {
						title: {
							display: true,
							text: canvasRegistrations.dataset.title
						}
					}
				}
			});
	});
}