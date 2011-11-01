function ChangeReputationBarColor(value,text, selector){
			   jQuery(selector).html('<div class="ui-progressbar-value ui-widget-header ui-corner-left ReputationBarValue" style="width: ' + value + '%; "></div>');
			   jQuery(selector).attr('aria-valuenow', value);
			   
			    // 0%
		        if (value == 0){
                    jQuery(selector).children('.ReputationBarValue').addClass("BarDarkRed");
					
					jQuery(selector).children('.ReputationBarValue').prepend('<p class="ReputationBarText">' + text + '</p>');
                }
				// 1-29%
				else if (value < 20){
					
					jQuery(selector).children('.ReputationBarValue').addClass("BarRed");
					
					jQuery(selector).children('.ReputationBarValue').prepend('<p class="ReputationBarText">' + text + '</p>');
                }
				// 30-39%
				else if (value < 30){
                    jQuery(selector).children('.ReputationBarValue').addClass("BarOrangeRed");
					
					jQuery(selector).children('.ReputationBarValue').prepend('<p class="ReputationBarText">' + text + '</p>');
                }
				// 40-49%
				else if (value < 40){
                    jQuery(selector).children('.ReputationBarValue').addClass("BarDarkOrange");
					
					jQuery(selector).children('.ReputationBarValue').prepend('<p class="ReputationBarText">' + text + '</p>');
                }
				// 50-59%
				else if (value < 50){
                    jQuery(selector).children('.ReputationBarValue').addClass("BarOrange");
					
					jQuery(selector).children('.ReputationBarValue').prepend('<p class="ReputationBarText">' + text + '</p>');
                }
				// 60-69%
				else if (value < 60){
                   jQuery(selector).children('.ReputationBarValue').addClass("BarYellow");
				   
				   jQuery(selector).children('.ReputationBarValue').prepend('<p class="ReputationBarText">' + text + '</p>');
                }
				// 70-79%
				else if (value < 80){
                    jQuery(selector).children('.ReputationBarValue').addClass("BarLightGreen");
					
					jQuery(selector).children('.ReputationBarValue').prepend('<p class="ReputationBarText">' + text + '</p>');
                }
				// 80-89%
				else if (value < 90){
                    jQuery(selector).children('.ReputationBarValue').addClass("BarGreen");
	
					jQuery(selector).children('.ReputationBarValue').prepend('<p class="ReputationBarText">' + text + '</p>');
                }
				// 90-100%
				else{
					jQuery(selector).html('<div class="ui-progressbar-value ui-widget-header ui-corner-left ui-corner-right ReputationBarValue" style="width: ' + value + '%; "></div>');
                    
					jQuery(selector).children('.ReputationBarValue').addClass("BarDarkGreen");
					
					jQuery(selector).children('.ReputationBarValue').prepend('<p class="ReputationBarText">' + text + '</p>');
                }
}

function ScrollTop() { jQuery('body,html').animate({ scrollTop: 0 }, 820); return false; }


function toggleContainer(id, senderId, showText, hideText){
	var el = jQuery('#' + id);
	var sender = jQuery('#' + senderId);

	el.toggle(function() {
		sender.attr("title", hideText);
		sender.html(hideText);
		sender.addClass('hide');
	}, function() {
		sender.attr("title", showText);
		sender.html(showText);
		sender.addClass('show');
	});
}

function getEvent(eventobj) {
	if(eventobj.stopPropagation) {
		eventobj.stopPropagation();
		eventobj.preventDefault();
		return eventobj;
	} else {
		window.event.returnValue = false;
		window.event.cancelBubble = true;
		return window.event;
	}
}

function yaf_mouseover() {
	var evt = getEvent(window.event);
	if(evt.srcElement) {
		evt.srcElement.style.cursor = "hand";
	} else if(evt.target) {
		evt.target.style.cursor = "pointer";
	}
}

function yaf_left(obj) {
    return jQuery(obj).position().left; 
}

function yaf_top(obj) {
    return jQuery(obj).position().top + jQuery(obj).outerHeight() + 1;
}

function yaf_popit(menuName) {
	var evt = getEvent(window.event);
	var target,newmenu;

	if(!document.getElementById) {
		throw('ERROR: missing getElementById');
	}
	
	if(evt.srcElement)
		target = evt.srcElement;
	else if(evt.target)
		target = evt.target;
	else {
		throw('ERROR: missing event target');
	}
	
	newmenu = document.getElementById(menuName);

	if(window.themenu && window.themenu.id!=newmenu.id)
		yaf_hidemenu();

	window.themenu = newmenu;
	if(!window.themenu.style) {
		throw('ERROR: missing style');
	}

	if (!jQuery(themenu).is(":visible")) {
	    var x = yaf_left(target);
	    // Make sure the menu stays inside the page
	    // offsetWidth or clientWidth?!?
	    if (x + jQuery(themenu).outerWidth() + 2 > jQuery(document).width()) {
	        x = jQuery(document).width() - jQuery(themenu).outerWidth() - 2;
	    }

	    themenu.style.left = x + "px";
	    themenu.style.top = yaf_top(target) + "px";
	    themenu.style.zIndex = 100;

	    jQuery(themenu).fadeIn();
	} else {
	    yaf_hidemenu();
	}

	return false;
}

function yaf_hidemenu() {
	if(window.themenu) {
		jQuery(window.themenu).fadeOut();
		window.themenu = null;
	}
}

function mouseHover(cell,hover) {
	if(hover) {
		cell.className = "popupitemhover";
		try {
			cell.style.cursor = "pointer";
		}
		catch(e) {
			cell.style.cursor = "hand";
		}
	} else {
		cell.className = "popupitem";
	}
}

document.onclick = yaf_hidemenu;
if(document.addEventListener) document.addEventListener("click",function(e){window.event=e;},true);
if(document.addEventListener) document.addEventListener("mouseover",function(e){window.event=e;},true);
