/**
 * Javascript functions to show top nitification
 * Error/Success/Info/Warning messages
 * Developed By: Ravi Tamada
 * url: http://androidhive.info
 * © androidhive.info
 * 
 * Created On: 10/4/2011
 * version 1.0
 * 
 * Usage: call this function with params 
 showNotification(params);
 **/

function showNotification(params){
    // options array
    var options = { 
        'showAfter': 0, // number of sec to wait after page loads
        'duration': 0, // display duration
        'autoClose' : false, // flag to autoClose notification message
        'type' : 'success', // type of info message error/success/info/warning
        'message': '', // message to dispaly
        'link_notification' : '', // link flag to show extra description
        'description' : '', // link to desciption to display on clicking link message
		'imagepath' : ''
    }; 
    // Extending array from params
    $.extend(true, options, params);
    
    var msgclass = 'succ_bg'; // default success message will shown
    if(options['type'] == 'error'){
        msgclass = 'error_bg'; // over write the message to error message
    } else if(options['type'] == 'information'){
        msgclass = 'info_bg'; // over write the message to information message
    } else if(options['type'] == 'warning'){
        msgclass = 'warn_bg'; // over write the message to warning message
    } 
	
	var icon = 'tick.png';
	if(options['type'] == 'error'){
        icon = 'error.png'; // over write the message to error message
    } else if(options['type'] == 'information'){
        icon = 'information.png'; // over write the message to information message
    } else if(options['type'] == 'warning'){
        icon = 'warning.png'; // over write the message to warning message
    } 
	
    // Parent Div container
    var container = '<div id="info_message" class="notification_background '+msgclass+'" onclick="return closeNotification();" title="Click to Hide Notification"><div class="center_auto"><div class="info_message_text message_area">';
	container += '<img class="message_icon" src="' + options['imagepath'] + icon + '" alt="'+ options['type'] + '" title="'+ options['type'] + '" />&nbsp;';
    container += options['message'].replaceAll('\\n', '<br />');
	container += '</div><div class="info_progress"></div><div class="clearboth"></div>';
	container += '</div>';
    
    $notification = $(container);
    
    // Appeding notification to Body
    $('body').append($notification);
    
    var divHeight = $('div#info_message').height();
	
    // see CSS top to minus of div height
    $('div#info_message').css({
        top : '-'+divHeight+'px'
    });
    
    // showing notification message, default it will be hidden
    $('div#info_message').show();
    
    // Slide Down notification message after startAfter seconds
    slideDownNotification(options['showAfter'], options['autoClose'],options['duration']);
	
	var animationDuration = options['duration'] + "s";
	var progressDuration = (options['duration'] -1) + "s";
	
	$('div#info_message').css("-webkit-animation-duration", animationDuration).css("-moz-animation-duration", animationDuration).css("-o-animation-duration", animationDuration).css("-ms-animation-duration", animationDuration).css("animation-duration", animationDuration);
	
	$('#info_message .info_progress').css("-webkit-animation-duration", progressDuration).css("-moz-animation-duration", progressDuration).css("-o-animation-duration", progressDuration).css("-ms-animation-duration", progressDuration).css("animation-duration", progressDuration);
    
	$('body').on('click', '.link_notification', function () {
        $('.info_more_descrption').html(options['description']).slideDown('fast');
    });
    
}
String.prototype.replaceAll = function(token, newToken, ignoreCase) {
    var str, i = -1, _token;
    if((str = this.toString()) && typeof token === "string") {
        _token = ignoreCase === true? token.toLowerCase() : undefined;
        while((i = (
            _token !== undefined? 
                str.toLowerCase().indexOf(
                            _token, 
                            i >= 0? i + newToken.length : 0
                ) : str.indexOf(
                            token,
                            i >= 0? i + newToken.length : 0
                )
        )) !== -1 ) {
            str = str.substring(0, i)
                    .concat(newToken)
                    .concat(str.substring(i + token.length));
        }
    }
return str;
};
// function to close notification message
// slideUp the message
function closeNotification(duration){
    var divHeight = $('div#info_message').height();
    setTimeout(function(){
        $('div#info_message').animate({
            top: '-'+divHeight
        }); 
        // removing the notification from body
        setTimeout(function(){
            $('div#info_message').remove();
        },200);
    }, parseInt(duration * 1000));   
    

    
}

// sliding down the notification
function slideDownNotification(startAfter, autoClose, duration){    
    setTimeout(function(){
        $('div#info_message').animate({
            top: 0
        }); 
        if(autoClose){
            setTimeout(function(){
                closeNotification(duration);
            }, duration);
        }
    }, parseInt(startAfter * 1000));    
}