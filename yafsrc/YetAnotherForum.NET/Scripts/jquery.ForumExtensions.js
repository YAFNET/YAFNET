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
/*!
 * jQuery blockUI plugin
 * Version 2.66.0-2013.10.09
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

      $.blockUI.version = 2.66; // 2nd generation blocking at no extra cost!

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
               opts.onBlock();
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

var XRegExp;
if (XRegExp) {
    throw Error("can't load XRegExp twice in the same frame");
} (function () {
    XRegExp = function (b, c) {
        var d = [],
            currScope = XRegExp.OUTSIDE_CLASS,
            pos = 0,
            context, tokenResult, match, chr, regex;
        if (XRegExp.isRegExp(b)) {
            if (c !== undefined) throw TypeError("can't supply flags when constructing one RegExp from another");
            return clone(b);
        }
        if (isInsideConstructor) throw Error("can't call the XRegExp constructor within token definition functions");
        c = c || "";
        context = {
            hasNamedCapture: false,
            captureNames: [],
            hasFlag: function (a) {
                return c.indexOf(a) > -1;
            },
            setFlag: function (a) {
                c += a;
            }
        };
        while (pos < b.length) {
            tokenResult = runTokens(b, pos, currScope, context);
            if (tokenResult) {
                d.push(tokenResult.output);
                pos += (tokenResult.match[0].length || 1);
            } else {
                if (match = real.exec.call(nativeTokens[currScope], b.slice(pos))) {
                    d.push(match[0]);
                    pos += match[0].length;
                } else {
                    chr = b.charAt(pos);
                    if (chr === "[") currScope = XRegExp.INSIDE_CLASS;
                    else if (chr === "]") currScope = XRegExp.OUTSIDE_CLASS;
                    d.push(chr);
                    pos++;
                }
            }
        }
        regex = RegExp(d.join(""), real.replace.call(c, flagClip, ""));
        regex._xregexp = {
            source: b,
            captureNames: context.hasNamedCapture ? context.captureNames : null
        };
        return regex;
    };
    XRegExp.version = "1.5.0";
    XRegExp.INSIDE_CLASS = 1;
    XRegExp.OUTSIDE_CLASS = 2;
    var j = /\$(?:(\d\d?|[$&`'])|{([$\w]+)})/g,
        flagClip = /[^gimy]+|([\s\S])(?=[\s\S]*\1)/g,
        quantifier = /^(?:[?*+]|{\d+(?:,\d*)?})\??/,
        isInsideConstructor = false,
        tokens = [],
        real = {
            exec: RegExp.prototype.exec,
            test: RegExp.prototype.test,
            match: String.prototype.match,
            replace: String.prototype.replace,
            split: String.prototype.split
        }, compliantExecNpcg = real.exec.call(/()??/, "")[1] === undefined,
        compliantLastIndexIncrement = function () {
            var x = /^/g;
            real.test.call(x, "");
            return !x.lastIndex;
        }(),
        compliantLastIndexReset = function () {
            var x = /x/g;
            real.replace.call("x", x, "");
            return !x.lastIndex;
        }(),
        hasNativeY = RegExp.prototype.sticky !== undefined,
        nativeTokens = {};
    nativeTokens[XRegExp.INSIDE_CLASS] = /^(?:\\(?:[0-3][0-7]{0,2}|[4-7][0-7]?|x[\dA-Fa-f]{2}|u[\dA-Fa-f]{4}|c[A-Za-z]|[\s\S]))/;
    nativeTokens[XRegExp.OUTSIDE_CLASS] = /^(?:\\(?:0(?:[0-3][0-7]{0,2}|[4-7][0-7]?)?|[1-9]\d*|x[\dA-Fa-f]{2}|u[\dA-Fa-f]{4}|c[A-Za-z]|[\s\S])|\(\?[:=!]|[?*+]\?|{\d+(?:,\d*)?}\??)/;
    XRegExp.addToken = function (a, b, c, d) {
        tokens.push({
            pattern: clone(a, "g" + (hasNativeY ? "y" : "")),
            handler: b,
            scope: c || XRegExp.OUTSIDE_CLASS,
            trigger: d || null
        });
    };
    XRegExp.cache = function (a, b) {
        var c = a + "/" + (b || "");
        return XRegExp.cache[c] || (XRegExp.cache[c] = XRegExp(a, b));
    };
    XRegExp.copyAsGlobal = function (a) {
        return clone(a, "g");
    };
    XRegExp.escape = function (a) {
        return a.replace(/[-[\]{}()*+?.,\\^$|#\s]/g, "\\$&");
    };
    XRegExp.execAt = function (a, b, c, d) {
        b = clone(b, "g" + ((d && hasNativeY) ? "y" : ""));
        b.lastIndex = c = c || 0;
        var e = b.exec(a);
        if (d) return (e && e.index === c) ? e : null;
        else return e;
    };
    XRegExp.freezeTokens = function () {
        XRegExp.addToken = function () {
            throw Error("can't run addToken after freezeTokens");
        };
    };
    XRegExp.isRegExp = function (o) {
        return Object.prototype.toString.call(o) === "[object RegExp]";
    };
    XRegExp.iterate = function (a, b, c, d) {
        var e = clone(b, "g"),
            i = -1,
            match;
        while (match = e.exec(a)) {
            c.call(d, match, ++i, a, e);
            if (e.lastIndex === match.index) e.lastIndex++;
        }
        if (b.global) b.lastIndex = 0;
    };
    XRegExp.matchChain = function (e, f) {
        return function recurseChain(b, c) {
            var d = f[c].regex ? f[c] : {
                regex: f[c]
            }, regex = clone(d.regex, "g"),
                matches = [],
                i;
            for (i = 0; i < b.length; i++) {
                XRegExp.iterate(b[i], regex, function (a) {
                    matches.push(d.backref ? (a[d.backref] || "") : a[0]);
                });
            }
            return ((c === f.length - 1) || !matches.length) ? matches : recurseChain(matches, c + 1);
        }([e], 0);
    };
    RegExp.prototype.apply = function (a, b) {
        return this.exec(b[0]);
    };
    RegExp.prototype.call = function (a, b) {
        return this.exec(b);
    };
    RegExp.prototype.exec = function (a) {
        var b = real.exec.apply(this, arguments),
            name, r2;
        if (b) {
            if (!compliantExecNpcg && b.length > 1 && indexOf(b, "") > -1 && a) {
                r2 = RegExp(this.source, real.replace.call(getNativeFlags(this), "g", ""));
                real.replace.call(a.toString().slice(b.index), r2, function () {
                    for (var i = 1; i < arguments.length - 2; i++) {
                        if (arguments[i] === undefined) b[i] = undefined;
                    }
                });
            }
            if (this._xregexp && this._xregexp.captureNames) {
                for (var i = 1; i < b.length; i++) {
                    name = this._xregexp.captureNames[i - 1];
                    if (name) b[name] = b[i];
                }
            }
            if (!compliantLastIndexIncrement && this.global && !b[0].length && (this.lastIndex > b.index)) this.lastIndex--;
        }
        return b;
    };
    if (!compliantLastIndexIncrement) {
        RegExp.prototype.test = function (a) {
            var b = real.exec.call(this, a);
            if (b && this.global && !b[0].length && (this.lastIndex > b.index)) this.lastIndex--;
            return !!b;
        };
    }
    String.prototype.match = function (a) {
        if (!XRegExp.isRegExp(a)) a = RegExp(a);
        if (a.global) {
            var b = real.match.apply(this, arguments);
            a.lastIndex = 0;
            return b;
        }
        return a.exec(this);
    };
    String.prototype.replace = function (f, g) {
        var h = XRegExp.isRegExp(f),
            captureNames, result, str;
        if (h && typeof g.valueOf() === "string" && g.indexOf("${") === -1 && compliantLastIndexReset) return real.replace.apply(this, arguments);
        if (!h) f = f + "";
        else if (f._xregexp) captureNames = f._xregexp.captureNames;
        if (typeof g === "function") {
            result = real.replace.call(this, f, function () {
                if (captureNames) {
                    arguments[0] = new String(arguments[0]);
                    for (var i = 0; i < captureNames.length; i++) {
                        if (captureNames[i]) arguments[0][captureNames[i]] = arguments[i + 1];
                    }
                }
                if (h && f.global) f.lastIndex = arguments[arguments.length - 2] + arguments[0].length;
                return g.apply(null, arguments);
            });
        } else {
            str = this + "";
            result = real.replace.call(str, f, function () {
                var e = arguments;
                return real.replace.call(g, j, function (a, b, c) {
                    if (b) {
                        switch (b) {
                            case "$":
                                return "$";
                            case "&":
                                return e[0];
                            case "`":
                                return e[e.length - 1].slice(0, e[e.length - 2]);
                            case "'":
                                return e[e.length - 1].slice(e[e.length - 2] + e[0].length);
                            default:
                                var d = "";
                                b = +b;
                                if (!b) return a;
                                while (b > e.length - 3) {
                                    d = String.prototype.slice.call(b, -1) + d;
                                    b = Math.floor(b / 10);
                                }
                                return (b ? e[b] || "" : "$") + d;
                        }
                    } else {
                        var n = +c;
                        if (n <= e.length - 3) return e[n];
                        n = captureNames ? indexOf(captureNames, c) : -1;
                        return n > -1 ? e[n + 1] : a;
                    }
                });
            });
        } if (h && f.global) f.lastIndex = 0;
        return result;
    };
    String.prototype.split = function (s, a) {
        if (!XRegExp.isRegExp(s)) return real.split.apply(this, arguments);
        var b = this + "",
            output = [],
            lastLastIndex = 0,
            match, lastLength;
        if (a === undefined || +a < 0) {
            a = Infinity;
        } else {
            a = Math.floor(+a);
            if (!a) return [];
        }
        s = XRegExp.copyAsGlobal(s);
        while (match = s.exec(b)) {
            if (s.lastIndex > lastLastIndex) {
                output.push(b.slice(lastLastIndex, match.index));
                if (match.length > 1 && match.index < b.length) Array.prototype.push.apply(output, match.slice(1));
                lastLength = match[0].length;
                lastLastIndex = s.lastIndex;
                if (output.length >= a) break;
            }
            if (s.lastIndex === match.index) s.lastIndex++;
        }
        if (lastLastIndex === b.length) {
            if (!real.test.call(s, "") || lastLength) output.push("");
        } else {
            output.push(b.slice(lastLastIndex));
        }
        return output.length > a ? output.slice(0, a) : output;
    };
    function clone(a, b) {
        if (!XRegExp.isRegExp(a)) throw TypeError("type RegExp expected");
        var x = a._xregexp;
        a = XRegExp(a.source, getNativeFlags(a) + (b || ""));
        if (x) {
            a._xregexp = {
                source: x.source,
                captureNames: x.captureNames ? x.captureNames.slice(0) : null
            };
        }
        return a;
    };
    function getNativeFlags(a) {
        return (a.global ? "g" : "") + (a.ignoreCase ? "i" : "") + (a.multiline ? "m" : "") + (a.extended ? "x" : "") + (a.sticky ? "y" : "");
    };
    function runTokens(a, b, c, d) {
        var i = tokens.length,
            result, match, t;
        isInsideConstructor = true;
        try {
            while (i--) {
                t = tokens[i];
                if ((c & t.scope) && (!t.trigger || t.trigger.call(d))) {
                    t.pattern.lastIndex = b;
                    match = t.pattern.exec(a);
                    if (match && match.index === b) {
                        result = {
                            output: t.handler.call(d, match, c),
                            match: match
                        };
                        break;
                    }
                }
            }
        } catch (err) {
            throw err;
        } finally {
            isInsideConstructor = false;
        }
        return result;
    };
    function indexOf(a, b, c) {
        if (Array.prototype.indexOf) return a.indexOf(b, c);
        for (var i = c || 0; i < a.length; i++) {
            if (a[i] === b) return i;
        }
        return -1;
    };
    XRegExp.addToken(/\(\?#[^)]*\)/, function (a) {
        return real.test.call(quantifier, a.input.slice(a.index + a[0].length)) ? "" : "(?:)";
    });
    XRegExp.addToken(/\((?!\?)/, function () {
        this.captureNames.push(null);
        return "(";
    });
    XRegExp.addToken(/\(\?<([$\w]+)>/, function (a) {
        this.captureNames.push(a[1]);
        this.hasNamedCapture = true;
        return "(";
    });
    XRegExp.addToken(/\\k<([\w$]+)>/, function (a) {
        var b = indexOf(this.captureNames, a[1]);
        return b > -1 ? "\\" + (b + 1) + (isNaN(a.input.charAt(a.index + a[0].length)) ? "" : "(?:)") : a[0];
    });
    XRegExp.addToken(/\[\^?]/, function (a) {
        return a[0] === "[]" ? "\\b\\B" : "[\\s\\S]";
    });
    XRegExp.addToken(/^\(\?([imsx]+)\)/, function (a) {
        this.setFlag(a[1]);
        return "";
    });
    XRegExp.addToken(/(?:\s+|#.*)+/, function (a) {
        return real.test.call(quantifier, a.input.slice(a.index + a[0].length)) ? "" : "(?:)";
    }, XRegExp.OUTSIDE_CLASS, function () {
        return this.hasFlag("x");
    });
    XRegExp.addToken(/\./, function () {
        return "[\\s\\S]";
    }, XRegExp.OUTSIDE_CLASS, function () {
        return this.hasFlag("s");
    });
})();
typeof (exports) != 'undefined' ? exports.XRegExp = XRegExp : null;


if (typeof SyntaxHighlighter == "undefined") var SyntaxHighlighter = function () {
    function r(a, b) {
        a.className.indexOf(b) != -1 || (a.className += " " + b);
    }
    function t(a) {
        return a.indexOf("highlighter_") == 0 ? a : "highlighter_" + a;
    }
    function B(a) {
        return f.vars.highlighters[t(a)];
    }
    function p(a, b, c) {
        if (a == null) return null;
        var d = c != true ? a.childNodes : [a.parentNode],
            h = {
                "#": "id",
                ".": "className"
            }[b.substr(0, 1)] || "nodeName",
            g, i;
        g = h != "nodeName" ? b.substr(1) : b.toUpperCase();
        if ((a[h] || "").indexOf(g) != -1) return a;
        for (a = 0; d && a < d.length && i == null; a++) i = p(d[a], b, c);
        return i;
    }
    function C(a, b) {
        var c = {}, d;
        for (d in a) c[d] = a[d];
        for (d in b) c[d] = b[d];
        return c;
    }
    function w(a, b, c, d) {
        function h(g) {
            g = g || window.event;
            if (!g.target) {
                g.target = g.srcElement;
                g.preventDefault = function () {
                    this.returnValue = false;
                };
            }
            c.call(d || window, g);
        }
        a.attachEvent ? a.attachEvent("on" + b, h) : a.addEventListener(b, h, false);
    }
    function A(a, b) {
        var c = f.vars.discoveredBrushes,
            d = null;
        if (c == null) {
            c = {};
            for (var h in f.brushes) {
                var g = f.brushes[h];
                d = g.aliases;
                if (d != null) {
                    g.brushName = h.toLowerCase();
                    for (g = 0; g < d.length; g++) c[d[g]] = h;
                }
            }
            f.vars.discoveredBrushes = c;
        }
        d = f.brushes[c[a]];
        d == null && b != false && window.alert(f.config.strings.alert + (f.config.strings.noBrush + a));
        return d;
    }
    function v(a, b) {
        for (var c = a.split("\n"), d = 0; d < c.length; d++) c[d] = b(c[d], d);
        return c.join("\n");
    }
    function u(a, b) {
        if (a == null || a.length == 0 || a == "\n") return a;
        a = a.replace(/</g, "&lt;");
        a = a.replace(/ {2,}/g, function (c) {
            for (var d = "", h = 0; h < c.length - 1; h++) d += f.config.space;
            return d + " ";
        });
        if (b != null) a = v(a, function (c) {
            if (c.length == 0) return "";
            var d = "";
            c = c.replace(/^(&nbsp;| )+/, function (h) {
                d = h;
                return "";
            });
            if (c.length == 0) return d;
            return d + '<code class="' + b + '">' + c + "</code>";
        });
        return a;
    }
    function n(a, b) {
        a.split("\n");
        for (var c = "", d = 0; d < 50; d++) c += "                    ";
        return a = v(a, function (h) {
            if (h.indexOf("\t") == -1) return h;
            for (var g = 0;
            (g = h.indexOf("\t")) != -1;) h = h.substr(0, g) + c.substr(0, b - g % b) + h.substr(g + 1, h.length);
            return h;
        });
    }
    function x(a) {
        return a.replace(/^\s+|\s+$/g, "");
    }
    function D(a, b) {
        if (a.index < b.index) return -1;
        else if (a.index > b.index) return 1;
        else if (a.length < b.length) return -1;
        else if (a.length > b.length) return 1;
        return 0;
    }
    function y(a, b) {
        function c(k) {
            return k[0];
        }
        for (var d = null, h = [], g = b.func ? b.func : c;
        (d = b.regex.exec(a)) != null;) {
            var i = g(d, b);
            if (typeof i == "string") i = [new f.Match(i, d.index, b.css)];
            h = h.concat(i);
        }
        return h;
    }
    function E(a) {
        var b = /(.*)((&gt;|&lt;).*)/;
        return a.replace(f.regexLib.url, function (c) {
            var d = "",
                h = null;
            if (h = b.exec(c)) {
                c = h[1];
                d = h[2];
            }
            return '<a href="' + c + '">' + c + "</a>" + d;
        });
    }
    function z() {
        for (var a = document.getElementsByTagName("script"), b = [], c = 0; c < a.length; c++) a[c].type == "syntaxhighlighter" && b.push(a[c]);
        return b;
    }
    function e(a) {
        a = a.target;
        var b = p(a, ".syntaxhighlighter", true);
        a = p(a, ".container", true);
        var c = document.createElement("textarea");
        if (!(!a || !b || p(a, "textarea"))) {
            B(b.id);
            r(b, "source");
            for (var d = a.childNodes, h = [], g = 0; g < d.length; g++) h.push(d[g].innerText || d[g].textContent);
            h = h.join("\r");
            c.appendChild(document.createTextNode(h));
            a.appendChild(c);
            c.focus();
            c.select();
            w(c, "blur", function () {
                c.parentNode.removeChild(c);
                b.className = b.className.replace("source", "");
            });
        }
    }
    if (typeof require != "undefined" && typeof XRegExp == "undefined") XRegExp = require("XRegExp").XRegExp;
    var f = {
        defaults: {
            "class-name": "",
            "first-line": 1,
            "pad-line-numbers": false,
            highlight: null,
            title: null,
            "smart-tabs": true,
            "tab-size": 4,
            gutter: true,
            toolbar: true,
            "quick-code": true,
            collapse: false,
            "auto-links": true,
            light: false,
            "html-script": false
        },
        config: {
            space: "&nbsp;",
            useScriptTags: true,
            bloggerMode: false,
            stripBrs: false,
            tagName: "pre",
            strings: {
                expandSource: "expand source",
                help: "?",
                alert: "SyntaxHighlighter\n\n",
                noBrush: "Can't find brush for: ",
                brushNotHtmlScript: "Brush wasn't configured for html-script option: ",
                aboutDialog: '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"><html xmlns="http://www.w3.org/1999/xhtml"><head><meta http-equiv="Content-Type" content="text/html; charset=utf-8" /><title>About SyntaxHighlighter</title></head><body style="font-family:Geneva,Arial,Helvetica,sans-serif;background-color:#fff;color:#000;font-size:1em;text-align:center;"><div style="text-align:center;margin-top:1.5em;"><div style="font-size:xx-large;">SyntaxHighlighter</div><div style="font-size:.75em;margin-bottom:3em;"><div>version 3.0.87 (November 12 2010)</div><div><a href="http://alexgorbatchev.com/SyntaxHighlighter" target="_blank" style="color:#005896">http://alexgorbatchev.com/SyntaxHighlighter</a></div><div>JavaScript code syntax highlighter.</div><div>Copyright 2004-2010 Alex Gorbatchev.</div></div><div>If you like this script, please <a href="https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=2930402" style="color:#005896">donate</a> to <br/>keep development active!</div></div></body></html>'
            }
        },
        vars: {
            discoveredBrushes: null,
            highlighters: {}
        },
        brushes: {},
        regexLib: {
            multiLineCComments: /\/\*[\s\S]*?\*\//gm,
            singleLineCComments: /\/\/.*$/gm,
            singleLinePerlComments: /#.*$/gm,
            doubleQuotedString: /"([^\\"\n]|\\.)*"/g,
            singleQuotedString: /'([^\\'\n]|\\.)*'/g,
            multiLineDoubleQuotedString: new XRegExp('"([^\\\\"]|\\\\.)*"', "gs"),
            multiLineSingleQuotedString: new XRegExp("'([^\\\\']|\\\\.)*'", "gs"),
            xmlComments: /(&lt;|<)!--[\s\S]*?--(&gt;|>)/gm,
            url: /\w+:\/\/[\w-.\/?%&=:@;]*/g,
            phpScriptTags: {
                left: /(&lt;|<)\?=?/g,
                right: /\?(&gt;|>)/g
            },
            aspScriptTags: {
                left: /(&lt;|<)%=?/g,
                right: /%(&gt;|>)/g
            },
            scriptScriptTags: {
                left: /(&lt;|<)\s*script.*?(&gt;|>)/gi,
                right: /(&lt;|<)\/\s*script\s*(&gt;|>)/gi
            }
        },
        toolbar: {
            getHtml: function (a) {
                function b(i, k) {
                    return f.toolbar.getButtonHtml(i, k, f.config.strings[k]);
                }
                for (var c = '<div class="toolbar">', d = f.toolbar.items, h = d.list, g = 0; g < h.length; g++) c += (d[h[g]].getHtml || b)(a, h[g]);
                c += "</div>";
                return c;
            },
            getButtonHtml: function (a, b, c) {
                return '<span><a href="#" class="toolbar_item command_' + b + " " + b + '">' + c + "</a></span>";
            },
            handler: function (a) {
                var b = a.target,
                    c = b.className || "";
                b = B(p(b, ".syntaxhighlighter", true).id);
                var d = function (h) {
                    return (h = RegExp(h + "_(\\w+)").exec(c)) ? h[1] : null;
                }("command");
                b && d && f.toolbar.items[d].execute(b);
                a.preventDefault();
            },
            items: {
                list: ["expandSource", "help"],
                expandSource: {
                    getHtml: function (a) {
                        if (a.getParam("collapse") != true) return "";
                        var b = a.getParam("title");
                        return f.toolbar.getButtonHtml(a, "expandSource", b ? b : f.config.strings.expandSource);
                    },
                    execute: function (a) {
                        a = document.getElementById(t(a.id));
                        a.className = a.className.replace("collapsed", "");
                    }
                },
                help: {
                    execute: function () {
                        var a = "scrollbars=0";
                        a += ", left=" + (screen.width - 500) / 2 + ", top=" + (screen.height - 250) / 2 + ", width=500, height=250";
                        a = a.replace(/^,/, "");
                        a = window.open("", "_blank", a);
                        a.focus();
                        var b = a.document;
                        b.write(f.config.strings.aboutDialog);
                        b.close();
                        a.focus();
                    }
                }
            }
        },
        findElements: function (a, b) {
            var c;
            if (b) c = [b];
            else {
                c = document.getElementsByTagName(f.config.tagName);
                for (var d = [], h = 0; h < c.length; h++) d.push(c[h]);
                c = d;
            }
            c = c;
            d = [];
            if (f.config.useScriptTags) c = c.concat(z());
            if (c.length === 0) return d;
            for (h = 0; h < c.length; h++) {
                for (var g = c[h], i = a, k = c[h].className, j = void 0, l = {}, m = new XRegExp("^\\[(?<values>(.*?))\\]$"), s = new XRegExp("(?<name>[\\w-]+)\\s*:\\s*(?<value>[\\w-%#]+|\\[.*?\\]|\".*?\"|'.*?')\\s*;?", "g") ;
                (j = s.exec(k)) != null;) {
                    var o = j.value.replace(/^['"]|['"]$/g, "");
                    if (o != null && m.test(o)) {
                        o = m.exec(o);
                        o = o.values.length > 0 ? o.values.split(/\s*,\s*/) : [];
                    }
                    l[j.name] = o;
                }
                g = {
                    target: g,
                    params: C(i, l)
                };
                g.params.brush != null && d.push(g);
            }
            return d;
        },
        highlight: function (a, b) {
            var c = this.findElements(a, b),
                d = null,
                h = f.config;
            if (c.length !== 0) for (var g = 0; g < c.length; g++) {
                b = c[g];
                var i = b.target,
                    k = b.params,
                    j = k.brush,
                    l;
                if (j != null) {
                    if (k["html-script"] == "true" || f.defaults["html-script"] == true) {
                        d = new f.HtmlScript(j);
                        j = "htmlscript";
                    } else if (d = A(j)) d = new d;
                    else continue;
                    l = i.innerHTML;
                    if (h.useScriptTags) {
                        l = l;
                        var m = x(l),
                            s = false;
                        if (m.indexOf("<![CDATA[") == 0) {
                            m = m.substring(9);
                            s = true;
                        }
                        var o = m.length;
                        if (m.indexOf("]]\>") == o - 3) {
                            m = m.substring(0, o - 3);
                            s = true;
                        }
                        l = s ? m : l;
                    }
                    if ((i.title || "") != "") k.title = i.title;
                    k.brush = j;
                    d.init(k);
                    b = d.getDiv(l);
                    if ((i.id || "") != "") b.id = i.id;
                    i.parentNode.replaceChild(b, i);
                }
            }
        },
        all: function (a) {
            w(window, "load", function () {
                f.highlight(a);
            });
        }
    };
    f.Match = function (a, b, c) {
        this.value = a;
        this.index = b;
        this.length = a.length;
        this.css = c;
        this.brushName = null;
    };
    f.Match.prototype.toString = function () {
        return this.value;
    };
    f.HtmlScript = function (a) {
        function b(j, l) {
            for (var m = 0; m < j.length; m++) j[m].index += l;
        }
        var c = A(a),
            d, h = new f.brushes.Xml,
            g = this,
            i = "getDiv getHtml init".split(" ");
        if (c != null) {
            d = new c;
            for (var k = 0; k < i.length; k++) (function () {
                var j = i[k];
                g[j] = function () {
                    return h[j].apply(h, arguments);
                };
            })();
            d.htmlScript == null ? window.alert(f.config.strings.alert + (f.config.strings.brushNotHtmlScript + a)) : h.regexList.push({
                regex: d.htmlScript.code,
                func: function (j) {
                    for (var l = j.code, m = [], s = d.regexList, o = j.index + j.left.length, F = d.htmlScript, q, G = 0; G < s.length; G++) {
                        q = y(l, s[G]);
                        b(q, o);
                        m = m.concat(q);
                    }
                    if (F.left != null && j.left != null) {
                        q = y(j.left, F.left);
                        b(q, j.index);
                        m = m.concat(q);
                    }
                    if (F.right != null && j.right != null) {
                        q = y(j.right, F.right);
                        b(q, j.index + j[0].lastIndexOf(j.right));
                        m = m.concat(q);
                    }
                    for (j = 0; j < m.length; j++) m[j].brushName = c.brushName;
                    return m;
                }
            });
        }
    };
    f.Highlighter = function () { };
    f.Highlighter.prototype = {
        getParam: function (a, b) {
            var c = this.params[a];
            c = c == null ? b : c;
            var d = {
                "true": true,
                "false": false
            }[c];
            return d == null ? c : d;
        },
        create: function (a) {
            return document.createElement(a);
        },
        findMatches: function (a, b) {
            var c = [];
            if (a != null) for (var d = 0; d < a.length; d++) if (typeof a[d] == "object") c = c.concat(y(b, a[d]));
            return this.removeNestedMatches(c.sort(D));
        },
        removeNestedMatches: function (a) {
            for (var b = 0; b < a.length; b++) if (a[b] !== null) for (var c = a[b], d = c.index + c.length, h = b + 1; h < a.length && a[b] !== null; h++) {
                var g = a[h];
                if (g !== null) if (g.index > d) break;
                else if (g.index == c.index && g.length > c.length) a[b] = null;
                else if (g.index >= c.index && g.index < d) a[h] = null;
                                                                  }
            return a;
        },
        figureOutLineNumbers: function (a) {
            var b = [],
                c = parseInt(this.getParam("first-line"));
            v(a, function (d, h) {
                b.push(h + c);
            });
            return b;
        },
        isLineHighlighted: function (a) {
            var b = this.getParam("highlight", []);
            if (typeof b != "object" && b.push == null) b = [b];
            a: {
                a = a.toString();
                var c = void 0;
                for (c = c = Math.max(c || 0, 0) ; c < b.length; c++) if (b[c] == a) {
                    b = c;
                    break a;
                                                                      }
                b = -1;
               }
            return b != -1;
        },
        getLineHtml: function (a, b, c) {
            a = ["line", "number" + b, "index" + a, "alt" + (b % 2 == 0 ? 1 : 2).toString()];
            this.isLineHighlighted(b) && a.push("highlighted");
            b == 0 && a.push("break");
            return '<div class="' + a.join(" ") + '">' + c + "</div>";
        },
        getLineNumbersHtml: function (a, b) {
            var c = "",
                d = a.split("\n").length,
                h = parseInt(this.getParam("first-line")),
                g = this.getParam("pad-line-numbers");
            if (g == true) g = (h + d - 1).toString().length;
            else if (isNaN(g) == true) g = 0;
            for (var i = 0; i < d; i++) {
                var k = b ? b[i] : h + i,
                    j;
                if (k == 0) j = f.config.space;
                else {
                    j = g;
                    for (var l = k.toString() ; l.length < j;) l = "0" + l;
                    j = l;
                }
                a = j;
                c += this.getLineHtml(i, k, a);
            }
            return c;
        },
        getCodeLinesHtml: function (a, b) {
            a = x(a);
            var c = a.split("\n");
            this.getParam("pad-line-numbers");
            var d = parseInt(this.getParam("first-line"));
            a = "";
            for (var h = this.getParam("brush"), g = 0; g < c.length; g++) {
                var i = c[g],
                    k = /^(&nbsp;|\s)+/.exec(i),
                    j = null,
                    l = b ? b[g] : d + g;
                if (k != null) {
                    j = k[0].toString();
                    i = i.substr(j.length);
                    j = j.replace(" ", f.config.space);
                }
                i = x(i);
                if (i.length == 0) i = f.config.space;
                a += this.getLineHtml(g, l, (j != null ? '<code class="' + h + ' spaces">' + j + "</code>" : "") + i);
            }
            return a;
        },
        getTitleHtml: function (a) {
            return a ? "<caption>" + a + "</caption>" : "";
        },
        getMatchesHtml: function (a, b) {
            function c(l) {
                return (l = l ? l.brushName || g : g) ? l + " " : "";
            }
            for (var d = 0, h = "", g = this.getParam("brush", ""), i = 0; i < b.length; i++) {
                var k = b[i],
                    j;
                if (!(k === null || k.length === 0)) {
                    j = c(k);
                    h += u(a.substr(d, k.index - d), j + "plain") + u(k.value, j + k.css);
                    d = k.index + k.length + (k.offset || 0);
                }
            }
            h += u(a.substr(d), c() + "plain");
            return h;
        },
        getHtml: function (a) {
            var b = "",
                c = ["syntaxhighlighter"],
                d;
            if (this.getParam("light") == true) this.params.toolbar = this.params.gutter = false;
            className = "syntaxhighlighter";
            this.getParam("collapse") == true && c.push("collapsed");
            if ((gutter = this.getParam("gutter")) == false) c.push("nogutter");
            c.push(this.getParam("class-name"));
            c.push(this.getParam("brush"));
            a = a.replace(/^[ ]*[\n]+|[\n]*[ ]*$/g, "").replace(/\r/g, " ");
            b = this.getParam("tab-size");
            if (this.getParam("smart-tabs") == true) a = n(a, b);
            else {
                for (var h = "", g = 0; g < b; g++) h += " ";
                a = a.replace(/\t/g, h);
            }
            a = a;
            a: {
                b = a = a;
                h = /<br\s*\/?>|&lt;br\s*\/?&gt;/gi;
                if (f.config.bloggerMode == true) b = b.replace(h, "\n");
                if (f.config.stripBrs == true) b = b.replace(h, "");
                b = b.split("\n");
                h = /^\s*/;
                g = 1E3;
                for (var i = 0; i < b.length && g > 0; i++) {
                    var k = b[i];
                    if (x(k).length != 0) {
                        k = h.exec(k);
                        if (k == null) {
                            a = a;
                            break a;
                        }
                        g = Math.min(k[0].length, g);
                    }
                }
                if (g > 0) for (i = 0; i < b.length; i++) b[i] = b[i].substr(g);
                a = b.join("\n");
               }
            if (gutter) d = this.figureOutLineNumbers(a);
            b = this.findMatches(this.regexList, a);
            b = this.getMatchesHtml(a, b);
            b = this.getCodeLinesHtml(b, d);
            if (this.getParam("auto-links")) b = E(b);
            typeof navigator != "undefined" && navigator.userAgent && navigator.userAgent.match(/MSIE/) && c.push("ie");
            return b = '<div id="' + t(this.id) + '" class="' + c.join(" ") + '">' + (this.getParam("toolbar") ? f.toolbar.getHtml(this) : "") + '<table border="0" cellpadding="0" cellspacing="0">' + this.getTitleHtml(this.getParam("title")) + "<tbody><tr>" + (gutter ? '<td class="gutter">' + this.getLineNumbersHtml(a) + "</td>" : "") + '<td class="code"><div class="container">' + b + "</div></td></tr></tbody></table></div>";
        },
        getDiv: function (a) {
            if (a === null) a = "";
            this.code = a;
            var b = this.create("div");
            b.innerHTML = this.getHtml(a);
            this.getParam("toolbar") && w(p(b, ".toolbar"), "click", f.toolbar.handler);
            this.getParam("quick-code") && w(p(b, ".code"), "dblclick", e);
            return b;
        },
        init: function (a) {
            this.id = "" + Math.round(Math.random() * 1E6).toString();
            f.vars.highlighters[t(this.id)] = this;
            this.params = C(f.defaults, a || {});
            if (this.getParam("light") == true) this.params.toolbar = this.params.gutter = false;
        },
        getKeywords: function (a) {
            a = a.replace(/^\s+|\s+$/g, "").replace(/\s+/g, "|");
            return "\\b(?:" + a + ")\\b";
        },
        forHtmlScript: function (a) {
            this.htmlScript = {
                left: {
                    regex: a.left,
                    css: "script"
                },
                right: {
                    regex: a.right,
                    css: "script"
                },
                code: new XRegExp("(?<left>" + a.left.source + ")(?<code>.*?)(?<right>" + a.right.source + ")", "sgi")
            };
        }
    };
    return f;
}();
typeof exports != "undefined" && (exports.SyntaxHighlighter = SyntaxHighlighter);

 
;
(function () {
    typeof (require) != 'undefined' ? SyntaxHighlighter = require('shCore').SyntaxHighlighter : null;
    function Brush() {
        var a = 'class interface function package';
        var b = '-Infinity ...rest Array as AS3 Boolean break case catch const continue Date decodeURI ' + 'decodeURIComponent default delete do dynamic each else encodeURI encodeURIComponent escape ' + 'extends false final finally flash_proxy for get if implements import in include Infinity ' + 'instanceof int internal is isFinite isNaN isXMLName label namespace NaN native new null ' + 'Null Number Object object_proxy override parseFloat parseInt private protected public ' + 'return set static String super switch this throw true try typeof uint undefined unescape ' + 'use void while with';
        this.regexList = [{
            regex: SyntaxHighlighter.regexLib.singleLineCComments,
            css: 'comments'
        }, {
            regex: SyntaxHighlighter.regexLib.multiLineCComments,
            css: 'comments'
        }, {
            regex: SyntaxHighlighter.regexLib.doubleQuotedString,
            css: 'string'
        }, {
            regex: SyntaxHighlighter.regexLib.singleQuotedString,
            css: 'string'
        }, {
            regex: /\b([\d]+(\.[\d]+)?|0x[a-f0-9]+)\b/gi,
            css: 'value'
        }, {
            regex: new RegExp(this.getKeywords(a), 'gm'),
            css: 'color3'
        }, {
            regex: new RegExp(this.getKeywords(b), 'gm'),
            css: 'keyword'
        }, {
            regex: new RegExp('var', 'gm'),
            css: 'variable'
        }, {
            regex: new RegExp('trace', 'gm'),
            css: 'color1'
        }
        ];
        this.forHtmlScript(SyntaxHighlighter.regexLib.scriptScriptTags);
    };
    Brush.prototype = new SyntaxHighlighter.Highlighter();
    Brush.aliases = ['actionscript3', 'as3'];
    SyntaxHighlighter.brushes.AS3 = Brush;
    typeof (exports) != 'undefined' ? exports.Brush = Brush : null;
})();
(function () {
    typeof (require) != 'undefined' ? SyntaxHighlighter = require('shCore').SyntaxHighlighter : null;
    function Brush() {
        var a = 'if fi then elif else for do done until while break continue case function return in eq ne ge le';
        var b = 'alias apropos awk basename bash bc bg builtin bzip2 cal cat cd cfdisk chgrp chmod chown chroot' + 'cksum clear cmp comm command cp cron crontab csplit cut date dc dd ddrescue declare df ' + 'diff diff3 dig dir dircolors dirname dirs du echo egrep eject enable env ethtool eval ' + 'exec exit expand export expr false fdformat fdisk fg fgrep file find fmt fold format ' + 'free fsck ftp gawk getopts grep groups gzip hash head history hostname id ifconfig ' + 'import install join kill less let ln local locate logname logout look lpc lpr lprint ' + 'lprintd lprintq lprm ls lsof make man mkdir mkfifo mkisofs mknod more mount mtools ' + 'mv netstat nice nl nohup nslookup open op passwd paste pathchk ping popd pr printcap ' + 'printenv printf ps pushd pwd quota quotacheck quotactl ram rcp read readonly renice ' + 'remsync rm rmdir rsync screen scp sdiff sed select seq set sftp shift shopt shutdown ' + 'sleep sort source split ssh strace su sudo sum symlink sync tail tar tee test time ' + 'times touch top traceroute trap tr true tsort tty type ulimit umask umount unalias ' + 'uname unexpand uniq units unset unshar useradd usermod users uuencode uudecode v vdir ' + 'vi watch wc whereis which who whoami Wget xargs yes';
        this.regexList = [{
            regex: /^#!.*$/gm,
            css: 'preprocessor bold'
        }, {
            regex: /\/[\w-\/]+/gm,
            css: 'plain'
        }, {
            regex: SyntaxHighlighter.regexLib.singleLinePerlComments,
            css: 'comments'
        }, {
            regex: SyntaxHighlighter.regexLib.doubleQuotedString,
            css: 'string'
        }, {
            regex: SyntaxHighlighter.regexLib.singleQuotedString,
            css: 'string'
        }, {
            regex: new RegExp(this.getKeywords(a), 'gm'),
            css: 'keyword'
        }, {
            regex: new RegExp(this.getKeywords(b), 'gm'),
            css: 'functions'
        }
        ];
    }
    Brush.prototype = new SyntaxHighlighter.Highlighter();
    Brush.aliases = ['bash', 'shell'];
    SyntaxHighlighter.brushes.Bash = Brush;
    typeof (exports) != 'undefined' ? exports.Brush = Brush : null;
})();
(function () {
    typeof (require) != 'undefined' ? SyntaxHighlighter = require('shCore').SyntaxHighlighter : null;
    function Brush() {
        var a = 'Abs ACos AddSOAPRequestHeader AddSOAPResponseHeader AjaxLink AjaxOnLoad ArrayAppend ArrayAvg ArrayClear ArrayDeleteAt ' + 'ArrayInsertAt ArrayIsDefined ArrayIsEmpty ArrayLen ArrayMax ArrayMin ArraySet ArraySort ArraySum ArraySwap ArrayToList ' + 'Asc ASin Atn BinaryDecode BinaryEncode BitAnd BitMaskClear BitMaskRead BitMaskSet BitNot BitOr BitSHLN BitSHRN BitXor ' + 'Ceiling CharsetDecode CharsetEncode Chr CJustify Compare CompareNoCase Cos CreateDate CreateDateTime CreateObject ' + 'CreateODBCDate CreateODBCDateTime CreateODBCTime CreateTime CreateTimeSpan CreateUUID DateAdd DateCompare DateConvert ' + 'DateDiff DateFormat DatePart Day DayOfWeek DayOfWeekAsString DayOfYear DaysInMonth DaysInYear DE DecimalFormat DecrementValue ' + 'Decrypt DecryptBinary DeleteClientVariable DeserializeJSON DirectoryExists DollarFormat DotNetToCFType Duplicate Encrypt ' + 'EncryptBinary Evaluate Exp ExpandPath FileClose FileCopy FileDelete FileExists FileIsEOF FileMove FileOpen FileRead ' + 'FileReadBinary FileReadLine FileSetAccessMode FileSetAttribute FileSetLastModified FileWrite Find FindNoCase FindOneOf ' + 'FirstDayOfMonth Fix FormatBaseN GenerateSecretKey GetAuthUser GetBaseTagData GetBaseTagList GetBaseTemplatePath ' + 'GetClientVariablesList GetComponentMetaData GetContextRoot GetCurrentTemplatePath GetDirectoryFromPath GetEncoding ' + 'GetException GetFileFromPath GetFileInfo GetFunctionList GetGatewayHelper GetHttpRequestData GetHttpTimeString ' + 'GetK2ServerDocCount GetK2ServerDocCountLimit GetLocale GetLocaleDisplayName GetLocalHostIP GetMetaData GetMetricData ' + 'GetPageContext GetPrinterInfo GetProfileSections GetProfileString GetReadableImageFormats GetSOAPRequest GetSOAPRequestHeader ' + 'GetSOAPResponse GetSOAPResponseHeader GetTempDirectory GetTempFile GetTemplatePath GetTickCount GetTimeZoneInfo GetToken ' + 'GetUserRoles GetWriteableImageFormats Hash Hour HTMLCodeFormat HTMLEditFormat IIf ImageAddBorder ImageBlur ImageClearRect ' + 'ImageCopy ImageCrop ImageDrawArc ImageDrawBeveledRect ImageDrawCubicCurve ImageDrawLine ImageDrawLines ImageDrawOval ' + 'ImageDrawPoint ImageDrawQuadraticCurve ImageDrawRect ImageDrawRoundRect ImageDrawText ImageFlip ImageGetBlob ImageGetBufferedImage ' + 'ImageGetEXIFTag ImageGetHeight ImageGetIPTCTag ImageGetWidth ImageGrayscale ImageInfo ImageNegative ImageNew ImageOverlay ImagePaste ' + 'ImageRead ImageReadBase64 ImageResize ImageRotate ImageRotateDrawingAxis ImageScaleToFit ImageSetAntialiasing ImageSetBackgroundColor ' + 'ImageSetDrawingColor ImageSetDrawingStroke ImageSetDrawingTransparency ImageSharpen ImageShear ImageShearDrawingAxis ImageTranslate ' + 'ImageTranslateDrawingAxis ImageWrite ImageWriteBase64 ImageXORDrawingMode IncrementValue InputBaseN Insert Int IsArray IsBinary ' + 'IsBoolean IsCustomFunction IsDate IsDDX IsDebugMode IsDefined IsImage IsImageFile IsInstanceOf IsJSON IsLeapYear IsLocalHost ' + 'IsNumeric IsNumericDate IsObject IsPDFFile IsPDFObject IsQuery IsSimpleValue IsSOAPRequest IsStruct IsUserInAnyRole IsUserInRole ' + 'IsUserLoggedIn IsValid IsWDDX IsXML IsXmlAttribute IsXmlDoc IsXmlElem IsXmlNode IsXmlRoot JavaCast JSStringFormat LCase Left Len ' + 'ListAppend ListChangeDelims ListContains ListContainsNoCase ListDeleteAt ListFind ListFindNoCase ListFirst ListGetAt ListInsertAt ' + 'ListLast ListLen ListPrepend ListQualify ListRest ListSetAt ListSort ListToArray ListValueCount ListValueCountNoCase LJustify Log ' + 'Log10 LSCurrencyFormat LSDateFormat LSEuroCurrencyFormat LSIsCurrency LSIsDate LSIsNumeric LSNumberFormat LSParseCurrency LSParseDateTime ' + 'LSParseEuroCurrency LSParseNumber LSTimeFormat LTrim Max Mid Min Minute Month MonthAsString Now NumberFormat ParagraphFormat ParseDateTime ' + 'Pi PrecisionEvaluate PreserveSingleQuotes Quarter QueryAddColumn QueryAddRow QueryConvertForGrid QueryNew QuerySetCell QuotedValueList Rand ' + 'Randomize RandRange REFind REFindNoCase ReleaseComObject REMatch REMatchNoCase RemoveChars RepeatString Replace ReplaceList ReplaceNoCase ' + 'REReplace REReplaceNoCase Reverse Right RJustify Round RTrim Second SendGatewayMessage SerializeJSON SetEncoding SetLocale SetProfileString ' + 'SetVariable Sgn Sin Sleep SpanExcluding SpanIncluding Sqr StripCR StructAppend StructClear StructCopy StructCount StructDelete StructFind ' + 'StructFindKey StructFindValue StructGet StructInsert StructIsEmpty StructKeyArray StructKeyExists StructKeyList StructKeyList StructNew ' + 'StructSort StructUpdate Tan TimeFormat ToBase64 ToBinary ToScript ToString Trim UCase URLDecode URLEncodedFormat URLSessionFormat Val ' + 'ValueList VerifyClient Week Wrap Wrap WriteOutput XmlChildPos XmlElemNew XmlFormat XmlGetNodeType XmlNew XmlParse XmlSearch XmlTransform ' + 'XmlValidate Year YesNoFormat';
        var b = 'cfabort cfajaximport cfajaxproxy cfapplet cfapplication cfargument cfassociate cfbreak cfcache cfcalendar ' + 'cfcase cfcatch cfchart cfchartdata cfchartseries cfcol cfcollection cfcomponent cfcontent cfcookie cfdbinfo ' + 'cfdefaultcase cfdirectory cfdiv cfdocument cfdocumentitem cfdocumentsection cfdump cfelse cfelseif cferror ' + 'cfexchangecalendar cfexchangeconnection cfexchangecontact cfexchangefilter cfexchangemail cfexchangetask ' + 'cfexecute cfexit cffeed cffile cfflush cfform cfformgroup cfformitem cfftp cffunction cfgrid cfgridcolumn ' + 'cfgridrow cfgridupdate cfheader cfhtmlhead cfhttp cfhttpparam cfif cfimage cfimport cfinclude cfindex ' + 'cfinput cfinsert cfinterface cfinvoke cfinvokeargument cflayout cflayoutarea cfldap cflocation cflock cflog ' + 'cflogin cfloginuser cflogout cfloop cfmail cfmailparam cfmailpart cfmenu cfmenuitem cfmodule cfNTauthenticate ' + 'cfobject cfobjectcache cfoutput cfparam cfpdf cfpdfform cfpdfformparam cfpdfparam cfpdfsubform cfpod cfpop ' + 'cfpresentation cfpresentationslide cfpresenter cfprint cfprocessingdirective cfprocparam cfprocresult ' + 'cfproperty cfquery cfqueryparam cfregistry cfreport cfreportparam cfrethrow cfreturn cfsavecontent cfschedule ' + 'cfscript cfsearch cfselect cfset cfsetting cfsilent cfslider cfsprydataset cfstoredproc cfswitch cftable ' + 'cftextarea cfthread cfthrow cftimer cftooltip cftrace cftransaction cftree cftreeitem cftry cfupdate cfwddx ' + 'cfwindow cfxml cfzip cfzipparam';
        var c = 'all and any between cross in join like not null or outer some';
        this.regexList = [{
            regex: new RegExp('--(.*)$', 'gm'),
            css: 'comments'
        }, {
            regex: SyntaxHighlighter.regexLib.xmlComments,
            css: 'comments'
        }, {
            regex: SyntaxHighlighter.regexLib.doubleQuotedString,
            css: 'string'
        }, {
            regex: SyntaxHighlighter.regexLib.singleQuotedString,
            css: 'string'
        }, {
            regex: new RegExp(this.getKeywords(a), 'gmi'),
            css: 'functions'
        }, {
            regex: new RegExp(this.getKeywords(c), 'gmi'),
            css: 'color1'
        }, {
            regex: new RegExp(this.getKeywords(b), 'gmi'),
            css: 'keyword'
        }
        ];
    }
    Brush.prototype = new SyntaxHighlighter.Highlighter();
    Brush.aliases = ['coldfusion', 'cf'];
    SyntaxHighlighter.brushes.ColdFusion = Brush;
    typeof (exports) != 'undefined' ? exports.Brush = Brush : null;
})();
(function () {
    typeof (require) != 'undefined' ? SyntaxHighlighter = require('shCore').SyntaxHighlighter : null;
    function Brush() {
        var a = 'Abs ACos AddSOAPRequestHeader AddSOAPResponseHeader AjaxLink AjaxOnLoad ArrayAppend ArrayAvg ArrayClear ArrayDeleteAt ' + 'ArrayInsertAt ArrayIsDefined ArrayIsEmpty ArrayLen ArrayMax ArrayMin ArraySet ArraySort ArraySum ArraySwap ArrayToList ' + 'Asc ASin Atn BinaryDecode BinaryEncode BitAnd BitMaskClear BitMaskRead BitMaskSet BitNot BitOr BitSHLN BitSHRN BitXor ' + 'Ceiling CharsetDecode CharsetEncode Chr CJustify Compare CompareNoCase Cos CreateDate CreateDateTime CreateObject ' + 'CreateODBCDate CreateODBCDateTime CreateODBCTime CreateTime CreateTimeSpan CreateUUID DateAdd DateCompare DateConvert ' + 'DateDiff DateFormat DatePart Day DayOfWeek DayOfWeekAsString DayOfYear DaysInMonth DaysInYear DE DecimalFormat DecrementValue ' + 'Decrypt DecryptBinary DeleteClientVariable DeserializeJSON DirectoryExists DollarFormat DotNetToCFType Duplicate Encrypt ' + 'EncryptBinary Evaluate Exp ExpandPath FileClose FileCopy FileDelete FileExists FileIsEOF FileMove FileOpen FileRead ' + 'FileReadBinary FileReadLine FileSetAccessMode FileSetAttribute FileSetLastModified FileWrite Find FindNoCase FindOneOf ' + 'FirstDayOfMonth Fix FormatBaseN GenerateSecretKey GetAuthUser GetBaseTagData GetBaseTagList GetBaseTemplatePath ' + 'GetClientVariablesList GetComponentMetaData GetContextRoot GetCurrentTemplatePath GetDirectoryFromPath GetEncoding ' + 'GetException GetFileFromPath GetFileInfo GetFunctionList GetGatewayHelper GetHttpRequestData GetHttpTimeString ' + 'GetK2ServerDocCount GetK2ServerDocCountLimit GetLocale GetLocaleDisplayName GetLocalHostIP GetMetaData GetMetricData ' + 'GetPageContext GetPrinterInfo GetProfileSections GetProfileString GetReadableImageFormats GetSOAPRequest GetSOAPRequestHeader ' + 'GetSOAPResponse GetSOAPResponseHeader GetTempDirectory GetTempFile GetTemplatePath GetTickCount GetTimeZoneInfo GetToken ' + 'GetUserRoles GetWriteableImageFormats Hash Hour HTMLCodeFormat HTMLEditFormat IIf ImageAddBorder ImageBlur ImageClearRect ' + 'ImageCopy ImageCrop ImageDrawArc ImageDrawBeveledRect ImageDrawCubicCurve ImageDrawLine ImageDrawLines ImageDrawOval ' + 'ImageDrawPoint ImageDrawQuadraticCurve ImageDrawRect ImageDrawRoundRect ImageDrawText ImageFlip ImageGetBlob ImageGetBufferedImage ' + 'ImageGetEXIFTag ImageGetHeight ImageGetIPTCTag ImageGetWidth ImageGrayscale ImageInfo ImageNegative ImageNew ImageOverlay ImagePaste ' + 'ImageRead ImageReadBase64 ImageResize ImageRotate ImageRotateDrawingAxis ImageScaleToFit ImageSetAntialiasing ImageSetBackgroundColor ' + 'ImageSetDrawingColor ImageSetDrawingStroke ImageSetDrawingTransparency ImageSharpen ImageShear ImageShearDrawingAxis ImageTranslate ' + 'ImageTranslateDrawingAxis ImageWrite ImageWriteBase64 ImageXORDrawingMode IncrementValue InputBaseN Insert Int IsArray IsBinary ' + 'IsBoolean IsCustomFunction IsDate IsDDX IsDebugMode IsDefined IsImage IsImageFile IsInstanceOf IsJSON IsLeapYear IsLocalHost ' + 'IsNumeric IsNumericDate IsObject IsPDFFile IsPDFObject IsQuery IsSimpleValue IsSOAPRequest IsStruct IsUserInAnyRole IsUserInRole ' + 'IsUserLoggedIn IsValid IsWDDX IsXML IsXmlAttribute IsXmlDoc IsXmlElem IsXmlNode IsXmlRoot JavaCast JSStringFormat LCase Left Len ' + 'ListAppend ListChangeDelims ListContains ListContainsNoCase ListDeleteAt ListFind ListFindNoCase ListFirst ListGetAt ListInsertAt ' + 'ListLast ListLen ListPrepend ListQualify ListRest ListSetAt ListSort ListToArray ListValueCount ListValueCountNoCase LJustify Log ' + 'Log10 LSCurrencyFormat LSDateFormat LSEuroCurrencyFormat LSIsCurrency LSIsDate LSIsNumeric LSNumberFormat LSParseCurrency LSParseDateTime ' + 'LSParseEuroCurrency LSParseNumber LSTimeFormat LTrim Max Mid Min Minute Month MonthAsString Now NumberFormat ParagraphFormat ParseDateTime ' + 'Pi PrecisionEvaluate PreserveSingleQuotes Quarter QueryAddColumn QueryAddRow QueryConvertForGrid QueryNew QuerySetCell QuotedValueList Rand ' + 'Randomize RandRange REFind REFindNoCase ReleaseComObject REMatch REMatchNoCase RemoveChars RepeatString Replace ReplaceList ReplaceNoCase ' + 'REReplace REReplaceNoCase Reverse Right RJustify Round RTrim Second SendGatewayMessage SerializeJSON SetEncoding SetLocale SetProfileString ' + 'SetVariable Sgn Sin Sleep SpanExcluding SpanIncluding Sqr StripCR StructAppend StructClear StructCopy StructCount StructDelete StructFind ' + 'StructFindKey StructFindValue StructGet StructInsert StructIsEmpty StructKeyArray StructKeyExists StructKeyList StructKeyList StructNew ' + 'StructSort StructUpdate Tan TimeFormat ToBase64 ToBinary ToScript ToString Trim UCase URLDecode URLEncodedFormat URLSessionFormat Val ' + 'ValueList VerifyClient Week Wrap Wrap WriteOutput XmlChildPos XmlElemNew XmlFormat XmlGetNodeType XmlNew XmlParse XmlSearch XmlTransform ' + 'XmlValidate Year YesNoFormat';
        var b = 'cfabort cfajaximport cfajaxproxy cfapplet cfapplication cfargument cfassociate cfbreak cfcache cfcalendar ' + 'cfcase cfcatch cfchart cfchartdata cfchartseries cfcol cfcollection cfcomponent cfcontent cfcookie cfdbinfo ' + 'cfdefaultcase cfdirectory cfdiv cfdocument cfdocumentitem cfdocumentsection cfdump cfelse cfelseif cferror ' + 'cfexchangecalendar cfexchangeconnection cfexchangecontact cfexchangefilter cfexchangemail cfexchangetask ' + 'cfexecute cfexit cffeed cffile cfflush cfform cfformgroup cfformitem cfftp cffunction cfgrid cfgridcolumn ' + 'cfgridrow cfgridupdate cfheader cfhtmlhead cfhttp cfhttpparam cfif cfimage cfimport cfinclude cfindex ' + 'cfinput cfinsert cfinterface cfinvoke cfinvokeargument cflayout cflayoutarea cfldap cflocation cflock cflog ' + 'cflogin cfloginuser cflogout cfloop cfmail cfmailparam cfmailpart cfmenu cfmenuitem cfmodule cfNTauthenticate ' + 'cfobject cfobjectcache cfoutput cfparam cfpdf cfpdfform cfpdfformparam cfpdfparam cfpdfsubform cfpod cfpop ' + 'cfpresentation cfpresentationslide cfpresenter cfprint cfprocessingdirective cfprocparam cfprocresult ' + 'cfproperty cfquery cfqueryparam cfregistry cfreport cfreportparam cfrethrow cfreturn cfsavecontent cfschedule ' + 'cfscript cfsearch cfselect cfset cfsetting cfsilent cfslider cfsprydataset cfstoredproc cfswitch cftable ' + 'cftextarea cfthread cfthrow cftimer cftooltip cftrace cftransaction cftree cftreeitem cftry cfupdate cfwddx ' + 'cfwindow cfxml cfzip cfzipparam';
        var c = 'all and any between cross in join like not null or outer some';
        this.regexList = [{
            regex: new RegExp('--(.*)$', 'gm'),
            css: 'comments'
        }, {
            regex: SyntaxHighlighter.regexLib.xmlComments,
            css: 'comments'
        }, {
            regex: SyntaxHighlighter.regexLib.doubleQuotedString,
            css: 'string'
        }, {
            regex: SyntaxHighlighter.regexLib.singleQuotedString,
            css: 'string'
        }, {
            regex: new RegExp(this.getKeywords(a), 'gmi'),
            css: 'functions'
        }, {
            regex: new RegExp(this.getKeywords(c), 'gmi'),
            css: 'color1'
        }, {
            regex: new RegExp(this.getKeywords(b), 'gmi'),
            css: 'keyword'
        }
        ];
    }
    Brush.prototype = new SyntaxHighlighter.Highlighter();
    Brush.aliases = ['coldfusion', 'cf'];
    SyntaxHighlighter.brushes.ColdFusion = Brush;
    typeof (exports) != 'undefined' ? exports.Brush = Brush : null;
})();
(function () {
    typeof (require) != 'undefined' ? SyntaxHighlighter = require('shCore').SyntaxHighlighter : null;
    function Brush() {
        var d = 'abstract as base bool break byte case catch char checked class const ' + 'continue decimal default delegate do double else enum event explicit ' + 'extern false finally fixed float for foreach get goto if implicit in int ' + 'interface internal is lock long namespace new null object operator out ' + 'override params private protected public readonly ref return sbyte sealed set ' + 'short sizeof stackalloc static string struct switch this throw true try ' + 'typeof uint ulong unchecked unsafe ushort using virtual void while';
        function fixComments(a, b) {
            var c = (a[0].indexOf("///") == 0) ? 'color1' : 'comments';
            return [new SyntaxHighlighter.Match(a[0], a.index, c)];
        }
        this.regexList = [{
            regex: SyntaxHighlighter.regexLib.singleLineCComments,
            func: fixComments
        }, {
            regex: SyntaxHighlighter.regexLib.multiLineCComments,
            css: 'comments'
        }, {
            regex: /@"(?:[^"]|"")*"/g,
            css: 'string'
        }, {
            regex: SyntaxHighlighter.regexLib.doubleQuotedString,
            css: 'string'
        }, {
            regex: SyntaxHighlighter.regexLib.singleQuotedString,
            css: 'string'
        }, {
            regex: /^\s*#.*/gm,
            css: 'preprocessor'
        }, {
            regex: new RegExp(this.getKeywords(d), 'gm'),
            css: 'keyword'
        }, {
            regex: /\bpartial(?=\s+(?:class|interface|struct)\b)/g,
            css: 'keyword'
        }, {
            regex: /\byield(?=\s+(?:return|break)\b)/g,
            css: 'keyword'
        }
        ];
        this.forHtmlScript(SyntaxHighlighter.regexLib.aspScriptTags);
    };
    Brush.prototype = new SyntaxHighlighter.Highlighter();
    Brush.aliases = ['c#', 'c-sharp', 'csharp'];
    SyntaxHighlighter.brushes.CSharp = Brush;
    typeof (exports) != 'undefined' ? exports.Brush = Brush : null;
})();
(function () {
    typeof (require) != 'undefined' ? SyntaxHighlighter = require('shCore').SyntaxHighlighter : null;
    function Brush() {
        function getKeywordsCSS(a) {
            return '\\b([a-z_]|)' + a.replace(/ /g, '(?=:)\\b|\\b([a-z_\\*]|\\*|)') + '(?=:)\\b';
        };
        function getValuesCSS(a) {
            return '\\b' + a.replace(/ /g, '(?!-)(?!:)\\b|\\b()') + '\:\\b';
        };
        var b = 'ascent azimuth background-attachment background-color background-image background-position ' + 'background-repeat background baseline bbox border-collapse border-color border-spacing border-style border-top ' + 'border-right border-bottom border-left border-top-color border-right-color border-bottom-color border-left-color ' + 'border-top-style border-right-style border-bottom-style border-left-style border-top-width border-right-width ' + 'border-bottom-width border-left-width border-width border bottom cap-height caption-side centerline clear clip color ' + 'content counter-increment counter-reset cue-after cue-before cue cursor definition-src descent direction display ' + 'elevation empty-cells float font-size-adjust font-family font-size font-stretch font-style font-variant font-weight font ' + 'height left letter-spacing line-height list-style-image list-style-position list-style-type list-style margin-top ' + 'margin-right margin-bottom margin-left margin marker-offset marks mathline max-height max-width min-height min-width orphans ' + 'outline-color outline-style outline-width outline overflow padding-top padding-right padding-bottom padding-left padding page ' + 'page-break-after page-break-before page-break-inside pause pause-after pause-before pitch pitch-range play-during position ' + 'quotes right richness size slope src speak-header speak-numeral speak-punctuation speak speech-rate stemh stemv stress ' + 'table-layout text-align top text-decoration text-indent text-shadow text-transform unicode-bidi unicode-range units-per-em ' + 'vertical-align visibility voice-family volume white-space widows width widths word-spacing x-height z-index';
        var c = 'above absolute all always aqua armenian attr aural auto avoid baseline behind below bidi-override black blink block blue bold bolder ' + 'both bottom braille capitalize caption center center-left center-right circle close-quote code collapse compact condensed ' + 'continuous counter counters crop cross crosshair cursive dashed decimal decimal-leading-zero default digits disc dotted double ' + 'embed embossed e-resize expanded extra-condensed extra-expanded fantasy far-left far-right fast faster fixed format fuchsia ' + 'gray green groove handheld hebrew help hidden hide high higher icon inline-table inline inset inside invert italic ' + 'justify landscape large larger left-side left leftwards level lighter lime line-through list-item local loud lower-alpha ' + 'lowercase lower-greek lower-latin lower-roman lower low ltr marker maroon medium message-box middle mix move narrower ' + 'navy ne-resize no-close-quote none no-open-quote no-repeat normal nowrap n-resize nw-resize oblique olive once open-quote outset ' + 'outside overline pointer portrait pre print projection purple red relative repeat repeat-x repeat-y rgb ridge right right-side ' + 'rightwards rtl run-in screen scroll semi-condensed semi-expanded separate se-resize show silent silver slower slow ' + 'small small-caps small-caption smaller soft solid speech spell-out square s-resize static status-bar sub super sw-resize ' + 'table-caption table-cell table-column table-column-group table-footer-group table-header-group table-row table-row-group teal ' + 'text-bottom text-top thick thin top transparent tty tv ultra-condensed ultra-expanded underline upper-alpha uppercase upper-latin ' + 'upper-roman url visible wait white wider w-resize x-fast x-high x-large x-loud x-low x-slow x-small x-soft xx-large xx-small yellow';
        var d = '[mM]onospace [tT]ahoma [vV]erdana [aA]rial [hH]elvetica [sS]ans-serif [sS]erif [cC]ourier mono sans serif';
        this.regexList = [{
            regex: SyntaxHighlighter.regexLib.multiLineCComments,
            css: 'comments'
        }, {
            regex: SyntaxHighlighter.regexLib.doubleQuotedString,
            css: 'string'
        }, {
            regex: SyntaxHighlighter.regexLib.singleQuotedString,
            css: 'string'
        }, {
            regex: /\#[a-fA-F0-9]{3,6}/g,
            css: 'value'
        }, {
            regex: /(-?\d+)(\.\d+)?(px|em|pt|\:|\%|)/g,
            css: 'value'
        }, {
            regex: /!important/g,
            css: 'color3'
        }, {
            regex: new RegExp(getKeywordsCSS(b), 'gm'),
            css: 'keyword'
        }, {
            regex: new RegExp(getValuesCSS(c), 'g'),
            css: 'value'
        }, {
            regex: new RegExp(this.getKeywords(d), 'g'),
            css: 'color1'
        }
        ];
        this.forHtmlScript({
            left: /(&lt;|<)\s*style.*?(&gt;|>)/gi,
            right: /(&lt;|<)\/\s*style\s*(&gt;|>)/gi
        });
    };
    Brush.prototype = new SyntaxHighlighter.Highlighter();
    Brush.aliases = ['css'];
    SyntaxHighlighter.brushes.CSS = Brush;
    typeof (exports) != 'undefined' ? exports.Brush = Brush : null;
})();
(function () {
    typeof (require) != 'undefined' ? SyntaxHighlighter = require('shCore').SyntaxHighlighter : null;
    function Brush() {
        var a = 'abs addr and ansichar ansistring array as asm begin boolean byte cardinal ' + 'case char class comp const constructor currency destructor div do double ' + 'downto else end except exports extended false file finalization finally ' + 'for function goto if implementation in inherited int64 initialization ' + 'integer interface is label library longint longword mod nil not object ' + 'of on or packed pansichar pansistring pchar pcurrency pdatetime pextended ' + 'pint64 pointer private procedure program property pshortstring pstring ' + 'pvariant pwidechar pwidestring protected public published raise real real48 ' + 'record repeat set shl shortint shortstring shr single smallint string then ' + 'threadvar to true try type unit until uses val var varirnt while widechar ' + 'widestring with word write writeln xor';
        this.regexList = [{
            regex: /\(\*[\s\S]*?\*\)/gm,
            css: 'comments'
        }, {
            regex: /{(?!\$)[\s\S]*?}/gm,
            css: 'comments'
        }, {
            regex: SyntaxHighlighter.regexLib.singleLineCComments,
            css: 'comments'
        }, {
            regex: SyntaxHighlighter.regexLib.singleQuotedString,
            css: 'string'
        }, {
            regex: /\{\$[a-zA-Z]+ .+\}/g,
            css: 'color1'
        }, {
            regex: /\b[\d\.]+\b/g,
            css: 'value'
        }, {
            regex: /\$[a-zA-Z0-9]+\b/g,
            css: 'value'
        }, {
            regex: new RegExp(this.getKeywords(a), 'gmi'),
            css: 'keyword'
        }
        ];
    };
    Brush.prototype = new SyntaxHighlighter.Highlighter();
    Brush.aliases = ['delphi', 'pascal', 'pas'];
    SyntaxHighlighter.brushes.Delphi = Brush;
    typeof (exports) != 'undefined' ? exports.Brush = Brush : null;
})();
(function () {
    typeof (require) != 'undefined' ? SyntaxHighlighter = require('shCore').SyntaxHighlighter : null;
    function Brush() {
        this.regexList = [{
            regex: /^\+\+\+.*$/gm,
            css: 'color2'
        }, {
            regex: /^\-\-\-.*$/gm,
            css: 'color2'
        }, {
            regex: /^\s.*$/gm,
            css: 'color1'
        }, {
            regex: /^@@.*@@$/gm,
            css: 'variable'
        }, {
            regex: /^\+[^\+]{1}.*$/gm,
            css: 'string'
        }, {
            regex: /^\-[^\-]{1}.*$/gm,
            css: 'comments'
        }
        ];
    };
    Brush.prototype = new SyntaxHighlighter.Highlighter();
    Brush.aliases = ['diff', 'patch'];
    SyntaxHighlighter.brushes.Diff = Brush;
    typeof (exports) != 'undefined' ? exports.Brush = Brush : null;
})();
(function () {
    typeof (require) != 'undefined' ? SyntaxHighlighter = require('shCore').SyntaxHighlighter : null;
    function Brush() {
        var a = 'after and andalso band begin bnot bor bsl bsr bxor ' + 'case catch cond div end fun if let not of or orelse ' + 'query receive rem try when xor' + ' module export import define';
        this.regexList = [{
            regex: new RegExp("[A-Z][A-Za-z0-9_]+", 'g'),
            css: 'constants'
        }, {
            regex: new RegExp("\\%.+", 'gm'),
            css: 'comments'
        }, {
            regex: new RegExp("\\?[A-Za-z0-9_]+", 'g'),
            css: 'preprocessor'
        }, {
            regex: new RegExp("[a-z0-9_]+:[a-z0-9_]+", 'g'),
            css: 'functions'
        }, {
            regex: SyntaxHighlighter.regexLib.doubleQuotedString,
            css: 'string'
        }, {
            regex: SyntaxHighlighter.regexLib.singleQuotedString,
            css: 'string'
        }, {
            regex: new RegExp(this.getKeywords(a), 'gm'),
            css: 'keyword'
        }
        ];
    };
    Brush.prototype = new SyntaxHighlighter.Highlighter();
    Brush.aliases = ['erl', 'erlang'];
    SyntaxHighlighter.brushes.Erland = Brush;
    typeof (exports) != 'undefined' ? exports.Brush = Brush : null;
})();
(function () {
    typeof (require) != 'undefined' ? SyntaxHighlighter = require('shCore').SyntaxHighlighter : null;
    function Brush() {
        var a = 'as assert break case catch class continue def default do else extends finally ' + 'if in implements import instanceof interface new package property return switch ' + 'throw throws try while public protected private static';
        var b = 'void boolean byte char short int long float double';
        var c = 'null';
        var d = 'allProperties count get size ' + 'collect each eachProperty eachPropertyName eachWithIndex find findAll ' + 'findIndexOf grep inject max min reverseEach sort ' + 'asImmutable asSynchronized flatten intersect join pop reverse subMap toList ' + 'padRight padLeft contains eachMatch toCharacter toLong toUrl tokenize ' + 'eachFile eachFileRecurse eachB yte eachLine readBytes readLine getText ' + 'splitEachLine withReader append encodeBase64 decodeBase64 filterLine ' + 'transformChar transformLine withOutputStream withPrintWriter withStream ' + 'withStreams withWriter withWriterAppend write writeLine ' + 'dump inspect invokeMethod print println step times upto use waitForOrKill ' + 'getText';
        this.regexList = [{
            regex: SyntaxHighlighter.regexLib.singleLineCComments,
            css: 'comments'
        }, {
            regex: SyntaxHighlighter.regexLib.multiLineCComments,
            css: 'comments'
        }, {
            regex: SyntaxHighlighter.regexLib.doubleQuotedString,
            css: 'string'
        }, {
            regex: SyntaxHighlighter.regexLib.singleQuotedString,
            css: 'string'
        }, {
            regex: /""".*"""/g,
            css: 'string'
        }, {
            regex: new RegExp('\\b([\\d]+(\\.[\\d]+)?|0x[a-f0-9]+)\\b', 'gi'),
            css: 'value'
        }, {
            regex: new RegExp(this.getKeywords(a), 'gm'),
            css: 'keyword'
        }, {
            regex: new RegExp(this.getKeywords(b), 'gm'),
            css: 'color1'
        }, {
            regex: new RegExp(this.getKeywords(c), 'gm'),
            css: 'constants'
        }, {
            regex: new RegExp(this.getKeywords(d), 'gm'),
            css: 'functions'
        }
        ];
        this.forHtmlScript(SyntaxHighlighter.regexLib.aspScriptTags);
    }
    Brush.prototype = new SyntaxHighlighter.Highlighter();
    Brush.aliases = ['groovy'];
    SyntaxHighlighter.brushes.Groovy = Brush;
    typeof (exports) != 'undefined' ? exports.Brush = Brush : null;
})();
(function () {
    typeof (require) != 'undefined' ? SyntaxHighlighter = require('shCore').SyntaxHighlighter : null;
    function Brush() {
        var a = 'abstract assert boolean break byte case catch char class const ' + 'continue default do double else enum extends ' + 'false final finally float for goto if implements import ' + 'instanceof int interface long native new null ' + 'package private protected public return ' + 'short static strictfp super switch synchronized this throw throws true ' + 'transient try void volatile while';
        this.regexList = [{
            regex: SyntaxHighlighter.regexLib.singleLineCComments,
            css: 'comments'
        }, {
            regex: /\/\*([^\*][\s\S]*)?\*\//gm,
            css: 'comments'
        }, {
            regex: /\/\*(?!\*\/)\*[\s\S]*?\*\//gm,
            css: 'preprocessor'
        }, {
            regex: SyntaxHighlighter.regexLib.doubleQuotedString,
            css: 'string'
        }, {
            regex: SyntaxHighlighter.regexLib.singleQuotedString,
            css: 'string'
        }, {
            regex: /\b([\d]+(\.[\d]+)?|0x[a-f0-9]+)\b/gi,
            css: 'value'
        }, {
            regex: /(?!\@interface\b)\@[\$\w]+\b/g,
            css: 'color1'
        }, {
            regex: /\@interface\b/g,
            css: 'color2'
        }, {
            regex: new RegExp(this.getKeywords(a), 'gm'),
            css: 'keyword'
        }
        ];
        this.forHtmlScript({
            left: /(&lt;|<)%[@!=]?/g,
            right: /%(&gt;|>)/g
        });
    };
    Brush.prototype = new SyntaxHighlighter.Highlighter();
    Brush.aliases = ['java'];
    SyntaxHighlighter.brushes.Java = Brush;
    typeof (exports) != 'undefined' ? exports.Brush = Brush : null;
})();
(function () {
    typeof (require) != 'undefined' ? SyntaxHighlighter = require('shCore').SyntaxHighlighter : null;
    function Brush() {
        var a = 'Boolean Byte Character Double Duration ' + 'Float Integer Long Number Short String Void';
        var b = 'abstract after and as assert at before bind bound break catch class ' + 'continue def delete else exclusive extends false finally first for from ' + 'function if import in indexof init insert instanceof into inverse last ' + 'lazy mixin mod nativearray new not null on or override package postinit ' + 'protected public public-init public-read replace return reverse sizeof ' + 'step super then this throw true try tween typeof var where while with ' + 'attribute let private readonly static trigger';
        this.regexList = [{
            regex: SyntaxHighlighter.regexLib.singleLineCComments,
            css: 'comments'
        }, {
            regex: SyntaxHighlighter.regexLib.multiLineCComments,
            css: 'comments'
        }, {
            regex: SyntaxHighlighter.regexLib.singleQuotedString,
            css: 'string'
        }, {
            regex: SyntaxHighlighter.regexLib.doubleQuotedString,
            css: 'string'
        }, {
            regex: /(-?\.?)(\b(\d*\.?\d+|\d+\.?\d*)(e[+-]?\d+)?|0x[a-f\d]+)\b\.?/gi,
            css: 'color2'
        }, {
            regex: new RegExp(this.getKeywords(a), 'gm'),
            css: 'variable'
        }, {
            regex: new RegExp(this.getKeywords(b), 'gm'),
            css: 'keyword'
        }
        ];
        this.forHtmlScript(SyntaxHighlighter.regexLib.aspScriptTags);
    };
    Brush.prototype = new SyntaxHighlighter.Highlighter();
    Brush.aliases = ['jfx', 'javafx'];
    SyntaxHighlighter.brushes.JavaFX = Brush;
    typeof (exports) != 'undefined' ? exports.Brush = Brush : null;
})();
(function () {
    typeof (require) != 'undefined' ? SyntaxHighlighter = require('shCore').SyntaxHighlighter : null;
    function Brush() {
        var a = 'break case catch continue ' + 'default delete do else false  ' + 'for function if in instanceof ' + 'new null return super switch ' + 'this throw true try typeof var while with';
        var r = SyntaxHighlighter.regexLib;
        this.regexList = [{
            regex: r.multiLineDoubleQuotedString,
            css: 'string'
        }, {
            regex: r.multiLineSingleQuotedString,
            css: 'string'
        }, {
            regex: r.singleLineCComments,
            css: 'comments'
        }, {
            regex: r.multiLineCComments,
            css: 'comments'
        }, {
            regex: /\s*#.*/gm,
            css: 'preprocessor'
        }, {
            regex: new RegExp(this.getKeywords(a), 'gm'),
            css: 'keyword'
        }
        ];
        this.forHtmlScript(r.scriptScriptTags);
    };
    Brush.prototype = new SyntaxHighlighter.Highlighter();
    Brush.aliases = ['js', 'jscript', 'javascript'];
    SyntaxHighlighter.brushes.JScript = Brush;
    typeof (exports) != 'undefined' ? exports.Brush = Brush : null;
})();
(function () {
    typeof (require) != 'undefined' ? SyntaxHighlighter = require('shCore').SyntaxHighlighter : null;
    function Brush() {
        var a = 'abs accept alarm atan2 bind binmode chdir chmod chomp chop chown chr ' + 'chroot close closedir connect cos crypt defined delete each endgrent ' + 'endhostent endnetent endprotoent endpwent endservent eof exec exists ' + 'exp fcntl fileno flock fork format formline getc getgrent getgrgid ' + 'getgrnam gethostbyaddr gethostbyname gethostent getlogin getnetbyaddr ' + 'getnetbyname getnetent getpeername getpgrp getppid getpriority ' + 'getprotobyname getprotobynumber getprotoent getpwent getpwnam getpwuid ' + 'getservbyname getservbyport getservent getsockname getsockopt glob ' + 'gmtime grep hex index int ioctl join keys kill lc lcfirst length link ' + 'listen localtime lock log lstat map mkdir msgctl msgget msgrcv msgsnd ' + 'oct open opendir ord pack pipe pop pos print printf prototype push ' + 'quotemeta rand read readdir readline readlink readpipe recv rename ' + 'reset reverse rewinddir rindex rmdir scalar seek seekdir select semctl ' + 'semget semop send setgrent sethostent setnetent setpgrp setpriority ' + 'setprotoent setpwent setservent setsockopt shift shmctl shmget shmread ' + 'shmwrite shutdown sin sleep socket socketpair sort splice split sprintf ' + 'sqrt srand stat study substr symlink syscall sysopen sysread sysseek ' + 'system syswrite tell telldir time times tr truncate uc ucfirst umask ' + 'undef unlink unpack unshift utime values vec wait waitpid warn write';
        var b = 'bless caller continue dbmclose dbmopen die do dump else elsif eval exit ' + 'for foreach goto if import last local my next no our package redo ref ' + 'require return sub tie tied unless untie until use wantarray while';
        this.regexList = [{
            regex: new RegExp('#[^!].*$', 'gm'),
            css: 'comments'
        }, {
            regex: new RegExp('^\\s*#!.*$', 'gm'),
            css: 'preprocessor'
        }, {
            regex: SyntaxHighlighter.regexLib.doubleQuotedString,
            css: 'string'
        }, {
            regex: SyntaxHighlighter.regexLib.singleQuotedString,
            css: 'string'
        }, {
            regex: new RegExp('(\\$|@|%)\\w+', 'g'),
            css: 'variable'
        }, {
            regex: new RegExp(this.getKeywords(a), 'gmi'),
            css: 'functions'
        }, {
            regex: new RegExp(this.getKeywords(b), 'gm'),
            css: 'keyword'
        }
        ];
        this.forHtmlScript(SyntaxHighlighter.regexLib.phpScriptTags);
    }
    Brush.prototype = new SyntaxHighlighter.Highlighter();
    Brush.aliases = ['perl', 'Perl', 'pl'];
    SyntaxHighlighter.brushes.Perl = Brush;
    typeof (exports) != 'undefined' ? exports.Brush = Brush : null;
})();
(function () {
    typeof (require) != 'undefined' ? SyntaxHighlighter = require('shCore').SyntaxHighlighter : null;
    function Brush() {
        var a = 'abs acos acosh addcslashes addslashes ' + 'array_change_key_case array_chunk array_combine array_count_values array_diff ' + 'array_diff_assoc array_diff_key array_diff_uassoc array_diff_ukey array_fill ' + 'array_filter array_flip array_intersect array_intersect_assoc array_intersect_key ' + 'array_intersect_uassoc array_intersect_ukey array_key_exists array_keys array_map ' + 'array_merge array_merge_recursive array_multisort array_pad array_pop array_product ' + 'array_push array_rand array_reduce array_reverse array_search array_shift ' + 'array_slice array_splice array_sum array_udiff array_udiff_assoc ' + 'array_udiff_uassoc array_uintersect array_uintersect_assoc ' + 'array_uintersect_uassoc array_unique array_unshift array_values array_walk ' + 'array_walk_recursive atan atan2 atanh base64_decode base64_encode base_convert ' + 'basename bcadd bccomp bcdiv bcmod bcmul bindec bindtextdomain bzclose bzcompress ' + 'bzdecompress bzerrno bzerror bzerrstr bzflush bzopen bzread bzwrite ceil chdir ' + 'checkdate checkdnsrr chgrp chmod chop chown chr chroot chunk_split class_exists ' + 'closedir closelog copy cos cosh count count_chars date decbin dechex decoct ' + 'deg2rad delete ebcdic2ascii echo empty end ereg ereg_replace eregi eregi_replace error_log ' + 'error_reporting escapeshellarg escapeshellcmd eval exec exit exp explode extension_loaded ' + 'feof fflush fgetc fgetcsv fgets fgetss file_exists file_get_contents file_put_contents ' + 'fileatime filectime filegroup fileinode filemtime fileowner fileperms filesize filetype ' + 'floatval flock floor flush fmod fnmatch fopen fpassthru fprintf fputcsv fputs fread fscanf ' + 'fseek fsockopen fstat ftell ftok getallheaders getcwd getdate getenv gethostbyaddr gethostbyname ' + 'gethostbynamel getimagesize getlastmod getmxrr getmygid getmyinode getmypid getmyuid getopt ' + 'getprotobyname getprotobynumber getrandmax getrusage getservbyname getservbyport gettext ' + 'gettimeofday gettype glob gmdate gmmktime ini_alter ini_get ini_get_all ini_restore ini_set ' + 'interface_exists intval ip2long is_a is_array is_bool is_callable is_dir is_double ' + 'is_executable is_file is_finite is_float is_infinite is_int is_integer is_link is_long ' + 'is_nan is_null is_numeric is_object is_readable is_real is_resource is_scalar is_soap_fault ' + 'is_string is_subclass_of is_uploaded_file is_writable is_writeable mkdir mktime nl2br ' + 'parse_ini_file parse_str parse_url passthru pathinfo print readlink realpath rewind rewinddir rmdir ' + 'round str_ireplace str_pad str_repeat str_replace str_rot13 str_shuffle str_split ' + 'str_word_count strcasecmp strchr strcmp strcoll strcspn strftime strip_tags stripcslashes ' + 'stripos stripslashes stristr strlen strnatcasecmp strnatcmp strncasecmp strncmp strpbrk ' + 'strpos strptime strrchr strrev strripos strrpos strspn strstr strtok strtolower strtotime ' + 'strtoupper strtr strval substr substr_compare';
        var b = 'abstract and array as break case catch cfunction class clone const continue declare default die do ' + 'else elseif enddeclare endfor endforeach endif endswitch endwhile extends final for foreach ' + 'function include include_once global goto if implements interface instanceof namespace new ' + 'old_function or private protected public return require require_once static switch ' + 'throw try use var while xor ';
        var c = '__FILE__ __LINE__ __METHOD__ __FUNCTION__ __CLASS__';
        this.regexList = [{
            regex: SyntaxHighlighter.regexLib.singleLineCComments,
            css: 'comments'
        }, {
            regex: SyntaxHighlighter.regexLib.multiLineCComments,
            css: 'comments'
        }, {
            regex: SyntaxHighlighter.regexLib.doubleQuotedString,
            css: 'string'
        }, {
            regex: SyntaxHighlighter.regexLib.singleQuotedString,
            css: 'string'
        }, {
            regex: /\$\w+/g,
            css: 'variable'
        }, {
            regex: new RegExp(this.getKeywords(a), 'gmi'),
            css: 'functions'
        }, {
            regex: new RegExp(this.getKeywords(c), 'gmi'),
            css: 'constants'
        }, {
            regex: new RegExp(this.getKeywords(b), 'gm'),
            css: 'keyword'
        }
        ];
        this.forHtmlScript(SyntaxHighlighter.regexLib.phpScriptTags);
    };
    Brush.prototype = new SyntaxHighlighter.Highlighter();
    Brush.aliases = ['php'];
    SyntaxHighlighter.brushes.Php = Brush;
    typeof (exports) != 'undefined' ? exports.Brush = Brush : null;
})();
(function () {
    typeof (require) != 'undefined' ? SyntaxHighlighter = require('shCore').SyntaxHighlighter : null;
    function Brush() { };
    Brush.prototype = new SyntaxHighlighter.Highlighter();
    Brush.aliases = ['text', 'plain'];
    SyntaxHighlighter.brushes.Plain = Brush;
    typeof (exports) != 'undefined' ? exports.Brush = Brush : null;
})();
(function () {
    typeof (require) != 'undefined' ? SyntaxHighlighter = require('shCore').SyntaxHighlighter : null;
    function Brush() {
        var a = 'Add-Content Add-History Add-Member Add-PSSnapin Clear(-Content)? Clear-Item ' + 'Clear-ItemProperty Clear-Variable Compare-Object ConvertFrom-SecureString Convert-Path ' + 'ConvertTo-Html ConvertTo-SecureString Copy(-Item)? Copy-ItemProperty Export-Alias ' + 'Export-Clixml Export-Console Export-Csv ForEach(-Object)? Format-Custom Format-List ' + 'Format-Table Format-Wide Get-Acl Get-Alias Get-AuthenticodeSignature Get-ChildItem Get-Command ' + 'Get-Content Get-Credential Get-Culture Get-Date Get-EventLog Get-ExecutionPolicy ' + 'Get-Help Get-History Get-Host Get-Item Get-ItemProperty Get-Location Get-Member ' + 'Get-PfxCertificate Get-Process Get-PSDrive Get-PSProvider Get-PSSnapin Get-Service ' + 'Get-TraceSource Get-UICulture Get-Unique Get-Variable Get-WmiObject Group-Object ' + 'Import-Alias Import-Clixml Import-Csv Invoke-Expression Invoke-History Invoke-Item ' + 'Join-Path Measure-Command Measure-Object Move(-Item)? Move-ItemProperty New-Alias ' + 'New-Item New-ItemProperty New-Object New-PSDrive New-Service New-TimeSpan ' + 'New-Variable Out-Default Out-File Out-Host Out-Null Out-Printer Out-String Pop-Location ' + 'Push-Location Read-Host Remove-Item Remove-ItemProperty Remove-PSDrive Remove-PSSnapin ' + 'Remove-Variable Rename-Item Rename-ItemProperty Resolve-Path Restart-Service Resume-Service ' + 'Select-Object Select-String Set-Acl Set-Alias Set-AuthenticodeSignature Set-Content ' + 'Set-Date Set-ExecutionPolicy Set-Item Set-ItemProperty Set-Location Set-PSDebug ' + 'Set-Service Set-TraceSource Set(-Variable)? Sort-Object Split-Path Start-Service ' + 'Start-Sleep Start-Transcript Stop-Process Stop-Service Stop-Transcript Suspend-Service ' + 'Tee-Object Test-Path Trace-Command Update-FormatData Update-TypeData Where(-Object)? ' + 'Write-Debug Write-Error Write(-Host)? Write-Output Write-Progress Write-Verbose Write-Warning';
        var b = 'ac asnp clc cli clp clv cpi cpp cvpa diff epal epcsv fc fl ' + 'ft fw gal gc gci gcm gdr ghy gi gl gm gp gps group gsv ' + 'gsnp gu gv gwmi iex ihy ii ipal ipcsv mi mp nal ndr ni nv oh rdr ' + 'ri rni rnp rp rsnp rv rvpa sal sasv sc select si sl sleep sort sp ' + 'spps spsv sv tee cat cd cp h history kill lp ls ' + 'mount mv popd ps pushd pwd r rm rmdir echo cls chdir del dir ' + 'erase rd ren type % \\?';
        this.regexList = [{
            regex: /#.*$/gm,
            css: 'comments'
        }, {
            regex: /\$[a-zA-Z0-9]+\b/g,
            css: 'value'
        }, {
            regex: /\-[a-zA-Z]+\b/g,
            css: 'keyword'
        }, {
            regex: SyntaxHighlighter.regexLib.doubleQuotedString,
            css: 'string'
        }, {
            regex: SyntaxHighlighter.regexLib.singleQuotedString,
            css: 'string'
        }, {
            regex: new RegExp(this.getKeywords(a), 'gmi'),
            css: 'keyword'
        }, {
            regex: new RegExp(this.getKeywords(b), 'gmi'),
            css: 'keyword'
        }
        ];
    };
    Brush.prototype = new SyntaxHighlighter.Highlighter();
    Brush.aliases = ['powershell', 'ps'];
    SyntaxHighlighter.brushes.PowerShell = Brush;
    typeof (exports) != 'undefined' ? exports.Brush = Brush : null;
})();
(function () {
    typeof (require) != 'undefined' ? SyntaxHighlighter = require('shCore').SyntaxHighlighter : null;
    function Brush() {
        var a = 'and assert break class continue def del elif else ' + 'except exec finally for from global if import in is ' + 'lambda not or pass print raise return try yield while';
        var b = '__import__ abs all any apply basestring bin bool buffer callable ' + 'chr classmethod cmp coerce compile complex delattr dict dir ' + 'divmod enumerate eval execfile file filter float format frozenset ' + 'getattr globals hasattr hash help hex id input int intern ' + 'isinstance issubclass iter len list locals long map max min next ' + 'object oct open ord pow print property range raw_input reduce ' + 'reload repr reversed round set setattr slice sorted staticmethod ' + 'str sum super tuple type type unichr unicode vars xrange zip';
        var c = 'None True False self cls class_';
        this.regexList = [{
            regex: SyntaxHighlighter.regexLib.singleLinePerlComments,
            css: 'comments'
        }, {
            regex: /^\s*@\w+/gm,
            css: 'decorator'
        }, {
            regex: /(['\"]{3})([^\1])*?\1/gm,
            css: 'comments'
        }, {
            regex: /"(?!")(?:\.|\\\"|[^\""\n])*"/gm,
            css: 'string'
        }, {
            regex: /'(?!')(?:\.|(\\\')|[^\''\n])*'/gm,
            css: 'string'
        }, {
            regex: /\+|\-|\*|\/|\%|=|==/gm,
            css: 'keyword'
        }, {
            regex: /\b\d+\.?\w*/g,
            css: 'value'
        }, {
            regex: new RegExp(this.getKeywords(b), 'gmi'),
            css: 'functions'
        }, {
            regex: new RegExp(this.getKeywords(a), 'gm'),
            css: 'keyword'
        }, {
            regex: new RegExp(this.getKeywords(c), 'gm'),
            css: 'color1'
        }
        ];
        this.forHtmlScript(SyntaxHighlighter.regexLib.aspScriptTags);
    };
    Brush.prototype = new SyntaxHighlighter.Highlighter();
    Brush.aliases = ['py', 'python'];
    SyntaxHighlighter.brushes.Python = Brush;
    typeof (exports) != 'undefined' ? exports.Brush = Brush : null;
})();
(function () {
    typeof (require) != 'undefined' ? SyntaxHighlighter = require('shCore').SyntaxHighlighter : null;
    function Brush() {
        var a = 'alias and BEGIN begin break case class def define_method defined do each else elsif ' + 'END end ensure false for if in module new next nil not or raise redo rescue retry return ' + 'self super then throw true undef unless until when while yield';
        var b = 'Array Bignum Binding Class Continuation Dir Exception FalseClass File::Stat File Fixnum Fload ' + 'Hash Integer IO MatchData Method Module NilClass Numeric Object Proc Range Regexp String Struct::TMS Symbol ' + 'ThreadGroup Thread Time TrueClass';
        this.regexList = [{
            regex: SyntaxHighlighter.regexLib.singleLinePerlComments,
            css: 'comments'
        }, {
            regex: SyntaxHighlighter.regexLib.doubleQuotedString,
            css: 'string'
        }, {
            regex: SyntaxHighlighter.regexLib.singleQuotedString,
            css: 'string'
        }, {
            regex: /\b[A-Z0-9_]+\b/g,
            css: 'constants'
        }, {
            regex: /:[a-z][A-Za-z0-9_]*/g,
            css: 'color2'
        }, {
            regex: /(\$|@@|@)\w+/g,
            css: 'variable bold'
        }, {
            regex: new RegExp(this.getKeywords(a), 'gm'),
            css: 'keyword'
        }, {
            regex: new RegExp(this.getKeywords(b), 'gm'),
            css: 'color1'
        }
        ];
        this.forHtmlScript(SyntaxHighlighter.regexLib.aspScriptTags);
    };
    Brush.prototype = new SyntaxHighlighter.Highlighter();
    Brush.aliases = ['ruby', 'rails', 'ror', 'rb'];
    SyntaxHighlighter.brushes.Ruby = Brush;
    typeof (exports) != 'undefined' ? exports.Brush = Brush : null;
})();
(function () {
    typeof (require) != 'undefined' ? SyntaxHighlighter = require('shCore').SyntaxHighlighter : null;
    function Brush() {
        function getKeywordsCSS(a) {
            return '\\b([a-z_]|)' + a.replace(/ /g, '(?=:)\\b|\\b([a-z_\\*]|\\*|)') + '(?=:)\\b';
        };
        function getValuesCSS(a) {
            return '\\b' + a.replace(/ /g, '(?!-)(?!:)\\b|\\b()') + '\:\\b';
        };
        var b = 'ascent azimuth background-attachment background-color background-image background-position ' + 'background-repeat background baseline bbox border-collapse border-color border-spacing border-style border-top ' + 'border-right border-bottom border-left border-top-color border-right-color border-bottom-color border-left-color ' + 'border-top-style border-right-style border-bottom-style border-left-style border-top-width border-right-width ' + 'border-bottom-width border-left-width border-width border bottom cap-height caption-side centerline clear clip color ' + 'content counter-increment counter-reset cue-after cue-before cue cursor definition-src descent direction display ' + 'elevation empty-cells float font-size-adjust font-family font-size font-stretch font-style font-variant font-weight font ' + 'height left letter-spacing line-height list-style-image list-style-position list-style-type list-style margin-top ' + 'margin-right margin-bottom margin-left margin marker-offset marks mathline max-height max-width min-height min-width orphans ' + 'outline-color outline-style outline-width outline overflow padding-top padding-right padding-bottom padding-left padding page ' + 'page-break-after page-break-before page-break-inside pause pause-after pause-before pitch pitch-range play-during position ' + 'quotes right richness size slope src speak-header speak-numeral speak-punctuation speak speech-rate stemh stemv stress ' + 'table-layout text-align top text-decoration text-indent text-shadow text-transform unicode-bidi unicode-range units-per-em ' + 'vertical-align visibility voice-family volume white-space widows width widths word-spacing x-height z-index';
        var c = 'above absolute all always aqua armenian attr aural auto avoid baseline behind below bidi-override black blink block blue bold bolder ' + 'both bottom braille capitalize caption center center-left center-right circle close-quote code collapse compact condensed ' + 'continuous counter counters crop cross crosshair cursive dashed decimal decimal-leading-zero digits disc dotted double ' + 'embed embossed e-resize expanded extra-condensed extra-expanded fantasy far-left far-right fast faster fixed format fuchsia ' + 'gray green groove handheld hebrew help hidden hide high higher icon inline-table inline inset inside invert italic ' + 'justify landscape large larger left-side left leftwards level lighter lime line-through list-item local loud lower-alpha ' + 'lowercase lower-greek lower-latin lower-roman lower low ltr marker maroon medium message-box middle mix move narrower ' + 'navy ne-resize no-close-quote none no-open-quote no-repeat normal nowrap n-resize nw-resize oblique olive once open-quote outset ' + 'outside overline pointer portrait pre print projection purple red relative repeat repeat-x repeat-y rgb ridge right right-side ' + 'rightwards rtl run-in screen scroll semi-condensed semi-expanded separate se-resize show silent silver slower slow ' + 'small small-caps small-caption smaller soft solid speech spell-out square s-resize static status-bar sub super sw-resize ' + 'table-caption table-cell table-column table-column-group table-footer-group table-header-group table-row table-row-group teal ' + 'text-bottom text-top thick thin top transparent tty tv ultra-condensed ultra-expanded underline upper-alpha uppercase upper-latin ' + 'upper-roman url visible wait white wider w-resize x-fast x-high x-large x-loud x-low x-slow x-small x-soft xx-large xx-small yellow';
        var d = '[mM]onospace [tT]ahoma [vV]erdana [aA]rial [hH]elvetica [sS]ans-serif [sS]erif [cC]ourier mono sans serif';
        var e = '!important !default';
        var f = '@import @extend @debug @warn @if @for @while @mixin @include';
        var r = SyntaxHighlighter.regexLib;
        this.regexList = [{
            regex: r.multiLineCComments,
            css: 'comments'
        }, {
            regex: r.singleLineCComments,
            css: 'comments'
        }, {
            regex: r.doubleQuotedString,
            css: 'string'
        }, {
            regex: r.singleQuotedString,
            css: 'string'
        }, {
            regex: /\#[a-fA-F0-9]{3,6}/g,
            css: 'value'
        }, {
            regex: /\b(-?\d+)(\.\d+)?(px|em|pt|\:|\%|)\b/g,
            css: 'value'
        }, {
            regex: /\$\w+/g,
            css: 'variable'
        }, {
            regex: new RegExp(this.getKeywords(e), 'g'),
            css: 'color3'
        }, {
            regex: new RegExp(this.getKeywords(f), 'g'),
            css: 'preprocessor'
        }, {
            regex: new RegExp(getKeywordsCSS(b), 'gm'),
            css: 'keyword'
        }, {
            regex: new RegExp(getValuesCSS(c), 'g'),
            css: 'value'
        }, {
            regex: new RegExp(this.getKeywords(d), 'g'),
            css: 'color1'
        }
        ];
    };
    Brush.prototype = new SyntaxHighlighter.Highlighter();
    Brush.aliases = ['sass', 'scss'];
    SyntaxHighlighter.brushes.Sass = Brush;
    typeof (exports) != 'undefined' ? exports.Brush = Brush : null;
})();
(function() {
    typeof(require) != 'undefined' ? SyntaxHighlighter = require('shCore').SyntaxHighlighter : null;

    function Brush() {
        var a = 'val sealed case def true trait implicit forSome import match object null finally super ' + 'override try lazy for var catch throw type extends class while with new final yield abstract ' + 'else do if return protected private this package false';
        var b = '[_:=><%#@]+';
        this.regexList = [{
                regex: SyntaxHighlighter.regexLib.singleLineCComments,
                css: 'comments'
            }, {
                regex: SyntaxHighlighter.regexLib.multiLineCComments,
                css: 'comments'
            }, {
                regex: SyntaxHighlighter.regexLib.multiLineSingleQuotedString,
                css: 'string'
            }, {
                regex: SyntaxHighlighter.regexLib.multiLineDoubleQuotedString,
                css: 'string'
            }, {
                regex: SyntaxHighlighter.regexLib.singleQuotedString,
                css: 'string'
            }, {
                regex: /0x[a-f0-9]+|\d+(\.\d+)?/gi,
                css: 'value'
            }, {
                regex: new RegExp(this.getKeywords(a), 'gm'),
                css: 'keyword'
            }, {
                regex: new RegExp(b, 'gm'),
                css: 'keyword'
            }
        ];
    }

    Brush.prototype = new SyntaxHighlighter.Highlighter();
    Brush.aliases = ['scala'];
    SyntaxHighlighter.brushes.Scala = Brush;
    typeof(exports) != 'undefined' ? exports.Brush = Brush : null;
})();
(function() {
    typeof(require) != 'undefined' ? SyntaxHighlighter = require('shCore').SyntaxHighlighter : null;

    function Brush() {
        var a = 'abs avg case cast coalesce convert count current_timestamp ' + 'current_user day isnull left lower month nullif replace right ' + 'session_user space substring sum system_user upper user year';
        var b = 'absolute action add after alter as asc at authorization begin bigint ' + 'binary bit by cascade char character check checkpoint close collate ' + 'column commit committed connect connection constraint contains continue ' + 'create cube current current_date current_time cursor database date ' + 'deallocate dec decimal declare default delete desc distinct double drop ' + 'dynamic else end end-exec escape except exec execute false fetch first ' + 'float for force foreign forward free from full function global goto grant ' + 'group grouping having hour ignore index inner insensitive insert instead ' + 'int integer intersect into is isolation key last level load local max min ' + 'minute modify move name national nchar next no numeric of off on only ' + 'open option order out output partial password precision prepare primary ' + 'prior privileges procedure public read real references relative repeatable ' + 'restrict return returns revoke rollback rollup rows rule schema scroll ' + 'second section select sequence serializable set size smallint static ' + 'statistics table temp temporary then time timestamp to top transaction ' + 'translation trigger true truncate uncommitted union unique update values ' + 'varchar varying view when where with work';
        var c = 'all and any between cross in join like not null or outer some';
        this.regexList = [{
                regex: /--(.*)$/gm,
                css: 'comments'
            }, {
                regex: SyntaxHighlighter.regexLib.multiLineDoubleQuotedString,
                css: 'string'
            }, {
                regex: SyntaxHighlighter.regexLib.multiLineSingleQuotedString,
                css: 'string'
            }, {
                regex: new RegExp(this.getKeywords(a), 'gmi'),
                css: 'color2'
            }, {
                regex: new RegExp(this.getKeywords(c), 'gmi'),
                css: 'color1'
            }, {
                regex: new RegExp(this.getKeywords(b), 'gmi'),
                css: 'keyword'
            }
        ];
    }

    ;
    Brush.prototype = new SyntaxHighlighter.Highlighter();
    Brush.aliases = ['sql'];
    SyntaxHighlighter.brushes.Sql = Brush;
    typeof(exports) != 'undefined' ? exports.Brush = Brush : null;
})();
(function () {
    typeof (require) != 'undefined' ? SyntaxHighlighter = require('shCore').SyntaxHighlighter : null;
    function Brush() {
        var a = 'AddHandler AddressOf AndAlso Alias And Ansi As Assembly Auto ' + 'Boolean ByRef Byte ByVal Call Case Catch CBool CByte CChar CDate ' + 'CDec CDbl Char CInt Class CLng CObj Const CShort CSng CStr CType ' + 'Date Decimal Declare Default Delegate Dim DirectCast Do Double Each ' + 'Else ElseIf End Enum Erase Error Event Exit False Finally For Friend ' + 'Function Get GetType GoSub GoTo Handles If Implements Imports In ' + 'Inherits Integer Interface Is Let Lib Like Long Loop Me Mod Module ' + 'MustInherit MustOverride MyBase MyClass Namespace New Next Not Nothing ' + 'NotInheritable NotOverridable Object On Option Optional Or OrElse ' + 'Overloads Overridable Overrides ParamArray Preserve Private Property ' + 'Protected Public RaiseEvent ReadOnly ReDim REM RemoveHandler Resume ' + 'Return Select Set Shadows Shared Short Single Static Step Stop String ' + 'Structure Sub SyncLock Then Throw To True Try TypeOf Unicode Until ' + 'Variant When While With WithEvents WriteOnly Xor';
        this.regexList = [{
            regex: /'.*$/gm,
            css: 'comments'
        }, {
            regex: SyntaxHighlighter.regexLib.doubleQuotedString,
            css: 'string'
        }, {
            regex: /^\s*#.*$/gm,
            css: 'preprocessor'
        }, {
            regex: new RegExp(this.getKeywords(a), 'gm'),
            css: 'keyword'
        }
        ];
        this.forHtmlScript(SyntaxHighlighter.regexLib.aspScriptTags);
    };
    Brush.prototype = new SyntaxHighlighter.Highlighter();
    Brush.aliases = ['vb', 'vbnet'];
    SyntaxHighlighter.brushes.Vb = Brush;
    typeof (exports) != 'undefined' ? exports.Brush = Brush : null;
})();
(function () {
    typeof (require) != 'undefined' ? SyntaxHighlighter = require('shCore').SyntaxHighlighter : null;
    function Brush() {
        function process(a, b) {
            var constructor = SyntaxHighlighter.Match,
                code = a[0],
                tag = new XRegExp('(&lt;|<)[\\s\\/\\?]*(?<name>[:\\w-\\.]+)', 'xg').exec(code),
                result = [];
            if (a.attributes != null) {
                var c, regex = new XRegExp('(?<name> [\\w:\\-\\.]+)' + '\\s*=\\s*' + '(?<value> ".*?"|\'.*?\'|\\w+)', 'xg');
                while ((c = regex.exec(code)) != null) {
                    result.push(new constructor(c.name, a.index + c.index, 'color1'));
                    result.push(new constructor(c.value, a.index + c.index + c[0].indexOf(c.value), 'string'));
                }
            }
            if (tag != null) result.push(new constructor(tag.name, a.index + tag[0].indexOf(tag.name), 'keyword'));
            return result;
        }
        this.regexList = [{
            regex: new XRegExp('(\\&lt;|<)\\!\\[[\\w\\s]*?\\[(.|\\s)*?\\]\\](\\&gt;|>)', 'gm'),
            css: 'color2'
        }, {
            regex: SyntaxHighlighter.regexLib.xmlComments,
            css: 'comments'
        }, {
            regex: new XRegExp('(&lt;|<)[\\s\\/\\?]*(\\w+)(?<attributes>.*?)[\\s\\/\\?]*(&gt;|>)', 'sg'),
            func: process
        }
        ];
    };
    Brush.prototype = new SyntaxHighlighter.Highlighter();
    Brush.aliases = ['xml', 'xhtml', 'xslt', 'html'];
    SyntaxHighlighter.brushes.Xml = Brush;
    typeof (exports) != 'undefined' ? exports.Brush = Brush : null;
})();
(function () {
    typeof (require) != 'undefined' ? SyntaxHighlighter = require('shCore').SyntaxHighlighter : null;
    function Brush() {
        var a = 'ATOM BOOL BOOLEAN BYTE CHAR COLORREF DWORD DWORDLONG DWORD_PTR ' + 'DWORD32 DWORD64 FLOAT HACCEL HALF_PTR HANDLE HBITMAP HBRUSH ' + 'HCOLORSPACE HCONV HCONVLIST HCURSOR HDC HDDEDATA HDESK HDROP HDWP ' + 'HENHMETAFILE HFILE HFONT HGDIOBJ HGLOBAL HHOOK HICON HINSTANCE HKEY ' + 'HKL HLOCAL HMENU HMETAFILE HMODULE HMONITOR HPALETTE HPEN HRESULT ' + 'HRGN HRSRC HSZ HWINSTA HWND INT INT_PTR INT32 INT64 LANGID LCID LCTYPE ' + 'LGRPID LONG LONGLONG LONG_PTR LONG32 LONG64 LPARAM LPBOOL LPBYTE LPCOLORREF ' + 'LPCSTR LPCTSTR LPCVOID LPCWSTR LPDWORD LPHANDLE LPINT LPLONG LPSTR LPTSTR ' + 'LPVOID LPWORD LPWSTR LRESULT PBOOL PBOOLEAN PBYTE PCHAR PCSTR PCTSTR PCWSTR ' + 'PDWORDLONG PDWORD_PTR PDWORD32 PDWORD64 PFLOAT PHALF_PTR PHANDLE PHKEY PINT ' + 'PINT_PTR PINT32 PINT64 PLCID PLONG PLONGLONG PLONG_PTR PLONG32 PLONG64 POINTER_32 ' + 'POINTER_64 PSHORT PSIZE_T PSSIZE_T PSTR PTBYTE PTCHAR PTSTR PUCHAR PUHALF_PTR ' + 'PUINT PUINT_PTR PUINT32 PUINT64 PULONG PULONGLONG PULONG_PTR PULONG32 PULONG64 ' + 'PUSHORT PVOID PWCHAR PWORD PWSTR SC_HANDLE SC_LOCK SERVICE_STATUS_HANDLE SHORT ' + 'SIZE_T SSIZE_T TBYTE TCHAR UCHAR UHALF_PTR UINT UINT_PTR UINT32 UINT64 ULONG ' + 'ULONGLONG ULONG_PTR ULONG32 ULONG64 USHORT USN VOID WCHAR WORD WPARAM WPARAM WPARAM ' + 'char bool short int __int32 __int64 __int8 __int16 long float double __wchar_t ' + 'clock_t _complex _dev_t _diskfree_t div_t ldiv_t _exception _EXCEPTION_POINTERS ' + 'FILE _finddata_t _finddatai64_t _wfinddata_t _wfinddatai64_t __finddata64_t ' + '__wfinddata64_t _FPIEEE_RECORD fpos_t _HEAPINFO _HFILE lconv intptr_t ' + 'jmp_buf mbstate_t _off_t _onexit_t _PNH ptrdiff_t _purecall_handler ' + 'sig_atomic_t size_t _stat __stat64 _stati64 terminate_function ' + 'time_t __time64_t _timeb __timeb64 tm uintptr_t _utimbuf ' + 'va_list wchar_t wctrans_t wctype_t wint_t signed';
        var b = 'break case catch class const __finally __exception __try ' + 'const_cast continue private public protected __declspec ' + 'default delete deprecated dllexport dllimport do dynamic_cast ' + 'else enum explicit extern if for friend goto inline ' + 'mutable naked namespace new noinline noreturn nothrow ' + 'register reinterpret_cast return selectany ' + 'sizeof static static_cast struct switch template this ' + 'thread throw true false try typedef typeid typename union ' + 'using uuid virtual void volatile whcar_t while';
        var c = 'assert isalnum isalpha iscntrl isdigit isgraph islower isprint' + 'ispunct isspace isupper isxdigit tolower toupper errno localeconv ' + 'setlocale acos asin atan atan2 ceil cos cosh exp fabs floor fmod ' + 'frexp ldexp log log10 modf pow sin sinh sqrt tan tanh jmp_buf ' + 'longjmp setjmp raise signal sig_atomic_t va_arg va_end va_start ' + 'clearerr fclose feof ferror fflush fgetc fgetpos fgets fopen ' + 'fprintf fputc fputs fread freopen fscanf fseek fsetpos ftell ' + 'fwrite getc getchar gets perror printf putc putchar puts remove ' + 'rename rewind scanf setbuf setvbuf sprintf sscanf tmpfile tmpnam ' + 'ungetc vfprintf vprintf vsprintf abort abs atexit atof atoi atol ' + 'bsearch calloc div exit free getenv labs ldiv malloc mblen mbstowcs ' + 'mbtowc qsort rand realloc srand strtod strtol strtoul system ' + 'wcstombs wctomb memchr memcmp memcpy memmove memset strcat strchr ' + 'strcmp strcoll strcpy strcspn strerror strlen strncat strncmp ' + 'strncpy strpbrk strrchr strspn strstr strtok strxfrm asctime ' + 'clock ctime difftime gmtime localtime mktime strftime time';
        this.regexList = [{
            regex: SyntaxHighlighter.regexLib.singleLineCComments,
            css: 'comments'
        }, {
            regex: SyntaxHighlighter.regexLib.multiLineCComments,
            css: 'comments'
        }, {
            regex: SyntaxHighlighter.regexLib.doubleQuotedString,
            css: 'string'
        }, {
            regex: SyntaxHighlighter.regexLib.singleQuotedString,
            css: 'string'
        }, {
            regex: /^ *#.*/gm,
            css: 'preprocessor'
        }, {
            regex: new RegExp(this.getKeywords(a), 'gm'),
            css: 'color1 bold'
        }, {
            regex: new RegExp(this.getKeywords(c), 'gm'),
            css: 'functions bold'
        }, {
            regex: new RegExp(this.getKeywords(b), 'gm'),
            css: 'keyword bold'
        }
        ];
    };
    Brush.prototype = new SyntaxHighlighter.Highlighter();
    Brush.aliases = ['cpp', 'c'];
    SyntaxHighlighter.brushes.Cpp = Brush;
    typeof (exports) != 'undefined' ? exports.Brush = Brush : null;
})();
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

        var icon = $(settings.Dialog).find(".DialogIcon").eq(0).attr('src');

        var iconsPath = settings.ImagePath;

        iconsPath = iconsPath.replace("images/", "icons/");

        if (settings.Type == 'error') {
            icon = iconsPath + 'ErrorBig.png'; // over write the message to error message
        } else if (settings.Type == 'information') {
            icon = iconsPath + 'InfoBig.png'; // over write the message to information message
        } else if (settings.Type == 'warning') {
            icon = iconsPath + 'WarningBig.png'; // over write the message to warning message
        }

        $(settings.Dialog).find(".DialogIcon").eq(0).attr('src', icon);

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
    var target, newmenu;

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

    newmenu = document.getElementById(menuName);

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

    jQuery(".standardSelectMenu").selectmenu();
});

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

document.onclick = yaf_hidemenu;
if (document.addEventListener) document.addEventListener("click", function(e) { window.event = e; }, true);
if (document.addEventListener) document.addEventListener("mouseover", function(e) { window.event = e; }, true);
