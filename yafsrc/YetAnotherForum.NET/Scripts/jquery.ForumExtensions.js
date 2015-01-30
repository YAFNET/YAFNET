function DataPanel_ExpandCollapse(hd, cht, cha, st, tc, te) {
    if (document.getElementById(hd).style.display == '') {
        document.getElementById(hd).style.display = 'none';
        if (cht != '') {
            document.getElementById(cht).title = te;
        }
        if (document.getElementById(cha) != null) {
            document.getElementById(cha).innerHTML = te;
            document.getElementById(cha).title = te;
        }
        document.getElementById(st).value = 'true';
    } else {
        document.getElementById(hd).style.display = '';
        if (cht != '') {
            document.getElementById(cht).title = tc;
        }
        if (document.getElementById(cha) != null) {
            document.getElementById(cha).innerHTML = tc;
            document.getElementById(cha).title = tc;
        }
        document.getElementById(st).value = 'false';
    }
}

function DataPanel_ExpandCollapseImage(hd, cht, cha, st, ex, cl, tc, te) {
    var elImg = (document.getElementById(cha)).getElementsByTagName("img");
    if (document.getElementById(hd).style.display == '') {
        document.getElementById(hd).style.display = 'none';
        if (cht != '') {
            document.getElementById(cht).title = te;
        }
        elImg[0].src = ex;
        elImg[0].alt = te;
        elImg[0].title = te; // logan fix
        document.getElementById(st).value = 'true';
    } else {
        document.getElementById(hd).style.display = '';
        if (cht != '') {
            document.getElementById(cht).title = tc;
        }
        elImg[0].src = cl;
        elImg[0].alt = tc;
        elImg[0].title = te; // logan fix
        document.getElementById(st).value = 'false';
    }
}
/*
 * CeeBox 2.1.4 jQuery Plugin (minimized version)
 * Requires jQuery 1.3.2 and swfobject.jquery.js plugin to work
 * Code hosted on GitHub (http://github.com/catcubed/ceebox) Please visit there for version history information
 * By Colin Fahrion (http://www.catcubed.com)
 * Inspiration for ceebox comes from Thickbox (http://jquery.com/demo/thickbox/) and Videobox (http://videobox-lb.sourceforge.net/)
 * However, along the upgrade path ceebox has morphed a long way from those roots.
 * Copyright (c) 2009 Colin Fahrion
 * Licensed under the MIT License: http://www.opensource.org/licenses/mit-license.php
*/

(function (b) {
    function v(c, a, d) {
        l.vidRegex = function () {
            var f = "";
            b.each(b.fn.ceebox.videos, function (e, g) {
                if (g.siteRgx !== null && typeof g.siteRgx !== "string") {
                    e = String(g.siteRgx);
                    f = f + e.slice(1, e.length - 2) + "|";
                }
            });
            return new RegExp(f + "\\.swf$", "i");
        }();
        l.userAgent = navigator.userAgent;
		b(".cee_close").off()
		b("body").on("click",".cee_close", function () {
            b.fn.ceebox.closebox();
            return false;
        });
		
        d != false && b(c).each(function (f) {
            B(this, f, a, d);
        });
        b(c).on("click",null, function (f) {
			
            var e = b(f.target).closest("[href]"),
                g = e.data("ceebox");
            if (g) {
                var h = g.opts ? b.extend({}, a, g.opts) : a;
                b.fn.ceebox.overlay(h);
                if (g.type == "image") {
                    var i = new Image;
                    i.onload = function () {
                        var m = i.width,
                            j = i.height;
                        h.imageWidth = s(m, b.fn.ceebox.defaults.imageWidth);
                        h.imageHeight = s(j, b.fn.ceebox.defaults.imageHeight);
                        h.imageRatio = m / j;
                        b.fn.ceebox.popup(e, b.extend(h, {
                            type: g.type
                        }, {
                            gallery: g.gallery
                        }));
                    };
                    i.src = b(e).attr("href");
                } else b.fn.ceebox.popup(e, b.extend(h, {
                    type: g.type
                }, {
                    gallery: g.gallery
                }));
				
                return false;
            }
        });
    }
    function w(c) {
        var a = document.documentElement;
        c = c || 100;
        this.width = (window.innerWidth || self.innerWidth || a && a.clientWidth || document.body.clientWidth) - c;
        this.height = (window.innerHeight || self.innerHeight || a && a.clientHeight || document.body.clientHeight) - c;
        return this;
    }
    function y(c) {
        var a = "fixed",
            d = 0,
            f = z(c.borderWidth, /[0-9]+/g);
        if (!window.XMLHttpRequest) {
            b("#cee_HideSelect") === null && b("body").append("<iframe id='cee_HideSelect'></iframe>");
            a = "absolute";
            d = parseInt(document.documentElement && document.documentElement.scrollTop || document.body.scrollTop, 10);
        }
        this.mleft = parseInt(-1 * (c.width / 2 + Number(f[3])),
        10);
        this.mtop = parseInt(-1 * (c.height / 2 + Number(f[0])), 10) + d;
        this.position = a;
        return this;
    }
    function z(c, a) {
        c = c.match(a);
        a = [];
        var d = c.length;
        if (d > 1) {
            a[0] = c[0];
            a[1] = c[1];
            a[2] = d == 2 ? c[0] : c[2];
            a[3] = d == 4 ? c[3] : c[1];
        } else a = [c, c, c, c];
        return a;
    }
    function C() {
        document.onkeydown = function (c) {
            c = c || window.event;
            switch (c.keyCode || c.which) {
            case 13:
                return false;
            case 27:
                b.fn.ceebox.closebox();
                document.onkeydown = null;
                break;
            case 188:
            case 37:
                b("#cee_prev").trigger("click");
                break;
            case 190:
            case 39:
                b("#cee_next").trigger("click");
                break;
            default:
                break;
            }
            return true;
        };
    }
    function D(c, a, d) {
        function f(m, j) {
            var k, o = i[d.type].bgtop,
                p = o - 2E3;
            m == "prev" ? (k = [{
                left: 0
            }, "left"]) : (k = [{
                right: 0
            },
            x = "right"]);
            var n = function (q) {
                return b.extend({
                    zIndex: 105,
                    width: i[d.type].w + "px",
                    height: i[d.type].h + "px",
                    position: "absolute",
                    top: i[d.type].top,
                    backgroundPosition: k[1] + " " + q + "px"
                }, k[0]);
            };
            b("<a href='#'></a>").text(m).attr({
                id: "cee_" + m
            }).css(n(p)).hover(function () {
                b(this).css(n(o));
            }, function () {
                b(this).css(n(p));
            }).one("click", function (q) {
                q.preventDefault();
                (function (E, F, G) {
                    b("#cee_prev,#cee_next").unbind().click(function () {
                        return false;
                    });
                    document.onkeydown = null;
                    var u = b("#cee_box").children(),
                        H = u.length;
                    u.fadeOut(G, function () {
                        b(this).remove();
                        this == u[H - 1] && E.eq(F).trigger("click");
                    });
                })(a, j, d.fadeOut);
            }).appendTo("#cee_box");
        }
        var e = d.height,
            g = d.titleHeight,
            h = d.padding,
            i = {
                image: {
                    w: parseInt(d.width / 2, 10),
                    h: e - g - 2 * h,
                    top: h,
                    bgtop: (e - g - 2 * h) / 2
                },
                video: {
                    w: 60,
                    h: 80,
                    top: parseInt((e - g - 10 - 2 * h) / 2, 10),
                    bgtop: 24
                }
            };
        i.html = i.video;
        c.prevId >= 0 && f("prev", c.prevId);
        c.nextId && f("next",
        c.nextId);
        b("#cee_title").append("<div id='cee_count'>Item " + (c.gNum + 1) + " of " + c.gLen + "</div>");
    }
    function s(c, a) {
        return c && c < a || !a ? c : a;
    }
    function t(c) {
        return typeof c == "function";
    }
    function r(c) {
        var a = c.length;
        return a > 1 ? c[a - 1] : c;
    }
    b.ceebox = {
        version: "2.1.5"
    };
    b.fn.ceebox = function (c) {
        c = b.extend({
            selector: b(this).selector
        }, b.fn.ceebox.defaults, c);
        var a = this,
            d = b(this).selector;
        c.videoJSON ? b.getJSON(c.videoJSON, function (f) {
            b.extend(b.fn.ceebox.videos, f);
            v(a, c, d);
        }) : v(a, c, d);
        return this;
    };
    b.fn.ceebox.defaults = {
        html: true,
        image: true,
        video: true,
        modal: false,
        titles: true,
        htmlGallery: true,
        imageGallery: true,
        videoGallery: true,
        videoWidth: false,
        videoHeight: false,
        videoRatio: "16:9",
        htmlWidth: false,
        htmlHeight: false,
        htmlRatio: false,
        imageWidth: false,
        imageHeight: false,
        animSpeed: "normal",
        easing: "swing",
        fadeOut: 400,
        fadeIn: 400,
        overlayColor: "#000",
        overlayOpacity: 0.8,
        boxColor: "",
        textColor: "",
        borderColor: "",
        borderWidth: "3px",
        padding: 15,
        margin: 150,
        onload: null,
        unload: null,
        videoJSON: null,
        iPhoneRedirect: true
    };
    b.fn.ceebox.ratios = {
        "4:3": 1.333,
        "3:2": 1.5,
        "16:9": 1.778,
        "1:1": 1,
        square: 1
    };
    b.fn.ceebox.relMatch = {
        width: /(?:width:)([0-9]+)/i,
        height: /(?:height:)([0-9]+)/i,
        ratio: /(?:ratio:)([0-9\.:]+)/i,
        modal: /modal:true/i,
        nonmodal: /modal:false/i,
        videoSrc: /(?:videoSrc:)(http:[\/\-\._0-9a-zA-Z:]+)/i,
        videoId: /(?:videoId:)([\-\._0-9a-zA-Z:]+)/i
    };
    b.fn.ceebox.loader = "<div id='cee_load' style='z-index:99999;top:50%;left:50%;position:fixed'></div>";
    b.fn.ceebox.videos = {
        base: {
            param: {
                wmode: "transparent",
                allowFullScreen: "true",
                allowScriptAccess: "always"
            },
            flashvars: {
                autoplay: true
            }
        },
        facebook: {
            siteRgx: /facebook\.com\/video/i,
            idRgx: /(?:v=)([a-zA-Z0-9_]+)/i,
            src: "http://www.facebook.com/v/[id]"
        },
        youtube: {
            siteRgx: /youtube\.com\/watch/i,
            idRgx: /(?:v=)([a-zA-Z0-9_\-]+)/i,
            src: "http://www.youtube.com/v/[id]&hl=en&fs=1&autoplay=1"
        },
        metacafe: {
            siteRgx: /metacafe\.com\/watch/i,
            idRgx: /(?:watch\/)([a-zA-Z0-9_]+)/i,
            src: "http://www.metacafe.com/fplayer/[id]/.swf"
        },
        google: {
            siteRgx: /google\.com\/videoplay/i,
            idRgx: /(?:id=)([a-zA-Z0-9_\-]+)/i,
            src: "http://video.google.com/googleplayer.swf?docId=[id]&hl=en&fs=true",
            flashvars: {
                playerMode: "normal",
                fs: true
            }
        },
        spike: {
            siteRgx: /spike\.com\/video|ifilm\.com\/video/i,
            idRgx: /(?:\/)([0-9]+)/i,
            src: "http://www.spike.com/efp",
            flashvars: {
                flvbaseclip: "[id]"
            }
        },
        vimeo: {
            siteRgx: /vimeo\.com\/[0-9]+/i,
            idRgx: /(?:\.com\/)([a-zA-Z0-9_]+)/i,
            src: "http://www.vimeo.com/moogaloop.swf?clip_id=[id]&server=vimeo.com&show_title=1&show_byline=1&show_portrait=0&color=&fullscreen=1"
        },
        dailymotion: {
            siteRgx: /dailymotion\.com\/video/i,
            idRgx: /(?:video\/)([a-zA-Z0-9_]+)/i,
            src: "http://www.dailymotion.com/swf/[id]&related=0&autoplay=1"
        },
        cnn: {
            siteRgx: /cnn\.com\/video/i,
            idRgx: /(?:\?\/video\/)([a-zA-Z0-9_\/\.]+)/i,
            src: "http://i.cdn.turner.com/cnn/.element/apps/cvp/3.0/swf/cnn_416x234_embed.swf?context=embed&videoId=[id]",
            width: 416,
            height: 374
        }
    };
    b.fn.ceebox.overlay = function (c) {
        c = b.extend({
            width: 60,
            height: 30,
            type: "html"
        }, b.fn.ceebox.defaults, c);
        b("#cee_overlay").size() === 0 && b("<div id='cee_overlay'></div>").css({
            opacity: c.overlayOpacity,
            position: "absolute",
            top: 0,
            left: 0,
            backgroundColor: c.overlayColor,
            width: "100%",
            height: b(document).height(),
            zIndex: 9995
        }).appendTo(b("body"));
        if (b("#cee_box").size() === 0) {
            var a = y(c);
            a = {
                position: a.position,
                zIndex: 9999,
                top: "50%",
                left: "50%",
                height: c.height + "px",
                width: c.width + "px",
                marginLeft: a.mleft + "px",
                marginTop: a.mtop + "px",
                opacity: 0,
                borderWidth: c.borderWidth,
                borderColor: c.borderColor,
                backgroundColor: c.boxColor,
                color: c.textColor
            };
            b("<div id='cee_box'></div>").css(a).appendTo("body").animate({
                opacity: 1
            }, c.animSpeed, function () {
                b("#cee_overlay").addClass("cee_close");
            });
        }
        b("#cee_box").removeClass().addClass("cee_" + c.type);
        b("#cee_load").size() === 0 && b(b.fn.ceebox.loader).appendTo("body");
        b("#cee_load").show("fast").animate({
            opacity: 1
        }, "fast");
    };

    b.fn.ceebox.popup = function (c, a) {
        var d = w(a.margin);
        a = b.extend({
            width: d.width,
            height: d.height,
            modal: false,
            type: "html",
            onload: null
        }, b.fn.ceebox.defaults, a);
        var f;
        if (b(c).is("a,area,input") && (a.type == "html" || a.type == "image" || a.type == "video")) {
            if (a.gallery) f = b(a.selector).eq(a.gallery.parentId).find("a[href],area[href],input[href]");
            A[a.type].prototype = new I(c, a);
            d = new A[a.type];
            c = d.content;
            a.action = d.action;
            a.modal = d.modal;
            if (a.titles) {
                a.titleHeight = b(d.titlebox).contents().contents().wrap("<div></div>").parent().attr("id", "ceetitletest").css({
                    position: "absolute",
                    top: "-300px",
                    width: d.width + "px"
                }).appendTo("body").height();
                b("#ceetitletest").remove();
                a.titleHeight = a.titleHeight >= 10 ? a.titleHeight + 20 : 30;
            } else a.titleHeight = 0;
            a.width = d.width + 2 * a.padding;
            a.height = d.height + a.titleHeight + 2 * a.padding;
        }
        b.fn.ceebox.overlay(a);
        l.action = a.action;
        l.onload = a.onload;
        l.unload = a.unload;
        d = y(a);
        d = {
            marginLeft: d.mleft,
            marginTop: d.mtop,
            width: a.width + "px",
            height: a.height + "px",
            borderWidth: a.borderWidth
        };
        if (a.borderColor) {
            var e = z(a.borderColor, /#[1-90a-f]+/gi);
            d = b.extend(d, {
                borderTopColor: e[0],
                borderRightColor: e[1],
                borderBottomColor: e[2],
                borderLeftColor: e[3]
            });
        }
        d = a.textColor ? b.extend(d, {
            color: a.textColor
        }) : d;
        d = a.boxColor ? b.extend(d, {
            backgroundColor: a.boxColor
        }) : d;
        b("#cee_box").animate(d, a.animSpeed, a.easing, function () {
            var g = b(this).append(c).children().hide(),
                h = g.length,
                i = true;
            g.fadeIn(a.fadeIn,

            function () {
                if (b(this).is("#cee_iframeContent")) i = false;
                i && this == g[h - 1] && b.fn.ceebox.onload();
            });
            if (a.modal === true) b("#cee_overlay").removeClass("cee_close");
            else {
                b("<a href='#' id='cee_closeBtn' class='cee_close' title='Close'>close</a>").prependTo("#cee_box");
                a.gallery && D(a.gallery, f, a);
                C(void 0, f, a.fadeOut);
            }
        });
    };
    b.fn.ceebox.closebox = function (c, a) {
        c = c || 400;
        b("#cee_box").fadeOut(c);
        b("#cee_overlay").fadeOut(typeof c == "number" ? c * 2 : "slow", function () {
            b("#cee_box,#cee_overlay,#cee_HideSelect,#cee_load").unbind().trigger("unload").remove();
            if (t(a)) a();
            else t(l.unload) && l.unload();
            l.unload = null;
        });
        document.onkeydown = null;
    };
    b.fn.ceebox.onload = function () {
        b("#cee_load").hide(300).fadeOut(600, function () {
            b(this).remove();
        });
        if (t(l.action)) {
            l.action();
            l.action = null;
        }
        if (t(l.onload)) {
            l.onload();
            l.onload = null;
        }
    };
    var l = {}, B = function (c, a, d) {
        var f, e = [],
            g = [],
            h = 0;
        b(c).is("[href]") ? (f = b(c)) : (f = b(c).find("[href]"));
        var i = {
            image: function (j, k) {
                return k && k.match(/\bimage\b/i) ? true : j.match(/\.ashx|\.jpg$|\.jpeg$|\.png$|\.gif$|\.bmp$/i) || false;
            },
            video: function (j, k) {
                return k && k.match(/\bvideo\b/i) ? true : j.match(l.vidRegex) || false;
            },
            html: function () {
                return true;
            }
        };
        f.each(function (j) {
            var k = this,
                o = b.metadata ? b(k).metadata() : false,
                p = o ? b.extend({}, d, o) : d;
            b.each(i, function (n) {
                if (i[n](b(k).attr("href"), b(k).attr("rel")) && p[n]) {
                    var q = false;
                    if (p[n + "Gallery"] === true) {
                        g[g.length] = j;
                        q = true;
                    }
                    e[e.length] = {
                        linkObj: k,
                        type: n,
                        gallery: q,
                        linkOpts: p
                    };
                    return false;
                }
                return false;
            });
        });
        var m = g.length;
        b.each(e, function (j) {
            if (e[j].gallery) {
                var k = {
                    parentId: a,
                    gNum: h,
                    gLen: m
                };
                if (h > 0) k.prevId = g[h - 1];
                if (h < m - 1) k.nextId = g[h + 1];
                h++;
            }!b.support.opacity && b(c).is("map") && b(e[j].linkObj).click(function (o) {
                o.preventDefault();
            });
            b.data(e[j].linkObj, "ceebox", {
                type: e[j].type,
                opts: e[j].linkOpts,
                gallery: k
            });
        });
    }, I = function (c, a) {
        var d = a[a.type + "Width"],
            f = a[a.type + "Height"],
            e = a[a.type + "Ratio"] || d / f,
            g = b(c).attr("rel");
        if (g && g !== "") {
            var h = {};
            b.each(b.fn.ceebox.relMatch, function (m, j) {
                h[m] = j.exec(g);
            });
            if (h.modal) a.modal = true;
            if (h.nonmodal) a.modal = false;
            if (h.width) d = Number(r(h.width));
            if (h.height) f = Number(r(h.height));
            if (h.ratio) {
                e = r(h.ratio);
                e = Number(e) ? Number(e) : String(e);
            }
            if (h.videoSrc) this.videoSrc = String(r(h.videoSrc));
            if (h.videoId) this.videoId = String(r(h.videoId));
        }
        var i = w(a.margin);
        d = s(d, i.width);
        f = s(f, i.height);
        if (e) {
            Number(e) || (e = b.fn.ceebox.ratios[e] ? Number(b.fn.ceebox.ratios[e]) : 1);
            if (d / f > e) d = parseInt(f * e, 10);
            if (d / f < e) f = parseInt(d / e, 10);
        }
        this.modal = a.modal;
        this.href = b(c).attr("href");
        this.downloadhref = b(c).attr("date-img");
        this.title = b(c).attr("title") || c.t || "";
        this.downloadbox = this.downloadhref ? "<a style='padding-left:5px' href='" + this.downloadhref + "'>Download Image</a>" : "";
        this.titlebox = a.titles ? "<div id='cee_title'><h2>" + this.title + "</h2>" + this.downloadbox + "</div>" : "";
        this.width = d;
        this.height = f;
        this.rel = g;
        this.iPhoneRedirect = a.iPhoneRedirect;
    }, A = {
        image: function () {
            this.content = "<img id='cee_img' src='" + this.href + "' width='" + this.width + "' height='" + this.height + "' alt='" + this.title + "'/>" + this.titlebox;
        },
        video: function () {
            var c = "",
                a = this,
                d = function () {
                    var e = this,
                        g = a.videoId;
                    e.flashvars = e.param = {};
                    e.src = a.videoSrc || a.href;
                    e.width = a.width;
                    e.height = a.height;
                    b.each(b.fn.ceebox.videos, function (h, i) {
                        if (i.siteRgx && typeof i.siteRgx != "string" && i.siteRgx.test(a.href)) {
                            if (i.idRgx) {
                                i.idRgx = new RegExp(i.idRgx);
                                g = String(r(i.idRgx.exec(a.href)));
                            }
                            e.src = i.src ? i.src.replace("[id]", g) : e.src;
                            i.flashvars && b.each(i.flashvars, function (m, j) {
                                if (typeof j == "string") e.flashvars[m] = j.replace("[id]", g);
                            });
                            i.param && b.each(i.param, function (m, j) {
                                if (typeof j == "string") e.param[m] = j.replace("[id]", g);
                            });
                            e.width = i.width || e.width;
                            e.height = i.height || e.height;
                            e.site = h;
                        }
                    });
                    return e;
                }();
            if (b.flash.hasVersion(8)) {
                this.width = d.width;
                this.height = d.height;
                this.action = function () {
                    b("#cee_vid").flash({
                        swf: d.src,
                        params: b.extend(b.fn.ceebox.videos.base.param, d.param),
                        flashvars: b.extend(b.fn.ceebox.videos.base.flashvars,
                        d.flashvars),
                        width: d.width,
                        height: d.height
                    });
                };
            } else {
                this.width = 400;
                this.height = 200;
                if (l.userAgent.match(/iPhone/i) && this.iPhoneRedirect || l.userAgent.match(/iPod/i) && this.iPhoneRedirect) {
                    var f = this.href;
                    this.action = function () {
                        b.fn.ceebox.closebox(400, function () {
                            window.location = f;
                        });
                    };
                } else {
                    d.site = d.site || "SWF file";
                    c = "<p style='margin:20px'>Adobe Flash 8 or higher is required to view this movie. You can either:</p><ul><li>Follow link to <a href='" + this.href + "'>" + d.site + " </a></li><li>or <a href='http://www.adobe.com/products/flashplayer/'>Install Flash</a></li><li> or <a href='#' class='cee_close'>Close This Popup</a></li></ul>";
                }
            }
            this.content = "<div id='cee_vid' style='width:" + this.width + "px;height:" + this.height + "px;'>" + c + "</div>" + this.titlebox;
        },
        html: function () {
            var c = this.href,
                a = this.rel;
            a = [c.match(/[a-zA-Z0-9_\.]+\.[a-zA-Z]{2,4}/i), c.match(/^http:+/), a ? a.match(/^iframe/) : false];
            if (document.domain == a[0] && a[1] && !a[2] || !a[1] && !a[2]) {
                var d, f = (d = c.match(/#[a-zA-Z0-9_\-]+/)) ? String(c.split("#")[0] + " " + d) : c;
                this.action = function () {
                    b("#cee_ajax").load(f);
                };
                this.content = this.titlebox + "<div id='cee_ajax' style='width:" + (this.width - 30) + "px;height:" + (this.height - 20) + "px'></div>";
            } else {
                b("#cee_iframe").remove();
                this.content = this.titlebox + "<iframe frameborder='0' hspace='0' src='" + c + "' id='cee_iframeContent' name='cee_iframeContent" + Math.round(Math.random() * 1E3) + "' onload='jQuery.fn.ceebox.onload()' style='width:" + this.width + "px;height:" + this.height + "px;' > </iframe>";
            }
        }
    };
})(jQuery);


(function ($) {
    $.extend({
        metadata: {
            defaults: {
                type: 'class',
                name: 'metadata',
                cre: /({.*})/,
                single: 'metadata'
            },
            setType: function (type, name) {
                this.defaults.type = type;
                this.defaults.name = name;
            },
            get: function (elem, opts) {
                var settings = $.extend({}, this.defaults, opts);
                if (!settings.single.length) settings.single = 'metadata'; {
                    var data = $.data(elem, settings.single);
                }
                if (data) {
                    return data;
                }
                data = "{}";
                var getData = function (data) {
                    if (typeof data != "string") {
                        return data;
                    }
                    if (data.indexOf('{') < 0) {
                        data = eval("(" + data + ")");
                    }
                };
                var getObject = function (data) {
                    if (typeof data != "string") {
                        return data;
                    }
                    data = eval("(" + data + ")");
                    return data;
                };
                if (settings.type == "html5") {
                    var object = {};
                    $(elem.attributes).each(function () {
                        var name = this.nodeName;
                        if (name.match(/^data-/)) {
                            name = name.replace(/^data-/, '');
                        } else {
                            return true;
                        }
                        object[name] = getObject(this.nodeValue);
                    });
                } else {
                    if (settings.type == "class") {
                        var m = settings.cre.exec(elem.className);
                        if (m) {
                            data = m[1];
                        }
                    } else if (settings.type == "elem") {
                        if (!elem.getElementsByTagName) {
                            return;
                        }
                        var e = elem.getElementsByTagName(settings.name);
                        if (e.length) {
                            data = $.trim(e[0].innerHTML);
                        }
                    } else if (elem.getAttribute != undefined) {
                        var attr = elem.getAttribute(settings.name);
                        if (attr) {
                            data = attr;
                        }
                    }
                    object = getObject(data.indexOf("{") < 0 ? "{" + data + "}" : data);
                }
                $.data(elem, settings.single, object);
                return object;
            }
        }
    });
    $.fn.metadata = function (opts) {
        return $.metadata.get(this[0], opts);
    };
})(jQuery);
window.$ = window.$ || {};(function() { if ($ && $.fn && $.fn.select2 && $.fn.select2.amd) { define = $.fn.select2.amd.define; require = $.fn.select2.amd.require; }/**
 * @license almond 0.2.9 Copyright (c) 2011-2014, The Dojo Foundation All Rights Reserved.
 * Available via the MIT or new BSD license.
 * see: http://github.com/jrburke/almond for details
 */
//Going sloppy to avoid 'use strict' string cost, but strict practices should
//be followed.
/*jslint sloppy: true */
/*global setTimeout: false */

var requirejs, require, define;
(function (undef) {
    var main, req, makeMap, handlers,
        defined = {},
        waiting = {},
        config = {},
        defining = {},
        hasOwn = Object.prototype.hasOwnProperty,
        aps = [].slice,
        jsSuffixRegExp = /\.js$/;

    function hasProp(obj, prop) {
        return hasOwn.call(obj, prop);
    }

    /**
     * Given a relative module name, like ./something, normalize it to
     * a real name that can be mapped to a path.
     * @param {String} name the relative name
     * @param {String} baseName a real name that the name arg is relative
     * to.
     * @returns {String} normalized name
     */
    function normalize(name, baseName) {
        var nameParts, nameSegment, mapValue, foundMap, lastIndex,
            foundI, foundStarMap, starI, i, j, part,
            baseParts = baseName && baseName.split("/"),
            map = config.map,
            starMap = (map && map['*']) || {};

        //Adjust any relative paths.
        if (name && name.charAt(0) === ".") {
            //If have a base name, try to normalize against it,
            //otherwise, assume it is a top-level require that will
            //be relative to baseUrl in the end.
            if (baseName) {
                //Convert baseName to array, and lop off the last part,
                //so that . matches that "directory" and not name of the baseName's
                //module. For instance, baseName of "one/two/three", maps to
                //"one/two/three.js", but we want the directory, "one/two" for
                //this normalization.
                baseParts = baseParts.slice(0, baseParts.length - 1);
                name = name.split('/');
                lastIndex = name.length - 1;

                // Node .js allowance:
                if (config.nodeIdCompat && jsSuffixRegExp.test(name[lastIndex])) {
                    name[lastIndex] = name[lastIndex].replace(jsSuffixRegExp, '');
                }

                name = baseParts.concat(name);

                //start trimDots
                for (i = 0; i < name.length; i += 1) {
                    part = name[i];
                    if (part === ".") {
                        name.splice(i, 1);
                        i -= 1;
                    } else if (part === "..") {
                        if (i === 1 && (name[2] === '..' || name[0] === '..')) {
                            //End of the line. Keep at least one non-dot
                            //path segment at the front so it can be mapped
                            //correctly to disk. Otherwise, there is likely
                            //no path mapping for a path starting with '..'.
                            //This can still fail, but catches the most reasonable
                            //uses of ..
                            break;
                        } else if (i > 0) {
                            name.splice(i - 1, 2);
                            i -= 2;
                        }
                    }
                }
                //end trimDots

                name = name.join("/");
            } else if (name.indexOf('./') === 0) {
                // No baseName, so this is ID is resolved relative
                // to baseUrl, pull off the leading dot.
                name = name.substring(2);
            }
        }

        //Apply map config if available.
        if ((baseParts || starMap) && map) {
            nameParts = name.split('/');

            for (i = nameParts.length; i > 0; i -= 1) {
                nameSegment = nameParts.slice(0, i).join("/");

                if (baseParts) {
                    //Find the longest baseName segment match in the config.
                    //So, do joins on the biggest to smallest lengths of baseParts.
                    for (j = baseParts.length; j > 0; j -= 1) {
                        mapValue = map[baseParts.slice(0, j).join('/')];

                        //baseName segment has  config, find if it has one for
                        //this name.
                        if (mapValue) {
                            mapValue = mapValue[nameSegment];
                            if (mapValue) {
                                //Match, update name to the new value.
                                foundMap = mapValue;
                                foundI = i;
                                break;
                            }
                        }
                    }
                }

                if (foundMap) {
                    break;
                }

                //Check for a star map match, but just hold on to it,
                //if there is a shorter segment match later in a matching
                //config, then favor over this star map.
                if (!foundStarMap && starMap && starMap[nameSegment]) {
                    foundStarMap = starMap[nameSegment];
                    starI = i;
                }
            }

            if (!foundMap && foundStarMap) {
                foundMap = foundStarMap;
                foundI = starI;
            }

            if (foundMap) {
                nameParts.splice(0, foundI, foundMap);
                name = nameParts.join('/');
            }
        }

        return name;
    }

    function makeRequire(relName, forceSync) {
        return function () {
            //A version of a require function that passes a moduleName
            //value for items that may need to
            //look up paths relative to the moduleName
            return req.apply(undef, aps.call(arguments, 0).concat([relName, forceSync]));
        };
    }

    function makeNormalize(relName) {
        return function (name) {
            return normalize(name, relName);
        };
    }

    function makeLoad(depName) {
        return function (value) {
            defined[depName] = value;
        };
    }

    function callDep(name) {
        if (hasProp(waiting, name)) {
            var args = waiting[name];
            delete waiting[name];
            defining[name] = true;
            main.apply(undef, args);
        }

        if (!hasProp(defined, name) && !hasProp(defining, name)) {
            throw new Error('No ' + name);
        }
        return defined[name];
    }

    //Turns a plugin!resource to [plugin, resource]
    //with the plugin being undefined if the name
    //did not have a plugin prefix.
    function splitPrefix(name) {
        var prefix,
            index = name ? name.indexOf('!') : -1;
        if (index > -1) {
            prefix = name.substring(0, index);
            name = name.substring(index + 1, name.length);
        }
        return [prefix, name];
    }

    /**
     * Makes a name map, normalizing the name, and using a plugin
     * for normalization if necessary. Grabs a ref to plugin
     * too, as an optimization.
     */
    makeMap = function (name, relName) {
        var plugin,
            parts = splitPrefix(name),
            prefix = parts[0];

        name = parts[1];

        if (prefix) {
            prefix = normalize(prefix, relName);
            plugin = callDep(prefix);
        }

        //Normalize according
        if (prefix) {
            if (plugin && plugin.normalize) {
                name = plugin.normalize(name, makeNormalize(relName));
            } else {
                name = normalize(name, relName);
            }
        } else {
            name = normalize(name, relName);
            parts = splitPrefix(name);
            prefix = parts[0];
            name = parts[1];
            if (prefix) {
                plugin = callDep(prefix);
            }
        }

        //Using ridiculous property names for space reasons
        return {
            f: prefix ? prefix + '!' + name : name, //fullName
            n: name,
            pr: prefix,
            p: plugin
        };
    };

    function makeConfig(name) {
        return function () {
            return (config && config.config && config.config[name]) || {};
        };
    }

    handlers = {
        require: function (name) {
            return makeRequire(name);
        },
        exports: function (name) {
            var e = defined[name];
            if (typeof e !== 'undefined') {
                return e;
            } else {
                return (defined[name] = {});
            }
        },
        module: function (name) {
            return {
                id: name,
                uri: '',
                exports: defined[name],
                config: makeConfig(name)
            };
        }
    };

    main = function (name, deps, callback, relName) {
        var cjsModule, depName, ret, map, i,
            args = [],
            callbackType = typeof callback,
            usingExports;

        //Use name if no relName
        relName = relName || name;

        //Call the callback to define the module, if necessary.
        if (callbackType === 'undefined' || callbackType === 'function') {
            //Pull out the defined dependencies and pass the ordered
            //values to the callback.
            //Default to [require, exports, module] if no deps
            deps = !deps.length && callback.length ? ['require', 'exports', 'module'] : deps;
            for (i = 0; i < deps.length; i += 1) {
                map = makeMap(deps[i], relName);
                depName = map.f;

                //Fast path CommonJS standard dependencies.
                if (depName === "require") {
                    args[i] = handlers.require(name);
                } else if (depName === "exports") {
                    //CommonJS module spec 1.1
                    args[i] = handlers.exports(name);
                    usingExports = true;
                } else if (depName === "module") {
                    //CommonJS module spec 1.1
                    cjsModule = args[i] = handlers.module(name);
                } else if (hasProp(defined, depName) ||
                           hasProp(waiting, depName) ||
                           hasProp(defining, depName)) {
                    args[i] = callDep(depName);
                } else if (map.p) {
                    map.p.load(map.n, makeRequire(relName, true), makeLoad(depName), {});
                    args[i] = defined[depName];
                } else {
                    throw new Error(name + ' missing ' + depName);
                }
            }

            ret = callback ? callback.apply(defined[name], args) : undefined;

            if (name) {
                //If setting exports via "module" is in play,
                //favor that over return value and exports. After that,
                //favor a non-undefined return value over exports use.
                if (cjsModule && cjsModule.exports !== undef &&
                        cjsModule.exports !== defined[name]) {
                    defined[name] = cjsModule.exports;
                } else if (ret !== undef || !usingExports) {
                    //Use the return value from the function.
                    defined[name] = ret;
                }
            }
        } else if (name) {
            //May just be an object definition for the module. Only
            //worry about defining if have a module name.
            defined[name] = callback;
        }
    };

    requirejs = require = req = function (deps, callback, relName, forceSync, alt) {
        if (typeof deps === "string") {
            if (handlers[deps]) {
                //callback in this case is really relName
                return handlers[deps](callback);
            }
            //Just return the module wanted. In this scenario, the
            //deps arg is the module name, and second arg (if passed)
            //is just the relName.
            //Normalize module name, if it contains . or ..
            return callDep(makeMap(deps, callback).f);
        } else if (!deps.splice) {
            //deps is a config object, not an array.
            config = deps;
            if (config.deps) {
                req(config.deps, config.callback);
            }
            if (!callback) {
                return;
            }

            if (callback.splice) {
                //callback is an array, which means it is a dependency list.
                //Adjust args if there are dependencies
                deps = callback;
                callback = relName;
                relName = null;
            } else {
                deps = undef;
            }
        }

        //Support require(['a'])
        callback = callback || function () {};

        //If relName is a function, it is an errback handler,
        //so remove it.
        if (typeof relName === 'function') {
            relName = forceSync;
            forceSync = alt;
        }

        //Simulate async callback;
        if (forceSync) {
            main(undef, deps, callback, relName);
        } else {
            //Using a non-zero value because of concern for what old browsers
            //do, and latest browsers "upgrade" to 4 if lower value is used:
            //http://www.whatwg.org/specs/web-apps/current-work/multipage/timers.html#dom-windowtimers-settimeout:
            //If want a value immediately, use require('id') instead -- something
            //that works in almond on the global level, but not guaranteed and
            //unlikely to work in other AMD implementations.
            setTimeout(function () {
                main(undef, deps, callback, relName);
            }, 4);
        }

        return req;
    };

    /**
     * Just drops the config on the floor, but returns req in case
     * the config return value is used.
     */
    req.config = function (cfg) {
        return req(cfg);
    };

    /**
     * Expose module registry for debugging and tooling
     */
    requirejs._defined = defined;

    define = function (name, deps, callback) {

        //This module may not have dependencies
        if (!deps.splice) {
            //deps is not an array, so probably means
            //an object literal or factory function for
            //the value. Adjust args.
            callback = deps;
            deps = [];
        }

        if (!hasProp(defined, name) && !hasProp(waiting, name)) {
            waiting[name] = [name, deps, callback];
        }
    };

    define.amd = {
        jQuery: true
    };
}());

define("almond", function(){});

define('jquery',[],function () {
    return jQuery;
});

define('select2/utils',[], function () {
  var Utils = {};

  Utils.Extend = function (ChildClass, SuperClass) {
    var __hasProp = {}.hasOwnProperty;

    function BaseConstructor () {
      this.constructor = ChildClass;
    }

    for (var key in SuperClass) {
      if (__hasProp.call(SuperClass, key)) {
        ChildClass[key] = SuperClass[key];
      }
    }

    BaseConstructor.prototype = SuperClass.prototype;
    ChildClass.prototype = new BaseConstructor();
    ChildClass.__super__ = SuperClass.prototype;

    return ChildClass;
  };

  function getMethods (theClass) {
    var proto = theClass.prototype;

    var methods = [];

    for (var methodName in proto) {
      var m = proto[methodName];

      if (typeof m !== 'function') {
        continue;
      }

      if (methodName === 'constructor') {
        continue;
      }

      methods.push(methodName);
    }

    return methods;
  }

  Utils.Decorate = function (SuperClass, DecoratorClass) {
    var decoratedMethods = getMethods(DecoratorClass);
    var superMethods = getMethods(SuperClass);

    function DecoratedClass () {
      var unshift = Array.prototype.unshift;

      var argCount = DecoratorClass.prototype.constructor.length;

      var calledConstructor = SuperClass.prototype.constructor;

      if (argCount > 0) {
        unshift.call(arguments, SuperClass.prototype.constructor);

        calledConstructor = DecoratorClass.prototype.constructor;
      }

      calledConstructor.apply(this, arguments);
    }

    DecoratorClass.displayName = SuperClass.displayName;

    function ctr () {
      this.constructor = DecoratedClass;
    }

    DecoratedClass.prototype = new ctr();

    for (var m = 0; m < superMethods.length; m++) {
        var superMethod = superMethods[m];

        DecoratedClass.prototype[superMethod] =
          SuperClass.prototype[superMethod];
    }

    var calledMethod = function (methodName) {
      // Stub out the original method if it's not decorating an actual method
      var originalMethod = function () {};

      if (methodName in DecoratedClass.prototype) {
        originalMethod = DecoratedClass.prototype[methodName];
      }

      var decoratedMethod = DecoratorClass.prototype[methodName];

      return function () {
        var unshift = Array.prototype.unshift;

        unshift.call(arguments, originalMethod);

        return decoratedMethod.apply(this, arguments);
      };
    };

    for (var d = 0; d < decoratedMethods.length; d++) {
      var decoratedMethod = decoratedMethods[d];

      DecoratedClass.prototype[decoratedMethod] = calledMethod(decoratedMethod);
    }

    return DecoratedClass;
  };

  var Observable = function () {
    this.listeners = {};
  };

  Observable.prototype.on = function (event, callback) {
    this.listeners = this.listeners || {};

    if (event in this.listeners) {
      this.listeners[event].push(callback);
    } else {
      this.listeners[event] = [callback];
    }
  };

  Observable.prototype.trigger = function (event) {
    var slice = Array.prototype.slice;

    this.listeners = this.listeners || {};

    if (event in this.listeners) {
      this.invoke(this.listeners[event], slice.call(arguments, 1));
    }

    if ('*' in this.listeners) {
      this.invoke(this.listeners['*'], arguments);
    }
  };

  Observable.prototype.invoke = function (listeners, params) {
    for (var i = 0, len = listeners.length; i < len; i++) {
      listeners[i].apply(this, params);
    }
  };

  Utils.Observable = Observable;

  Utils.generateChars = function (length) {
    var chars = '';

    for (var i = 0; i < length; i++) {
      var randomChar = Math.floor(Math.random() * 36);
      chars += randomChar.toString(36);
    }

    return chars;
  };

  Utils.bind = function (func, context) {
    return function () {
      func.apply(context, arguments);
    };
  };

  return Utils;
});

define('select2/results',[
  'jquery',
  './utils'
], function ($, Utils) {
  function Results ($element, options, dataAdapter) {
    this.$element = $element;
    this.data = dataAdapter;
    this.options = options;

    Results.__super__.constructor.call(this);
  }

  Utils.Extend(Results, Utils.Observable);

  Results.prototype.render = function () {
    var $results = $(
      '<ul class="select2-results__options" role="tree"></ul>'
    );

    if (this.options.get('multiple')) {
      $results.attr('aria-multiselectable', 'true');
    }

    this.$results = $results;

    return $results;
  };

  Results.prototype.clear = function () {
    this.$results.empty();
  };

  Results.prototype.displayMessage = function (params) {
    this.clear();
    this.hideLoading();

    var $message = $(
      '<li role="treeitem" class="select2-results__option"></li>'
    );

    var message = this.options.get('translations').get(params.message);

    $message.text(message(params.args));

    this.$results.append($message);
  };

  Results.prototype.append = function (data) {
    this.hideLoading();

    var $options = [];

    if (data.results == null || data.results.length === 0) {
      if (this.$results.children().length === 0) {
        this.trigger('results:message', {
          message: 'noResults'
        });
      }

      return;
    }

    data.results = this.sort(data.results);

    for (var d = 0; d < data.results.length; d++) {
      var item = data.results[d];

      var $option = this.option(item);

      $options.push($option);
    }

    this.$results.append($options);
  };

  Results.prototype.position = function ($results, $dropdown) {
    var $resultsContainer = $dropdown.find('.select2-results');
    $resultsContainer.append($results);
  };

  Results.prototype.sort = function (data) {
    var sorter = this.options.get('sorter');

    return sorter(data);
  };

  Results.prototype.setClasses = function () {
    var self = this;

    this.data.current(function (selected) {
      var selectedIds = $.map(selected, function (s) {
        return s.id.toString();
      });

      var $options = self.$results
        .find('.select2-results__option[aria-selected]');

      $options.each(function () {
        var $option = $(this);

        var item = $.data(this, 'data');

        if (item.id != null && selectedIds.indexOf(item.id.toString()) > -1) {
          $option.attr('aria-selected', 'true');
        } else {
          $option.attr('aria-selected', 'false');
        }
      });

      var $selected = $options.filter('[aria-selected=true]');

      // Check if there are any selected options
      if ($selected.length > 0) {
        // If there are selected options, highlight the first
        $selected.first().trigger('mouseenter');
      } else {
        // If there are no selected options, highlight the first option
        // in the dropdown
        $options.first().trigger('mouseenter');
      }
    });
  };

  Results.prototype.showLoading = function (params) {
    this.hideLoading();

    var loadingMore = this.options.get('translations').get('searching');

    var loading = {
      disabled: true,
      loading: true,
      text: loadingMore(params)
    };
    var $loading = this.option(loading);
    $loading.className += ' loading-results';

    this.$results.prepend($loading);
  };

  Results.prototype.hideLoading = function () {
    this.$results.find('.loading-results').remove();
  };

  Results.prototype.option = function (data) {
    var option = document.createElement('li');
    option.className = 'select2-results__option';

    var attrs = {
      'role': 'treeitem',
      'aria-selected': 'false'
    };

    if (data.disabled) {
      delete attrs['aria-selected'];
      attrs['aria-disabled'] = 'true';
    }

    if (data.id == null) {
      delete attrs['aria-selected'];
    }

    if (data._resultId != null) {
      option.id = data._resultId;
    }

    if (data.children) {
      attrs.role = 'group';
      attrs['aria-label'] = data.text;
      delete attrs['aria-selected'];
    }

    for (var attr in attrs) {
      var val = attrs[attr];

      option.setAttribute(attr, val);
    }

    if (data.children) {
      var $option = $(option);

      var label = document.createElement('strong');
      label.className = 'select2-results__group';

      var $label = $(label);
      this.template(data, label);

      var $children = [];

      for (var c = 0; c < data.children.length; c++) {
        var child = data.children[c];

        var $child = this.option(child);

        $children.push($child);
      }

      var $childrenContainer = $('<ul></ul>', {
        'class': 'select2-results__options select2-results__options--nested'
      });

      $childrenContainer.append($children);

      $option.append(label);
      $option.append($childrenContainer);
    } else {
      this.template(data, option);
    }

    $.data(option, 'data', data);

    return option;
  };

  Results.prototype.bind = function (container, $container) {
    var self = this;

    var id = container.id + '-results';

    this.$results.attr('id', id);

    container.on('results:all', function (params) {
      self.clear();
      self.append(params.data);

      if (container.isOpen()) {
        self.setClasses();
      }
    });

    container.on('results:append', function (params) {
      self.append(params.data);

      if (container.isOpen()) {
        self.setClasses();
      }
    });

    container.on('query', function (params) {
      self.showLoading(params);
    });

    container.on('select', function () {
      if (!container.isOpen()) {
        return;
      }

      self.setClasses();
    });

    container.on('unselect', function () {
      if (!container.isOpen()) {
        return;
      }

      self.setClasses();
    });

    container.on('open', function () {
      // When the dropdown is open, aria-expended="true"
      self.$results.attr('aria-expanded', 'true');
      self.$results.attr('aria-hidden', 'false');

      self.setClasses();
      self.ensureHighlightVisible();
    });

    container.on('close', function () {
      // When the dropdown is closed, aria-expended="false"
      self.$results.attr('aria-expanded', 'false');
      self.$results.attr('aria-hidden', 'true');
      self.$results.removeAttr('aria-activedescendant');
    });

    container.on('results:select', function () {
      var $highlighted = self.getHighlightedResults();

      if ($highlighted.length === 0) {
        return;
      }

      var data = $highlighted.data('data');

      if ($highlighted.attr('aria-selected') == 'true') {
        self.trigger('unselect', {
          data: data
        });
      } else {
        self.trigger('select', {
          data: data
        });
      }
    });

    container.on('results:previous', function () {
      var $highlighted = self.getHighlightedResults();

      var $options = self.$results.find('[aria-selected]');

      var currentIndex = $options.index($highlighted);

      // If we are already at te top, don't move further
      if (currentIndex === 0) {
        return;
      }

      var nextIndex = currentIndex - 1;

      // If none are highlighted, highlight the first
      if ($highlighted.length === 0) {
        nextIndex = 0;
      }

      var $next = $options.eq(nextIndex);

      $next.trigger('mouseenter');

      var currentOffset = self.$results.offset().top;
      var nextTop = $next.offset().top;
      var nextOffset = self.$results.scrollTop() + (nextTop - currentOffset);

      if (nextIndex === 0) {
        self.$results.scrollTop(0);
      } else if (nextTop - currentOffset < 0) {
        self.$results.scrollTop(nextOffset);
      }
    });

    container.on('results:next', function () {
      var $highlighted = self.getHighlightedResults();

      var $options = self.$results.find('[aria-selected]');

      var currentIndex = $options.index($highlighted);

      var nextIndex = currentIndex + 1;

      // If we are at the last option, stay there
      if (nextIndex >= $options.length) {
        return;
      }

      var $next = $options.eq(nextIndex);

      $next.trigger('mouseenter');

      var currentOffset = self.$results.offset().top +
        self.$results.outerHeight(false);
      var nextBottom = $next.offset().top + $next.outerHeight(false);
      var nextOffset = self.$results.scrollTop() + nextBottom - currentOffset;

      if (nextIndex === 0) {
        self.$results.scrollTop(0);
      } else if (nextBottom > currentOffset) {
        self.$results.scrollTop(nextOffset);
      }
    });

    container.on('results:focus', function (params) {
      params.element.addClass('select2-results__option--highlighted');
    });

    container.on('results:message', function (params) {
      self.displayMessage(params);
    });

    if ($.fn.mousewheel) {
      this.$results.on('mousewheel', function (e) {
        var top = self.$results.scrollTop();

        var bottom = (
          self.$results.get(0).scrollHeight -
          self.$results.scrollTop() +
          e.deltaY
        );

        var isAtTop = e.deltaY > 0 && top - e.deltaY <= 0;
        var isAtBottom = e.deltaY < 0 && bottom <= self.$results.height();

        if (isAtTop) {
          self.$results.scrollTop(0);

          e.preventDefault();
          e.stopPropagation();
        } else if (isAtBottom) {
          self.$results.scrollTop(
            self.$results.get(0).scrollHeight - self.$results.height()
          );

          e.preventDefault();
          e.stopPropagation();
        }
      });
    }

    this.$results.on('mouseup', '.select2-results__option[aria-selected]',
      function (evt) {
      var $this = $(this);

      var data = $this.data('data');

      if ($this.attr('aria-selected') === 'true') {
        self.trigger('unselect', {
          originalEvent: evt,
          data: data
        });

        return;
      }

      self.trigger('select', {
        originalEvent: evt,
        data: data
      });
    });

    this.$results.on('mouseenter', '.select2-results__option[aria-selected]',
      function (evt) {
      var data = $(this).data('data');

      self.getHighlightedResults()
          .removeClass('select2-results__option--highlighted');

      self.trigger('results:focus', {
        data: data,
        element: $(this)
      });
    });
  };

  Results.prototype.getHighlightedResults = function () {
    var $highlighted = this.$results
    .find('.select2-results__option--highlighted');

    return $highlighted;
  };

  Results.prototype.destroy = function () {
    this.$results.remove();
  };

  Results.prototype.ensureHighlightVisible = function () {
    var $highlighted = this.getHighlightedResults();

    if ($highlighted.length === 0) {
      return;
    }

    var $options = this.$results.find('[aria-selected]');

    var currentIndex = $options.index($highlighted);

    var currentOffset = this.$results.offset().top;
    var nextTop = $highlighted.offset().top;
    var nextOffset = this.$results.scrollTop() + (nextTop - currentOffset);

    var offsetDelta = nextTop - currentOffset;
    nextOffset -= $highlighted.outerHeight(false) * 2;

    if (currentIndex <= 2) {
      this.$results.scrollTop(0);
    } else if (offsetDelta > this.$results.outerHeight() || offsetDelta < 0) {
      this.$results.scrollTop(nextOffset);
    }
  };

  Results.prototype.template = function (result, container) {
    var template = this.options.get('templateResult');

    container.innerHTML = template(result);
  };

  return Results;
});

define('select2/keys',[

], function () {
  var KEYS = {
    BACKSPACE: 8,
    TAB: 9,
    ENTER: 13,
    SHIFT: 16,
    CTRL: 17,
    ALT: 18,
    ESC: 27,
    SPACE: 32,
    PAGE_UP: 33,
    PAGE_DOWN: 34,
    END: 35,
    HOME: 36,
    LEFT: 37,
    UP: 38,
    RIGHT: 39,
    DOWN: 40,
    DELETE: 46,

    isArrow: function (k) {
        k = k.which ? k.which : k;

        switch (k) {
        case KEY.LEFT:
        case KEY.RIGHT:
        case KEY.UP:
        case KEY.DOWN:
            return true;
        }

        return false;
    }
  };

  return KEYS;
});

define('select2/selection/base',[
  '../utils',
  '../keys'
], function (Utils, KEYS) {
  function BaseSelection ($element, options) {
    this.$element = $element;
    this.options = options;

    BaseSelection.__super__.constructor.call(this);
  }

  Utils.Extend(BaseSelection, Utils.Observable);

  BaseSelection.prototype.render = function () {
    throw new Error('The `render` method must be defined in child classes.');
  };

  BaseSelection.prototype.bind = function (container, $container) {
    var self = this;

    var id = container.id + '-container';
    var resultsId = container.id + '-results';

    this.container = container;

    this.$selection.attr('aria-owns', resultsId);

    this.$selection.on('keydown', function (evt) {
      self.trigger('keypress', evt);

      if (evt.which === KEYS.SPACE) {
        evt.preventDefault();
      }
    });

    container.on('results:focus', function (params) {
      self.$selection.attr('aria-activedescendant', params.data._resultId);
    });

    container.on('selection:update', function (params) {
      self.update(params.data);
    });

    container.on('open', function () {
      // When the dropdown is open, aria-expanded="true"
      self.$selection.attr('aria-expanded', 'true');

      self._attachCloseHandler(container);
    });

    container.on('close', function () {
      // When the dropdown is closed, aria-expanded="false"
      self.$selection.attr('aria-expanded', 'false');
      self.$selection.removeAttr('aria-activedescendant');

      self.$selection.focus();

      self._detachCloseHandler(container);
    });
  };

  BaseSelection.prototype._attachCloseHandler = function (container) {
    var self = this;

    $(document.body).on('mousedown.select2.' + container.id, function (e) {
      var $target = $(e.target);

      var $select = $target.closest('.select2');

      var $all = $('.select2.select2-container--open');

      $all.each(function () {
        var $this = $(this);

        if (this == $select[0]) {
          return;
        }

        var $element = $this.data('element');

        $element.select2('close');
      });
    });

    $(window).on('scroll.select2.' + container.id, function (e) {
      self.trigger('close');
    });
  };

  BaseSelection.prototype._detachCloseHandler = function (container) {
    $(document.body).off('mousedown.select2.' + container.id);
    $(window).off('scroll.select2.' + container.id);
  };

  BaseSelection.prototype.position = function ($selection, $container) {
    var $selectionContainer = $container.find('.selection');
    $selectionContainer.append($selection);
  };

  BaseSelection.prototype.destroy = function () {
    this._detachCloseHandler(this.container);
  };

  BaseSelection.prototype.update = function (data) {
    throw new Error('The `update` method must be defined in child classes.');
  };

  return BaseSelection;
});

define('select2/selection/single',[
  './base',
  '../utils',
  '../keys'
], function (BaseSelection, Utils, KEYS) {
  function SingleSelection () {
    SingleSelection.__super__.constructor.apply(this, arguments);
  }

  Utils.Extend(SingleSelection, BaseSelection);

  SingleSelection.prototype.render = function () {
    var $selection = $(
      '<span class="select2-selection select2-selection--single" tabindex="0"' +
        ' role="combobox" aria-autocomplete="list" aria-haspopup="true"' +
        ' aria-expanded="false">' +
        '<span class="select2-selection__rendered"></span>' +
        '<span class="select2-selection__arrow" role="presentation">' +
          '<b role="presentation"></b>' +
        '</span>' +
      '</span>'
    );

    $selection.attr('title', this.$element.attr('title'));

    this.$selection = $selection;

    return $selection;
  };

  SingleSelection.prototype.bind = function (container, $container) {
    var self = this;

    SingleSelection.__super__.bind.apply(this, arguments);

    var id = container.id + '-container';

    this.$selection.find('.select2-selection__rendered').attr('id', id);
    this.$selection.attr('aria-labelledby', id);

    this.$selection.on('mousedown', function (evt) {
      // Only respond to left clicks
      if (evt.which !== 1) {
        return;
      }

      self.trigger('toggle', {
        originalEvent: evt
      });
    });

    this.$selection.on('focus', function (evt) {
      // User focuses on the container
    });

    this.$selection.on('blur', function (evt) {
      // User exits the container
    });

    container.on('enable', function () {
      self.$selection.attr('tabindex', '0');
    });

    container.on('disable', function () {
      self.$selection.attr('tabindex', '-1');
    });

    container.on('selection:update', function (params) {
      self.update(params.data);
    });
  };

  SingleSelection.prototype.clear = function () {
    this.$selection.find('.select2-selection__rendered').empty();
  };

  SingleSelection.prototype.display = function (data) {
    var template = this.options.get('templateSelection');

    return template(data);
  };

  SingleSelection.prototype.selectionContainer = function () {
    return $('<span></span>');
  };

  SingleSelection.prototype.update = function (data) {
    if (data.length === 0) {
      this.clear();
      return;
    }

    var selection = data[0];

    var formatted = this.display(selection);

    this.$selection.find('.select2-selection__rendered').html(formatted);
  };

  return SingleSelection;
});

define('select2/selection/multiple',[
  './base',
  '../utils'
], function (BaseSelection, Utils) {
  function MultipleSelection ($element, options) {
    MultipleSelection.__super__.constructor.apply(this, arguments);
  }

  Utils.Extend(MultipleSelection, BaseSelection);

  MultipleSelection.prototype.render = function () {
    var $selection = $(
      '<span class="select2-selection select2-selection--multiple"' +
        ' tabindex="0" role="combobox" aria-autocomplete="list"' +
        ' aria-haspopup="true" aria-expanded="false">' +
        '<ul class="select2-selection__rendered"></ul>' +
      '</span>'
    );

    $selection.attr('title', this.$element.attr('title'));

    this.$selection = $selection;

    return $selection;
  };

  MultipleSelection.prototype.bind = function (container, $container) {
    var self = this;

    MultipleSelection.__super__.bind.apply(this, arguments);

    this.$selection.on('click', function (evt) {
      self.trigger('toggle', {
        originalEvent: evt
      });
    });

    this.$selection.on('click', '.select2-selection__choice__remove',
      function (evt) {
      var $remove = $(this);
      var $selection = $remove.parent();

      var data = $selection.data('data');

      self.trigger('unselect', {
        originalEvent: evt,
        data: data
      });
    });

    container.on('enable', function () {
      self.$selection.attr('tabindex', '0');
    });

    container.on('disable', function () {
      self.$selection.attr('tabindex', '-1');
    });
  };

  MultipleSelection.prototype.clear = function () {
    this.$selection.find('.select2-selection__rendered').empty();
  };

  MultipleSelection.prototype.display = function (data) {
    var template = this.options.get('templateSelection');

    return template(data);
  };

  MultipleSelection.prototype.selectionContainer = function () {
    var $container = $(
      '<li class="select2-selection__choice">' +
        '<span class="select2-selection__choice__remove" role="presentation">' +
          '&times;' +
        '</span>' +
      '</li>'
    );

    return $container;
  };

  MultipleSelection.prototype.update = function (data) {
    this.clear();

    if (data.length === 0) {
      return;
    }

    var $selections = [];

    for (var d = 0; d < data.length; d++) {
      var selection = data[d];

      var formatted = this.display(selection);
      var $selection = this.selectionContainer();

      $selection.append(formatted);
      $selection.data('data', selection);

      $selections.push($selection);
    }

    this.$selection.find('.select2-selection__rendered').append($selections);
  };

  return MultipleSelection;
});

define('select2/selection/placeholder',[
  '../utils'
], function (Utils) {
  function Placeholder (decorated, $element, options) {
    this.placeholder = this.normalizePlaceholder(options.get('placeholder'));

    decorated.call(this, $element, options);
  }

  Placeholder.prototype.normalizePlaceholder = function (_, placeholder) {
    if (typeof placeholder === 'string') {
      placeholder = {
        id: '',
        text: placeholder
      };
    }

    return placeholder;
  };

  Placeholder.prototype.createPlaceholder = function (decorated, placeholder) {
    var $placeholder = this.selectionContainer();

    $placeholder.html(this.display(placeholder));
    $placeholder.addClass('select2-selection__placeholder')
                .removeClass('select2-selection__choice');

    return $placeholder;
  };

  Placeholder.prototype.update = function (decorated, data) {
    var singlePlaceholder = (
      data.length == 1 && data[0].id != this.placeholder.id
    );
    var multipleSelections = data.length > 1;

    if (multipleSelections || singlePlaceholder) {
      return decorated.call(this, data);
    }

    this.clear();

    var $placeholder = this.createPlaceholder(this.placeholder);

    this.$selection.find('.select2-selection__rendered').append($placeholder);
  };

  return Placeholder;
});

define('select2/selection/allowClear',[

], function () {
  function AllowClear () { }

  AllowClear.prototype.bind = function (decorated, container, $container) {
    var self = this;

    decorated.call(this, container, $container);

    this.$selection.on('mousedown', '.select2-selection__clear',
      function (evt) {
        // Ignore the event if it is disabled
        if (self.options.get('disabled')) {
          return;
        }

        evt.stopPropagation();

        self.$element.val(self.placeholder.id).trigger('change');

        self.trigger('toggle');
    });
  };

  AllowClear.prototype.update = function (decorated, data) {
    decorated.call(this, data);

    if (this.$selection.find('.select2-selection__placeholder').length > 0 ||
        data.length === 0) {
      return;
    }

    var $remove = $(
      '<span class="select2-selection__clear">' +
        '&times;' +
      '</span>'
    );

    this.$selection.find('.select2-selection__rendered').append($remove);
  };

  return AllowClear;
});

define('select2/selection/search',[
  '../utils',
  '../keys'
], function (Utils, KEYS) {
  function Search (decorated, $element, options) {
    decorated.call(this, $element, options);
  }

  Search.prototype.render = function (decorated) {
    var $search = $(
      '<li class="select2-search select2-search--inline">' +
        '<input class="select2-search__field" type="search" tabindex="-1"' +
          ' role="textbox" />' +
      '</li>'
    );

    this.$searchContainer = $search;
    this.$search = $search.find('input');

    var $rendered = decorated.call(this);

    return $rendered;
  };

  Search.prototype.bind = function (decorated, container, $container) {
    var self = this;

    decorated.call(this, container, $container);

    container.on('open', function () {
      self.$search.attr('tabindex', 0);

      self.$search.focus();
    });

    container.on('close', function () {
      self.$search.attr('tabindex', -1);

      self.$search.val('');
    });

    container.on('enable', function () {
      self.$search.prop('disabled', false);
    });

    container.on('disable', function () {
      self.$search.prop('disabled', true);
    });

    this.$selection.on('keydown', '.select2-search--inline', function (evt) {
      evt.stopPropagation();

      self.trigger('keypress', evt);

      self._keyUpPrevented = evt.isDefaultPrevented();

      var key = evt.which;

      if (key === KEYS.BACKSPACE && self.$search.val() === '') {
        var $previousChoice = self.$searchContainer
          .prev('.select2-selection__choice');

        if ($previousChoice.length > 0) {
          var item = $previousChoice.data('data');

          self.searchRemoveChoice(item);
        }
      }
    });

    this.$selection.on('keyup', '.select2-search--inline', function (evt) {
      self.handleSearch(evt);
    });
  };

  Search.prototype.createPlaceholder = function (decorated, placeholder) {
    this.$search.attr('placeholder', placeholder.text);
  };

  Search.prototype.update = function (decorated, data) {
    this.$search.attr('placeholder', '');

    decorated.call(this, data);

    this.$selection.find('.select2-selection__rendered')
                   .append(this.$searchContainer);

    this.resizeSearch();
  };

  Search.prototype.handleSearch = function () {
    this.resizeSearch();

    if (!this._keyUpPrevented) {
      var input = this.$search.val();

      this.trigger('query', {
        term: input
      });
    }

    this._keyUpPrevented = false;
  };

  Search.prototype.searchRemoveChoice = function (decorated, item) {
    this.trigger('unselect', {
      data: item
    });

    this.trigger('open');

    this.$search.val(item.text + ' ');
  };

  Search.prototype.resizeSearch = function () {
    this.$search.css('width', '25px');

    var width = '';

    if (this.$search.attr('placeholder') !== '') {
      width = this.$selection.find('.select2-selection__rendered').innerWidth();
    } else {
      var minimumWidth = this.$search.val().length + 1;

      width = (minimumWidth * 0.75) + 'em';
    }

    this.$search.css('width', width);
  };

  return Search;
});

define('select2/selection/eventRelay',[
  'jquery'
], function ($) {
  function EventRelay () { }

  EventRelay.prototype.bind = function (decorated, container, $container) {
    var self = this;
    var relayEvents = [
      'open', 'opening',
      'close', 'closing',
      'select', 'selecting',
      'unselect', 'unselecting'
    ];

    var preventableEvents = ['opening', 'closing', 'selecting', 'unselecting'];

    decorated.call(this, container, $container);

    container.on('*', function (name, params) {
      // Ignore events that should not be relayed
      if (relayEvents.indexOf(name) === -1) {
        return;
      }

      // The parameters should always be an object
      params = params || {};

      // Generate the jQuery event for the Select2 event
      var evt = $.Event('select2:' + name, {
        params: params
      });

      self.$element.trigger(evt);

      // Only handle preventable events if it was one
      if (preventableEvents.indexOf(name) === -1) {
        return;
      }

      params.prevented = evt.isDefaultPrevented();
    });
  };

  return EventRelay;
});

define('select2/translation',[

], function () {
  function Translation (dict) {
    this.dict = dict || {};
  }

  Translation.prototype.all = function () {
    return this.dict;
  };

  Translation.prototype.get = function (key) {
    return this.dict[key];
  };

  Translation.prototype.extend = function (translation) {
    this.dict = $.extend({}, translation.all(), this.dict);
  };

  // Static functions

  Translation._cache = {};

  Translation.loadPath = function (path) {
    if (!(path in Translation._cache)) {
      var translations = require(path);

      Translation._cache[path] = translations;
    }

    return new Translation(Translation._cache[path]);
  };

  return Translation;
});

define('select2/diacritics',[

], function () {
  var diacritics = {
    '\u24B6': 'A',
    '\uFF21': 'A',
    '\u00C0': 'A',
    '\u00C1': 'A',
    '\u00C2': 'A',
    '\u1EA6': 'A',
    '\u1EA4': 'A',
    '\u1EAA': 'A',
    '\u1EA8': 'A',
    '\u00C3': 'A',
    '\u0100': 'A',
    '\u0102': 'A',
    '\u1EB0': 'A',
    '\u1EAE': 'A',
    '\u1EB4': 'A',
    '\u1EB2': 'A',
    '\u0226': 'A',
    '\u01E0': 'A',
    '\u00C4': 'A',
    '\u01DE': 'A',
    '\u1EA2': 'A',
    '\u00C5': 'A',
    '\u01FA': 'A',
    '\u01CD': 'A',
    '\u0200': 'A',
    '\u0202': 'A',
    '\u1EA0': 'A',
    '\u1EAC': 'A',
    '\u1EB6': 'A',
    '\u1E00': 'A',
    '\u0104': 'A',
    '\u023A': 'A',
    '\u2C6F': 'A',
    '\uA732': 'AA',
    '\u00C6': 'AE',
    '\u01FC': 'AE',
    '\u01E2': 'AE',
    '\uA734': 'AO',
    '\uA736': 'AU',
    '\uA738': 'AV',
    '\uA73A': 'AV',
    '\uA73C': 'AY',
    '\u24B7': 'B',
    '\uFF22': 'B',
    '\u1E02': 'B',
    '\u1E04': 'B',
    '\u1E06': 'B',
    '\u0243': 'B',
    '\u0182': 'B',
    '\u0181': 'B',
    '\u24B8': 'C',
    '\uFF23': 'C',
    '\u0106': 'C',
    '\u0108': 'C',
    '\u010A': 'C',
    '\u010C': 'C',
    '\u00C7': 'C',
    '\u1E08': 'C',
    '\u0187': 'C',
    '\u023B': 'C',
    '\uA73E': 'C',
    '\u24B9': 'D',
    '\uFF24': 'D',
    '\u1E0A': 'D',
    '\u010E': 'D',
    '\u1E0C': 'D',
    '\u1E10': 'D',
    '\u1E12': 'D',
    '\u1E0E': 'D',
    '\u0110': 'D',
    '\u018B': 'D',
    '\u018A': 'D',
    '\u0189': 'D',
    '\uA779': 'D',
    '\u01F1': 'DZ',
    '\u01C4': 'DZ',
    '\u01F2': 'Dz',
    '\u01C5': 'Dz',
    '\u24BA': 'E',
    '\uFF25': 'E',
    '\u00C8': 'E',
    '\u00C9': 'E',
    '\u00CA': 'E',
    '\u1EC0': 'E',
    '\u1EBE': 'E',
    '\u1EC4': 'E',
    '\u1EC2': 'E',
    '\u1EBC': 'E',
    '\u0112': 'E',
    '\u1E14': 'E',
    '\u1E16': 'E',
    '\u0114': 'E',
    '\u0116': 'E',
    '\u00CB': 'E',
    '\u1EBA': 'E',
    '\u011A': 'E',
    '\u0204': 'E',
    '\u0206': 'E',
    '\u1EB8': 'E',
    '\u1EC6': 'E',
    '\u0228': 'E',
    '\u1E1C': 'E',
    '\u0118': 'E',
    '\u1E18': 'E',
    '\u1E1A': 'E',
    '\u0190': 'E',
    '\u018E': 'E',
    '\u24BB': 'F',
    '\uFF26': 'F',
    '\u1E1E': 'F',
    '\u0191': 'F',
    '\uA77B': 'F',
    '\u24BC': 'G',
    '\uFF27': 'G',
    '\u01F4': 'G',
    '\u011C': 'G',
    '\u1E20': 'G',
    '\u011E': 'G',
    '\u0120': 'G',
    '\u01E6': 'G',
    '\u0122': 'G',
    '\u01E4': 'G',
    '\u0193': 'G',
    '\uA7A0': 'G',
    '\uA77D': 'G',
    '\uA77E': 'G',
    '\u24BD': 'H',
    '\uFF28': 'H',
    '\u0124': 'H',
    '\u1E22': 'H',
    '\u1E26': 'H',
    '\u021E': 'H',
    '\u1E24': 'H',
    '\u1E28': 'H',
    '\u1E2A': 'H',
    '\u0126': 'H',
    '\u2C67': 'H',
    '\u2C75': 'H',
    '\uA78D': 'H',
    '\u24BE': 'I',
    '\uFF29': 'I',
    '\u00CC': 'I',
    '\u00CD': 'I',
    '\u00CE': 'I',
    '\u0128': 'I',
    '\u012A': 'I',
    '\u012C': 'I',
    '\u0130': 'I',
    '\u00CF': 'I',
    '\u1E2E': 'I',
    '\u1EC8': 'I',
    '\u01CF': 'I',
    '\u0208': 'I',
    '\u020A': 'I',
    '\u1ECA': 'I',
    '\u012E': 'I',
    '\u1E2C': 'I',
    '\u0197': 'I',
    '\u24BF': 'J',
    '\uFF2A': 'J',
    '\u0134': 'J',
    '\u0248': 'J',
    '\u24C0': 'K',
    '\uFF2B': 'K',
    '\u1E30': 'K',
    '\u01E8': 'K',
    '\u1E32': 'K',
    '\u0136': 'K',
    '\u1E34': 'K',
    '\u0198': 'K',
    '\u2C69': 'K',
    '\uA740': 'K',
    '\uA742': 'K',
    '\uA744': 'K',
    '\uA7A2': 'K',
    '\u24C1': 'L',
    '\uFF2C': 'L',
    '\u013F': 'L',
    '\u0139': 'L',
    '\u013D': 'L',
    '\u1E36': 'L',
    '\u1E38': 'L',
    '\u013B': 'L',
    '\u1E3C': 'L',
    '\u1E3A': 'L',
    '\u0141': 'L',
    '\u023D': 'L',
    '\u2C62': 'L',
    '\u2C60': 'L',
    '\uA748': 'L',
    '\uA746': 'L',
    '\uA780': 'L',
    '\u01C7': 'LJ',
    '\u01C8': 'Lj',
    '\u24C2': 'M',
    '\uFF2D': 'M',
    '\u1E3E': 'M',
    '\u1E40': 'M',
    '\u1E42': 'M',
    '\u2C6E': 'M',
    '\u019C': 'M',
    '\u24C3': 'N',
    '\uFF2E': 'N',
    '\u01F8': 'N',
    '\u0143': 'N',
    '\u00D1': 'N',
    '\u1E44': 'N',
    '\u0147': 'N',
    '\u1E46': 'N',
    '\u0145': 'N',
    '\u1E4A': 'N',
    '\u1E48': 'N',
    '\u0220': 'N',
    '\u019D': 'N',
    '\uA790': 'N',
    '\uA7A4': 'N',
    '\u01CA': 'NJ',
    '\u01CB': 'Nj',
    '\u24C4': 'O',
    '\uFF2F': 'O',
    '\u00D2': 'O',
    '\u00D3': 'O',
    '\u00D4': 'O',
    '\u1ED2': 'O',
    '\u1ED0': 'O',
    '\u1ED6': 'O',
    '\u1ED4': 'O',
    '\u00D5': 'O',
    '\u1E4C': 'O',
    '\u022C': 'O',
    '\u1E4E': 'O',
    '\u014C': 'O',
    '\u1E50': 'O',
    '\u1E52': 'O',
    '\u014E': 'O',
    '\u022E': 'O',
    '\u0230': 'O',
    '\u00D6': 'O',
    '\u022A': 'O',
    '\u1ECE': 'O',
    '\u0150': 'O',
    '\u01D1': 'O',
    '\u020C': 'O',
    '\u020E': 'O',
    '\u01A0': 'O',
    '\u1EDC': 'O',
    '\u1EDA': 'O',
    '\u1EE0': 'O',
    '\u1EDE': 'O',
    '\u1EE2': 'O',
    '\u1ECC': 'O',
    '\u1ED8': 'O',
    '\u01EA': 'O',
    '\u01EC': 'O',
    '\u00D8': 'O',
    '\u01FE': 'O',
    '\u0186': 'O',
    '\u019F': 'O',
    '\uA74A': 'O',
    '\uA74C': 'O',
    '\u01A2': 'OI',
    '\uA74E': 'OO',
    '\u0222': 'OU',
    '\u24C5': 'P',
    '\uFF30': 'P',
    '\u1E54': 'P',
    '\u1E56': 'P',
    '\u01A4': 'P',
    '\u2C63': 'P',
    '\uA750': 'P',
    '\uA752': 'P',
    '\uA754': 'P',
    '\u24C6': 'Q',
    '\uFF31': 'Q',
    '\uA756': 'Q',
    '\uA758': 'Q',
    '\u024A': 'Q',
    '\u24C7': 'R',
    '\uFF32': 'R',
    '\u0154': 'R',
    '\u1E58': 'R',
    '\u0158': 'R',
    '\u0210': 'R',
    '\u0212': 'R',
    '\u1E5A': 'R',
    '\u1E5C': 'R',
    '\u0156': 'R',
    '\u1E5E': 'R',
    '\u024C': 'R',
    '\u2C64': 'R',
    '\uA75A': 'R',
    '\uA7A6': 'R',
    '\uA782': 'R',
    '\u24C8': 'S',
    '\uFF33': 'S',
    '\u1E9E': 'S',
    '\u015A': 'S',
    '\u1E64': 'S',
    '\u015C': 'S',
    '\u1E60': 'S',
    '\u0160': 'S',
    '\u1E66': 'S',
    '\u1E62': 'S',
    '\u1E68': 'S',
    '\u0218': 'S',
    '\u015E': 'S',
    '\u2C7E': 'S',
    '\uA7A8': 'S',
    '\uA784': 'S',
    '\u24C9': 'T',
    '\uFF34': 'T',
    '\u1E6A': 'T',
    '\u0164': 'T',
    '\u1E6C': 'T',
    '\u021A': 'T',
    '\u0162': 'T',
    '\u1E70': 'T',
    '\u1E6E': 'T',
    '\u0166': 'T',
    '\u01AC': 'T',
    '\u01AE': 'T',
    '\u023E': 'T',
    '\uA786': 'T',
    '\uA728': 'TZ',
    '\u24CA': 'U',
    '\uFF35': 'U',
    '\u00D9': 'U',
    '\u00DA': 'U',
    '\u00DB': 'U',
    '\u0168': 'U',
    '\u1E78': 'U',
    '\u016A': 'U',
    '\u1E7A': 'U',
    '\u016C': 'U',
    '\u00DC': 'U',
    '\u01DB': 'U',
    '\u01D7': 'U',
    '\u01D5': 'U',
    '\u01D9': 'U',
    '\u1EE6': 'U',
    '\u016E': 'U',
    '\u0170': 'U',
    '\u01D3': 'U',
    '\u0214': 'U',
    '\u0216': 'U',
    '\u01AF': 'U',
    '\u1EEA': 'U',
    '\u1EE8': 'U',
    '\u1EEE': 'U',
    '\u1EEC': 'U',
    '\u1EF0': 'U',
    '\u1EE4': 'U',
    '\u1E72': 'U',
    '\u0172': 'U',
    '\u1E76': 'U',
    '\u1E74': 'U',
    '\u0244': 'U',
    '\u24CB': 'V',
    '\uFF36': 'V',
    '\u1E7C': 'V',
    '\u1E7E': 'V',
    '\u01B2': 'V',
    '\uA75E': 'V',
    '\u0245': 'V',
    '\uA760': 'VY',
    '\u24CC': 'W',
    '\uFF37': 'W',
    '\u1E80': 'W',
    '\u1E82': 'W',
    '\u0174': 'W',
    '\u1E86': 'W',
    '\u1E84': 'W',
    '\u1E88': 'W',
    '\u2C72': 'W',
    '\u24CD': 'X',
    '\uFF38': 'X',
    '\u1E8A': 'X',
    '\u1E8C': 'X',
    '\u24CE': 'Y',
    '\uFF39': 'Y',
    '\u1EF2': 'Y',
    '\u00DD': 'Y',
    '\u0176': 'Y',
    '\u1EF8': 'Y',
    '\u0232': 'Y',
    '\u1E8E': 'Y',
    '\u0178': 'Y',
    '\u1EF6': 'Y',
    '\u1EF4': 'Y',
    '\u01B3': 'Y',
    '\u024E': 'Y',
    '\u1EFE': 'Y',
    '\u24CF': 'Z',
    '\uFF3A': 'Z',
    '\u0179': 'Z',
    '\u1E90': 'Z',
    '\u017B': 'Z',
    '\u017D': 'Z',
    '\u1E92': 'Z',
    '\u1E94': 'Z',
    '\u01B5': 'Z',
    '\u0224': 'Z',
    '\u2C7F': 'Z',
    '\u2C6B': 'Z',
    '\uA762': 'Z',
    '\u24D0': 'a',
    '\uFF41': 'a',
    '\u1E9A': 'a',
    '\u00E0': 'a',
    '\u00E1': 'a',
    '\u00E2': 'a',
    '\u1EA7': 'a',
    '\u1EA5': 'a',
    '\u1EAB': 'a',
    '\u1EA9': 'a',
    '\u00E3': 'a',
    '\u0101': 'a',
    '\u0103': 'a',
    '\u1EB1': 'a',
    '\u1EAF': 'a',
    '\u1EB5': 'a',
    '\u1EB3': 'a',
    '\u0227': 'a',
    '\u01E1': 'a',
    '\u00E4': 'a',
    '\u01DF': 'a',
    '\u1EA3': 'a',
    '\u00E5': 'a',
    '\u01FB': 'a',
    '\u01CE': 'a',
    '\u0201': 'a',
    '\u0203': 'a',
    '\u1EA1': 'a',
    '\u1EAD': 'a',
    '\u1EB7': 'a',
    '\u1E01': 'a',
    '\u0105': 'a',
    '\u2C65': 'a',
    '\u0250': 'a',
    '\uA733': 'aa',
    '\u00E6': 'ae',
    '\u01FD': 'ae',
    '\u01E3': 'ae',
    '\uA735': 'ao',
    '\uA737': 'au',
    '\uA739': 'av',
    '\uA73B': 'av',
    '\uA73D': 'ay',
    '\u24D1': 'b',
    '\uFF42': 'b',
    '\u1E03': 'b',
    '\u1E05': 'b',
    '\u1E07': 'b',
    '\u0180': 'b',
    '\u0183': 'b',
    '\u0253': 'b',
    '\u24D2': 'c',
    '\uFF43': 'c',
    '\u0107': 'c',
    '\u0109': 'c',
    '\u010B': 'c',
    '\u010D': 'c',
    '\u00E7': 'c',
    '\u1E09': 'c',
    '\u0188': 'c',
    '\u023C': 'c',
    '\uA73F': 'c',
    '\u2184': 'c',
    '\u24D3': 'd',
    '\uFF44': 'd',
    '\u1E0B': 'd',
    '\u010F': 'd',
    '\u1E0D': 'd',
    '\u1E11': 'd',
    '\u1E13': 'd',
    '\u1E0F': 'd',
    '\u0111': 'd',
    '\u018C': 'd',
    '\u0256': 'd',
    '\u0257': 'd',
    '\uA77A': 'd',
    '\u01F3': 'dz',
    '\u01C6': 'dz',
    '\u24D4': 'e',
    '\uFF45': 'e',
    '\u00E8': 'e',
    '\u00E9': 'e',
    '\u00EA': 'e',
    '\u1EC1': 'e',
    '\u1EBF': 'e',
    '\u1EC5': 'e',
    '\u1EC3': 'e',
    '\u1EBD': 'e',
    '\u0113': 'e',
    '\u1E15': 'e',
    '\u1E17': 'e',
    '\u0115': 'e',
    '\u0117': 'e',
    '\u00EB': 'e',
    '\u1EBB': 'e',
    '\u011B': 'e',
    '\u0205': 'e',
    '\u0207': 'e',
    '\u1EB9': 'e',
    '\u1EC7': 'e',
    '\u0229': 'e',
    '\u1E1D': 'e',
    '\u0119': 'e',
    '\u1E19': 'e',
    '\u1E1B': 'e',
    '\u0247': 'e',
    '\u025B': 'e',
    '\u01DD': 'e',
    '\u24D5': 'f',
    '\uFF46': 'f',
    '\u1E1F': 'f',
    '\u0192': 'f',
    '\uA77C': 'f',
    '\u24D6': 'g',
    '\uFF47': 'g',
    '\u01F5': 'g',
    '\u011D': 'g',
    '\u1E21': 'g',
    '\u011F': 'g',
    '\u0121': 'g',
    '\u01E7': 'g',
    '\u0123': 'g',
    '\u01E5': 'g',
    '\u0260': 'g',
    '\uA7A1': 'g',
    '\u1D79': 'g',
    '\uA77F': 'g',
    '\u24D7': 'h',
    '\uFF48': 'h',
    '\u0125': 'h',
    '\u1E23': 'h',
    '\u1E27': 'h',
    '\u021F': 'h',
    '\u1E25': 'h',
    '\u1E29': 'h',
    '\u1E2B': 'h',
    '\u1E96': 'h',
    '\u0127': 'h',
    '\u2C68': 'h',
    '\u2C76': 'h',
    '\u0265': 'h',
    '\u0195': 'hv',
    '\u24D8': 'i',
    '\uFF49': 'i',
    '\u00EC': 'i',
    '\u00ED': 'i',
    '\u00EE': 'i',
    '\u0129': 'i',
    '\u012B': 'i',
    '\u012D': 'i',
    '\u00EF': 'i',
    '\u1E2F': 'i',
    '\u1EC9': 'i',
    '\u01D0': 'i',
    '\u0209': 'i',
    '\u020B': 'i',
    '\u1ECB': 'i',
    '\u012F': 'i',
    '\u1E2D': 'i',
    '\u0268': 'i',
    '\u0131': 'i',
    '\u24D9': 'j',
    '\uFF4A': 'j',
    '\u0135': 'j',
    '\u01F0': 'j',
    '\u0249': 'j',
    '\u24DA': 'k',
    '\uFF4B': 'k',
    '\u1E31': 'k',
    '\u01E9': 'k',
    '\u1E33': 'k',
    '\u0137': 'k',
    '\u1E35': 'k',
    '\u0199': 'k',
    '\u2C6A': 'k',
    '\uA741': 'k',
    '\uA743': 'k',
    '\uA745': 'k',
    '\uA7A3': 'k',
    '\u24DB': 'l',
    '\uFF4C': 'l',
    '\u0140': 'l',
    '\u013A': 'l',
    '\u013E': 'l',
    '\u1E37': 'l',
    '\u1E39': 'l',
    '\u013C': 'l',
    '\u1E3D': 'l',
    '\u1E3B': 'l',
    '\u017F': 'l',
    '\u0142': 'l',
    '\u019A': 'l',
    '\u026B': 'l',
    '\u2C61': 'l',
    '\uA749': 'l',
    '\uA781': 'l',
    '\uA747': 'l',
    '\u01C9': 'lj',
    '\u24DC': 'm',
    '\uFF4D': 'm',
    '\u1E3F': 'm',
    '\u1E41': 'm',
    '\u1E43': 'm',
    '\u0271': 'm',
    '\u026F': 'm',
    '\u24DD': 'n',
    '\uFF4E': 'n',
    '\u01F9': 'n',
    '\u0144': 'n',
    '\u00F1': 'n',
    '\u1E45': 'n',
    '\u0148': 'n',
    '\u1E47': 'n',
    '\u0146': 'n',
    '\u1E4B': 'n',
    '\u1E49': 'n',
    '\u019E': 'n',
    '\u0272': 'n',
    '\u0149': 'n',
    '\uA791': 'n',
    '\uA7A5': 'n',
    '\u01CC': 'nj',
    '\u24DE': 'o',
    '\uFF4F': 'o',
    '\u00F2': 'o',
    '\u00F3': 'o',
    '\u00F4': 'o',
    '\u1ED3': 'o',
    '\u1ED1': 'o',
    '\u1ED7': 'o',
    '\u1ED5': 'o',
    '\u00F5': 'o',
    '\u1E4D': 'o',
    '\u022D': 'o',
    '\u1E4F': 'o',
    '\u014D': 'o',
    '\u1E51': 'o',
    '\u1E53': 'o',
    '\u014F': 'o',
    '\u022F': 'o',
    '\u0231': 'o',
    '\u00F6': 'o',
    '\u022B': 'o',
    '\u1ECF': 'o',
    '\u0151': 'o',
    '\u01D2': 'o',
    '\u020D': 'o',
    '\u020F': 'o',
    '\u01A1': 'o',
    '\u1EDD': 'o',
    '\u1EDB': 'o',
    '\u1EE1': 'o',
    '\u1EDF': 'o',
    '\u1EE3': 'o',
    '\u1ECD': 'o',
    '\u1ED9': 'o',
    '\u01EB': 'o',
    '\u01ED': 'o',
    '\u00F8': 'o',
    '\u01FF': 'o',
    '\u0254': 'o',
    '\uA74B': 'o',
    '\uA74D': 'o',
    '\u0275': 'o',
    '\u01A3': 'oi',
    '\u0223': 'ou',
    '\uA74F': 'oo',
    '\u24DF': 'p',
    '\uFF50': 'p',
    '\u1E55': 'p',
    '\u1E57': 'p',
    '\u01A5': 'p',
    '\u1D7D': 'p',
    '\uA751': 'p',
    '\uA753': 'p',
    '\uA755': 'p',
    '\u24E0': 'q',
    '\uFF51': 'q',
    '\u024B': 'q',
    '\uA757': 'q',
    '\uA759': 'q',
    '\u24E1': 'r',
    '\uFF52': 'r',
    '\u0155': 'r',
    '\u1E59': 'r',
    '\u0159': 'r',
    '\u0211': 'r',
    '\u0213': 'r',
    '\u1E5B': 'r',
    '\u1E5D': 'r',
    '\u0157': 'r',
    '\u1E5F': 'r',
    '\u024D': 'r',
    '\u027D': 'r',
    '\uA75B': 'r',
    '\uA7A7': 'r',
    '\uA783': 'r',
    '\u24E2': 's',
    '\uFF53': 's',
    '\u00DF': 's',
    '\u015B': 's',
    '\u1E65': 's',
    '\u015D': 's',
    '\u1E61': 's',
    '\u0161': 's',
    '\u1E67': 's',
    '\u1E63': 's',
    '\u1E69': 's',
    '\u0219': 's',
    '\u015F': 's',
    '\u023F': 's',
    '\uA7A9': 's',
    '\uA785': 's',
    '\u1E9B': 's',
    '\u24E3': 't',
    '\uFF54': 't',
    '\u1E6B': 't',
    '\u1E97': 't',
    '\u0165': 't',
    '\u1E6D': 't',
    '\u021B': 't',
    '\u0163': 't',
    '\u1E71': 't',
    '\u1E6F': 't',
    '\u0167': 't',
    '\u01AD': 't',
    '\u0288': 't',
    '\u2C66': 't',
    '\uA787': 't',
    '\uA729': 'tz',
    '\u24E4': 'u',
    '\uFF55': 'u',
    '\u00F9': 'u',
    '\u00FA': 'u',
    '\u00FB': 'u',
    '\u0169': 'u',
    '\u1E79': 'u',
    '\u016B': 'u',
    '\u1E7B': 'u',
    '\u016D': 'u',
    '\u00FC': 'u',
    '\u01DC': 'u',
    '\u01D8': 'u',
    '\u01D6': 'u',
    '\u01DA': 'u',
    '\u1EE7': 'u',
    '\u016F': 'u',
    '\u0171': 'u',
    '\u01D4': 'u',
    '\u0215': 'u',
    '\u0217': 'u',
    '\u01B0': 'u',
    '\u1EEB': 'u',
    '\u1EE9': 'u',
    '\u1EEF': 'u',
    '\u1EED': 'u',
    '\u1EF1': 'u',
    '\u1EE5': 'u',
    '\u1E73': 'u',
    '\u0173': 'u',
    '\u1E77': 'u',
    '\u1E75': 'u',
    '\u0289': 'u',
    '\u24E5': 'v',
    '\uFF56': 'v',
    '\u1E7D': 'v',
    '\u1E7F': 'v',
    '\u028B': 'v',
    '\uA75F': 'v',
    '\u028C': 'v',
    '\uA761': 'vy',
    '\u24E6': 'w',
    '\uFF57': 'w',
    '\u1E81': 'w',
    '\u1E83': 'w',
    '\u0175': 'w',
    '\u1E87': 'w',
    '\u1E85': 'w',
    '\u1E98': 'w',
    '\u1E89': 'w',
    '\u2C73': 'w',
    '\u24E7': 'x',
    '\uFF58': 'x',
    '\u1E8B': 'x',
    '\u1E8D': 'x',
    '\u24E8': 'y',
    '\uFF59': 'y',
    '\u1EF3': 'y',
    '\u00FD': 'y',
    '\u0177': 'y',
    '\u1EF9': 'y',
    '\u0233': 'y',
    '\u1E8F': 'y',
    '\u00FF': 'y',
    '\u1EF7': 'y',
    '\u1E99': 'y',
    '\u1EF5': 'y',
    '\u01B4': 'y',
    '\u024F': 'y',
    '\u1EFF': 'y',
    '\u24E9': 'z',
    '\uFF5A': 'z',
    '\u017A': 'z',
    '\u1E91': 'z',
    '\u017C': 'z',
    '\u017E': 'z',
    '\u1E93': 'z',
    '\u1E95': 'z',
    '\u01B6': 'z',
    '\u0225': 'z',
    '\u0240': 'z',
    '\u2C6C': 'z',
    '\uA763': 'z',
    '\u0386': '\u0391',
    '\u0388': '\u0395',
    '\u0389': '\u0397',
    '\u038A': '\u0399',
    '\u03AA': '\u0399',
    '\u038C': '\u039F',
    '\u038E': '\u03A5',
    '\u03AB': '\u03A5',
    '\u038F': '\u03A9',
    '\u03AC': '\u03B1',
    '\u03AD': '\u03B5',
    '\u03AE': '\u03B7',
    '\u03AF': '\u03B9',
    '\u03CA': '\u03B9',
    '\u0390': '\u03B9',
    '\u03CC': '\u03BF',
    '\u03CD': '\u03C5',
    '\u03CB': '\u03C5',
    '\u03B0': '\u03C5',
    '\u03C9': '\u03C9',
    '\u03C2': '\u03C3'
  };

  return diacritics;
});

define('select2/data/base',[
  '../utils'
], function (Utils) {
  function BaseAdapter ($element, options) {
    BaseAdapter.__super__.constructor.call(this);
  }

  Utils.Extend(BaseAdapter, Utils.Observable);

  BaseAdapter.prototype.current = function (callback) {
    throw new Error('The `current` method must be defined in child classes.');
  };

  BaseAdapter.prototype.query = function (params, callback) {
    throw new Error('The `query` method must be defined in child classes.');
  };

  BaseAdapter.prototype.bind = function (container, $container) {
    // Can be implemented in subclasses
  };

  BaseAdapter.prototype.destroy = function () {
    // Can be implemented in subclasses
  };

  BaseAdapter.prototype.generateResultId = function (container, data) {
    var id = container.id + '-result-';

    id += Utils.generateChars(4);

    if (data.id != null) {
      id += '-' + data.id.toString();
    } else {
      id += '-' + Utils.generateChars(4);
    }
    return id;
  };

  return BaseAdapter;
});

define('select2/data/select',[
  './base',
  '../utils',
  'jquery'
], function (BaseAdapter, Utils, $) {
  function SelectAdapter ($element, options) {
    this.$element = $element;
    this.options = options;

    SelectAdapter.__super__.constructor.call(this);
  }

  Utils.Extend(SelectAdapter, BaseAdapter);

  SelectAdapter.prototype.current = function (callback) {
    var data = [];
    var self = this;

    this.$element.find(':selected').each(function () {
      var $option = $(this);

      var option = self.item($option);

      data.push(option);
    });

    callback(data);
  };

  SelectAdapter.prototype.select = function (data) {
    var self = this;

    // If data.element is a DOM nose, use it instead
    if ($(data.element).is('option')) {
      data.element.selected = true;

      this.$element.trigger('change');

      return;
    }

    if (this.$element.prop('multiple')) {
      this.current(function (currentData) {
        var val = [];

        data = [data];
        data.push.apply(data, currentData);

        for (var d = 0; d < data.length; d++) {
          id = data[d].id;

          if (val.indexOf(id) === -1) {
            val.push(id);
          }
        }

        self.$element.val(val);
        self.$element.trigger('change');
      });
    } else {
      var val = data.id;

      this.$element.val(val);

      this.$element.trigger('change');
    }
  };

  SelectAdapter.prototype.unselect = function (data) {
    var self = this;

    if (!this.$element.prop('multiple')) {
      return;
    }

    if ($(data.element).is('option')) {
      data.element.selected = false;

      this.$element.trigger('change');

      return;
    }

    this.current(function (currentData) {
      var val = [];

      for (var d = 0; d < currentData.length; d++) {
        id = currentData[d].id;

        if (id !== data.id && val.indexOf(id) === -1) {
          val.push(id);
        }
      }

      self.$element.val(val);

      self.$element.trigger('change');
    });
  };

  SelectAdapter.prototype.bind = function (container, $container) {
    var self = this;

    this.container = container;

    container.on('select', function (params) {
      self.select(params.data);
    });

    container.on('unselect', function (params) {
      self.unselect(params.data);
    });
  };

  SelectAdapter.prototype.destroy = function () {
    // Remove anything added to child elements
    this.$element.find('*').each(function () {
      // Remove any custom data set by Select2
      $.removeData(this, 'data');
    });
  };

  SelectAdapter.prototype.query = function (params, callback) {
    var data = [];
    var self = this;

    var $options = this.$element.children();

    $options.each(function () {
      var $option = $(this);

      if (!$option.is('option') && !$option.is('optgroup')) {
        return;
      }

      var option = self.item($option);

      var matches = self.matches(params, option);

      if (matches !== null) {
        data.push(matches);
      }
    });

    callback({
      results: data
    });
  };

  SelectAdapter.prototype.option = function (data) {
    var option;

    if (data.children) {
      option = document.createElement('optgroup');
      option.label = data.text;
    } else {
      option = document.createElement('option');
      option.innerText = data.text;
    }

    if (data.id) {
      option.value = data.id;
    }

    if (data.disabled) {
      option.disabled = true;
    }

    if (data.selected) {
      option.selected = true;
    }

    var $option = $(option);

    var normalizedData = this._normalizeItem(data);
    normalizedData.element = option;

    // Override the option's data with the combined data
    $.data(option, 'data', normalizedData);

    return $option;
  };

  SelectAdapter.prototype.item = function ($option) {
    var data = {};

    data = $.data($option[0], 'data');

    if (data != null) {
      return data;
    }

    if ($option.is('option')) {
      data = {
        id: $option.val(),
        text: $option.html(),
        disabled: $option.prop('disabled'),
        selected: $option.prop('selected')
      };
    } else if ($option.is('optgroup')) {
      data = {
        text: $option.prop('label'),
        children: []
      };

      var $children = $option.children('option');
      var children = [];

      for (var c = 0; c < $children.length; c++) {
        var $child = $($children[c]);

        var child = this.item($child);

        children.push(child);
      }

      data.children = children;
    }

    data = this._normalizeItem(data);
    data.element = $option[0];

    $.data($option[0], 'data', data);

    return data;
  };

  SelectAdapter.prototype._normalizeItem = function (item) {
    if (!$.isPlainObject(item)) {
      item = {
        id: item,
        text: item
      };
    }

    item = $.extend({}, {
      text: ''
    }, item);

    var defaults = {
      selected: false,
      disabled: false
    };

    if (item.id != null) {
      item.id = item.id.toString();
    }

    if (item.text != null) {
      item.text = item.text.toString();
    }

    if (item._resultId == null && item.id && this.container != null) {
      item._resultId = this.generateResultId(this.container, item);
    }

    return $.extend({}, defaults, item);
  };

  SelectAdapter.prototype.matches = function (params, data) {
    var matcher = this.options.get('matcher');

    return matcher(params, data);
  };

  return SelectAdapter;
});

define('select2/data/array',[
  './select',
  '../utils',
  'jquery'
], function (SelectAdapter, Utils, $) {
  function ArrayAdapter ($element, options) {
    var data = options.get('data');

    ArrayAdapter.__super__.constructor.call(this, $element, options);

    $element.append(this.convertToOptions(data));
  }

  Utils.Extend(ArrayAdapter, SelectAdapter);

  ArrayAdapter.prototype.select = function (data) {
    var $option = this.$element.find('option[value="' + data.id + '"]');

    if ($option.length === 0) {
      $option = this.option(data);

      this.$element.append($option);
    }

    ArrayAdapter.__super__.select.call(this, data);
  };

  ArrayAdapter.prototype.convertToOptions = function (data) {
    var self = this;

    var $existing = this.$element.find('option');
    var existingIds = $existing.map(function () {
      return self.item($(this)).id;
    }).get();

    var $options = [];

    // Filter out all items except for the one passed in the argument
    function onlyItem (item) {
      return function () {
        return $(this).val() == item.id;
      };
    }

    for (var d = 0; d < data.length; d++) {
      var item = this._normalizeItem(data[d]);

      // Skip items which were pre-loaded, only merge the data
      if (existingIds.indexOf(item.id) >= 0) {
        var $existingOption = $existing.filter(onlyItem(item));

        var existingData = this.item($existingOption);
        var newData = $.extend(true, {}, existingData, item);

        var $newOption = this.option(existingData);

        $existingOption.replaceWith($newOption);

        continue;
      }

      var $option = this.option(item);

      if (item.children) {
        var $children = this.convertToOptions(item.children);

        $option.append($children);
      }

      $options.push($option);
    }

    return $options;
  };

  return ArrayAdapter;
});

define('select2/data/ajax',[
  './array',
  '../utils',
  'jquery'
], function (ArrayAdapter, Utils, $) {
  function AjaxAdapter ($element, options) {
    this.ajaxOptions = options.get('ajax');

    if (this.ajaxOptions.processResults != null) {
      this.processResults = this.ajaxOptions.processResults;
    }

    ArrayAdapter.__super__.constructor.call(this, $element, options);
  }

  Utils.Extend(AjaxAdapter, ArrayAdapter);

  AjaxAdapter.prototype.processResults = function (results) {
    return results;
  };

  AjaxAdapter.prototype.query = function (params, callback) {
    var matches = [];
    var self = this;

    if (this._request) {
      this._request.abort();
      this._request = null;
    }

    var options = $.extend({
      type: 'GET'
    }, this.ajaxOptions);

    if (typeof options.url === 'function') {
      options.url = options.url(params);
    }

    if (typeof options.data === 'function') {
      options.data = options.data(params);
    }

    function request () {
      var $request = $.ajax(options);

      $request.success(function (data) {
        var results = self.processResults(data, params);

        callback(results);
      });

      self._request = $request;
    }

    if (this.ajaxOptions.delay && params.term !== '') {
      if (this._queryTimeout) {
        window.clearTimeout(this._queryTimeout);
      }

      this._queryTimeout = window.setTimeout(request, this.ajaxOptions.delay);
    } else {
      request();
    }
  };

  return AjaxAdapter;
});

define('select2/data/tags',[

], function () {
  function Tags (decorated, $element, options) {
    var tags = options.get('tags');

    var createTag = options.get('createTag');

    if (createTag !== undefined) {
      this.createTag = createTag;
    }

    decorated.call(this, $element, options);

    if ($.isArray(tags)) {
      for (var t = 0; t < tags.length; t++) {
        var tag = tags[t];
        var item = this._normalizeItem(tag);

        var $option = this.option(item);

        this.$element.append($option);
      }
    }
  }

  Tags.prototype.query = function (decorated, params, callback) {
    var self = this;

    this._removeOldTags();

    if (params.term == null || params.term === '' || params.page != null) {
      decorated.call(this, params, callback);
      return;
    }

    function wrapper (obj, child) {
      var data = obj.results;

      for (var i = 0; i < data.length; i++) {
        var option = data[i];

        var checkChildren = (
          option.children != null &&
          !wrapper({
            results: option.children
          }, true)
        );

        var checkText = option.text === params.term;

        if (checkText || checkChildren) {
          if (child) {
            return false;
          }

          obj.data = data;
          callback(obj);

          return;
        }
      }

      if (child) {
        return true;
      }

      var tag = self.createTag(params);

      if (tag != null) {
        var $option = self.option(tag);
        $option.attr('data-select2-tag', true);

        self.$element.append($option);

        self.insertTag(data, tag);
      }

      obj.results = data;

      callback(obj);
    }

    decorated.call(this, params, wrapper);
  };

  Tags.prototype.createTag = function (decorated, params) {
    return {
      id: params.term,
      text: params.term
    };
  };

  Tags.prototype.insertTag = function (_, data, tag) {
    data.unshift(tag);
  };

  Tags.prototype._removeOldTags = function (_) {
    var tag = this._lastTag;

    var $options = this.$element.find('option[data-select2-tag]');

    $options.each(function () {
      if (this.selected) {
        return;
      }

      $(this).remove();
    });
  };

  return Tags;
});

define('select2/data/tokenizer',[

], function () {
  function Tokenizer (decorated, $element, options) {
    var tokenizer = options.get('tokenizer');

    if (tokenizer !== undefined) {
      this.tokenizer = tokenizer;
    }

    decorated.call(this, $element, options);
  }

  Tokenizer.prototype.bind = function (decorated, container, $container) {
    decorated.call(this, container, $container);

    this.$search =  container.dropdown.$search || container.selection.$search ||
      $container.find('.select2-search__field');
  };

  Tokenizer.prototype.query = function (decorated, params, callback) {
    var self = this;

    function select (data) {
      self.select(data);
    }

    params.term = params.term || '';

    var tokenData = this.tokenizer(params, this.options, select);

    if (tokenData.term !== params.term) {
      // Replace the search term if we have the search box
      if (this.$search.length) {
        this.$search.val(tokenData.term);
        this.$search.focus();
      }

      params.term = tokenData.term;
    }

    decorated.call(this, params, callback);
  };

  Tokenizer.prototype.tokenizer = function (_, params, options, callback) {
    var separators = options.get('tokenSeparators') || [];
    var term = params.term;
    var i = 0;

    var createTag = this.createTag || function (params) {
      return {
        id: params.term,
        text: params.term
      };
    };

    while (i < term.length) {
      var termChar = term[i];

      if (separators.indexOf(termChar) === -1) {
        i++;

        continue;
      }

      var part = term.substr(0, i);
      var partParams = $.extend({}, params, {
        term: part
      });

      var data = createTag(partParams);

      callback(data);

      // Reset the term to not include the tokenized portion
      term = term.substr(i + 1) || '';
      i = 0;
    }

    return {
      term: term
    };
  };

  return Tokenizer;
});

define('select2/data/minimumInputLength',[

], function () {
  function MinimumInputLength (decorated, $e, options) {
    this.minimumInputLength = options.get('minimumInputLength');

    decorated.call(this, $e, options);
  }

  MinimumInputLength.prototype.query = function (decorated, params, callback) {
    params.term = params.term || '';

    if (params.term.length < this.minimumInputLength) {
      this.trigger('results:message', {
        message: 'inputTooShort',
        args: {
          minimum: this.minimumInputLength,
          input: params.term,
          params: params
        }
      });

      return;
    }

    decorated.call(this, params, callback);
  };

  return MinimumInputLength;
});

define('select2/data/maximumInputLength',[

], function () {
  function MaximumInputLength (decorated, $e, options) {
    this.maximumInputLength = options.get('maximumInputLength');

    decorated.call(this, $e, options);
  }

  MaximumInputLength.prototype.query = function (decorated, params, callback) {
    params.term = params.term || '';

    if (this.maximumInputLength > 0 &&
        params.term.length > this.maximumInputLength) {
      this.trigger('results:message', {
        message: 'inputTooLong',
        args: {
          minimum: this.maximumInputLength,
          input: params.term,
          params: params
        }
      });

      return;
    }

    decorated.call(this, params, callback);
  };

  return MaximumInputLength;
});

define('select2/data/maximumSelectionLength',[

], function (){
  function MaximumSelectionLength (decorated, $e, options) {
    this.maximumSelectionLength = options.get('maximumSelectionLength');

    decorated.call(this, $e, options);
  }

  MaximumSelectionLength.prototype.query =
    function (decorated, params, callback) {
      var self = this;

      this.current(function (currentData) {
        var count = currentData != null ? currentData.length : 0;
        if (self.maximumSelectionLength > 0 &&
          count >= self.maximumSelectionLength) {
          self.trigger('results:message', {
            message: 'maximumSelected',
            args: {
              maximum: self.maximumSelectionLength
            }
          });
          return;
        }
        decorated.call(self, params, callback);
      });
  };

  return MaximumSelectionLength;
});

define('select2/dropdown',[
  './utils'
], function (Utils) {
  function Dropdown ($element, options) {
    this.$element = $element;
    this.options = options;

    Dropdown.__super__.constructor.call(this);
  }

  Utils.Extend(Dropdown, Utils.Observable);

  Dropdown.prototype.render = function () {
    var $dropdown = $(
      '<span class="select2-dropdown">' +
        '<span class="select2-results"></span>' +
      '</span>'
    );

    $dropdown.attr('dir', this.options.get('dir'));

    this.$dropdown = $dropdown;

    return $dropdown;
  };

  Dropdown.prototype.position = function ($dropdown, $container) {
    // Should be implmented in subclasses
  };

  Dropdown.prototype.destroy = function () {
    // Remove the dropdown from the DOM
    this.$dropdown.remove();
  };

  Dropdown.prototype.bind = function (container, $container) {
    var self = this;

    container.on('select', function (params) {
      self._onSelect(params);
    });

    container.on('unselect', function (params) {
      self._onUnSelect(params);
    });
  };

  Dropdown.prototype._onSelect = function () {
    this.trigger('close');
  };

  Dropdown.prototype._onUnSelect = function () {
    this.trigger('close');
  };

  return Dropdown;
});

define('select2/dropdown/search',[
  '../utils'
], function (Utils) {
  function Search () { }

  Search.prototype.render = function (decorated) {
    var $rendered = decorated.call(this);

    var $search = $(
      '<span class="select2-search select2-search--dropdown">' +
        '<input class="select2-search__field" type="search" tabindex="-1"' +
        ' role="textbox" />' +
      '</span>'
    );

    this.$searchContainer = $search;
    this.$search = $search.find('input');

    $rendered.prepend($search);

    return $rendered;
  };

  Search.prototype.bind = function (decorated, container, $container) {
    var self = this;

    decorated.call(this, container, $container);

    this.$search.on('keydown', function (evt) {
      self.trigger('keypress', evt);

      self._keyUpPrevented = evt.isDefaultPrevented();
    });

    this.$search.on('keyup', function (evt) {
      self.handleSearch(evt);
    });

    container.on('open', function () {
      self.$search.attr('tabindex', 0);

      self.$search.focus();

      window.setTimeout(function () {
        self.$search.focus();
      }, 0);
    });

    container.on('close', function () {
      self.$search.attr('tabindex', -1);

      self.$search.val('');
    });

    container.on('results:all', function (params) {
      if (params.query.term == null || params.query.term === '') {
        var showSearch = self.showSearch(params);

        if (showSearch) {
          self.$searchContainer.removeClass('select2-search--hide');
        } else {
          self.$searchContainer.addClass('select2-search--hide');
        }
      }
    });
  };

  Search.prototype.handleSearch = function (evt) {
    if (!this._keyUpPrevented) {
      var input = this.$search.val();

      this.trigger('query', {
        term: input
      });
    }

    this._keyUpPrevented = false;
  };

  Search.prototype.showSearch = function (_, params) {
    return true;
  };

  return Search;
});

define('select2/dropdown/hidePlaceholder',[

], function () {
  function HidePlaceholder (decorated, $element, options, dataAdapter) {
    this.placeholder = this.normalizePlaceholder(options.get('placeholder'));

    decorated.call(this, $element, options, dataAdapter);
  }

  HidePlaceholder.prototype.append = function (decorated, data) {
    data.results = this.removePlaceholder(data.results);

    decorated.call(this, data);
  };

  HidePlaceholder.prototype.normalizePlaceholder = function (_, placeholder) {
    if (typeof placeholder === 'string') {
      placeholder = {
        id: '',
        text: placeholder
      };
    }

    return placeholder;
  };

  HidePlaceholder.prototype.removePlaceholder = function (_, data) {
    var modifiedData = data.slice(0);

    for (var d = data.length - 1; d >= 0; d--) {
      var item = data[d];

      if (this.placeholder.id === item.id) {
        modifiedData.splice(d, 1);
      }
    }

    return modifiedData;
  };

  return HidePlaceholder;
});

define('select2/dropdown/infiniteScroll',[
  'jquery'
], function ($) {
  function InfiniteScroll (decorated, $element, options, dataAdapter) {
    this.lastParams = {};

    decorated.call(this, $element, options, dataAdapter);

    this.$loadingMore = this.createLoadingMore();
    this.loading = false;
  }

  InfiniteScroll.prototype.append = function (decorated, data) {
    this.$loadingMore.remove();
    this.loading = false;

    decorated.call(this, data);

    if (this.showLoadingMore(data)) {
      this.$results.append(this.$loadingMore);
    }
  };

  InfiniteScroll.prototype.bind = function (decorated, container, $container) {
    var self = this;

    decorated.call(this, container, $container);

    container.on('query', function (params) {
      self.lastParams = params;
      self.loading = true;
    });

    container.on('query:append', function (params) {
      self.lastParams = params;
      self.loading = true;
    });

    this.$results.on('scroll', function () {
      var isLoadMoreVisible = $.contains(
        document.documentElement,
        self.$loadingMore[0]
      );

      if (self.loading || !isLoadMoreVisible) {
        return;
      }

      var currentOffset = self.$results.offset().top +
        self.$results.outerHeight(false);
      var loadingMoreOffset = self.$loadingMore.offset().top +
        self.$loadingMore.outerHeight(false);

      if (currentOffset + 50 >= loadingMoreOffset) {
        self.loadMore();
      }
    });
  };

  InfiniteScroll.prototype.loadMore = function () {
    this.loading = true;

    var params = $.extend({}, {page: 1}, this.lastParams);

    params.page++;

    this.trigger('query:append', params);
  };

  InfiniteScroll.prototype.showLoadingMore = function (_, data) {
    return data.pagination && data.pagination.more;
  };

  InfiniteScroll.prototype.createLoadingMore = function () {
    var $option = $(
      '<li class="option load-more" role="treeitem"></li>'
    );

    var message = this.options.get('translations').get('loadingMore');

    $option.html(message(this.lastParams));

    return $option;
  };

  return InfiniteScroll;
});

define('select2/dropdown/attachBody',[

], function () {
  function AttachBody (decorated, $element, options) {
    this.$dropdownParent = options.get('dropdownParent') || document.body;

    decorated.call(this, $element, options);
  }

  AttachBody.prototype.bind = function (decorated, container, $container) {
    var self = this;

    var setupResultsEvents = false;

    decorated.call(this, container, $container);

    container.on('open', function () {
      self._showDropdown();

      if (!setupResultsEvents) {
        setupResultsEvents = true;

        container.on('results:all', function () {
          self._positionDropdown();
        });

        container.on('results:append', function () {
          self._positionDropdown();
        });
      }
    });

    container.on('close', function () {
      self._hideDropdown();
    });

    this.$dropdownContainer.on('mousedown', function (evt) {
      evt.stopPropagation();
    });
  };

  AttachBody.prototype.position = function (decorated, $dropdown, $container) {
    // Clone all of the container classes
    $dropdown.attr('class', $container.attr('class'));

    $dropdown.removeClass('select2');
    $dropdown.addClass('select2-container--open');

    $dropdown.css({
      position: 'absolute',
      top: -999999
    });

    $dropdown.width($container.outerWidth(false));

    this.$container = $container;
  };

  AttachBody.prototype.render = function (decorated) {
    var $container = $('<span></span>');

    var $dropdown = decorated.call(this);
    $container.append($dropdown);

    this.$dropdownContainer = $container;

    return $container;
  };

  AttachBody.prototype._hideDropdown = function (decorated) {
    this.$dropdownContainer.detach();
  };

  AttachBody.prototype._positionDropdown = function () {
    var $window = $(window);

    var isCurrentlyAbove = this.$dropdown.hasClass('select2-dropdown--above');
    var isCurrentlyBelow = this.$dropdown.hasClass('select2-dropdown--below');

    var newDirection = null;

    var position = this.$container.position();
    var offset = this.$container.offset();

    offset.bottom = offset.top + this.$container.outerHeight(false);

    var container = {
      height: this.$container.outerHeight(false)
    };

    container.top = offset.top;
    container.bottom = offset.top + container.height;

    var dropdown = {
      height: this.$dropdown.outerHeight(false)
    };

    var viewport = {
      top: $window.scrollTop(),
      bottom: $window.scrollTop() + $window.height()
    };

    var enoughRoomAbove = viewport.top < (offset.top - dropdown.height);
    var enoughRoomBelow = viewport.bottom > (offset.bottom + dropdown.height);

    var css = {
      left: offset.left,
      top: container.bottom
    };

    if (!isCurrentlyAbove && !isCurrentlyBelow) {
      newDirection = 'below';
    }

    if (!enoughRoomBelow && enoughRoomAbove && !isCurrentlyAbove) {
      newDirection = 'above';
    } else if (!enoughRoomAbove && enoughRoomBelow && isCurrentlyAbove) {
      newDirection = 'below';
    }

    if (newDirection == 'above' || isCurrentlyAbove) {
      css.top = container.top - dropdown.height;
    }

    if (newDirection != null) {
      this.$dropdown
        .removeClass('select2-dropdown--below select2-dropdown--above')
        .addClass('select2-dropdown--' + newDirection);
      this.$container
        .removeClass('select2-container--below select2-container--above')
        .addClass('select2-container--' + newDirection);
    }

    this.$dropdownContainer.css(css);
  };

  AttachBody.prototype._showDropdown = function (decorated) {
    this.$dropdownContainer.appendTo(this.$dropdownParent);

    this._positionDropdown();
  };

  return AttachBody;
});

define('select2/dropdown/minimumResultsForSearch',[

], function () {
  function countResults (data) {
    count = 0;

    for (var d = 0; d < data.length; d++) {
      var item = data[d];

      if (item.children) {
        count += countResults(item.children);
      } else {
        count++;
      }
    }

    return count;
  }

  function MinimumResultsForSearch (decorated, $element, options, dataAdapter) {
    this.minimumResultsForSearch = options.get('minimumResultsForSearch');

    decorated.call(this, $element, options, dataAdapter);
  }

  MinimumResultsForSearch.prototype.showSearch = function (decorated, params) {
    if (countResults(params.data.results) < this.minimumResultsForSearch) {
      return false;
    }

    return decorated.call(this, params);
  };

  return MinimumResultsForSearch;
});

define('select2/dropdown/selectOnClose',[

], function () {
  function SelectOnClose () { }

  SelectOnClose.prototype.bind = function (decorated, container, $container) {
    var self = this;

    decorated.call(this, container, $container);

    container.on('close', function () {
      self._handleSelectOnClose();
    });
  };

  SelectOnClose.prototype._handleSelectOnClose = function () {
    var $highlightedResults = this.getHighlightedResults();

    if ($highlightedResults.length < 1) {
      return;
    }

    $highlightedResults.trigger('mouseup');
  };

  return SelectOnClose;
});

define('select2/i18n/en',[],function () {
  // English
  return {
    errorLoading: function () {
      return 'The results could not be loaded.';
    },
    inputTooLong: function (args) {
      var overChars = args.input.length - args.maximum;

      var message = 'Please delete ' + overChars + ' character';

      if (overChars != 1) {
        message += 's';
      }

      return message;
    },
    inputTooShort: function (args) {
      var remainingChars = args.minimum - args.input.length;

      var message = 'Please enter ' + remainingChars + ' or more characters';

      return message;
    },
    loadingMore: function () {
      return 'Loading more results…';
    },
    maximumSelected: function (args) {
      var message = 'You can only select ' + args.maximum + ' item';

      if (args.maximum != 1) {
        message += 's';
      }

      return message;
    },
    noResults: function () {
      return 'No results found';
    },
    searching: function () {
      return 'Searching…';
    }
  };
});

define('select2/defaults',[
  'jquery',
  './results',

  './selection/single',
  './selection/multiple',
  './selection/placeholder',
  './selection/allowClear',
  './selection/search',
  './selection/eventRelay',

  './utils',
  './translation',
  './diacritics',

  './data/select',
  './data/array',
  './data/ajax',
  './data/tags',
  './data/tokenizer',
  './data/minimumInputLength',
  './data/maximumInputLength',
  './data/maximumSelectionLength',

  './dropdown',
  './dropdown/search',
  './dropdown/hidePlaceholder',
  './dropdown/infiniteScroll',
  './dropdown/attachBody',
  './dropdown/minimumResultsForSearch',
  './dropdown/selectOnClose',

  './i18n/en'
], function ($, ResultsList,

             SingleSelection, MultipleSelection, Placeholder, AllowClear,
             SelectionSearch, EventRelay,

             Utils, Translation, DIACRITICS,

             SelectData, ArrayData, AjaxData, Tags, Tokenizer,
             MinimumInputLength, MaximumInputLength, MaximumSelectionLength,

             Dropdown, DropdownSearch, HidePlaceholder, InfiniteScroll,
             AttachBody, MinimumResultsForSearch, SelectOnClose,

             EnglishTranslation) {
  function Defaults () {
    this.reset();
  }

  Defaults.prototype.apply = function (options) {
    options = $.extend({}, this.defaults, options);

    if (options.dataAdapter == null) {
      if (options.ajax != null) {
        options.dataAdapter = AjaxData;
      } else if (options.data != null) {
        options.dataAdapter = ArrayData;
      } else {
        options.dataAdapter = SelectData;
      }

      if (options.minimumInputLength > 0) {
        options.dataAdapter = Utils.Decorate(
          options.dataAdapter,
          MinimumInputLength
        );
      }

      if (options.maximumInputLength > 0) {
        options.dataAdapter = Utils.Decorate(
          options.dataAdapter,
          MaximumInputLength
        );
      }

      if (options.maximumSelectionLength > 0) {
        options.dataAdapter = Utils.Decorate(
          options.dataAdapter,
          MaximumSelectionLength
        );
      }

      if (options.tags != null) {
        options.dataAdapter = Utils.Decorate(options.dataAdapter, Tags);
      }

      if (options.tokenSeparators != null || options.tokenizer != null) {
        options.dataAdapter = Utils.Decorate(
          options.dataAdapter,
          Tokenizer
        );
      }
    }

    if (options.resultsAdapter == null) {
      options.resultsAdapter = ResultsList;

      if (options.ajax != null) {
        options.resultsAdapter = Utils.Decorate(
          options.resultsAdapter,
          InfiniteScroll
        );
      }

      if (options.placeholder != null) {
        options.resultsAdapter = Utils.Decorate(
          options.resultsAdapter,
          HidePlaceholder
        );
      }
    }

    if (options.dropdownAdapter == null) {
      if (options.multiple) {
        options.dropdownAdapter = Dropdown;
      } else {
        var SearchableDropdown = Utils.Decorate(Dropdown, DropdownSearch);

        options.dropdownAdapter = SearchableDropdown;
      }

      if (options.minimumResultsForSearch > 0) {
        options.dropdownAdapter = Utils.Decorate(
          options.dropdownAdapter,
          MinimumResultsForSearch
        );
      }

      if (options.selectOnClose) {
        options.dropdownAdapter = Utils.Decorate(
          options.dropdownAdapter,
          SelectOnClose
        );
      }

      options.dropdownAdapter = Utils.Decorate(
        options.dropdownAdapter,
        AttachBody
      );
    }

    if (options.selectionAdapter == null) {
      if (options.multiple) {
        options.selectionAdapter = MultipleSelection;
      } else {
        options.selectionAdapter = SingleSelection;
      }

      // Add the placeholder mixin if a placeholder was specified
      if (options.placeholder != null) {
        options.selectionAdapter = Utils.Decorate(
          options.selectionAdapter,
          Placeholder
        );

        if (options.allowClear) {
          options.selectionAdapter = Utils.Decorate(
            options.selectionAdapter,
            AllowClear
          );
        }
      }

      if (options.multiple) {
        options.selectionAdapter = Utils.Decorate(
          options.selectionAdapter,
          SelectionSearch
        );
      }

      options.selectionAdapter = Utils.Decorate(
        options.selectionAdapter,
        EventRelay
      );
    }

    if (typeof options.language === 'string') {
      options.language = [options.language];
    }

    if ($.isArray(options.language)) {
      var languages = new Translation();
      options.language.push('en');

      var languageNames = options.language;

      for (var l = 0; l < languageNames.length; l++) {
        var name = languageNames[l];
        var language = {};

        try {
          // Try to load it with the original name
          language = Translation.loadPath(name);
        } catch (e) {
          // If we couldn't load it, check if it wasn't the full path
          name = this.defaults.amdLanguageBase + name;
          language = Translation.loadPath(name);
        }

        languages.extend(language);
      }

      options.translations = languages;
    } else {
      options.translations = new Translation(options.language);
    }

    return options;
  };

  Defaults.prototype.reset = function () {
    function stripDiacritics (text) {
      // Used 'uni range + named function' from http://jsperf.com/diacritics/18
      function match(a) {
        return DIACRITICS[a] || a;
      }

      return text.replace(/[^\u0000-\u007E]/g, match);
    }

    function matcher (params, data) {
      // Always return the object if there is nothing to compare
      if ($.trim(params.term) === '') {
        return data;
      }

      // Do a recursive check for options with children
      if (data.children && data.children.length > 0) {
        // Clone the data object if there are children
        // This is required as we modify the object to remove any non-matches
        var match = $.extend(true, {}, data);

        // Check each child of the option
        for (var c = data.children.length - 1; c >= 0; c--) {
          var child = data.children[c];

          var matches = matcher(params, child);

          // If there wasn't a match, remove the object in the array
          if (matches == null) {
            match.children.splice(c, 1);
          }
        }

        // If any children matched, return the new object
        if (match.children.length > 0) {
          return match;
        }

        // If there were no matching children, check just the plain object
        return matcher(params, match);
      }

      var original = stripDiacritics(data.text).toUpperCase();
      var term = stripDiacritics(params.term).toUpperCase();

      // Check if the text contains the term
      if (original.indexOf(term) > -1) {
        return data;
      }

      // If it doesn't contain the term, don't return anything
      return null;
    }

    this.defaults = {
      amdBase: 'select2/',
      amdLanguageBase: 'select2/i18n/',
      language: EnglishTranslation,
      matcher: matcher,
      minimumInputLength: 0,
      maximumInputLength: 0,
      maximumSelectionLength: 0,
      minimumResultsForSearch: 0,
      selectOnClose: false,
      sorter: function (data) {
        return data;
      },
      templateResult: function (result) {
        return result.text;
      },
      templateSelection: function (selection) {
        return selection.text;
      },
      theme: 'default',
      width: 'resolve'
    };
  };

  var defaults = new Defaults();

  return defaults;
});

define('select2/options',[
  'jquery',
  './defaults'
], function ($, Defaults) {
  function Options (options, $element) {
    this.options = options;

    if ($element != null) {
      this.fromElement($element);
    }

    this.options = Defaults.apply(this.options);
  }

  Options.prototype.fromElement = function ($e) {
    var excludedData = ['select2'];

    if (this.options.multiple == null) {
      this.options.multiple = $e.prop('multiple');
    }

    if (this.options.disabled == null) {
      this.options.disabled = $e.prop('disabled');
    }

    if (this.options.language == null) {
      if ($e.prop('lang')) {
        this.options.language = $e.prop('lang').toLowerCase();
      } else if ($e.closest('[lang]').prop('lang')) {
        this.options.language = $e.closest('[lang]').prop('lang');
      }
    }

    if (this.options.dir == null) {
      if ($e.prop('dir')) {
        this.options.dir = $e.prop('dir');
      } else if ($e.closest('[dir]').prop('dir')) {
        this.options.dir = $e.closest('[dir]').prop('dir');
      } else {
        this.options.dir = 'ltr';
      }
    }

    $e.prop('disabled', this.options.disabled);
    $e.prop('multiple', this.options.multiple);

    if ($e.data('select2-tags')) {
      if (console && console.warn) {
        console.warn(
          'Select2: The `data-select2-tags` attribute has been changed to ' +
          'use the `data-data` and `data-tags="true"` attributes and will be ' +
          'removed in future versions of Select2.'
        );
      }

      $e.data('data', $e.data('select2-tags'));
      $e.data('tags', true);
    }

    if ($e.data('ajax-url')) {
      if (console && console.warn) {
        console.warn(
          'Select2: The `data-ajax-attribute` has been changed to ' +
          '`data-ajax--url` and support for the old attribute will be removed' +
          ' in future versions of Select2.'
        );
      }

      $e.data('ajax--url', $e.data('ajax-url'));
    }

    var data = $e.data();

    function convertData (data) {
      for (var originalKey in data) {
        var keys = originalKey.split('-');

        var dataLevel = data;

        if (keys.length === 1) {
          continue;
        }

        for (var k = 0; k < keys.length; k++) {
          var key = keys[k];

          // Lowercase the first letter
          // By default, dash-separated becomes camelCase
          key = key.substring(0, 1).toLowerCase() + key.substring(1);

          if (!(key in dataLevel)) {
            dataLevel[key] = {};
          }

          if (k == keys.length - 1) {
            dataLevel[key] = data[originalKey];
          }

          dataLevel = dataLevel[key];
        }

        delete data[originalKey];
      }

      return data;
    }

    data = convertData(data);

    for (var key in data) {
      if (excludedData.indexOf(key) > -1) {
        continue;
      }

      if ($.isPlainObject(this.options[key])) {
        $.extend(this.options[key], data[key]);
      } else {
        this.options[key] = data[key];
      }
    }

    return this;
  };

  Options.prototype.get = function (key) {
    return this.options[key];
  };

  Options.prototype.set = function (key, val) {
    this.options[key] = val;
  };

  return Options;
});

define('select2/core',[
  'jquery',
  './options',
  './utils',
  './keys'
], function ($, Options, Utils, KEYS) {
  var Select2 = function ($element, options) {
    if ($element.data('select2') != null) {
      $element.data('select2').destroy();
    }

    this.$element = $element;

    this.id = this._generateId($element);

    options = options || {};

    this.options = new Options(options, $element);

    Select2.__super__.constructor.call(this);

    // Set up containers and adapters

    var DataAdapter = this.options.get('dataAdapter');
    this.data = new DataAdapter($element, this.options);

    var $container = this.render();

    this._placeContainer($container);

    var SelectionAdapter = this.options.get('selectionAdapter');
    this.selection = new SelectionAdapter($element, this.options);
    this.$selection = this.selection.render();

    this.selection.position(this.$selection, $container);

    var DropdownAdapter = this.options.get('dropdownAdapter');
    this.dropdown = new DropdownAdapter($element, this.options);
    this.$dropdown = this.dropdown.render();

    this.dropdown.position(this.$dropdown, $container);

    var ResultsAdapter = this.options.get('resultsAdapter');
    this.results = new ResultsAdapter($element, this.options, this.data);
    this.$results = this.results.render();

    this.results.position(this.$results, this.$dropdown);

    // Bind events

    var self = this;

    // Bind the container to all of the adapters
    this._bindAdapters();

    // Register any DOM event handlers
    this._registerDomEvents();

    // Register any internal event handlers
    this._registerDataEvents();
    this._registerSelectionEvents();
    this._registerDropdownEvents();
    this._registerResultsEvents();
    this._registerEvents();

    // Set the initial state
    this.data.current(function (initialData) {
      self.trigger('selection:update', {
        data: initialData
      });
    });

    // Hide the original select
    $element.hide();

    // Synchronize any monitored attributes
    this._syncAttributes();

    this._tabindex = $element.attr('tabindex') || 0;

    $element.attr('tabindex', '-1');

    $element.data('select2', this);
  };

  Utils.Extend(Select2, Utils.Observable);

  Select2.prototype._generateId = function ($element) {
    var id = '';

    if ($element.attr('id') != null) {
      id = $element.attr('id');
    } else if ($element.attr('name') != null) {
      id = $element.attr('name') + '-' + Utils.generateChars(2);
    } else {
      id = Utils.generateChars(4);
    }

    id = 'select2-' + id;

    return id;
  };

  Select2.prototype._placeContainer = function ($container) {
    $container.insertAfter(this.$element);

    var width = this._resolveWidth(this.$element, this.options.get('width'));

    if (width != null) {
      $container.css('width', width);
    }
  };

  Select2.prototype._resolveWidth = function ($element, method) {
    var WIDTH = /^width:(([-+]?([0-9]*\.)?[0-9]+)(px|em|ex|%|in|cm|mm|pt|pc))/i;

    if (method == 'resolve') {
      var styleWidth = this._resolveWidth($element, 'style');

      if (styleWidth != null) {
        return styleWidth;
      }

      return this._resolveWidth($element, 'element');
    }

    if (method == 'element') {
      var elementWidth = $element.outerWidth(false);

      if (elementWidth <= 0) {
        return 'auto';
      }

      return elementWidth + 'px';
    }

    if (method == 'style') {
      var style = $element.attr('style');

      if (typeof(style) !== 'string') {
        return null;
      }

      var attrs = style.split(';');

      for (i = 0, l = attrs.length; i < l; i = i + 1) {
        attr = attrs[i].replace(/\s/g, '');
        var matches = attr.match(WIDTH);

        if (matches !== null && matches.length >= 1) {
          return matches[1];
        }
      }
    }

    return method;
  };

  Select2.prototype._bindAdapters = function () {
    this.data.bind(this, this.$container);
    this.selection.bind(this, this.$container);

    this.dropdown.bind(this, this.$container);
    this.results.bind(this, this.$container);
  };

  Select2.prototype._registerDomEvents = function () {
    var self = this;

    this.$element.on('change.select2', function () {
      self.data.current(function (data) {
        self.trigger('selection:update', {
          data: data
        });
      });
    });

    this._sync = Utils.bind(this._syncAttributes, this);

    if (this.$element[0].attachEvent) {
      this.$element[0].attachEvent('onpropertychange', this._sync);
    }

    var observer = window.MutationObserver ||
      window.WebKitMutationObserver ||
      window.MozMutationObserver
    ;

    if (observer != null) {
      this._observer = new observer(function (mutations) {
        $.each(mutations, self._sync);
      });
      this._observer.observe(this.$element[0], {
        attributes: true,
        subtree: false
      });
    }
  };

  Select2.prototype._registerDataEvents = function () {
    var self = this;

    this.data.on('*', function (name, params) {
      self.trigger(name, params);
    });
  };

  Select2.prototype._registerSelectionEvents = function () {
    var self = this;
    var nonRelayEvents = ['toggle'];

    this.selection.on('toggle', function () {
      self.toggleDropdown();
    });

    this.selection.on('*', function (name, params) {
      if (nonRelayEvents.indexOf(name) !== -1) {
        return;
      }

      self.trigger(name, params);
    });
  };

  Select2.prototype._registerDropdownEvents = function () {
    var self = this;

    this.dropdown.on('*', function (name, params) {
      self.trigger(name, params);
    });
  };

  Select2.prototype._registerResultsEvents = function () {
    var self = this;

    this.results.on('*', function (name, params) {
      self.trigger(name, params);
    });
  };

  Select2.prototype._registerEvents = function () {
    var self = this;

    this.on('open', function () {
      self.$container.addClass('select2-container--open');
    });

    this.on('close', function () {
      self.$container.removeClass('select2-container--open');
    });

    this.on('enable', function () {
      self.$container.removeClass('select2-container--disabled');
    });

    this.on('disable', function () {
      self.$container.addClass('select2-container--disabled');
    });

    this.on('query', function (params) {
      this.data.query(params, function (data) {
        self.trigger('results:all', {
          data: data,
          query: params
        });
      });
    });

    this.on('query:append', function (params) {
      this.data.query(params, function (data) {
        self.trigger('results:append', {
          data: data,
          query: params
        });
      });
    });

    this.on('keypress', function (evt) {
      var key = evt.which;

      if (self.isOpen()) {
        if (key === KEYS.ENTER) {
          self.trigger('results:select');

          evt.preventDefault();
        } else if (key === KEYS.UP) {
          self.trigger('results:previous');

          evt.preventDefault();
        } else if (key === KEYS.DOWN) {
          self.trigger('results:next');

          evt.preventDefault();
        } else if (key === KEYS.ESC || key === KEYS.TAB) {
          self.close();

          evt.preventDefault();
        }
      } else {
        if (key === KEYS.ENTER || key === KEYS.SPACE ||
            ((key === KEYS.DOWN || key === KEYS.UP) && evt.altKey)) {
          self.open();

          evt.preventDefault();
        }
      }
    });
  };

  Select2.prototype._syncAttributes = function () {
    this.options.set('disabled', this.$element.prop('disabled'));

    if (this.options.get('disabled')) {
      if (this.isOpen()) {
        this.close();
      }

      this.trigger('disable');
    } else {
      this.trigger('enable');
    }
  };

  /**
   * Override the trigger method to automatically trigger pre-events when
   * there are events that can be prevented.
   */
  Select2.prototype.trigger = function (name, args) {
    var actualTrigger = Select2.__super__.trigger;
    var preTriggerMap = {
      'open': 'opening',
      'close': 'closing',
      'select': 'selecting',
      'unselect': 'unselecting'
    };

    if (name in preTriggerMap) {
      var preTriggerName = preTriggerMap[name];
      var preTriggerArgs = {
        prevented: false,
        name: name,
        args: args
      };

      actualTrigger.call(this, preTriggerName, preTriggerArgs);

      if (preTriggerArgs.prevented) {
        args.prevented = true;

        return;
      }
    }

    actualTrigger.call(this, name, args);
  };

  Select2.prototype.toggleDropdown = function () {
    if (this.options.get('disabled')) {
      return;
    }

    if (this.isOpen()) {
      this.close();
    } else {
      this.open();
    }
  };

  Select2.prototype.open = function () {
    if (this.isOpen()) {
      return;
    }

    this.trigger('query', {});

    this.trigger('open');
  };

  Select2.prototype.close = function () {
    if (!this.isOpen()) {
      return;
    }

    this.trigger('close');
  };

  Select2.prototype.isOpen = function () {
    return this.$container.hasClass('select2-container--open');
  };

  Select2.prototype.enable = function (args) {
    if (console && console.warn) {
      console.warn(
        'Select2: The `select2("enable")` method has been deprecated and will' +
        ' be removed in later Select2 versions. Use $element.prop("disabled")' +
        ' instead.'
      );
    }

    if (args.length === 0) {
      args = [true];
    }

    var disabled = !args[0];

    this.$element.prop('disabled', disabled);
  };

  Select2.prototype.val = function (args) {
    if (console && console.warn) {
      console.warn(
        'Select2: The `select2("val")` method has been deprecated and will be' +
        ' removed in later Select2 versions. Use $element.val() instead.'
      );
    }

    if (args.length === 0) {
      return this.$element.val();
    }

    var newVal = args[0];

    if ($.isArray(newVal)) {
      newVal = $.map(newVal, function (obj) {
        return obj.toString();
      });
    }

    this.$element.val(newVal).trigger('change');
  };

  Select2.prototype.destroy = function () {
    this.$container.remove();

    if (this.$element[0].detachEvent) {
      this.$element[0].detachEvent('onpropertychange', this._sync);
    }

    if (this._observer != null) {
      this._observer.disconnect();
      this._observer = null;
    }

    this._sync = null;

    this.$element.off('.select2');
    this.$element.attr('tabindex', this._tabindex);

    this.$element.show();
    this.$element.removeData('select2');

    this.data.destroy();
    this.selection.destroy();
    this.dropdown.destroy();
    this.results.destroy();

    this.data = null;
    this.selection = null;
    this.dropdown = null;
    this.results = null;
  };

  Select2.prototype.render = function () {
    var $container = $(
      '<span class="select2 select2-container">' +
        '<span class="selection"></span>' +
        '<span class="dropdown-wrapper" aria-hidden="true"></span>' +
      '</span>'
    );

    $container.attr('dir', this.options.get('dir'));

    this.$container = $container;

    this.$container.addClass('select2-container--' + this.options.get('theme'));

    $container.data('element', this.$element);

    return $container;
  };

  return Select2;
});

define('jquery.select2',[
  'jquery',
  './select2/core'
], function ($, Select2) {
  // Force jQuery.mousewheel to be loaded if it hasn't already
  try {
    require('jquery.mousewheel');
  } catch (Exception) { }

  if ($.fn.select2 == null) {
    $.fn.select2 = function (options) {
      options = options || {};

      if (typeof options === 'object') {
        this.each(function () {
          var instanceOptions = $.extend({}, options, true);

          var instance = new Select2($(this), instanceOptions);
        });

        return this;
      } else if (typeof options === 'string') {
        var instance = this.data('select2');
        var args = Array.prototype.slice.call(arguments, 1);

        return instance[options](args);
      } else {
        throw new Error('Invalid arguments for Select2: ' + options);
      }
    };
  }

  return Select2;
});

require('jquery.select2'); $.fn.select2.amd = { define: define, require: require };}());
/*!
 * jQuery blockUI plugin
 * Version 2.70.0-2014.11.23
 * Requires jQuery v1.7 or later
 *
 * Examples at: http://malsup.com/jquery/block/
 * Copyright (c) 2007-2013 M. Alsup
 * Dual licensed under the MIT and GPL licenses:
 * http://www.opensource.org/licenses/mit-license.php
 * http://www.gnu.org/licenses/gpl.html
 *
 * Thanks to Amir-Hossein Sobhi for some excellent contributions!
 */

; (function () {
   /*jshint eqeqeq:false curly:false latedef:false */
   "use strict";

   function setup($) {
      $.fn._fadeIn = $.fn.fadeIn;

      var noOp = $.noop || function () { };

      // this bit is to ensure we don't call setExpression when we shouldn't (with extra muscle to handle
      // confusing userAgent strings on Vista)
      var msie = /MSIE/.test(navigator.userAgent);
      var ie6 = /MSIE 6.0/.test(navigator.userAgent) && ! /MSIE 8.0/.test(navigator.userAgent);
      var mode = document.documentMode || 0;
      var setExpr = $.isFunction(document.createElement('div').style.setExpression);

      // global $ methods for blocking/unblocking the entire page
      $.blockUI = function (opts) { install(window, opts); };
      $.unblockUI = function (opts) { remove(window, opts); };

      // convenience method for quick growl-like notifications  (http://www.google.com/search?q=growl)
      $.growlUI = function (title, message, timeout, onClose) {
         var $m = $('<div class="growlUI"></div>');
         if (title) $m.append('<h1>' + title + '</h1>');
         if (message) $m.append('<h2>' + message + '</h2>');
         if (timeout === undefined) timeout = 3000;

         // Added by konapun: Set timeout to 30 seconds if this growl is moused over, like normal toast notifications
         var callBlock = function (opts) {
            opts = opts || {};

            $.blockUI({
               message: $m,
               fadeIn: typeof opts.fadeIn !== 'undefined' ? opts.fadeIn : 700,
               fadeOut: typeof opts.fadeOut !== 'undefined' ? opts.fadeOut : 1000,
               timeout: typeof opts.timeout !== 'undefined' ? opts.timeout : timeout,
               centerY: false,
               showOverlay: false,
               onUnblock: onClose,
               css: $.blockUI.defaults.growlCSS
            });
         };

         callBlock();
         var nonmousedOpacity = $m.css('opacity');
         $m.mouseover(function () {
            callBlock({
               fadeIn: 0,
               timeout: 30000
            });

            var displayBlock = $('.blockMsg');
            displayBlock.stop(); // cancel fadeout if it has started
            displayBlock.fadeTo(300, 1); // make it easier to read the message by removing transparency
         }).mouseout(function () {
            $('.blockMsg').fadeOut(1000);
         });
         // End konapun additions
      };

      // plugin method for blocking element content
      $.fn.block = function (opts) {
         if (this[0] === window) {
            $.blockUI(opts);
            return this;
         }
         var fullOpts = $.extend({}, $.blockUI.defaults, opts || {});
         this.each(function () {
            var $el = $(this);
            if (fullOpts.ignoreIfBlocked && $el.data('blockUI.isBlocked'))
               return;
            $el.unblock({ fadeOut: 0 });
         });

         return this.each(function () {
            if ($.css(this, 'position') == 'static') {
               this.style.position = 'relative';
               $(this).data('blockUI.static', true);
            }
            this.style.zoom = 1; // force 'hasLayout' in ie
            install(this, opts);
         });
      };

      // plugin method for unblocking element content
      $.fn.unblock = function (opts) {
         if (this[0] === window) {
            $.unblockUI(opts);
            return this;
         }
         return this.each(function () {
            remove(this, opts);
         });
      };

      $.blockUI.version = 2.70; // 2nd generation blocking at no extra cost!

      // override these in your code to change the default behavior and style
      $.blockUI.defaults = {
         // message displayed when blocking (use null for no message)
         message: '<h1>Please wait...</h1>',

         title: null,		// title string; only used when theme == true
         draggable: true,	// only used when theme == true (requires jquery-ui.js to be loaded)

         theme: false, // set to true to use with jQuery UI themes

         // styles for the message when blocking; if you wish to disable
         // these and use an external stylesheet then do this in your code:
         // $.blockUI.defaults.css = {};
         css: {
            padding: 0,
            margin: 0,
            width: '30%',
            top: '40%',
            left: '35%',
            textAlign: 'center',
            color: '#000',
            border: '3px solid #aaa',
            backgroundColor: '#fff',
            cursor: 'wait'
         },

         // minimal style set used when themes are used
         themedCSS: {
            width: '30%',
            top: '40%',
            left: '35%'
         },

         // styles for the overlay
         overlayCSS: {
            backgroundColor: '#000',
            opacity: 0.6,
            cursor: 'wait'
         },

         // style to replace wait cursor before unblocking to correct issue
         // of lingering wait cursor
         cursorReset: 'default',

         // styles applied when using $.growlUI
         growlCSS: {
            width: '350px',
            top: '10px',
            left: '',
            right: '10px',
            border: 'none',
            padding: '5px',
            opacity: 0.6,
            cursor: 'default',
            color: '#fff',
            backgroundColor: '#000',
            '-webkit-border-radius': '10px',
            '-moz-border-radius': '10px',
            'border-radius': '10px'
         },

         // IE issues: 'about:blank' fails on HTTPS and javascript:false is s-l-o-w
         // (hat tip to Jorge H. N. de Vasconcelos)
         /*jshint scripturl:true */
         iframeSrc: /^https/i.test(window.location.href || '') ? 'javascript:false' : 'about:blank',

         // force usage of iframe in non-IE browsers (handy for blocking applets)
         forceIframe: false,

         // z-index for the blocking overlay
         baseZ: 1000,

         // set these to true to have the message automatically centered
         centerX: true, // <-- only effects element blocking (page block controlled via css above)
         centerY: true,

         // allow body element to be stetched in ie6; this makes blocking look better
         // on "short" pages.  disable if you wish to prevent changes to the body height
         allowBodyStretch: true,

         // enable if you want key and mouse events to be disabled for content that is blocked
         bindEvents: true,

         // be default blockUI will supress tab navigation from leaving blocking content
         // (if bindEvents is true)
         constrainTabKey: true,

         // fadeIn time in millis; set to 0 to disable fadeIn on block
         fadeIn: 200,

         // fadeOut time in millis; set to 0 to disable fadeOut on unblock
         fadeOut: 400,

         // time in millis to wait before auto-unblocking; set to 0 to disable auto-unblock
         timeout: 0,

         // disable if you don't want to show the overlay
         showOverlay: true,

         // if true, focus will be placed in the first available input field when
         // page blocking
         focusInput: true,

         // elements that can receive focus
         focusableElements: ':input:enabled:visible',

         // suppresses the use of overlay styles on FF/Linux (due to performance issues with opacity)
         // no longer needed in 2012
         // applyPlatformOpacityRules: true,

         // callback method invoked when fadeIn has completed and blocking message is visible
         onBlock: null,

         // callback method invoked when unblocking has completed; the callback is
         // passed the element that has been unblocked (which is the window object for page
         // blocks) and the options that were passed to the unblock call:
         //	onUnblock(element, options)
         onUnblock: null,

         // callback method invoked when the overlay area is clicked.
         // setting this will turn the cursor to a pointer, otherwise cursor defined in overlayCss will be used.
         onOverlayClick: null,

         // don't ask; if you really must know: http://groups.google.com/group/jquery-en/browse_thread/thread/36640a8730503595/2f6a79a77a78e493#2f6a79a77a78e493
         quirksmodeOffsetHack: 4,

         // class name of the message block
         blockMsgClass: 'blockMsg',

         // if it is already blocked, then ignore it (don't unblock and reblock)
         ignoreIfBlocked: false
      };

      // private data and functions follow...

      var pageBlock = null;
      var pageBlockEls = [];

      function install(el, opts) {
         var css, themedCSS;
         var full = (el == window);
         var msg = (opts && opts.message !== undefined ? opts.message : undefined);
         opts = $.extend({}, $.blockUI.defaults, opts || {});

         if (opts.ignoreIfBlocked && $(el).data('blockUI.isBlocked'))
            return;

         opts.overlayCSS = $.extend({}, $.blockUI.defaults.overlayCSS, opts.overlayCSS || {});
         css = $.extend({}, $.blockUI.defaults.css, opts.css || {});
         if (opts.onOverlayClick)
            opts.overlayCSS.cursor = 'pointer';

         themedCSS = $.extend({}, $.blockUI.defaults.themedCSS, opts.themedCSS || {});
         msg = msg === undefined ? opts.message : msg;

         // remove the current block (if there is one)
         if (full && pageBlock)
            remove(window, { fadeOut: 0 });

         // if an existing element is being used as the blocking content then we capture
         // its current place in the DOM (and current display style) so we can restore
         // it when we unblock
         if (msg && typeof msg != 'string' && (msg.parentNode || msg.jquery)) {
            var node = msg.jquery ? msg[0] : msg;
            var data = {};
            $(el).data('blockUI.history', data);
            data.el = node;
            data.parent = node.parentNode;
            data.display = node.style.display;
            data.position = node.style.position;
            if (data.parent)
               data.parent.removeChild(node);
         }

         $(el).data('blockUI.onUnblock', opts.onUnblock);
         var z = opts.baseZ;

         // blockUI uses 3 layers for blocking, for simplicity they are all used on every platform;
         // layer1 is the iframe layer which is used to supress bleed through of underlying content
         // layer2 is the overlay layer which has opacity and a wait cursor (by default)
         // layer3 is the message content that is displayed while blocking
         var lyr1, lyr2, lyr3, s;
         if (msie || opts.forceIframe)
            lyr1 = $('<iframe class="blockUI" style="z-index:' + (z++) + ';display:none;border:none;margin:0;padding:0;position:absolute;width:100%;height:100%;top:0;left:0" src="' + opts.iframeSrc + '"></iframe>');
         else
            lyr1 = $('<div class="blockUI" style="display:none"></div>');

         if (opts.theme)
            lyr2 = $('<div class="blockUI blockOverlay ui-widget-overlay" style="z-index:' + (z++) + ';display:none"></div>');
         else
            lyr2 = $('<div class="blockUI blockOverlay" style="z-index:' + (z++) + ';display:none;border:none;margin:0;padding:0;width:100%;height:100%;top:0;left:0"></div>');

         if (opts.theme && full) {
            s = '<div class="blockUI ' + opts.blockMsgClass + ' blockPage ui-dialog ui-widget ui-corner-all" style="z-index:' + (z + 10) + ';display:none;position:fixed">';
            if (opts.title) {
               s += '<div class="ui-widget-header ui-dialog-titlebar ui-corner-all blockTitle">' + (opts.title || '&nbsp;') + '</div>';
            }
            s += '<div class="ui-widget-content ui-dialog-content"></div>';
            s += '</div>';
         }
         else if (opts.theme) {
            s = '<div class="blockUI ' + opts.blockMsgClass + ' blockElement ui-dialog ui-widget ui-corner-all" style="z-index:' + (z + 10) + ';display:none;position:absolute">';
            if (opts.title) {
               s += '<div class="ui-widget-header ui-dialog-titlebar ui-corner-all blockTitle">' + (opts.title || '&nbsp;') + '</div>';
            }
            s += '<div class="ui-widget-content ui-dialog-content"></div>';
            s += '</div>';
         }
         else if (full) {
            s = '<div class="blockUI ' + opts.blockMsgClass + ' blockPage" style="z-index:' + (z + 10) + ';display:none;position:fixed"></div>';
         }
         else {
            s = '<div class="blockUI ' + opts.blockMsgClass + ' blockElement" style="z-index:' + (z + 10) + ';display:none;position:absolute"></div>';
         }
         lyr3 = $(s);

         // if we have a message, style it
         if (msg) {
            if (opts.theme) {
               lyr3.css(themedCSS);
               lyr3.addClass('ui-widget-content');
            }
            else
               lyr3.css(css);
         }

         // style the overlay
         if (!opts.theme /*&& (!opts.applyPlatformOpacityRules)*/)
            lyr2.css(opts.overlayCSS);
         lyr2.css('position', full ? 'fixed' : 'absolute');

         // make iframe layer transparent in IE
         if (msie || opts.forceIframe)
            lyr1.css('opacity', 0.0);

         //$([lyr1[0],lyr2[0],lyr3[0]]).appendTo(full ? 'body' : el);
         var layers = [lyr1, lyr2, lyr3], $par = full ? $('body') : $(el);
         $.each(layers, function () {
            this.appendTo($par);
         });

         if (opts.theme && opts.draggable && $.fn.draggable) {
            lyr3.draggable({
               handle: '.ui-dialog-titlebar',
               cancel: 'li'
            });
         }

         // ie7 must use absolute positioning in quirks mode and to account for activex issues (when scrolling)
         var expr = setExpr && (!$.support.boxModel || $('object,embed', full ? null : el).length > 0);
         if (ie6 || expr) {
            // give body 100% height
            if (full && opts.allowBodyStretch && $.support.boxModel)
               $('html,body').css('height', '100%');

            // fix ie6 issue when blocked element has a border width
            if ((ie6 || !$.support.boxModel) && !full) {
               var t = sz(el, 'borderTopWidth'), l = sz(el, 'borderLeftWidth');
               var fixT = t ? '(0 - ' + t + ')' : 0;
               var fixL = l ? '(0 - ' + l + ')' : 0;
            }

            // simulate fixed position
            $.each(layers, function (i, o) {
               var s = o[0].style;
               s.position = 'absolute';
               if (i < 2) {
                  if (full)
                     s.setExpression('height', 'Math.max(document.body.scrollHeight, document.body.offsetHeight) - (jQuery.support.boxModel?0:' + opts.quirksmodeOffsetHack + ') + "px"');
                  else
                     s.setExpression('height', 'this.parentNode.offsetHeight + "px"');
                  if (full)
                     s.setExpression('width', 'jQuery.support.boxModel && document.documentElement.clientWidth || document.body.clientWidth + "px"');
                  else
                     s.setExpression('width', 'this.parentNode.offsetWidth + "px"');
                  if (fixL) s.setExpression('left', fixL);
                  if (fixT) s.setExpression('top', fixT);
               }
               else if (opts.centerY) {
                  if (full) s.setExpression('top', '(document.documentElement.clientHeight || document.body.clientHeight) / 2 - (this.offsetHeight / 2) + (blah = document.documentElement.scrollTop ? document.documentElement.scrollTop : document.body.scrollTop) + "px"');
                  s.marginTop = 0;
               }
               else if (!opts.centerY && full) {
                  var top = (opts.css && opts.css.top) ? parseInt(opts.css.top, 10) : 0;
                  var expression = '((document.documentElement.scrollTop ? document.documentElement.scrollTop : document.body.scrollTop) + ' + top + ') + "px"';
                  s.setExpression('top', expression);
               }
            });
         }

         // show the message
         if (msg) {
            if (opts.theme)
               lyr3.find('.ui-widget-content').append(msg);
            else
               lyr3.append(msg);
            if (msg.jquery || msg.nodeType)
               $(msg).show();
         }

         if ((msie || opts.forceIframe) && opts.showOverlay)
            lyr1.show(); // opacity is zero
         if (opts.fadeIn) {
            var cb = opts.onBlock ? opts.onBlock : noOp;
            var cb1 = (opts.showOverlay && !msg) ? cb : noOp;
            var cb2 = msg ? cb : noOp;
            if (opts.showOverlay)
               lyr2._fadeIn(opts.fadeIn, cb1);
            if (msg)
               lyr3._fadeIn(opts.fadeIn, cb2);
         }
         else {
            if (opts.showOverlay)
               lyr2.show();
            if (msg)
               lyr3.show();
            if (opts.onBlock)
               opts.onBlock.bind(lyr3)();
         }

         // bind key and mouse events
         bind(1, el, opts);

         if (full) {
            pageBlock = lyr3[0];
            pageBlockEls = $(opts.focusableElements, pageBlock);
            if (opts.focusInput)
               setTimeout(focus, 20);
         }
         else
            center(lyr3[0], opts.centerX, opts.centerY);

         if (opts.timeout) {
            // auto-unblock
            var to = setTimeout(function () {
               if (full)
                  $.unblockUI(opts);
               else
                  $(el).unblock(opts);
            }, opts.timeout);
            $(el).data('blockUI.timeout', to);
         }
      }

      // remove the block
      function remove(el, opts) {
         var count;
         var full = (el == window);
         var $el = $(el);
         var data = $el.data('blockUI.history');
         var to = $el.data('blockUI.timeout');
         if (to) {
            clearTimeout(to);
            $el.removeData('blockUI.timeout');
         }
         opts = $.extend({}, $.blockUI.defaults, opts || {});
         bind(0, el, opts); // unbind events

         if (opts.onUnblock === null) {
            opts.onUnblock = $el.data('blockUI.onUnblock');
            $el.removeData('blockUI.onUnblock');
         }

         var els;
         if (full) // crazy selector to handle odd field errors in ie6/7
            els = $('body').children().filter('.blockUI').add('body > .blockUI');
         else
            els = $el.find('>.blockUI');

         // fix cursor issue
         if (opts.cursorReset) {
            if (els.length > 1)
               els[1].style.cursor = opts.cursorReset;
            if (els.length > 2)
               els[2].style.cursor = opts.cursorReset;
         }

         if (full)
            pageBlock = pageBlockEls = null;

         if (opts.fadeOut) {
            count = els.length;
            els.stop().fadeOut(opts.fadeOut, function () {
               if (--count === 0)
                  reset(els, data, opts, el);
            });
         }
         else
            reset(els, data, opts, el);
      }

      // move blocking element back into the DOM where it started
      function reset(els, data, opts, el) {
         var $el = $(el);
         if ($el.data('blockUI.isBlocked'))
            return;

         els.each(function (i, o) {
            // remove via DOM calls so we don't lose event handlers
            if (this.parentNode)
               this.parentNode.removeChild(this);
         });

         if (data && data.el) {
            data.el.style.display = data.display;
            data.el.style.position = data.position;
            data.el.style.cursor = 'default'; // #59
            if (data.parent)
               data.parent.appendChild(data.el);
            $el.removeData('blockUI.history');
         }

         if ($el.data('blockUI.static')) {
            $el.css('position', 'static'); // #22
         }

         if (typeof opts.onUnblock == 'function')
            opts.onUnblock(el, opts);

         // fix issue in Safari 6 where block artifacts remain until reflow
         var body = $(document.body), w = body.width(), cssW = body[0].style.width;
         body.width(w - 1).width(w);
         body[0].style.width = cssW;
      }

      // bind/unbind the handler
      function bind(b, el, opts) {
         var full = el == window, $el = $(el);

         // don't bother unbinding if there is nothing to unbind
         if (!b && (full && !pageBlock || !full && !$el.data('blockUI.isBlocked')))
            return;

         $el.data('blockUI.isBlocked', b);

         // don't bind events when overlay is not in use or if bindEvents is false
         if (!full || !opts.bindEvents || (b && !opts.showOverlay))
            return;

         // bind anchors and inputs for mouse and key events
         var events = 'mousedown mouseup keydown keypress keyup touchstart touchend touchmove';
         if (b)
            $(document).bind(events, opts, handler);
         else
            $(document).unbind(events, handler);

         // former impl...
         //		var $e = $('a,:input');
         //		b ? $e.bind(events, opts, handler) : $e.unbind(events, handler);
      }

      // event handler to suppress keyboard/mouse events when blocking
      function handler(e) {
         // allow tab navigation (conditionally)
         if (e.type === 'keydown' && e.keyCode && e.keyCode == 9) {
            if (pageBlock && e.data.constrainTabKey) {
               var els = pageBlockEls;
               var fwd = !e.shiftKey && e.target === els[els.length - 1];
               var back = e.shiftKey && e.target === els[0];
               if (fwd || back) {
                  setTimeout(function () { focus(back); }, 10);
                  return false;
               }
            }
         }
         var opts = e.data;
         var target = $(e.target);
         if (target.hasClass('blockOverlay') && opts.onOverlayClick)
            opts.onOverlayClick(e);

         // allow events within the message content
         if (target.parents('div.' + opts.blockMsgClass).length > 0)
            return true;

         // allow events for content that is not being blocked
         return target.parents().children().filter('div.blockUI').length === 0;
      }

      function focus(back) {
         if (!pageBlockEls)
            return;
         var e = pageBlockEls[back === true ? pageBlockEls.length - 1 : 0];
         if (e)
            e.focus();
      }

      function center(el, x, y) {
         var p = el.parentNode, s = el.style;
         var l = ((p.offsetWidth - el.offsetWidth) / 2) - sz(p, 'borderLeftWidth');
         var t = ((p.offsetHeight - el.offsetHeight) / 2) - sz(p, 'borderTopWidth');
         if (x) s.left = l > 0 ? (l + 'px') : '0';
         if (y) s.top = t > 0 ? (t + 'px') : '0';
      }

      function sz(el, p) {
         return parseInt($.css(el, p), 10) || 0;
      }

   }


   /*global define:true */
   if (typeof define === 'function' && define.amd && define.amd.jQuery) {
      define(['jquery'], setup);
   } else {
      setup(jQuery);
   }

})();


//Title: Hovercard plugin by PC 
//Documentation: http://designwithpc.com/Plugins/Hovercard
//Author: PC 
//Website: http://designwithpc.com
//Twitter: @chaudharyp

(function($) {
    $.fn.hovercard = function(options) {

        //Set defauls for the control
        var defaults = {
            width: 300,
            openOnLeft: false,
            openOnTop: false,
            cardImgSrc: "",
            detailsHTML: "",
            loadingHTML: "Loading...",
            errorHTML: "Sorry, no data found.",
            twitterURL: "",
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
            obj.closest(".hc-preview").hoverIntent(function() {

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

                        LoadSocialProfile("yaf", '', dataUrl, curHCDetails, options.customCardJSON);
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

                        LoadSocialProfile("twitter", obj.attr('href') + dataUrl, tUsername, curHCDetails);
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

                    if (typeof options.onHoverOut == 'function') {
                        options.onHoverOut.call(this);
                    }
                });
                
                $("body").off("keydown");
            }

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
                var cardHTML, dataType, urlToRequest, customCallback, loadingHTML, errorHTML;

                switch (type) {
                case "twitter":
                    {
                        dataType = 'json',
                        urlToRequest = options.twitterURL + username;
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
                        customCallback = function() {
                        };

                        //Append the twitter script to the document to add a follow button
                        if ($('#t-follow-script').length <= 0) {
                            var script = document.createElement('script');
                            script.type = 'text/javascript';
                            script.src = '//platform.twitter.com/widgets.js';
                            script.id = 't-follow-script';
                            $('body').append(script);
                        }
                        curHCDetails.append('<span class="s-action"><a href="https://twitter.com/' + username + '" class="twitter-follow-button" data-show-count="false" data-show-name="false" data-button="grey" data-width="65px" class="twitter-follow-button">Follow</a></span>');
						curHCDetails.append('<span class="s-action s-close"><a href="javascript:void(0)"></a></span>');
                    }
					
                    break;
                case "facebook":
                    {
                        dataType = 'json',
                        urlToRequest = 'https://graph.facebook.com/' + username,
                        cardHTML = function(profileData) {
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

                        customCallback = function(profileData) {
                            if ($('#fb-like' + profileData.id).length > 0) {
                                curHCDetails.append('<span class="s-action">' + $('#fb-like' + profileData.id).html() + '</span>');
                            } else {
                                curHCDetails.append('<span class="s-action"><div class="fb-like" id="fb-like' + profileData.id + '"><iframe src="//www.facebook.com/plugins/like.php?href=' + profileData.link + ';send=false&amp;layout=standard&amp;width=90&amp;show_faces=false&amp;action=like&amp;layout=button_count&amp;font&amp;height=21&amp" scrolling="no" frameborder="0" style="border:none; overflow:hidden;width:77px;height:21px" allowTransparency="true"></iframe></div></span>');
								curHCDetails.append('<span class="s-action s-close"><a href="javascript:void(0)"></a></span>');
                            }
                        };
                    }
                    break;
                case "custom":
                    {
                        dataType = 'jsonp',
                        urlToRequest = username,
                        cardHTML = function(profileData) {
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
                        customCallback = function() {
                        };
                    }
                    break;
                case "yaf":
                    {
                        dataType = 'json',
                        urlToRequest = username,
                        cardHTML = function(profileData) {

                            var online = (profileData.online ? ('border-left: 4px solid green') : 'border-left: 4px solid red');

                            return '<div class="s-card s-card-pad">' +
                                (profileData.Avatar ? ('<img class="s-img" style="' + online + '" src=' + profileData.Avatar + ' />') : '') +
                                (profileData.RealName ? ('<label class="s-name">' + profileData.RealName + ' </label>') : ('<label class="s-name">' + profileData.Name + ' </label>')) +
                                (href ? ('(<a class="s-username" title="Visit full profile for ' + profileData.Name + '" href="' + href + '">' + profileData.Name + '</a>)<br/>') : '') +
                                (profileData.Location ? ('<label class="s-location">' + profileData.Location + '</label><br />') : '') +
                                (profileData.Rank ? ('<label class="s-rank">' + profileData.Rank + '</label>') : '') +
                                (profileData.Interests ? ('<p class="s-interests">' + profileData.Interests + '</p>') : '') +
                                (profileData.Joined ? ('<p class="s-joined"><span class="s-strong">Member since:</span><br/>' + profileData.Joined + '</p>') : '') +
                                (profileData.HomePage ? ('<a class="s-href" href="' + profileData.homepage + '">' + profileData.HomePage + '</a><br/>') : '') +
                                '<ul class="s-stats">' +
                                (profileData.Posts ? ('<li>Posts<br /><span class="s-posts">' + profileData.Posts + '</span></li>') : '') +
                                (profileData.Points ? ('<li>Reputation<br /><span class="s-points">' + profileData.Points + '</span></li>') : '') +
                                '</ul>' +
                                (profileData.ActionButtons ? ('<span class="s-action">' + profileData.ActionButtons + '</span>') : '') +
                                '</div>';
                        };
                        loadingHTML = options.loadingHTML;
                        errorHTML = options.errorHTML;
                        customCallback = function() {
                        };
						
						curHCDetails.append('<span class="s-action s-close"><a href="javascript:void(0)"></a></span>');
                    }
                    break;
                default:
                    {
                    }
                    break;
                }

                if ($.isEmptyObject(customCardJSON)) {
					$.ajax({
                        url: urlToRequest,
                        type: 'GET',
                        dataType: dataType, //jsonp for cross domain request
                        timeout: 6000, //timeout if cross domain request didn't respond, or failed silently
                        // crossDomain: true,
                        cache: true,
                        beforeSend: function() {
                            curHCDetails.find('.s-message').remove();
                            curHCDetails.append('<p class="s-message">' + loadingHTML + '</p>');
                        },
                        success: function(data) {
                            if (data.length <= 0) {

                                curHCDetails.find('.s-message').html(errorHTML);
                            } else {
                                curHCDetails.find('.s-message').replaceWith(cardHTML(data));
                                //curHCDetails.prepend(cardHTML(data));

                                $(".hc-details").hide();

                                adjustToViewPort(curHCDetails.closest('.hc-preview'));
                                curHCDetails.stop(true, true).delay(options.delay).fadeIn();
                                customCallback(data);
                            }
                        },
                        error: function(jqXHR, textStatus, errorThrown) {
                            curHCDetails.find('.s-message').html(errorHTML + errorThrown);
                        }
                    });
                } else {
                    curHCDetails.prepend(cardHTML(customCardJSON));
                }
            }

            ;
        });

    };
})(jQuery);

(function ($) {
    $.fn.hoverIntent = function (handlerIn, handlerOut, selector) {

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
        var track = function (ev) {
            cX = ev.pageX;
            cY = ev.pageY;
        };

        // A private function for comparing current and previous mouse position
        var compare = function (ev, ob) {
            ob.hoverIntent_t = clearTimeout(ob.hoverIntent_t);
            // compare mouse positions to see if they've crossed the threshold
            if ((Math.abs(pX - cX) + Math.abs(pY - cY)) < cfg.sensitivity) {
                $(ob).off("mousemove.hoverIntent", track);
                // set hoverIntent state to true (so mouseOut can be called)
                ob.hoverIntent_s = 1;
                return cfg.over.apply(ob, [ev]);
            } else {
                // set previous coordinates for next time
                pX = cX; pY = cY;
                // use self-calling timeout, guarantees intervals are spaced out properly (avoids JavaScript timer bugs)
                ob.hoverIntent_t = setTimeout(function () { compare(ev, ob); }, cfg.interval);
            }
        };

        // A private function for delaying the mouseOut function
        var delay = function (ev, ob) {
            ob.hoverIntent_t = clearTimeout(ob.hoverIntent_t);
            ob.hoverIntent_s = 0;
            return cfg.out.apply(ob, [ev]);
        };

        // A private function for handling mouse 'hovering'
        var handleHover = function (e) {
            // copy objects to be passed into t (required for event object to be passed in IE)
            var ev = jQuery.extend({}, e);
            var ob = this;

            // cancel hoverIntent timer if it exists
            if (ob.hoverIntent_t) { ob.hoverIntent_t = clearTimeout(ob.hoverIntent_t); }

            // if e.type == "mouseenter"
            if (e.type == "mouseenter") {
                // set "previous" X and Y position based on initial entry point
                pX = ev.pageX; pY = ev.pageY;
                // update "current" X and Y position based on mousemove
                $(ob).on("mousemove.hoverIntent", track);
                // start polling interval (self-calling timeout) to compare mouse coordinates over time
                if (ob.hoverIntent_s != 1) { ob.hoverIntent_t = setTimeout(function () { compare(ev, ob); }, cfg.interval); }

                // else e.type == "mouseleave"
            } else {
                // unbind expensive mousemove event
                $(ob).off("mousemove.hoverIntent", track);
                // if hoverIntent state is true, then call the mouseOut function after the specified delay
                if (ob.hoverIntent_s == 1) { ob.hoverIntent_t = setTimeout(function () { delay(ev, ob); }, cfg.timeout); }
            }
        };

        // listen for mouseenter and mouseleave
        return this.on({ 'mouseenter.hoverIntent': handleHover, 'mouseleave.hoverIntent': handleHover }, cfg.selector);
    };
})(jQuery);
// MSDropDown - jquery.dd.js
// author: Marghoob Suleman - http://www.marghoobsuleman.com/
// Date: 10 Nov, 2012 
// Version: 3.5
// Revision: 25
// web: www.marghoobsuleman.com
/*
// msDropDown is free jQuery Plugin: you can redistribute it and/or modify
// it under the terms of the either the MIT License or the Gnu General Public License (GPL) Version 2
*/
var msBeautify = msBeautify || {};
(function ($) {
	msBeautify = {
	version: {msDropdown:'3.5'},
	author: "Marghoob Suleman",
	counter: 20,
	debug: function (v) {
		if (v !== false) {
			$(".ddOutOfVision").css({height: 'auto', position: 'relative'});
		} else {
			$(".ddOutOfVision").css({height: '0px', position: 'absolute'});
		}
	},
	oldDiv: '',
	create: function (id, settings, type) {
		type = type || "dropdown";
		var data;
		switch (type.toLowerCase()) {
		case "dropdown":
		case "select":
			data = $(id).msDropdown(settings).data("dd");
			break;
		}
		return data;
	}
};

$.msDropDown = {}; //Legacy
$.msDropdown = {}; //camelCaps
$.extend(true, $.msDropDown, msBeautify);
$.extend(true, $.msDropdown, msBeautify);
// make compatibiliy with old and new jquery
if ($.fn.prop === undefined) {$.fn.prop = $.fn.attr;}
if ($.fn.on === undefined) {$.fn.on = $.fn.bind;$.fn.off = $.fn.unbind;}
if (typeof $.expr.createPseudo === 'function') {
	//jQuery 1.8  or greater
	$.expr[':'].Contains = $.expr.createPseudo(function (arg) {return function (elem) { return $(elem).text().toUpperCase().indexOf(arg.toUpperCase()) >= 0; }; });
} else {
	//lower version
	$.expr[':'].Contains = function (a, i, m) {return $(a).text().toUpperCase().indexOf(m[3].toUpperCase()) >= 0; };
}
//dropdown class
function dd(element, settings) {
	var settings = $.extend(true,
		{byJson: {data: null, selectedIndex: 0, name: null, size: 0, multiple: false, width: 250},
		mainCSS: 'dd',
		height: 120, //not using currently
		visibleRows: 7,
		rowHeight: 0,
		showIcon: true,
		zIndex: 9999,
		useSprite: false,
		animStyle: 'slideDown',
		event:'click',
		openDirection: 'auto', //auto || alwaysUp || alwaysDown
		jsonTitle: true,
		style: '',
		disabledOpacity: 0.7,
		disabledOptionEvents: true,
		childWidth:300,
		enableCheckbox:false, //this needs to multiple or it will set element to multiple
		checkboxNameSuffix:'_mscheck',
		append:'',
		prepend:'',
		reverseMode:true, //it will update the msdropdown UI/value if you update the original dropdown - will be usefull if are using knockout.js or playing with original dropdown
		roundedCorner:true,
		enableAutoFilter:true,
		on: {create: null,open: null,close: null,add: null,remove: null,change: null,blur: null,click: null,dblclick: null,mousemove: null,mouseover: null,mouseout: null,focus: null,mousedown: null,mouseup: null}
		}, settings);								  
	var $this = this; //this class	 
	var holderId = {postElementHolder: '_msddHolder', postID: '_msdd', postTitleID: '_title',postTitleTextID: '_titleText', postChildID: '_child'};
	var css = {dd:settings.mainCSS, ddTitle: 'ddTitle', arrow: 'arrow arrowoff', ddChild: 'ddChild', ddTitleText: 'ddTitleText',disabled: 'disabled', enabled: 'enabled', ddOutOfVision: 'ddOutOfVision', borderTop: 'borderTop', noBorderTop: 'noBorderTop', selected: 'selected', divider: 'divider', optgroup: "optgroup", optgroupTitle: "optgroupTitle", description: "description", label: "ddlabel",hover: 'hover',disabledAll: 'disabledAll'};
	var css_i = {li: '_msddli_',borderRadiusTp: 'borderRadiusTp',ddChildMore: 'border shadow',fnone: "fnone"};
	var isList = false, isMultiple=false,isDisabled=false, cacheElement = {}, element, orginial = {}, isOpen=false;
	var DOWN_ARROW = 40, UP_ARROW = 38, LEFT_ARROW=37, RIGHT_ARROW=39, ESCAPE = 27, ENTER = 13, ALPHABETS_START = 47, SHIFT=16, CONTROL = 17, BACKSPACE=8, DELETE=46;
	var shiftHolded=false, controlHolded=false,lastTarget=null,forcedTrigger=false, oldSelected, isCreated = false;
	var doc = document, ua = window.navigator.userAgent, isIE = ua.match(/msie/i);
	settings.reverseMode = settings.reverseMode.toString();
	settings.roundedCorner = settings.roundedCorner.toString();
	var msieversion = function()
   	{      
      var msie = ua.indexOf("MSIE");
      if ( msie > 0 ) {      // If Internet Explorer, return version number
         return parseInt (ua.substring (msie+5, ua.indexOf (".", msie)));
	  } else {                // If another browser, return 0
         return 0;
	  };
   	};
	var checkDataSetting = function() {
		settings.mainCSS = $("#"+element).data("maincss") || settings.mainCSS;
		settings.visibleRows = $("#"+element).data("visiblerows") || settings.visibleRows;
		if($("#"+element).data("showicon")==false) {settings.showIcon = $("#"+element).data("showicon");};
		settings.useSprite = $("#"+element).data("usesprite") || settings.useSprite;
		settings.animStyle = $("#"+element).data("animstyle") || settings.animStyle;
		settings.event = $("#"+element).data("event") || settings.event;
		settings.openDirection = $("#"+element).data("opendirection") || settings.openDirection;
		settings.jsonTitle = $("#"+element).data("jsontitle") || settings.jsonTitle;
		settings.disabledOpacity = $("#"+element).data("disabledopacity") || settings.disabledOpacity;
		settings.childWidth = $("#"+element).data("childwidth") || settings.childWidth;
		settings.enableCheckbox = $("#"+element).data("enablecheckbox") || settings.enableCheckbox;
		settings.checkboxNameSuffix = $("#"+element).data("checkboxnamesuffix") || settings.checkboxNameSuffix;
		settings.append = $("#"+element).data("append") || settings.append;
		settings.prepend = $("#"+element).data("prepend") || settings.prepend;
		settings.reverseMode = $("#"+element).data("reversemode") || settings.reverseMode;
		settings.roundedCorner = $("#"+element).data("roundedcorner") || settings.roundedCorner;
		settings.enableAutoFilter = $("#"+element).data("enableautofilter") || settings.enableAutoFilter;
		
		//make string
		settings.reverseMode = settings.reverseMode.toString();
		settings.roundedCorner = settings.roundedCorner.toString();
		settings.enableAutoFilter = settings.enableAutoFilter.toString();
	};	
	var getElement = function(ele) {
		if (cacheElement[ele] === undefined) {
			cacheElement[ele] = doc.getElementById(ele);
		}
		return cacheElement[ele];
	}; 	
	var getIndex = function(opt) {
		var childid = getPostID("postChildID"); 
		return $("#"+childid + " li."+css_i.li).index(opt);
	};
	var createByJson = function() {
		if (settings.byJson.data) {
				var validData = ["description","image","title"];
				try {
					if (!element.id) {
						element.id = "dropdown"+msBeautify.counter;
					};
					settings.byJson.data = eval(settings.byJson.data);
					//change element
					var id = "msdropdown"+(msBeautify.counter++);
					var obj = {};
					obj.id = id;
					obj.name = settings.byJson.name || element.id; //its name
					if (settings.byJson.size>0) {
						obj.size = settings.byJson.size;
					};
					obj.multiple = settings.byJson.multiple;
					var oSelect = createElement("select", obj);
					for(var i=0;i<settings.byJson.data.length;i++) {
						var current = settings.byJson.data[i];
						var opt = new Option(current.text, current.value);
						for(var p in current) { 
							if (p.toLowerCase() != 'text') { 
								var key = ($.inArray(p.toLowerCase(), validData)!=-1) ? "data-" : "";
								opt.setAttribute(key+p, current[p]);
							};
						};
						oSelect.options[i] = opt;
					};
					getElement(element.id).appendChild(oSelect);
					oSelect.selectedIndex = settings.byJson.selectedIndex;
					$(oSelect).css({width: settings.byJson.width+'px'});
					//now change element for access other things
					element = oSelect;
				} catch(e) {
					throw "There is an error in json data.";
				};
		};			
	};
	var init = function() {		
		 //set properties
		 createByJson();
		if (!element.id) {
			element.id = "msdrpdd"+(msBeautify.counter++);
		};						
		element = element.id;
		$this.element = element;
		checkDataSetting();		
		isDisabled = getElement(element).disabled;
		var useCheckbox = settings.enableCheckbox;
		if(useCheckbox.toString()==="true") {
			getElement(element).multiple = true;
			settings.enableCheckbox = true;
		};
		isList = (getElement(element).size>1 || getElement(element).multiple==true) ? true : false;
		//trace("isList "+isList);
		if (isList) {isMultiple = getElement(element).multiple;};			
		mergeAllProp();		
		//create layout
		createLayout();		
		//set ui prop
		updateProp("uiData", getDataAndUI());
		updateProp("selectedOptions", $("#"+element +" option:selected"));
		var childid = getPostID("postChildID");
		oldSelected = $("#" + childid + " li." + css.selected);
		
		if(settings.reverseMode==="true") {
			$("#"+element).on("change", function() {
				setValue(this.selectedIndex);
			});
		};
		//add refresh method
		getElement(element).refresh = function(e) {
			 $("#"+element).msDropdown().data("dd").refresh();
		};

	 };	
	 /********************************************************************************************/	
	var getPostID = function (id) {
		return element+holderId[id];
	};
	var getInternalStyle = function(ele) {		 
		 var s = (ele.style === undefined) ? "" : ele.style.cssText;
		 return s;
	};
	var parseOption = function(opt) {
		var imagePath = '', title ='', description='', value=-1, text='', className='', imagecss = '';
		if (opt !== undefined) {
			var attrTitle = opt.title || "";
			//data-title
			if (attrTitle!="") {
				var reg = /^\{.*\}$/;
				var isJson = reg.test(attrTitle);
				if (isJson && settings.jsonTitle) {
					var obj =  eval("["+attrTitle+"]");	
				};				 
				title = (isJson && settings.jsonTitle) ? obj[0].title : title;
				description = (isJson && settings.jsonTitle) ? obj[0].description : description;
				imagePath = (isJson && settings.jsonTitle) ? obj[0].image : attrTitle;
				imagecss = (isJson && settings.jsonTitle) ? obj[0].imagecss : imagecss;
			};

			text = opt.text || '';
			value = opt.value || '';
			className = opt.className || "";
			//ignore title attribute if playing with data tags			
			title = $(opt).prop("data-title") || $(opt).data("title") || (title || "");
			description = $(opt).prop("data-description") || $(opt).data("description") || (description || "");
			imagePath = $(opt).prop("data-image") || $(opt).data("image") || (imagePath || "");
			imagecss = $(opt).prop("data-imagecss") || $(opt).data("imagecss") || (imagecss || "");
			
		};
		var o = {image: imagePath, title: title, description: description, value: value, text: text, className: className, imagecss:imagecss};
		return o;
	};	 
	var createElement = function(nm, attr, html) {
		var tag = doc.createElement(nm);
		
		if (attr) {
		 for(var i in attr) {
			 switch(i) {
				 case "style":
					tag.style.cssText  = attr[i];
				 break;
				 default:
					tag[i]  = attr[i];
				 break;
			 };	
		 };
		};
		if (html) {
		 tag.innerHTML = html;
		};
		return tag;
	};
	 /********************************************************************************************/
	  /*********************** <layout> *************************************/
	var hideOriginal = function() {
		var hidid = getPostID("postElementHolder");
		if ($("#"+hidid).length==0) {			 
			var obj = {style: 'height: 0px;overflow: hidden;position: absolute;',className: css.ddOutOfVision};	
			obj.id = hidid;
			var oDiv = createElement("div", obj);	
			$("#"+element).after(oDiv);
			$("#"+element).appendTo($("#"+hidid));
		} else {
			$("#"+hidid).css({height: 0,overflow: 'hidden',position: 'absolute'});
		};
		getElement(element).tabIndex = -1;
	};
	var createWrapper = function () {
		var brdRds = (settings.roundedCorner == "true") ? " borderRadius" : "";
		var obj = {
			className: css.dd + " ddcommon"+brdRds
		};
		var intcss = getInternalStyle(getElement(element));

		var w = $("#" + element).outerWidth();
		
		if (w === 0) {
			obj.style = "width: auto";
		}
		else {
			obj.style = "width: " + w + "px;";
		}
		
		if (intcss.length > 0) {
			obj.style = obj.style + "" + intcss;
		};
		obj.id = getPostID("postID");
		obj.tabIndex = getElement(element).tabIndex;
		var oDiv = createElement("div", obj);
		return oDiv;
	};
	var createTitle = function () {
		var selectedOption;
		if(getElement(element).selectedIndex>=0) {
			selectedOption = getElement(element).options[getElement(element).selectedIndex];
		} else {
			selectedOption = {value:'', text:''};
		}
		var spriteClass = "", selectedClass = "";
		//check sprite
		var useSprite = $("#"+element).data("usesprite");
		if(useSprite) { settings.useSprite = useSprite; };
		if (settings.useSprite != false) {
			spriteClass = " " + settings.useSprite;
			selectedClass = " " + selectedOption.className;
		};
		var brdRdsTp = (settings.roundedCorner == "true") ? " "+css_i.borderRadiusTp : "" ;
		var oTitle = createElement("div", {className: css.ddTitle + spriteClass + brdRdsTp});
		//divider
		var oDivider = createElement("span", {className: css.divider});
		//arrow
		var oArrow = createElement("span", {className: css.arrow});
		//title Text
		var titleid = getPostID("postTitleID");
		var oTitleText = createElement("span", {className: css.ddTitleText + selectedClass, id: titleid});
	
		var parsed = parseOption(selectedOption);
		var arrowPath = parsed.image;
		var sText = parsed.text || "";		
		if (arrowPath != "" && settings.showIcon) {
			var oIcon = createElement("img");
			oIcon.src = arrowPath;
			if(parsed.imagecss!="") {
				oIcon.className = parsed.imagecss+" ";
			};
		};
		var oTitleText_in = createElement("span", {className: css.label}, sText);
		oTitle.appendChild(oDivider);
		oTitle.appendChild(oArrow);
		if (oIcon) {
			oTitleText.appendChild(oIcon);
		};
		oTitleText.appendChild(oTitleText_in);
		oTitle.appendChild(oTitleText);
		var oDescription = createElement("span", {className: css.description}, parsed.description);
		oTitleText.appendChild(oDescription);
		
		return oTitle;
	};
	var createFilterBox = function () {
		var tid = getPostID("postTitleTextID");
		var brdRds = (settings.roundedCorner == "true") ? "borderRadius" : "";
		var sText = createElement("input", {id: tid, type: 'text', value: '', autocomplete: 'off', className: 'text shadow '+brdRds, style: 'display: none'});
		return sText;
	};
	var createChild = function (opt) {
		var obj = {};
		var intcss = getInternalStyle(opt);
		if (intcss.length > 0) {obj.style = intcss; };
		var css2 = (opt.disabled) ? css.disabled : css.enabled;
		css2 = (opt.selected) ? (css2 + " " + css.selected) : css2;
		css2 = css2 + " " + css_i.li;
		obj.className = css2;
		if (settings.useSprite != false) {
			obj.className = css2 + " " + opt.className;
		};
		var li = createElement("li", obj);
		var parsed = parseOption(opt);
		if (parsed.title != "") {
			li.title = parsed.title;
		};
		var arrowPath = parsed.image;
		if (arrowPath != "" && settings.showIcon) {
			var oIcon = createElement("img");
			oIcon.src = arrowPath;
			if(parsed.imagecss!="") {
				oIcon.className = parsed.imagecss+" ";
			};
		};
		if (parsed.description != "") {
			var oDescription = createElement("span", {
				className: css.description
			}, parsed.description);
		};
		var sText = opt.text || "";
		var oTitleText = createElement("span", {
			className: css.label
		}, sText);
		//checkbox
		if(settings.enableCheckbox===true) {
			var chkbox = createElement("input", {
			type: 'checkbox', name:element+settings.checkboxNameSuffix+'[]', value:opt.value||""}); //this can be used for future
			li.appendChild(chkbox);
			if(settings.enableCheckbox===true) {
				chkbox.checked = (opt.selected) ? true : false;
			};
		};
		if (oIcon) {
			li.appendChild(oIcon);
		};
		li.appendChild(oTitleText);
		if (oDescription) {
			li.appendChild(oDescription);
		} else {
			if (oIcon) {
				oIcon.className = oIcon.className+css_i.fnone;
			};
		};
		var oClear = createElement("div", {className: 'clear'});
		li.appendChild(oClear);
		return li;
	};
	var createChildren = function () {
		var childid = getPostID("postChildID");
		var obj = {className: css.ddChild + " ddchild_ " + css_i.ddChildMore, id: childid};
		if (isList == false) {
			obj.style = "z-index: " + settings.zIndex;
		} else {
			obj.style = "z-index:1";
		};
		var childWidth = $("#"+element).data("childwidth") || settings.childWidth;
		if(childWidth) {
			obj.style =  (obj.style || "") + ";width:"+childWidth;
		};		
		var oDiv = createElement("div", obj);
		var ul = createElement("ul");
		if (settings.useSprite != false) {
			ul.className = settings.useSprite;
		};
		var allOptions = getElement(element).children;
		for (var i = 0; i < allOptions.length; i++) {
			var current = allOptions[i];
			var li;
			if (current.nodeName.toLowerCase() == "optgroup") {
				//create ul
				li = createElement("li", {className: css.optgroup});
				var span = createElement("span", {className: css.optgroupTitle}, current.label);
				li.appendChild(span);
				var optChildren = current.children;
				var optul = createElement("ul");
				for (var j = 0; j < optChildren.length; j++) {
					var opt_li = createChild(optChildren[j]);
					optul.appendChild(opt_li);
				};
				li.appendChild(optul);
			} else {
				li = createChild(current);
			};
			ul.appendChild(li);
		};
		oDiv.appendChild(ul);		
		return oDiv;
	};
	var childHeight = function (val) {
		var childid = getPostID("postChildID");
		if (val) {
			if (val == -1) { //auto
				$("#"+childid).css({height: "auto", overflow: "auto"});
			} else {				
				$("#"+childid).css("height", val+"px");
			};
			return false;
		};
		//else return height
		var iHeight;
		if (getElement(element).options.length > settings.visibleRows) {
			var margin = parseInt($("#" + childid + " li:first").css("padding-bottom")) + parseInt($("#" + childid + " li:first").css("padding-top"));
			if(settings.rowHeight===0) {
				$("#" + childid).css({visibility:'hidden',display:'block'}); //hack for first child
				settings.rowHeight = Math.round($("#" + childid + " li:first").height());
				$("#" + childid).css({visibility:'visible'});
				if(!isList || settings.enableCheckbox===true) {
					$("#" + childid).css({display:'none'});
				};
			};
			iHeight = ((settings.rowHeight + margin) * settings.visibleRows);
		} else if (isList) {
			iHeight = $("#" + element).height(); //get height from original element
		};		
		return iHeight;
	};
	var applyChildEvents = function () {
		var childid = getPostID("postChildID");
		$("#" + childid).on("click", function (e) {
			if (isDisabled === true) return false;
			//prevent body click
			e.preventDefault();
			e.stopPropagation();
			if (isList) {
				bind_on_events();
			};
		});
		$("#" + childid + " li." + css.enabled).on("click", function (e) {
			if(e.target.nodeName.toLowerCase() !== "input") {
				close(this);
			};
		});
		$("#" + childid + " li." + css.enabled).on("mousedown", function (e) {
			if (isDisabled === true) return false;
			oldSelected = $("#" + childid + " li." + css.selected);
			lastTarget = this;
			e.preventDefault();
			e.stopPropagation();
			//select current input
			if(settings.enableCheckbox===true) {
				if(e.target.nodeName.toLowerCase() === "input") {
					controlHolded = true;
				};	
			};
			if (isList === true) {
				if (isMultiple) {					
					if (shiftHolded === true) {
						$(this).addClass(css.selected);
						var selected = $("#" + childid + " li." + css.selected);
						var lastIndex = getIndex(this);
						if (selected.length > 1) {
							var items = $("#" + childid + " li." + css_i.li);
							var ind1 = getIndex(selected[0]);
							var ind2 = getIndex(selected[1]);
							if (lastIndex > ind2) {
								ind1 = (lastIndex);
								ind2 = ind2 + 1;
							};
							for (var i = Math.min(ind1, ind2); i <= Math.max(ind1, ind2); i++) {
								var current = items[i];
								if ($(current).hasClass(css.enabled)) {
									$(current).addClass(css.selected);
								};
							};
						};
					} else if (controlHolded === true) {
						$(this).toggleClass(css.selected); //toggle
						if(settings.enableCheckbox===true) {
							var checkbox = this.childNodes[0];
							checkbox.checked = !checkbox.checked; //toggle
						};
					} else {
						$("#" + childid + " li." + css.selected).removeClass(css.selected);
						$("#" + childid + " input:checkbox").prop("checked", false);
						$(this).addClass(css.selected);
						if(settings.enableCheckbox===true) {
							this.childNodes[0].checked = true;
						};
					};					
				} else {
					$("#" + childid + " li." + css.selected).removeClass(css.selected);
					$(this).addClass(css.selected);
				};
				//fire event on mouseup
			} else {
				$("#" + childid + " li." + css.selected).removeClass(css.selected);
				$(this).addClass(css.selected);
			};		
		});
		$("#" + childid + " li." + css.enabled).on("mouseenter", function (e) {
			if (isDisabled === true) return false;
			e.preventDefault();
			e.stopPropagation();
			if (lastTarget != null) {
				if (isMultiple) {
					$(this).addClass(css.selected);
					if(settings.enableCheckbox===true) {
						this.childNodes[0].checked = true;
					};
				};
			};
		});
	
		$("#" + childid + " li." + css.enabled).on("mouseover", function (e) {
			if (isDisabled === true) return false;
			$(this).addClass(css.hover);
		});
		$("#" + childid + " li." + css.enabled).on("mouseout", function (e) {
			if (isDisabled === true) return false;
			$("#" + childid + " li." + css.hover).removeClass(css.hover);
		});
	
		$("#" + childid + " li." + css.enabled).on("mouseup", function (e) {
			if (isDisabled === true) return false;
			e.preventDefault();
			e.stopPropagation();
			if(settings.enableCheckbox===true) {
				controlHolded = false;
			};
			var selected = $("#" + childid + " li." + css.selected).length;			
			forcedTrigger = (oldSelected.length != selected || selected == 0) ? true : false;	
			fireAfterItemClicked();
			unbind_on_events(); //remove old one
			bind_on_events();
			lastTarget = null;
		});
	
		/* options events */
		if (settings.disabledOptionEvents == false) {
			$("#" + childid + " li." + css_i.li).on("click", function (e) {
				if (isDisabled === true) return false;
				fireOptionEventIfExist(this, "click");
			});
			$("#" + childid + " li." + css_i.li).on("mouseenter", function (e) {
				if (isDisabled === true) return false;
				fireOptionEventIfExist(this, "mouseenter");
			});
			$("#" + childid + " li." + css_i.li).on("mouseover", function (e) {
				if (isDisabled === true) return false;
				fireOptionEventIfExist(this, "mouseover");
			});
			$("#" + childid + " li." + css_i.li).on("mouseout", function (e) {
				if (isDisabled === true) return false;
				fireOptionEventIfExist(this, "mouseout");
			});
			$("#" + childid + " li." + css_i.li).on("mousedown", function (e) {
				if (isDisabled === true) return false;
				fireOptionEventIfExist(this, "mousedown");
			});
			$("#" + childid + " li." + css_i.li).on("mouseup", function (e) {
				if (isDisabled === true) return false;
				fireOptionEventIfExist(this, "mouseup");
			});
		};
	};
	var removeChildEvents = function () {
		var childid = getPostID("postChildID");
		$("#" + childid).off("click");
		$("#" + childid + " li." + css.enabled).off("mouseenter");
		$("#" + childid + " li." + css.enabled).off("click");
		$("#" + childid + " li." + css.enabled).off("mouseover");
		$("#" + childid + " li." + css.enabled).off("mouseout");
		$("#" + childid + " li." + css.enabled).off("mousedown");
		$("#" + childid + " li." + css.enabled).off("mouseup");
	};
	var triggerBypassingHandler = function (id, evt_n, handler) {
		$("#" + id).off(evt_n, handler);
		$("#" + id).trigger(evt_n);
		$("#" + id).on(evt_n, handler);
	};
	var applyEvents = function () {
		var id = getPostID("postID");
		var tid = getPostID("postTitleTextID");
		var childid = getPostID("postChildID");		
		$("#" + id).on(settings.event, function (e) {			
			if (isDisabled === true) return false;
			fireEventIfExist(settings.event);
			//prevent body click
			e.preventDefault();
			e.stopPropagation();
			open(e);
		});
		$("#" + id).on("keydown", function (e) {
			var k = e.which;
			if (!isOpen && (k == ENTER || k == UP_ARROW || k == DOWN_ARROW ||
				k == LEFT_ARROW || k == RIGHT_ARROW ||
				(k >= ALPHABETS_START && !isList))) {
				open(e);
				if (k >= ALPHABETS_START) {
					showFilterBox();
				} else {
					e.preventDefault();
					e.stopImmediatePropagation();
				};
			};
		});
		$("#" + id).on("focus", wrapperFocusHandler);
		$("#" + id).on("blur", wrapperBlurHandler);
		$("#" + tid).on("blur", function (e) {
			//return focus to the wrapper without triggering the handler
			triggerBypassingHandler(id, "focus", wrapperFocusHandler);
		});
		applyChildEvents();		
		$("#" + id).on("dblclick", on_dblclick);
		$("#" + id).on("mousemove", on_mousemove);
		$("#" + id).on("mouseenter", on_mouseover);
		$("#" + id).on("mouseleave", on_mouseout);
		$("#" + id).on("mousedown", on_mousedown);
		$("#" + id).on("mouseup", on_mouseup);
	};
	var wrapperFocusHandler = function (e) {
		fireEventIfExist("focus");
	};
	var wrapperBlurHandler = function (e) {
		fireEventIfExist("blur");
	};
	//after create
	var fixedForList = function () {
		var id = getPostID("postID");
		var childid = getPostID("postChildID");		
		if (isList === true && settings.enableCheckbox===false) {
			$("#" + id + " ." + css.ddTitle).hide();
			$("#" + childid).css({display: 'block', position: 'relative'});	
			//open();
		} else {
			if(settings.enableCheckbox===false) {
				isMultiple = false; //set multiple off if this is not a list
			};
			$("#" + id + " ." + css.ddTitle).show();
			$("#" + childid).css({display: 'none', position: 'absolute', width: 'auto'});
			//set value
			var first = $("#" + childid + " li." + css.selected)[0];
			$("#" + childid + " li." + css.selected).removeClass(css.selected);
			var index = getIndex($(first).addClass(css.selected));
			setValue(index);
		};
		childHeight(childHeight()); //get and set height 
	};
	var fixedForDisabled = function () {
		var id = getPostID("postID");
		var opc = (isDisabled == true) ? settings.disabledOpacity : 1;
		if (isDisabled === true) {
			$("#" + id).addClass(css.disabledAll);
		} else {
			$("#" + id).removeClass(css.disabledAll);
		};
	};
	var fixedSomeUI = function () {
		//auto filter
		var tid = getPostID("postTitleTextID");
		if(settings.enableAutoFilter=="true") {
			$("#" + tid).on("keyup", applyFilters);
		};
		//if is list
		fixedForList();
		fixedForDisabled();
	};
	var createLayout = function () {		
		var oDiv = createWrapper();
		var oTitle = createTitle();
		oDiv.appendChild(oTitle);
		//auto filter box
		var oFilterBox = createFilterBox();
		oDiv.appendChild(oFilterBox);
	
		var oChildren = createChildren();
		oDiv.appendChild(oChildren);
		$("#" + element).after(oDiv);

		hideOriginal(); //hideOriginal
		fixedSomeUI();
		applyEvents();
		
		var childid = getPostID("postChildID");
		//append
		if(settings.append!='') {
			$("#" + childid).append(settings.append);
		};
		//prepend
		if(settings.prepend!='') {
			$("#" + childid).prepend(settings.prepend);
		};		
		if (typeof settings.on.create == "function") {
			settings.on.create.apply($this, arguments);
		};
	};
	var selectMutipleOptions = function (bySelected) {
		var childid = getPostID("postChildID");
		var selected = bySelected || $("#" + childid + " li." + css.selected); //bySelected or by argument
		for (var i = 0; i < selected.length; i++) {
			var ind = getIndex(selected[i]);
			getElement(element).options[ind].selected = "selected";
		};
		setValue(selected);
	};
	var fireAfterItemClicked = function () {
		//console.log("fireAfterItemClicked")
		var childid = getPostID("postChildID");
		var selected = $("#" + childid + " li." + css.selected);		
		if (isMultiple && (shiftHolded || controlHolded) || forcedTrigger) {
			getElement(element).selectedIndex = -1; //reset old
		};
		var index;
		if (selected.length == 0) {
			index = -1;
		} else if (selected.length > 1) {
			//selected multiple
			selectMutipleOptions(selected);
			//index = $("#" + childid + " li." + css.selected);
			
		} else {
			//if one selected
			index = getIndex($("#" + childid + " li." + css.selected));
		};		
		if ((getElement(element).selectedIndex != index || forcedTrigger) && selected.length<=1) {			
			forcedTrigger = false;			
			var evt = has_handler("change");
			getElement(element).selectedIndex = index;	
			setValue(index);
			//local
			if (typeof settings.on.change == "function") {
				var d = getDataAndUI();
				settings.on.change(d.data, d.ui);
			};			
			$("#" + element).trigger("change");			
		};
	};
	var setValue = function (index, byvalue) {
		if (index !== undefined) {
			var selectedIndex, value, selectedText;
			if (index == -1) {
				selectedIndex = -1;
				value = "";
				selectedText = "";
				updateTitleUI(-1);
			} else {
				//by index or byvalue
				if (typeof index != "object") {
					var opt = getElement(element).options[index];
					getElement(element).selectedIndex = index;
					selectedIndex = index;
					value = parseOption(opt);
					selectedText = (index >= 0) ? getElement(element).options[index].text : "";
					updateTitleUI(undefined, value);
					value = value.value; //for bottom
				} else {
					//this is multiple or by option
					selectedIndex = (byvalue && byvalue.index) || getElement(element).selectedIndex;
					value = (byvalue && byvalue.value) || getElement(element).value;
					selectedText = (byvalue && byvalue.text) || getElement(element).options[getElement(element).selectedIndex].text || "";
					updateTitleUI(selectedIndex);
				};
			};
			updateProp("selectedIndex", selectedIndex);
			updateProp("value", value);
			updateProp("selectedText", selectedText);
			updateProp("children", getElement(element).children);
			updateProp("uiData", getDataAndUI());
			updateProp("selectedOptions", $("#" + element + " option:selected"));
		};
	};
	var has_handler = function (name) {
		//True if a handler has been added in the html.
		var evt = {byElement: false, byJQuery: false, hasEvent: false};
		var obj = $("#" + element);
		//console.log(name)
		try {
			//console.log(obj.prop("on" + name) + " "+name);
			if (obj.prop("on" + name) !== null) {
				evt.hasEvent = true;
				evt.byElement = true;
			};
		} catch(e) {
			//console.log(e.message);
		}
		// True if a handler has been added using jQuery.
		var evs;
		if (typeof $._data == "function") { //1.8
			evs = $._data(obj[0], "events");
		} else {
			evs = obj.data("events");
		};
		if (evs && evs[name]) {
			evt.hasEvent = true;
			evt.byJQuery = true;
		};
		return evt;
	};
	var bind_on_events = function () {
		unbind_on_events();
		$("body").on("click", close);
		//bind more events		 
		$(document).on("keydown", on_keydown);
		$(document).on("keyup", on_keyup);
		//focus will work on this	 		 
	};
	var unbind_on_events = function () {
		$("body").off("click", close);
		//bind more events
		$(document).off("keydown", on_keydown);
		$(document).off("keyup", on_keyup);
	};
	var applyFilters = function (e) {
		if(e.keyCode < ALPHABETS_START && e.keyCode!=BACKSPACE && e.keyCode!=DELETE) {
			return false;
		};
		var childid = getPostID("postChildID");
		var tid = getPostID("postTitleTextID");
		var sText = getElement(tid).value;
		if (sText.length == 0) {
			$("#" + childid + " li:hidden").show(); //show if hidden
			childHeight(childHeight());
		} else {
			$("#" + childid + " li").hide();
			var items = $("#" + childid + " li:Contains('" + sText + "')").show();
			if ($("#" + childid + " li:visible").length <= settings.visibleRows) {
				childHeight(-1); //set autoheight
			};
			if (items.length > 0 && !isList || !isMultiple) {
				$("#" + childid + " ." + css.selected).removeClass(css.selected);
				$(items[0]).addClass(css.selected);
			};	
		};		
		if (!isList) {
			adjustOpen();
		};
	};
	var showFilterBox = function () {
		if(settings.enableAutoFilter=="true") {
			var id = getPostID("postID");
			var tid = getPostID("postTitleTextID");
			if ($("#" + tid + ":hidden").length > 0 && controlHolded == false) {
				$("#" + tid + ":hidden").show().val("");
				//blur the wrapper without triggering the handler
				triggerBypassingHandler(id, "blur", wrapperBlurHandler);
				getElement(tid).focus();
			};
		};
	};
	var hideFilterBox = function () {
		var tid = getPostID("postTitleTextID");
		if ($("#" + tid + ":visible").length > 0) {
			$("#" + tid + ":visible").hide();
			getElement(tid).blur();
		};
	};
	var on_keydown = function (evt) {
		var tid = getPostID("postTitleTextID");
		var childid = getPostID("postChildID");
		switch (evt.keyCode) {
			case DOWN_ARROW:
			case RIGHT_ARROW:
				evt.preventDefault();
				evt.stopPropagation();
				//hideFilterBox();
				next();
				break;
			case UP_ARROW:
			case LEFT_ARROW:
				evt.preventDefault();
				evt.stopPropagation();
				//hideFilterBox();
				previous();
				break;
			case ESCAPE:
			case ENTER:
				evt.preventDefault();
				evt.stopPropagation();
				close();
				var selected = $("#" + childid + " li." + css.selected).length;	
				forcedTrigger = (oldSelected.length != selected || selected == 0) ? true : false;				
				fireAfterItemClicked();
				unbind_on_events(); //remove old one				
				lastTarget = null;			
				break;
			case SHIFT:
				shiftHolded = true;
				break;
			case CONTROL:
				controlHolded = true;
				break;
			default:
				if (evt.keyCode >= ALPHABETS_START && isList === false) {
					showFilterBox();
				};
				break;
		};
		if (isDisabled === true) return false;
		fireEventIfExist("keydown");
	};
	var on_keyup = function (evt) {
		switch (evt.keyCode) {
			case SHIFT:
				shiftHolded = false;
				break;
			case CONTROL:
				controlHolded = false;
				break;
		};
		if (isDisabled === true) return false;
		fireEventIfExist("keyup");
	};
	var on_dblclick = function (evt) {
		if (isDisabled === true) return false;
		fireEventIfExist("dblclick");
	};
	var on_mousemove = function (evt) {
		if (isDisabled === true) return false;
		fireEventIfExist("mousemove");
	};
	
	var on_mouseover = function (evt) {
		if (isDisabled === true) return false;
		evt.preventDefault();
		fireEventIfExist("mouseover");
	};
	var on_mouseout = function (evt) {
		if (isDisabled === true) return false;
		evt.preventDefault();
		fireEventIfExist("mouseout");
	};
	var on_mousedown = function (evt) {
		if (isDisabled === true) return false;
		fireEventIfExist("mousedown");
	};
	var on_mouseup = function (evt) {
		if (isDisabled === true) return false;
		fireEventIfExist("mouseup");
	};
	var option_has_handler = function (opt, name) {
		//True if a handler has been added in the html.
		var evt = {byElement: false, byJQuery: false, hasEvent: false};
		if ($(opt).prop("on" + name) != undefined) {
			evt.hasEvent = true;
			evt.byElement = true;
		};
		// True if a handler has been added using jQuery.
		var evs = $(opt).data("events");
		if (evs && evs[name]) {
			evt.hasEvent = true;
			evt.byJQuery = true;
		};
		return evt;
	};
	var fireOptionEventIfExist = function (li, evt_n) {
		if (settings.disabledOptionEvents == false) {
			var opt = getElement(element).options[getIndex(li)];
			//check if original has some
			if (option_has_handler(opt, evt_n).hasEvent === true) {
				if (option_has_handler(opt, evt_n).byElement === true) {
					opt["on" + evt_n]();
				};
				if (option_has_handler(opt, evt_n).byJQuery === true) {
					switch (evt_n) {
						case "keydown":
						case "keyup":
							//key down/up will check later
							break;
						default:
							$(opt).trigger(evt_n);
							break;
					};
				};
				return false;
			};
		};
	};
	var fireEventIfExist = function (evt_n) {
		//local
		if (typeof settings.on[evt_n] == "function") {
			settings.on[evt_n].apply(this, arguments);
		};
		//check if original has some
		if (has_handler(evt_n).hasEvent === true) {
			if (has_handler(evt_n).byElement === true) {
				getElement(element)["on" + evt_n]();
			} else if (has_handler(evt_n).byJQuery === true) {
				switch (evt_n) {
					case "keydown":
					case "keyup":
						//key down/up will check later
						break;
					default:
						$("#" + element).triggerHandler(evt_n);
						break;
				};
			};
			return false;
		};
	};
	/******************************* navigation **********************************************/
	var scrollToIfNeeded = function (opt) {
		var childid = getPostID("postChildID");
		//if scroll is needed
		opt = (opt !== undefined) ? opt : $("#" + childid + " li." + css.selected);
		if (opt.length > 0) {
			var pos = parseInt(($(opt).position().top));
			var ch = parseInt($("#" + childid).height());
			if (pos > ch) {
				var top = pos + $("#" + childid).scrollTop() - (ch/2);
				$("#" + childid).animate({scrollTop:top}, 500);
			};
		};
	};
	var next = function () {
		var childid = getPostID("postChildID");
		var items = $("#" + childid + " li:visible." + css_i.li);
		var selected = $("#" + childid + " li:visible." + css.selected);
		selected = (selected.length==0) ? items[0] : selected;
		var index = $("#" + childid + " li:visible." + css_i.li).index(selected);
		if ((index < items.length - 1)) {
			index = getNext(index);
			if (index < items.length) { //check again - hack for last disabled 
				if (!shiftHolded || !isList || !isMultiple) {
					$("#" + childid + " ." + css.selected).removeClass(css.selected);
				};
				$(items[index]).addClass(css.selected);
				updateTitleUI(index);
				if (isList == true) {
					fireAfterItemClicked();
				};
				scrollToIfNeeded($(items[index]));
			};
			if (!isList) {
				adjustOpen();
			};
		};	
		function getNext(ind) {
			ind = ind + 1;
			if (ind > items.length) {
				return ind;
			};
			if ($(items[ind]).hasClass(css.enabled) === true) {
				return ind;
			};
			return ind = getNext(ind);
		};
	};
	var previous = function () {
		var childid = getPostID("postChildID");
		var selected = $("#" + childid + " li:visible." + css.selected);
		var items = $("#" + childid + " li:visible." + css_i.li);
		var index = $("#" + childid + " li:visible." + css_i.li).index(selected[0]);
		if (index >= 0) {
			index = getPrev(index);
			if (index >= 0) { //check again - hack for disabled 
				if (!shiftHolded || !isList || !isMultiple) {
					$("#" + childid + " ." + css.selected).removeClass(css.selected);
				};
				$(items[index]).addClass(css.selected);
				updateTitleUI(index);
				if (isList == true) {
					fireAfterItemClicked();
				};
				if (parseInt(($(items[index]).position().top + $(items[index]).height())) <= 0) {
					var top = ($("#" + childid).scrollTop() - $("#" + childid).height()) - $(items[index]).height();
					$("#" + childid).animate({scrollTop: top}, 500);
				};
			};
			if (!isList) {
				adjustOpen();
			};
		};
	
		function getPrev(ind) {
			ind = ind - 1;
			if (ind < 0) {
				return ind;
			};
			if ($(items[ind]).hasClass(css.enabled) === true) {
				return ind;
			};
			return ind = getPrev(ind);
		};
	};
	var adjustOpen = function () {
		var id = getPostID("postID");
		var childid = getPostID("postChildID");
		var pos = $("#" + id).offset();
		var mH = $("#" + id).height();
		var wH = $(window).height();
		var st = $(window).scrollTop();
		var cH = $("#" + childid).height();
		var top = $("#" + id).height(); //this close so its title height
		var direction = settings.openDirection.toLowerCase();
		if (((wH + st) < Math.floor(cH + mH + pos.top) || direction == 'alwaysup') && direction != 'alwaysdown') {
			top = cH;
			$("#" + childid).css({top: "-" + top + "px", display: 'block', zIndex: settings.zIndex});			
			if(settings.roundedCorner == "true") {
				$("#" + id).removeClass("borderRadius borderRadiusTp").addClass("borderRadiusBtm");
			};
			var top = $("#" + childid).offset().top;
			if (top < -10) {
				$("#" + childid).css({top: (parseInt($("#" + childid).css("top")) - top + 20 + st) + "px", zIndex: settings.zIndex});
				if(settings.roundedCorner == "true") {
					$("#" + id).removeClass("borderRadiusBtm borderRadiusTp").addClass("borderRadius");
				};
			};
		} else {
			$("#" + childid).css({top: top + "px", zIndex: settings.zIndex});			
			if(settings.roundedCorner == "true") {
				$("#" + id).removeClass("borderRadius borderRadiusBtm").addClass("borderRadiusTp");
			};
		};
		//hack for ie zindex
		//i hate ie :D
		if(isIE) {
			if(msieversion()<=7) {
				$('div.ddcommon').css("zIndex", settings.zIndex-10);
				$("#" + id).css("zIndex", settings.zIndex+5);
			};
		};		
	};
	var open = function (e) {
		if (isDisabled === true) return false;
		var id = getPostID("postID");
		var childid = getPostID("postChildID");
		if (!isOpen) {
			isOpen = true;
			if (msBeautify.oldDiv != '') {
				$("#" + msBeautify.oldDiv).css({display: "none"}); //hide all 
			};
			msBeautify.oldDiv = childid;
			$("#" + childid + " li:hidden").show(); //show if hidden
			adjustOpen();
			var animStyle = settings.animStyle;
			if(animStyle=="" || animStyle=="none") {
				$("#" + childid).css({display:"block"});
				scrollToIfNeeded();
				if (typeof settings.on.open == "function") {
					var d = getDataAndUI();
					settings.on.open(d.data, d.ui);
				};
			} else {				
				$("#" + childid)[animStyle]("fast", function () {
					scrollToIfNeeded();
					if (typeof settings.on.open == "function") {
						var d = getDataAndUI();
						settings.on.open(d.data, d.ui);
					};
				});
			};
			bind_on_events();
		} else {
			if(settings.event!=='mouseover') {
				close();
			};
		};
	};
	var close = function (e) {
		isOpen = false;
		var id = getPostID("postID");
		var childid = getPostID("postChildID");
		if (isList === false || settings.enableCheckbox===true) {
			$("#" + childid).css({display: "none"});			
			if(settings.roundedCorner == "true") {
				$("#" + id).removeClass("borderRadiusTp borderRadiusBtm").addClass("borderRadius");
			};
		};
		unbind_on_events();
		if (typeof settings.on.close == "function") {
			var d = getDataAndUI();
			settings.on.close(d.data, d.ui);
		};
		//rest some old stuff
		hideFilterBox();
		childHeight(childHeight()); //its needed after filter applied
		$("#" + childid).css({zIndex:1});
		//update the title in case the user clicked outside
		updateTitleUI(getElement(element).selectedIndex);
	};
	/*********************** </layout> *************************************/	
	var mergeAllProp = function () {
		try {
			orginial = $.extend(true, {}, getElement(element));
			for (var i in orginial) {
				if (typeof orginial[i] != "function") {				
					$this[i] = orginial[i]; //properties
				};
			};
		} catch(e) {
			//silent
		};
		$this.selectedText = (getElement(element).selectedIndex >= 0) ? getElement(element).options[getElement(element).selectedIndex].text : "";		
		$this.version = msBeautify.version.msDropdown;
		$this.author = msBeautify.author;
	};
	var getDataAndUIByOption = function (opt) {
		if (opt != null && typeof opt != "undefined") {
			var childid = getPostID("postChildID");
			var data = parseOption(opt);
			var ui = $("#" + childid + " li." + css_i.li + ":eq(" + (opt.index) + ")");
			return {data: data, ui: ui, option: opt, index: opt.index};
		};
		return null;
	};
	var getDataAndUI = function () {
		var childid = getPostID("postChildID");
		var ele = getElement(element);
		var data, ui, option, index;
		if (ele.selectedIndex == -1) {
			data = null;
			ui = null;
			option = null;
			index = -1;
		} else {
			ui = $("#" + childid + " li." + css.selected);
			if (ui.length > 1) {
				var d = [], op = [], ind = [];
				for (var i = 0; i < ui.length; i++) {
					var pd = getIndex(ui[i]);
					d.push(pd);
					op.push(ele.options[pd]);
				};
				data = d;
				option = op;
				index = d;
			} else {
				option = ele.options[ele.selectedIndex];
				data = parseOption(option);
				index = ele.selectedIndex;
			};
		};
		return {data: data, ui: ui, index: index, option: option};
	};
	var updateTitleUI = function (index, byvalue) {
		var titleid = getPostID("postTitleID");
		var value = {};
		if (index == -1) {
			value.text = "&nbsp;";
			value.className = "";
			value.description = "";
			value.image = "";
		} else if (typeof index != "undefined") {
			var opt = getElement(element).options[index];
			value = parseOption(opt);
		} else {
			value = byvalue;
		};
		//update title and current
		$("#" + titleid).find("." + css.label).html(value.text);
		getElement(titleid).className = css.ddTitleText + " " + value.className;
		//update desction		 
		if (value.description != "") {
			$("#" + titleid).find("." + css.description).html(value.description).show();
		} else {
			$("#" + titleid).find("." + css.description).html("").hide();
		};
		//update icon
		var img = $("#" + titleid).find("img");
		if (img.length > 0) {
			$(img).remove();
		};
		if (value.image != "" && settings.showIcon) {
			img = createElement("img", {src: value.image});
			$("#" + titleid).prepend(img);
			if(value.imagecss!="") {
				img.className = value.imagecss+" ";
			};
			if (value.description == "") {
				img.className = img.className+css_i.fnone;
			};
		};
	};
	var updateProp = function (p, v) {
		$this[p] = v;
	};
	var updateUI = function (a, opt, i) { //action, index, opt
		var childid = getPostID("postChildID");
		var wasSelected = false;
		switch (a) {
			case "add":
				var li = createChild(opt || getElement(element).options[i]);				
				var index;
				if (arguments.length == 3) {
					index = i;
				} else {
					index = $("#" + childid + " li." + css_i.li).length - 1;
				};				
				if (index < 0 || !index) {
					$("#" + childid + " ul").append(li);
				} else {
					var at = $("#" + childid + " li." + css_i.li)[index];
					$(at).before(li);
				};
				removeChildEvents();
				applyChildEvents();
				if (settings.on.add != null) {
					settings.on.add.apply(this, arguments);
				};
				break;
			case "remove":
				wasSelected = $($("#" + childid + " li." + css_i.li)[i]).hasClass(css.selected);
				$("#" + childid + " li." + css_i.li + ":eq(" + i + ")").remove();
				var items = $("#" + childid + " li." + css.enabled);
				if (wasSelected == true) {
					if (items.length > 0) {
						$(items[0]).addClass(css.selected);
						var ind = $("#" + childid + " li." + css_i.li).index(items[0]);
						setValue(ind);
					};
				};
				if (items.length == 0) {
					setValue(-1);
				};
				if ($("#" + childid + " li." + css_i.li).length < settings.visibleRows && !isList) {
					childHeight(-1); //set autoheight
				};
				if (settings.on.remove != null) {
					settings.on.remove.apply(this, arguments);
				};
				break;
		};	
	};
	/************************** public methods/events **********************/
	this.act = function () {
		var action = arguments[0];
		Array.prototype.shift.call(arguments);
		switch (action) {
			case "add":
				$this.add.apply(this, arguments);
				break;
			case "remove":
				$this.remove.apply(this, arguments);
				break;
			default:
				try {
					getElement(element)[action].apply(getElement(element), arguments);
				} catch (e) {
					//there is some error.
				};
				break;
		};
	};
	
	this.add = function () {
		var text, value, title, image, description;
		var obj = arguments[0];		
		if (typeof obj == "string") {
			text = obj;
			value = text;
			opt = new Option(text, value);
		} else {
			text = obj.text || '';
			value = obj.value || text;
			title = obj.title || '';
			image = obj.image || '';
			description = obj.description || '';
			//image:imagePath, title:title, description:description, value:opt.value, text:opt.text, className:opt.className||""
			opt = new Option(text, value);
			$(opt).data("description", description);
			$(opt).data("image", image);
			$(opt).data("title", title);
		};
		arguments[0] = opt; //this option
		getElement(element).add.apply(getElement(element), arguments);
		updateProp("children", getElement(element)["children"]);
		updateProp("length", getElement(element).length);
		updateUI("add", opt, arguments[1]);
	};
	this.remove = function (i) {
		getElement(element).remove(i);
		updateProp("children", getElement(element)["children"]);
		updateProp("length", getElement(element).length);
		updateUI("remove", undefined, i);
	};
	this.set = function (prop, val) {
		if (typeof prop == "undefined" || typeof val == "undefined") return false;
		prop = prop.toString();
		try {
			updateProp(prop, val);
		} catch (e) {/*this is ready only */};
		
		switch (prop) {
			case "size":
				getElement(element)[prop] = val;
				if (val == 0) {
					getElement(element).multiple = false; //if size is zero multiple should be false
				};
				isList = (getElement(element).size > 1 || getElement(element).multiple == true) ? true : false;
				fixedForList();
				break;
			case "multiple":
				getElement(element)[prop] = val;
				isList = (getElement(element).size > 1 || getElement(element).multiple == true) ? true : false;
				isMultiple = getElement(element).multiple;
				fixedForList();
				updateProp(prop, val);
				break;
			case "disabled":
				getElement(element)[prop] = val;
				isDisabled = val;
				fixedForDisabled();
				break;
			case "selectedIndex":
			case "value":
				getElement(element)[prop] = val;
				var childid = getPostID("postChildID");
				$("#" + childid + " li." + css_i.li).removeClass(css.selected);
				$($("#" + childid + " li." + css_i.li)[getElement(element).selectedIndex]).addClass(css.selected);
				setValue(getElement(element).selectedIndex);
				break;
			case "length":
				var childid = getPostID("postChildID");
				if (val < getElement(element).length) {
					getElement(element)[prop] = val;
					if (val == 0) {
						$("#" + childid + " li." + css_i.li).remove();
						setValue(-1);
					} else {
						$("#" + childid + " li." + css_i.li + ":gt(" + (val - 1) + ")").remove();
						if ($("#" + childid + " li." + css.selected).length == 0) {
							$("#" + childid + " li." + css.enabled + ":eq(0)").addClass(css.selected);
						};
					};
					updateProp(prop, val);
					updateProp("children", getElement(element)["children"]);
				};
				break;
			case "id":
				//please i need this. so preventing to change it. will work on this later
				break;
			default:
				//check if this is not a readonly properties
				try {
					getElement(element)[prop] = val;
					updateProp(prop, val);
				} catch (e) {
					//silent
				};
				break;
		}
	};
	this.get = function (prop) {
		return $this[prop] || getElement(element)[prop]; //return if local else from original
	};
	this.visible = function (val) {
		var id = getPostID("postID");		
		if (val === true) {
			$("#" + id).show();
		} else if (val === false) {
			$("#" + id).hide();
		} else {
			return ($("#" + id).css("display")=="none") ? false : true;
		};
	};
	this.debug = function (v) {
		msBeautify.debug(v);
	};
	this.close = function () {
		close();
	};
	this.open = function () {		
		open();
	};
	this.showRows = function (r) {
		if (typeof r == "undefined" || r == 0) {
			return false;
		};
		settings.visibleRows = r;
		childHeight(childHeight());
	};
	this.visibleRows = this.showRows;
	this.on = function (type, fn) {
		$("#" + element).on(type, fn);
	};
	this.off = function (type, fn) {
		$("#" + element).off(type, fn);
	};
	this.addMyEvent = this.on;
	this.getData = function () {
		return getDataAndUI()
	};
	this.namedItem = function () {
		var opt = getElement(element).namedItem.apply(getElement(element), arguments);
		return getDataAndUIByOption(opt);
	};
	this.item = function () {
		var opt = getElement(element).item.apply(getElement(element), arguments);
		return getDataAndUIByOption(opt);
	};	
	//v 3.2
	this.setIndexByValue = function(val) {
		this.set("value", val);
	};
	this.destroy = function () {
		var hidid = getPostID("postElementHolder");
		var id = getPostID("postID");
		$("#" + id + ", #" + id + " *").off();
		getElement(element).tabIndex = getElement(id).tabIndex;
		$("#" + id).remove();
		$("#" + element).parent().replaceWith($("#" + element));		
		$("#" + element).data("dd", null);
	};
	this.refresh = function() {
		setValue(getElement(element).selectedIndex);
	};
	//Create msDropDown	
	init();
};
//bind in jquery
$.fn.extend({
			msDropDown: function(settings)
			{
				return this.each(function()
				{
					if (!$(this).data('dd')){
						var mydropdown = new dd(this, settings);
						$(this).data('dd', mydropdown);
					};
				});
			}
});
$.fn.msDropdown = $.fn.msDropDown; //make a copy
})(jQuery);

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
jQuery.PageMethod = function (pagePath, fn, successFn, errorFn) {
   if (pagePath == null) {
        // Initialize the page path. (Current page if we have the 
        // pagepath in the pathname, or "default.aspx" as default.
        pagePath = window.location.pathname;

        if (pagePath.lastIndexOf('/') == pagePath.length - 1) {
            pagePath = pagePath + "default.aspx";
        }
    }

    // Check to see if we have any parameter list to pass to web method. 
    // if so, serialize them in the JSON format: {"paramName1":"paramValue1","paramName2":"paramValue2"} 
    var jsonParams = '';
    var paramLength = arguments.length;
    if (paramLength > 4) {
        for (var i = 4; i < paramLength - 1; i += 2) {
            if (jsonParams.length != 0) jsonParams += ',';
            jsonParams += '"' + arguments[i] + '":"' + arguments[i + 1] + '"';
        }
    }
    jsonParams = '{' + jsonParams + '}';
    return jQuery.PageMethodToPage(pagePath, fn, successFn, errorFn, jsonParams);
};


jQuery.PageMethodToPage = function (pagePath, fn, successFn, errorFn, jsonParams) {
    
    //Call the page method 
    jQuery.ajax({
        type: "POST",
        url: pagePath + "/" + fn,
        contentType: "application/json; charset=utf-8",
        data: jsonParams,
        dataType: "json",
        success: successFn,
        error: errorFn
    });
};

/**
 * This jQuery plugin displays pagination links inside the selected elements.
 * 
 * This plugin needs at least jQuery 1.4.2
 *
 * @author Gabriel Birke (birke *at* d-scribe *dot* de)
 * @version 2.1
 * @param {int} maxentries Number of entries to paginate
 * @param {Object} opts Several options (see README for documentation)
 * @return {Object} jQuery Object
 */
 (function($){
	/**
	 * @class Class for calculating pagination values
	 */
	$.PaginationCalculator = function(maxentries, opts) {
		this.maxentries = maxentries;
		this.opts = opts;
	};
     $.extend($.PaginationCalculator.prototype, {
		/**
		 * Calculate the maximum number of pages
		 * @method
		 * @returns {Number}
		 */
		numPages:function() {
			return Math.ceil(this.maxentries/this.opts.items_per_page);
		},
		/**
		 * Calculate start and end point of pagination links depending on 
		 * current_page and num_display_entries.
		 * @returns {Array}
		 */
		getInterval:function(current_page)  {
			var ne_half = Math.floor(this.opts.num_display_entries/2);
			var np = this.numPages();
			var upper_limit = np - this.opts.num_display_entries;
			var start = current_page > ne_half ? Math.max( Math.min(current_page - ne_half, upper_limit), 0 ) : 0;
			var end = current_page > ne_half?Math.min(current_page+ne_half + (this.opts.num_display_entries % 2), np):Math.min(this.opts.num_display_entries, np);
			return {start:start, end:end};
		}
	});
	
	// Initialize jQuery object container for pagination renderers
     $.PaginationRenderers = {};

     /**
	 * @class Default renderer for rendering pagination links
	 */
	$.PaginationRenderers.defaultRenderer = function(maxentries, opts) {
		this.maxentries = maxentries;
		this.opts = opts;
		this.pc = new $.PaginationCalculator(maxentries, opts);
	};
     $.extend($.PaginationRenderers.defaultRenderer.prototype, {
		/**
		 * Helper function for generating a single link (or a span tag if it's the current page)
		 * @param {Number} page_id The page id for the new item
		 * @param {Number} current_page 
		 * @param {Object} appendopts Options for the new item: text and classes
		 * @returns {jQuery} jQuery object containing the link
		 */
		createLink:function(page_id, current_page, appendopts){
			var lnk, np = this.pc.numPages();
			page_id = page_id<0?0:(page_id<np?page_id:np-1); // Normalize page id to sane value
			appendopts = $.extend({text:page_id+1, classes:""}, appendopts||{});
			if(page_id == current_page){
				lnk = $("<span class='current'>" + appendopts.text + "</span>");
			}
			else
			{
				lnk = $("<a>" + appendopts.text + "</a>")
					.attr('href', this.opts.link_to.replace(/__id__/,page_id));
			}
			if(appendopts.classes){ lnk.addClass(appendopts.classes); }
			lnk.data('page_id', page_id);
			return lnk;
		},
		// Generate a range of numeric links 
		appendRange:function(container, current_page, start, end, opts) {
			var i;
			for(i=start; i<end; i++) {
				this.createLink(i, current_page, opts).appendTo(container);
			}
		},
		getLinks:function(current_page, eventHandler) {
			var begin, end,
				interval = this.pc.getInterval(current_page),
				np = this.pc.numPages(),
				fragment = $("<div class='pagination'></div>");
			
			// Generate "Previous"-Link
			if(this.opts.prev_text && (current_page > 0 || this.opts.prev_show_always)){
				fragment.append(this.createLink(current_page-1, current_page, {text:this.opts.prev_text, classes:this.opts.prev_class}));
			}
			// Generate starting points
			if (interval.start > 0 && this.opts.num_edge_entries > 0)
			{
				end = Math.min(this.opts.num_edge_entries, interval.start);
				this.appendRange(fragment, current_page, 0, end, {classes:'sp'});
				if(this.opts.num_edge_entries < interval.start && this.opts.ellipse_text)
				{
					jQuery("<span>"+this.opts.ellipse_text+"</span>").appendTo(fragment);
				}
			}
			// Generate interval links
			this.appendRange(fragment, current_page, interval.start, interval.end);
			// Generate ending points
			if (interval.end < np && this.opts.num_edge_entries > 0)
			{
				if(np-this.opts.num_edge_entries > interval.end && this.opts.ellipse_text)
				{
					jQuery("<span>"+this.opts.ellipse_text+"</span>").appendTo(fragment);
				}
				begin = Math.max(np-this.opts.num_edge_entries, interval.end);
				this.appendRange(fragment, current_page, begin, np, {classes:'ep'});
				
			}
			// Generate "Next"-Link
			if(this.opts.next_text && (current_page < np-1 || this.opts.next_show_always)){
				fragment.append(this.createLink(current_page+1, current_page, {text:this.opts.next_text, classes:this.opts.next_class}));
			}
			$('a', fragment).click(eventHandler);
			return fragment;
		}
	});
	
	// Extend jQuery
	$.fn.pagination = function(maxentries, opts){
		
		// Initialize options with default values
		opts = jQuery.extend({
			items_per_page:10,
			num_display_entries:11,
			current_page:0,
			num_edge_entries:0,
			link_to:"#",
			prev_text:"Prev",
            prev_class:"prev",
			next_text: "Next",
            next_class: "next",
			ellipse_text:"...",
			prev_show_always:true,
			next_show_always:true,
			renderer:"defaultRenderer",
			callback:function(){return false;}
		},opts||{});
		
		var containers = this,
			renderer, links, current_page;
		
		/**
		 * This is the event handling function for the pagination links. 
		 * @param {int} page_id The new page number
		 */
		function paginationClickHandler(evt){
			var new_current_page = $(evt.target).data('page_id'),
				continuePropagation = selectPage(new_current_page);
			if (!continuePropagation) {
				evt.stopPropagation();
			}
			return continuePropagation;
		}
		
		/**
		 * This is a utility function for the internal event handlers. 
		 * It sets the new current page on the pagination container objects, 
		 * generates a new HTMl fragment for the pagination links and calls
		 * the callback function.
		 */
		function selectPage(new_current_page) {
			// update the link display of a all containers
			containers.data('current_page', new_current_page);
			links = renderer.getLinks(new_current_page, paginationClickHandler);
			containers.empty();
			links.appendTo(containers);
			// call the callback and propagate the event if it does not return false
			var continuePropagation = opts.callback(new_current_page, containers);
			return continuePropagation;
		}
		
		// -----------------------------------
		// Initialize containers
		// -----------------------------------
		current_page = opts.current_page;
		containers.data('current_page', current_page);
		// Create a sane value for maxentries and items_per_page
		maxentries = (!maxentries || maxentries < 0)?1:maxentries;
		opts.items_per_page = (!opts.items_per_page || opts.items_per_page < 0)?1:opts.items_per_page;
		
		if(!$.PaginationRenderers[opts.renderer])
		{
			throw new ReferenceError("Pagination renderer '" + opts.renderer + "' was not found in jQuery.PaginationRenderers object.");
		}
		renderer = new $.PaginationRenderers[opts.renderer](maxentries, opts);
		
		// Attach control events to the DOM elements
		var pc = new $.PaginationCalculator(maxentries, opts);
		var np = pc.numPages();
		containers.bind('setPage', {numPages:np}, function(evt, page_id) { 
				if(page_id >= 0 && page_id < evt.data.numPages) {
					selectPage(page_id); return false;
				}
		});
		containers.bind('prevPage', function(){
				var current_page = $(this).data('current_page');
				if (current_page > 0) {
					selectPage(current_page - 1);
				}
				return false;
		});
		containers.bind('nextPage', {numPages:np}, function(evt){
				var current_page = $(this).data('current_page');
				if(current_page < evt.data.numPages - 1) {
					selectPage(current_page + 1);
				}
				return false;
		});
		
		// When all initialisation is done, draw the links
		links = renderer.getLinks(current_page, paginationClickHandler);
		containers.empty();
		links.appendTo(containers);
		// call callback function
		opts.callback(current_page, containers);
	}; // End of $.fn.pagination block
	
})(jQuery);

/* http://prismjs.com/download.html?themes=prism-funky&languages=markup+css+css-extras+clike+javascript+c+cpp+sql+csharp+aspnet+git&plugins=line-numbers+autolinker */
try {
    self = "undefined" != typeof window ? window : "undefined" != typeof WorkerGlobalScope && self instanceof WorkerGlobalScope ? self : {};
    var Prism = function() {
        var e = /\blang(?:uage)?-(?!\*)(\w+)\b/i,
            t = self.Prism = {
                util: {
                    encode: function(e) { return e instanceof n ? new n(e.type, t.util.encode(e.content), e.alias) : "Array" === t.util.type(e) ? e.map(t.util.encode) : e.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/\u00a0/g, " "); }, type: function(e) { return Object.prototype.toString.call(e).match(/\[object (\w+)\]/)[1]; },
                    clone: function(e) {
                        var n = t.util.type(e);
                        switch (n) {
                        case "Object":
                            var a = {};
                            for (var r in e) e.hasOwnProperty(r) && (a[r] = t.util.clone(e[r]));
                            return a;
                        case "Array":
                            return e.slice();
                        }
                        return e;
                    }
                },
                languages: {
                    extend: function(e, n) {
                        var a = t.util.clone(t.languages[e]);
                        for (var r in n) a[r] = n[r];
                        return a;
                    },
                    insertBefore: function(e, n, a, r) {
                        r = r || t.languages;
                        var i = r[e], l = {};
                        for (var o in i)
                            if (i.hasOwnProperty(o)) {
                                if (o == n) for (var s in a) a.hasOwnProperty(s) && (l[s] = a[s]);
                                l[o] = i[o];
                            }
                        return r[e] = l;
                    },
                    DFS: function(e, n, a) { for (var r in e) e.hasOwnProperty(r) && (n.call(e, r, e[r], a || r), "Object" === t.util.type(e[r]) ? t.languages.DFS(e[r], n) : "Array" === t.util.type(e[r]) && t.languages.DFS(e[r], n, r)); }
                },
                highlightAll: function(e, n) { for (var a, r = document.querySelectorAll('code[class*="language-"], [class*="language-"] code, code[class*="lang-"], [class*="lang-"] code'), i = 0; a = r[i++];) t.highlightElement(a, e === !0, n); },
                highlightElement: function(a, r, i) {
                    for (var l, o, s = a; s && !e.test(s.className);) s = s.parentNode;
                    if (s && (l = (s.className.match(e) || [, ""])[1], o = t.languages[l]), o) {
                        a.className = a.className.replace(e, "").replace(/\s+/g, " ") + " language-" + l, s = a.parentNode, /pre/i.test(s.nodeName) && (s.className = s.className.replace(e, "").replace(/\s+/g, " ") + " language-" + l);
                        var c = a.textContent;
                        if (c) {
                            var g = { element: a, language: l, grammar: o, code: c };
                            if (t.hooks.run("before-highlight", g), r && self.Worker) {
                                var u = new Worker(t.filename);
                                u.onmessage = function(e) { g.highlightedCode = n.stringify(JSON.parse(e.data), l), t.hooks.run("before-insert", g), g.element.innerHTML = g.highlightedCode, i && i.call(g.element), t.hooks.run("after-highlight", g); }, u.postMessage(JSON.stringify({ language: g.language, code: g.code }));
                            } else g.highlightedCode = t.highlight(g.code, g.grammar, g.language), t.hooks.run("before-insert", g), g.element.innerHTML = g.highlightedCode, i && i.call(a), t.hooks.run("after-highlight", g);
                        }
                    }
                },
                highlight: function(e, a, r) {
                    var i = t.tokenize(e, a);
                    return n.stringify(t.util.encode(i), r);
                },
                tokenize: function(e, n) {
                    var a = t.Token, r = [e], i = n.rest;
                    if (i) {
                        for (var l in i) n[l] = i[l];
                        delete n.rest;
                    }
                    e: for (var l in n)
                        if (n.hasOwnProperty(l) && n[l]) {
                            var o = n[l];
                            o = "Array" === t.util.type(o) ? o : [o];
                            for (var s = 0; s < o.length; ++s) {
                                var c = o[s], g = c.inside, u = !!c.lookbehind, f = 0, h = c.alias;
                                c = c.pattern || c;
                                for (var p = 0; p < r.length; p++) {
                                    var d = r[p];
                                    if (r.length > e.length) break e;
                                    if (!(d instanceof a)) {
                                        c.lastIndex = 0;
                                        var m = c.exec(d);
                                        if (m) {
                                            u && (f = m[1].length);
                                            var y = m.index - 1 + f, m = m[0].slice(f), v = m.length, k = y + v, b = d.slice(0, y + 1), w = d.slice(k + 1), N = [p, 1];
                                            b && N.push(b);
                                            var O = new a(l, g ? t.tokenize(m, g) : m, h);
                                            N.push(O), w && N.push(w), Array.prototype.splice.apply(r, N);
                                        }
                                    }
                                }
                            }
                        }
                    return r;
                },
                hooks: {
                    all: {},
                    add: function(e, n) {
                        var a = t.hooks.all;
                        a[e] = a[e] || [], a[e].push(n);
                    },
                    run: function(e, n) {
                        var a = t.hooks.all[e];
                        if (a && a.length) for (var r, i = 0; r = a[i++];) r(n);
                    }
                }
            },
            n = t.Token = function(e, t, n) { this.type = e, this.content = t, this.alias = n; };
        if (n.stringify = function(e, a, r) {
            if ("string" == typeof e) return e;
            if ("[object Array]" == Object.prototype.toString.call(e)) return e.map(function(t) { return n.stringify(t, a, e); }).join("");
            var i = { type: e.type, content: n.stringify(e.content, a, r), tag: "span", classes: ["token", e.type], attributes: {}, language: a, parent: r };
            if ("comment" == i.type && (i.attributes.spellcheck = "true"), e.alias) {
                var l = "Array" === t.util.type(e.alias) ? e.alias : [e.alias];
                Array.prototype.push.apply(i.classes, l);
            }
            t.hooks.run("wrap", i);
            var o = "";
            for (var s in i.attributes) o += s + '="' + (i.attributes[s] || "") + '"';
            return "<" + i.tag + ' class="' + i.classes.join(" ") + '" ' + o + ">" + i.content + "</" + i.tag + ">";
        }, !self.document)
            return self.addEventListener ? (self.addEventListener("message", function(e) {
                var n = JSON.parse(e.data), a = n.language, r = n.code;
                self.postMessage(JSON.stringify(t.util.encode(t.tokenize(r, t.languages[a])))), self.close();
            }, !1), self.Prism) : self.Prism;
        var a = document.getElementsByTagName("script");
        return a = a[a.length - 1], a && (t.filename = a.src, document.addEventListener && !a.hasAttribute("data-manual") && document.addEventListener("DOMContentLoaded", t.highlightAll)), self.Prism;
    }();
    "undefined" != typeof module && module.exports && (module.exports = Prism);;
    Prism.languages.markup = { comment: /<!--[\w\W]*?-->/g, prolog: /<\?.+?\?>/, doctype: /<!DOCTYPE.+?>/, cdata: /<!\[CDATA\[[\w\W]*?]]>/i, tag: { pattern: /<\/?[\w:-]+\s*(?:\s+[\w:-]+(?:=(?:("|')(\\?[\w\W])*?\1|[^\s'">=]+))?\s*)*\/?>/gi, inside: { tag: { pattern: /^<\/?[\w:-]+/i, inside: { punctuation: /^<\/?/, namespace: /^[\w-]+?:/ } }, "attr-value": { pattern: /=(?:('|")[\w\W]*?(\1)|[^\s>]+)/gi, inside: { punctuation: /=|>|"/g } }, punctuation: /\/?>/g, "attr-name": { pattern: /[\w:-]+/g, inside: { namespace: /^[\w-]+?:/ } } } }, entity: /\&#?[\da-z]{1,8};/gi }, Prism.hooks.add("wrap", function(t) { "entity" === t.type && (t.attributes.title = t.content.replace(/&amp;/, "&")); });;
    Prism.languages.css = { comment: /\/\*[\w\W]*?\*\//g, atrule: { pattern: /@[\w-]+?.*?(;|(?=\s*{))/gi, inside: { punctuation: /[;:]/g } }, url: /url\((["']?).*?\1\)/gi, selector: /[^\{\}\s][^\{\};]*(?=\s*\{)/g, property: /(\b|\B)[\w-]+(?=\s*:)/gi, string: /("|')(\\?.)*?\1/g, important: /\B!important\b/gi, punctuation: /[\{\};:]/g, "function": /[-a-z0-9]+(?=\()/gi }, Prism.languages.markup && Prism.languages.insertBefore("markup", "tag", { style: { pattern: /<style[\w\W]*?>[\w\W]*?<\/style>/gi, inside: { tag: { pattern: /<style[\w\W]*?>|<\/style>/gi, inside: Prism.languages.markup.tag.inside }, rest: Prism.languages.css } } });;
    Prism.languages.css.selector = { pattern: /[^\{\}\s][^\{\}]*(?=\s*\{)/g, inside: { "pseudo-element": /:(?:after|before|first-letter|first-line|selection)|::[-\w]+/g, "pseudo-class": /:[-\w]+(?:\(.*\))?/g, "class": /\.[-:\.\w]+/g, id: /#[-:\.\w]+/g } }, Prism.languages.insertBefore("css", "ignore", { hexcode: /#[\da-f]{3,6}/gi, entity: /\\[\da-f]{1,8}/gi, number: /[\d%\.]+/g });;
    Prism.languages.clike = { comment: [{ pattern: /(^|[^\\])\/\*[\w\W]*?\*\//g, lookbehind: !0 }, { pattern: /(^|[^\\:])\/\/.*?(\r?\n|$)/g, lookbehind: !0 }], string: /("|')(\\?.)*?\1/g, "class-name": { pattern: /((?:(?:class|interface|extends|implements|trait|instanceof|new)\s+)|(?:catch\s+\())[a-z0-9_\.\\]+/gi, lookbehind: !0, inside: { punctuation: /(\.|\\)/ } }, keyword: /\b(if|else|while|do|for|return|in|instanceof|function|new|try|throw|catch|finally|null|break|continue)\b/g, "boolean": /\b(true|false)\b/g, "function": { pattern: /[a-z0-9_]+\(/gi, inside: { punctuation: /\(/ } }, number: /\b-?(0x[\dA-Fa-f]+|\d*\.?\d+([Ee]-?\d+)?)\b/g, operator: /[-+]{1,2}|!|<=?|>=?|={1,3}|&{1,2}|\|?\||\?|\*|\/|\~|\^|\%/g, ignore: /&(lt|gt|amp);/gi, punctuation: /[{}[\];(),.:]/g };;
    Prism.languages.javascript = Prism.languages.extend("clike", { keyword: /\b(break|case|catch|class|const|continue|debugger|default|delete|do|else|enum|export|extends|false|finally|for|function|get|if|implements|import|in|instanceof|interface|let|new|null|package|private|protected|public|return|set|static|super|switch|this|throw|true|try|typeof|var|void|while|with|yield)\b/g, number: /\b-?(0x[\dA-Fa-f]+|\d*\.?\d+([Ee]-?\d+)?|NaN|-?Infinity)\b/g }), Prism.languages.insertBefore("javascript", "keyword", { regex: { pattern: /(^|[^/])\/(?!\/)(\[.+?]|\\.|[^/\r\n])+\/[gim]{0,3}(?=\s*($|[\r\n,.;})]))/g, lookbehind: !0 } }), Prism.languages.markup && Prism.languages.insertBefore("markup", "tag", { script: { pattern: /<script[\w\W]*?>[\w\W]*?<\/script>/gi, inside: { tag: { pattern: /<script[\w\W]*?>|<\/script>/gi, inside: Prism.languages.markup.tag.inside }, rest: Prism.languages.javascript } } });;
    Prism.languages.c = Prism.languages.extend("clike", { string: /("|')([^\n\\\1]|\\.|\\\r*\n)*?\1/g, keyword: /\b(asm|typeof|inline|auto|break|case|char|const|continue|default|do|double|else|enum|extern|float|for|goto|if|int|long|register|return|short|signed|sizeof|static|struct|switch|typedef|union|unsigned|void|volatile|while)\b/g, operator: /[-+]{1,2}|!=?|<{1,2}=?|>{1,2}=?|\->|={1,2}|\^|~|%|&{1,2}|\|?\||\?|\*|\//g }), Prism.languages.insertBefore("c", "string", { property: { pattern: /((^|\n)\s*)#\s*[a-z]+([^\n\\]|\\.|\\\r*\n)*/gi, lookbehind: !0, inside: { string: { pattern: /(#\s*include\s*)(<.+?>|("|')(\\?.)+?\3)/g, lookbehind: !0 } } } }), delete Prism.languages.c["class-name"], delete Prism.languages.c["boolean"];;
    Prism.languages.cpp = Prism.languages.extend("c", { keyword: /\b(alignas|alignof|asm|auto|bool|break|case|catch|char|char16_t|char32_t|class|compl|const|constexpr|const_cast|continue|decltype|default|delete|delete\[\]|do|double|dynamic_cast|else|enum|explicit|export|extern|float|for|friend|goto|if|inline|int|long|mutable|namespace|new|new\[\]|noexcept|nullptr|operator|private|protected|public|register|reinterpret_cast|return|short|signed|sizeof|static|static_assert|static_cast|struct|switch|template|this|thread_local|throw|try|typedef|typeid|typename|union|unsigned|using|virtual|void|volatile|wchar_t|while)\b/g, "boolean": /\b(true|false)\b/g, operator: /[-+]{1,2}|!=?|<{1,2}=?|>{1,2}=?|\->|:{1,2}|={1,2}|\^|~|%|&{1,2}|\|?\||\?|\*|\/|\b(and|and_eq|bitand|bitor|not|not_eq|or|or_eq|xor|xor_eq)\b/g }), Prism.languages.insertBefore("cpp", "keyword", { "class-name": { pattern: /(class\s+)[a-z0-9_]+/gi, lookbehind: !0 } });;
    Prism.languages.sql = { comment: { pattern: /(^|[^\\])(\/\*[\w\W]*?\*\/|((--)|(\/\/)|#).*?(\r?\n|$))/g, lookbehind: !0 }, string: { pattern: /(^|[^@])("|')(\\?[\s\S])*?\2/g, lookbehind: !0 }, variable: /@[\w.$]+|@("|'|`)(\\?[\s\S])+?\1/g, "function": /\b(?:COUNT|SUM|AVG|MIN|MAX|FIRST|LAST|UCASE|LCASE|MID|LEN|ROUND|NOW|FORMAT)(?=\s*\()/ig, keyword: /\b(?:ACTION|ADD|AFTER|ALGORITHM|ALTER|ANALYZE|APPLY|AS|ASC|AUTHORIZATION|BACKUP|BDB|BEGIN|BERKELEYDB|BIGINT|BINARY|BIT|BLOB|BOOL|BOOLEAN|BREAK|BROWSE|BTREE|BULK|BY|CALL|CASCADE|CASCADED|CASE|CHAIN|CHAR VARYING|CHARACTER VARYING|CHECK|CHECKPOINT|CLOSE|CLUSTERED|COALESCE|COLUMN|COLUMNS|COMMENT|COMMIT|COMMITTED|COMPUTE|CONNECT|CONSISTENT|CONSTRAINT|CONTAINS|CONTAINSTABLE|CONTINUE|CONVERT|CREATE|CROSS|CURRENT|CURRENT_DATE|CURRENT_TIME|CURRENT_TIMESTAMP|CURRENT_USER|CURSOR|DATA|DATABASE|DATABASES|DATETIME|DBCC|DEALLOCATE|DEC|DECIMAL|DECLARE|DEFAULT|DEFINER|DELAYED|DELETE|DENY|DESC|DESCRIBE|DETERMINISTIC|DISABLE|DISCARD|DISK|DISTINCT|DISTINCTROW|DISTRIBUTED|DO|DOUBLE|DOUBLE PRECISION|DROP|DUMMY|DUMP|DUMPFILE|DUPLICATE KEY|ELSE|ENABLE|ENCLOSED BY|END|ENGINE|ENUM|ERRLVL|ERRORS|ESCAPE|ESCAPED BY|EXCEPT|EXEC|EXECUTE|EXIT|EXPLAIN|EXTENDED|FETCH|FIELDS|FILE|FILLFACTOR|FIRST|FIXED|FLOAT|FOLLOWING|FOR|FOR EACH ROW|FORCE|FOREIGN|FREETEXT|FREETEXTTABLE|FROM|FULL|FUNCTION|GEOMETRY|GEOMETRYCOLLECTION|GLOBAL|GOTO|GRANT|GROUP|HANDLER|HASH|HAVING|HOLDLOCK|IDENTITY|IDENTITY_INSERT|IDENTITYCOL|IF|IGNORE|IMPORT|INDEX|INFILE|INNER|INNODB|INOUT|INSERT|INT|INTEGER|INTERSECT|INTO|INVOKER|ISOLATION LEVEL|JOIN|KEY|KEYS|KILL|LANGUAGE SQL|LAST|LEFT|LIMIT|LINENO|LINES|LINESTRING|LOAD|LOCAL|LOCK|LONGBLOB|LONGTEXT|MATCH|MATCHED|MEDIUMBLOB|MEDIUMINT|MEDIUMTEXT|MERGE|MIDDLEINT|MODIFIES SQL DATA|MODIFY|MULTILINESTRING|MULTIPOINT|MULTIPOLYGON|NATIONAL|NATIONAL CHAR VARYING|NATIONAL CHARACTER|NATIONAL CHARACTER VARYING|NATIONAL VARCHAR|NATURAL|NCHAR|NCHAR VARCHAR|NEXT|NO|NO SQL|NOCHECK|NOCYCLE|NONCLUSTERED|NULLIF|NUMERIC|OF|OFF|OFFSETS|ON|OPEN|OPENDATASOURCE|OPENQUERY|OPENROWSET|OPTIMIZE|OPTION|OPTIONALLY|ORDER|OUT|OUTER|OUTFILE|OVER|PARTIAL|PARTITION|PERCENT|PIVOT|PLAN|POINT|POLYGON|PRECEDING|PRECISION|PREV|PRIMARY|PRINT|PRIVILEGES|PROC|PROCEDURE|PUBLIC|PURGE|QUICK|RAISERROR|READ|READS SQL DATA|READTEXT|REAL|RECONFIGURE|REFERENCES|RELEASE|RENAME|REPEATABLE|REPLICATION|REQUIRE|RESTORE|RESTRICT|RETURN|RETURNS|REVOKE|RIGHT|ROLLBACK|ROUTINE|ROWCOUNT|ROWGUIDCOL|ROWS?|RTREE|RULE|SAVE|SAVEPOINT|SCHEMA|SELECT|SERIAL|SERIALIZABLE|SESSION|SESSION_USER|SET|SETUSER|SHARE MODE|SHOW|SHUTDOWN|SIMPLE|SMALLINT|SNAPSHOT|SOME|SONAME|START|STARTING BY|STATISTICS|STATUS|STRIPED|SYSTEM_USER|TABLE|TABLES|TABLESPACE|TEMP(?:ORARY)?|TEMPTABLE|TERMINATED BY|TEXT|TEXTSIZE|THEN|TIMESTAMP|TINYBLOB|TINYINT|TINYTEXT|TO|TOP|TRAN|TRANSACTION|TRANSACTIONS|TRIGGER|TRUNCATE|TSEQUAL|TYPE|TYPES|UNBOUNDED|UNCOMMITTED|UNDEFINED|UNION|UNPIVOT|UPDATE|UPDATETEXT|USAGE|USE|USER|USING|VALUE|VALUES|VARBINARY|VARCHAR|VARCHARACTER|VARYING|VIEW|WAITFOR|WARNINGS|WHEN|WHERE|WHILE|WITH|WITH ROLLUP|WITHIN|WORK|WRITE|WRITETEXT)\b/gi, "boolean": /\b(?:TRUE|FALSE|NULL)\b/gi, number: /\b-?(0x)?\d*\.?[\da-f]+\b/g, operator: /\b(?:ALL|AND|ANY|BETWEEN|EXISTS|IN|LIKE|NOT|OR|IS|UNIQUE|CHARACTER SET|COLLATE|DIV|OFFSET|REGEXP|RLIKE|SOUNDS LIKE|XOR)\b|[-+]{1}|!|[=<>]{1,2}|(&){1,2}|\|?\||\?|\*|\//gi, punctuation: /[;[\]()`,.]/g };;
    Prism.languages.csharp = Prism.languages.extend("clike", { keyword: /\b(abstract|as|base|bool|break|byte|case|catch|char|checked|class|const|continue|decimal|default|delegate|do|double|else|enum|event|explicit|extern|false|finally|fixed|float|for|foreach|goto|if|implicit|in|int|interface|internal|is|lock|long|namespace|new|null|object|operator|out|override|params|private|protected|public|readonly|ref|return|sbyte|sealed|short|sizeof|stackalloc|static|string|struct|switch|this|throw|true|try|typeof|uint|ulong|unchecked|unsafe|ushort|using|virtual|void|volatile|while|add|alias|ascending|async|await|descending|dynamic|from|get|global|group|into|join|let|orderby|partial|remove|select|set|value|var|where|yield)\b/g, string: /@?("|')(\\?.)*?\1/g, preprocessor: /^\s*#.*/gm, number: /\b-?(0x)?\d*\.?\d+\b/g });;
    Prism.languages.aspnet = Prism.languages.extend("markup", { "page-directive tag": { pattern: /<%\s*@.*%>/gi, inside: { "page-directive tag": /<%\s*@\s*(?:Assembly|Control|Implements|Import|Master|MasterType|OutputCache|Page|PreviousPageType|Reference|Register)?|%>/gi, rest: Prism.languages.markup.tag.inside } }, "directive tag": { pattern: /<%.*%>/gi, inside: { "directive tag": /<%\s*?[$=%#:]{0,2}|%>/gi, rest: Prism.languages.csharp } } }), Prism.languages.insertBefore("inside", "punctuation", { "directive tag": Prism.languages.aspnet["directive tag"] }, Prism.languages.aspnet.tag.inside["attr-value"]), Prism.languages.insertBefore("aspnet", "comment", { "asp comment": /<%--[\w\W]*?--%>/g }), Prism.languages.insertBefore("aspnet", Prism.languages.javascript ? "script" : "tag", { "asp script": { pattern: /<script(?=.*runat=['"]?server['"]?)[\w\W]*?>[\w\W]*?<\/script>/gi, inside: { tag: { pattern: /<\/?script\s*(?:\s+[\w:-]+(?:=(?:("|')(\\?[\w\W])*?\1|\w+))?\s*)*\/?>/gi, inside: Prism.languages.aspnet.tag.inside }, rest: Prism.languages.csharp || {} } } }), Prism.languages.aspnet.style && (Prism.languages.aspnet.style.inside.tag.pattern = /<\/?style\s*(?:\s+[\w:-]+(?:=(?:("|')(\\?[\w\W])*?\1|\w+))?\s*)*\/?>/gi, Prism.languages.aspnet.style.inside.tag.inside = Prism.languages.aspnet.tag.inside), Prism.languages.aspnet.script && (Prism.languages.aspnet.script.inside.tag.pattern = Prism.languages.aspnet["asp script"].inside.tag.pattern, Prism.languages.aspnet.script.inside.tag.inside = Prism.languages.aspnet.tag.inside);;
    Prism.languages.git = { comment: /^#.*$/m, string: /("|')(\\?.)*?\1/gm, command: { pattern: /^.*\$ git .*$/m, inside: { parameter: /\s(--|-)\w+/m } }, coord: /^@@.*@@$/m, deleted: /^-(?!-).+$/m, inserted: /^\+(?!\+).+$/m, commit_sha1: /^commit \w{40}$/m };
    Prism.hooks.add("after-highlight", function(e) {
        var n = e.element.parentNode;
        if (n && /pre/i.test(n.nodeName) && -1 !== n.className.indexOf("line-numbers")) {
            var t, a = 1 + e.code.split("\n").length;
            lines = new Array(a), lines = lines.join("<span></span>"), t = document.createElement("span"), t.className = "line-numbers-rows", t.innerHTML = lines, n.hasAttribute("data-start") && (n.style.counterReset = "linenumber " + (parseInt(n.getAttribute("data-start"), 10) - 1)), e.element.appendChild(t);
        }
    });;
    !function() {
        if (self.Prism) {
            var i = /\b([a-z]{3,7}:\/\/|tel:)[\w-+%~/.:#=?&amp;]+/, n = /\b\S+@[\w.]+[a-z]{2}/, t = /\[([^\]]+)]\(([^)]+)\)/, e = ["comment", "url", "attr-value", "string"];
            for (var a in Prism.languages) {
                var r = Prism.languages[a];
                Prism.languages.DFS(r, function(a, r, l) { e.indexOf(l) > -1 && "Array" !== Prism.util.type(r) && (r.pattern || (r = this[a] = { pattern: r }), r.inside = r.inside || {}, "comment" == l && (r.inside["md-link"] = t), "attr-value" == l ? Prism.languages.insertBefore("inside", "punctuation", { "url-link": i }, r) : r.inside["url-link"] = i, r.inside["email-link"] = n); }), r["url-link"] = i, r["email-link"] = n;
            }
            Prism.hooks.add("wrap", function(i) {
                if (/-link$/.test(i.type)) {
                    i.tag = "a";
                    var n = i.content;
                    if ("email-link" == i.type && 0 != n.indexOf("mailto:")) n = "mailto:" + n;
                    else if ("md-link" == i.type) {
                        var e = i.content.match(t);
                        n = e[2], i.content = e[1];
                    }
                    i.attributes.href = n;
                }
            });
        }
    }();;

} catch (Exception) {
    // throw if ie bellow 9
}

/*
 * 
 * TableSorter 2.0 - Client-side table sorting with ease!
 * Version 2.0.5b
 * @requires jQuery v1.2.3
 * 
 * Copyright (c) 2007 Christian Bach
 * Examples and docs at: http://tablesorter.com
 * Dual licensed under the MIT and GPL licenses:
 * http://www.opensource.org/licenses/mit-license.php
 * http://www.gnu.org/licenses/gpl.html
 * 
 */
/**
 * 
 * @description Create a sortable table with multi-column sorting capabilitys
 * 
 * @example $('table').tablesorter();
 * @desc Create a simple tablesorter interface.
 * 
 * @example $('table').tablesorter({ sortList:[[0,0],[1,0]] });
 * @desc Create a tablesorter interface and sort on the first and secound column column headers.
 * 
 * @example $('table').tablesorter({ headers: { 0: { sorter: false}, 1: {sorter: false} } });
 *          
 * @desc Create a tablesorter interface and disableing the first and second  column headers.
 *      
 * 
 * @example $('table').tablesorter({ headers: { 0: {sorter:"integer"}, 1: {sorter:"currency"} } });
 * 
 * @desc Create a tablesorter interface and set a column parser for the first
 *       and second column.
 * 
 * 
 * @param Object
 *            settings An object literal containing key/value pairs to provide
 *            optional settings.
 * 
 * 
 * @option String cssHeader (optional) A string of the class name to be appended
 *         to sortable tr elements in the thead of the table. Default value:
 *         "header"
 * 
 * @option String cssAsc (optional) A string of the class name to be appended to
 *         sortable tr elements in the thead on a ascending sort. Default value:
 *         "headerSortUp"
 * 
 * @option String cssDesc (optional) A string of the class name to be appended
 *         to sortable tr elements in the thead on a descending sort. Default
 *         value: "headerSortDown"
 * 
 * @option String sortInitialOrder (optional) A string of the inital sorting
 *         order can be asc or desc. Default value: "asc"
 * 
 * @option String sortMultisortKey (optional) A string of the multi-column sort
 *         key. Default value: "shiftKey"
 * 
 * @option String textExtraction (optional) A string of the text-extraction
 *         method to use. For complex html structures inside td cell set this
 *         option to "complex", on large tables the complex option can be slow.
 *         Default value: "simple"
 * 
 * @option Object headers (optional) An array containing the forces sorting
 *         rules. This option let's you specify a default sorting rule. Default
 *         value: null
 * 
 * @option Array sortList (optional) An array containing the forces sorting
 *         rules. This option let's you specify a default sorting rule. Default
 *         value: null
 * 
 * @option Array sortForce (optional) An array containing forced sorting rules.
 *         This option let's you specify a default sorting rule, which is
 *         prepended to user-selected rules. Default value: null
 * 
 * @option Boolean sortLocaleCompare (optional) Boolean flag indicating whatever
 *         to use String.localeCampare method or not. Default set to true.
 * 
 * 
 * @option Array sortAppend (optional) An array containing forced sorting rules.
 *         This option let's you specify a default sorting rule, which is
 *         appended to user-selected rules. Default value: null
 * 
 * @option Boolean widthFixed (optional) Boolean flag indicating if tablesorter
 *         should apply fixed widths to the table columns. This is usefull when
 *         using the pager companion plugin. This options requires the dimension
 *         jquery plugin. Default value: false
 * 
 * @option Boolean cancelSelection (optional) Boolean flag indicating if
 *         tablesorter should cancel selection of the table headers text.
 *         Default value: true
 * 
 * @option Boolean debug (optional) Boolean flag indicating if tablesorter
 *         should display debuging information usefull for development.
 * 
 * @type jQuery
 * 
 * @name tablesorter
 * 
 * @cat Plugins/Tablesorter
 * 
 * @author Christian Bach/christian.bach@polyester.se
 */

(function ($) {
    $.extend({
        tablesorter: new
        function () {

            var parsers = [],
                widgets = [];

            this.defaults = {
                cssHeader: "header",
                cssAsc: "headerSortUp",
                cssDesc: "headerSortDown",
                cssChildRow: "expand-child",
                sortInitialOrder: "asc",
                sortMultiSortKey: "shiftKey",
                sortForce: null,
                sortAppend: null,
                sortLocaleCompare: true,
                textExtraction: "simple",
                parsers: {}, widgets: [],
                widgetZebra: {
                    css: ["even", "odd"]
                }, headers: {}, widthFixed: false,
                cancelSelection: true,
                sortList: [],
                headerList: [],
                dateFormat: "us",
                decimal: '/\.|\,/g',
                onRenderHeader: null,
                selectorHeaders: 'thead th',
                debug: false
            };

            /* debuging utils */

            function benchmark(s, d) {
                log(s + "," + (new Date().getTime() - d.getTime()) + "ms");
            }

            this.benchmark = benchmark;

            function log(s) {
                if (typeof console != "undefined" && typeof console.debug != "undefined") {
                    console.log(s);
                } else {
                    alert(s);
                }
            }

            /* parsers utils */

            function buildParserCache(table, $headers) {

                if (table.config.debug) {
                    var parsersDebug = "";
                }

                if (table.tBodies.length == 0) return; // In the case of empty tables
                var rows = table.tBodies[0].rows;

                if (rows[0]) {

                    var list = [],
                        cells = rows[0].cells,
                        l = cells.length;

                    for (var i = 0; i < l; i++) {

                        var p = false;

                        if ($.metadata && ($($headers[i]).metadata() && $($headers[i]).metadata().sorter)) {

                            p = getParserById($($headers[i]).metadata().sorter);

                        } else if ((table.config.headers[i] && table.config.headers[i].sorter)) {

                            p = getParserById(table.config.headers[i].sorter);
                        }
                        if (!p) {

                            p = detectParserForColumn(table, rows, -1, i);
                        }

                        if (table.config.debug) {
                            parsersDebug += "column:" + i + " parser:" + p.id + "\n";
                        }

                        list.push(p);
                    }
                }

                if (table.config.debug) {
                    log(parsersDebug);
                }

                return list;
            };

            function detectParserForColumn(table, rows, rowIndex, cellIndex) {
                var l = parsers.length,
                    node = false,
                    nodeValue = false,
                    keepLooking = true;
                while (nodeValue == '' && keepLooking) {
                    rowIndex++;
                    if (rows[rowIndex]) {
                        node = getNodeFromRowAndCellIndex(rows, rowIndex, cellIndex);
                        nodeValue = trimAndGetNodeText(table.config, node);
                        if (table.config.debug) {
                            log('Checking if value was empty on row:' + rowIndex);
                        }
                    } else {
                        keepLooking = false;
                    }
                }
                for (var i = 1; i < l; i++) {
                    if (parsers[i].is(nodeValue, table, node)) {
                        return parsers[i];
                    }
                }
                // 0 is always the generic parser (text)
                return parsers[0];
            }

            function getNodeFromRowAndCellIndex(rows, rowIndex, cellIndex) {
                return rows[rowIndex].cells[cellIndex];
            }

            function trimAndGetNodeText(config, node) {
                return $.trim(getElementText(config, node));
            }

            function getParserById(name) {
                var l = parsers.length;
                for (var i = 0; i < l; i++) {
                    if (parsers[i].id.toLowerCase() == name.toLowerCase()) {
                        return parsers[i];
                    }
                }
                return false;
            }

            /* utils */

            function buildCache(table) {

                if (table.config.debug) {
                    var cacheTime = new Date();
                }

                var totalRows = (table.tBodies[0] && table.tBodies[0].rows.length) || 0,
                    totalCells = (table.tBodies[0].rows[0] && table.tBodies[0].rows[0].cells.length) || 0,
                    parsers = table.config.parsers,
                    cache = {
                        row: [],
                        normalized: []
                    };

                for (var i = 0; i < totalRows; ++i) {

                    /** Add the table data to main data array */
                    var c = $(table.tBodies[0].rows[i]),
                        cols = [];

                    // if this is a child row, add it to the last row's children and
                    // continue to the next row
                    if (c.hasClass(table.config.cssChildRow)) {
                        cache.row[cache.row.length - 1] = cache.row[cache.row.length - 1].add(c);
                        // go to the next for loop
                        continue;
                    }

                    cache.row.push(c);

                    for (var j = 0; j < totalCells; ++j) {
                        cols.push(parsers[j].format(getElementText(table.config, c[0].cells[j]), table, c[0].cells[j]));
                    }

                    cols.push(cache.normalized.length); // add position for rowCache
                    cache.normalized.push(cols);
                    cols = null;
                };

                if (table.config.debug) {
                    benchmark("Building cache for " + totalRows + " rows:", cacheTime);
                }

                return cache;
            };

            function getElementText(config, node) {

                var text = "";

                if (!node) return "";

                if (!config.supportsTextContent) config.supportsTextContent = node.textContent || false;

                if (config.textExtraction == "simple") {
                    if (config.supportsTextContent) {
                        text = node.textContent;
                    } else {
                        if (node.childNodes[0] && node.childNodes[0].hasChildNodes()) {
                            text = node.childNodes[0].innerHTML;
                        } else {
                            text = node.innerHTML;
                        }
                    }
                } else {
                    if (typeof(config.textExtraction) == "function") {
                        text = config.textExtraction(node);
                    } else {
                        text = $(node).text();
                    }
                }
                return text;
            }

            function appendToTable(table, cache) {

                if (table.config.debug) {
                    var appendTime = new Date();
                }

                var c = cache,
                    r = c.row,
                    n = c.normalized,
                    totalRows = n.length,
                    checkCell = (n[0].length - 1),
                    tableBody = $(table.tBodies[0]),
                    rows = [];


                for (var i = 0; i < totalRows; i++) {
                    var pos = n[i][checkCell];

                    rows.push(r[pos]);

                    if (!table.config.appender) {

                        //var o = ;
                        var l = r[pos].length;
                        for (var j = 0; j < l; j++) {
                            tableBody[0].appendChild(r[pos][j]);
                        }

                        // 
                    }
                }



                if (table.config.appender) {

                    table.config.appender(table, rows);
                }

                rows = null;

                if (table.config.debug) {
                    benchmark("Rebuilt table:", appendTime);
                }

                // apply table widgets
                applyWidget(table);

                // trigger sortend
                setTimeout(function () {
                    $(table).trigger("sortEnd");
                }, 0);

            };

            function buildHeaders(table) {

                if (table.config.debug) {
                    var time = new Date();
                }

                var meta = ($.metadata) ? true : false;
                
                var header_index = computeTableHeaderCellIndexes(table);

                $tableHeaders = $(table.config.selectorHeaders, table).each(function (index) {

                    this.column = header_index[this.parentNode.rowIndex + "-" + this.cellIndex];
                    // this.column = index;
                    this.order = formatSortingOrder(table.config.sortInitialOrder);
                    
					
					this.count = this.order;

                    if (checkHeaderMetadata(this) || checkHeaderOptions(table, index)) this.sortDisabled = true;
					if (checkHeaderOptionsSortingLocked(table, index)) this.order = this.lockedOrder = checkHeaderOptionsSortingLocked(table, index);

                    if (!this.sortDisabled) {
                        var $th = $(this).addClass(table.config.cssHeader);
                        if (table.config.onRenderHeader) table.config.onRenderHeader.apply($th);
                    }

                    // add cell to headerList
                    table.config.headerList[index] = this;
                });

                if (table.config.debug) {
                    benchmark("Built headers:", time);
                    log($tableHeaders);
                }

                return $tableHeaders;

            };

            // from:
            // http://www.javascripttoolbox.com/lib/table/examples.php
            // http://www.javascripttoolbox.com/temp/table_cellindex.html


            function computeTableHeaderCellIndexes(t) {
                var matrix = [];
                var lookup = {};
                var thead = t.getElementsByTagName('THEAD')[0];
                var trs = thead.getElementsByTagName('TR');

                for (var i = 0; i < trs.length; i++) {
                    var cells = trs[i].cells;
                    for (var j = 0; j < cells.length; j++) {
                        var c = cells[j];

                        var rowIndex = c.parentNode.rowIndex;
                        var cellId = rowIndex + "-" + c.cellIndex;
                        var rowSpan = c.rowSpan || 1;
                        var colSpan = c.colSpan || 1;
                        var firstAvailCol;
                        if (typeof(matrix[rowIndex]) == "undefined") {
                            matrix[rowIndex] = [];
                        }
                        // Find first available column in the first row
                        for (var k = 0; k < matrix[rowIndex].length + 1; k++) {
                            if (typeof(matrix[rowIndex][k]) == "undefined") {
                                firstAvailCol = k;
                                break;
                            }
                        }
                        lookup[cellId] = firstAvailCol;
                        for (var k = rowIndex; k < rowIndex + rowSpan; k++) {
                            if (typeof(matrix[k]) == "undefined") {
                                matrix[k] = [];
                            }
                            var matrixrow = matrix[k];
                            for (var l = firstAvailCol; l < firstAvailCol + colSpan; l++) {
                                matrixrow[l] = "x";
                            }
                        }
                    }
                }
                return lookup;
            }

            function checkCellColSpan(table, rows, row) {
                var arr = [],
                    r = table.tHead.rows,
                    c = r[row].cells;

                for (var i = 0; i < c.length; i++) {
                    var cell = c[i];

                    if (cell.colSpan > 1) {
                        arr = arr.concat(checkCellColSpan(table, headerArr, row++));
                    } else {
                        if (table.tHead.length == 1 || (cell.rowSpan > 1 || !r[row + 1])) {
                            arr.push(cell);
                        }
                        // headerArr[row] = (i+row);
                    }
                }
                return arr;
            };

            function checkHeaderMetadata(cell) {
                if (($.metadata) && ($(cell).metadata().sorter === false)) {
                    return true;
                };
                return false;
            }

            function checkHeaderOptions(table, i) {
                if ((table.config.headers[i]) && (table.config.headers[i].sorter === false)) {
                    return true;
                };
                return false;
            }
			
			 function checkHeaderOptionsSortingLocked(table, i) {
                if ((table.config.headers[i]) && (table.config.headers[i].lockedOrder)) return table.config.headers[i].lockedOrder;
                return false;
            }
			
            function applyWidget(table) {
                var c = table.config.widgets;
                var l = c.length;
                for (var i = 0; i < l; i++) {

                    getWidgetById(c[i]).format(table);
                }

            }

            function getWidgetById(name) {
                var l = widgets.length;
                for (var i = 0; i < l; i++) {
                    if (widgets[i].id.toLowerCase() == name.toLowerCase()) {
                        return widgets[i];
                    }
                }
            };

            function formatSortingOrder(v) {
                if (typeof(v) != "Number") {
                    return (v.toLowerCase() == "desc") ? 1 : 0;
                } else {
                    return (v == 1) ? 1 : 0;
                }
            }

            function isValueInArray(v, a) {
                var l = a.length;
                for (var i = 0; i < l; i++) {
                    if (a[i][0] == v) {
                        return true;
                    }
                }
                return false;
            }

            function setHeadersCss(table, $headers, list, css) {
                // remove all header information
                $headers.removeClass(css[0]).removeClass(css[1]);

                var h = [];
                $headers.each(function (offset) {
                    if (!this.sortDisabled) {
                        h[this.column] = $(this);
                    }
                });

                var l = list.length;
                for (var i = 0; i < l; i++) {
                    h[list[i][0]].addClass(css[list[i][1]]);
                }
            }

            function fixColumnWidth(table, $headers) {
                var c = table.config;
                if (c.widthFixed) {
                    var colgroup = $('<colgroup>');
                    $("tr:first td", table.tBodies[0]).each(function () {
                        colgroup.append($('<col>').css('width', $(this).width()));
                    });
                    $(table).prepend(colgroup);
                };
            }

            function updateHeaderSortCount(table, sortList) {
                var c = table.config,
                    l = sortList.length;
                for (var i = 0; i < l; i++) {
                    var s = sortList[i],
                        o = c.headerList[s[0]];
                    o.count = s[1];
                    o.count++;
                }
            }

            /* sorting methods */

            function multisort(table, sortList, cache) {

                if (table.config.debug) {
                    var sortTime = new Date();
                }

                var dynamicExp = "var sortWrapper = function(a,b) {",
                    l = sortList.length;

                // TODO: inline functions.
                for (var i = 0; i < l; i++) {

                    var c = sortList[i][0];
                    var order = sortList[i][1];
                    // var s = (getCachedSortType(table.config.parsers,c) == "text") ?
                    // ((order == 0) ? "sortText" : "sortTextDesc") : ((order == 0) ?
                    // "sortNumeric" : "sortNumericDesc");
                    // var s = (table.config.parsers[c].type == "text") ? ((order == 0)
                    // ? makeSortText(c) : makeSortTextDesc(c)) : ((order == 0) ?
                    // makeSortNumeric(c) : makeSortNumericDesc(c));
                    var s = (table.config.parsers[c].type == "text") ? ((order == 0) ? makeSortFunction("text", "asc", c) : makeSortFunction("text", "desc", c)) : ((order == 0) ? makeSortFunction("numeric", "asc", c) : makeSortFunction("numeric", "desc", c));
                    var e = "e" + i;

                    dynamicExp += "var " + e + " = " + s; // + "(a[" + c + "],b[" + c
                    // + "]); ";
                    dynamicExp += "if(" + e + ") { return " + e + "; } ";
                    dynamicExp += "else { ";

                }

                // if value is the same keep orignal order
                var orgOrderCol = cache.normalized[0].length - 1;
                dynamicExp += "return a[" + orgOrderCol + "]-b[" + orgOrderCol + "];";

                for (var i = 0; i < l; i++) {
                    dynamicExp += "}; ";
                }

                dynamicExp += "return 0; ";
                dynamicExp += "}; ";

                if (table.config.debug) {
                    benchmark("Evaling expression:" + dynamicExp, new Date());
                }

                eval(dynamicExp);

                cache.normalized.sort(sortWrapper);

                if (table.config.debug) {
                    benchmark("Sorting on " + sortList.toString() + " and dir " + order + " time:", sortTime);
                }

                return cache;
            };

            function makeSortFunction(type, direction, index) {
                var a = "a[" + index + "]",
                    b = "b[" + index + "]";
                if (type == 'text' && direction == 'asc') {
                    return "(" + a + " == " + b + " ? 0 : (" + a + " === null ? Number.POSITIVE_INFINITY : (" + b + " === null ? Number.NEGATIVE_INFINITY : (" + a + " < " + b + ") ? -1 : 1 )));";
                } else if (type == 'text' && direction == 'desc') {
                    return "(" + a + " == " + b + " ? 0 : (" + a + " === null ? Number.POSITIVE_INFINITY : (" + b + " === null ? Number.NEGATIVE_INFINITY : (" + b + " < " + a + ") ? -1 : 1 )));";
                } else if (type == 'numeric' && direction == 'asc') {
                    return "(" + a + " === null && " + b + " === null) ? 0 :(" + a + " === null ? Number.POSITIVE_INFINITY : (" + b + " === null ? Number.NEGATIVE_INFINITY : " + a + " - " + b + "));";
                } else if (type == 'numeric' && direction == 'desc') {
                    return "(" + a + " === null && " + b + " === null) ? 0 :(" + a + " === null ? Number.POSITIVE_INFINITY : (" + b + " === null ? Number.NEGATIVE_INFINITY : " + b + " - " + a + "));";
                }
            };

            function makeSortText(i) {
                return "((a[" + i + "] < b[" + i + "]) ? -1 : ((a[" + i + "] > b[" + i + "]) ? 1 : 0));";
            };

            function makeSortTextDesc(i) {
                return "((b[" + i + "] < a[" + i + "]) ? -1 : ((b[" + i + "] > a[" + i + "]) ? 1 : 0));";
            };

            function makeSortNumeric(i) {
                return "a[" + i + "]-b[" + i + "];";
            };

            function makeSortNumericDesc(i) {
                return "b[" + i + "]-a[" + i + "];";
            };

            function sortText(a, b) {
                if (table.config.sortLocaleCompare) return a.localeCompare(b);
                return ((a < b) ? -1 : ((a > b) ? 1 : 0));
            };

            function sortTextDesc(a, b) {
                if (table.config.sortLocaleCompare) return b.localeCompare(a);
                return ((b < a) ? -1 : ((b > a) ? 1 : 0));
            };

            function sortNumeric(a, b) {
                return a - b;
            };

            function sortNumericDesc(a, b) {
                return b - a;
            };

            function getCachedSortType(parsers, i) {
                return parsers[i].type;
            }; /* public methods */
            this.construct = function (settings) {
                return this.each(function () {
                    // if no thead or tbody quit.
                    if (!this.tHead || !this.tBodies) return;
                    // declare
                    var $this, $document, $headers, cache, config, shiftDown = 0,
                        sortOrder;
                    // new blank config object
                    this.config = {};
                    // merge and extend.
                    config = $.extend(this.config, $.tablesorter.defaults, settings);
                    // store common expression for speed
                    $this = $(this);
                    // save the settings where they read
                    $.data(this, "tablesorter", config);
                    // build headers
                    $headers = buildHeaders(this);
                    // try to auto detect column type, and store in tables config
                    this.config.parsers = buildParserCache(this, $headers);
                    // build the cache for the tbody cells
                    cache = buildCache(this);
                    // get the css class names, could be done else where.
                    var sortCSS = [config.cssDesc, config.cssAsc];
                    // fixate columns if the users supplies the fixedWidth option
                    fixColumnWidth(this);
                    // apply event handling to headers
                    // this is to big, perhaps break it out?
                    $headers.click(

                    function (e) {
                        var totalRows = ($this[0].tBodies[0] && $this[0].tBodies[0].rows.length) || 0;
                        if (!this.sortDisabled && totalRows > 0) {
                            // Only call sortStart if sorting is
                            // enabled.
                            $this.trigger("sortStart");
                            // store exp, for speed
                            var $cell = $(this);
                            // get current column index
                            var i = this.column;
                            // get current column sort order
                            this.order = this.count++ % 2;
							// always sort on the locked order.
							if(this.lockedOrder) this.order = this.lockedOrder;
							
							// user only whants to sort on one
                            // column
                            if (!e[config.sortMultiSortKey]) {
                                // flush the sort list
                                config.sortList = [];
                                if (config.sortForce != null) {
                                    var a = config.sortForce;
                                    for (var j = 0; j < a.length; j++) {
                                        if (a[j][0] != i) {
                                            config.sortList.push(a[j]);
                                        }
                                    }
                                }
                                // add column to sort list
                                config.sortList.push([i, this.order]);
                                // multi column sorting
                            } else {
                                // the user has clicked on an all
                                // ready sortet column.
                                if (isValueInArray(i, config.sortList)) {
                                    // revers the sorting direction
                                    // for all tables.
                                    for (var j = 0; j < config.sortList.length; j++) {
                                        var s = config.sortList[j],
                                            o = config.headerList[s[0]];
                                        if (s[0] == i) {
                                            o.count = s[1];
                                            o.count++;
                                            s[1] = o.count % 2;
                                        }
                                    }
                                } else {
                                    // add column to sort list array
                                    config.sortList.push([i, this.order]);
                                }
                            };
                            setTimeout(function () {
                                // set css for headers
                                setHeadersCss($this[0], $headers, config.sortList, sortCSS);
                                appendToTable(
	                                $this[0], multisort(
	                                $this[0], config.sortList, cache)
								);
                            }, 1);
                            // stop normal event by returning false
                            return false;
                        }
                        // cancel selection
                    }).mousedown(function () {
                        if (config.cancelSelection) {
                            this.onselectstart = function () {
                                return false;
                            };
                            return false;
                        }
                    });
                    // apply easy methods that trigger binded events
                    $this.bind("update", function () {
                        var me = this;
                        setTimeout(function () {
                            // rebuild parsers.
                            me.config.parsers = buildParserCache(
                            me, $headers);
                            // rebuild the cache map
                            cache = buildCache(me);
                        }, 1);
                    }).bind("updateCell", function (e, cell) {
                        var config = this.config;
                        // get position from the dom.
                        var pos = [(cell.parentNode.rowIndex - 1), cell.cellIndex];
                        // update cache
                        cache.normalized[pos[0]][pos[1]] = config.parsers[pos[1]].format(
                        getElementText(config, cell), cell);
                    }).bind("sorton", function (e, list) {
                        $(this).trigger("sortStart");
                        config.sortList = list;
                        // update and store the sortlist
                        var sortList = config.sortList;
                        // update header count index
                        updateHeaderSortCount(this, sortList);
                        // set css for headers
                        setHeadersCss(this, $headers, sortList, sortCSS);
                        // sort the table and append it to the dom
                        appendToTable(this, multisort(this, sortList, cache));
                    }).bind("appendCache", function () {
                        appendToTable(this, cache);
                    }).bind("applyWidgetId", function (e, id) {
                        getWidgetById(id).format(this);
                    }).bind("applyWidgets", function () {
                        // apply widgets
                        applyWidget(this);
                    });
                    if ($.metadata && ($(this).metadata() && $(this).metadata().sortlist)) {
                        config.sortList = $(this).metadata().sortlist;
                    }
                    // if user has supplied a sort list to constructor.
                    if (config.sortList.length > 0) {
                        $this.trigger("sorton", [config.sortList]);
                    }
                    // apply widgets
                    applyWidget(this);
                });
            };
            this.addParser = function (parser) {
                var l = parsers.length,
                    a = true;
                for (var i = 0; i < l; i++) {
                    if (parsers[i].id.toLowerCase() == parser.id.toLowerCase()) {
                        a = false;
                    }
                }
                if (a) {
                    parsers.push(parser);
                };
            };
            this.addWidget = function (widget) {
                widgets.push(widget);
            };
            this.formatFloat = function (s) {
                var i = parseFloat(s);
                return (isNaN(i)) ? 0 : i;
            };
            this.formatInt = function (s) {
                var i = parseInt(s);
                return (isNaN(i)) ? 0 : i;
            };
            this.isDigit = function (s, config) {
                // replace all an wanted chars and match.
                return /^[-+]?\d*$/.test($.trim(s.replace(/[,.']/g, '')));
            };
            this.clearTableBody = function (table) {
                if ($.browser.msie) {
                    function empty() {
                        while (this.firstChild)
                        this.removeChild(this.firstChild);
                    }
                    empty.apply(table.tBodies[0]);
                } else {
                    table.tBodies[0].innerHTML = "";
                }
            };
        }
    });

    // extend plugin scope
    $.fn.extend({
        tablesorter: $.tablesorter.construct
    });

    // make shortcut
    var ts = $.tablesorter;

    // add default parsers
    ts.addParser({
        id: "text",
        is: function (s) {
            return true;
        }, format: function (s) {
            return $.trim(s.toLocaleLowerCase());
        }, type: "text"
    });

    ts.addParser({
        id: "digit",
        is: function (s, table) {
            var c = table.config;
            return $.tablesorter.isDigit(s, c);
        }, format: function (s) {
            return $.tablesorter.formatFloat(s);
        }, type: "numeric"
    });

    ts.addParser({
        id: "currency",
        is: function (s) {
            return /^[£$€?.]/.test(s);
        }, format: function (s) {
            return $.tablesorter.formatFloat(s.replace(new RegExp(/[£$€]/g), ""));
        }, type: "numeric"
    });

    ts.addParser({
        id: "ipAddress",
        is: function (s) {
            return /^\d{2,3}[\.]\d{2,3}[\.]\d{2,3}[\.]\d{2,3}$/.test(s);
        }, format: function (s) {
            var a = s.split("."),
                r = "",
                l = a.length;
            for (var i = 0; i < l; i++) {
                var item = a[i];
                if (item.length == 2) {
                    r += "0" + item;
                } else {
                    r += item;
                }
            }
            return $.tablesorter.formatFloat(r);
        }, type: "numeric"
    });

    ts.addParser({
        id: "url",
        is: function (s) {
            return /^(https?|ftp|file):\/\/$/.test(s);
        }, format: function (s) {
            return jQuery.trim(s.replace(new RegExp(/(https?|ftp|file):\/\//), ''));
        }, type: "text"
    });

    ts.addParser({
        id: "isoDate",
        is: function (s) {
            return /^\d{4}[\/-]\d{1,2}[\/-]\d{1,2}$/.test(s);
        }, format: function (s) {
            return $.tablesorter.formatFloat((s != "") ? new Date(s.replace(
            new RegExp(/-/g), "/")).getTime() : "0");
        }, type: "numeric"
    });

    ts.addParser({
        id: "percent",
        is: function (s) {
            return /\%$/.test($.trim(s));
        }, format: function (s) {
            return $.tablesorter.formatFloat(s.replace(new RegExp(/%/g), ""));
        }, type: "numeric"
    });

    ts.addParser({
        id: "usLongDate",
        is: function (s) {
            return s.match(new RegExp(/^[A-Za-z]{3,10}\.? [0-9]{1,2}, ([0-9]{4}|'?[0-9]{2}) (([0-2]?[0-9]:[0-5][0-9])|([0-1]?[0-9]:[0-5][0-9]\s(AM|PM)))$/));
        }, format: function (s) {
            return $.tablesorter.formatFloat(new Date(s).getTime());
        }, type: "numeric"
    });

    ts.addParser({
        id: "shortDate",
        is: function (s) {
            return /\d{1,2}[\/\-]\d{1,2}[\/\-]\d{2,4}/.test(s);
        }, format: function (s, table) {
            var c = table.config;
            s = s.replace(/\-/g, "/");
            if (c.dateFormat == "us") {
                // reformat the string in ISO format
                s = s.replace(/(\d{1,2})[\/\-](\d{1,2})[\/\-](\d{4})/, "$3/$1/$2");
            } else if (c.dateFormat == "uk") {
                // reformat the string in ISO format
                s = s.replace(/(\d{1,2})[\/\-](\d{1,2})[\/\-](\d{4})/, "$3/$2/$1");
            } else if (c.dateFormat == "dd/mm/yy" || c.dateFormat == "dd-mm-yy") {
                s = s.replace(/(\d{1,2})[\/\-](\d{1,2})[\/\-](\d{2})/, "$1/$2/$3");
            }
            return $.tablesorter.formatFloat(new Date(s).getTime());
        }, type: "numeric"
    });
    ts.addParser({
        id: "time",
        is: function (s) {
            return /^(([0-2]?[0-9]:[0-5][0-9])|([0-1]?[0-9]:[0-5][0-9]\s(am|pm)))$/.test(s);
        }, format: function (s) {
            return $.tablesorter.formatFloat(new Date("2000/01/01 " + s).getTime());
        }, type: "numeric"
    });
    ts.addParser({
        id: "metadata",
        is: function (s) {
            return false;
        }, format: function (s, table, cell) {
            var c = table.config,
                p = (!c.parserMetadataName) ? 'sortValue' : c.parserMetadataName;
            return $(cell).metadata()[p];
        }, type: "numeric"
    });
    // add default widgets
    ts.addWidget({
        id: "zebra",
        format: function (table) {
            if (table.config.debug) {
                var time = new Date();
            }
            var $tr, row = -1,
                odd;
            // loop through the visible rows
            $("tr:visible", table.tBodies[0]).each(function (i) {
                $tr = $(this);
                // style children rows the same way the parent
                // row was styled
                if (!$tr.hasClass(table.config.cssChildRow)) row++;
                odd = (row % 2 == 0);
                $tr.removeClass(
                table.config.widgetZebra.css[odd ? 0 : 1]).addClass(
                table.config.widgetZebra.css[odd ? 1 : 0]);
            });
            if (table.config.debug) {
                $.tablesorter.benchmark("Applying Zebra widget", time);
            }
        }
    });
})(jQuery);
/**
 * Timeago is a jQuery plugin that makes it easy to support automatically
 * updating fuzzy timestamps (e.g. "4 minutes ago" or "about 1 day ago").
 *
 * @name timeago
 * @version 1.1.0
 * @requires jQuery v1.2.3+
 * @author Ryan McGeary
 * @license MIT License - http://www.opensource.org/licenses/mit-license.php
 *
 * For usage and examples, visit:
 * http://timeago.yarp.com/
 *
 * Copyright (c) 2008-2013, Ryan McGeary (ryan -[at]- mcgeary [*dot*] org)
 */

(function (factory) {
  if (typeof define === 'function' && define.amd) {
    // AMD. Register as an anonymous module.
    define(['jquery'], factory);
  } else {
    // Browser globals
    factory(jQuery);
  }
}(function ($) {
  $.timeago = function(timestamp) {
    if (timestamp instanceof Date) {
      return inWords(timestamp);
    } else if (typeof timestamp === "string") {
      return inWords($.timeago.parse(timestamp));
    } else if (typeof timestamp === "number") {
      return inWords(new Date(timestamp));
    } else {
      return inWords($.timeago.datetime(timestamp));
    }
  };
  var $t = $.timeago;

  $.extend($.timeago, {
    settings: {
      refreshMillis: 60000,
      allowFuture: false,
      strings: {
        prefixAgo: null,
        prefixFromNow: null,
        suffixAgo: "ago",
        suffixFromNow: "from now",
        seconds: "less than a minute",
        minute: "about a minute",
        minutes: "%d minutes",
        hour: "about an hour",
        hours: "about %d hours",
        day: "a day",
        days: "%d days",
        month: "about a month",
        months: "%d months",
        year: "about a year",
        years: "%d years",
        wordSeparator: " ",
        numbers: []
      }
    },
    inWords: function(distanceMillis) {
      var $l = this.settings.strings;
      var prefix = $l.prefixAgo;
      var suffix = $l.suffixAgo;
      if (this.settings.allowFuture) {
        if (distanceMillis < 0) {
          prefix = $l.prefixFromNow;
          suffix = $l.suffixFromNow;
        }
      }

      var seconds = Math.abs(distanceMillis) / 1000;
      var minutes = seconds / 60;
      var hours = minutes / 60;
      var days = hours / 24;
      var years = days / 365;

      function substitute(stringOrFunction, number) {
        var string = $.isFunction(stringOrFunction) ? stringOrFunction(number, distanceMillis) : stringOrFunction;
        var value = ($l.numbers && $l.numbers[number]) || number;
        return string.replace(/%d/i, value);
      }

      var words = seconds < 45 && substitute($l.seconds, Math.round(seconds)) ||
        seconds < 90 && substitute($l.minute, 1) ||
        minutes < 45 && substitute($l.minutes, Math.round(minutes)) ||
        minutes < 90 && substitute($l.hour, 1) ||
        hours < 24 && substitute($l.hours, Math.round(hours)) ||
        hours < 42 && substitute($l.day, 1) ||
        days < 30 && substitute($l.days, Math.round(days)) ||
        days < 45 && substitute($l.month, 1) ||
        days < 365 && substitute($l.months, Math.round(days / 30)) ||
        years < 1.5 && substitute($l.year, 1) ||
        substitute($l.years, Math.round(years));

      var separator = $l.wordSeparator || "";
      if ($l.wordSeparator === undefined) { separator = " "; }
      return $.trim([prefix, words, suffix].join(separator));
    },
    parse: function(iso8601) {
      var s = $.trim(iso8601);
      s = s.replace(/\.\d+/,""); // remove milliseconds
      s = s.replace(/-/,"/").replace(/-/,"/");
      s = s.replace(/T/," ").replace(/Z/," UTC");
      s = s.replace(/([\+\-]\d\d)\:?(\d\d)/," $1$2"); // -04:00 -> -0400
      return new Date(s);
    },
    datetime: function(elem) {
      var iso8601 = $t.isTime(elem) ? $(elem).attr("datetime") : $(elem).attr("title");
      return $t.parse(iso8601);
    },
    isTime: function(elem) {
      // jQuery's `is()` doesn't play well with HTML5 in IE
      return $(elem).get(0).tagName.toLowerCase() === "time"; // $(elem).is("time");
    }
  });

  // functions that can be called via $(el).timeago('action')
  // init is default when no action is given
  // functions are called with context of a single element
  var functions = {
    init: function(){
      var refresh_el = $.proxy(refresh, this);
      refresh_el();
      var $s = $t.settings;
      if ($s.refreshMillis > 0) {
        setInterval(refresh_el, $s.refreshMillis);
      }
    },
    update: function(time){
      $(this).data('timeago', { datetime: $t.parse(time) });
      refresh.apply(this);
    }
  };

  $.fn.timeago = function(action, options) {
    var fn = action ? functions[action] : functions.init;
    if(!fn){
      throw new Error("Unknown function name '"+ action +"' for timeago");
    }
    // each over objects here and call the requested function
    this.each(function(){
      fn.call(this, options);
    });
    return this;
  };

  function refresh() {
    var data = prepareData(this);
    if (!isNaN(data.datetime)) {
      $(this).text(inWords(data.datetime));
    }
    return this;
  }

  function prepareData(element) {
    element = $(element);
    if (!element.data("timeago")) {
      element.data("timeago", { datetime: $t.datetime(element) });
      var text = $.trim(element.text());
      if (text.length > 0 && !($t.isTime(element) && element.attr("title"))) {
        element.attr("title", text);
      }
    }
    return element.data("timeago");
  }

  function inWords(date) {
    return $t.inWords(distance(date));
  }

  function distance(date) {
    return (new Date().getTime() - date.getTime());
  }

  // fix for IE6 suckage
  document.createElement("abbr");
  document.createElement("time");
}));

/*
 * YafModalDialog by Ingo Herbote  for YAF.NET based on Facebox http://famspam.com/facebox/ by Chris Wanstrath [ chris@ozmm.org ]
 * version: 1.02 (07/07/2012)
 * @requires jQuery v1.4.4 or later
 *
 * Licensed under the MIT:
 *   http://www.opensource.org/licenses/mit-license.php
 */
  
(function($) {
    // jQuery plugin definition
    $.fn.YafModalDialog = function(settings) {

        settings = $.extend({ Dialog: "#MessageBox", ImagePath: "images/", Type: "information" }, settings);

        // traverse all nodes
        this.each(function() {

            $($(this)).click(function() {
                $.fn.YafModalDialog.Show(settings);
            });

        });
        // allow jQuery chaining
        return this;
    };

    // jQuery plugin definition
    $.fn.YafModalDialog.Close = function(settings) {

        settings = $.extend({ Dialog: "#MessageBox", ImagePath: "images/", Type: "information" }, settings);

        var DialogId = settings.Dialog;
        DialogId = DialogId.replace("#", "");

        var MainDialogId = DialogId + 'Box';

        CloseDialog();

        function CloseDialog() {
            $(settings.Dialog).hide();
            $('#' + MainDialogId + '_overlay').fadeOut();
            $(document).unbind('keydown.' + DialogId);

            $('#' + MainDialogId + '_overlay').remove();

            var cnt = $("#" + MainDialogId + " .DialogContent").contents();
            $("#" + MainDialogId).replaceWith(cnt);

            $(settings.Dialog + '#ModalDialog' + ' #' + DialogId + 'Close').remove();
            $(settings.Dialog + '#ModalDialog_overlay').remove();

            return false;
        }

        ;

        // allow jQuery chaining
        return this;
    };

    $.fn.YafModalDialog.Show = function(settings) {

        settings = $.extend({ Dialog: "#MessageBox", ImagePath: "images/", Type: "information" }, settings);

        var dialogIcon = $(settings.Dialog).find(".DialogIcon").eq(0),
        iconSRC = dialogIcon.attr('src');

        var iconsPath = settings.ImagePath;

        iconsPath = iconsPath.replace("images/", "icons/");

        if (settings.Type == 'error') {
            iconSRC = iconsPath + 'ErrorBig.png'; // over write the message to error message
        } else if (settings.Type == 'information') {
            iconSRC = iconsPath + 'InfoBig.png'; // over write the message to information message
        } else if (settings.Type == 'warning') {
            iconSRC = iconsPath + 'WarningBig.png'; // over write the message to warning message
        }

        dialogIcon.attr({ src: iconSRC });

        if ($('#LoginBox').is(':visible')) {
            $.fn.YafModalDialog.Close({ Dialog: '#LoginBox' });
        }

        //var top = getPageScroll()[1] + (getPageHeight() / 100);
        var top = '25%';

        var left = $(window).width() / 2 - 205;

        var cookieScroll = readCookie('ScrollPosition');
        if (cookieScroll != null) {
            eraseCookie('ScrollPosition');
            top = 0;
            top = (parseInt(cookieScroll) + 100) + 'px';
        }

        var DialogId = settings.Dialog;
        DialogId = DialogId.replace("#", "");

        var MainDialogId = DialogId + 'Box';

        $(settings.Dialog).wrapInner("<div id=\"" + MainDialogId + "\" class=\"ModalDialog\" style=\"top: " + top + "; display: block; left: " + left + "px; \"><div class=\"yafpopup\"><div class=\"DialogContent\">");
        $('#' + MainDialogId + ' .popup').after("<a href=\"javascript:void(0);\" class=\"close\" id=\"" + DialogId + "Close\"><img src=\"" + settings.ImagePath + "closelabel.png\" title=\"close\" class=\"close_image\"></a>");
        $(settings.Dialog).after("<div id=\"" + MainDialogId + "_overlay\" class=\"ModalDialog_hide ModalDialog_overlayBG\" style=\"display: none; opacity: 0.2; \"></div>");

        $(settings.Dialog).fadeIn('normal');

        // IE FIX
        $('#' + MainDialogId + '_overlay').css('filter', 'alpha(opacity=20)');

        $('#' + MainDialogId + '_overlay').fadeIn('normal');

        $(document).bind('keydown.' + DialogId, function(e) {
            if (e.keyCode == 27) {
                $.fn.YafModalDialog.Close(settings);
            }
            return true;
        });
        $('#' + DialogId + 'Close').click(function() {
            $.fn.YafModalDialog.Close(settings);
        });
    };

})(jQuery);

function createCookie(name, value) {
    var expires = "";
    document.cookie = name + "=" + value + expires + "; path=/";
}

function readCookie(name) {
    var nameEQ = name + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
    }
    return null;
}

function eraseCookie(name) {
    createCookie(name, "", -1);
}
/***
This is part of jsdifflib v1.0. <http://snowtide.com/jsdifflib>

Copyright (c) 2007, Snowtide Informatics Systems, Inc.
All rights reserved.

Redistribution and use in source and binary forms, with or without modification,
are permitted provided that the following conditions are met:

	* Redistributions of source code must retain the above copyright notice, this
		list of conditions and the following disclaimer.
	* Redistributions in binary form must reproduce the above copyright notice,
		this list of conditions and the following disclaimer in the documentation
		and/or other materials provided with the distribution.
	* Neither the name of the Snowtide Informatics Systems nor the names of its
		contributors may be used to endorse or promote products derived from this
		software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY
EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT
SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED
TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR
BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN
ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH
DAMAGE.
***/
/* Author: Chas Emerick <cemerick@snowtide.com> */
__whitespace = { " ": true, "\t": true, "\n": true, "\f": true, "\r": true };

difflib = {
    defaultJunkFunction: function (c) {
        return c in __whitespace;
    },

    stripLinebreaks: function (str) { return str.replace(/^[\n\r]*|[\n\r]*$/g, ""); },

    stringAsLines: function (str) {
        var lfpos = str.indexOf("\n");
        var crpos = str.indexOf("\r");
        var linebreak = ((lfpos > -1 && crpos > -1) || crpos < 0) ? "\n" : "\r";

        var lines = str.split(linebreak);
        for (var i = 0; i < lines.length; i++) {
            lines[i] = difflib.stripLinebreaks(lines[i]);
        }

        return lines;
    },

    // iteration-based reduce implementation
    __reduce: function (func, list, initial) {
        if (initial != null) {
            var value = initial;
            var idx = 0;
        } else if (list) {
            var value = list[0];
            var idx = 1;
        } else {
            return null;
        }

        for (; idx < list.length; idx++) {
            value = func(value, list[idx]);
        }

        return value;
    },

    // comparison function for sorting lists of numeric tuples
    __ntuplecomp: function (a, b) {
        var mlen = Math.max(a.length, b.length);
        for (var i = 0; i < mlen; i++) {
            if (a[i] < b[i]) return -1;
            if (a[i] > b[i]) return 1;
        }

        return a.length == b.length ? 0 : (a.length < b.length ? -1 : 1);
    },

    __calculate_ratio: function (matches, length) {
        return length ? 2.0 * matches / length : 1.0;
    },

    // returns a function that returns true if a key passed to the returned function
    // is in the dict (js object) provided to this function; replaces being able to
    // carry around dict.has_key in python...
    __isindict: function (dict) {
        return function (key) { return key in dict; };
    },

    // replacement for python's dict.get function -- need easy default values
    __dictget: function (dict, key, defaultValue) {
        return key in dict ? dict[key] : defaultValue;
    },

    SequenceMatcher: function (a, b, isjunk) {
        this.set_seqs = function (a, b) {
            this.set_seq1(a);
            this.set_seq2(b);
        }

        this.set_seq1 = function (a) {
            if (a == this.a) return;
            this.a = a;
            this.matching_blocks = this.opcodes = null;
        }

        this.set_seq2 = function (b) {
            if (b == this.b) return;
            this.b = b;
            this.matching_blocks = this.opcodes = this.fullbcount = null;
            this.__chain_b();
        }

        this.__chain_b = function () {
            var b = this.b;
            var n = b.length;
            var b2j = this.b2j = {};
            var populardict = {};
            for (var i = 0; i < b.length; i++) {
                var elt = b[i];
                if (elt in b2j) {
                    var indices = b2j[elt];
                    if (n >= 200 && indices.length * 100 > n) {
                        populardict[elt] = 1;
                        delete b2j[elt];
                    } else {
                        indices.push(i);
                    }
                } else {
                    b2j[elt] = [i];
                }
            }

            for (var elt in populardict)
                delete b2j[elt];

            var isjunk = this.isjunk;
            var junkdict = {};
            if (isjunk) {
                for (var elt in populardict) {
                    if (isjunk(elt)) {
                        junkdict[elt] = 1;
                        delete populardict[elt];
                    }
                }
                for (var elt in b2j) {
                    if (isjunk(elt)) {
                        junkdict[elt] = 1;
                        delete b2j[elt];
                    }
                }
            }

            this.isbjunk = difflib.__isindict(junkdict);
            this.isbpopular = difflib.__isindict(populardict);
        }

        this.find_longest_match = function (alo, ahi, blo, bhi) {
            var a = this.a;
            var b = this.b;
            var b2j = this.b2j;
            var isbjunk = this.isbjunk;
            var besti = alo;
            var bestj = blo;
            var bestsize = 0;
            var j = null;

            var j2len = {};
            var nothing = [];
            for (var i = alo; i < ahi; i++) {
                var newj2len = {};
                var jdict = difflib.__dictget(b2j, a[i], nothing);
                for (var jkey in jdict) {
                    j = jdict[jkey];
                    if (j < blo) continue;
                    if (j >= bhi) break;
                    newj2len[j] = k = difflib.__dictget(j2len, j - 1, 0) + 1;
                    if (k > bestsize) {
                        besti = i - k + 1;
                        bestj = j - k + 1;
                        bestsize = k;
                    }
                }
                j2len = newj2len;
            }

            while (besti > alo && bestj > blo && !isbjunk(b[bestj - 1]) && a[besti - 1] == b[bestj - 1]) {
                besti--;
                bestj--;
                bestsize++;
            }

            while (besti + bestsize < ahi && bestj + bestsize < bhi &&
					!isbjunk(b[bestj + bestsize]) &&
					a[besti + bestsize] == b[bestj + bestsize]) {
                bestsize++;
            }

            while (besti > alo && bestj > blo && isbjunk(b[bestj - 1]) && a[besti - 1] == b[bestj - 1]) {
                besti--;
                bestj--;
                bestsize++;
            }

            while (besti + bestsize < ahi && bestj + bestsize < bhi && isbjunk(b[bestj + bestsize]) &&
				  a[besti + bestsize] == b[bestj + bestsize]) {
                bestsize++;
            }

            return [besti, bestj, bestsize];
        }

        this.get_matching_blocks = function () {
            if (this.matching_blocks != null) return this.matching_blocks;
            var la = this.a.length;
            var lb = this.b.length;

            var queue = [[0, la, 0, lb]];
            var matching_blocks = [];
            var alo, ahi, blo, bhi, qi, i, j, k, x;
            while (queue.length) {
                qi = queue.pop();
                alo = qi[0];
                ahi = qi[1];
                blo = qi[2];
                bhi = qi[3];
                x = this.find_longest_match(alo, ahi, blo, bhi);
                i = x[0];
                j = x[1];
                k = x[2];

                if (k) {
                    matching_blocks.push(x);
                    if (alo < i && blo < j)
                        queue.push([alo, i, blo, j]);
                    if (i + k < ahi && j + k < bhi)
                        queue.push([i + k, ahi, j + k, bhi]);
                }
            }

            matching_blocks.sort(difflib.__ntuplecomp);

            var i1 = j1 = k1 = block = 0;
            var non_adjacent = [];
            for (var idx in matching_blocks) {
                block = matching_blocks[idx];
                i2 = block[0];
                j2 = block[1];
                k2 = block[2];
                if (i1 + k1 == i2 && j1 + k1 == j2) {
                    k1 += k2;
                } else {
                    if (k1) non_adjacent.push([i1, j1, k1]);
                    i1 = i2;
                    j1 = j2;
                    k1 = k2;
                }
            }

            if (k1) non_adjacent.push([i1, j1, k1]);

            non_adjacent.push([la, lb, 0]);
            this.matching_blocks = non_adjacent;
            return this.matching_blocks;
        }

        this.get_opcodes = function () {
            if (this.opcodes != null) return this.opcodes;
            var i = 0;
            var j = 0;
            var answer = [];
            this.opcodes = answer;
            var block, ai, bj, size, tag;
            var blocks = this.get_matching_blocks();
            for (var idx in blocks) {
                block = blocks[idx];
                ai = block[0];
                bj = block[1];
                size = block[2];
                tag = '';
                if (i < ai && j < bj) {
                    tag = 'replace';
                } else if (i < ai) {
                    tag = 'delete';
                } else if (j < bj) {
                    tag = 'insert';
                }
                if (tag) answer.push([tag, i, ai, j, bj]);
                i = ai + size;
                j = bj + size;

                if (size) answer.push(['equal', ai, i, bj, j]);
            }

            return answer;
        }

        // this is a generator function in the python lib, which of course is not supported in javascript
        // the reimplementation builds up the grouped opcodes into a list in their entirety and returns that.
        this.get_grouped_opcodes = function (n) {
            if (!n) n = 3;
            var codes = this.get_opcodes();
            if (!codes) codes = [["equal", 0, 1, 0, 1]];
            var code, tag, i1, i2, j1, j2;
            if (codes[0][0] == 'equal') {
                code = codes[0];
                tag = code[0];
                i1 = code[1];
                i2 = code[2];
                j1 = code[3];
                j2 = code[4];
                codes[0] = [tag, Math.max(i1, i2 - n), i2, Math.max(j1, j2 - n), j2];
            }
            if (codes[codes.length - 1][0] == 'equal') {
                code = codes[codes.length - 1];
                tag = code[0];
                i1 = code[1];
                i2 = code[2];
                j1 = code[3];
                j2 = code[4];
                codes[codes.length - 1] = [tag, i1, Math.min(i2, i1 + n), j1, Math.min(j2, j1 + n)];
            }

            var nn = n + n;
            var groups = [];
            for (var idx in codes) {
                code = codes[idx];
                tag = code[0];
                i1 = code[1];
                i2 = code[2];
                j1 = code[3];
                j2 = code[4];
                if (tag == 'equal' && i2 - i1 > nn) {
                    groups.push([tag, i1, Math.min(i2, i1 + n), j1, Math.min(j2, j1 + n)]);
                    i1 = Math.max(i1, i2 - n);
                    j1 = Math.max(j1, j2 - n);
                }

                groups.push([tag, i1, i2, j1, j2]);
            }

            if (groups && groups[groups.length - 1][0] == 'equal') groups.pop();

            return groups;
        }

        this.ratio = function () {
            matches = difflib.__reduce(
							function (sum, triple) { return sum + triple[triple.length - 1]; },
							this.get_matching_blocks(), 0);
            return difflib.__calculate_ratio(matches, this.a.length + this.b.length);
        }

        this.quick_ratio = function () {
            var fullbcount, elt;
            if (this.fullbcount == null) {
                this.fullbcount = fullbcount = {};
                for (var i = 0; i < this.b.length; i++) {
                    elt = this.b[i];
                    fullbcount[elt] = difflib.__dictget(fullbcount, elt, 0) + 1;
                }
            }
            fullbcount = this.fullbcount;

            var avail = {};
            var availhas = difflib.__isindict(avail);
            var matches = numb = 0;
            for (var i = 0; i < this.a.length; i++) {
                elt = this.a[i];
                if (availhas(elt)) {
                    numb = avail[elt];
                } else {
                    numb = difflib.__dictget(fullbcount, elt, 0);
                }
                avail[elt] = numb - 1;
                if (numb > 0) matches++;
            }

            return difflib.__calculate_ratio(matches, this.a.length + this.b.length);
        }

        this.real_quick_ratio = function () {
            var la = this.a.length;
            var lb = this.b.length;
            return _calculate_ratio(Math.min(la, lb), la + lb);
        }

        this.isjunk = isjunk ? isjunk : difflib.defaultJunkFunction;
        this.a = this.b = null;
        this.set_seqs(a, b);
    }
}

/***
This is part of jsdifflib v1.0. <http://snowtide.com/jsdifflib>

Copyright (c) 2007, Snowtide Informatics Systems, Inc.
All rights reserved.

Redistribution and use in source and binary forms, with or without modification,
are permitted provided that the following conditions are met:

	* Redistributions of source code must retain the above copyright notice, this
		list of conditions and the following disclaimer.
	* Redistributions in binary form must reproduce the above copyright notice,
		this list of conditions and the following disclaimer in the documentation
		and/or other materials provided with the distribution.
	* Neither the name of the Snowtide Informatics Systems nor the names of its
		contributors may be used to endorse or promote products derived from this
		software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY
EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT
SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED
TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR
BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN
ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH
DAMAGE.
***/
/* Author: Chas Emerick <cemerick@snowtide.com> */
/* Modified by Richard Bondi http://richardbondi.net */
diffview = {
    /**
	 * Builds and returns a visual diff view.  The single parameter, `params', should contain
	 * the following values:
	 *
	 * - baseTextLines: the array of strings that was used as the base text input to SequenceMatcher
	 * - newTextLines: the array of strings that was used as the new text input to SequenceMatcher
	 * - opcodes: the array of arrays returned by SequenceMatcher.get_opcodes()
	 * - baseTextName: the title to be displayed above the base text listing in the diff view; defaults
	 *	   to "Base Text"
	 * - newTextName: the title to be displayed above the new text listing in the diff view; defaults
	 *	   to "New Text"
	 * - contextSize: the number of lines of context to show around differences; by default, all lines
	 *	   are shown
	 * - viewType: if 0, a side-by-side diff view is generated (default); if 1, an inline diff view is
	 *	   generated
	 */
    buildView: function (params) {
        var baseTextLines = params.baseTextLines;
        var newTextLines = params.newTextLines;
        var opcodes = params.opcodes;
        var baseTextName = params.baseTextName ? params.baseTextName : "Base Text";
        var newTextName = params.newTextName ? params.newTextName : "New Text";
        var contextSize = params.contextSize;
        var inline = (params.viewType == 0 || params.viewType == 1) ? params.viewType : 0;

        if (baseTextLines == null)
            throw "Cannot build diff view; baseTextLines is not defined.";
        if (newTextLines == null)
            throw "Cannot build diff view; newTextLines is not defined.";
        if (!opcodes)
            throw "Canno build diff view; opcodes is not defined.";

        function celt(name, clazz) {
            var e = document.createElement(name);
            e.className = clazz;
            return e;
        }

        function telt(name, text) {
            var e = document.createElement(name);
            e.appendChild(document.createTextNode(text));
            return e;
        }

        function ctelt(name, clazz, text) {
            var e = document.createElement(name);
            e.className = clazz;
            e.innerHTML = text;
            return e;
        }

        var tdata = document.createElement("thead");
        var node = document.createElement("tr");
        tdata.appendChild(node);
        if (inline) {
            node.appendChild(document.createElement("th"));
            node.appendChild(document.createElement("th"));
            node.appendChild(ctelt("th", "texttitle", baseTextName + " vs. " + newTextName));
        } else {
            node.appendChild(document.createElement("th"));
            node.appendChild(ctelt("th", "texttitle", baseTextName));
            node.appendChild(document.createElement("th"));
            node.appendChild(ctelt("th", "texttitle", newTextName));
        }
        tdata = [tdata];

        var rows = [];
        var node2;

        /**
		 * Adds two cells to the given row; if the given row corresponds to a real
		 * line number (based on the line index tidx and the endpoint of the 
		 * range in question tend), then the cells will contain the line number
		 * and the line of text from textLines at position tidx (with the class of
		 * the second cell set to the name of the change represented), and tidx + 1 will
		 * be returned.	 Otherwise, tidx is returned, and two empty cells are added
		 * to the given row.
		 */
        function addCells(row, tidx, tend, textLines, change) {
            if (tidx < tend) {
                row.appendChild(telt("th", (tidx + 1).toString()));
                row.appendChild(ctelt("td", change, textLines[tidx].replace(/\t/g, "\u00a0\u00a0\u00a0\u00a0")));
                return tidx + 1;
            } else {
                row.appendChild(document.createElement("th"));
                row.appendChild(celt("td", "empty"));
                return tidx;
            }
        }

        function addCellsInline(row, tidx, tidx2, textLines, change) {
            row.appendChild(telt("th", tidx == null ? "" : (tidx + 1).toString()));
            row.appendChild(telt("th", tidx2 == null ? "" : (tidx2 + 1).toString()));
            row.appendChild(ctelt("td", change, textLines[tidx != null ? tidx : tidx2].replace(/\t/g, "\u00a0\u00a0\u00a0\u00a0")));
        }

        for (var idx = 0; idx < opcodes.length; idx++) {
            code = opcodes[idx];
            change = code[0];
            var b = code[1];
            var be = code[2];
            var n = code[3];
            var ne = code[4];
            var rowcnt = Math.max(be - b, ne - n);
            var toprows = [];
            var botrows = [];
            for (var i = 0; i < rowcnt; i++) {
                // jump ahead if we've alredy provided leading context or if this is the first range
                if (contextSize && opcodes.length > 1 && ((idx > 0 && i == contextSize) || (idx == 0 && i == 0)) && change == "equal") {
                    var jump = rowcnt - ((idx == 0 ? 1 : 2) * contextSize);
                    if (jump > 1) {
                        toprows.push(node = document.createElement("tr"));

                        b += jump;
                        n += jump;
                        i += jump - 1;
                        node.appendChild(telt("th", "..."));
                        if (!inline) node.appendChild(ctelt("td", "skip", ""));
                        node.appendChild(telt("th", "..."));
                        node.appendChild(ctelt("td", "skip", ""));

                        // skip last lines if they're all equal
                        if (idx + 1 == opcodes.length) {
                            break;
                        } else {
                            continue;
                        }
                    }
                }

                toprows.push(node = document.createElement("tr"));
                if (inline) {
                    if (change == "insert") {
                        addCellsInline(node, null, n++, newTextLines, change);
                    } else if (change == "replace") {
                        botrows.push(node2 = document.createElement("tr"));
                        if (b < be) addCellsInline(node, b++, null, baseTextLines, "delete");
                        if (n < ne) addCellsInline(node2, null, n++, newTextLines, "insert");
                    } else if (change == "delete") {
                        addCellsInline(node, b++, null, baseTextLines, change);
                    } else {
                        // equal
                        addCellsInline(node, b++, n++, baseTextLines, change);
                    }
                } else {
                    var wdiff = diffString2(b < be ? baseTextLines[b] : "", n < ne ? newTextLines[n] : "");
                    if (b < be) baseTextLines[b] = wdiff.o;
                    if (n < ne) newTextLines[n] = wdiff.n;
                    b = addCells(node, b, be, baseTextLines, change == "replace" ? "delete" : change);
                    n = addCells(node, n, ne, newTextLines, change == "replace" ? "insert" : change);
                }
            }

            for (var i = 0; i < toprows.length; i++) rows.push(toprows[i]);
            for (var i = 0; i < botrows.length; i++) rows.push(botrows[i]);
        }

        var msg = "combined <a href='http://snowtide.com/jsdifflib'>jsdifflib</a> ";
        msg += "and John Resig's <a href='http://ejohn.org/projects/javascript-diff-algorithm/'>diff</a> ";
        msg += "by <a href='http://richardbondi.net'>Richard Bondi</a>";
        rows.push(node = ctelt("th", "author", msg));
        node.setAttribute("colspan", inline ? 3 : 4);

        tdata.push(node = document.createElement("tbody"));
        for (var idx in rows) node.appendChild(rows[idx]);

        node = celt("table", "diff" + (inline ? " inlinediff" : ""));
        for (var idx in tdata) node.appendChild(tdata[idx]);
        return node;
    }
}

/*
 * Javascript Diff Algorithm
 *  By John Resig (http://ejohn.org/)
 *  Modified by Chu Alan "sprite"
 *  diffstring2 random colors removed by Richard Bondi for use with jsdifflib
 *
 * Released under the MIT license.
 *
 * More Info:
 *  http://ejohn.org/projects/javascript-diff-algorithm/
 *
 */

function escape(s) {
    var n = s;
    n = n.replace(/&/g, "&amp;");
    n = n.replace(/</g, "&lt;");
    n = n.replace(/>/g, "&gt;");
    n = n.replace(/"/g, "&quot;");

    return n;
}

function diffString(o, n) {
    o = o.replace(/\s+$/, '');
    n = n.replace(/\s+$/, '');

    var out = diff(o == "" ? [] : o.split(/\s+/), n == "" ? [] : n.split(/\s+/));
    var str = "";

    var oSpace = o.match(/\s+/g);
    if (oSpace == null) {
        oSpace = ["\n"];
    } else {
        oSpace.push("\n");
    }
    var nSpace = n.match(/\s+/g);
    if (nSpace == null) {
        nSpace = ["\n"];
    } else {
        nSpace.push("\n");
    }

    if (out.n.length == 0) {
        for (var i = 0; i < out.o.length; i++) {
            str += '<del>' + escape(out.o[i]) + oSpace[i] + "</del>";
        }
    } else {
        if (out.n[0].text == null) {
            for (n = 0; n < out.o.length && out.o[n].text == null; n++) {
                str += '<del>' + escape(out.o[n]) + oSpace[n] + "</del>";
            }
        }

        for (var i = 0; i < out.n.length; i++) {
            if (out.n[i].text == null) {
                str += '<ins>' + escape(out.n[i]) + nSpace[i] + "</ins>";
            } else {
                var pre = "";

                for (n = out.n[i].row + 1; n < out.o.length && out.o[n].text == null; n++) {
                    pre += '<del>' + escape(out.o[n]) + oSpace[n] + "</del>";
                }
                str += " " + out.n[i].text + nSpace[i] + pre;
            }
        }
    }

    return str;
}

function randomColor() {
    return "rgb(" + (Math.random() * 100) + "%, " +
                    (Math.random() * 100) + "%, " +
                    (Math.random() * 100) + "%)";
}
function diffString2(o, n) {
    o = o.replace(/\s+$/, '');
    n = n.replace(/\s+$/, '');

    var out = diff(o == "" ? [] : o.split(/\s+/), n == "" ? [] : n.split(/\s+/));

    var oSpace = o.match(/\s+/g);
    if (oSpace == null) {
        oSpace = ["\n"];
    } else {
        oSpace.push("\n");
    }
    var nSpace = n.match(/\s+/g);
    if (nSpace == null) {
        nSpace = ["\n"];
    } else {
        nSpace.push("\n");
    }

    var os = "";
    var colors = new Array();
    for (var i = 0; i < out.o.length; i++) {
        colors[i] = randomColor();

        if (out.o[i].text != null) {
            os += escape(out.o[i].text) + oSpace[i];
        } else {
            os += "<del>" + escape(out.o[i]) + oSpace[i] + "</del>";
        }
    }

    var ns = "";
    for (var i = 0; i < out.n.length; i++) {
        if (out.n[i].text != null) {
            ns += escape(out.n[i].text) + nSpace[i];
        } else {
            ns += "<ins>" + escape(out.n[i]) + nSpace[i] + "</ins>";
        }
    }

    return { o: os, n: ns };
}

function diff(o, n) {
    var ns = new Object();
    var os = new Object();

    for (var i = 0; i < n.length; i++) {
        if (ns[n[i]] == null)
            ns[n[i]] = { rows: new Array(), o: null };
        ns[n[i]].rows.push(i);
    }

    for (var i = 0; i < o.length; i++) {
        if (os[o[i]] == null)
            os[o[i]] = { rows: new Array(), n: null };
        os[o[i]].rows.push(i);
    }

    for (var i in ns) {
        if (ns[i].rows.length == 1 && typeof (os[i]) != "undefined" && os[i].rows.length == 1) {
            n[ns[i].rows[0]] = { text: n[ns[i].rows[0]], row: os[i].rows[0] };
            o[os[i].rows[0]] = { text: o[os[i].rows[0]], row: ns[i].rows[0] };
        }
    }

    for (var i = 0; i < n.length - 1; i++) {
        if (n[i].text != null && n[i + 1].text == null && n[i].row + 1 < o.length && o[n[i].row + 1].text == null &&
             n[i + 1] == o[n[i].row + 1]) {
            n[i + 1] = { text: n[i + 1], row: n[i].row + 1 };
            o[n[i].row + 1] = { text: o[n[i].row + 1], row: i + 1 };
        }
    }

    for (var i = n.length - 1; i > 0; i--) {
        if (n[i].text != null && n[i - 1].text == null && n[i].row > 0 && o[n[i].row - 1].text == null &&
             n[i - 1] == o[n[i].row - 1]) {
            n[i - 1] = { text: n[i - 1], row: n[i].row - 1 };
            o[n[i].row - 1] = { text: o[n[i].row - 1], row: i - 1 };
        }
    }

    return { o: o, n: n };
}
function ChangeReputationBarColor(value, text, selector) {
    jQuery(selector).html('<div class="ui-progressbar-value ui-widget-header ui-corner-left ReputationBarValue" style="width: ' + value + '%; "></div>');
    jQuery(selector).attr('aria-valuenow', value);

    // 0%
    var reputationbarvalue = '.ReputationBarValue';
    var $repbar = jQuery(selector).children(reputationbarvalue);

    if (value == 0) {
        $repbar.addClass("BarDarkRed");
        $repbar.prepend('<p class="ReputationBarText">' + text + '</p>');
    }
        // 1-29%
    else if (value < 20) {

        $repbar.addClass("BarRed");

        $repbar.prepend('<p class="ReputationBarText">' + text + '</p>');
    }
        // 30-39%
    else if (value < 30) {
        $repbar.addClass("BarOrangeRed");

        $repbar.prepend('<p class="ReputationBarText">' + text + '</p>');
    }
        // 40-49%
    else if (value < 40) {
        $repbar.addClass("BarDarkOrange");

        $repbar.prepend('<p class="ReputationBarText">' + text + '</p>');
    }
        // 50-59%
    else if (value < 50) {
        $repbar.addClass("BarOrange");
        $repbar.prepend('<p class="ReputationBarText">' + text + '</p>');
    }
        // 60-69%
    else if (value < 60) {
        $repbar.addClass("BarYellow");

        $repbar.prepend('<p class="ReputationBarText">' + text + '</p>');
    }
        // 70-79%
    else if (value < 80) {
        $repbar.addClass("BarLightGreen");

        $repbar.prepend('<p class="ReputationBarText">' + text + '</p>');
    }
        // 80-89%
    else if (value < 90) {
        $repbar.addClass("BarGreen");

        $repbar.prepend('<p class="ReputationBarText">' + text + '</p>');
    }
        // 90-100%
    else {
        jQuery(selector).html('<div class="ui-progressbar-value ui-widget-header ui-corner-left ui-corner-right ReputationBarValue BarDarkGreen" style="width: ' + value + '%; "><p class="ReputationBarText">' + text + '</p></div>');
    }
}

function ScrollToTop() {
    jQuery('body,html').animate({ scrollTop: 0 }, 820);
    return false;
}


function toggleContainer(id, senderId, showText, hideText) {
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
    if (eventobj.stopPropagation) {
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
    if (evt.srcElement) {
        evt.srcElement.style.cursor = "hand";
    } else if (evt.target) {
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
    var target;

    if (!document.getElementById) {
        throw ('ERROR: missing getElementById');
    }

    if (evt.srcElement)
        target = evt.srcElement;
    else if (evt.target)
        target = evt.target;
    else {
        throw ('ERROR: missing event target');
    }

    var newmenu = document.getElementById(menuName);

    if (window.themenu && window.themenu.id != newmenu.id)
        yaf_hidemenu();

    window.themenu = newmenu;
    if (!window.themenu.style) {
        throw ('ERROR: missing style');
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
    if (window.themenu) {
        jQuery(window.themenu).fadeOut();
        window.themenu = null;
    }
}

function mouseHover(cell, hover) {
    if (hover) {
        cell.className = "popupitemhover";
        try {
            cell.style.cursor = "pointer";
        } catch(e) {
            cell.style.cursor = "hand";
        }
    } else {
        cell.className = "popupitem";
    }
}

// Generic Functions
jQuery(document).ready(function () {
    
    /// <summary>
    /// Convert user posted image to modal images
    /// </summary>
    jQuery(".postContainer .UserPostedImage,.postContainer_Alt .UserPostedImage").each(function() {
        var image = jQuery(this);

        if (!image.parents('a').length) {
            image.wrap('<a href="' + image.attr("src") + '" class="ceebox" title="' + image.attr("alt") + '"/>');
        }
    });

    jQuery(".standardSelectMenu").selectmenu({
        change: function() {
            if (typeof (jQuery(this).attr('onchange')) !== 'undefined') {
                __doPostBack(jQuery(this).attr('name'), '');
            }
        }
    });

    jQuery(".standardSelect2Menu").select2({
        width: '350px'
    });

    if (typeof (jQuery.fn.spinner) !== 'undefined') {
        jQuery('.Numeric').spinner({min: 0});
    }

    jQuery.widget.bridge('uitooltip', $.ui.tooltip);
    jQuery.widget.bridge('uibutton', $.ui.button);

    var dialog = jQuery(".UploadDialog").dialog({
        autoOpen: false,
        width: 530,
        modal: true,
        buttons: {
            Cancel: function () {
                dialog.dialog("close");
            }
        },
        close: function () {
        }
    });

    jQuery(".OpenUploadDialog,.UploadNewFileLine").on("click", function () {
        dialog.dialog("open");
    });

    if (jQuery('#AttachmentsListPager').length) {
        var Attachments_entries = jQuery('#AttachmentsPagerHidden div.result').length;
        jQuery('#AttachmentsListPager').pagination(Attachments_entries, {
            callback: AttachmentsPageSelectCallback,
            items_per_page: 1,
            num_display_entries: 3,
            num_edge_entries: 1,
            prev_class: 'smiliesPagerPrev',
            next_class: 'smiliesPagerNext',
            prev_text: '&laquo;',
            next_text: '&raquo;'
        });
    }

    if (typeof (jQuery.fn.uitooltip) !== 'undefined') {
        jQuery(document).uitooltip({
            items: "[data-url]",
            content: function() {
                var element = $(this);
                var text = element.text();
                var url = element.attr('data-url');
                return "<img alt='" + text + "'  src='" + url + "' style='max-width:300px' />";
            }
        });
    }
});

function AttachmentsPageSelectCallback(page_index) {
    var Attachments_content = jQuery('#AttachmentsPagerHidden div.result:eq(' + page_index + ')').clone();
    jQuery('#AttachmentsPagerResult').empty().append(Attachments_content);
    return false;
}

$(function () {
    $.widget("custom.iconselectmenu", $.ui.selectmenu, {
        _renderItem: function (ul, item) {
            var li = $("<li>", { text: item.label });

            if (item.disabled) {
                li.addClass("ui-state-disabled");
            }

            $("<span>", {
                style: item.element.attr("data-style"),
                "class": "ui-icon " + item.element.attr("data-class")
            })
              .appendTo(li);

            return li.appendTo(ul);
        }
    });
});

function toggleNewSelection(source) {
    var isChecked = source.checked;
    $("input[id*='New']").each(function () {
        $(this).attr('checked', false);
    });
    source.checked = isChecked;
}

function toggleOldSelection(source) {
    var isChecked = source.checked;
    $("input[id*='Old']").each(function () {
        $(this).attr('checked', false);
    });
    source.checked = isChecked;
}

function RenderMessageDiff(messageEditedAtText, nothingSelectedText, selectBothText, selectDifferentText) {
    var oldElement = $("input[id*='New']:checked");
    var newElement = $("input[id*='Old']:checked");

    if (newElement.length && oldElement.length) {
        // check if two different messages are selected
        if ($("input[id*='Old']:checked").attr('id').slice(-1) == $("input[id*='New']:checked").attr('id').slice(-1)) {
            alert(selectDifferentText);
        } else {
            var base = difflib.stringAsLines($("input[id*='Old']:checked").parent().next().next().find("input[id*='MessageField']").attr('value'));
            var newtxt = difflib.stringAsLines($("input[id*='New']:checked").parent().next().find("input[id*='MessageField']").attr('value'));
            var sm = new difflib.SequenceMatcher(base, newtxt);
            var opcodes = sm.get_opcodes();

            $("#diffContent").html('<div class="diffContent">' + diffview.buildView({
                baseTextLines: base,
                newTextLines: newtxt,
                opcodes: opcodes,
                baseTextName: messageEditedAtText + oldElement.parent().next().next().next().next().html(),
                newTextName: messageEditedAtText + oldElement.parent().next().next().next().next().html(),
                contextSize: 3,
                viewType: 0
            }).outerHTML + '</div>');
        }
    }
    else if (newElement.length || oldElement.length) {
        alert(selectBothText);
    } else {
        alert(nothingSelectedText);
    }
}

document.onclick = yaf_hidemenu;
if (document.addEventListener) document.addEventListener("click", function(e) { window.event = e; }, true);
if (document.addEventListener) document.addEventListener("mouseover", function(e) { window.event = e; }, true);
