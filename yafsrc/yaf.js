function getEvent(eventobj) {
	if (!eventobj || window.event) {
		window.event.returnValue = false;
		window.event.cancelBubble = true;
		return window.event;
	} else {
		eventobj.stopPropagation();
		eventobj.preventDefault();
		return eventobj;
	}
}

function yaf_mouseover(evt) {
	evt = getEvent(evt);
	if(evt.srcElement) {
		evt.srcElement.style.cursor = "hand";
	} else if(evt.target) {
		evt.target.style.cursor = "pointer";
	}
}

function yaf_left(obj) {
	var x = 0;
	while(obj) {
		x += obj.offsetLeft;
		obj = obj.offsetParent;
	}
	return x;
}

function yaf_top(obj) {
	//alert(obj.x);
	window.myobj = obj;
	var y = 20;
	while(obj) {
		y += obj.offsetTop;
		obj = obj.offsetParent;
	}
	return y;
}

function yaf_popit(evt) {
	evt = getEvent(evt);
	var newmenu;

	if(!document.getElementById) {
		alert('ERROR: missing getElementById');
		return false;
	}
	
	if(evt.srcElement)
		newmenu = document.getElementById(evt.srcElement.id + '_menu');
	else if(evt.target)
		newmenu = document.getElementById(evt.target.id + '_menu');
	else {
		alert('ERROR: missing event target');
		return false;
	}

	if(window.themenu && window.themenu.id!=newmenu.id)
		yaf_hidemenu();

	window.themenu = newmenu;
	if(!window.themenu.style) {
		alert('ERROR: missing style');
		return false;
	}

	if(themenu.style.visibility == "hidden") {
		themenu.style.left = yaf_left(evt.target ? evt.target : evt.srcElement);
		themenu.style.top = yaf_top(evt.target ? evt.target : evt.srcElement);
		themenu.style.visibility = "visible";
		themenu.style.zIndex = 100;
	} else {
		yaf_hidemenu();
	}

	return false;
}

function yaf_initmenu(menuname) {
	if(document.getElementById) {
		var obj = document.getElementById(menuname);
		if(!obj) {
			alert(menuname);
			return;
		}
		obj.onmouseover = yaf_mouseover;
		obj.onclick = yaf_popit;
	}
}

function yaf_hidemenu() {
	if(window.themenu) {
		window.themenu.style.visibility = "hidden";
		window.themenu = null;
	}
}

function mouseHover(cell,hover) {
	if(hover) {
		cell.className = "postfooter";
		try {
			cell.style.cursor = "pointer";
		}
		catch(e) {
			cell.style.cursor = "hand";
		}
	} else {
		cell.className = "post";
	}
}

document.onclick = yaf_hidemenu;
