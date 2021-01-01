//Title: Hovercard plugin by PC
//Documentation: http://designwithpc.com/Plugins/Hovercard
//Author: PC
//Website: http://designwithpc.com
//Twitter: @chaudharyp

(function($) {
    $.fn.hovercard = function(options) {

        //Set defaults for the control
        var defaults = {
            openOnLeft: false,
            openOnTop: false,
            cardImgSrc: "",
            detailsHTML: "",
            loadingHTML: "Loading...",
            errorHTML: "Sorry, no data found.",
            pointsText: "",
            postsText: "",
            background: "#ffffff",
            delay: 0,
            autoAdjust: true,
            onHoverIn: function() {
            },
            onHoverOut: function() {
            }
        };

        //Update unset options with defaults if needed
        options = $.extend(defaults, options); //Executing functionality on all selected elements
        return this.each(function() {
            var obj = $(this).eq(0);

            //wrap a parent span to the selected element
            obj.wrap('<div class="hc-preview" />');

            //add a relatively positioned class to the selected element
            obj.addClass("hc-name");

            //if card image src provided then generate the image element
            var hcImg = "";
            if (options.cardImgSrc.length > 0) {
                hcImg = '<img class="hc-pic" src="' + options.cardImgSrc + '" />';
            }

            //generate details span with html provided by the user
            var hcDetails = '<div class="hc-details" >' + hcImg + options.detailsHTML + "</div>";

            //append this detail after the selected element
            obj.after(hcDetails);
            obj.siblings(".hc-details").eq(0).css({ 'background': options.background });

            //toggle hover card details on hover
            obj.closest(".hc-preview").hoverIntent(function() {

                var $this = $(this);
                adjustToViewPort($this);

                // Up the z index for the .hc-name to overlay on .hc-details
                obj.css("zIndex", "200");

                var curHCDetails = $this.find(".hc-details").eq(0);
                curHCDetails.stop(true, true).delay(options.delay).fadeIn();

                // Default functionality on hover in, and also allows callback
                if (typeof options.onHoverIn == "function") {

                    //check for custom profile. If already loaded don't load again
                    var dataUrl;

                    //check for yaf profile. If already loaded don't load again
                    if (curHCDetails.find(".s-card").length <= 0) {

                        //Read data-hovercard url from the hovered element, otherwise look in the options. For custom card, complete url is required than just username.
                        dataUrl = options.customDataUrl;
                        if (typeof obj.attr("data-hovercard") == "undefined") {
                            //do nothing. detecting typeof obj.attr('data-hovercard') != 'undefined' didn't work as expected.
                        } else if (obj.attr("data-hovercard").length > 0) {
                            dataUrl = obj.attr("data-hovercard");
                        }

                        LoadSocialProfile("yaf", "", dataUrl, curHCDetails, options.customCardJSON);
                    }

                    $("body").on("keydown", function (event) {
                        if (event.keyCode === 27) {
                            closeHoverCard($this);
                        }
                    });

					var closeButton = curHCDetails.find(".s-close").eq(0);

					closeButton.click(function() {
						closeHoverCard($this);
					});

                    //Callback function
                    options.onHoverIn.call(this);
                }

            }, function () {
                 closeHoverCard($(this));
            });

            function closeHoverCard(card) {
                card.find(".hc-details").eq(0).stop(true, true).fadeOut(300, function () {

                    //Undo the z indices
                    obj.css("zIndex", "50");

                    if (typeof options.onHoverOut == "function") {
                        options.onHoverOut.call(this);
                    }
                });

                $("body").off("keydown");
            }

            //Opening Directions adjustment

            function adjustToViewPort(hcPreview) {

                var hcDetails = hcPreview.find(".hc-details").eq(0);
                var hcPreviewRect = hcPreview[0].getBoundingClientRect();

                var hcdRight = hcPreviewRect.left + 35 + hcDetails.width(); //Adding 35px of padding;
                var hcdBottom = hcPreviewRect.top + 35 + hcDetails.height(); //Adding 35px of padding;
                
                //Check for forced open directions, or if need to be auto adjusted
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
                var cardHTML, dataType, urlToRequest, customCallback, loadingHTML, errorHTML;

                switch (type) {
                case "yaf":
                    {
                        dataType = "json",
                        urlToRequest = username,
                        cardHTML = function(profileData) {

                            var online = profileData.Online ? "green" : "red";
                            var shtml = '<div class="s-card s-card-pad">' +
                                            '<div class="card rounded-0" style="width: 330px;">' +
                                                '<div class="card-header position-relative">' +
                                                    '<h6 class="card-title text-center">' + (profileData.RealName ? profileData.RealName : profileData.Name) + "</h6>" +
                                                    (profileData.Avatar ? '<img src="' + profileData.Avatar + '" class="rounded mx-auto d-block" style="width:75px" alt="" />' : "") +
                                                    (profileData.Avatar ? '<div class="position-absolute" style="top:0;right:0;border-width: 0 25px 25px 0; border-style: solid; border-color: transparent ' + online + ';" ></div>' : "") +
                                                "</div>" +
                                            '<div class="card-body p-2">' +
                                                '<ul class="list-unstyled mt-1 mb-3">' +
                                (profileData.Location ? '<li class="px-2 py-1"><i class="fas fa-home me-1"></i>' + profileData.Location + "</li>" : "") +
                                (profileData.Rank ? '<li class="px-2 py-1"><i class="fas fa-graduation-cap me-1"></i>' + profileData.Rank + "</li>" : "") +
                                (profileData.Interests ? '<li class="px-2 py-1"><i class="fas fa-running me-1"></i>' + profileData.Interests + "</li>" : "") +
                                (profileData.Joined ? '<li class="px-2 py-1"><i class="fas fa-user-check me-1"></i>' + profileData.Joined + "</li>" : "") +
                                (profileData.HomePage ? '<li class="px-2 py-1"><i class="fas fa-globe me-1"></i><a href="' + profileData.HomePage + '" target="_blank">' + profileData.HomePage + "</a></li>" : "") +
                                            '<li class="px-2 py-1"><i class="far fa-comment me-1"></i>' + profileData.Posts + "</li>" +
                                            "</ul>" +
                                            "</div>" +
                                            "</div>" +
                                        "</div>";
                            return shtml;

                        };
                        loadingHTML = options.loadingHTML;
                        errorHTML = options.errorHTML;
                        customCallback = function() {
                        };

                        curHCDetails.append('<span class="s-action s-close"><a href="javascript:void(0)"><i class="fas fa-times fa-fw"></i></a></span>');
                    }
                    break;
                default:
                    break;
                }

                if ($.isEmptyObject(customCardJSON)) {
					$.ajax({
                        url: urlToRequest,
                        type: "GET",
                        dataType: dataType, //jsonp for cross domain request
                        timeout: 6000, //timeout if cross domain request didn't respond, or failed silently
                        // crossDomain: true,
                        cache: true,
                        beforeSend: function() {
                            curHCDetails.find(".s-message").remove();
                            curHCDetails.append('<p class="s-message">' + loadingHTML + "</p>");
                        },
                        success: function(data) {
                            if (data.length <= 0) {
                                curHCDetails.find(".s-message").html(errorHTML);
                            } else {
                                curHCDetails.find(".s-message").replaceWith(cardHTML(data));

                                $(".hc-details").hide();

                                adjustToViewPort(curHCDetails.closest(".hc-preview"));
                                curHCDetails.stop(true, true).delay(options.delay).fadeIn();
                                customCallback(data);
                            }
                        },
                        error: function (jqXHR, textStatus, errorThrown) {
                            curHCDetails.find(".s-message").html(errorHTML + errorThrown);
                        }
                    });
                } else {
                    curHCDetails.prepend(cardHTML(customCardJSON));
                }
            }
        });

    };
})(jQuery);

(function($) {
    $.fn.hoverIntent = function(handlerIn, handlerOut, selector) {

        // default configuration values
        var cfg = {
            interval: 100,
            sensitivity: 7,
            timeout: 0
        };

        if (typeof handlerIn === "object") {
            cfg = $.extend(cfg, handlerIn);
        } else if ($.isFunction(handlerOut)) {
            cfg = $.extend(cfg, { over: handlerIn, out: handlerOut, selector: selector });
        } else {
            cfg = $.extend(cfg, { over: handlerIn, out: handlerIn, selector: handlerOut });
        }

        // instantiate variables
        // cX, cY = current X and Y position of mouse, updated by mousemove event
        // pX, pY = previous X and Y position of mouse, set by mouseover and polling interval
        var cX, cY, pX, pY;

        // A private function for getting mouse position
        var track = function(ev) {
            cX = ev.pageX;
            cY = ev.pageY;
        };

        // A private function for comparing current and previous mouse position
        var compare = function(ev, ob) {
            ob.hoverIntent_t = clearTimeout(ob.hoverIntent_t);
            // compare mouse positions to see if they've crossed the threshold
            if ((Math.abs(pX - cX) + Math.abs(pY - cY)) < cfg.sensitivity) {
                $(ob).off("mousemove.hoverIntent", track);
                // set hoverIntent state to true (so mouseOut can be called)
                ob.hoverIntent_s = 1;
                return cfg.over.apply(ob, [ev]);
            } else {
                // set previous coordinates for next time
                pX = cX;
                pY = cY;
                // use self-calling timeout, guarantees intervals are spaced out properly (avoids JavaScript timer bugs)
                ob.hoverIntent_t = setTimeout(function() { compare(ev, ob); }, cfg.interval);
            }
        };

        // A private function for delaying the mouseOut function
        var delay = function(ev, ob) {
            ob.hoverIntent_t = clearTimeout(ob.hoverIntent_t);
            ob.hoverIntent_s = 0;
            return cfg.out.apply(ob, [ev]);
        };

        // A private function for handling mouse 'hovering'
        var handleHover = function(e) {
            // copy objects to be passed into t (required for event object to be passed in IE)
            var ev = jQuery.extend({}, e);
            var ob = this;

            // cancel hoverIntent timer if it exists
            if (ob.hoverIntent_t) {
                ob.hoverIntent_t = clearTimeout(ob.hoverIntent_t);
            }

            // if e.type == "mouseenter"
            if (e.type == "mouseenter") {
                // set "previous" X and Y position based on initial entry point
                pX = ev.pageX;
                pY = ev.pageY;
                // update "current" X and Y position based on mousemove
                $(ob).on("mousemove.hoverIntent", track);
                // start polling interval (self-calling timeout) to compare mouse coordinates over time
                if (ob.hoverIntent_s != 1) {
                    ob.hoverIntent_t = setTimeout(function() { compare(ev, ob); }, cfg.interval);
                }

                // else e.type == "mouseleave"
            } else {
                // unbind expensive mousemove event
                $(ob).off("mousemove.hoverIntent", track);
                // if hoverIntent state is true, then call the mouseOut function after the specified delay
                if (ob.hoverIntent_s == 1) {
                    ob.hoverIntent_t = setTimeout(function() { delay(ev, ob); }, cfg.timeout);
                }
            }
        };

        // listen for mouseenter and mouseleave
        return this.on({ 'mouseenter.hoverIntent': handleHover, 'mouseleave.hoverIntent': handleHover }, cfg.selector);
    };
})(jQuery);