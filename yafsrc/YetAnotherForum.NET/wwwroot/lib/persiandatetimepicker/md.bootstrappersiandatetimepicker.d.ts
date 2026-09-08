declare module 'md.bootstrappersiandatetimepicker' {
	export class MdsPersianDateTimePickerSetting {
		targetTextSelector: string;
		selectedDate: Date;
		selectedDateToShow: Date;
	}

	export class MdsPersianDateTimePicker {
		constructor(element: HTMLInputElement, setting: MdsPersianDateTimePickerSetting);
	}
}
