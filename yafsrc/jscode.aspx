<%@ Page language="c#" Codebehind="jscode.aspx.cs" AutoEventWireup="false" Inherits="yaf.jscode" %>

// Agent: <%= Request.UserAgent %>
// Browser: <%= Request.Browser.Browser %>

function insertsmiley(code) {
	myReplace(code);
}

function myWrap(pre,post) {
<%if(IsIE)%>
	var range = document.selection.createRange();
	if(range.text)
		range.text = pre + range.text + post;
<%else if(IsNetscape)%>
	var textObj = document.getElementById('Message');
	var pretext = textObj.value.substring(0,textObj.selectionStart);
	var seltext = textObj.value.substring(textObj.selectionStart,textObj.selectionEnd);
	var posttext = textObj.value.substring(textObj.selectionEnd,textObj.textLength);
	
	textObj.value = pretext + pre + seltext + post + posttext;
<%else%>
	var textObj = document.getElementById('Message');
	textObj.value += pre + post;
<%%>
}

function myReplace(insrt) {
<%if(IsIE)%>	
	var range = document.selection.createRange();
	if(range.text)
		range.text = insrt;
	else {
		var textObj = document.getElementById('Message');
		textObj.value += insrt;
	}
<%else if(IsNetscape)%>
	var textObj = document.getElementById('Message');
	var pretext = textObj.value.substring(0,textObj.selectionStart);
	var posttext = textObj.value.substring(textObj.selectionEnd,textObj.textLength);
	textObj.value = pretext + insrt + posttext;
<%else%>
	var textObj = document.getElementById('Message');
	textObj.value += insrt;
<%%>
}
function makebold() {
	myWrap('[b]','[/b]');
}
function makeitalic() {
	myWrap('[i]','[/i]');
}
function makeunderline() {
	myWrap('[u]','[/u]');
}
function makequote() {
	myWrap('[quote]','[/quote]');
}
function makecode() {
	myWrap('[code]','[/code]');
}
function makeurl() {
	var url = prompt('Enter URL:','http://');
	if(url!='' && url!=null)
		myWrap('[url="'+url+'"]','[/url]');
}
function makeimg() {
	var img = prompt('Enter image URL:','http://');
	if(img!='' && img!=null)
		myReplace('[img]'+img+'[/img]');
}