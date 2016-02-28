/*! jQuery UI - v1.11.4 - 2015-03-11
* http://jqueryui.com
* Includes: datepicker-af.js, datepicker-ar-DZ.js, datepicker-ar.js, datepicker-az.js, datepicker-be.js, datepicker-bg.js, datepicker-bs.js, datepicker-ca.js, datepicker-cs.js, datepicker-cy-GB.js, datepicker-da.js, datepicker-de.js, datepicker-el.js, datepicker-en-AU.js, datepicker-en-GB.js, datepicker-en-NZ.js, datepicker-eo.js, datepicker-es.js, datepicker-et.js, datepicker-eu.js, datepicker-fa.js, datepicker-fi.js, datepicker-fo.js, datepicker-fr-CA.js, datepicker-fr-CH.js, datepicker-fr.js, datepicker-gl.js, datepicker-he.js, datepicker-hi.js, datepicker-hr.js, datepicker-hu.js, datepicker-hy.js, datepicker-id.js, datepicker-is.js, datepicker-it-CH.js, datepicker-it.js, datepicker-ja.js, datepicker-ka.js, datepicker-kk.js, datepicker-km.js, datepicker-ko.js, datepicker-ky.js, datepicker-lb.js, datepicker-lt.js, datepicker-lv.js, datepicker-mk.js, datepicker-ml.js, datepicker-ms.js, datepicker-nb.js, datepicker-nl-BE.js, datepicker-nl.js, datepicker-nn.js, datepicker-no.js, datepicker-pl.js, datepicker-pt-BR.js, datepicker-pt.js, datepicker-rm.js, datepicker-ro.js, datepicker-ru.js, datepicker-sk.js, datepicker-sl.js, datepicker-sq.js, datepicker-sr-SR.js, datepicker-sr.js, datepicker-sv.js, datepicker-ta.js, datepicker-th.js, datepicker-tj.js, datepicker-tr.js, datepicker-uk.js, datepicker-vi.js, datepicker-zh-CN.js, datepicker-zh-HK.js, datepicker-zh-TW.js
* Copyright 2015 jQuery Foundation and other contributors; Licensed MIT */

(function (factory) {
    if (typeof define === "function" && define.amd) {

        // AMD. Register as an anonymous module.
        define(["jquery"], factory);
    } else {

        // Browser globals
        factory(jQuery);
    }
}(function ($) {

    var datepicker = $.datepicker;

    /* Afrikaans initialisation for the jQuery UI date picker plugin. */
    /* Written by Renier Pretorius. */


    datepicker.regional['af'] = {
        closeText: 'Selekteer',
        prevText: 'Vorige',
        nextText: 'Volgende',
        currentText: 'Vandag',
        monthNames: ['Januarie', 'Februarie', 'Maart', 'April', 'Mei', 'Junie',
        'Julie', 'Augustus', 'September', 'Oktober', 'November', 'Desember'],
        monthNamesShort: ['Jan', 'Feb', 'Mrt', 'Apr', 'Mei', 'Jun',
        'Jul', 'Aug', 'Sep', 'Okt', 'Nov', 'Des'],
        dayNames: ['Sondag', 'Maandag', 'Dinsdag', 'Woensdag', 'Donderdag', 'Vrydag', 'Saterdag'],
        dayNamesShort: ['Son', 'Maa', 'Din', 'Woe', 'Don', 'Vry', 'Sat'],
        dayNamesMin: ['So', 'Ma', 'Di', 'Wo', 'Do', 'Vr', 'Sa'],
        weekHeader: 'Wk',
        dateFormat: 'dd/mm/yy',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    datepicker.setDefaults(datepicker.regional['af']);

    var i18nDatepickerAf = datepicker.regional['af'];


    /* Algerian Arabic Translation for jQuery UI date picker plugin. (can be used for Tunisia)*/
    /* Mohamed Cherif BOUCHELAGHEM -- cherifbouchelaghem@yahoo.fr */



    datepicker.regional['ar-DZ'] = {
        closeText: 'Ø¥ØºÙ„Ø§Ù‚',
        prevText: '&#x3C;Ø§Ù„Ø³Ø§Ø¨Ù‚',
        nextText: 'Ø§Ù„ØªØ§Ù„ÙŠ&#x3E;',
        currentText: 'Ø§Ù„ÙŠÙˆÙ…',
        monthNames: ['Ø¬Ø§Ù†ÙÙŠ', 'ÙÙŠÙØ±ÙŠ', 'Ù…Ø§Ø±Ø³', 'Ø£ÙØ±ÙŠÙ„', 'Ù…Ø§ÙŠ', 'Ø¬ÙˆØ§Ù†',
        'Ø¬ÙˆÙŠÙ„ÙŠØ©', 'Ø£ÙˆØª', 'Ø³Ø¨ØªÙ…Ø¨Ø±', 'Ø£ÙƒØªÙˆØ¨Ø±', 'Ù†ÙˆÙÙ…Ø¨Ø±', 'Ø¯ÙŠØ³Ù…Ø¨Ø±'],
        monthNamesShort: ['1', '2', '3', '4', '5', '6', '7', '8', '9', '10', '11', '12'],
        dayNames: ['Ø§Ù„Ø£Ø­Ø¯', 'Ø§Ù„Ø§Ø«Ù†ÙŠÙ†', 'Ø§Ù„Ø«Ù„Ø§Ø«Ø§Ø¡', 'Ø§Ù„Ø£Ø±Ø¨Ø¹Ø§Ø¡', 'Ø§Ù„Ø®Ù…ÙŠØ³', 'Ø§Ù„Ø¬Ù…Ø¹Ø©', 'Ø§Ù„Ø³Ø¨Øª'],
        dayNamesShort: ['Ø§Ù„Ø£Ø­Ø¯', 'Ø§Ù„Ø§Ø«Ù†ÙŠÙ†', 'Ø§Ù„Ø«Ù„Ø§Ø«Ø§Ø¡', 'Ø§Ù„Ø£Ø±Ø¨Ø¹Ø§Ø¡', 'Ø§Ù„Ø®Ù…ÙŠØ³', 'Ø§Ù„Ø¬Ù…Ø¹Ø©', 'Ø§Ù„Ø³Ø¨Øª'],
        dayNamesMin: ['Ø§Ù„Ø£Ø­Ø¯', 'Ø§Ù„Ø§Ø«Ù†ÙŠÙ†', 'Ø§Ù„Ø«Ù„Ø§Ø«Ø§Ø¡', 'Ø§Ù„Ø£Ø±Ø¨Ø¹Ø§Ø¡', 'Ø§Ù„Ø®Ù…ÙŠØ³', 'Ø§Ù„Ø¬Ù…Ø¹Ø©', 'Ø§Ù„Ø³Ø¨Øª'],
        weekHeader: 'Ø£Ø³Ø¨ÙˆØ¹',
        dateFormat: 'dd/mm/yy',
        firstDay: 6,
        isRTL: true,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    datepicker.setDefaults(datepicker.regional['ar-DZ']);

    var i18nDatepickerArDz = datepicker.regional['ar-DZ'];


    /* Arabic Translation for jQuery UI date picker plugin. */
    /* Used in most of Arab countries, primarily in Bahrain, Kuwait, Oman, Qatar, Saudi Arabia and the United Arab Emirates, Egypt, Sudan and Yemen. */
    /* Written by Mohammed Alshehri -- m@dralshehri.com */



    datepicker.regional['ar'] = {
        closeText: 'Ø¥ØºÙ„Ø§Ù‚',
        prevText: '&#x3C;Ø§Ù„Ø³Ø§Ø¨Ù‚',
        nextText: 'Ø§Ù„ØªØ§Ù„ÙŠ&#x3E;',
        currentText: 'Ø§Ù„ÙŠÙˆÙ…',
        monthNames: ['ÙŠÙ†Ø§ÙŠØ±', 'ÙØ¨Ø±Ø§ÙŠØ±', 'Ù…Ø§Ø±Ø³', 'Ø£Ø¨Ø±ÙŠÙ„', 'Ù…Ø§ÙŠÙˆ', 'ÙŠÙˆÙ†ÙŠÙˆ',
        'ÙŠÙˆÙ„ÙŠÙˆ', 'Ø£ØºØ³Ø·Ø³', 'Ø³Ø¨ØªÙ…Ø¨Ø±', 'Ø£ÙƒØªÙˆØ¨Ø±', 'Ù†ÙˆÙÙ…Ø¨Ø±', 'Ø¯ÙŠØ³Ù…Ø¨Ø±'],
        monthNamesShort: ['1', '2', '3', '4', '5', '6', '7', '8', '9', '10', '11', '12'],
        dayNames: ['Ø§Ù„Ø£Ø­Ø¯', 'Ø§Ù„Ø§Ø«Ù†ÙŠÙ†', 'Ø§Ù„Ø«Ù„Ø§Ø«Ø§Ø¡', 'Ø§Ù„Ø£Ø±Ø¨Ø¹Ø§Ø¡', 'Ø§Ù„Ø®Ù…ÙŠØ³', 'Ø§Ù„Ø¬Ù…Ø¹Ø©', 'Ø§Ù„Ø³Ø¨Øª'],
        dayNamesShort: ['Ø£Ø­Ø¯', 'Ø§Ø«Ù†ÙŠÙ†', 'Ø«Ù„Ø§Ø«Ø§Ø¡', 'Ø£Ø±Ø¨Ø¹Ø§Ø¡', 'Ø®Ù…ÙŠØ³', 'Ø¬Ù…Ø¹Ø©', 'Ø³Ø¨Øª'],
        dayNamesMin: ['Ø­', 'Ù†', 'Ø«', 'Ø±', 'Ø®', 'Ø¬', 'Ø³'],
        weekHeader: 'Ø£Ø³Ø¨ÙˆØ¹',
        dateFormat: 'dd/mm/yy',
        firstDay: 0,
        isRTL: true,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    datepicker.setDefaults(datepicker.regional['ar']);

    var i18nDatepickerAr = datepicker.regional['ar'];


    /* Azerbaijani (UTF-8) initialisation for the jQuery UI date picker plugin. */
    /* Written by Jamil Najafov (necefov33@gmail.com). */


    datepicker.regional['az'] = {
        closeText: 'BaÄŸla',
        prevText: '&#x3C;Geri',
        nextText: 'Ä°rÉ™li&#x3E;',
        currentText: 'BugÃ¼n',
        monthNames: ['Yanvar', 'Fevral', 'Mart', 'Aprel', 'May', 'Ä°yun',
        'Ä°yul', 'Avqust', 'Sentyabr', 'Oktyabr', 'Noyabr', 'Dekabr'],
        monthNamesShort: ['Yan', 'Fev', 'Mar', 'Apr', 'May', 'Ä°yun',
        'Ä°yul', 'Avq', 'Sen', 'Okt', 'Noy', 'Dek'],
        dayNames: ['Bazar', 'Bazar ertÉ™si', 'Ã‡É™rÅŸÉ™nbÉ™ axÅŸamÄ±', 'Ã‡É™rÅŸÉ™nbÉ™', 'CÃ¼mÉ™ axÅŸamÄ±', 'CÃ¼mÉ™', 'ÅžÉ™nbÉ™'],
        dayNamesShort: ['B', 'Be', 'Ã‡a', 'Ã‡', 'Ca', 'C', 'Åž'],
        dayNamesMin: ['B', 'B', 'Ã‡', 'Ð¡', 'Ã‡', 'C', 'Åž'],
        weekHeader: 'Hf',
        dateFormat: 'dd.mm.yy',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    datepicker.setDefaults(datepicker.regional['az']);

    var i18nDatepickerAz = datepicker.regional['az'];


    /* Belarusian initialisation for the jQuery UI date picker plugin. */
    /* Written by Pavel Selitskas <p.selitskas@gmail.com> */


    datepicker.regional['be'] = {
        closeText: 'Ð—Ð°Ñ‡Ñ‹Ð½Ñ–Ñ†ÑŒ',
        prevText: '&larr;ÐŸÐ°Ð¿ÑÑ€.',
        nextText: 'ÐÐ°ÑÑ‚.&rarr;',
        currentText: 'Ð¡Ñ‘Ð½ÑŒÐ½Ñ',
        monthNames: ['Ð¡Ñ‚ÑƒÐ´Ð·ÐµÐ½ÑŒ', 'Ð›ÑŽÑ‚Ñ‹', 'Ð¡Ð°ÐºÐ°Ð²Ñ–Ðº', 'ÐšÑ€Ð°ÑÐ°Ð²Ñ–Ðº', 'Ð¢Ñ€Ð°Ð²ÐµÐ½ÑŒ', 'Ð§ÑÑ€Ð²ÐµÐ½ÑŒ',
        'Ð›Ñ–Ð¿ÐµÐ½ÑŒ', 'Ð–Ð½Ñ–Ð²ÐµÐ½ÑŒ', 'Ð’ÐµÑ€Ð°ÑÐµÐ½ÑŒ', 'ÐšÐ°ÑÑ‚Ñ€Ñ‹Ñ‡Ð½Ñ–Ðº', 'Ð›Ñ–ÑÑ‚Ð°Ð¿Ð°Ð´', 'Ð¡ÑŒÐ½ÐµÐ¶Ð°Ð½ÑŒ'],
        monthNamesShort: ['Ð¡Ñ‚Ñƒ', 'Ð›ÑŽÑ‚', 'Ð¡Ð°Ðº', 'ÐšÑ€Ð°', 'Ð¢Ñ€Ð°', 'Ð§ÑÑ€',
        'Ð›Ñ–Ð¿', 'Ð–Ð½Ñ–', 'Ð’ÐµÑ€', 'ÐšÐ°Ñ', 'Ð›Ñ–Ñ', 'Ð¡ÑŒÐ½'],
        dayNames: ['Ð½ÑÐ´Ð·ÐµÐ»Ñ', 'Ð¿Ð°Ð½ÑÐ´Ð·ÐµÐ»Ð°Ðº', 'Ð°ÑžÑ‚Ð¾Ñ€Ð°Ðº', 'ÑÐµÑ€Ð°Ð´Ð°', 'Ñ‡Ð°Ñ†ÑŒÐ²ÐµÑ€', 'Ð¿ÑÑ‚Ð½Ñ–Ñ†Ð°', 'ÑÑƒÐ±Ð¾Ñ‚Ð°'],
        dayNamesShort: ['Ð½Ð´Ð·', 'Ð¿Ð½Ð´', 'Ð°ÑžÑ‚', 'ÑÑ€Ð´', 'Ñ‡Ñ†Ð²', 'Ð¿Ñ‚Ð½', 'ÑÐ±Ñ‚'],
        dayNamesMin: ['ÐÐ´', 'ÐŸÐ½', 'ÐÑž', 'Ð¡Ñ€', 'Ð§Ñ†', 'ÐŸÑ‚', 'Ð¡Ð±'],
        weekHeader: 'Ð¢Ð´',
        dateFormat: 'dd.mm.yy',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    datepicker.setDefaults(datepicker.regional['be']);

    var i18nDatepickerBe = datepicker.regional['be'];


    /* Bulgarian initialisation for the jQuery UI date picker plugin. */
    /* Written by Stoyan Kyosev (http://svest.org). */


    datepicker.regional['bg'] = {
        closeText: 'Ð·Ð°Ñ‚Ð²Ð¾Ñ€Ð¸',
        prevText: '&#x3C;Ð½Ð°Ð·Ð°Ð´',
        nextText: 'Ð½Ð°Ð¿Ñ€ÐµÐ´&#x3E;',
        nextBigText: '&#x3E;&#x3E;',
        currentText: 'Ð´Ð½ÐµÑ',
        monthNames: ['Ð¯Ð½ÑƒÐ°Ñ€Ð¸', 'Ð¤ÐµÐ²Ñ€ÑƒÐ°Ñ€Ð¸', 'ÐœÐ°Ñ€Ñ‚', 'ÐÐ¿Ñ€Ð¸Ð»', 'ÐœÐ°Ð¹', 'Ð®Ð½Ð¸',
        'Ð®Ð»Ð¸', 'ÐÐ²Ð³ÑƒÑÑ‚', 'Ð¡ÐµÐ¿Ñ‚ÐµÐ¼Ð²Ñ€Ð¸', 'ÐžÐºÑ‚Ð¾Ð¼Ð²Ñ€Ð¸', 'ÐÐ¾ÐµÐ¼Ð²Ñ€Ð¸', 'Ð”ÐµÐºÐµÐ¼Ð²Ñ€Ð¸'],
        monthNamesShort: ['Ð¯Ð½Ñƒ', 'Ð¤ÐµÐ²', 'ÐœÐ°Ñ€', 'ÐÐ¿Ñ€', 'ÐœÐ°Ð¹', 'Ð®Ð½Ð¸',
        'Ð®Ð»Ð¸', 'ÐÐ²Ð³', 'Ð¡ÐµÐ¿', 'ÐžÐºÑ‚', 'ÐÐ¾Ð²', 'Ð”ÐµÐº'],
        dayNames: ['ÐÐµÐ´ÐµÐ»Ñ', 'ÐŸÐ¾Ð½ÐµÐ´ÐµÐ»Ð½Ð¸Ðº', 'Ð’Ñ‚Ð¾Ñ€Ð½Ð¸Ðº', 'Ð¡Ñ€ÑÐ´Ð°', 'Ð§ÐµÑ‚Ð²ÑŠÑ€Ñ‚ÑŠÐº', 'ÐŸÐµÑ‚ÑŠÐº', 'Ð¡ÑŠÐ±Ð¾Ñ‚Ð°'],
        dayNamesShort: ['ÐÐµÐ´', 'ÐŸÐ¾Ð½', 'Ð’Ñ‚Ð¾', 'Ð¡Ñ€Ñ', 'Ð§ÐµÑ‚', 'ÐŸÐµÑ‚', 'Ð¡ÑŠÐ±'],
        dayNamesMin: ['ÐÐµ', 'ÐŸÐ¾', 'Ð’Ñ‚', 'Ð¡Ñ€', 'Ð§Ðµ', 'ÐŸÐµ', 'Ð¡ÑŠ'],
        weekHeader: 'Wk',
        dateFormat: 'dd.mm.yy',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    datepicker.setDefaults(datepicker.regional['bg']);

    var i18nDatepickerBg = datepicker.regional['bg'];


    /* Bosnian i18n for the jQuery UI date picker plugin. */
    /* Written by Kenan Konjo. */


    datepicker.regional['bs'] = {
        closeText: 'Zatvori',
        prevText: '&#x3C;',
        nextText: '&#x3E;',
        currentText: 'Danas',
        monthNames: ['Januar', 'Februar', 'Mart', 'April', 'Maj', 'Juni',
        'Juli', 'August', 'Septembar', 'Oktobar', 'Novembar', 'Decembar'],
        monthNamesShort: ['Jan', 'Feb', 'Mar', 'Apr', 'Maj', 'Jun',
        'Jul', 'Aug', 'Sep', 'Okt', 'Nov', 'Dec'],
        dayNames: ['Nedelja', 'Ponedeljak', 'Utorak', 'Srijeda', 'ÄŒetvrtak', 'Petak', 'Subota'],
        dayNamesShort: ['Ned', 'Pon', 'Uto', 'Sri', 'ÄŒet', 'Pet', 'Sub'],
        dayNamesMin: ['Ne', 'Po', 'Ut', 'Sr', 'ÄŒe', 'Pe', 'Su'],
        weekHeader: 'Wk',
        dateFormat: 'dd.mm.yy',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    datepicker.setDefaults(datepicker.regional['bs']);

    var i18nDatepickerBs = datepicker.regional['bs'];


    /* InicialitzaciÃ³ en catalÃ  per a l'extensiÃ³ 'UI date picker' per jQuery. */
    /* Writers: (joan.leon@gmail.com). */


    datepicker.regional['ca'] = {
        closeText: 'Tanca',
        prevText: 'Anterior',
        nextText: 'SegÃ¼ent',
        currentText: 'Avui',
        monthNames: ['gener', 'febrer', 'marÃ§', 'abril', 'maig', 'juny',
        'juliol', 'agost', 'setembre', 'octubre', 'novembre', 'desembre'],
        monthNamesShort: ['gen', 'feb', 'marÃ§', 'abr', 'maig', 'juny',
        'jul', 'ag', 'set', 'oct', 'nov', 'des'],
        dayNames: ['diumenge', 'dilluns', 'dimarts', 'dimecres', 'dijous', 'divendres', 'dissabte'],
        dayNamesShort: ['dg', 'dl', 'dt', 'dc', 'dj', 'dv', 'ds'],
        dayNamesMin: ['dg', 'dl', 'dt', 'dc', 'dj', 'dv', 'ds'],
        weekHeader: 'Set',
        dateFormat: 'dd/mm/yy',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    datepicker.setDefaults(datepicker.regional['ca']);

    var i18nDatepickerCa = datepicker.regional['ca'];


    /* Czech initialisation for the jQuery UI date picker plugin. */
    /* Written by Tomas Muller (tomas@tomas-muller.net). */


    datepicker.regional['cs'] = {
        closeText: 'ZavÅ™Ã­t',
        prevText: '&#x3C;DÅ™Ã­ve',
        nextText: 'PozdÄ›ji&#x3E;',
        currentText: 'NynÃ­',
        monthNames: ['leden', 'Ãºnor', 'bÅ™ezen', 'duben', 'kvÄ›ten', 'Äerven',
        'Äervenec', 'srpen', 'zÃ¡Å™Ã­', 'Å™Ã­jen', 'listopad', 'prosinec'],
        monthNamesShort: ['led', 'Ãºno', 'bÅ™e', 'dub', 'kvÄ›', 'Äer',
        'Ävc', 'srp', 'zÃ¡Å™', 'Å™Ã­j', 'lis', 'pro'],
        dayNames: ['nedÄ›le', 'pondÄ›lÃ­', 'ÃºterÃ½', 'stÅ™eda', 'Ätvrtek', 'pÃ¡tek', 'sobota'],
        dayNamesShort: ['ne', 'po', 'Ãºt', 'st', 'Ät', 'pÃ¡', 'so'],
        dayNamesMin: ['ne', 'po', 'Ãºt', 'st', 'Ät', 'pÃ¡', 'so'],
        weekHeader: 'TÃ½d',
        dateFormat: 'dd.mm.yy',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    datepicker.setDefaults(datepicker.regional['cs']);

    var i18nDatepickerCs = datepicker.regional['cs'];


    /* Welsh/UK initialisation for the jQuery UI date picker plugin. */
    /* Written by William Griffiths. */


    datepicker.regional['cy-GB'] = {
        closeText: 'Done',
        prevText: 'Prev',
        nextText: 'Next',
        currentText: 'Today',
        monthNames: ['Ionawr', 'Chwefror', 'Mawrth', 'Ebrill', 'Mai', 'Mehefin',
        'Gorffennaf', 'Awst', 'Medi', 'Hydref', 'Tachwedd', 'Rhagfyr'],
        monthNamesShort: ['Ion', 'Chw', 'Maw', 'Ebr', 'Mai', 'Meh',
        'Gor', 'Aws', 'Med', 'Hyd', 'Tac', 'Rha'],
        dayNames: ['Dydd Sul', 'Dydd Llun', 'Dydd Mawrth', 'Dydd Mercher', 'Dydd Iau', 'Dydd Gwener', 'Dydd Sadwrn'],
        dayNamesShort: ['Sul', 'Llu', 'Maw', 'Mer', 'Iau', 'Gwe', 'Sad'],
        dayNamesMin: ['Su', 'Ll', 'Ma', 'Me', 'Ia', 'Gw', 'Sa'],
        weekHeader: 'Wy',
        dateFormat: 'dd/mm/yy',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    datepicker.setDefaults(datepicker.regional['cy-GB']);

    var i18nDatepickerCyGb = datepicker.regional['cy-GB'];


    /* Danish initialisation for the jQuery UI date picker plugin. */
    /* Written by Jan Christensen ( deletestuff@gmail.com). */


    datepicker.regional['da'] = {
        closeText: 'Luk',
        prevText: '&#x3C;Forrige',
        nextText: 'NÃ¦ste&#x3E;',
        currentText: 'Idag',
        monthNames: ['Januar', 'Februar', 'Marts', 'April', 'Maj', 'Juni',
        'Juli', 'August', 'September', 'Oktober', 'November', 'December'],
        monthNamesShort: ['Jan', 'Feb', 'Mar', 'Apr', 'Maj', 'Jun',
        'Jul', 'Aug', 'Sep', 'Okt', 'Nov', 'Dec'],
        dayNames: ['SÃ¸ndag', 'Mandag', 'Tirsdag', 'Onsdag', 'Torsdag', 'Fredag', 'LÃ¸rdag'],
        dayNamesShort: ['SÃ¸n', 'Man', 'Tir', 'Ons', 'Tor', 'Fre', 'LÃ¸r'],
        dayNamesMin: ['SÃ¸', 'Ma', 'Ti', 'On', 'To', 'Fr', 'LÃ¸'],
        weekHeader: 'Uge',
        dateFormat: 'dd-mm-yy',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    datepicker.setDefaults(datepicker.regional['da']);

    var i18nDatepickerDa = datepicker.regional['da'];


    /* German initialisation for the jQuery UI date picker plugin. */
    /* Written by Milian Wolff (mail@milianw.de). */


    datepicker.regional['de'] = {
        closeText: 'SchlieÃŸen',
        prevText: '&#x3C;ZurÃ¼ck',
        nextText: 'Vor&#x3E;',
        currentText: 'Heute',
        monthNames: ['Januar', 'Februar', 'MÃ¤rz', 'April', 'Mai', 'Juni',
        'Juli', 'August', 'September', 'Oktober', 'November', 'Dezember'],
        monthNamesShort: ['Jan', 'Feb', 'MÃ¤r', 'Apr', 'Mai', 'Jun',
        'Jul', 'Aug', 'Sep', 'Okt', 'Nov', 'Dez'],
        dayNames: ['Sonntag', 'Montag', 'Dienstag', 'Mittwoch', 'Donnerstag', 'Freitag', 'Samstag'],
        dayNamesShort: ['So', 'Mo', 'Di', 'Mi', 'Do', 'Fr', 'Sa'],
        dayNamesMin: ['So', 'Mo', 'Di', 'Mi', 'Do', 'Fr', 'Sa'],
        weekHeader: 'KW',
        dateFormat: 'dd.mm.yy',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    datepicker.setDefaults(datepicker.regional['de']);

    var i18nDatepickerDe = datepicker.regional['de'];


    /* Greek (el) initialisation for the jQuery UI date picker plugin. */
    /* Written by Alex Cicovic (http://www.alexcicovic.com) */


    datepicker.regional['el'] = {
        closeText: 'ÎšÎ»ÎµÎ¯ÏƒÎ¹Î¼Î¿',
        prevText: 'Î ÏÎ¿Î·Î³Î¿ÏÎ¼ÎµÎ½Î¿Ï‚',
        nextText: 'Î•Ï€ÏŒÎ¼ÎµÎ½Î¿Ï‚',
        currentText: 'Î£Î®Î¼ÎµÏÎ±',
        monthNames: ['Î™Î±Î½Î¿Ï…Î¬ÏÎ¹Î¿Ï‚', 'Î¦ÎµÎ²ÏÎ¿Ï…Î¬ÏÎ¹Î¿Ï‚', 'ÎœÎ¬ÏÏ„Î¹Î¿Ï‚', 'Î‘Ï€ÏÎ¯Î»Î¹Î¿Ï‚', 'ÎœÎ¬Î¹Î¿Ï‚', 'Î™Î¿ÏÎ½Î¹Î¿Ï‚',
        'Î™Î¿ÏÎ»Î¹Î¿Ï‚', 'Î‘ÏÎ³Î¿Ï…ÏƒÏ„Î¿Ï‚', 'Î£ÎµÏ€Ï„Î­Î¼Î²ÏÎ¹Î¿Ï‚', 'ÎŸÎºÏ„ÏŽÎ²ÏÎ¹Î¿Ï‚', 'ÎÎ¿Î­Î¼Î²ÏÎ¹Î¿Ï‚', 'Î”ÎµÎºÎ­Î¼Î²ÏÎ¹Î¿Ï‚'],
        monthNamesShort: ['Î™Î±Î½', 'Î¦ÎµÎ²', 'ÎœÎ±Ï', 'Î‘Ï€Ï', 'ÎœÎ±Î¹', 'Î™Î¿Ï…Î½',
        'Î™Î¿Ï…Î»', 'Î‘Ï…Î³', 'Î£ÎµÏ€', 'ÎŸÎºÏ„', 'ÎÎ¿Îµ', 'Î”ÎµÎº'],
        dayNames: ['ÎšÏ…ÏÎ¹Î±ÎºÎ®', 'Î”ÎµÏ…Ï„Î­ÏÎ±', 'Î¤ÏÎ¯Ï„Î·', 'Î¤ÎµÏ„Î¬ÏÏ„Î·', 'Î Î­Î¼Ï€Ï„Î·', 'Î Î±ÏÎ±ÏƒÎºÎµÏ…Î®', 'Î£Î¬Î²Î²Î±Ï„Î¿'],
        dayNamesShort: ['ÎšÏ…Ï', 'Î”ÎµÏ…', 'Î¤ÏÎ¹', 'Î¤ÎµÏ„', 'Î ÎµÎ¼', 'Î Î±Ï', 'Î£Î±Î²'],
        dayNamesMin: ['ÎšÏ…', 'Î”Îµ', 'Î¤Ï', 'Î¤Îµ', 'Î Îµ', 'Î Î±', 'Î£Î±'],
        weekHeader: 'Î•Î²Î´',
        dateFormat: 'dd/mm/yy',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    datepicker.setDefaults(datepicker.regional['el']);

    var i18nDatepickerEl = datepicker.regional['el'];


    /* English/Australia initialisation for the jQuery UI date picker plugin. */
    /* Based on the en-GB initialisation. */


    datepicker.regional['en-AU'] = {
        closeText: 'Done',
        prevText: 'Prev',
        nextText: 'Next',
        currentText: 'Today',
        monthNames: ['January', 'February', 'March', 'April', 'May', 'June',
        'July', 'August', 'September', 'October', 'November', 'December'],
        monthNamesShort: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun',
        'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
        dayNames: ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'],
        dayNamesShort: ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'],
        dayNamesMin: ['Su', 'Mo', 'Tu', 'We', 'Th', 'Fr', 'Sa'],
        weekHeader: 'Wk',
        dateFormat: 'dd/mm/yy',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    datepicker.setDefaults(datepicker.regional['en-AU']);

    var i18nDatepickerEnAu = datepicker.regional['en-AU'];


    /* English/UK initialisation for the jQuery UI date picker plugin. */
    /* Written by Stuart. */


    datepicker.regional['en-GB'] = {
        closeText: 'Done',
        prevText: 'Prev',
        nextText: 'Next',
        currentText: 'Today',
        monthNames: ['January', 'February', 'March', 'April', 'May', 'June',
        'July', 'August', 'September', 'October', 'November', 'December'],
        monthNamesShort: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun',
        'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
        dayNames: ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'],
        dayNamesShort: ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'],
        dayNamesMin: ['Su', 'Mo', 'Tu', 'We', 'Th', 'Fr', 'Sa'],
        weekHeader: 'Wk',
        dateFormat: 'dd/mm/yy',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    datepicker.setDefaults(datepicker.regional['en-GB']);

    var i18nDatepickerEnGb = datepicker.regional['en-GB'];


    /* English/New Zealand initialisation for the jQuery UI date picker plugin. */
    /* Based on the en-GB initialisation. */


    datepicker.regional['en-NZ'] = {
        closeText: 'Done',
        prevText: 'Prev',
        nextText: 'Next',
        currentText: 'Today',
        monthNames: ['January', 'February', 'March', 'April', 'May', 'June',
        'July', 'August', 'September', 'October', 'November', 'December'],
        monthNamesShort: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun',
        'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
        dayNames: ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'],
        dayNamesShort: ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'],
        dayNamesMin: ['Su', 'Mo', 'Tu', 'We', 'Th', 'Fr', 'Sa'],
        weekHeader: 'Wk',
        dateFormat: 'dd/mm/yy',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    datepicker.setDefaults(datepicker.regional['en-NZ']);

    var i18nDatepickerEnNz = datepicker.regional['en-NZ'];


    /* Esperanto initialisation for the jQuery UI date picker plugin. */
    /* Written by Olivier M. (olivierweb@ifrance.com). */


    datepicker.regional['eo'] = {
        closeText: 'Fermi',
        prevText: '&#x3C;Anta',
        nextText: 'Sekv&#x3E;',
        currentText: 'Nuna',
        monthNames: ['Januaro', 'Februaro', 'Marto', 'Aprilo', 'Majo', 'Junio',
        'Julio', 'AÅ­gusto', 'Septembro', 'Oktobro', 'Novembro', 'Decembro'],
        monthNamesShort: ['Jan', 'Feb', 'Mar', 'Apr', 'Maj', 'Jun',
        'Jul', 'AÅ­g', 'Sep', 'Okt', 'Nov', 'Dec'],
        dayNames: ['DimanÄ‰o', 'Lundo', 'Mardo', 'Merkredo', 'Ä´aÅ­do', 'Vendredo', 'Sabato'],
        dayNamesShort: ['Dim', 'Lun', 'Mar', 'Mer', 'Ä´aÅ­', 'Ven', 'Sab'],
        dayNamesMin: ['Di', 'Lu', 'Ma', 'Me', 'Ä´a', 'Ve', 'Sa'],
        weekHeader: 'Sb',
        dateFormat: 'dd/mm/yy',
        firstDay: 0,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    datepicker.setDefaults(datepicker.regional['eo']);

    var i18nDatepickerEo = datepicker.regional['eo'];


    /* InicializaciÃ³n en espaÃ±ol para la extensiÃ³n 'UI date picker' para jQuery. */
    /* Traducido por Vester (xvester@gmail.com). */


    datepicker.regional['es'] = {
        closeText: 'Cerrar',
        prevText: '&#x3C;Ant',
        nextText: 'Sig&#x3E;',
        currentText: 'Hoy',
        monthNames: ['enero', 'febrero', 'marzo', 'abril', 'mayo', 'junio',
        'julio', 'agosto', 'septiembre', 'octubre', 'noviembre', 'diciembre'],
        monthNamesShort: ['ene', 'feb', 'mar', 'abr', 'may', 'jun',
        'jul', 'ago', 'sep', 'oct', 'nov', 'dic'],
        dayNames: ['domingo', 'lunes', 'martes', 'miÃ©rcoles', 'jueves', 'viernes', 'sÃ¡bado'],
        dayNamesShort: ['dom', 'lun', 'mar', 'miÃ©', 'jue', 'vie', 'sÃ¡b'],
        dayNamesMin: ['D', 'L', 'M', 'X', 'J', 'V', 'S'],
        weekHeader: 'Sm',
        dateFormat: 'dd/mm/yy',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    datepicker.setDefaults(datepicker.regional['es']);

    var i18nDatepickerEs = datepicker.regional['es'];


    /* Estonian initialisation for the jQuery UI date picker plugin. */
    /* Written by Mart SÃµmermaa (mrts.pydev at gmail com). */


    datepicker.regional['et'] = {
        closeText: 'Sulge',
        prevText: 'Eelnev',
        nextText: 'JÃ¤rgnev',
        currentText: 'TÃ¤na',
        monthNames: ['Jaanuar', 'Veebruar', 'MÃ¤rts', 'Aprill', 'Mai', 'Juuni',
        'Juuli', 'August', 'September', 'Oktoober', 'November', 'Detsember'],
        monthNamesShort: ['Jaan', 'Veebr', 'MÃ¤rts', 'Apr', 'Mai', 'Juuni',
        'Juuli', 'Aug', 'Sept', 'Okt', 'Nov', 'Dets'],
        dayNames: ['PÃ¼hapÃ¤ev', 'EsmaspÃ¤ev', 'TeisipÃ¤ev', 'KolmapÃ¤ev', 'NeljapÃ¤ev', 'Reede', 'LaupÃ¤ev'],
        dayNamesShort: ['PÃ¼hap', 'Esmasp', 'Teisip', 'Kolmap', 'Neljap', 'Reede', 'Laup'],
        dayNamesMin: ['P', 'E', 'T', 'K', 'N', 'R', 'L'],
        weekHeader: 'nÃ¤d',
        dateFormat: 'dd.mm.yy',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    datepicker.setDefaults(datepicker.regional['et']);

    var i18nDatepickerEt = datepicker.regional['et'];


    /* Karrikas-ek itzulia (karrikas@karrikas.com) */


    datepicker.regional['eu'] = {
        closeText: 'Egina',
        prevText: '&#x3C;Aur',
        nextText: 'Hur&#x3E;',
        currentText: 'Gaur',
        monthNames: ['urtarrila', 'otsaila', 'martxoa', 'apirila', 'maiatza', 'ekaina',
            'uztaila', 'abuztua', 'iraila', 'urria', 'azaroa', 'abendua'],
        monthNamesShort: ['urt.', 'ots.', 'mar.', 'api.', 'mai.', 'eka.',
            'uzt.', 'abu.', 'ira.', 'urr.', 'aza.', 'abe.'],
        dayNames: ['igandea', 'astelehena', 'asteartea', 'asteazkena', 'osteguna', 'ostirala', 'larunbata'],
        dayNamesShort: ['ig.', 'al.', 'ar.', 'az.', 'og.', 'ol.', 'lr.'],
        dayNamesMin: ['ig', 'al', 'ar', 'az', 'og', 'ol', 'lr'],
        weekHeader: 'As',
        dateFormat: 'yy-mm-dd',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    datepicker.setDefaults(datepicker.regional['eu']);

    var i18nDatepickerEu = datepicker.regional['eu'];


    /* Persian (Farsi) Translation for the jQuery UI date picker plugin. */
    /* Javad Mowlanezhad -- jmowla@gmail.com */
    /* Jalali calendar should supported soon! (Its implemented but I have to test it) */


    datepicker.regional['fa'] = {
        closeText: 'Ø¨Ø³ØªÙ†',
        prevText: '&#x3C;Ù‚Ø¨Ù„ÛŒ',
        nextText: 'Ø¨Ø¹Ø¯ÛŒ&#x3E;',
        currentText: 'Ø§Ù…Ø±ÙˆØ²',
        monthNames: [
            'Ú˜Ø§Ù†ÙˆÛŒÙ‡',
            'ÙÙˆØ±ÛŒÙ‡',
            'Ù…Ø§Ø±Ø³',
            'Ø¢ÙˆØ±ÛŒÙ„',
            'Ù…Ù‡',
            'Ú˜ÙˆØ¦Ù†',
            'Ú˜ÙˆØ¦ÛŒÙ‡',
            'Ø§ÙˆØª',
            'Ø³Ù¾ØªØ§Ù…Ø¨Ø±',
            'Ø§Ú©ØªØ¨Ø±',
            'Ù†ÙˆØ§Ù…Ø¨Ø±',
            'Ø¯Ø³Ø§Ù…Ø¨Ø±'
        ],
        monthNamesShort: ['1', '2', '3', '4', '5', '6', '7', '8', '9', '10', '11', '12'],
        dayNames: [
            'ÙŠÚ©Ø´Ù†Ø¨Ù‡',
            'Ø¯ÙˆØ´Ù†Ø¨Ù‡',
            'Ø³Ù‡â€ŒØ´Ù†Ø¨Ù‡',
            'Ú†Ù‡Ø§Ø±Ø´Ù†Ø¨Ù‡',
            'Ù¾Ù†Ø¬Ø´Ù†Ø¨Ù‡',
            'Ø¬Ù…Ø¹Ù‡',
            'Ø´Ù†Ø¨Ù‡'
        ],
        dayNamesShort: [
            'ÛŒ',
            'Ø¯',
            'Ø³',
            'Ú†',
            'Ù¾',
            'Ø¬',
            'Ø´'
        ],
        dayNamesMin: [
            'ÛŒ',
            'Ø¯',
            'Ø³',
            'Ú†',
            'Ù¾',
            'Ø¬',
            'Ø´'
        ],
        weekHeader: 'Ù‡Ù',
        dateFormat: 'yy/mm/dd',
        firstDay: 6,
        isRTL: true,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    datepicker.setDefaults(datepicker.regional['fa']);

    var i18nDatepickerFa = datepicker.regional['fa'];


    /* Finnish initialisation for the jQuery UI date picker plugin. */
    /* Written by Harri KilpiÃ¶ (harrikilpio@gmail.com). */


    datepicker.regional['fi'] = {
        closeText: 'Sulje',
        prevText: '&#xAB;Edellinen',
        nextText: 'Seuraava&#xBB;',
        currentText: 'TÃ¤nÃ¤Ã¤n',
        monthNames: ['Tammikuu', 'Helmikuu', 'Maaliskuu', 'Huhtikuu', 'Toukokuu', 'KesÃ¤kuu',
        'HeinÃ¤kuu', 'Elokuu', 'Syyskuu', 'Lokakuu', 'Marraskuu', 'Joulukuu'],
        monthNamesShort: ['Tammi', 'Helmi', 'Maalis', 'Huhti', 'Touko', 'KesÃ¤',
        'HeinÃ¤', 'Elo', 'Syys', 'Loka', 'Marras', 'Joulu'],
        dayNamesShort: ['Su', 'Ma', 'Ti', 'Ke', 'To', 'Pe', 'La'],
        dayNames: ['Sunnuntai', 'Maanantai', 'Tiistai', 'Keskiviikko', 'Torstai', 'Perjantai', 'Lauantai'],
        dayNamesMin: ['Su', 'Ma', 'Ti', 'Ke', 'To', 'Pe', 'La'],
        weekHeader: 'Vk',
        dateFormat: 'd.m.yy',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    datepicker.setDefaults(datepicker.regional['fi']);

    var i18nDatepickerFi = datepicker.regional['fi'];


    /* Faroese initialisation for the jQuery UI date picker plugin */
    /* Written by Sverri Mohr Olsen, sverrimo@gmail.com */


    datepicker.regional['fo'] = {
        closeText: 'Lat aftur',
        prevText: '&#x3C;Fyrra',
        nextText: 'NÃ¦sta&#x3E;',
        currentText: 'Ã dag',
        monthNames: ['Januar', 'Februar', 'Mars', 'AprÃ­l', 'Mei', 'Juni',
        'Juli', 'August', 'September', 'Oktober', 'November', 'Desember'],
        monthNamesShort: ['Jan', 'Feb', 'Mar', 'Apr', 'Mei', 'Jun',
        'Jul', 'Aug', 'Sep', 'Okt', 'Nov', 'Des'],
        dayNames: ['Sunnudagur', 'MÃ¡nadagur', 'TÃ½sdagur', 'Mikudagur', 'HÃ³sdagur', 'FrÃ­ggjadagur', 'Leyardagur'],
        dayNamesShort: ['Sun', 'MÃ¡n', 'TÃ½s', 'Mik', 'HÃ³s', 'FrÃ­', 'Ley'],
        dayNamesMin: ['Su', 'MÃ¡', 'TÃ½', 'Mi', 'HÃ³', 'Fr', 'Le'],
        weekHeader: 'Vk',
        dateFormat: 'dd-mm-yy',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    datepicker.setDefaults(datepicker.regional['fo']);

    var i18nDatepickerFo = datepicker.regional['fo'];


    /* Canadian-French initialisation for the jQuery UI date picker plugin. */


    datepicker.regional['fr-CA'] = {
        closeText: 'Fermer',
        prevText: 'PrÃ©cÃ©dent',
        nextText: 'Suivant',
        currentText: 'Aujourd\'hui',
        monthNames: ['janvier', 'fÃ©vrier', 'mars', 'avril', 'mai', 'juin',
            'juillet', 'aoÃ»t', 'septembre', 'octobre', 'novembre', 'dÃ©cembre'],
        monthNamesShort: ['janv.', 'fÃ©vr.', 'mars', 'avril', 'mai', 'juin',
            'juil.', 'aoÃ»t', 'sept.', 'oct.', 'nov.', 'dÃ©c.'],
        dayNames: ['dimanche', 'lundi', 'mardi', 'mercredi', 'jeudi', 'vendredi', 'samedi'],
        dayNamesShort: ['dim.', 'lun.', 'mar.', 'mer.', 'jeu.', 'ven.', 'sam.'],
        dayNamesMin: ['D', 'L', 'M', 'M', 'J', 'V', 'S'],
        weekHeader: 'Sem.',
        dateFormat: 'yy-mm-dd',
        firstDay: 0,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    datepicker.setDefaults(datepicker.regional['fr-CA']);

    var i18nDatepickerFrCa = datepicker.regional['fr-CA'];


    /* Swiss-French initialisation for the jQuery UI date picker plugin. */
    /* Written Martin Voelkle (martin.voelkle@e-tc.ch). */


    datepicker.regional['fr-CH'] = {
        closeText: 'Fermer',
        prevText: '&#x3C;PrÃ©c',
        nextText: 'Suiv&#x3E;',
        currentText: 'Courant',
        monthNames: ['janvier', 'fÃ©vrier', 'mars', 'avril', 'mai', 'juin',
            'juillet', 'aoÃ»t', 'septembre', 'octobre', 'novembre', 'dÃ©cembre'],
        monthNamesShort: ['janv.', 'fÃ©vr.', 'mars', 'avril', 'mai', 'juin',
            'juil.', 'aoÃ»t', 'sept.', 'oct.', 'nov.', 'dÃ©c.'],
        dayNames: ['dimanche', 'lundi', 'mardi', 'mercredi', 'jeudi', 'vendredi', 'samedi'],
        dayNamesShort: ['dim.', 'lun.', 'mar.', 'mer.', 'jeu.', 'ven.', 'sam.'],
        dayNamesMin: ['D', 'L', 'M', 'M', 'J', 'V', 'S'],
        weekHeader: 'Sm',
        dateFormat: 'dd.mm.yy',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    datepicker.setDefaults(datepicker.regional['fr-CH']);

    var i18nDatepickerFrCh = datepicker.regional['fr-CH'];


    /* French initialisation for the jQuery UI date picker plugin. */
    /* Written by Keith Wood (kbwood{at}iinet.com.au),
                  StÃ©phane Nahmani (sholby@sholby.net),
                  StÃ©phane Raimbault <stephane.raimbault@gmail.com> */


    datepicker.regional['fr'] = {
        closeText: 'Fermer',
        prevText: 'PrÃ©cÃ©dent',
        nextText: 'Suivant',
        currentText: 'Aujourd\'hui',
        monthNames: ['janvier', 'fÃ©vrier', 'mars', 'avril', 'mai', 'juin',
            'juillet', 'aoÃ»t', 'septembre', 'octobre', 'novembre', 'dÃ©cembre'],
        monthNamesShort: ['janv.', 'fÃ©vr.', 'mars', 'avr.', 'mai', 'juin',
            'juil.', 'aoÃ»t', 'sept.', 'oct.', 'nov.', 'dÃ©c.'],
        dayNames: ['dimanche', 'lundi', 'mardi', 'mercredi', 'jeudi', 'vendredi', 'samedi'],
        dayNamesShort: ['dim.', 'lun.', 'mar.', 'mer.', 'jeu.', 'ven.', 'sam.'],
        dayNamesMin: ['D', 'L', 'M', 'M', 'J', 'V', 'S'],
        weekHeader: 'Sem.',
        dateFormat: 'dd/mm/yy',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    datepicker.setDefaults(datepicker.regional['fr']);

    var i18nDatepickerFr = datepicker.regional['fr'];


    /* Galician localization for 'UI date picker' jQuery extension. */
    /* Translated by Jorge Barreiro <yortx.barry@gmail.com>. */


    datepicker.regional['gl'] = {
        closeText: 'Pechar',
        prevText: '&#x3C;Ant',
        nextText: 'Seg&#x3E;',
        currentText: 'Hoxe',
        monthNames: ['Xaneiro', 'Febreiro', 'Marzo', 'Abril', 'Maio', 'XuÃ±o',
        'Xullo', 'Agosto', 'Setembro', 'Outubro', 'Novembro', 'Decembro'],
        monthNamesShort: ['Xan', 'Feb', 'Mar', 'Abr', 'Mai', 'XuÃ±',
        'Xul', 'Ago', 'Set', 'Out', 'Nov', 'Dec'],
        dayNames: ['Domingo', 'Luns', 'Martes', 'MÃ©rcores', 'Xoves', 'Venres', 'SÃ¡bado'],
        dayNamesShort: ['Dom', 'Lun', 'Mar', 'MÃ©r', 'Xov', 'Ven', 'SÃ¡b'],
        dayNamesMin: ['Do', 'Lu', 'Ma', 'MÃ©', 'Xo', 'Ve', 'SÃ¡'],
        weekHeader: 'Sm',
        dateFormat: 'dd/mm/yy',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    datepicker.setDefaults(datepicker.regional['gl']);

    var i18nDatepickerGl = datepicker.regional['gl'];


    /* Hebrew initialisation for the UI Datepicker extension. */
    /* Written by Amir Hardon (ahardon at gmail dot com). */


    datepicker.regional['he'] = {
        closeText: '×¡×’×•×¨',
        prevText: '&#x3C;×”×§×•×“×',
        nextText: '×”×‘×&#x3E;',
        currentText: '×”×™×•×',
        monthNames: ['×™× ×•××¨', '×¤×‘×¨×•××¨', '×ž×¨×¥', '××¤×¨×™×œ', '×ž××™', '×™×•× ×™',
        '×™×•×œ×™', '××•×’×•×¡×˜', '×¡×¤×˜×ž×‘×¨', '××•×§×˜×•×‘×¨', '× ×•×‘×ž×‘×¨', '×“×¦×ž×‘×¨'],
        monthNamesShort: ['×™× ×•', '×¤×‘×¨', '×ž×¨×¥', '××¤×¨', '×ž××™', '×™×•× ×™',
        '×™×•×œ×™', '××•×’', '×¡×¤×˜', '××•×§', '× ×•×‘', '×“×¦×ž'],
        dayNames: ['×¨××©×•×Ÿ', '×©× ×™', '×©×œ×™×©×™', '×¨×‘×™×¢×™', '×—×ž×™×©×™', '×©×™×©×™', '×©×‘×ª'],
        dayNamesShort: ['×\'', '×‘\'', '×’\'', '×“\'', '×”\'', '×•\'', '×©×‘×ª'],
        dayNamesMin: ['×\'', '×‘\'', '×’\'', '×“\'', '×”\'', '×•\'', '×©×‘×ª'],
        weekHeader: 'Wk',
        dateFormat: 'dd/mm/yy',
        firstDay: 0,
        isRTL: true,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    datepicker.setDefaults(datepicker.regional['he']);

    var i18nDatepickerHe = datepicker.regional['he'];


    /* Hindi initialisation for the jQuery UI date picker plugin. */
    /* Written by Michael Dawart. */


    datepicker.regional['hi'] = {
        closeText: 'à¤¬à¤‚à¤¦',
        prevText: 'à¤ªà¤¿à¤›à¤²à¤¾',
        nextText: 'à¤…à¤—à¤²à¤¾',
        currentText: 'à¤†à¤œ',
        monthNames: ['à¤œà¤¨à¤µà¤°à¥€ ', 'à¤«à¤°à¤µà¤°à¥€', 'à¤®à¤¾à¤°à¥à¤š', 'à¤…à¤ªà¥à¤°à¥‡à¤²', 'à¤®à¤ˆ', 'à¤œà¥‚à¤¨',
        'à¤œà¥‚à¤²à¤¾à¤ˆ', 'à¤…à¤—à¤¸à¥à¤¤ ', 'à¤¸à¤¿à¤¤à¤®à¥à¤¬à¤°', 'à¤…à¤•à¥à¤Ÿà¥‚à¤¬à¤°', 'à¤¨à¤µà¤®à¥à¤¬à¤°', 'à¤¦à¤¿à¤¸à¤®à¥à¤¬à¤°'],
        monthNamesShort: ['à¤œà¤¨', 'à¤«à¤°', 'à¤®à¤¾à¤°à¥à¤š', 'à¤…à¤ªà¥à¤°à¥‡à¤²', 'à¤®à¤ˆ', 'à¤œà¥‚à¤¨',
        'à¤œà¥‚à¤²à¤¾à¤ˆ', 'à¤…à¤—', 'à¤¸à¤¿à¤¤', 'à¤…à¤•à¥à¤Ÿ', 'à¤¨à¤µ', 'à¤¦à¤¿'],
        dayNames: ['à¤°à¤µà¤¿à¤µà¤¾à¤°', 'à¤¸à¥‹à¤®à¤µà¤¾à¤°', 'à¤®à¤‚à¤—à¤²à¤µà¤¾à¤°', 'à¤¬à¥à¤§à¤µà¤¾à¤°', 'à¤—à¥à¤°à¥à¤µà¤¾à¤°', 'à¤¶à¥à¤•à¥à¤°à¤µà¤¾à¤°', 'à¤¶à¤¨à¤¿à¤µà¤¾à¤°'],
        dayNamesShort: ['à¤°à¤µà¤¿', 'à¤¸à¥‹à¤®', 'à¤®à¤‚à¤—à¤²', 'à¤¬à¥à¤§', 'à¤—à¥à¤°à¥', 'à¤¶à¥à¤•à¥à¤°', 'à¤¶à¤¨à¤¿'],
        dayNamesMin: ['à¤°à¤µà¤¿', 'à¤¸à¥‹à¤®', 'à¤®à¤‚à¤—à¤²', 'à¤¬à¥à¤§', 'à¤—à¥à¤°à¥', 'à¤¶à¥à¤•à¥à¤°', 'à¤¶à¤¨à¤¿'],
        weekHeader: 'à¤¹à¤«à¥à¤¤à¤¾',
        dateFormat: 'dd/mm/yy',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    datepicker.setDefaults(datepicker.regional['hi']);

    var i18nDatepickerHi = datepicker.regional['hi'];


    /* Croatian i18n for the jQuery UI date picker plugin. */
    /* Written by Vjekoslav Nesek. */


    datepicker.regional['hr'] = {
        closeText: 'Zatvori',
        prevText: '&#x3C;',
        nextText: '&#x3E;',
        currentText: 'Danas',
        monthNames: ['SijeÄanj', 'VeljaÄa', 'OÅ¾ujak', 'Travanj', 'Svibanj', 'Lipanj',
        'Srpanj', 'Kolovoz', 'Rujan', 'Listopad', 'Studeni', 'Prosinac'],
        monthNamesShort: ['Sij', 'Velj', 'OÅ¾u', 'Tra', 'Svi', 'Lip',
        'Srp', 'Kol', 'Ruj', 'Lis', 'Stu', 'Pro'],
        dayNames: ['Nedjelja', 'Ponedjeljak', 'Utorak', 'Srijeda', 'ÄŒetvrtak', 'Petak', 'Subota'],
        dayNamesShort: ['Ned', 'Pon', 'Uto', 'Sri', 'ÄŒet', 'Pet', 'Sub'],
        dayNamesMin: ['Ne', 'Po', 'Ut', 'Sr', 'ÄŒe', 'Pe', 'Su'],
        weekHeader: 'Tje',
        dateFormat: 'dd.mm.yy.',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    datepicker.setDefaults(datepicker.regional['hr']);

    var i18nDatepickerHr = datepicker.regional['hr'];


    /* Hungarian initialisation for the jQuery UI date picker plugin. */


    datepicker.regional['hu'] = {
        closeText: 'bezÃ¡r',
        prevText: 'vissza',
        nextText: 'elÅ‘re',
        currentText: 'ma',
        monthNames: ['JanuÃ¡r', 'FebruÃ¡r', 'MÃ¡rcius', 'Ãprilis', 'MÃ¡jus', 'JÃºnius',
        'JÃºlius', 'Augusztus', 'Szeptember', 'OktÃ³ber', 'November', 'December'],
        monthNamesShort: ['Jan', 'Feb', 'MÃ¡r', 'Ãpr', 'MÃ¡j', 'JÃºn',
        'JÃºl', 'Aug', 'Szep', 'Okt', 'Nov', 'Dec'],
        dayNames: ['VasÃ¡rnap', 'HÃ©tfÅ‘', 'Kedd', 'Szerda', 'CsÃ¼tÃ¶rtÃ¶k', 'PÃ©ntek', 'Szombat'],
        dayNamesShort: ['Vas', 'HÃ©t', 'Ked', 'Sze', 'CsÃ¼', 'PÃ©n', 'Szo'],
        dayNamesMin: ['V', 'H', 'K', 'Sze', 'Cs', 'P', 'Szo'],
        weekHeader: 'HÃ©t',
        dateFormat: 'yy.mm.dd.',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: true,
        yearSuffix: ''
    };
    datepicker.setDefaults(datepicker.regional['hu']);

    var i18nDatepickerHu = datepicker.regional['hu'];


    /* Armenian(UTF-8) initialisation for the jQuery UI date picker plugin. */
    /* Written by Levon Zakaryan (levon.zakaryan@gmail.com)*/


    datepicker.regional['hy'] = {
        closeText: 'Õ“Õ¡Õ¯Õ¥Õ¬',
        prevText: '&#x3C;Õ†Õ¡Õ­.',
        nextText: 'Õ€Õ¡Õ».&#x3E;',
        currentText: 'Ô±ÕµÕ½Ö…Ö€',
        monthNames: ['Õ€Õ¸Ö‚Õ¶Õ¾Õ¡Ö€', 'Õ“Õ¥Õ¿Ö€Õ¾Õ¡Ö€', 'Õ„Õ¡Ö€Õ¿', 'Ô±ÕºÖ€Õ«Õ¬', 'Õ„Õ¡ÕµÕ«Õ½', 'Õ€Õ¸Ö‚Õ¶Õ«Õ½',
        'Õ€Õ¸Ö‚Õ¬Õ«Õ½', 'Õ•Õ£Õ¸Õ½Õ¿Õ¸Õ½', 'ÕÕ¥ÕºÕ¿Õ¥Õ´Õ¢Õ¥Ö€', 'Õ€Õ¸Õ¯Õ¿Õ¥Õ´Õ¢Õ¥Ö€', 'Õ†Õ¸ÕµÕ¥Õ´Õ¢Õ¥Ö€', 'Ô´Õ¥Õ¯Õ¿Õ¥Õ´Õ¢Õ¥Ö€'],
        monthNamesShort: ['Õ€Õ¸Ö‚Õ¶Õ¾', 'Õ“Õ¥Õ¿Ö€', 'Õ„Õ¡Ö€Õ¿', 'Ô±ÕºÖ€', 'Õ„Õ¡ÕµÕ«Õ½', 'Õ€Õ¸Ö‚Õ¶Õ«Õ½',
        'Õ€Õ¸Ö‚Õ¬', 'Õ•Õ£Õ½', 'ÕÕ¥Õº', 'Õ€Õ¸Õ¯', 'Õ†Õ¸Õµ', 'Ô´Õ¥Õ¯'],
        dayNames: ['Õ¯Õ«Ö€Õ¡Õ¯Õ«', 'Õ¥Õ¯Õ¸Ö‚Õ·Õ¡Õ¢Õ©Õ«', 'Õ¥Ö€Õ¥Ö„Õ·Õ¡Õ¢Õ©Õ«', 'Õ¹Õ¸Ö€Õ¥Ö„Õ·Õ¡Õ¢Õ©Õ«', 'Õ°Õ«Õ¶Õ£Õ·Õ¡Õ¢Õ©Õ«', 'Õ¸Ö‚Ö€Õ¢Õ¡Õ©', 'Õ·Õ¡Õ¢Õ¡Õ©'],
        dayNamesShort: ['Õ¯Õ«Ö€', 'Õ¥Ö€Õ¯', 'Õ¥Ö€Ö„', 'Õ¹Ö€Ö„', 'Õ°Õ¶Õ£', 'Õ¸Ö‚Ö€Õ¢', 'Õ·Õ¢Õ©'],
        dayNamesMin: ['Õ¯Õ«Ö€', 'Õ¥Ö€Õ¯', 'Õ¥Ö€Ö„', 'Õ¹Ö€Ö„', 'Õ°Õ¶Õ£', 'Õ¸Ö‚Ö€Õ¢', 'Õ·Õ¢Õ©'],
        weekHeader: 'Õ‡Ô²Õ',
        dateFormat: 'dd.mm.yy',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    datepicker.setDefaults(datepicker.regional['hy']);

    var i18nDatepickerHy = datepicker.regional['hy'];


    /* Indonesian initialisation for the jQuery UI date picker plugin. */
    /* Written by Deden Fathurahman (dedenf@gmail.com). */


    datepicker.regional['id'] = {
        closeText: 'Tutup',
        prevText: '&#x3C;mundur',
        nextText: 'maju&#x3E;',
        currentText: 'hari ini',
        monthNames: ['Januari', 'Februari', 'Maret', 'April', 'Mei', 'Juni',
        'Juli', 'Agustus', 'September', 'Oktober', 'Nopember', 'Desember'],
        monthNamesShort: ['Jan', 'Feb', 'Mar', 'Apr', 'Mei', 'Jun',
        'Jul', 'Agus', 'Sep', 'Okt', 'Nop', 'Des'],
        dayNames: ['Minggu', 'Senin', 'Selasa', 'Rabu', 'Kamis', 'Jumat', 'Sabtu'],
        dayNamesShort: ['Min', 'Sen', 'Sel', 'Rab', 'kam', 'Jum', 'Sab'],
        dayNamesMin: ['Mg', 'Sn', 'Sl', 'Rb', 'Km', 'jm', 'Sb'],
        weekHeader: 'Mg',
        dateFormat: 'dd/mm/yy',
        firstDay: 0,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    datepicker.setDefaults(datepicker.regional['id']);

    var i18nDatepickerId = datepicker.regional['id'];


    /* Icelandic initialisation for the jQuery UI date picker plugin. */
    /* Written by Haukur H. Thorsson (haukur@eskill.is). */


    datepicker.regional['is'] = {
        closeText: 'Loka',
        prevText: '&#x3C; Fyrri',
        nextText: 'NÃ¦sti &#x3E;',
        currentText: 'Ã dag',
        monthNames: ['JanÃºar', 'FebrÃºar', 'Mars', 'AprÃ­l', 'MaÃ­', 'JÃºnÃ­',
        'JÃºlÃ­', 'ÃgÃºst', 'September', 'OktÃ³ber', 'NÃ³vember', 'Desember'],
        monthNamesShort: ['Jan', 'Feb', 'Mar', 'Apr', 'MaÃ­', 'JÃºn',
        'JÃºl', 'ÃgÃº', 'Sep', 'Okt', 'NÃ³v', 'Des'],
        dayNames: ['Sunnudagur', 'MÃ¡nudagur', 'ÃžriÃ°judagur', 'MiÃ°vikudagur', 'Fimmtudagur', 'FÃ¶studagur', 'Laugardagur'],
        dayNamesShort: ['Sun', 'MÃ¡n', 'Ãžri', 'MiÃ°', 'Fim', 'FÃ¶s', 'Lau'],
        dayNamesMin: ['Su', 'MÃ¡', 'Ãžr', 'Mi', 'Fi', 'FÃ¶', 'La'],
        weekHeader: 'Vika',
        dateFormat: 'dd.mm.yy',
        firstDay: 0,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    datepicker.setDefaults(datepicker.regional['is']);

    var i18nDatepickerIs = datepicker.regional['is'];


    /* Italian initialisation for the jQuery UI date picker plugin. */
    /* Written by Antonello Pasella (antonello.pasella@gmail.com). */


    datepicker.regional['it-CH'] = {
        closeText: 'Chiudi',
        prevText: '&#x3C;Prec',
        nextText: 'Succ&#x3E;',
        currentText: 'Oggi',
        monthNames: ['Gennaio', 'Febbraio', 'Marzo', 'Aprile', 'Maggio', 'Giugno',
            'Luglio', 'Agosto', 'Settembre', 'Ottobre', 'Novembre', 'Dicembre'],
        monthNamesShort: ['Gen', 'Feb', 'Mar', 'Apr', 'Mag', 'Giu',
            'Lug', 'Ago', 'Set', 'Ott', 'Nov', 'Dic'],
        dayNames: ['Domenica', 'LunedÃ¬', 'MartedÃ¬', 'MercoledÃ¬', 'GiovedÃ¬', 'VenerdÃ¬', 'Sabato'],
        dayNamesShort: ['Dom', 'Lun', 'Mar', 'Mer', 'Gio', 'Ven', 'Sab'],
        dayNamesMin: ['Do', 'Lu', 'Ma', 'Me', 'Gi', 'Ve', 'Sa'],
        weekHeader: 'Sm',
        dateFormat: 'dd.mm.yy',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    datepicker.setDefaults(datepicker.regional['it-CH']);

    var i18nDatepickerItCh = datepicker.regional['it-CH'];


    /* Italian initialisation for the jQuery UI date picker plugin. */
    /* Written by Antonello Pasella (antonello.pasella@gmail.com). */


    datepicker.regional['it'] = {
        closeText: 'Chiudi',
        prevText: '&#x3C;Prec',
        nextText: 'Succ&#x3E;',
        currentText: 'Oggi',
        monthNames: ['Gennaio', 'Febbraio', 'Marzo', 'Aprile', 'Maggio', 'Giugno',
            'Luglio', 'Agosto', 'Settembre', 'Ottobre', 'Novembre', 'Dicembre'],
        monthNamesShort: ['Gen', 'Feb', 'Mar', 'Apr', 'Mag', 'Giu',
            'Lug', 'Ago', 'Set', 'Ott', 'Nov', 'Dic'],
        dayNames: ['Domenica', 'LunedÃ¬', 'MartedÃ¬', 'MercoledÃ¬', 'GiovedÃ¬', 'VenerdÃ¬', 'Sabato'],
        dayNamesShort: ['Dom', 'Lun', 'Mar', 'Mer', 'Gio', 'Ven', 'Sab'],
        dayNamesMin: ['Do', 'Lu', 'Ma', 'Me', 'Gi', 'Ve', 'Sa'],
        weekHeader: 'Sm',
        dateFormat: 'dd/mm/yy',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    datepicker.setDefaults(datepicker.regional['it']);

    var i18nDatepickerIt = datepicker.regional['it'];


    /* Japanese initialisation for the jQuery UI date picker plugin. */
    /* Written by Kentaro SATO (kentaro@ranvis.com). */


    datepicker.regional['ja'] = {
        closeText: 'é–‰ã˜ã‚‹',
        prevText: '&#x3C;å‰',
        nextText: 'æ¬¡&#x3E;',
        currentText: 'ä»Šæ—¥',
        monthNames: ['1æœˆ', '2æœˆ', '3æœˆ', '4æœˆ', '5æœˆ', '6æœˆ',
        '7æœˆ', '8æœˆ', '9æœˆ', '10æœˆ', '11æœˆ', '12æœˆ'],
        monthNamesShort: ['1æœˆ', '2æœˆ', '3æœˆ', '4æœˆ', '5æœˆ', '6æœˆ',
        '7æœˆ', '8æœˆ', '9æœˆ', '10æœˆ', '11æœˆ', '12æœˆ'],
        dayNames: ['æ—¥æ›œæ—¥', 'æœˆæ›œæ—¥', 'ç«æ›œæ—¥', 'æ°´æ›œæ—¥', 'æœ¨æ›œæ—¥', 'é‡‘æ›œæ—¥', 'åœŸæ›œæ—¥'],
        dayNamesShort: ['æ—¥', 'æœˆ', 'ç«', 'æ°´', 'æœ¨', 'é‡‘', 'åœŸ'],
        dayNamesMin: ['æ—¥', 'æœˆ', 'ç«', 'æ°´', 'æœ¨', 'é‡‘', 'åœŸ'],
        weekHeader: 'é€±',
        dateFormat: 'yy/mm/dd',
        firstDay: 0,
        isRTL: false,
        showMonthAfterYear: true,
        yearSuffix: 'å¹´'
    };
    datepicker.setDefaults(datepicker.regional['ja']);

    var i18nDatepickerJa = datepicker.regional['ja'];


    /* Georgian (UTF-8) initialisation for the jQuery UI date picker plugin. */
    /* Written by Lado Lomidze (lado.lomidze@gmail.com). */


    datepicker.regional['ka'] = {
        closeText: 'áƒ“áƒáƒ®áƒ£áƒ áƒ•áƒ',
        prevText: '&#x3c; áƒ¬áƒ˜áƒœáƒ',
        nextText: 'áƒ¨áƒ”áƒ›áƒ“áƒ”áƒ’áƒ˜ &#x3e;',
        currentText: 'áƒ“áƒ¦áƒ”áƒ¡',
        monthNames: ['áƒ˜áƒáƒœáƒ•áƒáƒ áƒ˜', 'áƒ—áƒ”áƒ‘áƒ”áƒ áƒ•áƒáƒšáƒ˜', 'áƒ›áƒáƒ áƒ¢áƒ˜', 'áƒáƒžáƒ áƒ˜áƒšáƒ˜', 'áƒ›áƒáƒ˜áƒ¡áƒ˜', 'áƒ˜áƒ•áƒœáƒ˜áƒ¡áƒ˜', 'áƒ˜áƒ•áƒšáƒ˜áƒ¡áƒ˜', 'áƒáƒ’áƒ•áƒ˜áƒ¡áƒ¢áƒ', 'áƒ¡áƒ”áƒ¥áƒ¢áƒ”áƒ›áƒ‘áƒ”áƒ áƒ˜', 'áƒáƒ¥áƒ¢áƒáƒ›áƒ‘áƒ”áƒ áƒ˜', 'áƒœáƒáƒ”áƒ›áƒ‘áƒ”áƒ áƒ˜', 'áƒ“áƒ”áƒ™áƒ”áƒ›áƒ‘áƒ”áƒ áƒ˜'],
        monthNamesShort: ['áƒ˜áƒáƒœ', 'áƒ—áƒ”áƒ‘', 'áƒ›áƒáƒ ', 'áƒáƒžáƒ ', 'áƒ›áƒáƒ˜', 'áƒ˜áƒ•áƒœ', 'áƒ˜áƒ•áƒš', 'áƒáƒ’áƒ•', 'áƒ¡áƒ”áƒ¥', 'áƒáƒ¥áƒ¢', 'áƒœáƒáƒ”', 'áƒ“áƒ”áƒ™'],
        dayNames: ['áƒ™áƒ•áƒ˜áƒ áƒ', 'áƒáƒ áƒ¨áƒáƒ‘áƒáƒ—áƒ˜', 'áƒ¡áƒáƒ›áƒ¨áƒáƒ‘áƒáƒ—áƒ˜', 'áƒáƒ—áƒ®áƒ¨áƒáƒ‘áƒáƒ—áƒ˜', 'áƒ®áƒ£áƒ—áƒ¨áƒáƒ‘áƒáƒ—áƒ˜', 'áƒžáƒáƒ áƒáƒ¡áƒ™áƒ”áƒ•áƒ˜', 'áƒ¨áƒáƒ‘áƒáƒ—áƒ˜'],
        dayNamesShort: ['áƒ™áƒ•', 'áƒáƒ áƒ¨', 'áƒ¡áƒáƒ›', 'áƒáƒ—áƒ®', 'áƒ®áƒ£áƒ—', 'áƒžáƒáƒ ', 'áƒ¨áƒáƒ‘'],
        dayNamesMin: ['áƒ™áƒ•', 'áƒáƒ áƒ¨', 'áƒ¡áƒáƒ›', 'áƒáƒ—áƒ®', 'áƒ®áƒ£áƒ—', 'áƒžáƒáƒ ', 'áƒ¨áƒáƒ‘'],
        weekHeader: 'áƒ™áƒ•áƒ˜áƒ áƒ',
        dateFormat: 'dd-mm-yy',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    datepicker.setDefaults(datepicker.regional['ka']);

    var i18nDatepickerKa = datepicker.regional['ka'];


    /* Kazakh (UTF-8) initialisation for the jQuery UI date picker plugin. */
    /* Written by Dmitriy Karasyov (dmitriy.karasyov@gmail.com). */


    datepicker.regional['kk'] = {
        closeText: 'Ð–Ð°Ð±Ñƒ',
        prevText: '&#x3C;ÐÐ»Ð´Ñ‹Ò£Ò“Ñ‹',
        nextText: 'ÐšÐµÐ»ÐµÑÑ–&#x3E;',
        currentText: 'Ð‘Ò¯Ð³Ñ–Ð½',
        monthNames: ['ÒšÐ°Ò£Ñ‚Ð°Ñ€', 'ÐÒ›Ð¿Ð°Ð½', 'ÐÐ°ÑƒÑ€Ñ‹Ð·', 'Ð¡Ó™ÑƒÑ–Ñ€', 'ÐœÐ°Ð¼Ñ‹Ñ€', 'ÐœÐ°ÑƒÑÑ‹Ð¼',
        'Ð¨Ñ–Ð»Ð´Ðµ', 'Ð¢Ð°Ð¼Ñ‹Ð·', 'ÒšÑ‹Ñ€ÐºÒ¯Ð¹ÐµÐº', 'ÒšÐ°Ð·Ð°Ð½', 'ÒšÐ°Ñ€Ð°ÑˆÐ°', 'Ð–ÐµÐ»Ñ‚Ð¾Ò›ÑÐ°Ð½'],
        monthNamesShort: ['ÒšÐ°Ò£', 'ÐÒ›Ð¿', 'ÐÐ°Ñƒ', 'Ð¡Ó™Ñƒ', 'ÐœÐ°Ð¼', 'ÐœÐ°Ñƒ',
        'Ð¨Ñ–Ð»', 'Ð¢Ð°Ð¼', 'ÒšÑ‹Ñ€', 'ÒšÐ°Ð·', 'ÒšÐ°Ñ€', 'Ð–ÐµÐ»'],
        dayNames: ['Ð–ÐµÐºÑÐµÐ½Ð±Ñ–', 'Ð”Ò¯Ð¹ÑÐµÐ½Ð±Ñ–', 'Ð¡ÐµÐ¹ÑÐµÐ½Ð±Ñ–', 'Ð¡Ó™Ñ€ÑÐµÐ½Ð±Ñ–', 'Ð‘ÐµÐ¹ÑÐµÐ½Ð±Ñ–', 'Ð–Ò±Ð¼Ð°', 'Ð¡ÐµÐ½Ð±Ñ–'],
        dayNamesShort: ['Ð¶ÐºÑ', 'Ð´ÑÐ½', 'ÑÑÐ½', 'ÑÑ€Ñ', 'Ð±ÑÐ½', 'Ð¶Ð¼Ð°', 'ÑÐ½Ð±'],
        dayNamesMin: ['Ð–Ðº', 'Ð”Ñ', 'Ð¡Ñ', 'Ð¡Ñ€', 'Ð‘Ñ', 'Ð–Ð¼', 'Ð¡Ð½'],
        weekHeader: 'ÐÐµ',
        dateFormat: 'dd.mm.yy',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    datepicker.setDefaults(datepicker.regional['kk']);

    var i18nDatepickerKk = datepicker.regional['kk'];


    /* Khmer initialisation for the jQuery calendar extension. */
    /* Written by Chandara Om (chandara.teacher@gmail.com). */


    datepicker.regional['km'] = {
        closeText: 'áž’áŸ’ážœáž¾â€‹ážšáž½áž…',
        prevText: 'áž˜áž»áž“',
        nextText: 'áž”áž“áŸ’áž‘áž¶áž”áŸ‹',
        currentText: 'ážáŸ’áž„áŸƒâ€‹áž“áŸáŸ‡',
        monthNames: ['áž˜áž€ážšáž¶', 'áž€áž»áž˜áŸ’áž—áŸˆ', 'áž˜áž¸áž“áž¶', 'áž˜áŸážŸáž¶', 'áž§ážŸáž—áž¶', 'áž˜áž·ážáž»áž“áž¶',
        'áž€áž€áŸ’áž€ážŠáž¶', 'ážŸáž¸áž áž¶', 'áž€áž‰áŸ’áž‰áž¶', 'ážáž»áž›áž¶', 'ážœáž·áž…áŸ’áž†áž·áž€áž¶', 'áž’áŸ’áž“áž¼'],
        monthNamesShort: ['áž˜áž€ážšáž¶', 'áž€áž»áž˜áŸ’áž—áŸˆ', 'áž˜áž¸áž“áž¶', 'áž˜áŸážŸáž¶', 'áž§ážŸáž—áž¶', 'áž˜áž·ážáž»áž“áž¶',
        'áž€áž€áŸ’áž€ážŠáž¶', 'ážŸáž¸áž áž¶', 'áž€áž‰áŸ’áž‰áž¶', 'ážáž»áž›áž¶', 'ážœáž·áž…áŸ’áž†áž·áž€áž¶', 'áž’áŸ’áž“áž¼'],
        dayNames: ['áž¢áž¶áž‘áž·ážáŸ’áž™', 'áž…áž“áŸ’áž‘', 'áž¢áž„áŸ’áž‚áž¶ážš', 'áž–áž»áž’', 'áž–áŸ’ážšáž ážŸáŸ’áž”ážáž·áŸ', 'ážŸáž»áž€áŸ’ážš', 'ážŸáŸ…ážšáŸ'],
        dayNamesShort: ['áž¢áž¶', 'áž…', 'áž¢', 'áž–áž»', 'áž–áŸ’ážšáž ', 'ážŸáž»', 'ážŸáŸ…'],
        dayNamesMin: ['áž¢áž¶', 'áž…', 'áž¢', 'áž–áž»', 'áž–áŸ’ážšáž ', 'ážŸáž»', 'ážŸáŸ…'],
        weekHeader: 'ážŸáž”áŸ’ážŠáž¶áž áŸ',
        dateFormat: 'dd-mm-yy',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    datepicker.setDefaults(datepicker.regional['km']);

    var i18nDatepickerKm = datepicker.regional['km'];


    /* Korean initialisation for the jQuery calendar extension. */
    /* Written by DaeKwon Kang (ncrash.dk@gmail.com), Edited by Genie. */


    datepicker.regional['ko'] = {
        closeText: 'ë‹«ê¸°',
        prevText: 'ì´ì „ë‹¬',
        nextText: 'ë‹¤ìŒë‹¬',
        currentText: 'ì˜¤ëŠ˜',
        monthNames: ['1ì›”', '2ì›”', '3ì›”', '4ì›”', '5ì›”', '6ì›”',
        '7ì›”', '8ì›”', '9ì›”', '10ì›”', '11ì›”', '12ì›”'],
        monthNamesShort: ['1ì›”', '2ì›”', '3ì›”', '4ì›”', '5ì›”', '6ì›”',
        '7ì›”', '8ì›”', '9ì›”', '10ì›”', '11ì›”', '12ì›”'],
        dayNames: ['ì¼ìš”ì¼', 'ì›”ìš”ì¼', 'í™”ìš”ì¼', 'ìˆ˜ìš”ì¼', 'ëª©ìš”ì¼', 'ê¸ˆìš”ì¼', 'í† ìš”ì¼'],
        dayNamesShort: ['ì¼', 'ì›”', 'í™”', 'ìˆ˜', 'ëª©', 'ê¸ˆ', 'í† '],
        dayNamesMin: ['ì¼', 'ì›”', 'í™”', 'ìˆ˜', 'ëª©', 'ê¸ˆ', 'í† '],
        weekHeader: 'Wk',
        dateFormat: 'yy-mm-dd',
        firstDay: 0,
        isRTL: false,
        showMonthAfterYear: true,
        yearSuffix: 'ë…„'
    };
    datepicker.setDefaults(datepicker.regional['ko']);

    var i18nDatepickerKo = datepicker.regional['ko'];


    /* Kyrgyz (UTF-8) initialisation for the jQuery UI date picker plugin. */
    /* Written by Sergey Kartashov (ebishkek@yandex.ru). */


    datepicker.regional['ky'] = {
        closeText: 'Ð–Ð°Ð±ÑƒÑƒ',
        prevText: '&#x3c;ÐœÑƒÑ€',
        nextText: 'ÐšÐ¸Ð¹&#x3e;',
        currentText: 'Ð‘Ò¯Ð³Ò¯Ð½',
        monthNames: ['Ð¯Ð½Ð²Ð°Ñ€ÑŒ', 'Ð¤ÐµÐ²Ñ€Ð°Ð»ÑŒ', 'ÐœÐ°Ñ€Ñ‚', 'ÐÐ¿Ñ€ÐµÐ»ÑŒ', 'ÐœÐ°Ð¹', 'Ð˜ÑŽÐ½ÑŒ',
        'Ð˜ÑŽÐ»ÑŒ', 'ÐÐ²Ð³ÑƒÑÑ‚', 'Ð¡ÐµÐ½Ñ‚ÑÐ±Ñ€ÑŒ', 'ÐžÐºÑ‚ÑÐ±Ñ€ÑŒ', 'ÐÐ¾ÑÐ±Ñ€ÑŒ', 'Ð”ÐµÐºÐ°Ð±Ñ€ÑŒ'],
        monthNamesShort: ['Ð¯Ð½Ð²', 'Ð¤ÐµÐ²', 'ÐœÐ°Ñ€', 'ÐÐ¿Ñ€', 'ÐœÐ°Ð¹', 'Ð˜ÑŽÐ½',
        'Ð˜ÑŽÐ»', 'ÐÐ²Ð³', 'Ð¡ÐµÐ½', 'ÐžÐºÑ‚', 'ÐÐ¾Ñ', 'Ð”ÐµÐº'],
        dayNames: ['Ð¶ÐµÐºÑˆÐµÐ¼Ð±Ð¸', 'Ð´Ò¯Ð¹ÑˆÓ©Ð¼Ð±Ò¯', 'ÑˆÐµÐ¹ÑˆÐµÐ¼Ð±Ð¸', 'ÑˆÐ°Ñ€ÑˆÐµÐ¼Ð±Ð¸', 'Ð±ÐµÐ¹ÑˆÐµÐ¼Ð±Ð¸', 'Ð¶ÑƒÐ¼Ð°', 'Ð¸ÑˆÐµÐ¼Ð±Ð¸'],
        dayNamesShort: ['Ð¶ÐµÐº', 'Ð´Ò¯Ð¹', 'ÑˆÐµÐ¹', 'ÑˆÐ°Ñ€', 'Ð±ÐµÐ¹', 'Ð¶ÑƒÐ¼', 'Ð¸ÑˆÐµ'],
        dayNamesMin: ['Ð–Ðº', 'Ð”Ñˆ', 'Ð¨Ñˆ', 'Ð¨Ñ€', 'Ð‘Ñˆ', 'Ð–Ð¼', 'Ð˜Ñˆ'],
        weekHeader: 'Ð–ÑƒÐ¼',
        dateFormat: 'dd.mm.yy',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    datepicker.setDefaults(datepicker.regional['ky']);

    var i18nDatepickerKy = datepicker.regional['ky'];


    /* Luxembourgish initialisation for the jQuery UI date picker plugin. */
    /* Written by Michel Weimerskirch <michel@weimerskirch.net> */


    datepicker.regional['lb'] = {
        closeText: 'FÃ¤erdeg',
        prevText: 'ZrÃ©ck',
        nextText: 'Weider',
        currentText: 'Haut',
        monthNames: ['Januar', 'Februar', 'MÃ¤erz', 'AbrÃ«ll', 'Mee', 'Juni',
        'Juli', 'August', 'September', 'Oktober', 'November', 'Dezember'],
        monthNamesShort: ['Jan', 'Feb', 'MÃ¤e', 'Abr', 'Mee', 'Jun',
        'Jul', 'Aug', 'Sep', 'Okt', 'Nov', 'Dez'],
        dayNames: ['Sonndeg', 'MÃ©indeg', 'DÃ«nschdeg', 'MÃ«ttwoch', 'Donneschdeg', 'Freideg', 'Samschdeg'],
        dayNamesShort: ['Son', 'MÃ©i', 'DÃ«n', 'MÃ«t', 'Don', 'Fre', 'Sam'],
        dayNamesMin: ['So', 'MÃ©', 'DÃ«', 'MÃ«', 'Do', 'Fr', 'Sa'],
        weekHeader: 'W',
        dateFormat: 'dd.mm.yy',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    datepicker.setDefaults(datepicker.regional['lb']);

    var i18nDatepickerLb = datepicker.regional['lb'];


    /* Lithuanian (UTF-8) initialisation for the jQuery UI date picker plugin. */
    /* @author Arturas Paleicikas <arturas@avalon.lt> */


    datepicker.regional['lt'] = {
        closeText: 'UÅ¾daryti',
        prevText: '&#x3C;Atgal',
        nextText: 'Pirmyn&#x3E;',
        currentText: 'Å iandien',
        monthNames: ['Sausis', 'Vasaris', 'Kovas', 'Balandis', 'GeguÅ¾Ä—', 'BirÅ¾elis',
        'Liepa', 'RugpjÅ«tis', 'RugsÄ—jis', 'Spalis', 'Lapkritis', 'Gruodis'],
        monthNamesShort: ['Sau', 'Vas', 'Kov', 'Bal', 'Geg', 'Bir',
        'Lie', 'Rugp', 'Rugs', 'Spa', 'Lap', 'Gru'],
        dayNames: ['sekmadienis', 'pirmadienis', 'antradienis', 'treÄiadienis', 'ketvirtadienis', 'penktadienis', 'Å¡eÅ¡tadienis'],
        dayNamesShort: ['sek', 'pir', 'ant', 'tre', 'ket', 'pen', 'Å¡eÅ¡'],
        dayNamesMin: ['Se', 'Pr', 'An', 'Tr', 'Ke', 'Pe', 'Å e'],
        weekHeader: 'SAV',
        dateFormat: 'yy-mm-dd',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: true,
        yearSuffix: ''
    };
    datepicker.setDefaults(datepicker.regional['lt']);

    var i18nDatepickerLt = datepicker.regional['lt'];


    /* Latvian (UTF-8) initialisation for the jQuery UI date picker plugin. */
    /* @author Arturas Paleicikas <arturas.paleicikas@metasite.net> */


    datepicker.regional['lv'] = {
        closeText: 'AizvÄ“rt',
        prevText: 'Iepr.',
        nextText: 'NÄk.',
        currentText: 'Å odien',
        monthNames: ['JanvÄris', 'FebruÄris', 'Marts', 'AprÄ«lis', 'Maijs', 'JÅ«nijs',
        'JÅ«lijs', 'Augusts', 'Septembris', 'Oktobris', 'Novembris', 'Decembris'],
        monthNamesShort: ['Jan', 'Feb', 'Mar', 'Apr', 'Mai', 'JÅ«n',
        'JÅ«l', 'Aug', 'Sep', 'Okt', 'Nov', 'Dec'],
        dayNames: ['svÄ“tdiena', 'pirmdiena', 'otrdiena', 'treÅ¡diena', 'ceturtdiena', 'piektdiena', 'sestdiena'],
        dayNamesShort: ['svt', 'prm', 'otr', 'tre', 'ctr', 'pkt', 'sst'],
        dayNamesMin: ['Sv', 'Pr', 'Ot', 'Tr', 'Ct', 'Pk', 'Ss'],
        weekHeader: 'Ned.',
        dateFormat: 'dd.mm.yy',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    datepicker.setDefaults(datepicker.regional['lv']);

    var i18nDatepickerLv = datepicker.regional['lv'];


    /* Macedonian i18n for the jQuery UI date picker plugin. */
    /* Written by Stojce Slavkovski. */


    datepicker.regional['mk'] = {
        closeText: 'Ð—Ð°Ñ‚Ð²Ð¾Ñ€Ð¸',
        prevText: '&#x3C;',
        nextText: '&#x3E;',
        currentText: 'Ð”ÐµÐ½ÐµÑ',
        monthNames: ['ÐˆÐ°Ð½ÑƒÐ°Ñ€Ð¸', 'Ð¤ÐµÐ²Ñ€ÑƒÐ°Ñ€Ð¸', 'ÐœÐ°Ñ€Ñ‚', 'ÐÐ¿Ñ€Ð¸Ð»', 'ÐœÐ°Ñ˜', 'ÐˆÑƒÐ½Ð¸',
        'ÐˆÑƒÐ»Ð¸', 'ÐÐ²Ð³ÑƒÑÑ‚', 'Ð¡ÐµÐ¿Ñ‚ÐµÐ¼Ð²Ñ€Ð¸', 'ÐžÐºÑ‚Ð¾Ð¼Ð²Ñ€Ð¸', 'ÐÐ¾ÐµÐ¼Ð²Ñ€Ð¸', 'Ð”ÐµÐºÐµÐ¼Ð²Ñ€Ð¸'],
        monthNamesShort: ['ÐˆÐ°Ð½', 'Ð¤ÐµÐ²', 'ÐœÐ°Ñ€', 'ÐÐ¿Ñ€', 'ÐœÐ°Ñ˜', 'ÐˆÑƒÐ½',
        'ÐˆÑƒÐ»', 'ÐÐ²Ð³', 'Ð¡ÐµÐ¿', 'ÐžÐºÑ‚', 'ÐÐ¾Ðµ', 'Ð”ÐµÐº'],
        dayNames: ['ÐÐµÐ´ÐµÐ»Ð°', 'ÐŸÐ¾Ð½ÐµÐ´ÐµÐ»Ð½Ð¸Ðº', 'Ð’Ñ‚Ð¾Ñ€Ð½Ð¸Ðº', 'Ð¡Ñ€ÐµÐ´Ð°', 'Ð§ÐµÑ‚Ð²Ñ€Ñ‚Ð¾Ðº', 'ÐŸÐµÑ‚Ð¾Ðº', 'Ð¡Ð°Ð±Ð¾Ñ‚Ð°'],
        dayNamesShort: ['ÐÐµÐ´', 'ÐŸÐ¾Ð½', 'Ð’Ñ‚Ð¾', 'Ð¡Ñ€Ðµ', 'Ð§ÐµÑ‚', 'ÐŸÐµÑ‚', 'Ð¡Ð°Ð±'],
        dayNamesMin: ['ÐÐµ', 'ÐŸÐ¾', 'Ð’Ñ‚', 'Ð¡Ñ€', 'Ð§Ðµ', 'ÐŸÐµ', 'Ð¡Ð°'],
        weekHeader: 'Ð¡ÐµÐ´',
        dateFormat: 'dd.mm.yy',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    datepicker.setDefaults(datepicker.regional['mk']);

    var i18nDatepickerMk = datepicker.regional['mk'];


    /* Malayalam (UTF-8) initialisation for the jQuery UI date picker plugin. */
    /* Written by Saji Nediyanchath (saji89@gmail.com). */


    datepicker.regional['ml'] = {
        closeText: 'à´¶à´°à´¿',
        prevText: 'à´®àµà´¨àµà´¨à´¤àµà´¤àµ†',
        nextText: 'à´…à´Ÿàµà´¤àµà´¤à´¤àµ ',
        currentText: 'à´‡à´¨àµà´¨àµ',
        monthNames: ['à´œà´¨àµà´µà´°à´¿', 'à´«àµ†à´¬àµà´°àµà´µà´°à´¿', 'à´®à´¾à´°àµâ€à´šàµà´šàµ', 'à´à´ªàµà´°à´¿à´²àµâ€', 'à´®àµ‡à´¯àµ', 'à´œàµ‚à´£àµâ€',
        'à´œàµ‚à´²àµˆ', 'à´†à´—à´¸àµà´±àµà´±àµ', 'à´¸àµ†à´ªàµà´±àµà´±à´‚à´¬à´°àµâ€', 'à´’à´•àµà´Ÿàµ‹à´¬à´°àµâ€', 'à´¨à´µà´‚à´¬à´°àµâ€', 'à´¡à´¿à´¸à´‚à´¬à´°àµâ€'],
        monthNamesShort: ['à´œà´¨àµ', 'à´«àµ†à´¬àµ', 'à´®à´¾à´°àµâ€', 'à´à´ªàµà´°à´¿', 'à´®àµ‡à´¯àµ', 'à´œàµ‚à´£àµâ€',
        'à´œàµ‚à´²à´¾', 'à´†à´—', 'à´¸àµ†à´ªàµ', 'à´’à´•àµà´Ÿàµ‹', 'à´¨à´µà´‚', 'à´¡à´¿à´¸'],
        dayNames: ['à´žà´¾à´¯à´°àµâ€', 'à´¤à´¿à´™àµà´•à´³àµâ€', 'à´šàµŠà´µàµà´µ', 'à´¬àµà´§à´¨àµâ€', 'à´µàµà´¯à´¾à´´à´‚', 'à´µàµ†à´³àµà´³à´¿', 'à´¶à´¨à´¿'],
        dayNamesShort: ['à´žà´¾à´¯', 'à´¤à´¿à´™àµà´•', 'à´šàµŠà´µàµà´µ', 'à´¬àµà´§', 'à´µàµà´¯à´¾à´´à´‚', 'à´µàµ†à´³àµà´³à´¿', 'à´¶à´¨à´¿'],
        dayNamesMin: ['à´žà´¾', 'à´¤à´¿', 'à´šàµŠ', 'à´¬àµ', 'à´µàµà´¯à´¾', 'à´µàµ†', 'à´¶'],
        weekHeader: 'à´†',
        dateFormat: 'dd/mm/yy',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    datepicker.setDefaults(datepicker.regional['ml']);

    var i18nDatepickerMl = datepicker.regional['ml'];


    /* Malaysian initialisation for the jQuery UI date picker plugin. */
    /* Written by Mohd Nawawi Mohamad Jamili (nawawi@ronggeng.net). */


    datepicker.regional['ms'] = {
        closeText: 'Tutup',
        prevText: '&#x3C;Sebelum',
        nextText: 'Selepas&#x3E;',
        currentText: 'hari ini',
        monthNames: ['Januari', 'Februari', 'Mac', 'April', 'Mei', 'Jun',
        'Julai', 'Ogos', 'September', 'Oktober', 'November', 'Disember'],
        monthNamesShort: ['Jan', 'Feb', 'Mac', 'Apr', 'Mei', 'Jun',
        'Jul', 'Ogo', 'Sep', 'Okt', 'Nov', 'Dis'],
        dayNames: ['Ahad', 'Isnin', 'Selasa', 'Rabu', 'Khamis', 'Jumaat', 'Sabtu'],
        dayNamesShort: ['Aha', 'Isn', 'Sel', 'Rab', 'kha', 'Jum', 'Sab'],
        dayNamesMin: ['Ah', 'Is', 'Se', 'Ra', 'Kh', 'Ju', 'Sa'],
        weekHeader: 'Mg',
        dateFormat: 'dd/mm/yy',
        firstDay: 0,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    datepicker.setDefaults(datepicker.regional['ms']);

    var i18nDatepickerMs = datepicker.regional['ms'];


    /* Norwegian BokmÃ¥l initialisation for the jQuery UI date picker plugin. */
    /* Written by BjÃ¸rn Johansen (post@bjornjohansen.no). */


    datepicker.regional['nb'] = {
        closeText: 'Lukk',
        prevText: '&#xAB;Forrige',
        nextText: 'Neste&#xBB;',
        currentText: 'I dag',
        monthNames: ['januar', 'februar', 'mars', 'april', 'mai', 'juni', 'juli', 'august', 'september', 'oktober', 'november', 'desember'],
        monthNamesShort: ['jan', 'feb', 'mar', 'apr', 'mai', 'jun', 'jul', 'aug', 'sep', 'okt', 'nov', 'des'],
        dayNamesShort: ['sÃ¸n', 'man', 'tir', 'ons', 'tor', 'fre', 'lÃ¸r'],
        dayNames: ['sÃ¸ndag', 'mandag', 'tirsdag', 'onsdag', 'torsdag', 'fredag', 'lÃ¸rdag'],
        dayNamesMin: ['sÃ¸', 'ma', 'ti', 'on', 'to', 'fr', 'lÃ¸'],
        weekHeader: 'Uke',
        dateFormat: 'dd.mm.yy',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    datepicker.setDefaults(datepicker.regional['nb']);

    var i18nDatepickerNb = datepicker.regional['nb'];


    /* Dutch (Belgium) initialisation for the jQuery UI date picker plugin. */
    /* David De Sloovere @DavidDeSloovere */


    datepicker.regional['nl-BE'] = {
        closeText: 'Sluiten',
        prevText: 'â†',
        nextText: 'â†’',
        currentText: 'Vandaag',
        monthNames: ['januari', 'februari', 'maart', 'april', 'mei', 'juni',
        'juli', 'augustus', 'september', 'oktober', 'november', 'december'],
        monthNamesShort: ['jan', 'feb', 'mrt', 'apr', 'mei', 'jun',
        'jul', 'aug', 'sep', 'okt', 'nov', 'dec'],
        dayNames: ['zondag', 'maandag', 'dinsdag', 'woensdag', 'donderdag', 'vrijdag', 'zaterdag'],
        dayNamesShort: ['zon', 'maa', 'din', 'woe', 'don', 'vri', 'zat'],
        dayNamesMin: ['zo', 'ma', 'di', 'wo', 'do', 'vr', 'za'],
        weekHeader: 'Wk',
        dateFormat: 'dd/mm/yy',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    datepicker.setDefaults(datepicker.regional['nl-BE']);

    var i18nDatepickerNlBe = datepicker.regional['nl-BE'];


    /* Dutch (UTF-8) initialisation for the jQuery UI date picker plugin. */
    /* Written by Mathias Bynens <http://mathiasbynens.be/> */


    datepicker.regional.nl = {
        closeText: 'Sluiten',
        prevText: 'â†',
        nextText: 'â†’',
        currentText: 'Vandaag',
        monthNames: ['januari', 'februari', 'maart', 'april', 'mei', 'juni',
        'juli', 'augustus', 'september', 'oktober', 'november', 'december'],
        monthNamesShort: ['jan', 'feb', 'mrt', 'apr', 'mei', 'jun',
        'jul', 'aug', 'sep', 'okt', 'nov', 'dec'],
        dayNames: ['zondag', 'maandag', 'dinsdag', 'woensdag', 'donderdag', 'vrijdag', 'zaterdag'],
        dayNamesShort: ['zon', 'maa', 'din', 'woe', 'don', 'vri', 'zat'],
        dayNamesMin: ['zo', 'ma', 'di', 'wo', 'do', 'vr', 'za'],
        weekHeader: 'Wk',
        dateFormat: 'dd-mm-yy',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    datepicker.setDefaults(datepicker.regional.nl);

    var i18nDatepickerNl = datepicker.regional.nl;


    /* Norwegian Nynorsk initialisation for the jQuery UI date picker plugin. */
    /* Written by BjÃ¸rn Johansen (post@bjornjohansen.no). */


    datepicker.regional['nn'] = {
        closeText: 'Lukk',
        prevText: '&#xAB;FÃ¸rre',
        nextText: 'Neste&#xBB;',
        currentText: 'I dag',
        monthNames: ['januar', 'februar', 'mars', 'april', 'mai', 'juni', 'juli', 'august', 'september', 'oktober', 'november', 'desember'],
        monthNamesShort: ['jan', 'feb', 'mar', 'apr', 'mai', 'jun', 'jul', 'aug', 'sep', 'okt', 'nov', 'des'],
        dayNamesShort: ['sun', 'mÃ¥n', 'tys', 'ons', 'tor', 'fre', 'lau'],
        dayNames: ['sundag', 'mÃ¥ndag', 'tysdag', 'onsdag', 'torsdag', 'fredag', 'laurdag'],
        dayNamesMin: ['su', 'mÃ¥', 'ty', 'on', 'to', 'fr', 'la'],
        weekHeader: 'Veke',
        dateFormat: 'dd.mm.yy',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    datepicker.setDefaults(datepicker.regional['nn']);

    var i18nDatepickerNn = datepicker.regional['nn'];


    /* Norwegian initialisation for the jQuery UI date picker plugin. */
    /* Written by Naimdjon Takhirov (naimdjon@gmail.com). */



    datepicker.regional['no'] = {
        closeText: 'Lukk',
        prevText: '&#xAB;Forrige',
        nextText: 'Neste&#xBB;',
        currentText: 'I dag',
        monthNames: ['januar', 'februar', 'mars', 'april', 'mai', 'juni', 'juli', 'august', 'september', 'oktober', 'november', 'desember'],
        monthNamesShort: ['jan', 'feb', 'mar', 'apr', 'mai', 'jun', 'jul', 'aug', 'sep', 'okt', 'nov', 'des'],
        dayNamesShort: ['sÃ¸n', 'man', 'tir', 'ons', 'tor', 'fre', 'lÃ¸r'],
        dayNames: ['sÃ¸ndag', 'mandag', 'tirsdag', 'onsdag', 'torsdag', 'fredag', 'lÃ¸rdag'],
        dayNamesMin: ['sÃ¸', 'ma', 'ti', 'on', 'to', 'fr', 'lÃ¸'],
        weekHeader: 'Uke',
        dateFormat: 'dd.mm.yy',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    datepicker.setDefaults(datepicker.regional['no']);

    var i18nDatepickerNo = datepicker.regional['no'];


    /* Polish initialisation for the jQuery UI date picker plugin. */
    /* Written by Jacek Wysocki (jacek.wysocki@gmail.com). */


    datepicker.regional['pl'] = {
        closeText: 'Zamknij',
        prevText: '&#x3C;Poprzedni',
        nextText: 'NastÄ™pny&#x3E;',
        currentText: 'DziÅ›',
        monthNames: ['StyczeÅ„', 'Luty', 'Marzec', 'KwiecieÅ„', 'Maj', 'Czerwiec',
        'Lipiec', 'SierpieÅ„', 'WrzesieÅ„', 'PaÅºdziernik', 'Listopad', 'GrudzieÅ„'],
        monthNamesShort: ['Sty', 'Lu', 'Mar', 'Kw', 'Maj', 'Cze',
        'Lip', 'Sie', 'Wrz', 'Pa', 'Lis', 'Gru'],
        dayNames: ['Niedziela', 'PoniedziaÅ‚ek', 'Wtorek', 'Åšroda', 'Czwartek', 'PiÄ…tek', 'Sobota'],
        dayNamesShort: ['Nie', 'Pn', 'Wt', 'Åšr', 'Czw', 'Pt', 'So'],
        dayNamesMin: ['N', 'Pn', 'Wt', 'Åšr', 'Cz', 'Pt', 'So'],
        weekHeader: 'Tydz',
        dateFormat: 'dd.mm.yy',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    datepicker.setDefaults(datepicker.regional['pl']);

    var i18nDatepickerPl = datepicker.regional['pl'];


    /* Brazilian initialisation for the jQuery UI date picker plugin. */
    /* Written by Leonildo Costa Silva (leocsilva@gmail.com). */


    datepicker.regional['pt-BR'] = {
        closeText: 'Fechar',
        prevText: '&#x3C;Anterior',
        nextText: 'PrÃ³ximo&#x3E;',
        currentText: 'Hoje',
        monthNames: ['Janeiro', 'Fevereiro', 'MarÃ§o', 'Abril', 'Maio', 'Junho',
        'Julho', 'Agosto', 'Setembro', 'Outubro', 'Novembro', 'Dezembro'],
        monthNamesShort: ['Jan', 'Fev', 'Mar', 'Abr', 'Mai', 'Jun',
        'Jul', 'Ago', 'Set', 'Out', 'Nov', 'Dez'],
        dayNames: ['Domingo', 'Segunda-feira', 'TerÃ§a-feira', 'Quarta-feira', 'Quinta-feira', 'Sexta-feira', 'SÃ¡bado'],
        dayNamesShort: ['Dom', 'Seg', 'Ter', 'Qua', 'Qui', 'Sex', 'SÃ¡b'],
        dayNamesMin: ['Dom', 'Seg', 'Ter', 'Qua', 'Qui', 'Sex', 'SÃ¡b'],
        weekHeader: 'Sm',
        dateFormat: 'dd/mm/yy',
        firstDay: 0,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    datepicker.setDefaults(datepicker.regional['pt-BR']);

    var i18nDatepickerPtBr = datepicker.regional['pt-BR'];


    /* Portuguese initialisation for the jQuery UI date picker plugin. */


    datepicker.regional['pt'] = {
        closeText: 'Fechar',
        prevText: 'Anterior',
        nextText: 'Seguinte',
        currentText: 'Hoje',
        monthNames: ['Janeiro', 'Fevereiro', 'MarÃ§o', 'Abril', 'Maio', 'Junho',
        'Julho', 'Agosto', 'Setembro', 'Outubro', 'Novembro', 'Dezembro'],
        monthNamesShort: ['Jan', 'Fev', 'Mar', 'Abr', 'Mai', 'Jun',
        'Jul', 'Ago', 'Set', 'Out', 'Nov', 'Dez'],
        dayNames: ['Domingo', 'Segunda-feira', 'TerÃ§a-feira', 'Quarta-feira', 'Quinta-feira', 'Sexta-feira', 'SÃ¡bado'],
        dayNamesShort: ['Dom', 'Seg', 'Ter', 'Qua', 'Qui', 'Sex', 'SÃ¡b'],
        dayNamesMin: ['Dom', 'Seg', 'Ter', 'Qua', 'Qui', 'Sex', 'SÃ¡b'],
        weekHeader: 'Sem',
        dateFormat: 'dd/mm/yy',
        firstDay: 0,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    datepicker.setDefaults(datepicker.regional['pt']);

    var i18nDatepickerPt = datepicker.regional['pt'];


    /* Romansh initialisation for the jQuery UI date picker plugin. */
    /* Written by Yvonne Gienal (yvonne.gienal@educa.ch). */


    datepicker.regional['rm'] = {
        closeText: 'Serrar',
        prevText: '&#x3C;Suandant',
        nextText: 'Precedent&#x3E;',
        currentText: 'Actual',
        monthNames: ['Schaner', 'Favrer', 'Mars', 'Avrigl', 'Matg', 'Zercladur', 'Fanadur', 'Avust', 'Settember', 'October', 'November', 'December'],
        monthNamesShort: ['Scha', 'Fev', 'Mar', 'Avr', 'Matg', 'Zer', 'Fan', 'Avu', 'Sett', 'Oct', 'Nov', 'Dec'],
        dayNames: ['Dumengia', 'Glindesdi', 'Mardi', 'Mesemna', 'Gievgia', 'Venderdi', 'Sonda'],
        dayNamesShort: ['Dum', 'Gli', 'Mar', 'Mes', 'Gie', 'Ven', 'Som'],
        dayNamesMin: ['Du', 'Gl', 'Ma', 'Me', 'Gi', 'Ve', 'So'],
        weekHeader: 'emna',
        dateFormat: 'dd/mm/yy',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    datepicker.setDefaults(datepicker.regional['rm']);

    var i18nDatepickerRm = datepicker.regional['rm'];


    /* Romanian initialisation for the jQuery UI date picker plugin.
     *
     * Written by Edmond L. (ll_edmond@walla.com)
     * and Ionut G. Stan (ionut.g.stan@gmail.com)
     */


    datepicker.regional['ro'] = {
        closeText: 'ÃŽnchide',
        prevText: '&#xAB; Luna precedentÄƒ',
        nextText: 'Luna urmÄƒtoare &#xBB;',
        currentText: 'Azi',
        monthNames: ['Ianuarie', 'Februarie', 'Martie', 'Aprilie', 'Mai', 'Iunie',
        'Iulie', 'August', 'Septembrie', 'Octombrie', 'Noiembrie', 'Decembrie'],
        monthNamesShort: ['Ian', 'Feb', 'Mar', 'Apr', 'Mai', 'Iun',
        'Iul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
        dayNames: ['DuminicÄƒ', 'Luni', 'MarÅ£i', 'Miercuri', 'Joi', 'Vineri', 'SÃ¢mbÄƒtÄƒ'],
        dayNamesShort: ['Dum', 'Lun', 'Mar', 'Mie', 'Joi', 'Vin', 'SÃ¢m'],
        dayNamesMin: ['Du', 'Lu', 'Ma', 'Mi', 'Jo', 'Vi', 'SÃ¢'],
        weekHeader: 'SÄƒpt',
        dateFormat: 'dd.mm.yy',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    datepicker.setDefaults(datepicker.regional['ro']);

    var i18nDatepickerRo = datepicker.regional['ro'];


    /* Russian (UTF-8) initialisation for the jQuery UI date picker plugin. */
    /* Written by Andrew Stromnov (stromnov@gmail.com). */


    datepicker.regional['ru'] = {
        closeText: 'Ð—Ð°ÐºÑ€Ñ‹Ñ‚ÑŒ',
        prevText: '&#x3C;ÐŸÑ€ÐµÐ´',
        nextText: 'Ð¡Ð»ÐµÐ´&#x3E;',
        currentText: 'Ð¡ÐµÐ³Ð¾Ð´Ð½Ñ',
        monthNames: ['Ð¯Ð½Ð²Ð°Ñ€ÑŒ', 'Ð¤ÐµÐ²Ñ€Ð°Ð»ÑŒ', 'ÐœÐ°Ñ€Ñ‚', 'ÐÐ¿Ñ€ÐµÐ»ÑŒ', 'ÐœÐ°Ð¹', 'Ð˜ÑŽÐ½ÑŒ',
        'Ð˜ÑŽÐ»ÑŒ', 'ÐÐ²Ð³ÑƒÑÑ‚', 'Ð¡ÐµÐ½Ñ‚ÑÐ±Ñ€ÑŒ', 'ÐžÐºÑ‚ÑÐ±Ñ€ÑŒ', 'ÐÐ¾ÑÐ±Ñ€ÑŒ', 'Ð”ÐµÐºÐ°Ð±Ñ€ÑŒ'],
        monthNamesShort: ['Ð¯Ð½Ð²', 'Ð¤ÐµÐ²', 'ÐœÐ°Ñ€', 'ÐÐ¿Ñ€', 'ÐœÐ°Ð¹', 'Ð˜ÑŽÐ½',
        'Ð˜ÑŽÐ»', 'ÐÐ²Ð³', 'Ð¡ÐµÐ½', 'ÐžÐºÑ‚', 'ÐÐ¾Ñ', 'Ð”ÐµÐº'],
        dayNames: ['Ð²Ð¾ÑÐºÑ€ÐµÑÐµÐ½ÑŒÐµ', 'Ð¿Ð¾Ð½ÐµÐ´ÐµÐ»ÑŒÐ½Ð¸Ðº', 'Ð²Ñ‚Ð¾Ñ€Ð½Ð¸Ðº', 'ÑÑ€ÐµÐ´Ð°', 'Ñ‡ÐµÑ‚Ð²ÐµÑ€Ð³', 'Ð¿ÑÑ‚Ð½Ð¸Ñ†Ð°', 'ÑÑƒÐ±Ð±Ð¾Ñ‚Ð°'],
        dayNamesShort: ['Ð²ÑÐº', 'Ð¿Ð½Ð´', 'Ð²Ñ‚Ñ€', 'ÑÑ€Ð´', 'Ñ‡Ñ‚Ð²', 'Ð¿Ñ‚Ð½', 'ÑÐ±Ñ‚'],
        dayNamesMin: ['Ð’Ñ', 'ÐŸÐ½', 'Ð’Ñ‚', 'Ð¡Ñ€', 'Ð§Ñ‚', 'ÐŸÑ‚', 'Ð¡Ð±'],
        weekHeader: 'ÐÐµÐ´',
        dateFormat: 'dd.mm.yy',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    datepicker.setDefaults(datepicker.regional['ru']);

    var i18nDatepickerRu = datepicker.regional['ru'];


    /* Slovak initialisation for the jQuery UI date picker plugin. */
    /* Written by Vojtech Rinik (vojto@hmm.sk). */


    datepicker.regional['sk'] = {
        closeText: 'ZavrieÅ¥',
        prevText: '&#x3C;PredchÃ¡dzajÃºci',
        nextText: 'NasledujÃºci&#x3E;',
        currentText: 'Dnes',
        monthNames: ['januÃ¡r', 'februÃ¡r', 'marec', 'aprÃ­l', 'mÃ¡j', 'jÃºn',
        'jÃºl', 'august', 'september', 'oktÃ³ber', 'november', 'december'],
        monthNamesShort: ['Jan', 'Feb', 'Mar', 'Apr', 'MÃ¡j', 'JÃºn',
        'JÃºl', 'Aug', 'Sep', 'Okt', 'Nov', 'Dec'],
        dayNames: ['nedeÄ¾a', 'pondelok', 'utorok', 'streda', 'Å¡tvrtok', 'piatok', 'sobota'],
        dayNamesShort: ['Ned', 'Pon', 'Uto', 'Str', 'Å tv', 'Pia', 'Sob'],
        dayNamesMin: ['Ne', 'Po', 'Ut', 'St', 'Å t', 'Pia', 'So'],
        weekHeader: 'Ty',
        dateFormat: 'dd.mm.yy',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    datepicker.setDefaults(datepicker.regional['sk']);

    var i18nDatepickerSk = datepicker.regional['sk'];


    /* Slovenian initialisation for the jQuery UI date picker plugin. */
    /* Written by Jaka Jancar (jaka@kubje.org). */
    /* c = Ä, s = Å¡ z = Å¾ C = ÄŒ S = Å  Z = Å½ */


    datepicker.regional['sl'] = {
        closeText: 'Zapri',
        prevText: '&#x3C;PrejÅ¡nji',
        nextText: 'Naslednji&#x3E;',
        currentText: 'Trenutni',
        monthNames: ['Januar', 'Februar', 'Marec', 'April', 'Maj', 'Junij',
        'Julij', 'Avgust', 'September', 'Oktober', 'November', 'December'],
        monthNamesShort: ['Jan', 'Feb', 'Mar', 'Apr', 'Maj', 'Jun',
        'Jul', 'Avg', 'Sep', 'Okt', 'Nov', 'Dec'],
        dayNames: ['Nedelja', 'Ponedeljek', 'Torek', 'Sreda', 'ÄŒetrtek', 'Petek', 'Sobota'],
        dayNamesShort: ['Ned', 'Pon', 'Tor', 'Sre', 'ÄŒet', 'Pet', 'Sob'],
        dayNamesMin: ['Ne', 'Po', 'To', 'Sr', 'ÄŒe', 'Pe', 'So'],
        weekHeader: 'Teden',
        dateFormat: 'dd.mm.yy',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    datepicker.setDefaults(datepicker.regional['sl']);

    var i18nDatepickerSl = datepicker.regional['sl'];


    /* Albanian initialisation for the jQuery UI date picker plugin. */
    /* Written by Flakron Bytyqi (flakron@gmail.com). */


    datepicker.regional['sq'] = {
        closeText: 'mbylle',
        prevText: '&#x3C;mbrapa',
        nextText: 'PÃ«rpara&#x3E;',
        currentText: 'sot',
        monthNames: ['Janar', 'Shkurt', 'Mars', 'Prill', 'Maj', 'Qershor',
        'Korrik', 'Gusht', 'Shtator', 'Tetor', 'NÃ«ntor', 'Dhjetor'],
        monthNamesShort: ['Jan', 'Shk', 'Mar', 'Pri', 'Maj', 'Qer',
        'Kor', 'Gus', 'Sht', 'Tet', 'NÃ«n', 'Dhj'],
        dayNames: ['E Diel', 'E HÃ«nÃ«', 'E MartÃ«', 'E MÃ«rkurÃ«', 'E Enjte', 'E Premte', 'E Shtune'],
        dayNamesShort: ['Di', 'HÃ«', 'Ma', 'MÃ«', 'En', 'Pr', 'Sh'],
        dayNamesMin: ['Di', 'HÃ«', 'Ma', 'MÃ«', 'En', 'Pr', 'Sh'],
        weekHeader: 'Ja',
        dateFormat: 'dd.mm.yy',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    datepicker.setDefaults(datepicker.regional['sq']);

    var i18nDatepickerSq = datepicker.regional['sq'];


    /* Serbian i18n for the jQuery UI date picker plugin. */
    /* Written by Dejan DimiÄ‡. */


    datepicker.regional['sr-SR'] = {
        closeText: 'Zatvori',
        prevText: '&#x3C;',
        nextText: '&#x3E;',
        currentText: 'Danas',
        monthNames: ['Januar', 'Februar', 'Mart', 'April', 'Maj', 'Jun',
        'Jul', 'Avgust', 'Septembar', 'Oktobar', 'Novembar', 'Decembar'],
        monthNamesShort: ['Jan', 'Feb', 'Mar', 'Apr', 'Maj', 'Jun',
        'Jul', 'Avg', 'Sep', 'Okt', 'Nov', 'Dec'],
        dayNames: ['Nedelja', 'Ponedeljak', 'Utorak', 'Sreda', 'ÄŒetvrtak', 'Petak', 'Subota'],
        dayNamesShort: ['Ned', 'Pon', 'Uto', 'Sre', 'ÄŒet', 'Pet', 'Sub'],
        dayNamesMin: ['Ne', 'Po', 'Ut', 'Sr', 'ÄŒe', 'Pe', 'Su'],
        weekHeader: 'Sed',
        dateFormat: 'dd.mm.yy',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    datepicker.setDefaults(datepicker.regional['sr-SR']);

    var i18nDatepickerSrSr = datepicker.regional['sr-SR'];


    /* Serbian i18n for the jQuery UI date picker plugin. */
    /* Written by Dejan DimiÄ‡. */


    datepicker.regional['sr'] = {
        closeText: 'Ð—Ð°Ñ‚Ð²Ð¾Ñ€Ð¸',
        prevText: '&#x3C;',
        nextText: '&#x3E;',
        currentText: 'Ð”Ð°Ð½Ð°Ñ',
        monthNames: ['ÐˆÐ°Ð½ÑƒÐ°Ñ€', 'Ð¤ÐµÐ±Ñ€ÑƒÐ°Ñ€', 'ÐœÐ°Ñ€Ñ‚', 'ÐÐ¿Ñ€Ð¸Ð»', 'ÐœÐ°Ñ˜', 'ÐˆÑƒÐ½',
        'ÐˆÑƒÐ»', 'ÐÐ²Ð³ÑƒÑÑ‚', 'Ð¡ÐµÐ¿Ñ‚ÐµÐ¼Ð±Ð°Ñ€', 'ÐžÐºÑ‚Ð¾Ð±Ð°Ñ€', 'ÐÐ¾Ð²ÐµÐ¼Ð±Ð°Ñ€', 'Ð”ÐµÑ†ÐµÐ¼Ð±Ð°Ñ€'],
        monthNamesShort: ['ÐˆÐ°Ð½', 'Ð¤ÐµÐ±', 'ÐœÐ°Ñ€', 'ÐÐ¿Ñ€', 'ÐœÐ°Ñ˜', 'ÐˆÑƒÐ½',
        'ÐˆÑƒÐ»', 'ÐÐ²Ð³', 'Ð¡ÐµÐ¿', 'ÐžÐºÑ‚', 'ÐÐ¾Ð²', 'Ð”ÐµÑ†'],
        dayNames: ['ÐÐµÐ´ÐµÑ™Ð°', 'ÐŸÐ¾Ð½ÐµÐ´ÐµÑ™Ð°Ðº', 'Ð£Ñ‚Ð¾Ñ€Ð°Ðº', 'Ð¡Ñ€ÐµÐ´Ð°', 'Ð§ÐµÑ‚Ð²Ñ€Ñ‚Ð°Ðº', 'ÐŸÐµÑ‚Ð°Ðº', 'Ð¡ÑƒÐ±Ð¾Ñ‚Ð°'],
        dayNamesShort: ['ÐÐµÐ´', 'ÐŸÐ¾Ð½', 'Ð£Ñ‚Ð¾', 'Ð¡Ñ€Ðµ', 'Ð§ÐµÑ‚', 'ÐŸÐµÑ‚', 'Ð¡ÑƒÐ±'],
        dayNamesMin: ['ÐÐµ', 'ÐŸÐ¾', 'Ð£Ñ‚', 'Ð¡Ñ€', 'Ð§Ðµ', 'ÐŸÐµ', 'Ð¡Ñƒ'],
        weekHeader: 'Ð¡ÐµÐ´',
        dateFormat: 'dd.mm.yy',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    datepicker.setDefaults(datepicker.regional['sr']);

    var i18nDatepickerSr = datepicker.regional['sr'];


    /* Swedish initialisation for the jQuery UI date picker plugin. */
    /* Written by Anders Ekdahl ( anders@nomadiz.se). */


    datepicker.regional['sv'] = {
        closeText: 'StÃ¤ng',
        prevText: '&#xAB;FÃ¶rra',
        nextText: 'NÃ¤sta&#xBB;',
        currentText: 'Idag',
        monthNames: ['Januari', 'Februari', 'Mars', 'April', 'Maj', 'Juni',
        'Juli', 'Augusti', 'September', 'Oktober', 'November', 'December'],
        monthNamesShort: ['Jan', 'Feb', 'Mar', 'Apr', 'Maj', 'Jun',
        'Jul', 'Aug', 'Sep', 'Okt', 'Nov', 'Dec'],
        dayNamesShort: ['SÃ¶n', 'MÃ¥n', 'Tis', 'Ons', 'Tor', 'Fre', 'LÃ¶r'],
        dayNames: ['SÃ¶ndag', 'MÃ¥ndag', 'Tisdag', 'Onsdag', 'Torsdag', 'Fredag', 'LÃ¶rdag'],
        dayNamesMin: ['SÃ¶', 'MÃ¥', 'Ti', 'On', 'To', 'Fr', 'LÃ¶'],
        weekHeader: 'Ve',
        dateFormat: 'yy-mm-dd',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    datepicker.setDefaults(datepicker.regional['sv']);

    var i18nDatepickerSv = datepicker.regional['sv'];


    /* Tamil (UTF-8) initialisation for the jQuery UI date picker plugin. */
    /* Written by S A Sureshkumar (saskumar@live.com). */


    datepicker.regional['ta'] = {
        closeText: 'à®®à¯‚à®Ÿà¯',
        prevText: 'à®®à¯à®©à¯à®©à¯ˆà®¯à®¤à¯',
        nextText: 'à®…à®Ÿà¯à®¤à¯à®¤à®¤à¯',
        currentText: 'à®‡à®©à¯à®±à¯',
        monthNames: ['à®¤à¯ˆ', 'à®®à®¾à®šà®¿', 'à®ªà®™à¯à®•à¯à®©à®¿', 'à®šà®¿à®¤à¯à®¤à®¿à®°à¯ˆ', 'à®µà¯ˆà®•à®¾à®šà®¿', 'à®†à®©à®¿',
        'à®†à®Ÿà®¿', 'à®†à®µà®£à®¿', 'à®ªà¯à®°à®Ÿà¯à®Ÿà®¾à®šà®¿', 'à®à®ªà¯à®ªà®šà®¿', 'à®•à®¾à®°à¯à®¤à¯à®¤à®¿à®•à¯ˆ', 'à®®à®¾à®°à¯à®•à®´à®¿'],
        monthNamesShort: ['à®¤à¯ˆ', 'à®®à®¾à®šà®¿', 'à®ªà®™à¯', 'à®šà®¿à®¤à¯', 'à®µà¯ˆà®•à®¾', 'à®†à®©à®¿',
        'à®†à®Ÿà®¿', 'à®†à®µ', 'à®ªà¯à®°', 'à®à®ªà¯', 'à®•à®¾à®°à¯', 'à®®à®¾à®°à¯'],
        dayNames: ['à®žà®¾à®¯à®¿à®±à¯à®±à¯à®•à¯à®•à®¿à®´à®®à¯ˆ', 'à®¤à®¿à®™à¯à®•à®Ÿà¯à®•à®¿à®´à®®à¯ˆ', 'à®šà¯†à®µà¯à®µà®¾à®¯à¯à®•à¯à®•à®¿à®´à®®à¯ˆ', 'à®ªà¯à®¤à®©à¯à®•à®¿à®´à®®à¯ˆ', 'à®µà®¿à®¯à®¾à®´à®•à¯à®•à®¿à®´à®®à¯ˆ', 'à®µà¯†à®³à¯à®³à®¿à®•à¯à®•à®¿à®´à®®à¯ˆ', 'à®šà®©à®¿à®•à¯à®•à®¿à®´à®®à¯ˆ'],
        dayNamesShort: ['à®žà®¾à®¯à®¿à®±à¯', 'à®¤à®¿à®™à¯à®•à®³à¯', 'à®šà¯†à®µà¯à®µà®¾à®¯à¯', 'à®ªà¯à®¤à®©à¯', 'à®µà®¿à®¯à®¾à®´à®©à¯', 'à®µà¯†à®³à¯à®³à®¿', 'à®šà®©à®¿'],
        dayNamesMin: ['à®žà®¾', 'à®¤à®¿', 'à®šà¯†', 'à®ªà¯', 'à®µà®¿', 'à®µà¯†', 'à®š'],
        weekHeader: 'ÐÐµ',
        dateFormat: 'dd/mm/yy',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    datepicker.setDefaults(datepicker.regional['ta']);

    var i18nDatepickerTa = datepicker.regional['ta'];


    /* Thai initialisation for the jQuery UI date picker plugin. */
    /* Written by pipo (pipo@sixhead.com). */


    datepicker.regional['th'] = {
        closeText: 'à¸›à¸´à¸”',
        prevText: '&#xAB;&#xA0;à¸¢à¹‰à¸­à¸™',
        nextText: 'à¸–à¸±à¸”à¹„à¸›&#xA0;&#xBB;',
        currentText: 'à¸§à¸±à¸™à¸™à¸µà¹‰',
        monthNames: ['à¸¡à¸à¸£à¸²à¸„à¸¡', 'à¸à¸¸à¸¡à¸ à¸²à¸žà¸±à¸™à¸˜à¹Œ', 'à¸¡à¸µà¸™à¸²à¸„à¸¡', 'à¹€à¸¡à¸©à¸²à¸¢à¸™', 'à¸žà¸¤à¸©à¸ à¸²à¸„à¸¡', 'à¸¡à¸´à¸–à¸¸à¸™à¸²à¸¢à¸™',
        'à¸à¸£à¸à¸Žà¸²à¸„à¸¡', 'à¸ªà¸´à¸‡à¸«à¸²à¸„à¸¡', 'à¸à¸±à¸™à¸¢à¸²à¸¢à¸™', 'à¸•à¸¸à¸¥à¸²à¸„à¸¡', 'à¸žà¸¤à¸¨à¸ˆà¸´à¸à¸²à¸¢à¸™', 'à¸˜à¸±à¸™à¸§à¸²à¸„à¸¡'],
        monthNamesShort: ['à¸¡.à¸„.', 'à¸.à¸ž.', 'à¸¡à¸µ.à¸„.', 'à¹€à¸¡.à¸¢.', 'à¸ž.à¸„.', 'à¸¡à¸´.à¸¢.',
        'à¸.à¸„.', 'à¸ª.à¸„.', 'à¸.à¸¢.', 'à¸•.à¸„.', 'à¸ž.à¸¢.', 'à¸˜.à¸„.'],
        dayNames: ['à¸­à¸²à¸—à¸´à¸•à¸¢à¹Œ', 'à¸ˆà¸±à¸™à¸—à¸£à¹Œ', 'à¸­à¸±à¸‡à¸„à¸²à¸£', 'à¸žà¸¸à¸˜', 'à¸žà¸¤à¸«à¸±à¸ªà¸šà¸”à¸µ', 'à¸¨à¸¸à¸à¸£à¹Œ', 'à¹€à¸ªà¸²à¸£à¹Œ'],
        dayNamesShort: ['à¸­à¸².', 'à¸ˆ.', 'à¸­.', 'à¸ž.', 'à¸žà¸¤.', 'à¸¨.', 'à¸ª.'],
        dayNamesMin: ['à¸­à¸².', 'à¸ˆ.', 'à¸­.', 'à¸ž.', 'à¸žà¸¤.', 'à¸¨.', 'à¸ª.'],
        weekHeader: 'Wk',
        dateFormat: 'dd/mm/yy',
        firstDay: 0,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    datepicker.setDefaults(datepicker.regional['th']);

    var i18nDatepickerTh = datepicker.regional['th'];


    /* Tajiki (UTF-8) initialisation for the jQuery UI date picker plugin. */
    /* Written by Abdurahmon Saidov (saidovab@gmail.com). */


    datepicker.regional['tj'] = {
        closeText: 'Ð˜Ð´Ð¾Ð¼Ð°',
        prevText: '&#x3c;ÒšÐ°Ñ„Ð¾',
        nextText: 'ÐŸÐµÑˆ&#x3e;',
        currentText: 'Ð˜Ð¼Ñ€Ó¯Ð·',
        monthNames: ['Ð¯Ð½Ð²Ð°Ñ€', 'Ð¤ÐµÐ²Ñ€Ð°Ð»', 'ÐœÐ°Ñ€Ñ‚', 'ÐÐ¿Ñ€ÐµÐ»', 'ÐœÐ°Ð¹', 'Ð˜ÑŽÐ½',
        'Ð˜ÑŽÐ»', 'ÐÐ²Ð³ÑƒÑÑ‚', 'Ð¡ÐµÐ½Ñ‚ÑÐ±Ñ€', 'ÐžÐºÑ‚ÑÐ±Ñ€', 'ÐÐ¾ÑÐ±Ñ€', 'Ð”ÐµÐºÐ°Ð±Ñ€'],
        monthNamesShort: ['Ð¯Ð½Ð²', 'Ð¤ÐµÐ²', 'ÐœÐ°Ñ€', 'ÐÐ¿Ñ€', 'ÐœÐ°Ð¹', 'Ð˜ÑŽÐ½',
        'Ð˜ÑŽÐ»', 'ÐÐ²Ð³', 'Ð¡ÐµÐ½', 'ÐžÐºÑ‚', 'ÐÐ¾Ñ', 'Ð”ÐµÐº'],
        dayNames: ['ÑÐºÑˆÐ°Ð½Ð±Ðµ', 'Ð´ÑƒÑˆÐ°Ð½Ð±Ðµ', 'ÑÐµÑˆÐ°Ð½Ð±Ðµ', 'Ñ‡Ð¾Ñ€ÑˆÐ°Ð½Ð±Ðµ', 'Ð¿Ð°Ð½Ò·ÑˆÐ°Ð½Ð±Ðµ', 'Ò·ÑƒÐ¼ÑŠÐ°', 'ÑˆÐ°Ð½Ð±Ðµ'],
        dayNamesShort: ['ÑÐºÑˆ', 'Ð´ÑƒÑˆ', 'ÑÐµÑˆ', 'Ñ‡Ð¾Ñ€', 'Ð¿Ð°Ð½', 'Ò·ÑƒÐ¼', 'ÑˆÐ°Ð½'],
        dayNamesMin: ['Ð¯Ðº', 'Ð”Ñˆ', 'Ð¡Ñˆ', 'Ð§Ñˆ', 'ÐŸÑˆ', 'Ò¶Ð¼', 'Ð¨Ð½'],
        weekHeader: 'Ð¥Ñ„',
        dateFormat: 'dd.mm.yy',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    datepicker.setDefaults(datepicker.regional['tj']);

    var i18nDatepickerTj = datepicker.regional['tj'];


    /* Turkish initialisation for the jQuery UI date picker plugin. */
    /* Written by Izzet Emre Erkan (kara@karalamalar.net). */


    datepicker.regional['tr'] = {
        closeText: 'kapat',
        prevText: '&#x3C;geri',
        nextText: 'ileri&#x3e',
        currentText: 'bugÃ¼n',
        monthNames: ['Ocak', 'Åžubat', 'Mart', 'Nisan', 'MayÄ±s', 'Haziran',
        'Temmuz', 'AÄŸustos', 'EylÃ¼l', 'Ekim', 'KasÄ±m', 'AralÄ±k'],
        monthNamesShort: ['Oca', 'Åžub', 'Mar', 'Nis', 'May', 'Haz',
        'Tem', 'AÄŸu', 'Eyl', 'Eki', 'Kas', 'Ara'],
        dayNames: ['Pazar', 'Pazartesi', 'SalÄ±', 'Ã‡arÅŸamba', 'PerÅŸembe', 'Cuma', 'Cumartesi'],
        dayNamesShort: ['Pz', 'Pt', 'Sa', 'Ã‡a', 'Pe', 'Cu', 'Ct'],
        dayNamesMin: ['Pz', 'Pt', 'Sa', 'Ã‡a', 'Pe', 'Cu', 'Ct'],
        weekHeader: 'Hf',
        dateFormat: 'dd.mm.yy',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    datepicker.setDefaults(datepicker.regional['tr']);

    var i18nDatepickerTr = datepicker.regional['tr'];


    /* Ukrainian (UTF-8) initialisation for the jQuery UI date picker plugin. */
    /* Written by Maxim Drogobitskiy (maxdao@gmail.com). */
    /* Corrected by Igor Milla (igor.fsp.milla@gmail.com). */


    datepicker.regional['uk'] = {
        closeText: 'Ð—Ð°ÐºÑ€Ð¸Ñ‚Ð¸',
        prevText: '&#x3C;',
        nextText: '&#x3E;',
        currentText: 'Ð¡ÑŒÐ¾Ð³Ð¾Ð´Ð½Ñ–',
        monthNames: ['Ð¡Ñ–Ñ‡ÐµÐ½ÑŒ', 'Ð›ÑŽÑ‚Ð¸Ð¹', 'Ð‘ÐµÑ€ÐµÐ·ÐµÐ½ÑŒ', 'ÐšÐ²Ñ–Ñ‚ÐµÐ½ÑŒ', 'Ð¢Ñ€Ð°Ð²ÐµÐ½ÑŒ', 'Ð§ÐµÑ€Ð²ÐµÐ½ÑŒ',
        'Ð›Ð¸Ð¿ÐµÐ½ÑŒ', 'Ð¡ÐµÑ€Ð¿ÐµÐ½ÑŒ', 'Ð’ÐµÑ€ÐµÑÐµÐ½ÑŒ', 'Ð–Ð¾Ð²Ñ‚ÐµÐ½ÑŒ', 'Ð›Ð¸ÑÑ‚Ð¾Ð¿Ð°Ð´', 'Ð“Ñ€ÑƒÐ´ÐµÐ½ÑŒ'],
        monthNamesShort: ['Ð¡Ñ–Ñ‡', 'Ð›ÑŽÑ‚', 'Ð‘ÐµÑ€', 'ÐšÐ²Ñ–', 'Ð¢Ñ€Ð°', 'Ð§ÐµÑ€',
        'Ð›Ð¸Ð¿', 'Ð¡ÐµÑ€', 'Ð’ÐµÑ€', 'Ð–Ð¾Ð²', 'Ð›Ð¸Ñ', 'Ð“Ñ€Ñƒ'],
        dayNames: ['Ð½ÐµÐ´Ñ–Ð»Ñ', 'Ð¿Ð¾Ð½ÐµÐ´Ñ–Ð»Ð¾Ðº', 'Ð²Ñ–Ð²Ñ‚Ð¾Ñ€Ð¾Ðº', 'ÑÐµÑ€ÐµÐ´Ð°', 'Ñ‡ÐµÑ‚Ð²ÐµÑ€', 'Ð¿â€™ÑÑ‚Ð½Ð¸Ñ†Ñ', 'ÑÑƒÐ±Ð¾Ñ‚Ð°'],
        dayNamesShort: ['Ð½ÐµÐ´', 'Ð¿Ð½Ð´', 'Ð²Ñ–Ð²', 'ÑÑ€Ð´', 'Ñ‡Ñ‚Ð²', 'Ð¿Ñ‚Ð½', 'ÑÐ±Ñ‚'],
        dayNamesMin: ['ÐÐ´', 'ÐŸÐ½', 'Ð’Ñ‚', 'Ð¡Ñ€', 'Ð§Ñ‚', 'ÐŸÑ‚', 'Ð¡Ð±'],
        weekHeader: 'Ð¢Ð¸Ð¶',
        dateFormat: 'dd.mm.yy',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    datepicker.setDefaults(datepicker.regional['uk']);

    var i18nDatepickerUk = datepicker.regional['uk'];


    /* Vietnamese initialisation for the jQuery UI date picker plugin. */
    /* Translated by Le Thanh Huy (lthanhhuy@cit.ctu.edu.vn). */


    datepicker.regional['vi'] = {
        closeText: 'ÄÃ³ng',
        prevText: '&#x3C;TrÆ°á»›c',
        nextText: 'Tiáº¿p&#x3E;',
        currentText: 'HÃ´m nay',
        monthNames: ['ThÃ¡ng Má»™t', 'ThÃ¡ng Hai', 'ThÃ¡ng Ba', 'ThÃ¡ng TÆ°', 'ThÃ¡ng NÄƒm', 'ThÃ¡ng SÃ¡u',
        'ThÃ¡ng Báº£y', 'ThÃ¡ng TÃ¡m', 'ThÃ¡ng ChÃ­n', 'ThÃ¡ng MÆ°á»i', 'ThÃ¡ng MÆ°á»i Má»™t', 'ThÃ¡ng MÆ°á»i Hai'],
        monthNamesShort: ['ThÃ¡ng 1', 'ThÃ¡ng 2', 'ThÃ¡ng 3', 'ThÃ¡ng 4', 'ThÃ¡ng 5', 'ThÃ¡ng 6',
        'ThÃ¡ng 7', 'ThÃ¡ng 8', 'ThÃ¡ng 9', 'ThÃ¡ng 10', 'ThÃ¡ng 11', 'ThÃ¡ng 12'],
        dayNames: ['Chá»§ Nháº­t', 'Thá»© Hai', 'Thá»© Ba', 'Thá»© TÆ°', 'Thá»© NÄƒm', 'Thá»© SÃ¡u', 'Thá»© Báº£y'],
        dayNamesShort: ['CN', 'T2', 'T3', 'T4', 'T5', 'T6', 'T7'],
        dayNamesMin: ['CN', 'T2', 'T3', 'T4', 'T5', 'T6', 'T7'],
        weekHeader: 'Tu',
        dateFormat: 'dd/mm/yy',
        firstDay: 0,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    datepicker.setDefaults(datepicker.regional['vi']);

    var i18nDatepickerVi = datepicker.regional['vi'];


    /* Chinese initialisation for the jQuery UI date picker plugin. */
    /* Written by Cloudream (cloudream@gmail.com). */


    datepicker.regional['zh-CN'] = {
        closeText: 'å…³é—­',
        prevText: '&#x3C;ä¸Šæœˆ',
        nextText: 'ä¸‹æœˆ&#x3E;',
        currentText: 'ä»Šå¤©',
        monthNames: ['ä¸€æœˆ', 'äºŒæœˆ', 'ä¸‰æœˆ', 'å››æœˆ', 'äº”æœˆ', 'å…­æœˆ',
        'ä¸ƒæœˆ', 'å…«æœˆ', 'ä¹æœˆ', 'åæœˆ', 'åä¸€æœˆ', 'åäºŒæœˆ'],
        monthNamesShort: ['ä¸€æœˆ', 'äºŒæœˆ', 'ä¸‰æœˆ', 'å››æœˆ', 'äº”æœˆ', 'å…­æœˆ',
        'ä¸ƒæœˆ', 'å…«æœˆ', 'ä¹æœˆ', 'åæœˆ', 'åä¸€æœˆ', 'åäºŒæœˆ'],
        dayNames: ['æ˜ŸæœŸæ—¥', 'æ˜ŸæœŸä¸€', 'æ˜ŸæœŸäºŒ', 'æ˜ŸæœŸä¸‰', 'æ˜ŸæœŸå››', 'æ˜ŸæœŸäº”', 'æ˜ŸæœŸå…­'],
        dayNamesShort: ['å‘¨æ—¥', 'å‘¨ä¸€', 'å‘¨äºŒ', 'å‘¨ä¸‰', 'å‘¨å››', 'å‘¨äº”', 'å‘¨å…­'],
        dayNamesMin: ['æ—¥', 'ä¸€', 'äºŒ', 'ä¸‰', 'å››', 'äº”', 'å…­'],
        weekHeader: 'å‘¨',
        dateFormat: 'yy-mm-dd',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: true,
        yearSuffix: 'å¹´'
    };
    datepicker.setDefaults(datepicker.regional['zh-CN']);

    var i18nDatepickerZhCn = datepicker.regional['zh-CN'];


    /* Chinese initialisation for the jQuery UI date picker plugin. */
    /* Written by SCCY (samuelcychan@gmail.com). */


    datepicker.regional['zh-HK'] = {
        closeText: 'é—œé–‰',
        prevText: '&#x3C;ä¸Šæœˆ',
        nextText: 'ä¸‹æœˆ&#x3E;',
        currentText: 'ä»Šå¤©',
        monthNames: ['ä¸€æœˆ', 'äºŒæœˆ', 'ä¸‰æœˆ', 'å››æœˆ', 'äº”æœˆ', 'å…­æœˆ',
        'ä¸ƒæœˆ', 'å…«æœˆ', 'ä¹æœˆ', 'åæœˆ', 'åä¸€æœˆ', 'åäºŒæœˆ'],
        monthNamesShort: ['ä¸€æœˆ', 'äºŒæœˆ', 'ä¸‰æœˆ', 'å››æœˆ', 'äº”æœˆ', 'å…­æœˆ',
        'ä¸ƒæœˆ', 'å…«æœˆ', 'ä¹æœˆ', 'åæœˆ', 'åä¸€æœˆ', 'åäºŒæœˆ'],
        dayNames: ['æ˜ŸæœŸæ—¥', 'æ˜ŸæœŸä¸€', 'æ˜ŸæœŸäºŒ', 'æ˜ŸæœŸä¸‰', 'æ˜ŸæœŸå››', 'æ˜ŸæœŸäº”', 'æ˜ŸæœŸå…­'],
        dayNamesShort: ['å‘¨æ—¥', 'å‘¨ä¸€', 'å‘¨äºŒ', 'å‘¨ä¸‰', 'å‘¨å››', 'å‘¨äº”', 'å‘¨å…­'],
        dayNamesMin: ['æ—¥', 'ä¸€', 'äºŒ', 'ä¸‰', 'å››', 'äº”', 'å…­'],
        weekHeader: 'å‘¨',
        dateFormat: 'dd-mm-yy',
        firstDay: 0,
        isRTL: false,
        showMonthAfterYear: true,
        yearSuffix: 'å¹´'
    };
    datepicker.setDefaults(datepicker.regional['zh-HK']);

    var i18nDatepickerZhHk = datepicker.regional['zh-HK'];


    /* Chinese initialisation for the jQuery UI date picker plugin. */
    /* Written by Ressol (ressol@gmail.com). */


    datepicker.regional['zh-TW'] = {
        closeText: 'é—œé–‰',
        prevText: '&#x3C;ä¸Šæœˆ',
        nextText: 'ä¸‹æœˆ&#x3E;',
        currentText: 'ä»Šå¤©',
        monthNames: ['ä¸€æœˆ', 'äºŒæœˆ', 'ä¸‰æœˆ', 'å››æœˆ', 'äº”æœˆ', 'å…­æœˆ',
        'ä¸ƒæœˆ', 'å…«æœˆ', 'ä¹æœˆ', 'åæœˆ', 'åä¸€æœˆ', 'åäºŒæœˆ'],
        monthNamesShort: ['ä¸€æœˆ', 'äºŒæœˆ', 'ä¸‰æœˆ', 'å››æœˆ', 'äº”æœˆ', 'å…­æœˆ',
        'ä¸ƒæœˆ', 'å…«æœˆ', 'ä¹æœˆ', 'åæœˆ', 'åä¸€æœˆ', 'åäºŒæœˆ'],
        dayNames: ['æ˜ŸæœŸæ—¥', 'æ˜ŸæœŸä¸€', 'æ˜ŸæœŸäºŒ', 'æ˜ŸæœŸä¸‰', 'æ˜ŸæœŸå››', 'æ˜ŸæœŸäº”', 'æ˜ŸæœŸå…­'],
        dayNamesShort: ['å‘¨æ—¥', 'å‘¨ä¸€', 'å‘¨äºŒ', 'å‘¨ä¸‰', 'å‘¨å››', 'å‘¨äº”', 'å‘¨å…­'],
        dayNamesMin: ['æ—¥', 'ä¸€', 'äºŒ', 'ä¸‰', 'å››', 'äº”', 'å…­'],
        weekHeader: 'å‘¨',
        dateFormat: 'yy/mm/dd',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: true,
        yearSuffix: 'å¹´'
    };
    datepicker.setDefaults(datepicker.regional['zh-TW']);

    var i18nDatepickerZhTw = datepicker.regional['zh-TW'];



}));