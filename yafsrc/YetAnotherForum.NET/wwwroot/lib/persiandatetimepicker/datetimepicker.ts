﻿import { Modal, Popover } from "bootstrap";

export class MdsPersianDateTimePicker {
  constructor(element: Element, setting: MdsPersianDateTimePickerSetting) {
    setting = MdsPersianDateTimePicker.extend(new MdsPersianDateTimePickerSetting(), setting);
    if (!element) throw new Error(`MdsPersianDateTimePicker => element is null!`);
    if (setting.rangeSelector && (setting.toDate || setting.fromDate)) throw new Error(`MdsPersianDateTimePicker => You can not set true 'toDate' or 'fromDate' and 'rangeSelector' together`);
    if (setting.toDate && setting.fromDate) throw new Error(`MdsPersianDateTimePicker => You can not set true 'toDate' and 'fromDate' together`);
    if (!setting.groupId && (setting.toDate || setting.fromDate)) throw new Error(`MdsPersianDateTimePicker => When you set 'toDate' or 'fromDate' true, you have to set 'groupId'`);

    if (!setting.textFormat) {
      setting.textFormat = 'yyyy/MM/dd';
      if (setting.enableTimePicker)
        setting.textFormat += ' HH:mm';
    }
    if (!setting.dateFormat) {
      setting.dateFormat = 'yyyy/MM/dd';
      if (setting.enableTimePicker)
        setting.dateFormat += ' HH:mm';
    }
    if (setting.yearOffset > 15)
      setting.yearOffset = 15;

    this.setting = setting;
    this.setting.selectedDate = setting.selectedDate ? MdsPersianDateTimePicker.getClonedDate(setting.selectedDate) : null;
    this.setting.selectedDateToShow = MdsPersianDateTimePicker.getClonedDate(setting.selectedDateToShow) ?? new Date();

    this.guid = MdsPersianDateTimePicker.newGuid();
    this.element = element;
    this.element.setAttribute("data-mds-dtp-guid", this.guid);
    MdsPersianDateTimePickerData.set(this.guid, this);

    this.initializeBsPopover(setting);
  }

  // #region jalali calendar

  private static toJalali(gy: number, gm: number, gd: number): JalaliJsonModel {
    return this.d2j(this.g2d(gy, gm, gd));
  }
  private static toGregorian(jy: number, jm: number, jd: number): GregorianJsonModel {
    return this.d2g(this.j2d(jy, jm, jd));
  }
  // private static isValidJalaliDate(jy: number, jm: number, jd: number): boolean {
  //   return jy >= -61 && jy <= 3177 &&
  //     jm >= 1 && jm <= 12 &&
  //     jd >= 1 && jd <= this.jalaliMonthLength(jy, jm);
  // }
  private static isLeapJalaliYear(jy: number): boolean {
    return this.jalCal(jy).leap === 0;
  }
  // private static jalaliMonthLength(jy: number, jm: number): number {
  //   if (jm <= 6) return 31;
  //   if (jm <= 11) return 30;
  //   if (this.isLeapJalaliYear(jy)) return 30;
  //   return 29;
  // }
  private static jalCal(jy: number): JalCalModel {
    // Jalali years starting the 33-year rule.
    let breaks = [-61, 9, 38, 199, 426, 686, 756, 818, 1111, 1181, 1210, 1635, 2060, 2097, 2192, 2262, 2324, 2394, 2456, 3178],
      bl = breaks.length,
      gy = jy + 621,
      leapJ = -14,
      jp = breaks[0],
      jm,
      jump = 1,
      leap,
      n,
      i;

    if (jy < jp || jy >= breaks[bl - 1])
      throw new Error('Invalid Jalali year ' + jy);

    // Find the limiting years for the Jalali year jy.
    for (i = 1; i < bl; i += 1) {
      jm = breaks[i];
      jump = jm - jp;
      if (jy < jm)
        break;
      leapJ = leapJ + this.div(jump, 33) * 8 + this.div(this.mod(jump, 33), 4);
      jp = jm;
    }
    n = jy - jp;

    // Find the number of leap years from AD 621 to the beginning
    // of the current Jalali year in the Persian calendar.
    leapJ = leapJ + this.div(n, 33) * 8 + this.div(this.mod(n, 33) + 3, 4);
    if (this.mod(jump, 33) === 4 && jump - n === 4)
      leapJ += 1;

    // And the same in the Gregorian calendar (until the year gy).
    let leapG = this.div(gy, 4) - this.div((this.div(gy, 100) + 1) * 3, 4) - 150;

    // Determine the Gregorian date of Farvardin the 1st.
    let march = 20 + leapJ - leapG;

    // Find how many years have passed since the last leap year.
    if (jump - n < 6)
      n = n - jump + this.div(jump + 4, 33) * 33;
    leap = this.mod(this.mod(n + 1, 33) - 1, 4);
    if (leap === -1) leap = 4;

    return {
      leap: leap,
      gy: gy,
      march: march
    };
  }
  private static j2d(jy: number, jm: number, jd: number): number {
    let r = this.jalCal(jy);
    return this.g2d(r.gy, 3, r.march) + (jm - 1) * 31 - this.div(jm, 7) * (jm - 7) + jd - 1;
  }
  private static d2j(jdn: number): JalaliJsonModel {
    let gy = this.d2g(jdn).gy, // Calculate Gregorian year (gy).
      jy = gy - 621,
      r = this.jalCal(jy),
      jdn1F = this.g2d(gy, 3, r.march),
      jd,
      jm,
      k;

    // Find number of days that passed since 1 Farvardin.
    k = jdn - jdn1F;
    if (k >= 0) {
      if (k <= 185) {
        // The first 6 months.
        jm = 1 + this.div(k, 31);
        jd = this.mod(k, 31) + 1;
        return {
          jy: jy,
          jm: jm,
          jd: jd
        };
      } else {
        // The remaining months.
        k -= 186;
      }
    } else {
      // Previous Jalali year.
      jy -= 1;
      k += 179;
      if (r.leap === 1)
        k += 1;
    }
    jm = 7 + this.div(k, 30);
    jd = this.mod(k, 30) + 1;
    return {
      jy: jy,
      jm: jm,
      jd: jd
    };
  }
  private static g2d(gy: number, gm: number, gd: number): number {
    let d = this.div((gy + this.div(gm - 8, 6) + 100100) * 1461, 4) +
      this.div(153 * this.mod(gm + 9, 12) + 2, 5) +
      gd - 34840408;
    d = d - this.div(this.div(gy + 100100 + this.div(gm - 8, 6), 100) * 3, 4) + 752;
    return d;
  }
  private static d2g(jdn: number): GregorianJsonModel {
    let j;
    j = 4 * jdn + 139361631;
    j = j + this.div(this.div(4 * jdn + 183187720, 146097) * 3, 4) * 4 - 3908;
    let i = this.div(this.mod(j, 1461), 4) * 5 + 308;
    let gd = this.div(this.mod(i, 153), 5) + 1;
    let gm = this.mod(this.div(i, 153), 12) + 1;
    let gy = this.div(j, 1461) - 100100 + this.div(8 - gm, 6);
    return {
      gy: gy,
      gm: gm,
      gd: gd
    };
  }
  private static div(a: number, b: number): number {
    return ~~(a / b);
  }
  private static mod(a: number, b: number): number {
    return a - ~~(a / b) * b;
  }

  //#endregion jalali calendar

  // #region Template

  private static modalHtmlTemplate =
    `<div data-mds-dtp data-mds-dtp-guid="{{guid}}" class="modal fade mds-bs-persian-datetime-picker-modal" tabindex="-1" role="dialog" aria-hidden="true">
  <div class="modal-dialog">
	  <div class="modal-content">
      <div class="modal-header" data-mds-dtp-title="true">
        <h5 class="modal-title">Modal title</h5>
      </div>
      <div class="modal-body">
        <div class="select-year-box w-0" data-mds-dtp-year-list-box="true"></div>
        <div data-name="mds-dtp-body"></div>
      </div>
    </div>
  </div>
</div>`;
  private static popoverHtmlTemplate = `<div class="popover mds-bs-persian-datetime-picker-popover" role="tooltip" data-mds-dtp>
<div class="popover-arrow"></div>
<h3 class="popover-header text-center p-1" data-mds-dtp-title="true"></h3>
<div class="popover-body p-0" data-name="mds-dtp-body"></div>
</div>`;
  private static popoverHeaderSelectYearHtmlTemplate = `<table class="table table-sm table-borderless text-center p-0 m-0 {{rtlCssClass}}" dir="{{dirAttrValue}}">
<tr>
<th>
<button type="button" class="btn btn-sm btn-light w-100" title="{{previousText}}" data-year="{{latestPreviousYear}}" data-year-range-button-change="-1" {{prevYearButtonAttr}}> &lt; </button>
</th>
<th class="pt-1">
{{yearsRangeText}}
</th>
<th>
<button type="button" class="btn btn-sm btn-light w-100" title="{{nextText}}" data-year="{{latestNextYear}}" data-year-range-button-change="1" {{nextYearButtonAttr}}> &gt; </button>
</th>
</tr>
</table>`;
  private static dateTimePickerYearsToSelectHtmlTemplate = `<table class="table table-sm text-center p-0 m-0">
<tbody>
{{yearsBoxHtml}}
<tr>
<td colspan="100" class="text-center">
<button class="btn btn-sm btn-light w-100" data-mds-hide-year-list-box="true">{{cancelText}}</button>
</td>
</tr>
</tbody>
</table>`;

  private static dateTimePickerHtmlTemplate = `<div class="mds-bs-dtp-container {{rtlCssClass}}" {{inlineAttr}}>
<div class="select-year-inline-box w-0" data-name="dtp-years-container">
</div>
<div class="select-year-box w-0" data-mds-dtp-year-list-box="true"></div>
<table class="table table-sm text-center p-0 m-0">
<thead>
<tr {{selectedDateStringAttribute}}>
<th mds-dtp-inline-header colspan="100">{{dtpInlineHeader}}</th>
</tr>
</thead>
<tbody>
<tr>
{{monthsTdHtml}}
</tr>
</tbody>
<tfoot>
<tr {{timePickerAttribute}}>
<td colspan="100" class="text-center border-0">
<input type="time" value="{{time}}" maxlength="2" data-mds-dtp-time />
</td>
</tr>
<tr>
<td colspan="100">
<button type="button" class="btn btn-light" title="{{goTodayText}}" data-mds-dtp-go-today>{{todayDateString}}</button>
</td>
</tr>
</tfoot>
</table>
</div>`;

  private static dateTimePickerMonthTableHtmlTemplate = `<td class="border-0" style="{{monthTdStyle}}" {{monthTdAttribute}} data-td-month>
<table class="table table-sm table-striped table-borderless">
<thead>
<tr {{monthNameAttribute}}>
<th colspan="100" class="border-0">
<table class="table table-sm table-borderless">
<thead>
<tr>
<th>
<button type="button" class="btn btn-light"> {{currentMonthInfo}} </button>
</th>
</tr>
</thead>
</table>
</th>
</tr>
<tr {{theadSelectDateButtonTrAttribute}}>
<td colspan="100" class="border-0">
<table class="table table-sm table-borderless">
<tr>
<th>
<button type="button" class="btn btn-light btn-sm w-100" title="{{previousYearText}}" data-change-date-button="true" data-number="{{previousYearButtonDateNumber}}" {{previousYearButtonDisabledAttribute}}> &lt;&lt; </button>
</th>
<th>
<button type="button" class="btn btn-light btn-sm w-100" title="{{previousMonthText}}" data-change-date-button="true" data-number="{{previousMonthButtonDateNumber}}" {{previousMonthButtonDisabledAttribute}}> &lt; </button>
</th>
<th style="width: 120px;">
<div class="dropdown">
<button type="button" class="btn btn-light btn-sm dropdown-toggle w-100" id="mdtp-month-selector-button-{{guid}}"
data-bs-toggle="dropdown" aria-expanded="false">
{{selectedMonthName}}
</button>
<div class="dropdown-menu" aria-labelledby="mdtp-month-selector-button-{{guid}}">
<a class="dropdown-item {{selectMonth1ButtonCssClass}}" data-change-date-button="true" data-number="{{dropDownMenuMonth1DateNumber}}">{{monthName1}}</a>
<a class="dropdown-item {{selectMonth2ButtonCssClass}}" data-change-date-button="true" data-number="{{dropDownMenuMonth2DateNumber}}">{{monthName2}}</a>
<a class="dropdown-item {{selectMonth3ButtonCssClass}}" data-change-date-button="true" data-number="{{dropDownMenuMonth3DateNumber}}">{{monthName3}}</a>
<div class="dropdown-divider"></div>
<a class="dropdown-item {{selectMonth4ButtonCssClass}}" data-change-date-button="true" data-number="{{dropDownMenuMonth4DateNumber}}">{{monthName4}}</a>
<a class="dropdown-item {{selectMonth5ButtonCssClass}}" data-change-date-button="true" data-number="{{dropDownMenuMonth5DateNumber}}">{{monthName5}}</a>
<a class="dropdown-item {{selectMonth6ButtonCssClass}}" data-change-date-button="true" data-number="{{dropDownMenuMonth6DateNumber}}">{{monthName6}}</a>
<div class="dropdown-divider"></div>
<a class="dropdown-item {{selectMonth7ButtonCssClass}}" data-change-date-button="true" data-number="{{dropDownMenuMonth7DateNumber}}">{{monthName7}}</a>
<a class="dropdown-item {{selectMonth8ButtonCssClass}}" data-change-date-button="true" data-number="{{dropDownMenuMonth8DateNumber}}">{{monthName8}}</a>
<a class="dropdown-item {{selectMonth9ButtonCssClass}}" data-change-date-button="true" data-number="{{dropDownMenuMonth9DateNumber}}">{{monthName9}}</a>
<div class="dropdown-divider"></div>
<a class="dropdown-item {{selectMonth10ButtonCssClass}}" data-change-date-button="true" data-number="{{dropDownMenuMonth10DateNumber}}">{{monthName10}}</a>
<a class="dropdown-item {{selectMonth11ButtonCssClass}}" data-change-date-button="true" data-number="{{dropDownMenuMonth11DateNumber}}">{{monthName11}}</a>
<a class="dropdown-item {{selectMonth12ButtonCssClass}}" data-change-date-button="true" data-number="{{dropDownMenuMonth12DateNumber}}">{{monthName12}}</a>
</div>
</div>
</th>
<th style="width: 50px;">
<button type="button" class="btn btn-light btn-sm w-100" mds-pdtp-select-year-button {{selectYearButtonDisabledAttribute}}>{{selectedYear}}</button>
</th>
<th>
<button type="button" class="btn btn-light btn-sm w-100" title="{{nextMonthText}}" data-change-date-button="true" data-number="{{nextMonthButtonDateNumber}}" {{nextMonthButtonDisabledAttribute}}> &gt; </button>
</th>
<th>
<button type="button" class="btn btn-light btn-sm w-100" title="{{nextYearText}}" data-change-date-button="true" data-number="{{nextYearButtonDateNumber}}" {{nextYearButtonDisabledAttribute}}> &gt;&gt; </button>
</th>
</tr>
</table>
</td>
</tr>
</thead>
<tbody class="days">
<tr>
<td class="{{weekDayShortName1CssClass}}">{{weekDayShortName1}}</td>
<td>{{weekDayShortName2}}</td>
<td>{{weekDayShortName3}}</td>
<td>{{weekDayShortName4}}</td>
<td>{{weekDayShortName5}}</td>
<td>{{weekDayShortName6}}</td>
<td class="{{weekDayShortName7CssClass}}">{{weekDayShortName7}}</td>
</tr>
{{daysHtml}}
</tbody>
</table>
</td>`;

  private static previousYearTextPersian = 'سال قبل';
  private static previousMonthTextPersian = 'ماه قبل';
  private static previousTextPersian = 'قبلی';
  private static nextYearTextPersian = 'سال بعد';
  private static nextMonthTextPersian = 'ماه بعد';
  private static nextTextPersian = 'بعدی';
  private static todayTextPersian = 'امروز';
  private static goTodayTextPersian = 'برو به امروز';
  private static cancelTextPersian = 'انصراف';
  private static currentYearTextPersian = 'سال جاری';
  private static previousText = 'Previous';
  private static previousYearText = 'Previous Year';
  private static previousMonthText = 'Previous Month';
  private static nextText = 'Next';
  private static nextYearText = 'Next Year';
  private static nextMonthText = 'Next Month';
  private static todayText = 'Today';
  private static goTodayText = 'Go Today';
  private static cancelText = 'Cancel';
  private static currentYearText = 'Current Year';
  private static shortDayNamesPersian = [
    'ش',
    'ی',
    'د',
    'س',
    'چ',
    'پ',
    'ج',
  ];
  private static shortDayNames = [
    'Su',
    'Mo',
    'Tu',
    'We',
    'Th',
    'Fr',
    'Sa',
  ];
  private static monthNamesPersian = [
    'فروردین',
    'اردیبهشت',
    'خرداد',
    'تیر',
    'مرداد',
    'شهریور',
    'مهر',
    'آبان',
    'آذر',
    'دی',
    'بهمن',
    'اسفند'
  ];
  private static monthNames = [
    'January',
    'February',
    'March',
    'April',
    'May',
    'June',
    'July',
    'August',
    'September',
    'October',
    'November',
    'December'
  ];
  private static weekDayNames = [
    'Sunday',
    'Monday',
    'Tuesday',
    'Wednesday',
    'Thursday',
    'Friday',
    'Saturday'
  ];
  private static weekDayNamesPersian = [
    'یک شنبه',
    'دوشنبه',
    'سه شنبه',
    'چهارشنبه',
    'پنج شنبه',
    'جمعه',
    'شنبه'
  ];

  // #endregion

  // #region Properties

  readonly guid: string = '';
  readonly setting: MdsPersianDateTimePickerSetting;

  private readonly element: Element;

  private bsPopover: Popover | null = null;
  private bsModal: Modal | null = null;
  private tempTitleString = '';

  // #endregion

  // #region Methods

  private initializeBsPopover(setting: MdsPersianDateTimePickerSetting): void {

    // Validation

    if (setting.rangeSelector && (setting.toDate || setting.fromDate))
      throw new Error(`MdsPersianDateTimePicker => You can not set true 'toDate' or 'fromDate' and 'rangeSelector' together`);
    if (setting.toDate && setting.fromDate)
      throw new Error(`MdsPersianDateTimePicker => You can not set true 'toDate' and 'fromDate' together`);
    if (!setting.groupId && (setting.toDate || setting.fromDate))
      throw new Error(`MdsPersianDateTimePicker => When you set 'toDate' or 'fromDate' true, you have to set 'groupId'`);

    // ---------------------

    // آپشن هایی که باید همان لحظه تغییر اعمال شوند

    if (setting.disabled) {
      this.element.setAttribute("disabled", '');
    } else {
      this.element.removeAttribute("disabled");
    }
    if (setting.toDate || setting.fromDate) {
      setting.rangeSelector = false;
      this.element.setAttribute("data-mds-dtp-group", setting.groupId);
      if (setting.toDate)
        this.element.setAttribute("data-to-date", 'true');
      else if (setting.fromDate)
        this.element.setAttribute("data-from-date", 'true');
    }
    if (!setting.rangeSelector) {
      setting.rangeSelectorMonthsToShow = [0, 0];
    }

    // ---------------------

    setTimeout(() => {
      this.dispose();
      const title = this.getPopoverHeaderTitle(setting);
      let datePickerBodyHtml = this.getDateTimePickerBodyHtml(setting);
      let tempDiv = document.createElement('div');
      tempDiv.innerHTML = datePickerBodyHtml;
      const dropDowns = tempDiv.querySelectorAll('.dropdown>button');
      dropDowns.forEach(e => {
        if (setting.disabled) {
          e.setAttribute('disabled', '');
          e.classList.add('disabled');
        }
        else {
          e.removeAttribute('disabled');
          e.classList.remove('disabled');
        }
      });
      datePickerBodyHtml = tempDiv.innerHTML;
      if (setting.modalMode == true) {
        this.setModalHtml(title, datePickerBodyHtml, setting);
        this.bsPopover = null;
        setTimeout(() => {
          const el = this.getModal();
          if (el != null) {
            this.bsModal = new Modal(el);
            this.enableMainEvents();
          }
        }, 200);
      } else if (setting.inLine == true) {
        this.bsPopover = null;
        this.element.innerHTML = datePickerBodyHtml;
        this.enableInLineEvents();
      } else {
        this.bsPopover = new Popover(this.element, {
          container: 'body',
          content: datePickerBodyHtml,
          title: title,
          html: true,
          placement: setting.placement,
          trigger: 'manual',
          template: MdsPersianDateTimePicker.popoverHtmlTemplate,
          sanitize: false,
        });
        this.enableMainEvents();
      }
      MdsPersianDateTimePicker.setSelectedData(setting);
      this.tempTitleString = title;
    }, setting.inLine ? 10 : 100);
  }
  private static newGuid(): string {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, c => {
      let r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
      return v.toString(16);
    });
  }
  private static extend(...args: any[]): any {
    for (let i = 1; i < args.length; i++)
      for (let key in args[i])
        if (args[i].hasOwnProperty(key))
          args[0][key] = args[i][key];
    return args[0];
  }
  private static getClonedDate(dateTime: Date): Date {
    return new Date(dateTime.getTime());
  }
  private static getDateTimeJson1(dateTime: Date): GetDateTimeJson1 {
    return {
      year: dateTime.getFullYear(),
      month: dateTime.getMonth() + 1,
      day: dateTime.getDate(),
      hour: dateTime.getHours(),
      minute: dateTime.getMinutes(),
      second: dateTime.getSeconds(),
      millisecond: dateTime.getMilliseconds(),
      dayOfWeek: dateTime.getDay()
    };
  }
  private static getDateTimeJson2(dateNumber: number): GetDateTimeJson1 {
    return {
      year: Math.floor(dateNumber / 10000),
      month: Math.floor(dateNumber / 100) % 100,
      day: dateNumber % 100,
      hour: 0,
      minute: 0,
      second: 0,
      millisecond: 0,
      dayOfWeek: -1
    };
  }
  private static getDateTimeJsonPersian1(dateTime: Date): GetDateTimeJson1 {
    let persianDate = this.toJalali(dateTime.getFullYear(), dateTime.getMonth() + 1, dateTime.getDate());
    return {
      year: persianDate.jy,
      month: persianDate.jm,
      day: persianDate.jd,
      hour: dateTime.getHours(),
      minute: dateTime.getMinutes(),
      second: dateTime.getSeconds(),
      millisecond: dateTime.getMilliseconds(),
      dayOfWeek: dateTime.getDay(),
    };
  }
  private static getDateTimeJsonPersian2(yearPersian: number, monthPersian: number, dayPersian: number, hour: number, minute: number, second: number): GetDateTimeJson1 {
    if (!this.isNumber(hour)) hour = 0;
    if (!this.isNumber(minute)) minute = 0;
    if (!this.isNumber(second)) second = 0;
    let gregorian = this.toGregorian(yearPersian, monthPersian, dayPersian);
    return MdsPersianDateTimePicker.getDateTimeJsonPersian1(new Date(gregorian.gy, gregorian.gm - 1, gregorian.gd, hour, minute, second));
  }
  private static getWeekDayName(englishWeekDayIndex: number, isGregorian: boolean): string {
    if (!isGregorian) return this.weekDayNamesPersian[englishWeekDayIndex];
    return this.weekDayNames[englishWeekDayIndex];
  }
  private static getMonthName(monthIndex: number, isGregorian: boolean): string {
    if (monthIndex < 0)
      monthIndex = 11;
    else if (monthIndex > 11)
      monthIndex = 0;
    if (!isGregorian) return this.monthNamesPersian[monthIndex];
    return this.monthNames[monthIndex];
  }
  private static getWeekDayShortName(englishWeekDayIndex: number, isGregorian: boolean): string {
    if (!isGregorian)
      return this.shortDayNamesPersian[englishWeekDayIndex];
    return this.shortDayNames[englishWeekDayIndex];
  }
  private static isLeapYear(persianYear: number): boolean {
    return this.isLeapJalaliYear(persianYear);
  }
  private static getDaysInMonthPersian(year: number, month: number): number {
    let numberOfDaysInMonth = 31;
    if (month > 6 && month < 12)
      numberOfDaysInMonth = 30;
    else if (month == 12)
      numberOfDaysInMonth = this.isLeapYear(year) ? 30 : 29;
    return numberOfDaysInMonth;
  }
  private static getDaysInMonth(year: number, month: number): number {
    return new Date(year, month + 1, 0).getDate();
  }
  private static getLastDayDateOfPreviousMonth(dateTime: Date, isGregorian: boolean): Date {
    let dateTimeLocal = MdsPersianDateTimePicker.getClonedDate(dateTime);
    if (isGregorian) {
      let previousMonth = new Date(dateTimeLocal.getFullYear(), dateTimeLocal.getMonth() - 1, 1),
        daysInMonth = MdsPersianDateTimePicker.getDaysInMonth(previousMonth.getFullYear(), previousMonth.getMonth());
      return new Date(previousMonth.getFullYear(), previousMonth.getMonth(), daysInMonth);
    }
    let dateTimeJsonPersian = MdsPersianDateTimePicker.getDateTimeJsonPersian1(dateTimeLocal);
    dateTimeJsonPersian.month += -1;
    if (dateTimeJsonPersian.month <= 0) {
      dateTimeJsonPersian.month = 12;
      dateTimeJsonPersian.year--;
    } else if (dateTimeJsonPersian.month > 12) {
      dateTimeJsonPersian.year++;
      dateTimeJsonPersian.month = 1;
    }
    return MdsPersianDateTimePicker.getDateTime1(dateTimeJsonPersian.year, dateTimeJsonPersian.month, MdsPersianDateTimePicker.getDaysInMonthPersian(dateTimeJsonPersian.year, dateTimeJsonPersian.month));
  }
  private static getFirstDayDateOfNextMonth(dateTime: Date, isGregorian: boolean): Date {
    let dateTimeLocal = MdsPersianDateTimePicker.getClonedDate(dateTime);
    if (isGregorian) {
      let nextMonth = new Date(dateTimeLocal.getFullYear(), dateTimeLocal.getMonth() + 1, 1);
      return new Date(nextMonth.getFullYear(), nextMonth.getMonth(), 1);
    }
    let dateTimeJsonPersian = MdsPersianDateTimePicker.getDateTimeJsonPersian1(dateTimeLocal);
    dateTimeJsonPersian.month += 1;
    if (dateTimeJsonPersian.month <= 0) {
      dateTimeJsonPersian.month = 12;
      dateTimeJsonPersian.year--;
    }
    if (dateTimeJsonPersian.month > 12) {
      dateTimeJsonPersian.year++;
      dateTimeJsonPersian.month = 1;
    }
    return MdsPersianDateTimePicker.getDateTime1(dateTimeJsonPersian.year, dateTimeJsonPersian.month, 1);
  }
  private static getDateTime1(yearPersian: number, monthPersian: number, dayPersian: number, hour?: number, minute?: number, second?: number): Date {
    if (!this.isNumber(hour)) hour = 0;
    if (!this.isNumber(minute)) minute = 0;
    if (!this.isNumber(second)) second = 0;
    let gregorian = this.toGregorian(yearPersian, monthPersian, dayPersian);
    return new Date(gregorian.gy, gregorian.gm - 1, gregorian.gd, hour, minute, second);
  }
  private static getDateTime2(dateTimeJsonPersian: GetDateTimeJson1): Date {
    if (!dateTimeJsonPersian.hour) dateTimeJsonPersian.hour = 0;
    if (!dateTimeJsonPersian.minute) dateTimeJsonPersian.minute = 0;
    if (!dateTimeJsonPersian.second) dateTimeJsonPersian.second = 0;
    let gregorian = this.toGregorian(dateTimeJsonPersian.year, dateTimeJsonPersian.month, dateTimeJsonPersian.day);
    return new Date(gregorian.gy, gregorian.gm - 1, gregorian.gd, dateTimeJsonPersian.hour, dateTimeJsonPersian.minute, dateTimeJsonPersian.second);
  }
  private static getDateTime3(dateTimeJson: GetDateTimeJson1): Date {
    return new Date(dateTimeJson.year, dateTimeJson.month - 1, dateTimeJson.day, dateTimeJson.hour, dateTimeJson.minute, dateTimeJson.second);
  }
  private static getDateTime4(dateNumber: number, dateTime: Date, isGregorian: boolean): Date {
    let dateTimeJson = MdsPersianDateTimePicker.getDateTimeJson2(dateNumber);
    if (!isGregorian) {
      let dateTimeJsonPersian = MdsPersianDateTimePicker.getDateTimeJsonPersian1(dateTime);
      dateTimeJsonPersian.year = dateTimeJson.year;
      dateTimeJsonPersian.month = dateTimeJson.month;
      dateTimeJsonPersian.day = dateTimeJson.day;
      dateTime = this.getDateTime2(dateTimeJsonPersian);
    } else
      dateTime = new Date(dateTimeJson.year, dateTimeJson.month - 1, dateTimeJson.day,
        dateTime.getHours(), dateTime.getMinutes(), dateTime.getSeconds());
    return dateTime;
  }
  private static getLesserDisableBeforeDate(setting: MdsPersianDateTimePickerSetting): GetDateTimeJson1 | null {
    // دریافت تاریخ کوچکتر
    // از بین تاریخ های غیر فعال شده در گذشته
    let resultDate: Date | null = null;
    const dateNow = new Date();
    if (setting.disableBeforeToday && setting.disableBeforeDate) {
      if (setting.disableBeforeDate.getTime() <= dateNow.getTime())
        resultDate = MdsPersianDateTimePicker.getClonedDate(setting.disableBeforeDate);
      else
        resultDate = dateNow;
    } else if (setting.disableBeforeDate)
      resultDate = MdsPersianDateTimePicker.getClonedDate(setting.disableBeforeDate);
    else if (setting.disableBeforeToday)
      resultDate = dateNow;
    if (resultDate == null)
      return null;
    if (setting.isGregorian)
      return MdsPersianDateTimePicker.getDateTimeJson1(resultDate);
    return MdsPersianDateTimePicker.getDateTimeJsonPersian1(resultDate);
  }
  private static getBiggerDisableAfterDate(setting: MdsPersianDateTimePickerSetting): GetDateTimeJson1 | null {
    // دریافت تاریخ بزرگتر
    // از بین تاریخ های غیر فعال شده در آینده
    let resultDate: Date | null = null;
    const dateNow = new Date();
    if (setting.disableAfterDate && setting.disableAfterToday) {
      if (setting.disableAfterDate.getTime() >= dateNow.getTime())
        resultDate = MdsPersianDateTimePicker.getClonedDate(setting.disableAfterDate);
      else
        resultDate = dateNow;
    } else if (setting.disableAfterDate)
      resultDate = MdsPersianDateTimePicker.getClonedDate(setting.disableAfterDate);
    else if (setting.disableAfterToday)
      resultDate = dateNow;
    if (resultDate == null)
      return null;
    if (setting.isGregorian)
      return MdsPersianDateTimePicker.getDateTimeJson1(resultDate);
    return MdsPersianDateTimePicker.getDateTimeJsonPersian1(resultDate);
  }
  private static addMonthToDateTimeJson(dateTimeJson: GetDateTimeJson1, addedMonth: number, isGregorian: boolean): GetDateTimeJson1 {
    // وقتی نیاز هست تا ماه یا روز به تاریخی اضافه کنم
    // پس از اضافه کردن ماه یا روز این متد را استفاده میکنم تا سال و ماه
    // با مقادیر جدید تصحیح و برگشت داده شوند
    const dateTimeJson1 = Object.assign({}, dateTimeJson);
    dateTimeJson1.day = 1;
    dateTimeJson1.month += addedMonth;
    if (!isGregorian) {
      if (dateTimeJson1.month <= 0) {
        dateTimeJson1.month = 12;
        dateTimeJson1.year--;
      }
      if (dateTimeJson1.month > 12) {
        dateTimeJson1.year++;
        dateTimeJson1.month = 1;
      }
      return dateTimeJson1;
    }
    return MdsPersianDateTimePicker.getDateTimeJson1(this.getDateTime3(dateTimeJson1));
  }
  private static convertToNumber1(dateTimeJson: GetDateTimeJson1): number {
    return Number(MdsPersianDateTimePicker.zeroPad(dateTimeJson.year) + MdsPersianDateTimePicker.zeroPad(dateTimeJson.month) + MdsPersianDateTimePicker.zeroPad(dateTimeJson.day));
  }
  private static convertToNumber2(year: number, month: number, day: number): number {
    return Number(MdsPersianDateTimePicker.zeroPad(year) + MdsPersianDateTimePicker.zeroPad(month) + MdsPersianDateTimePicker.zeroPad(day));
  }
  private static convertToNumber3(dateTime: Date): number {
    return MdsPersianDateTimePicker.convertToNumber1(MdsPersianDateTimePicker.getDateTimeJson1(dateTime));
  }
  private static correctOptionValue(optionName: string, value: any): any {
    const setting = new MdsPersianDateTimePickerSetting();
    Object.keys(setting).filter(key => key === optionName).forEach(key => {
      switch (typeof (<any>setting)[key]) {
        case 'number':
          value = +value;
          break;
        case 'string':
          value = value.toString();
          break;
        case 'boolean':
          value = !!value;
          break;
        case 'object':
          if ((<any>setting)[key] instanceof Date) {
            value = new Date(value);
          } else if (Array.isArray((<any>setting)[key])) {
            switch (optionName) {
              case 'holidays':
              case 'disabledDates':
              case 'specialDates':
              case 'selectedRangeDate':
                value.forEach((item: any, i: number) => {
                  value[i] = new Date(item);
                });
                break;
              case 'disabledDays':
              case 'rangeSelectorMonthsToShow':
                value.forEach((item: any, i: number) => {
                  value[i] = +item;
                });
                break;
            }
          }
          break;
      }
    });
    return value;
  }
  private static getShortHour(hour: number): number {
    let shortHour;
    if (hour > 12)
      shortHour = hour - 12;
    else
      shortHour = hour;
    return shortHour;
  }
  private static getAmPm(hour: number, isGregorian: boolean): string {
    let amPm;
    if (hour > 12) {
      if (isGregorian)
        amPm = 'PM';
      else
        amPm = 'ب.ظ';
    } else
      if (isGregorian)
        amPm = 'AM';
      else
        amPm = 'ق.ظ';
    return amPm;
  }
  private static addMonthToDateTime(dateTime: Date, addedMonth: number, isGregorian: boolean): Date {
    let dateTimeJson: GetDateTimeJson1;
    if (!isGregorian) {
      dateTimeJson = MdsPersianDateTimePicker.getDateTimeJsonPersian1(dateTime);
      dateTimeJson = MdsPersianDateTimePicker.addMonthToDateTimeJson(dateTimeJson, addedMonth, isGregorian);
      return this.getDateTime2(dateTimeJson);
    }
    dateTimeJson = MdsPersianDateTimePicker.getDateTimeJson1(dateTime);
    dateTimeJson = MdsPersianDateTimePicker.addMonthToDateTimeJson(dateTimeJson, addedMonth, isGregorian);
    return this.getDateTime3(dateTimeJson);
  }
  private static isNumber(n: any): boolean {
    return !isNaN(parseFloat(n)) && isFinite(n);
  }
  private static toPersianNumber(inputNumber1: number | string): string {
    /* ۰ ۱ ۲ ۳ ۴ ۵ ۶ ۷ ۸ ۹ */
    if (!inputNumber1) return '';
    let str1 = inputNumber1.toString().trim();
    if (!str1) return '';
    str1 = str1.replace(/0/img, '۰');
    str1 = str1.replace(/1/img, '۱');
    str1 = str1.replace(/2/img, '۲');
    str1 = str1.replace(/3/img, '۳');
    str1 = str1.replace(/4/img, '۴');
    str1 = str1.replace(/5/img, '۵');
    str1 = str1.replace(/6/img, '۶');
    str1 = str1.replace(/7/img, '۷');
    str1 = str1.replace(/8/img, '۸');
    str1 = str1.replace(/9/img, '۹');
    return str1;
  }
  private static toEnglishNumber(inputNumber1: number | string): string {
    /* ۰ ۱ ۲ ۳ ۴ ۵ ۶ ۷ ۸ ۹ */
    if (!inputNumber1) return '';
    let str1 = inputNumber1.toString().trim();
    if (!str1) return '';
    str1 = str1.replace(/۰/img, '0');
    str1 = str1.replace(/۱/img, '1');
    str1 = str1.replace(/۲/img, '2');
    str1 = str1.replace(/۳/img, '3');
    str1 = str1.replace(/۴/img, '4');
    str1 = str1.replace(/۵/img, '5');
    str1 = str1.replace(/۶/img, '6');
    str1 = str1.replace(/۷/img, '7');
    str1 = str1.replace(/۸/img, '8');
    str1 = str1.replace(/۹/img, '9');
    return str1;
  }
  private static zeroPad(nr: any, base?: string): string {
    if (nr == undefined || nr == '') return '00';
    if (base == undefined || base == '') base = '00';
    let len = (String(base).length - String(nr).length) + 1;
    return len > 0 ? new Array(len).join('0') + nr : nr;
  }
  private static getDateTimeString(dateTimeJson: GetDateTimeJson1, format: string, isGregorian: boolean, persianNumber: boolean): string {

    /// فرمت های که پشتیبانی می شوند
    /// <para />
    /// yyyy: سال چهار رقمی
    /// <para />
    /// yy: سال دو رقمی
    /// <para />
    /// MMMM: نام ماه
    /// <para />
    /// MM: عدد دو رقمی ماه
    /// <para />
    /// M: عدد یک رقمی ماه
    /// <para />
    /// dddd: نام روز هفته
    /// <para />
    /// dd: عدد دو رقمی روز ماه
    /// <para />
    /// d: عدد یک رقمی روز ماه
    /// <para />
    /// HH: ساعت دو رقمی با فرمت 00 تا 24
    /// <para />
    /// H: ساعت یک رقمی با فرمت 0 تا 24
    /// <para />
    /// hh: ساعت دو رقمی با فرمت 00 تا 12
    /// <para />
    /// h: ساعت یک رقمی با فرمت 0 تا 12
    /// <para />
    /// mm: عدد دو رقمی دقیقه
    /// <para />
    /// m: عدد یک رقمی دقیقه
    /// <para />
    /// ss: ثانیه دو رقمی
    /// <para />
    /// s: ثانیه یک رقمی
    /// <para />
    /// fff: میلی ثانیه 3 رقمی
    /// <para />
    /// ff: میلی ثانیه 2 رقمی
    /// <para />
    /// f: میلی ثانیه یک رقمی
    /// <para />
    /// tt: ب.ظ یا ق.ظ
    /// <para />
    /// t: حرف اول از ب.ظ یا ق.ظ

    format = format.replace(/yyyy/mg, dateTimeJson.year.toString());
    format = format.replace(/yy/mg, (dateTimeJson.year % 100).toString());
    format = format.replace(/MMMM/mg, MdsPersianDateTimePicker.getMonthName(dateTimeJson.month - 1, isGregorian));
    format = format.replace(/MM/mg, MdsPersianDateTimePicker.zeroPad(dateTimeJson.month));
    format = format.replace(/M/mg, dateTimeJson.month.toString());
    format = format.replace(/dddd/mg, MdsPersianDateTimePicker.getWeekDayName(dateTimeJson.dayOfWeek, isGregorian));
    format = format.replace(/dd/mg, MdsPersianDateTimePicker.zeroPad(dateTimeJson.day));
    format = format.replace(/d/mg, dateTimeJson.day.toString());
    format = format.replace(/HH/mg, MdsPersianDateTimePicker.zeroPad(dateTimeJson.hour));
    format = format.replace(/H/mg, dateTimeJson.hour.toString());
    format = format.replace(/hh/mg, MdsPersianDateTimePicker.zeroPad(this.getShortHour(dateTimeJson.hour).toString()));
    format = format.replace(/h/mg, MdsPersianDateTimePicker.zeroPad(dateTimeJson.hour));
    format = format.replace(/mm/mg, MdsPersianDateTimePicker.zeroPad(dateTimeJson.minute));
    format = format.replace(/m/mg, dateTimeJson.minute.toString());
    format = format.replace(/ss/mg, MdsPersianDateTimePicker.zeroPad(dateTimeJson.second));
    format = format.replace(/s/mg, dateTimeJson.second.toString());
    format = format.replace(/fff/mg, MdsPersianDateTimePicker.zeroPad(dateTimeJson.millisecond, '000'));
    format = format.replace(/ff/mg, MdsPersianDateTimePicker.zeroPad(dateTimeJson.millisecond / 10));
    format = format.replace(/f/mg, (dateTimeJson.millisecond / 100).toString());
    format = format.replace(/tt/mg, this.getAmPm(dateTimeJson.hour, isGregorian));
    format = format.replace(/t/mg, this.getAmPm(dateTimeJson.hour, isGregorian)[0]);

    if (persianNumber)
      format = MdsPersianDateTimePicker.toPersianNumber(format);
    return format;
  }
  private static getSelectedDateTimeTextFormatted(setting: MdsPersianDateTimePickerSetting): string {
    if (setting.selectedDate == undefined) return '';
    if (!setting.enableTimePicker) {
      setting.selectedDate.setHours(0);
      setting.selectedDate.setMinutes(0);
      setting.selectedDate.setSeconds(0);
    }
    if (setting.rangeSelector && setting.rangeSelectorStartDate != undefined && setting.rangeSelectorEndDate != undefined)
      return MdsPersianDateTimePicker.getDateTimeString(!setting.isGregorian ? MdsPersianDateTimePicker.getDateTimeJsonPersian1(setting.rangeSelectorStartDate) : MdsPersianDateTimePicker.getDateTimeJson1(setting.rangeSelectorStartDate), setting.textFormat, setting.isGregorian, setting.persianNumber) + ' - ' +
        MdsPersianDateTimePicker.getDateTimeString(!setting.isGregorian ? MdsPersianDateTimePicker.getDateTimeJsonPersian1(setting.rangeSelectorEndDate) : MdsPersianDateTimePicker.getDateTimeJson1(setting.rangeSelectorEndDate), setting.textFormat, setting.isGregorian, setting.persianNumber);
    return MdsPersianDateTimePicker.getDateTimeString(!setting.isGregorian ? MdsPersianDateTimePicker.getDateTimeJsonPersian1(setting.selectedDate) : MdsPersianDateTimePicker.getDateTimeJson1(setting.selectedDate), setting.textFormat, setting.isGregorian, setting.persianNumber);
  }
  private static getSelectedDateFormatted(setting: MdsPersianDateTimePickerSetting): string {
    // دریافت رشته تاریخ انتخاب شده
    if ((!setting.rangeSelector && !setting.selectedDate) ||
      (setting.rangeSelector && !setting.rangeSelectorStartDate && !setting.rangeSelectorEndDate))
      return '';
    if (setting.rangeSelector)
      return MdsPersianDateTimePicker.getDateTimeString(MdsPersianDateTimePicker.getDateTimeJson1(setting.rangeSelectorStartDate!), setting.dateFormat, true, setting.persianNumber) + ' - ' +
        MdsPersianDateTimePicker.getDateTimeString(MdsPersianDateTimePicker.getDateTimeJson1(setting.rangeSelectorEndDate!), setting.dateFormat, true, setting.persianNumber);
    return MdsPersianDateTimePicker.getDateTimeString(MdsPersianDateTimePicker.getDateTimeJson1(setting.selectedDate!), setting.dateFormat, true, setting.persianNumber);
  }
  private static getDisabledDateObject(setting: MdsPersianDateTimePickerSetting): [GetDateTimeJson1 | null, GetDateTimeJson1 | null] {
    let disableBeforeDateTimeJson = this.getLesserDisableBeforeDate(setting);
    let disableAfterDateTimeJson = this.getBiggerDisableAfterDate(setting);
    // بررسی پراپرتی های از تاریخ، تا تاریخ
    if ((setting.fromDate || setting.toDate) && setting.groupId) {
      const toDateElement = document.querySelector(`[data-mds-dtp-group="${setting.groupId}"][data-to-date]`);
      const fromDateElement = document.querySelector(`[data-mds-dtp-group="${setting.groupId}"][data-from-date]`);
      if (toDateElement != null && setting.fromDate) {
        const toDateSetting = MdsPersianDateTimePicker.getInstance(toDateElement)?.setting;
        const toDateSelectedDate = !toDateSetting ? null : toDateSetting.selectedDate;
        disableAfterDateTimeJson = !toDateSelectedDate ? null : setting.isGregorian ? MdsPersianDateTimePicker.getDateTimeJson1(toDateSelectedDate) : MdsPersianDateTimePicker.getDateTimeJsonPersian1(toDateSelectedDate);
      } else if (fromDateElement != null && setting.toDate) {
        const fromDateSetting = MdsPersianDateTimePicker.getInstance(fromDateElement)?.setting;
        const fromDateSelectedDate = !fromDateSetting ? null : fromDateSetting.selectedDate;
        disableBeforeDateTimeJson = !fromDateSelectedDate ? null : setting.isGregorian ? MdsPersianDateTimePicker.getDateTimeJson1(fromDateSelectedDate) : MdsPersianDateTimePicker.getDateTimeJsonPersian1(fromDateSelectedDate);
      }
    }
    return [disableBeforeDateTimeJson, disableAfterDateTimeJson];
  }
  private static setSelectedData(setting: MdsPersianDateTimePickerSetting): void {
    const targetTextElement = setting.targetTextSelector ? document.querySelector(setting.targetTextSelector) : undefined;
    const targetDateElement = setting.targetDateSelector ? document.querySelector(setting.targetDateSelector) : undefined;
    const changeEvent = new Event('change');
    if (targetTextElement != undefined) {
      const dateTimeTextFormat = this.getSelectedDateTimeTextFormatted(setting);
      switch (targetTextElement.tagName.toLowerCase()) {
        case 'input':
          (<any>targetTextElement).value = dateTimeTextFormat;
          break;
        default:
          targetTextElement.innerHTML = dateTimeTextFormat;
          break;
      }
      targetTextElement.dispatchEvent(changeEvent);
    }
    if (targetDateElement != undefined) {
      const dateTimeFormat = this.toEnglishNumber(this.getSelectedDateFormatted(setting));
      switch (targetDateElement.tagName.toLowerCase()) {
        case 'input':
          (<any>targetDateElement).value = dateTimeFormat;
          break;
        default:
          targetDateElement.innerHTML = dateTimeFormat;
          break;
      }
      targetDateElement.dispatchEvent(changeEvent);
    }
  }
  private getPopover(element: Element): Element | null {
    let popoverId = element.getAttribute('aria-describedby');
    if (popoverId == undefined || popoverId == '')
      return element.closest('[data-mds-dtp]');
    return document.getElementById(popoverId.toString());
  }
  private getModal(): Element | null {
    return document.querySelector(`.modal[data-mds-dtp-guid="${this.guid}"]`);
  }
  private setModalHtml(title: string, datePickerBodyHtml: string, setting: MdsPersianDateTimePickerSetting): void {
    const prevModalElement = this.getModal();
    if (prevModalElement == null) {
      let modalHtml = MdsPersianDateTimePicker.modalHtmlTemplate;
      modalHtml = modalHtml.replace(/\{\{guid\}\}/img, this.guid);
      const tempDiv = document.createElement('div');
      tempDiv.innerHTML = modalHtml;
      tempDiv.querySelector('[data-mds-dtp-title] .modal-title')!.innerHTML = title;
      tempDiv.querySelector('[data-name="mds-dtp-body"]')!.innerHTML = datePickerBodyHtml;
      document.querySelector('body')!.appendChild(tempDiv);
    } else {
      prevModalElement.querySelector('[data-mds-dtp-title] .modal-title')!.innerHTML = title;
      prevModalElement.querySelector('[data-name="mds-dtp-body"]')!.innerHTML = datePickerBodyHtml;
    }
    const modalDialogElement = document.querySelector(`[data-mds-dtp-guid="${this.guid}"] .modal-dialog`);
    if (modalDialogElement != null) {
      if (setting.rangeSelector) {
        if (setting.rangeSelectorMonthsToShow[0] > 0 || setting.rangeSelectorMonthsToShow[1] > 0)
          modalDialogElement.classList.add('modal-xl');
        else
          modalDialogElement.classList.remove('modal-xl');
      } else {
        modalDialogElement.classList.remove('modal-xl');
      }
    } else {
      console.warn("mds.bs.datetimepicker: element with `data-mds-dtp-guid` selector not found !")
    }
  }
  private getYearsBoxBodyHtml(setting: MdsPersianDateTimePickerSetting, yearToStart: number): MdsPersianDateTimePickerYearToSelect {
    // بدست آوردن اچ تی ام ال انتخاب سال
    // yearToStart سال شروع

    setting.yearOffset = Number(setting.yearOffset);

    const selectedDateToShow = MdsPersianDateTimePicker.getClonedDate(setting.selectedDateToShow);
    const disabledDateObj = MdsPersianDateTimePicker.getDisabledDateObject(setting);
    const disableBeforeDateTimeJson = disabledDateObj[0];
    const disableAfterDateTimeJson = disabledDateObj[1];
    let html = MdsPersianDateTimePicker.dateTimePickerYearsToSelectHtmlTemplate;
    let yearsBoxHtml = '';
    let todayDateTimeJson: GetDateTimeJson1;
    let selectedDateTimeToShowJson: GetDateTimeJson1;
    let counter = 1;

    if (setting.isGregorian) {
      selectedDateTimeToShowJson = MdsPersianDateTimePicker.getDateTimeJson1(selectedDateToShow);
      todayDateTimeJson = MdsPersianDateTimePicker.getDateTimeJson1(new Date());
    } else {
      selectedDateTimeToShowJson = MdsPersianDateTimePicker.getDateTimeJsonPersian1(selectedDateToShow);
      todayDateTimeJson = MdsPersianDateTimePicker.getDateTimeJsonPersian1(new Date());
    }
    counter = 1;
    const yearStart = yearToStart ? yearToStart : todayDateTimeJson.year - setting.yearOffset;
    const yearEnd = yearToStart ? yearToStart + setting.yearOffset * 2 : todayDateTimeJson.year + setting.yearOffset;
    for (let i = yearStart; i < yearEnd; i++) {
      let disabledAttr = '';
      if (disableBeforeDateTimeJson != null) {
        disabledAttr = i < disableBeforeDateTimeJson.year ? 'disabled' : '';
      }
      if (!disabledAttr && disableAfterDateTimeJson != null) {
        disabledAttr = i > disableAfterDateTimeJson.year ? 'disabled' : '';
      }
      let currentYearDateTimeJson = MdsPersianDateTimePicker.getDateTimeJson2(MdsPersianDateTimePicker.convertToNumber2(i, selectedDateTimeToShowJson.month, MdsPersianDateTimePicker.getDaysInMonthPersian(i, selectedDateTimeToShowJson.month)));
      let currentYearDisabledAttr = '';
      let yearText = setting.isGregorian ? i.toString() : MdsPersianDateTimePicker.toPersianNumber(i);
      let yearDateNumber = MdsPersianDateTimePicker.convertToNumber2(i, selectedDateTimeToShowJson.month, 1);
      let todayAttr = todayDateTimeJson.year == i ? 'data-current-year="true"' : ''
      let selectedYearAttr = selectedDateTimeToShowJson.year == i ? 'data-selected-year' : ''
      let selectedYearTitle = '';
      if (todayAttr)
        selectedYearTitle = setting.isGregorian ? MdsPersianDateTimePicker.currentYearText : MdsPersianDateTimePicker.currentYearTextPersian;
      if (disableBeforeDateTimeJson != undefined && disableBeforeDateTimeJson.year != undefined && currentYearDateTimeJson.year < disableBeforeDateTimeJson.year)
        currentYearDisabledAttr = 'disabled';
      if (disableAfterDateTimeJson != undefined && disableAfterDateTimeJson.year != undefined && currentYearDateTimeJson.year > disableAfterDateTimeJson.year)
        currentYearDisabledAttr = 'disabled';
      if (setting.disableBeforeToday && currentYearDateTimeJson.year < todayDateTimeJson.year)
        currentYearDisabledAttr = 'disabled';
      if (setting.disableAfterToday && currentYearDateTimeJson.year > todayDateTimeJson.year)
        currentYearDisabledAttr = 'disabled';
      if (counter == 1) yearsBoxHtml += '<tr>';
      yearsBoxHtml += `
<td class="text-center" title="${selectedYearTitle}" ${todayAttr} ${selectedYearAttr}>
  <button class="btn btn-sm btn-light w-100" type="button" data-change-date-button="true" data-number="${yearDateNumber}" ${currentYearDisabledAttr} ${disabledAttr}>${yearText}</button>
</td>
`;
      if (counter == 5) yearsBoxHtml += '</tr>';
      counter++;
      if (counter > 5) counter = 1;
    }
    html = html.replace(/\{\{yearsBoxHtml\}\}/img, yearsBoxHtml);
    html = html.replace(/\{\{cancelText\}\}/img, setting.isGregorian ? MdsPersianDateTimePicker.cancelText : MdsPersianDateTimePicker.cancelTextPersian);
    if (setting.inLine && setting.yearOffset > 15)
      html += '<div style="height: 30px;"></div>';
    return {
      yearStart,
      yearEnd,
      html
    };
  }
  private getYearsBoxHeaderHtml(setting: MdsPersianDateTimePickerSetting, yearStart: number, yearEnd: number): string {
    const yearsRangeText = ` ${yearStart} - ${yearEnd - 1} `;
    const disabledDateObj = MdsPersianDateTimePicker.getDisabledDateObject(setting);
    let html = MdsPersianDateTimePicker.popoverHeaderSelectYearHtmlTemplate;
    html = html.replace(/\{{rtlCssClass\}\}/img, setting.isGregorian ? '' : 'rtl');
    html = html.replace(/\{{dirAttrValue\}\}/img, setting.isGregorian ? 'ltr' : 'rtl');
    html = html.replace(/\{\{yearsRangeText\}\}/img, setting.isGregorian ? yearsRangeText : MdsPersianDateTimePicker.toPersianNumber(yearsRangeText));
    html = html.replace(/\{\{previousText\}\}/img, setting.isGregorian ? MdsPersianDateTimePicker.previousText : MdsPersianDateTimePicker.previousTextPersian);
    html = html.replace(/\{\{nextText\}\}/img, setting.isGregorian ? MdsPersianDateTimePicker.nextText : MdsPersianDateTimePicker.nextTextPersian);
    html = html.replace(/\{\{latestPreviousYear\}\}/img, yearStart > yearEnd ? yearEnd.toString() : yearStart.toString());
    html = html.replace(/\{\{latestNextYear\}\}/img, yearStart > yearEnd ? yearStart.toString() : yearEnd.toString());
    html = html.replace(/\{\{prevYearButtonAttr\}\}/img, disabledDateObj[0] != null && yearStart - 1 < disabledDateObj[0].year ? 'disabled' : '');
    html = html.replace(/\{\{nextYearButtonAttr\}\}/img, disabledDateObj[1] != null && yearEnd + 1 > disabledDateObj[1].year ? 'disabled' : '');
    return html;
  }
  private getDateTimePickerMonthHtml(setting: MdsPersianDateTimePickerSetting, isNextMonth: boolean, isPrevMonth: boolean): string {
    let selectedDateToShow = MdsPersianDateTimePicker.getClonedDate(setting.selectedDateToShow);
    let selectedDateToShowTemp = MdsPersianDateTimePicker.getClonedDate(selectedDateToShow);
    let selectedDateTime = setting.selectedDate != undefined ? MdsPersianDateTimePicker.getClonedDate(setting.selectedDate) : undefined;
    let isNextOrPrevMonth = isNextMonth || isPrevMonth;
    let html = MdsPersianDateTimePicker.dateTimePickerMonthTableHtmlTemplate;

    html = html.replace(/\{\{guid\}\}/img, this.guid);
    html = html.replace(/\{\{monthTdAttribute\}\}/img, isNextMonth ? 'data-next-month' : isPrevMonth ? 'data-prev-month' : '');
    html = html.replace(/\{\{monthNameAttribute\}\}/img, !isNextOrPrevMonth ? 'hidden' : '');
    html = html.replace(/\{\{theadSelectDateButtonTrAttribute\}\}/img, !isNextOrPrevMonth ? '' : 'hidden');
    html = html.replace(/\{\{weekDayShortName1CssClass\}\}/img, setting.isGregorian ? 'text-danger' : '');
    html = html.replace(/\{\{weekDayShortName7CssClass\}\}/img, !setting.isGregorian ? 'text-danger' : '');
    html = html.replace(/\{\{previousYearText\}\}/img, setting.isGregorian ? MdsPersianDateTimePicker.previousYearText : MdsPersianDateTimePicker.previousYearTextPersian);
    html = html.replace(/\{\{previousMonthText\}\}/img, setting.isGregorian ? MdsPersianDateTimePicker.previousMonthText : MdsPersianDateTimePicker.previousMonthTextPersian);
    html = html.replace(/\{\{nextMonthText\}\}/img, setting.isGregorian ? MdsPersianDateTimePicker.nextMonthText : MdsPersianDateTimePicker.nextMonthTextPersian);
    html = html.replace(/\{\{nextYearText\}\}/img, setting.isGregorian ? MdsPersianDateTimePicker.nextYearText : MdsPersianDateTimePicker.nextYearTextPersian);
    html = html.replace(/\{\{monthName1\}\}/img, MdsPersianDateTimePicker.getMonthName(0, setting.isGregorian));
    html = html.replace(/\{\{monthName2\}\}/img, MdsPersianDateTimePicker.getMonthName(1, setting.isGregorian));
    html = html.replace(/\{\{monthName3\}\}/img, MdsPersianDateTimePicker.getMonthName(2, setting.isGregorian));
    html = html.replace(/\{\{monthName4\}\}/img, MdsPersianDateTimePicker.getMonthName(3, setting.isGregorian));
    html = html.replace(/\{\{monthName5\}\}/img, MdsPersianDateTimePicker.getMonthName(4, setting.isGregorian));
    html = html.replace(/\{\{monthName6\}\}/img, MdsPersianDateTimePicker.getMonthName(5, setting.isGregorian));
    html = html.replace(/\{\{monthName7\}\}/img, MdsPersianDateTimePicker.getMonthName(6, setting.isGregorian));
    html = html.replace(/\{\{monthName8\}\}/img, MdsPersianDateTimePicker.getMonthName(7, setting.isGregorian));
    html = html.replace(/\{\{monthName9\}\}/img, MdsPersianDateTimePicker.getMonthName(8, setting.isGregorian));
    html = html.replace(/\{\{monthName10\}\}/img, MdsPersianDateTimePicker.getMonthName(9, setting.isGregorian));
    html = html.replace(/\{\{monthName11\}\}/img, MdsPersianDateTimePicker.getMonthName(10, setting.isGregorian));
    html = html.replace(/\{\{monthName12\}\}/img, MdsPersianDateTimePicker.getMonthName(11, setting.isGregorian));
    html = html.replace(/\{\{weekDayShortName1\}\}/img, MdsPersianDateTimePicker.getWeekDayShortName(0, setting.isGregorian));
    html = html.replace(/\{\{weekDayShortName2\}\}/img, MdsPersianDateTimePicker.getWeekDayShortName(1, setting.isGregorian));
    html = html.replace(/\{\{weekDayShortName3\}\}/img, MdsPersianDateTimePicker.getWeekDayShortName(2, setting.isGregorian));
    html = html.replace(/\{\{weekDayShortName4\}\}/img, MdsPersianDateTimePicker.getWeekDayShortName(3, setting.isGregorian));
    html = html.replace(/\{\{weekDayShortName5\}\}/img, MdsPersianDateTimePicker.getWeekDayShortName(4, setting.isGregorian));
    html = html.replace(/\{\{weekDayShortName6\}\}/img, MdsPersianDateTimePicker.getWeekDayShortName(5, setting.isGregorian));
    html = html.replace(/\{\{weekDayShortName7\}\}/img, MdsPersianDateTimePicker.getWeekDayShortName(6, setting.isGregorian));

    const disabledDateObj = MdsPersianDateTimePicker.getDisabledDateObject(setting);
    let i = 0,
      j = 0,
      firstWeekDayNumber,
      cellNumber = 0,
      tdNumber = 0,
      selectedDateNumber = 0,
      selectedMonthName = '',
      todayDateTimeJson: GetDateTimeJson1, // year, month, day, hour, minute, second
      dateTimeToShowJson: GetDateTimeJson1, // year, month, day, hour, minute, second
      numberOfDaysInCurrentMonth = 0,
      numberOfDaysInPreviousMonth = 0,
      tr = document.createElement('TR'),
      td = document.createElement("TD"),
      daysHtml = '',
      currentDateNumber = 0,
      previousMonthDateNumber = 0,
      nextMonthDateNumber = 0,
      previousYearDateNumber = 0,
      nextYearDateNumber = 0,
      rangeSelectorStartDate = !setting.rangeSelector || setting.rangeSelectorStartDate == undefined ? undefined : MdsPersianDateTimePicker.getClonedDate(setting.rangeSelectorStartDate),
      rangeSelectorEndDate = !setting.rangeSelector || setting.rangeSelectorEndDate == undefined ? undefined : MdsPersianDateTimePicker.getClonedDate(setting.rangeSelectorEndDate),
      rangeSelectorStartDateNumber = 0,
      rangeSelectorEndDateNumber = 0,
      dayNumberInString = '0',
      dayOfWeek = '', // نام روز هفته
      monthsDateNumberAndAttr: any = {
        month1DateNumber: 0,
        month2DateNumber: 0,
        month3DateNumber: 0,
        month4DateNumber: 0,
        month5DateNumber: 0,
        month6DateNumber: 0,
        month7DateNumber: 0,
        month8DateNumber: 0,
        month9DateNumber: 0,
        month10DateNumber: 0,
        month11DateNumber: 0,
        month12DateNumber: 0,
        selectMonth1ButtonCssClass: '',
        selectMonth2ButtonCssClass: '',
        selectMonth3ButtonCssClass: '',
        selectMonth4ButtonCssClass: '',
        selectMonth5ButtonCssClass: '',
        selectMonth6ButtonCssClass: '',
        selectMonth7ButtonCssClass: '',
        selectMonth8ButtonCssClass: '',
        selectMonth9ButtonCssClass: '',
        selectMonth10ButtonCssClass: '',
        selectMonth11ButtonCssClass: '',
        selectMonth12ButtonCssClass: '',
      },
      holidaysDateNumbers: number[] = [],
      disabledDatesNumber: number[] = [],
      specialDatesNumber: number[] = [],
      disableBeforeDateTimeJson = disabledDateObj[0],
      disableAfterDateTimeJson = disabledDateObj[1],
      previousYearButtonDisabledAttribute = '',
      previousMonthButtonDisabledAttribute = '',
      selectYearButtonDisabledAttribute = '',
      nextMonthButtonDisabledAttribute = '',
      nextYearButtonDisabledAttribute = '',
      isTrAppended = false;

    if (setting.isGregorian) {
      dateTimeToShowJson = MdsPersianDateTimePicker.getDateTimeJson1(selectedDateToShowTemp);
      todayDateTimeJson = MdsPersianDateTimePicker.getDateTimeJson1(new Date());
      firstWeekDayNumber = new Date(dateTimeToShowJson.year, dateTimeToShowJson.month - 1, 1).getDay();
      selectedDateNumber = !selectedDateTime ? 0 : MdsPersianDateTimePicker.convertToNumber1(MdsPersianDateTimePicker.getDateTimeJson1(selectedDateTime));
      numberOfDaysInCurrentMonth = MdsPersianDateTimePicker.getDaysInMonth(dateTimeToShowJson.year, dateTimeToShowJson.month - 1);
      numberOfDaysInPreviousMonth = MdsPersianDateTimePicker.getDaysInMonth(dateTimeToShowJson.year, dateTimeToShowJson.month - 2);
      previousMonthDateNumber = MdsPersianDateTimePicker.convertToNumber1(MdsPersianDateTimePicker.getDateTimeJson1(MdsPersianDateTimePicker.getLastDayDateOfPreviousMonth(selectedDateToShowTemp, true)));
      nextMonthDateNumber = MdsPersianDateTimePicker.convertToNumber1(MdsPersianDateTimePicker.getDateTimeJson1(MdsPersianDateTimePicker.getFirstDayDateOfNextMonth(selectedDateToShowTemp, true)));
      selectedDateToShowTemp = MdsPersianDateTimePicker.getClonedDate(selectedDateToShow);
      previousYearDateNumber = MdsPersianDateTimePicker.convertToNumber1(MdsPersianDateTimePicker.getDateTimeJson1(new Date(selectedDateToShowTemp.setFullYear(selectedDateToShowTemp.getFullYear() - 1))));
      selectedDateToShowTemp = MdsPersianDateTimePicker.getClonedDate(selectedDateToShow);
      nextYearDateNumber = MdsPersianDateTimePicker.convertToNumber1(MdsPersianDateTimePicker.getDateTimeJson1(new Date(selectedDateToShowTemp.setFullYear(selectedDateToShowTemp.getFullYear() + 1))));
      selectedDateToShowTemp = MdsPersianDateTimePicker.getClonedDate(selectedDateToShow);
      rangeSelectorStartDateNumber = !setting.rangeSelector || !rangeSelectorStartDate ? 0 : MdsPersianDateTimePicker.convertToNumber3(rangeSelectorStartDate);
      rangeSelectorEndDateNumber = !setting.rangeSelector || !rangeSelectorEndDate ? 0 : MdsPersianDateTimePicker.convertToNumber3(rangeSelectorEndDate);
      for (i = 1; i <= 12; i++) {
        monthsDateNumberAndAttr['month' + i.toString() + 'DateNumber'] = MdsPersianDateTimePicker.convertToNumber1(MdsPersianDateTimePicker.getDateTimeJson1(new Date(selectedDateToShowTemp.setMonth(i - 1))));
        selectedDateToShowTemp = MdsPersianDateTimePicker.getClonedDate(selectedDateToShow);
      }
      for (i = 0; i < setting.holidays.length; i++) {
        holidaysDateNumbers.push(MdsPersianDateTimePicker.convertToNumber1(MdsPersianDateTimePicker.getDateTimeJson1(setting.holidays[i])));
      }
      for (i = 0; i < setting.disabledDates.length; i++) {
        disabledDatesNumber.push(MdsPersianDateTimePicker.convertToNumber1(MdsPersianDateTimePicker.getDateTimeJson1(setting.disabledDates[i])));
      }
      for (i = 0; i < setting.specialDates.length; i++) {
        specialDatesNumber.push(MdsPersianDateTimePicker.convertToNumber1(MdsPersianDateTimePicker.getDateTimeJson1(setting.specialDates[i])));
      }
    } else {
      dateTimeToShowJson = MdsPersianDateTimePicker.getDateTimeJsonPersian1(selectedDateToShowTemp);
      todayDateTimeJson = MdsPersianDateTimePicker.getDateTimeJsonPersian1(new Date());
      firstWeekDayNumber = MdsPersianDateTimePicker.getDateTimeJsonPersian2(dateTimeToShowJson.year, dateTimeToShowJson.month, 1, 0, 0, 0).dayOfWeek;
      selectedDateNumber = !selectedDateTime ? 0 : MdsPersianDateTimePicker.convertToNumber1(MdsPersianDateTimePicker.getDateTimeJsonPersian1(selectedDateTime));
      numberOfDaysInCurrentMonth = MdsPersianDateTimePicker.getDaysInMonthPersian(dateTimeToShowJson.year, dateTimeToShowJson.month);
      numberOfDaysInPreviousMonth = MdsPersianDateTimePicker.getDaysInMonthPersian(dateTimeToShowJson.year - 1, dateTimeToShowJson.month - 1);
      previousMonthDateNumber = MdsPersianDateTimePicker.convertToNumber1(MdsPersianDateTimePicker.getDateTimeJsonPersian1(MdsPersianDateTimePicker.getLastDayDateOfPreviousMonth(selectedDateToShowTemp, false)));
      selectedDateToShowTemp = MdsPersianDateTimePicker.getClonedDate(selectedDateToShow);
      nextMonthDateNumber = MdsPersianDateTimePicker.convertToNumber1(MdsPersianDateTimePicker.getDateTimeJsonPersian1(MdsPersianDateTimePicker.getFirstDayDateOfNextMonth(selectedDateToShowTemp, false)));
      selectedDateToShowTemp = MdsPersianDateTimePicker.getClonedDate(selectedDateToShow);
      previousYearDateNumber = MdsPersianDateTimePicker.convertToNumber2(dateTimeToShowJson.year - 1, dateTimeToShowJson.month, dateTimeToShowJson.day);
      nextYearDateNumber = MdsPersianDateTimePicker.convertToNumber2(dateTimeToShowJson.year + 1, dateTimeToShowJson.month, dateTimeToShowJson.day);
      selectedDateToShowTemp = MdsPersianDateTimePicker.getClonedDate(selectedDateToShow);
      rangeSelectorStartDateNumber = !setting.rangeSelector || !rangeSelectorStartDate ? 0 : MdsPersianDateTimePicker.convertToNumber1(MdsPersianDateTimePicker.getDateTimeJsonPersian1(rangeSelectorStartDate));
      rangeSelectorEndDateNumber = !setting.rangeSelector || !rangeSelectorEndDate ? 0 : MdsPersianDateTimePicker.convertToNumber1(MdsPersianDateTimePicker.getDateTimeJsonPersian1(rangeSelectorEndDate));
      for (i = 1; i <= 12; i++) {
        monthsDateNumberAndAttr['month' + i.toString() + 'DateNumber'] = MdsPersianDateTimePicker.convertToNumber2(dateTimeToShowJson.year, i, MdsPersianDateTimePicker.getDaysInMonthPersian(dateTimeToShowJson.year, i));
        selectedDateToShowTemp = MdsPersianDateTimePicker.getClonedDate(selectedDateToShow);
      }
      for (i = 0; i < setting.holidays.length; i++) {
        holidaysDateNumbers.push(MdsPersianDateTimePicker.convertToNumber1(MdsPersianDateTimePicker.getDateTimeJsonPersian1(setting.holidays[i])));
      }
      for (i = 0; i < setting.disabledDates.length; i++) {
        disabledDatesNumber.push(MdsPersianDateTimePicker.convertToNumber1(MdsPersianDateTimePicker.getDateTimeJsonPersian1(setting.disabledDates[i])));
      }
      for (i = 0; i < setting.specialDates.length; i++) {
        specialDatesNumber.push(MdsPersianDateTimePicker.convertToNumber1(MdsPersianDateTimePicker.getDateTimeJsonPersian1(setting.specialDates[i])));
      }
    }

    let todayDateNumber = MdsPersianDateTimePicker.convertToNumber1(todayDateTimeJson);
    let selectedYear = setting.isGregorian ? dateTimeToShowJson.year.toString() : MdsPersianDateTimePicker.toPersianNumber(dateTimeToShowJson.year);
    let disableBeforeDateTimeNumber = !disableBeforeDateTimeJson ? undefined : MdsPersianDateTimePicker.convertToNumber1(disableBeforeDateTimeJson);
    let disableAfterDateTimeNumber = !disableAfterDateTimeJson ? undefined : MdsPersianDateTimePicker.convertToNumber1(disableAfterDateTimeJson);
    let currentMonthInfo = MdsPersianDateTimePicker.getMonthName(dateTimeToShowJson.month - 1, setting.isGregorian) + ' ' + dateTimeToShowJson.year.toString();
    if (!setting.isGregorian)
      currentMonthInfo = MdsPersianDateTimePicker.toPersianNumber(currentMonthInfo);
    selectedMonthName = MdsPersianDateTimePicker.getMonthName(dateTimeToShowJson.month - 1, setting.isGregorian);

    if (setting.yearOffset <= 0) {
      previousYearButtonDisabledAttribute = 'disabled';
      nextYearButtonDisabledAttribute = 'disabled';
      selectYearButtonDisabledAttribute = 'disabled';
    }

    // روز های ماه قبل
    if (!setting.isGregorian && firstWeekDayNumber != 6 || setting.isGregorian && firstWeekDayNumber != 0) {
      if (setting.isGregorian)
        firstWeekDayNumber--;
      let previousMonthDateTimeJson = MdsPersianDateTimePicker.addMonthToDateTimeJson(dateTimeToShowJson, -1, setting.isGregorian);
      for (i = numberOfDaysInPreviousMonth - firstWeekDayNumber; i <= numberOfDaysInPreviousMonth; i++) {
        currentDateNumber = MdsPersianDateTimePicker.convertToNumber2(previousMonthDateTimeJson.year, previousMonthDateTimeJson.month, i);
        dayNumberInString = setting.isGregorian ? MdsPersianDateTimePicker.zeroPad(i) : MdsPersianDateTimePicker.toPersianNumber(MdsPersianDateTimePicker.zeroPad(i));
        td = document.createElement('TD');
        td.setAttribute('data-nm', '');
        td.setAttribute('data-number', currentDateNumber.toString());
        td.innerHTML = dayNumberInString;
        if (setting.rangeSelector) {
          if (currentDateNumber == rangeSelectorStartDateNumber || currentDateNumber == rangeSelectorEndDateNumber)
            td.classList.add('selected-range-days-start-end');
          else if (rangeSelectorStartDateNumber > 0 && rangeSelectorEndDateNumber > 0 && currentDateNumber > rangeSelectorStartDateNumber && currentDateNumber < rangeSelectorEndDateNumber)
            td.classList.add('selected-range-days-nm');
        }
        // روز جمعه
        if (!setting.isGregorian && tdNumber == 6)
          td.classList.add('text-danger');
        // روز یکشنبه
        else if (setting.isGregorian && tdNumber == 0)
          td.classList.add('text-danger');
        tr.appendChild(td);
        cellNumber++;
        tdNumber++;
        if (tdNumber >= 7) {
          tdNumber = 0;
          daysHtml += tr.outerHTML;
          isTrAppended = true;
          tr = document.createElement('TR');
        }
      }
    }

    // روزهای ماه جاری
    for (i = 1; i <= numberOfDaysInCurrentMonth; i++) {

      if (tdNumber >= 7) {
        tdNumber = 0;
        daysHtml += tr.outerHTML;
        isTrAppended = true;
        tr = document.createElement('TR');
      }

      // عدد روز
      currentDateNumber = MdsPersianDateTimePicker.convertToNumber2(dateTimeToShowJson.year, dateTimeToShowJson.month, i);
      dayNumberInString = setting.isGregorian ? MdsPersianDateTimePicker.zeroPad(i) : MdsPersianDateTimePicker.toPersianNumber(MdsPersianDateTimePicker.zeroPad(i));

      td = document.createElement('TD');
      td.setAttribute('data-day', '');
      td.setAttribute('data-number', currentDateNumber.toString());
      td.innerHTML = dayNumberInString;

      // امروز
      if (currentDateNumber == todayDateNumber) {
        td.setAttribute('data-today', '');
        td.setAttribute('title', setting.isGregorian ? MdsPersianDateTimePicker.todayText : MdsPersianDateTimePicker.todayTextPersian);
        // اگر نام روز هفته انتخاب شده در تکس باکس قبل از تاریخ امروز باشد
        // نباید دیگر نام روز هفته تغییر کند
        if (!dayOfWeek)
          dayOfWeek = MdsPersianDateTimePicker.getWeekDayName(tdNumber - 1 < 0 ? 0 : tdNumber - 1, setting.isGregorian);
      }

      // روز از قبل انتخاب شده
      if (!setting.rangeSelector && selectedDateNumber == currentDateNumber) {
        td.setAttribute('data-mds-dtp-selected-day', '');
        dayOfWeek = MdsPersianDateTimePicker.getWeekDayName(tdNumber - 1 < 0 ? 0 : tdNumber - 1, setting.isGregorian);
      }

      // روزهای تعطیل
      for (j = 0; j < holidaysDateNumbers.length; j++) {
        if (holidaysDateNumbers[j] != currentDateNumber) continue;
        td.classList.add('text-danger');
        break;
      }

      // روز جمعه شمسی
      if (!setting.isGregorian && tdNumber == 6) {
        td.classList.add('text-danger');
      }
      // روز یکشنبه میلادی
      else if (setting.isGregorian && tdNumber == 0) {
        td.classList.add('text-danger');
      }

      // روزهای غیر فعال شده
      if (setting.disableBeforeToday) {
        if (currentDateNumber < todayDateNumber) td.setAttribute('disabled', '');
        if (nextMonthDateNumber < todayDateNumber)
          nextMonthButtonDisabledAttribute = 'disabled';
        if (nextYearDateNumber < todayDateNumber)
          nextYearButtonDisabledAttribute = 'disabled';
        if (previousMonthDateNumber < todayDateNumber)
          previousMonthButtonDisabledAttribute = 'disabled';
        if (previousYearDateNumber < todayDateNumber)
          previousYearButtonDisabledAttribute = 'disabled';
        for (j = 1; j <= 12; j++) {
          if (monthsDateNumberAndAttr['month' + j.toString() + 'DateNumber'] < todayDateNumber)
            monthsDateNumberAndAttr['selectMonth' + j.toString() + 'ButtonCssClass'] = 'disabled';
        }
      }
      if (setting.disableAfterToday) {
        if (currentDateNumber > todayDateNumber) td.setAttribute('disabled', '');
        if (nextMonthDateNumber > todayDateNumber)
          nextMonthButtonDisabledAttribute = 'disabled';
        if (nextYearDateNumber > todayDateNumber)
          nextYearButtonDisabledAttribute = 'disabled';
        if (previousMonthDateNumber > todayDateNumber)
          previousMonthButtonDisabledAttribute = 'disabled';
        if (previousYearDateNumber > todayDateNumber)
          previousYearButtonDisabledAttribute = 'disabled';
        for (j = 1; j <= 12; j++) {
          if (monthsDateNumberAndAttr['month' + j.toString() + 'DateNumber'] > todayDateNumber)
            monthsDateNumberAndAttr['selectMonth' + j.toString() + 'ButtonCssClass'] = 'disabled';
        }
      }
      if (disableAfterDateTimeNumber) {
        if (currentDateNumber > disableAfterDateTimeNumber) td.setAttribute('disabled', '');
        if (nextMonthDateNumber > disableAfterDateTimeNumber)
          nextMonthButtonDisabledAttribute = 'disabled';
        if (nextYearDateNumber > disableAfterDateTimeNumber)
          nextYearButtonDisabledAttribute = 'disabled';
        if (previousMonthDateNumber > disableAfterDateTimeNumber)
          previousMonthButtonDisabledAttribute = 'disabled';
        if (previousYearDateNumber > disableAfterDateTimeNumber)
          previousYearButtonDisabledAttribute = 'disabled';
        for (j = 1; j <= 12; j++) {
          if (monthsDateNumberAndAttr['month' + j.toString() + 'DateNumber'] > disableAfterDateTimeNumber)
            monthsDateNumberAndAttr['selectMonth' + j.toString() + 'ButtonCssClass'] = 'disabled';
        }
      }
      if (disableBeforeDateTimeNumber) {
        if (currentDateNumber < disableBeforeDateTimeNumber) td.setAttribute('disabled', '');
        if (nextMonthDateNumber < disableBeforeDateTimeNumber)
          nextMonthButtonDisabledAttribute = 'disabled';
        if (nextYearDateNumber < disableBeforeDateTimeNumber)
          nextYearButtonDisabledAttribute = 'disabled';
        if (previousMonthDateNumber < disableBeforeDateTimeNumber)
          previousMonthButtonDisabledAttribute = 'disabled';
        if (previousYearDateNumber < disableBeforeDateTimeNumber)
          previousYearButtonDisabledAttribute = 'disabled';
        for (j = 1; j <= 12; j++) {
          if (monthsDateNumberAndAttr['month' + j.toString() + 'DateNumber'] < disableBeforeDateTimeNumber)
            monthsDateNumberAndAttr['selectMonth' + j.toString() + 'ButtonCssClass'] = 'disabled';
        }
      }
      for (j = 0; j < disabledDatesNumber.length; j++) {
        if (currentDateNumber == disabledDatesNumber[j])
          td.setAttribute('disabled', '');
      }
      for (j = 0; j < specialDatesNumber.length; j++) {
        if (currentDateNumber == specialDatesNumber[j])
          td.setAttribute('data-special-date', '');
      }
      if (setting.disabledDays != null && setting.disabledDays.length > 0 && setting.disabledDays.indexOf(tdNumber) >= 0) {
        td.setAttribute('disabled', '');
      }
      // \\

      if (setting.rangeSelector) {
        if (currentDateNumber == rangeSelectorStartDateNumber || currentDateNumber == rangeSelectorEndDateNumber)
          td.classList.add('selected-range-days-start-end');
        else if (rangeSelectorStartDateNumber > 0 && rangeSelectorEndDateNumber > 0 && currentDateNumber > rangeSelectorStartDateNumber && currentDateNumber < rangeSelectorEndDateNumber)
          td.classList.add('selected-range-days');
      }

      tr.appendChild(td);
      isTrAppended = false;

      tdNumber++;
      cellNumber++;
    }

    if (tdNumber >= 7) {
      tdNumber = 0;
      daysHtml += tr.outerHTML;
      isTrAppended = true;
      tr = document.createElement('TR');
    }

    // روزهای ماه بعد
    let nextMonthDateTimeJson = MdsPersianDateTimePicker.addMonthToDateTimeJson(dateTimeToShowJson, 1, setting.isGregorian);
    for (i = 1; i <= 42 - cellNumber; i++) {
      dayNumberInString = setting.isGregorian ? MdsPersianDateTimePicker.zeroPad(i) : MdsPersianDateTimePicker.toPersianNumber(MdsPersianDateTimePicker.zeroPad(i));
      currentDateNumber = MdsPersianDateTimePicker.convertToNumber2(nextMonthDateTimeJson.year, nextMonthDateTimeJson.month, i);
      td = document.createElement('TD');
      td.setAttribute('data-nm', '');
      td.setAttribute('data-number', currentDateNumber.toString());
      td.innerHTML = dayNumberInString;
      if (setting.rangeSelector) {
        if (currentDateNumber == rangeSelectorStartDateNumber || currentDateNumber == rangeSelectorEndDateNumber)
          td.classList.add('selected-range-days-start-end');
        else if (rangeSelectorStartDateNumber > 0 && rangeSelectorEndDateNumber > 0 && currentDateNumber > rangeSelectorStartDateNumber && currentDateNumber < rangeSelectorEndDateNumber)
          td.classList.add('selected-range-days-nm');
      }
      // روز جمعه
      if (!setting.isGregorian && tdNumber == 6)
        td.classList.add('text-danger');
      // روز یکشنبه
      else if (setting.isGregorian && tdNumber == 0)
        td.classList.add('text-danger');
      tr.appendChild(td);
      tdNumber++;
      if (tdNumber >= 7) {
        tdNumber = 0;
        daysHtml += tr.outerHTML;
        isTrAppended = true;
        tr = document.createElement('TR');
      }
    }

    if (!isTrAppended) {
      daysHtml += tr.outerHTML;
      isTrAppended = true;
    }

    html = html.replace(/\{\{currentMonthInfo\}\}/img, currentMonthInfo);
    html = html.replace(/\{\{selectedYear\}\}/img, selectedYear);
    html = html.replace(/\{\{selectedMonthName\}\}/img, selectedMonthName);
    html = html.replace(/\{\{daysHtml\}\}/img, daysHtml);
    html = html.replace(/\{\{previousYearButtonDisabledAttribute\}\}/img, previousYearButtonDisabledAttribute);
    html = html.replace(/\{\{previousYearButtonDateNumber\}\}/img, previousYearDateNumber.toString());
    html = html.replace(/\{\{previousMonthButtonDisabledAttribute\}\}/img, previousMonthButtonDisabledAttribute);
    html = html.replace(/\{\{previousMonthButtonDateNumber\}\}/img, previousMonthDateNumber.toString());
    html = html.replace(/\{\{selectYearButtonDisabledAttribute\}\}/img, selectYearButtonDisabledAttribute);
    html = html.replace(/\{\{nextMonthButtonDisabledAttribute\}\}/img, nextMonthButtonDisabledAttribute);
    html = html.replace(/\{\{nextMonthButtonDateNumber\}\}/img, nextMonthDateNumber.toString());
    html = html.replace(/\{\{nextYearButtonDisabledAttribute\}\}/img, nextYearButtonDisabledAttribute);
    html = html.replace(/\{\{nextYearButtonDateNumber\}\}/img, nextYearDateNumber.toString());
    html = html.replace(/\{\{dropDownMenuMonth1DateNumber\}\}/img, monthsDateNumberAndAttr.month1DateNumber);
    html = html.replace(/\{\{dropDownMenuMonth2DateNumber\}\}/img, monthsDateNumberAndAttr.month2DateNumber);
    html = html.replace(/\{\{dropDownMenuMonth3DateNumber\}\}/img, monthsDateNumberAndAttr.month3DateNumber);
    html = html.replace(/\{\{dropDownMenuMonth4DateNumber\}\}/img, monthsDateNumberAndAttr.month4DateNumber);
    html = html.replace(/\{\{dropDownMenuMonth5DateNumber\}\}/img, monthsDateNumberAndAttr.month5DateNumber);
    html = html.replace(/\{\{dropDownMenuMonth6DateNumber\}\}/img, monthsDateNumberAndAttr.month6DateNumber);
    html = html.replace(/\{\{dropDownMenuMonth7DateNumber\}\}/img, monthsDateNumberAndAttr.month7DateNumber);
    html = html.replace(/\{\{dropDownMenuMonth8DateNumber\}\}/img, monthsDateNumberAndAttr.month8DateNumber);
    html = html.replace(/\{\{dropDownMenuMonth9DateNumber\}\}/img, monthsDateNumberAndAttr.month9DateNumber);
    html = html.replace(/\{\{dropDownMenuMonth10DateNumber\}\}/img, monthsDateNumberAndAttr.month10DateNumber);
    html = html.replace(/\{\{dropDownMenuMonth11DateNumber\}\}/img, monthsDateNumberAndAttr.month11DateNumber);
    html = html.replace(/\{\{dropDownMenuMonth12DateNumber\}\}/img, monthsDateNumberAndAttr.month12DateNumber);
    html = html.replace(/\{\{selectMonth1ButtonCssClass\}\}/img, monthsDateNumberAndAttr.selectMonth1ButtonCssClass);
    html = html.replace(/\{\{selectMonth2ButtonCssClass\}\}/img, monthsDateNumberAndAttr.selectMonth2ButtonCssClass);
    html = html.replace(/\{\{selectMonth3ButtonCssClass\}\}/img, monthsDateNumberAndAttr.selectMonth3ButtonCssClass);
    html = html.replace(/\{\{selectMonth4ButtonCssClass\}\}/img, monthsDateNumberAndAttr.selectMonth4ButtonCssClass);
    html = html.replace(/\{\{selectMonth5ButtonCssClass\}\}/img, monthsDateNumberAndAttr.selectMonth5ButtonCssClass);
    html = html.replace(/\{\{selectMonth6ButtonCssClass\}\}/img, monthsDateNumberAndAttr.selectMonth6ButtonCssClass);
    html = html.replace(/\{\{selectMonth7ButtonCssClass\}\}/img, monthsDateNumberAndAttr.selectMonth7ButtonCssClass);
    html = html.replace(/\{\{selectMonth8ButtonCssClass\}\}/img, monthsDateNumberAndAttr.selectMonth8ButtonCssClass);
    html = html.replace(/\{\{selectMonth9ButtonCssClass\}\}/img, monthsDateNumberAndAttr.selectMonth9ButtonCssClass);
    html = html.replace(/\{\{selectMonth10ButtonCssClass\}\}/img, monthsDateNumberAndAttr.selectMonth10ButtonCssClass);
    html = html.replace(/\{\{selectMonth11ButtonCssClass\}\}/img, monthsDateNumberAndAttr.selectMonth11ButtonCssClass);
    html = html.replace(/\{\{selectMonth12ButtonCssClass\}\}/img, monthsDateNumberAndAttr.selectMonth12ButtonCssClass);

    return html;
  }
  private hideYearsBox = (element: Element, setting: MdsPersianDateTimePickerSetting): void => {
    if (setting.inLine) {
      const dtpInLine = element.closest('[data-mds-dtp-guid]');
      if (dtpInLine == null) return;
      const dtpInlineHeaderElement = dtpInLine.querySelector('[mds-dtp-inline-header]');
      if (this.tempTitleString && dtpInlineHeaderElement != null)
        dtpInlineHeaderElement.innerHTML = this.tempTitleString;
      const yearListBoxElement = dtpInLine.querySelector('[data-mds-dtp-year-list-box]');
      if (yearListBoxElement != null) {
        yearListBoxElement.classList.add('w-0');
        yearListBoxElement.innerHTML = '';
      }
      const inlineYearsContainerElement = dtpInLine.querySelector('[data-name="dtp-years-container"]');
      if (inlineYearsContainerElement != null) {
        inlineYearsContainerElement.classList.add('w-0');
        inlineYearsContainerElement.innerHTML = '';
      }
      dtpInLine.classList.remove('overflow-hidden');
    } else {
      const popoverOrModalElement = setting.modalMode ? this.getModal() : this.getPopover(element);
      if (popoverOrModalElement == null) return;
      if (this.tempTitleString) {
        if (setting.modalMode)
          popoverOrModalElement.querySelector('[data-mds-dtp-title] .modal-title')!.innerHTML = this.tempTitleString;
        else {
          popoverOrModalElement.querySelector('[data-mds-dtp-title]')!.innerHTML = this.tempTitleString;
        }
        popoverOrModalElement.querySelector('[data-name="mds-dtp-body"]')!.removeAttribute('hidden');
      }
      const yearListBox = popoverOrModalElement.querySelector('[data-mds-dtp-year-list-box]');
      yearListBox!.classList.add('w-0');
      yearListBox!.innerHTML = '';
    }
  }
  private showYearsBox = (element: Element): void => {
    const instance = MdsPersianDateTimePicker.getInstance(element);
    if (!instance) {
      return;
    }
    const setting = instance.setting;
    const mdDatePickerContainer = setting.inLine ? element.closest('[data-mds-dtp-guid]') : element.closest('[data-mds-dtp]');
    if (mdDatePickerContainer == null) return;
    this.tempTitleString = setting.inLine
      ? mdDatePickerContainer.querySelector('[mds-dtp-inline-header]')!.textContent!.trim()
      : mdDatePickerContainer.querySelector('[data-mds-dtp-title]')!.textContent!.trim();
    const yearsToSelectObject = this.getYearsBoxBodyHtml(setting, 0);
    const dateTimePickerYearsToSelectHtml = yearsToSelectObject.html;
    const dateTimePickerYearsToSelectContainer = mdDatePickerContainer.querySelector('[data-mds-dtp-year-list-box]');
    this.setPopoverHeaderHtml(element, setting, this.getYearsBoxHeaderHtml(setting, yearsToSelectObject.yearStart, yearsToSelectObject.yearEnd));
    dateTimePickerYearsToSelectContainer!.innerHTML = dateTimePickerYearsToSelectHtml;
    dateTimePickerYearsToSelectContainer!.classList.remove('w-0');
    if (setting.inLine) {
      mdDatePickerContainer.classList.add('overflow-hidden')
      dateTimePickerYearsToSelectContainer!.classList.add('inline');
    } else if (setting.modalMode) {
      mdDatePickerContainer.querySelector('[data-name="mds-dtp-body"]')!.setAttribute('hidden', '');
    } else {
      dateTimePickerYearsToSelectContainer!.classList.remove('inline');
    }
  }
  private changeYearList = (element: Element): void => {
    // کلیک روی دکمه های عوض کردن رنج سال انتخابی
    const instance = MdsPersianDateTimePicker.getInstance(element);
    if (!instance) {
      return;
    }
    const setting = instance.setting;
    const isNext = element.getAttribute('data-year-range-button-change') == '1';
    const yearStart = Number(element.getAttribute('data-year'));
    const yearsToSelectObject = this.getYearsBoxBodyHtml(setting, isNext ? yearStart : yearStart - setting.yearOffset * 2);
    if (setting.inLine)
      element.closest('[data-mds-dtp-guid]')!.querySelector('[data-mds-dtp-year-list-box]')!.innerHTML = yearsToSelectObject.html;
    else
      element.closest('[data-mds-dtp]')!.querySelector('[data-mds-dtp-year-list-box]')!.innerHTML = yearsToSelectObject.html;
    this.setPopoverHeaderHtml(element, setting, this.getYearsBoxHeaderHtml(setting, yearsToSelectObject.yearStart, yearsToSelectObject.yearEnd));
  }
  private getPopoverHeaderTitle(setting: MdsPersianDateTimePickerSetting): string {
    let selectedDateToShowJson: GetDateTimeJson1;
    let title = '';
    if (setting.isGregorian) {
      selectedDateToShowJson = MdsPersianDateTimePicker.getDateTimeJson1(setting.selectedDateToShow);
    } else {
      selectedDateToShowJson = MdsPersianDateTimePicker.getDateTimeJsonPersian1(setting.selectedDateToShow);
    }
    if (setting.rangeSelector) {
      const startDate = MdsPersianDateTimePicker.addMonthToDateTime(setting.selectedDateToShow, -setting.rangeSelectorMonthsToShow[0], setting.isGregorian);
      const endDate = MdsPersianDateTimePicker.addMonthToDateTime(setting.selectedDateToShow, setting.rangeSelectorMonthsToShow[1], setting.isGregorian);
      let statDateJson: GetDateTimeJson1;
      let endDateJson: GetDateTimeJson1;
      if (setting.isGregorian) {
        statDateJson = MdsPersianDateTimePicker.getDateTimeJson1(startDate);
        endDateJson = MdsPersianDateTimePicker.getDateTimeJson1(endDate);
      } else {
        statDateJson = MdsPersianDateTimePicker.getDateTimeJsonPersian1(startDate);
        endDateJson = MdsPersianDateTimePicker.getDateTimeJsonPersian1(endDate);
      }
      const startMonthName = MdsPersianDateTimePicker.getMonthName(statDateJson.month - 1, setting.isGregorian);
      const endMonthName = MdsPersianDateTimePicker.getMonthName(endDateJson.month - 1, setting.isGregorian);
      title = `${startMonthName} ${statDateJson.year} - ${endMonthName} ${endDateJson.year}`;
    }
    else
      title = `${MdsPersianDateTimePicker.getMonthName(selectedDateToShowJson.month - 1, setting.isGregorian)} ${selectedDateToShowJson.year}`;
    if (!setting.isGregorian)
      title = MdsPersianDateTimePicker.toPersianNumber(title);
    return title;
  }
  private setPopoverHeaderHtml = (element: Element, setting: MdsPersianDateTimePickerSetting, htmlString: string): void => {
    // element = المانی که روی آن فعالیتی انجام شده و باید عنوان تقویم آن عوض شود    
    if (this.bsPopover != null) {
      const popoverElement = this.getPopover(element);
      if (popoverElement == null) return;
      popoverElement.querySelector('[data-mds-dtp-title]')!.innerHTML = htmlString;
    } else if (setting.inLine) {
      let inlineTitleBox = element.closest('[data-mds-dtp-guid]')!.querySelector('[data-name="dtp-years-container"]')!;
      inlineTitleBox.innerHTML = htmlString;
      inlineTitleBox.classList.remove('w-0');
    }
    else if (setting.modalMode) {
      let inlineTitleBox = element.closest('[data-mds-dtp-guid]')!.querySelector('[data-mds-dtp-title] .modal-title')!;
      inlineTitleBox.innerHTML = htmlString;
    }
  }
  private getDateTimePickerBodyHtml(setting: MdsPersianDateTimePickerSetting): string {
    let selectedDateToShow = MdsPersianDateTimePicker.getClonedDate(setting.selectedDateToShow);
    let html = MdsPersianDateTimePicker.dateTimePickerHtmlTemplate;

    html = html.replace(/\{\{inlineAttr\}\}/img, setting.inLine ? 'data-inline' : '');
    html = html.replace(/\{\{rtlCssClass\}\}/img, setting.isGregorian ? '' : 'rtl');
    html = html.replace(/\{\{selectedDateStringAttribute\}\}/img, setting.inLine ? '' : 'hidden');
    html = html.replace(/\{\{goTodayText\}\}/img, setting.isGregorian ? MdsPersianDateTimePicker.goTodayText : MdsPersianDateTimePicker.goTodayTextPersian);
    html = html.replace(/\{\{timePickerAttribute\}\}/img, setting.enableTimePicker ? '' : 'hidden');

    const disabledDays = MdsPersianDateTimePicker.getDisabledDateObject(setting);
    let title = '';
    let todayDateString = '';
    let todayDateTimeJson: GetDateTimeJson1;
    let selectedDateTimeToShowJson: GetDateTimeJson1;
    let disableBeforeDateTimeJson: GetDateTimeJson1 | null = disabledDays[0];
    let disableAfterDateTimeJson: GetDateTimeJson1 | null = disabledDays[1];

    if (setting.isGregorian) {
      selectedDateTimeToShowJson = MdsPersianDateTimePicker.getDateTimeJson1(selectedDateToShow);
      todayDateTimeJson = MdsPersianDateTimePicker.getDateTimeJson1(new Date());
    } else {
      selectedDateTimeToShowJson = MdsPersianDateTimePicker.getDateTimeJsonPersian1(selectedDateToShow);
      todayDateTimeJson = MdsPersianDateTimePicker.getDateTimeJsonPersian1(new Date());
    }

    title = this.getPopoverHeaderTitle(setting);
    todayDateString = `${setting.isGregorian ? 'Today,' : 'امروز،'} ${todayDateTimeJson.day} ${MdsPersianDateTimePicker.getMonthName(todayDateTimeJson.month - 1, setting.isGregorian)} ${todayDateTimeJson.year}`;
    if (!setting.isGregorian) {
      todayDateString = MdsPersianDateTimePicker.toPersianNumber(todayDateString);
    }

    if (disableAfterDateTimeJson != undefined && disableAfterDateTimeJson.year <= selectedDateTimeToShowJson.year && disableAfterDateTimeJson.month < selectedDateTimeToShowJson.month)
      selectedDateToShow = setting.isGregorian ? new Date(disableAfterDateTimeJson.year, disableAfterDateTimeJson.month - 1, 1) : MdsPersianDateTimePicker.getDateTime1(disableAfterDateTimeJson.year, disableAfterDateTimeJson.month, disableAfterDateTimeJson.day);

    if (disableBeforeDateTimeJson != undefined && disableBeforeDateTimeJson.year >= selectedDateTimeToShowJson.year && disableBeforeDateTimeJson.month > selectedDateTimeToShowJson.month)
      selectedDateToShow = setting.isGregorian ? new Date(disableBeforeDateTimeJson.year, disableBeforeDateTimeJson.month - 1, 1) : MdsPersianDateTimePicker.getDateTime1(disableBeforeDateTimeJson.year, disableBeforeDateTimeJson.month, disableBeforeDateTimeJson.day);

    let monthsTdHtml = '';
    // let tempSelectedDateToShow = MdsPersianDateTimePicker.getClonedDate(selectedDateToShow);
    let numberOfNextMonths = setting.rangeSelectorMonthsToShow[1] <= 0 ? 0 : setting.rangeSelectorMonthsToShow[1];
    let numberOfPrevMonths = setting.rangeSelectorMonthsToShow[0] <= 0 ? 0 : setting.rangeSelectorMonthsToShow[0];
    numberOfPrevMonths *= -1;
    for (let i1 = numberOfPrevMonths; i1 < 0; i1++) {
      setting.selectedDateToShow = MdsPersianDateTimePicker.addMonthToDateTime(MdsPersianDateTimePicker.getClonedDate(selectedDateToShow), i1, setting.isGregorian);
      monthsTdHtml += this.getDateTimePickerMonthHtml(setting, false, true);
    }
    setting.selectedDateToShow = MdsPersianDateTimePicker.getClonedDate(selectedDateToShow);
    monthsTdHtml += this.getDateTimePickerMonthHtml(setting, false, false);
    for (let i2 = 1; i2 <= numberOfNextMonths; i2++) {
      setting.selectedDateToShow = MdsPersianDateTimePicker.addMonthToDateTime(MdsPersianDateTimePicker.getClonedDate(selectedDateToShow), i2, setting.isGregorian);
      monthsTdHtml += this.getDateTimePickerMonthHtml(setting, true, false);
    }
    // setting.selectedDateToShow = MdsPersianDateTimePicker.getClonedDate(selectedDateToShow);

    let totalMonthNumberToShow = Math.abs(numberOfPrevMonths) + 1 + numberOfNextMonths;
    let monthTdStyle = totalMonthNumberToShow > 1 ? 'width: ' + (100 / totalMonthNumberToShow).toString() + '%;' : '';

    monthsTdHtml = monthsTdHtml.replace(/\{\{monthTdStyle\}\}/img, monthTdStyle);

    html = html.replace(/\{\{dtpInlineHeader\}\}/img, title);
    html = html.replace(/\{\{todayDateString\}\}/img, todayDateString);
    html = html.replace(/\{\{time\}\}/img, `${MdsPersianDateTimePicker.zeroPad(selectedDateTimeToShowJson.hour)}:${MdsPersianDateTimePicker.zeroPad(selectedDateTimeToShowJson.minute)}`);
    html = html.replace(/\{\{monthsTdHtml\}\}/img, monthsTdHtml);

    return html;
  }
  private updateCalendarBodyHtml = (element: Element, setting: MdsPersianDateTimePickerSetting, updatePopoverContent = false): void => {
    const calendarHtml = this.getDateTimePickerBodyHtml(setting);
    const dtpInlineHeader = calendarHtml.match(/<th mds-dtp-inline-header\b[^>]*>(.*?)<\/th>/img)![0];
    this.tempTitleString = dtpInlineHeader;
    if (!setting.inLine && updatePopoverContent && !setting.modalMode) {
      const popover = this.getBsPopoverInstance();
      if (!popover) {
        console.error("mds.bs.datetimepicker: `BsPopoverInstance` is null!");
        return;
      }
      setTimeout(() => {
        popover.setContent({
          '.popover-header': dtpInlineHeader,
          '.popover-body': calendarHtml
        });
      }, 100);
      return;
    }
    let containerElement = element.closest('[data-name="mds-dtp-body"]');
    if (containerElement == null) {
      containerElement = element.closest('[data-mds-dtp-guid]');
      if (containerElement == null) {
        console.error("mds.bs.datetimepicker: `data-mds-dtp-guid` element not found !")
        return;
      }
      if (setting.modalMode)
        containerElement = containerElement.querySelector('[data-name="mds-dtp-body"]');
    }
    if (!containerElement) {
      console.error("mds.bs.datetimepicker: `data-mds-dtp-guid` element not found!")
      return;
    }
    this.setPopoverHeaderHtml(element, setting, dtpInlineHeader.trim());
    containerElement.innerHTML = calendarHtml;
    this.hideYearsBox(element, setting);
    this.enableEvents();
    this.enableInLineEvents();
  }
  private changeMonth = (element: Element): void => {
    const instance = MdsPersianDateTimePicker.getInstance(element);
    if (!instance || instance.setting.disabled)
      return;
    const dateNumber = Number(element.getAttribute('data-number'));
    const setting = instance.setting;
    let selectedDateToShow = MdsPersianDateTimePicker.getClonedDate(setting.selectedDateToShow);
    selectedDateToShow = MdsPersianDateTimePicker.getDateTime4(dateNumber, selectedDateToShow, setting.isGregorian);
    setting.selectedDateToShow = MdsPersianDateTimePicker.getClonedDate(selectedDateToShow);
    MdsPersianDateTimePickerData.set(instance.guid, instance);
    this.updateCalendarBodyHtml(element, setting);
    if (setting.calendarViewOnChange != undefined)
      setting.calendarViewOnChange(selectedDateToShow);
  }
  private selectDay = (element: Element): void => {
    // کلیک روی روزها
    // انتخاب روز
    const instance = MdsPersianDateTimePicker.getInstance(element);
    if (!instance) return;
    if (instance.setting.disabled || element.getAttribute('disabled') != undefined)
      return;
    let dateNumber = Number(element.getAttribute('data-number'));
    const setting = instance.setting;
    const disabled = element.getAttribute('disabled') != undefined;
    if (setting.selectedDate != undefined && !setting.enableTimePicker) {
      setting.selectedDate.setHours(0);
      setting.selectedDate.setMinutes(0);
      setting.selectedDate.setSeconds(0);
    }
    let selectedDateJson = !setting.selectedDate ? null : MdsPersianDateTimePicker.getDateTimeJson1(setting.selectedDate);
    let selectedDateToShow = !setting.selectedDateToShow ? new Date() : MdsPersianDateTimePicker.getClonedDate(setting.selectedDateToShow);
    let selectedDateToShowJson = MdsPersianDateTimePicker.getDateTimeJson1(selectedDateToShow);
    if (disabled) {
      if (setting.onDayClick != undefined) setting.onDayClick(setting);
      return;
    }
    selectedDateToShow = MdsPersianDateTimePicker.getDateTime4(dateNumber, selectedDateToShow, setting.isGregorian);
    if (setting.rangeSelector) { // اگر رنج سلکتور فعال بود
      if (setting.rangeSelectorStartDate != null && setting.rangeSelectorEndDate != null) {
        setting.selectedRangeDate = [];
        setting.rangeSelectorStartDate = null;
        setting.rangeSelectorEndDate = null;
        let closestSelector = '[data-mds-dtp]';
        if (setting.inLine)
          closestSelector = '[data-mds-dtp-guid]';
        element.closest(closestSelector)?.querySelectorAll('td.selected-range-days-start-end,td.selected-range-days')
          .forEach(e => {
            e.classList.remove('selected-range-days');
            e.classList.remove('selected-range-days-start-end');
          });
      }
      if (setting.rangeSelectorStartDate == undefined) {
        element.classList.add('selected-range-days-start-end');
        setting.rangeSelectorStartDate = MdsPersianDateTimePicker.getClonedDate(selectedDateToShow);
        setting.selectedDate = MdsPersianDateTimePicker.getClonedDate(selectedDateToShow);
        setting.selectedDateToShow = MdsPersianDateTimePicker.getClonedDate(selectedDateToShow);
      } else if (setting.rangeSelectorStartDate != undefined && setting.rangeSelectorEndDate == undefined) {
        if (setting.rangeSelectorStartDate.getTime() >= selectedDateToShow.getTime())
          return;
        element.classList.add('selected-range-days-start-end');
        setting.rangeSelectorEndDate = MdsPersianDateTimePicker.getClonedDate(selectedDateToShow);
        MdsPersianDateTimePicker.setSelectedData(setting);
      }
      MdsPersianDateTimePickerData.set(instance.guid, instance);
      if (setting.rangeSelectorStartDate != undefined && setting.rangeSelectorEndDate != undefined) {
        setting.selectedRangeDate = [MdsPersianDateTimePicker.getClonedDate(setting.rangeSelectorStartDate), MdsPersianDateTimePicker.getClonedDate(setting.rangeSelectorEndDate)];
        if (!setting.inLine)
          instance.hide();
        else
          this.updateCalendarBodyHtml(element, setting);
      }
      return;
    }
    setting.selectedDate = MdsPersianDateTimePicker.getClonedDate(selectedDateToShow);
    if (setting.selectedDate != undefined && !setting.enableTimePicker) {
      setting.selectedDate.setHours(0);
      setting.selectedDate.setMinutes(0);
      setting.selectedDate.setSeconds(0);
    }
    setting.selectedDateToShow = MdsPersianDateTimePicker.getClonedDate(selectedDateToShow);
    if (selectedDateJson != undefined) {
      if (setting.enableTimePicker) {
        selectedDateJson.hour = selectedDateToShowJson.hour;
        selectedDateJson.minute = selectedDateToShowJson.minute;
        selectedDateJson.second = selectedDateToShowJson.second;
      } else {
        selectedDateJson.hour = 0;
        selectedDateJson.minute = 0;
        selectedDateJson.second = 0;
      }
      setting.selectedDate.setHours(selectedDateJson.hour);
      setting.selectedDate.setMinutes(selectedDateJson.minute);
      setting.selectedDate.setSeconds(selectedDateJson.second);
    }
    MdsPersianDateTimePickerData.set(instance.guid, instance);
    MdsPersianDateTimePicker.setSelectedData(setting);
    element.setAttribute('data-mds-dtp-selected-day', '');
    if (setting.toDate || setting.fromDate) {
      // وقتی روی روز یکی از تقویم ها کلیک می شود
      // باید تقویم دیگر نیز تغییر کند و روزهایی از آن غیر فعال شود
      const toDateElement = document.querySelector(`[data-mds-dtp-group="${setting.groupId}"][data-to-date]`);
      const fromDateElement = document.querySelector(`[data-mds-dtp-group="${setting.groupId}"][data-from-date]`);
      if (setting.fromDate && toDateElement != undefined) {
        const toDateInstance = MdsPersianDateTimePicker.getInstance(toDateElement);
        if (toDateInstance != null) {
          if (setting.inLine)
            this.updateCalendarBodyHtml(toDateElement, toDateInstance.setting);
          else
            toDateInstance.initializeBsPopover(toDateInstance.setting);
        }
      } else if (setting.toDate && fromDateElement != undefined) {
        const fromDateInstance = MdsPersianDateTimePicker.getInstance(fromDateElement);
        if (fromDateInstance != null) {
          if (setting.inLine)
            this.updateCalendarBodyHtml(fromDateElement, fromDateInstance.setting);
          else
            fromDateInstance.initializeBsPopover(fromDateInstance.setting);
        }
      } else
        this.updateCalendarBodyHtml(element, setting);
    } else
      this.updateCalendarBodyHtml(element, setting, true);
    if (setting.onDayClick != undefined)
      setting.onDayClick(setting);
    if (!setting.inLine) {
      instance.hide();
    } else {
      // حذف روزهای انتخاب شده در تقویم این لاین
      let dtp = element.closest(`[data-mds-dtp-guid="${this.guid}"]`);
      if (!dtp)
        dtp = document.querySelector(`[data-mds-dtp-guid="${this.guid}"]`);
      dtp!.querySelectorAll(`[data-day]:not([data-number="${element.getAttribute('data-number')}"])`)
        .forEach(e => e.removeAttribute('data-mds-dtp-selected-day'));
    }
  }
  private hoverOnDays = (e: Event): void => {
    // هاور روی روزها
    const element = <Element>e.target;
    const instance = MdsPersianDateTimePicker.getInstance(element);
    if (!instance) return;
    const setting = instance.setting;

    if (element.getAttribute('disabled') != undefined || !setting.rangeSelector ||
      (setting.rangeSelectorStartDate != undefined && setting.rangeSelectorEndDate != undefined)) return;

    const dateNumber = Number(element.getAttribute('data-number'));
    const allDayElements: Element[] = [].slice.call(document.querySelectorAll('td[data-day]'));
    allDayElements.forEach(e => {
      e.classList.remove('selected-range-days');
      e.classList.remove('selected-range-days-nm');
    });

    const allNextOrPrevMonthDayElements: Element[] = [].slice.call(document.querySelectorAll('td[data-nm]'));
    allNextOrPrevMonthDayElements.forEach(e => {
      e.classList.remove('selected-range-days');
      e.classList.remove('selected-range-days-nm');
    });

    const rangeSelectorStartDate = !setting.rangeSelectorStartDate ? undefined : MdsPersianDateTimePicker.getClonedDate(setting.rangeSelectorStartDate);
    const rangeSelectorEndDate = !setting.rangeSelectorEndDate ? undefined : MdsPersianDateTimePicker.getClonedDate(setting.rangeSelectorEndDate);
    let rangeSelectorStartDateNumber = 0;
    let rangeSelectorEndDateNumber = 0;

    if (setting.isGregorian) {
      rangeSelectorStartDateNumber = !rangeSelectorStartDate ? 0 : MdsPersianDateTimePicker.convertToNumber3(rangeSelectorStartDate);
      rangeSelectorEndDateNumber = !rangeSelectorEndDate ? 0 : MdsPersianDateTimePicker.convertToNumber3(rangeSelectorEndDate);
    } else {
      rangeSelectorStartDateNumber = !rangeSelectorStartDate ? 0 : MdsPersianDateTimePicker.convertToNumber1(MdsPersianDateTimePicker.getDateTimeJsonPersian1(rangeSelectorStartDate));
      rangeSelectorEndDateNumber = !rangeSelectorEndDate ? 0 : MdsPersianDateTimePicker.convertToNumber1(MdsPersianDateTimePicker.getDateTimeJsonPersian1(rangeSelectorEndDate));
    }

    if (rangeSelectorStartDateNumber > 0 && dateNumber > rangeSelectorStartDateNumber) {
      for (var i1 = rangeSelectorStartDateNumber; i1 <= dateNumber; i1++) {
        allDayElements.filter(e => e.getAttribute('data-number') == i1.toString() && e.classList.value.indexOf('selected-range-days-start-end') <= -1)
          .forEach(e => e.classList.add('selected-range-days'));
        allNextOrPrevMonthDayElements.filter(e => e.getAttribute('data-number') == i1.toString() && e.classList.value.indexOf('selected-range-days-start-end') <= -1)
          .forEach(e => e.classList.add('selected-range-days-nm'));
      }
    } else if (rangeSelectorEndDateNumber > 0 && dateNumber < rangeSelectorEndDateNumber) {
      for (var i2 = dateNumber; i2 <= rangeSelectorEndDateNumber; i2++) {
        allDayElements.filter(e => e.getAttribute('data-number') == i2.toString() && e.classList.value.indexOf('selected-range-days-start-end') <= -1)
          .forEach(e => e.classList.add('selected-range-days'));
        allNextOrPrevMonthDayElements.filter(e => e.getAttribute('data-number') == i2.toString() && e.classList.value.indexOf('selected-range-days-start-end') <= -1)
          .forEach(e => e.classList.add('selected-range-days-nm'));
      }
    }

  }
  private goToday = (e: Event): void => {
    const element = <Element>e.target;
    const instance = MdsPersianDateTimePicker.getInstance(element);
    if (!instance)
      return;
    const setting = instance.setting;
    setting.selectedDateToShow = new Date();
    MdsPersianDateTimePickerData.set(instance.guid, instance);
    this.updateCalendarBodyHtml(element, setting);
  }
  private timeChanged = (e: Event): void => {
    // عوض کردن ساعت
    const element = <Element>e.target;
    const instance = MdsPersianDateTimePicker.getInstance(element);
    if (!instance) return;
    const setting = instance.setting;
    const value: string = (<any>element).value;
    if (!setting.enableTimePicker) return;
    if (setting.selectedDateToShow == undefined)
      setting.selectedDateToShow = new Date();
    let hour = Number(value.substr(0, 2));
    let minute = Number(value.substr(3, 2));
    setting.selectedDateToShow = new Date(setting.selectedDateToShow.setHours(hour));
    setting.selectedDateToShow = new Date(setting.selectedDateToShow.setMinutes(minute));
    if (setting.selectedDate == undefined)
      setting.selectedDate = new Date();
    setting.selectedDate = new Date(setting.selectedDate.setHours(hour));
    setting.selectedDate = new Date(setting.selectedDate.setMinutes(minute));
    MdsPersianDateTimePickerData.set(instance.guid, instance);
    MdsPersianDateTimePicker.setSelectedData(setting);
  }
  private enableMainEvents(): void {
    if (this.setting.inLine) return;
    if (this.bsPopover != null) {
      this.element.addEventListener('shown.bs.popover', this.popoverOrModalShownEvent);
      this.element.addEventListener('hidden.bs.popover', this.popoverOrModalHiddenEvent);
      this.element.addEventListener('inserted.bs.popover', this.popoverInsertedEvent);
      this.element.addEventListener('click', this.showPopoverEvent, true);
    } else if (this.bsModal != null) {
      const modalElement = this.getModal();
      if (modalElement == null) {
        console.error("mds.bs.datetimepicker: `modalElement` not found!");
        return;
      }
      modalElement.addEventListener('shown.bs.modal', this.popoverOrModalShownEvent);
      modalElement.addEventListener('hidden.bs.modal', this.popoverOrModalHiddenEvent);
    }
  }
  private popoverInsertedEvent = (e: Event): void => {
    const element = <Element>e.target;
    const instance = MdsPersianDateTimePicker.getInstance(element);
    if (!instance) return;
    const setting = instance.setting;
    this.hideYearsBox(element, setting);
  }
  private popoverOrModalShownEvent = (): void => {
    this.enableEvents();
  }
  private popoverOrModalHiddenEvent = (): void => {
    this.disableEvents();
  }
  private enableInLineEvents(): void {
    if (!this.setting.inLine) return;
    setTimeout(() => {
      const dtp = document.querySelector(`[data-mds-dtp-guid="${this.guid}"]`);
      if (dtp != null) {
        dtp.querySelector('[data-mds-dtp-time]')?.addEventListener('change', this.timeChanged, false);
        dtp.addEventListener('click', this.selectCorrectClickEvent);
        dtp.querySelectorAll('[data-day]').forEach(e => e.addEventListener('mouseenter', this.hoverOnDays, true));
      }
    }, 100);
  }
  private enableEvents(): void {
    if (this.setting.inLine) return;
    setTimeout(() => {
      document.addEventListener('click', this.selectCorrectClickEvent, false);
      document.querySelector('html')!.addEventListener('click', this.hidePopoverEvent, true);
      document.querySelectorAll('[data-mds-dtp-time]').forEach(e => e.addEventListener('change', this.timeChanged, false));
      document.querySelectorAll('[data-mds-dtp] [data-day]').forEach(e => e.addEventListener('mouseenter', this.hoverOnDays, true));
    }, 500);
  }
  private disableEvents(): void {
    document.removeEventListener('click', this.selectCorrectClickEvent);
    document.querySelector('html')!.removeEventListener('click', this.hidePopoverEvent);
    document.querySelectorAll('[data-mds-dtp-time]')?.forEach(e => e.removeEventListener('change', this.timeChanged));
    document.querySelectorAll('[data-mds-dtp] [data-day]').forEach(e => e.removeEventListener('mouseenter', this.hoverOnDays));
    const dtp = document.querySelector(`[data-mds-dtp-guid="${this.guid}"]`);
    if (dtp != null) {
      dtp.removeEventListener('click', this.selectCorrectClickEvent, false);
      dtp.querySelectorAll('[data-day]')?.forEach(e => e.removeEventListener('mouseenter', this.hoverOnDays, true));
    }
  }
  private selectCorrectClickEvent = (e: Event): void => {
    const element = <Element>e.target;
    const instance = MdsPersianDateTimePicker.getInstance(element);
    if (!instance) return;
    if (instance != null && (instance.setting.disabled || instance.element.getAttribute('disabled') != undefined))
      return;
    if (element.getAttribute('mds-pdtp-select-year-button') != null) {
      instance.showYearsBox(element);
    } else if (element.getAttribute('data-mds-dtp-go-today') != null) {
      this.goToday(e);
    } else if (element.getAttribute('data-day') != null) {
      this.selectDay(element);
    } else if (element.getAttribute('data-mds-hide-year-list-box')) {
      this.hideYearsBox(element, instance.setting);
    } else if (element.getAttribute('data-change-date-button')) {
      this.changeMonth(element);
    } else if (element.getAttribute('data-year-range-button-change') != null && element.getAttribute('disabled') == null) {
      this.changeYearList(element);
    }
  }
  private showPopoverEvent = (e: Event): void => {
    MdsPersianDateTimePickerData.getAll().forEach(i => i.hide());
    const element = <Element>e.target;
    const instance = MdsPersianDateTimePicker.getInstance(element);
    if (!instance || instance.setting.disabled)
      return;
    instance.show();
  }
  private hidePopoverEvent = (e: Event): void => {
    const element = <Element>e.target;
    if (element.tagName == 'HTML') {
      MdsPersianDateTimePickerData.getAll().forEach(i => !i.setting.modalMode ? i.hide() : () => { });
      return;
    }
    const isWithinDatePicker = element.closest('[data-mds-dtp]') != null || element.getAttribute('data-mds-dtp-guid') != null || element.getAttribute('data-mds-dtp-go-today') != null;
    if (!isWithinDatePicker) {
      MdsPersianDateTimePickerData.getAll().forEach(i => i.hide());
    }
  }

  /**
   * نمایش تقویم
   */
  show(): void {
    this.bsModal?.show();
    this.bsPopover?.show();
  }
  /**
   * مخفی کردن تقویم
   */
  hide(): void {
    this.bsModal?.hide();
    this.bsPopover?.hide();
  }
  /**
   * مخفی یا نمایش تقویم 
   */
  toggle(): void {
    if (this.bsPopover == null) return;
    this.bsPopover.toggle();
  }
  /**
   * فعال کردن تقویم
   */
  enable(): void {
    this.setting.disabled = false;
    this.element.removeAttribute("disabled");
    MdsPersianDateTimePickerData.set(this.guid, this);
    if (this.bsPopover != null)
      this.bsPopover.enable();
  }
  /**
   * غیر فعال کردن تقویم
   */
  disable(): void {
    this.setting.disabled = true;
    this.element.setAttribute("disabled", '');
    MdsPersianDateTimePickerData.set(this.guid, this);
    if (this.bsPopover != null)
      this.bsPopover.disable();
  }
  /**
   * بروز کردن محل قرار گرفتن تقویم
   */
  updatePosition(): void {
    this.bsPopover?.update();
    this.bsModal?.handleUpdate();
  }
  /**
   * به روز کردن متن نمایش تاریخ روز انتخاب شده
   */
  updateSelectedDateText(): void {
    MdsPersianDateTimePicker.setSelectedData(this.setting);
  }
  /**
   * از بین بردن تقویم
   */
  dispose(): void {
    if (this.bsPopover != null)
      this.bsPopover.dispose();
    if (this.bsModal != null)
      this.bsModal.dispose();
    this.element.removeEventListener('click', this.showPopoverEvent);
    this.bsPopover = null;
    this.bsModal = null;
  }
  /**
   * دریافت اینستنس پاپ آور بوت استرپ
   */
  getBsPopoverInstance(): Popover | null {
    return this.bsPopover;
  }
  /**
   * دریافت اینستنس مدال بوت استرپ
   * در صورتی که آپشن modalMode را صحیح کرده باشید
   */
  getBsModalInstance(): Modal | null {
    return this.bsModal;
  }
  /**
   * دریافت متن تاریخ انتخاب شده
   */
  getText(): string {
    return MdsPersianDateTimePicker.getSelectedDateFormatted(this.setting);
  }
  /**
   * دریافت آبجکت تاریخ انتخاب شده
   */
  getSelectedDate(): Date | null {
    return this.setting.selectedDate;
  }
  /**
   * دریافت آبجکت های تاریخ های انتخاب شده در مد رنج سلکتور
   */
  getSelectedDateRange(): Date[] {
    return this.setting.selectedRangeDate;
  }
  /**
  * بروز کردن تاریخ انتخاب شده
  */
  setDate(date: Date): void {
    this.updateOptions({
      selectedDate: date,
      selectedDateToShow: date
    });
  }
  /**
  * بروز کردن تاریخ انتخاب شده با استفاده از تاریخ شمسی
  */
  setDatePersian(yearPersian: number, monthPersian: number, dayPersian: number): void {
    const gregorianDateJson = MdsPersianDateTimePicker.toGregorian(yearPersian, monthPersian, dayPersian);
    console.log(gregorianDateJson);
    const date = new Date(gregorianDateJson.gy, gregorianDateJson.gm - 1, gregorianDateJson.gd);
    this.updateOptions({
      selectedDate: date,
      selectedDateToShow: date
    });
  }
  /**
  * بروز کردن رنج تاریخی انتخاب شده
  */
  setDateRange(startDate: Date, endDate: Date): void {
    this.updateOptions({
      selectedDate: startDate,
      selectedDateToShow: startDate,
      selectedRangeDate: [startDate, endDate]
    });
  }
  /**
  * حذف تاریخ انتخاب شده
  */
  clearDate(): void {
    this.updateOptions({
      selectedDate: null,
      selectedDateToShow: new Date(),
    });
  }
  /**
   * بروز کردن تنظیمات تقویم
   * @param optionName نام آپشن مورد نظر
   * @param value مقدار
   */
  updateOption(optionName: string, value: any): void {
    if (!optionName) return;
    value = MdsPersianDateTimePicker.correctOptionValue(optionName, value);
    (<any>this.setting)[optionName] = value;
    MdsPersianDateTimePickerData.set(this.guid, this);
    this.initializeBsPopover(this.setting);
  }
  /**
   * بروز کردن تنظیمات تقویم
   * @param options تنظیمات مورد نظر
   */
  updateOptions(options: any): void {
    Object.keys(options).forEach((key) => {
      (<any>this.setting)[key] = MdsPersianDateTimePicker.correctOptionValue(key, (<any>options)[key]);
    });
    MdsPersianDateTimePickerData.set(this.guid, this);
    this.initializeBsPopover(this.setting);
  }
  /**
 * تبدیل آبجکت تاریخ به رشته
 * @param date آبجکت تاریخ
 * @param isGregorian آیا تاریخ میلادی مد نظر است یا تبدیل به شمسی شود
 * @param format فرمت مورد نظر برای تبدیل تاریخ به رشته
 */
  static convertDateToString = (date: Date, isGregorian: boolean, format: string): string => {
    return MdsPersianDateTimePicker.getDateTimeString(!isGregorian ? MdsPersianDateTimePicker.getDateTimeJsonPersian1(date) : MdsPersianDateTimePicker.getDateTimeJson1(date), format, isGregorian, !isGregorian);
  };
  /**
 * تبدیل آبجکت تاریخ به شمسی
 * @param date آبجکت تاریخ
 */
  static convertDateToJalali = (date: Date): MdsPersianDateTimePickerConvertedDateModel => {
    const dateTimeJson1 = MdsPersianDateTimePicker.getDateTimeJson1(date);
    const jalaliJsonModel = MdsPersianDateTimePicker.toJalali(dateTimeJson1.year, dateTimeJson1.month, dateTimeJson1.day);
    return {
      year: jalaliJsonModel.jy,
      month: jalaliJsonModel.jm,
      day: jalaliJsonModel.jd,
    }
  };
  /**
   * دریافت اینستنس تقویم از روی المانی که تقویم روی آن فعال شده است
   * @param element المانی که تقویم روی آن فعال شده
   * @returns اینستنس تقویم
   */
  static getInstance(element: Element): MdsPersianDateTimePicker | null {
    let elementGuid = element.getAttribute('data-mds-dtp-guid');
    if (!elementGuid) {
      elementGuid = element.closest('[data-mds-dtp-guid]')?.getAttribute('data-mds-dtp-guid') ?? null;
      if (!elementGuid) {
        const id = element.closest('[data-mds-dtp]')?.getAttribute('id');
        if (!id)
          return null;
        elementGuid = document.querySelector('[aria-describedby="' + id + '"]')?.getAttribute('data-mds-dtp-guid') ?? null;
        if (!elementGuid)
          return null;
      }
    };
    return MdsPersianDateTimePickerData.get(elementGuid);
  }

  // #endregion
}

interface GetDateTimeJson1 {
  year: number,
  month: number,
  day: number,
  hour: number,
  minute: number,
  second: number,
  millisecond: number,
  dayOfWeek: number
}

interface JalaliJsonModel {
  jy: number,
  jm: number,
  jd: number
}


interface GregorianJsonModel {
  gy: number,
  gm: number,
  gd: number
}

interface JalCalModel {
  leap: number,
  gy: number,
  march: number
}

interface MdsPersianDateTimePickerYearToSelect {
  yearStart: number,
  yearEnd: number,
  html: string
}

type PopoverPlacement = 'auto' | 'top' | 'bottom' | 'left' | 'right';

export class MdsPersianDateTimePickerSetting {
  /**
   * محل قرار گرفتن تقویم
   */
  placement: PopoverPlacement | (() => PopoverPlacement) | undefined = 'bottom';
  /**
   * فعال بودن تایم پیکر
   */
  enableTimePicker = false;
  /**
   * سلکتور نمایش روز انتخاب شده
   */
  targetTextSelector = '';
  /**
   * سلکتور ذخیره تاریخ میلادی، برای روز انتخاب شده
   */
  targetDateSelector = '';
  /**
   * آیا تقویم برای کنترل روز پایانی تاریخ است
   */
  toDate = false;
  /**
   * آیا تقویم برای کنترل روز شروع تاریخ است
   */
  fromDate = false;
  /**
   * شناسه گروه در حالتی که از 
   * toDate
   * و
   * fromDate
   * استفاده شده است
   */
  groupId = '';
  /**
   * آیا تقویم غیر فعال است؟
   */
  disabled = false;
  /**
   * فرمت نمایش روز انتخاب شده تقویم
   */
  textFormat = '';
  /**
   * فرمت ذخیره تاریخ میلادی انتخاب شده
   */
  dateFormat = '';
  /**
   * آیا تقویم میلادی استفاده شود؟
   */
  isGregorian = false;
  /**
   * آیا تقویم به صورت این لاین نمایش داده شود؟
   */
  inLine = false;
  /**
   * تاریخ انتخاب شده
   */
  selectedDate: Date | null = null;
  /**
   * تاریخی که نمایش تقویم از آن شروع می شود
   */
  selectedDateToShow = new Date();
  /**
   * تعداد سال های قابل نمایش در لیست سال های قابل انتخاب
   */
  yearOffset = 15;
  /**
   * تاریخ میلادی روزهای تعطیل
   */
  holidays: Date[] = [];
  /**
   * تاریخ میلادی روزهای غیر فعال
   */
  disabledDates: Date[] = [];
  /**
   * عدد روزهایی از هفته که غیر فعال هستند
   */
  disabledDays: number[] = [];
  /**
   * تاریخ میلادی روزهای خاص
   */
  specialDates: Date[] = [];
  /**
   * آیا روزهای قبل از امروز غیر فعال شوند؟
   */
  disableBeforeToday = false;
  /**
   * آیا روزهای بعد از امروز غیر فعال شوند؟
   */
  disableAfterToday = false;
  /**
   * روزهای قبل از این تاریخ غیر فعال شود
   */
  disableBeforeDate: Date | null = null;
  /**
   * روزهای بعد از این تاریخ غیر فعال شود
   */
  disableAfterDate: Date | null = null;
  /**
   * آیا تقویم به صورت انتخاب بازه نمایش داده شود؟
   */
  rangeSelector = false;
  /**
   * تاریخ شروع تقویم در مد انتخاب بازه تاریخی برای نمایش
   */
  rangeSelectorStartDate: Date | null = null;
  /**
   * تاریخ پایان تقویم در مد انتخاب بازه تاریخی برای نمایش
   */
  rangeSelectorEndDate: Date | null = null;
  /**
   * تعداد ماه های قابل نمایش در قابلیت انتخاب بازه تاریخی
   */
  rangeSelectorMonthsToShow: number[] = [0, 0];
  /**
   * تاریخ های انتخاب شده در مد بازه انتخابی
   */
  selectedRangeDate: Date[] = [];
  /**
   * آیا تقویم به صورت مدال نمایش داده شود
   */
  modalMode = false;
  /**
   * تبدیل اعداد به فارسی
   */
  persianNumber = false;
  /**
   * رویداد عوض شدن ماه و تاریخ در دیت پیکر
   * @param _ تاریخ ماه انتخابی
   */
  calendarViewOnChange = (_: Date) => { };
  /**
   * رویداد انتخاب روز در دیت پیکر
   * @param _ تمامی تنظیمات دیت پیکر
   */
  onDayClick = (_: MdsPersianDateTimePickerSetting) => { }
}

export interface MdsPersianDateTimePickerConvertedDateModel {
  year: number,
  month: number,
  day: number,
}

const MdsPersianDateTimePickerElementMap = new Map();
var MdsPersianDateTimePickerData = {
  set(key: string, instance: MdsPersianDateTimePicker): void {
    if (!MdsPersianDateTimePickerElementMap.has(key)) {
      MdsPersianDateTimePickerElementMap.set(key, instance);
      return;
    }
    MdsPersianDateTimePickerElementMap.set(key, instance);
  },
  get(key: string): MdsPersianDateTimePicker {
    return MdsPersianDateTimePickerElementMap.get(key) || null;
  },
  getAll(): MdsPersianDateTimePicker[] {
    return Array.from(MdsPersianDateTimePickerElementMap, ([_name, value]) => value);
  },
  remove(key: string): void {
    if (!MdsPersianDateTimePickerElementMap.has(key)) {
      return;
    }
    MdsPersianDateTimePickerElementMap.delete(key);
  }
};
