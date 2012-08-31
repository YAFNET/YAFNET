//Title: Hovercard plugin by PC 
//Documentation: http://designwithpc.com/Plugins/Hovercard
//Author: PC 
//Website: http://designwithpc.com
//Twitter: @chaudharyp

(function ($) {
    $.fn.hovercard = function (options) {

        //Set defauls for the control
        var defaults = {
            width: 300,
            openOnLeft: false,
            openOnTop: false,
            cardImgSrc: "",
            detailsHTML: "",
			loadingHTML: "Loading1...",
			errorHTML: "Sorry, no data found.",
            twitterScreenName: '',
            showTwitterCard: false,
			showYafCard: false,
            facebookUserName: '',
            showFacebookCard: false,
            showCustomCard: false,
            customCardJSON: {},
            customDataUrl: '',
            background: "#ffffff",
            delay: 0,
            autoAdjust: true,
            onHoverIn: function () { },
            onHoverOut: function () { }
        };
        
        //Update unset options with defaults if needed
        options = $.extend(defaults, options); //Executing functionality on all selected elements
        return this.each(function () {
            var obj = $(this).eq(0);
			
			//wrap a parent span to the selected element
            obj.wrap('<div class="hc-preview" />');
		
            //add a relatively positioned class to the selected element
            obj.addClass("hc-name");

            //if card image src provided then generate the image elementk
            var hcImg = '';
            if (options.cardImgSrc.length > 0) {
                hcImg = '<img class="hc-pic" src="' + options.cardImgSrc + '" />';
            }

            //generate details span with html provided by the user
            var hcDetails = '<div class="hc-details ui-widget ui-widget-content ui-corner-all" >' + hcImg + options.detailsHTML + '</div>';

            //append this detail after the selected element
            obj.after(hcDetails);
            obj.siblings(".hc-details").eq(0).css({ 'width': options.width, 'background': options.background });
			
            //toggle hover card details on hover
            obj.closest(".hc-preview").hover(function () {
				
				var $this = $(this);
                adjustToViewPort($this);
				
				// Up the z indiex for the .hc-name to overlay on .hc-details
				obj.css("zIndex", "200");

                var curHCDetails = $this.find(".hc-details").eq(0);
                curHCDetails.stop(true, true).delay(options.delay).fadeIn();

                //Default functionality on hoverin, and also allows callback
                if (typeof options.onHoverIn == 'function') {

                    //check for custom profile. If already loaded don't load again
                    var dataUrl;
                    if (options.showCustomCard && curHCDetails.find('.s-card').length <= 0) {

                        //Read data-hovercard url from the hovered element, otherwise look in the options. For custom card, complete url is required than just username.
                        dataUrl = options.customDataUrl;
                        if (typeof obj.attr('data-hovercard') == 'undefined') {
                            //do nothing. detecting typeof obj.attr('data-hovercard') != 'undefined' didn't work as expected.
                        } else if (obj.attr('data-hovercard').length > 0) {
                            dataUrl = obj.attr('data-hovercard');
                        }

                        LoadSocialProfile("custom", "", dataUrl, curHCDetails, options.customCardJSON);
                    }
					
					//check for yaf profile. If already loaded don't load again
                    if (options.showYafCard && curHCDetails.find('.s-card').length <= 0) {

                        //Read data-hovercard url from the hovered element, otherwise look in the options. For custom card, complete url is required than just username.
                        dataUrl = options.customDataUrl;
                        if (typeof obj.attr('data-hovercard') == 'undefined') {
                            //do nothing. detecting typeof obj.attr('data-hovercard') != 'undefined' didn't work as expected.
                        } else if (obj.attr('data-hovercard').length > 0) {
                            dataUrl = obj.attr('data-hovercard');
                        }

                        LoadSocialProfile("yaf", obj.attr('href'), dataUrl, curHCDetails, options.customCardJSON);
                    }

                    //check for twitter profile. If already loaded don't load again
                    if (options.showTwitterCard && curHCDetails.find('.s-card').eq(0).length <= 0) {

                        //Look for twitter screen name in data-hovercard first, then in options, otherwise try with the hovered text
                        var tUsername = options.twitterScreenName.length > 0 ? options.twitterScreenName : obj.text();
                        if (typeof obj.attr('data-hovercard') == 'undefined') {
                            //do nothing. detecting typeof obj.attr('data-hovercard') != 'undefined' didn't work as expected.
                        } else if (obj.attr('data-hovercard').length > 0) {
                            tUsername = obj.attr('data-hovercard');
                        }

                        LoadSocialProfile("twitter", "", tUsername, curHCDetails);
                    }

                    //check for facebook profile. If already loaded don't load again
                    if (options.showFacebookCard && curHCDetails.find('.s-card').eq(0).length <= 0) {

                        //Look for twitter screen name in data-hovercard first, then in options, otherwise try with the hovered text
                        var fbUsername = options.facebookUserName.length > 0 ? options.facebookUserName : obj.text();
                        if (typeof obj.attr('data-hovercard') == 'undefined') {
                            //do nothing. detecting typeof obj.attr('data-hovercard') != 'undefined' didn't work as expected.
                        } else if (obj.attr('data-hovercard').length > 0) {
                            fbUsername = obj.attr('data-hovercard');
                        }

                        LoadSocialProfile("facebook", "", fbUsername, curHCDetails);
                    }

                    //Callback function                    
                    options.onHoverIn.call(this);                    
                }

            }, function () {
                var $this = $(this);
                $this.find(".hc-details").eq(0).stop(true, true).fadeOut(300, function () {
					
					//Undo the z indices 
					obj.css("zIndex", "50");
					
					if (typeof options.onHoverOut == 'function') {
                        options.onHoverOut.call(this);
                    }
                });
            });

            //Opening Directions adjustment
            function adjustToViewPort(hcPreview) {

                var hcDetails = hcPreview.find('.hc-details').eq(0);
                var hcPreviewRect = hcPreview[0].getBoundingClientRect();

                var hcdTop = hcPreviewRect.top - 20; //Subtracting 20px of padding;
                var hcdRight = hcPreviewRect.left + 35 + hcDetails.width(); //Adding 35px of padding;
                var hcdBottom = hcPreviewRect.top + 35 + hcDetails.height(); //Adding 35px of padding;
                var hcdLeft = hcPreviewRect.top - 10; //Subtracting 10px of padding;

                //Check for forced open directions, or if need to be autoadjusted
                if (options.openOnLeft || (options.autoAdjust && (hcdRight > window.innerWidth))) {
                    hcDetails.addClass("hc-details-open-left");
                } else {
                    hcDetails.removeClass("hc-details-open-left");
                }
                if (options.openOnTop || (options.autoAdjust && (hcdBottom > window.innerHeight))) {
                    hcDetails.addClass("hc-details-open-top");
                } else {
                   hcDetails.removeClass("hc-details-open-top");
                }
            }

            //Private base function to load any social profile
            function LoadSocialProfile(type, href, username, curHCDetails, customCardJSON) {
                var cardHTML,dataType, urlToRequest, customCallback, loadingHTML, errorHTML;

                switch (type) {
                    case "twitter":
                        {
                            dataType ='jsonp',
                            urlToRequest = 'https://api.twitter.com/1/users/lookup.json?screen_name=' + username;
                            cardHTML = function (profileData) {
                                profileData = profileData[0];
                                return '<div class="s-card s-card-pad">' +
                                                        (profileData.profile_image_url ? ('<img class="s-img" src="' + profileData.profile_image_url + '" />') : '') +
                                                        (profileData.name ? ('<label class="s-name">' + profileData.name + ' </label>') : '') +
                                                        (profileData.screen_name ? ('(<a class="s-username" title="Visit Twitter profile for ' + profileData.name + '" href="http://twitter.com/' + profileData.screen_name + '">@' + profileData.screen_name + '</a>)<br/>') : '') +
                                                        (profileData.location ? ('<label class="s-loc">' + profileData.location + '</label>') : '') +
                                                        (profileData.description ? ('<p class="s-desc">' + profileData.description + '</p>') : '') +
                                                        (profileData.url ? ('<a class="s-href" href="' + profileData.url + '">' + profileData.url + '</a><br/>') : '') +

                                                        '<ul class="s-stats">' +
                                                            (profileData.statuses_count ? ('<li>Tweets<br /><span class="s-count">' + profileData.statuses_count + '</span></li>') : '') +
                                                            (profileData.friends_count ? ('<li>Following<br /><span class="s-count">' + profileData.friends_count + '</span></li>') : '') +
                                                            (profileData.followers_count ? ('<li>Followers<br /><span class="s-count">' + profileData.followers_count + '</span></li>') : '') +
                                                        '</ul>' +
                                                    '</div>';
                            };
                            
							loadingHTML = options.loadingHTML;
                            errorHTML = options.errorHTML;
                            customCallback = function () { };
							
							//Append the twitter script to the document to add a follow button
                            if ($('#t-follow-script').length <= 0) {
                                var script = document.createElement('script');
                                script.type = 'text/javascript';
                                script.src = '//platform.twitter.com/widgets.js';
                                script.id = 't-follow-script';
                                $('body').append(script);
                            }
                            curHCDetails.append('<span class="s-action"><a href="https://twitter.com/' + username + '" class="twitter-follow-button" data-show-count="false" data-show-name="false" data-button="grey" data-width="65px" class="twitter-follow-button">Follow</a></span>');

                        }
                        break;
                    case "facebook":
                        {
                            dataType ='json',
                            urlToRequest = 'https://graph.facebook.com/' + username,
                            cardHTML = function (profileData) {
								return '<div class="s-card s-card-pad">' +
                                        '<img class="s-img" src="http://graph.facebook.com/' + profileData.id + '/picture" />' +
                                        '<label class="s-name">' + profileData.name + ' </label><br/>' +
                                        (profileData.link ? ('<a class="s-loc" href="' + profileData.link + '">' + profileData.link + '</a><br/>') : '') +
                                        (profileData.likes ? ('<label class="s-loc">Liked by </span> ' + profileData.likes + '</label><br/>') : '') +
                                        (profileData.description ? ('<p class="s-desc">' + profileData.description + '</p>') : '') +
                                        (profileData.start_time ? ('<p class="s-desc"><span class="s-strong">Start Time:</span><br/>' + profileData.start_time + '</p>') : '') +
                                        (profileData.end_time ? ('<p class="s-desc"><span class="s-strong">End Time:<br/>' + profileData.end_time + '</p>') : '') +
                                        (profileData.founded ? ('<p class="s-desc"><span class="s-strong">Founded:</span><br/>' + profileData.founded + '</p>') : '') +
                                        (profileData.mission ? ('<p class="s-desc"><span class="s-strong">Mission:</span><br/>' + profileData.mission + '</p>') : '') +
                                        (profileData.company_overview ? ('<p class="s-desc"><span class="s-strong">Overview:</span><br/>' + profileData.company_overview + '</p>') : '') +
                                        (profileData.products ? ('<p class="s-desc"><span class="s-strong">Products:</span><br/>' + profileData.products + '</p>') : '') +
                                        (profileData.website ? ('<p class="s-desc"><span class="s-strong">Web:</span><br/><a href="' + profileData.website + '">' + profileData.website + '</a></p>') : '') +
                                        (profileData.email ? ('<p class="s-desc"><span class="s-strong">Email:</span><br/><a href="' + profileData.email + '">' + profileData.email + '</a></p>') : '') +
                                        '</div>';
                            };
							loadingHTML = options.loadingHTML;
                            errorHTML = options.errorHTML;
                            
                            customCallback = function (profileData) {
                                if ($('#fb-like' + profileData.id).length > 0) {
                                    curHCDetails.append('<span class="s-action">' + $('#fb-like' + profileData.id).html() + '</span>');
								}
								else
								{
									curHCDetails.append('<span class="s-action"><div class="fb-like" id="fb-like' + profileData.id + '"><iframe src="//www.facebook.com/plugins/like.php?href=' + profileData.link +';send=false&amp;layout=standard&amp;width=90&amp;show_faces=false&amp;action=like&amp;layout=button_count&amp;font&amp;height=21&amp" scrolling="no" frameborder="0" style="border:none; overflow:hidden;width:77px;height:21px" allowTransparency="true"></iframe></div></span>');
								}
                            };
                        }
                        break;
                    case "custom":
                        {
                            dataType ='jsonp',
                            urlToRequest = username,
                            cardHTML = function (profileData) {
								profileData = profileData[0];
                                return '<div class="s-card s-card-pad">' +
                                        (profileData.image ? ('<img class="s-img" src=' + profileData.image + ' />') : '') +
                                        (profileData.name ? ('<label class="s-name">' + profileData.name + ' </label><br/>') : '') +
                                        (profileData.link ? ('<a class="s-loc" href="' + profileData.link + '">' + profileData.link + '</a><br/>') : '') +
                                        (profileData.bio ? ('<p class="s-desc">' + profileData.bio + '</p>') : '') +
                                        (profileData.website ? ('<p class="s-desc"><span class="s-strong">Web:</span><br/><a href="' + profileData.website + '">' + profileData.website + '</a></p>') : '') +
                                        (profileData.email ? ('<p class="s-desc"><span class="s-strong">Email:</span><br/><a href="' + profileData.email + '">' + profileData.email + '</a></p>') : '') +
                                        '</div>';
                            };
                            loadingHTML = options.loadingHTML;
                            errorHTML = options.errorHTML;
                            customCallback = function () { };
                        }
                        break;
					case "yaf":
                        {
                            dataType ='json',
                            urlToRequest = username,
                            cardHTML = function (profileData) {
								
								var online = (profileData.online ? ('border-left: 4px solid green') : 'border-left: 4px solid red');
								
                                return '<div class="s-card s-card-pad">' +
                                        (profileData.avatar ? ('<img class="s-img" style="' + online + '" src=' + profileData.avatar + ' />') : '') +
                                        (profileData.realname ? ('<label class="s-name">' + profileData.realname + ' </label>') : ('<label class="s-name">' + profileData.name + ' </label>')) +
										(href ? ('(<a class="s-username" title="Visit full profile for ' + profileData.name + '" href="' + href + '">' + profileData.name + '</a>)<br/>') : '') +
										(profileData.location ? ('<label class="s-loc">' + profileData.location + '</label><br />') : '') +
                                        (profileData.rank ? ('<label class="s-loc">' + profileData.rank + '</label>') : '') +
										(profileData.interests ? ('<p class="s-desc">' + profileData.interests + '</p>') : '') +
										(profileData.joined ? ('<p class="s-desc"><span class="s-strong">Member since:</span><br/>' + profileData.joined + '</p>') : '') +
                                        (profileData.homepage ? ('<a class="s-href" href="' + profileData.homepage + '">' + profileData.homepage + '</a><br/>') : '') +

										'<ul class="s-stats">' +
                                        (profileData.posts ? ('<li>Posts<br /><span class="s-count">' + profileData.posts + '</span></li>') : '') +
                                        (profileData.points ? ('<li>Reputation<br /><span class="s-count">' + profileData.points + '</span></li>') : '') +
                                        '</ul>' +
										
										(profileData.actionButtons ? ('<span class="s-action">'+ profileData.actionButtons + '</span>') : '') +

                                        '</div>';
                            };
                            loadingHTML = options.loadingHTML;
                            errorHTML = options.errorHTML;
                            customCallback = function () { };
                        }
                        break;
                    default: { } break;
                }

                if ($.isEmptyObject(customCardJSON)) {
                    $.ajax({
                        url: urlToRequest,
                        type: 'GET',
                        dataType: dataType, //jsonp for cross domain request
                        timeout: 6000, //timeout if cross domain request didn't respond, or failed silently
                       // crossDomain: true,
                        cache: true,
                        beforeSend: function () {
                            curHCDetails.find('.s-message').remove();
                            curHCDetails.append('<p class="s-message">' + loadingHTML + '</p>');
                        },
                        success: function (data) {
                            if (data.length <= 0) {
								
                                curHCDetails.find('.s-message').html(errorHTML);
                            }
                            else {
								curHCDetails.find('.s-message').replaceWith(cardHTML(data));
                                //curHCDetails.prepend(cardHTML(data));
								
								$(".hc-details").hide();
								
                                adjustToViewPort(curHCDetails.closest('.hc-preview'));
                                curHCDetails.stop(true, true).delay(options.delay).fadeIn();
                                customCallback(data);
                            }
                        },
                        error: function (jqXHR, textStatus, errorThrown) {
							curHCDetails.find('.s-message').html(errorHTML + errorThrown);
                        }
                    });
                }
                else {
                    curHCDetails.prepend(cardHTML(customCardJSON));
                }
            };
        });

    };
})(jQuery);