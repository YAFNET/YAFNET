import '@popperjs/core';
import 'bootstrap';

// Custom JS imports
import { MdsPersianDateTimePicker, MdsPersianDateTimePickerSetting } from '../../../node_modules/md.bootstrappersiandatetimepicker/src/mds.bs.datetimepicker';

import '../../../node_modules/md.bootstrappersiandatetimepicker/dist/mds.datetimepicker.min.css';

var input = document.getElementById('Input_Birthday') as HTMLInputElement;

if (input) {
	input.setAttribute('type', 'text');

	var settings = new MdsPersianDateTimePickerSetting();

	settings.targetTextSelector = '#Input_Birthday';
	settings.selectedDate = new Date(input.value);
	settings.selectedDateToShow = new Date(input.value);

	const picker = new MdsPersianDateTimePicker(input, settings);
}