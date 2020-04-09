/*
 * Selected-Quoting for Messages based on the
 * selection-sharer Plugin by
 *  Xavier Damman (@xdamman) at
    https://github.com/xdamman/share-selection
 *
 * Author: Xavier Damman (@xdamman)
 * MIT License
 */

(function($) {

    var SelectedQuoting = function(options) {
        var self = this;
        this.sel = null;

        this.parentID = options.parentID;
        this.URL = options.URL;
        this.ToolTip = options.ToolTip;

        this.textSelection = "";
        this.htmlSelection = "";

        this.getSelectionText = function(selection) {
            var html = "", text = "";
            var sel = selection || window.getSelection();
            if (sel.rangeCount) {
                var container = document.createElement("div");
                for (var i = 0, len = sel.rangeCount; i < len; ++i) {
                    container.appendChild(sel.getRangeAt(i).cloneContents());
                }
                text = container.textContent;
                html = container.innerHTML;
            }
            self.textSelection = text;
            self.htmlSelection = html || text;
            return text;
        };

        this.selectionDirection = function(selection) {
            var sel = selection || window.getSelection();
            var range = document.createRange();
            if (!sel.anchorNode) return 0;
            range.setStart(sel.anchorNode, sel.anchorOffset);
            range.setEnd(sel.focusNode, sel.focusOffset);
            var direction = (range.collapsed) ? "backward" : "forward";
            return direction;
        };

        this.pushSiblings = function(el) {
            while (el = el.nextElementSibling) {
                el.classList.add("selectionSharer");
                el.classList.add("moveDown");
            }
        };

        this.show = function(e) {
            setTimeout(function() {
                var sel = window.getSelection();
                var selection = self.getSelectionText(sel);
                if (!sel.isCollapsed && selection && selection.length > 10 && selection.match(/ /)) {
                    var range = sel.getRangeAt(0);
                    var topOffset = range.getBoundingClientRect().top - 5;
                    var top = topOffset + window.scrollY - self.$popover.height();
                    var left = 0;
                    if (e) {
                        left = e.pageX;
                    } else {
                        var obj = sel.anchorNode.parentNode;
                        left += obj.offsetWidth / 2;
                        do {
                            left += obj.offsetLeft;
                        } while (obj = obj.offsetParent);
                    }
                    switch (self.selectionDirection(sel)) {
                    case "forward":
                        left -= self.$popover.width();
                        break;
                    case "backward":
                        left += self.$popover.width();
                        break;
                    default:
                        return;
                    }
                    self.$popover.removeClass("anim").css("top", top + 10).css("left", left).show();
                    setTimeout(function() {
                        self.$popover.addClass("anim").css("top", top);
                    }, 0);
                }
            }, 10);
        };

        this.hide = function(e) {
            self.$popover.hide();
        };

        this.smart_truncate = function(str, n) {
            if (!str || !str.length) return str;
            var toLong = str.length > n,
                s_ = toLong ? str.substr(0, n - 1) : str;
            s_ = toLong ? s_.substr(0, s_.lastIndexOf(" ")) : s_;
            return toLong ? s_ + "..." : s_;
        };

        this.shareQuote = function (e) {
            e.preventDefault();

            var messageID = $(this).attr("id").replace("Message", "");

            var text = self.htmlSelection.replace(/<p[^>]*>/ig, "\n").replace(/<\/p>|  /ig, "").trim();
            var url = self.URL + "&q=" + messageID + "&text=" + encodeURIComponent(text);

            window.location.href = url;
        };

        this.render = function () {
            var popoverHTML = '<div class="selectionSharer" id="selectionSharerPopover" style="position:absolute;">'
                + '  <div id="selectionSharerPopover-inner">'
                + "    <ul>"
                + '      <li><a class="action quote" id="Message' + self.parentID + '" href="" title="' + self.ToolTip + '"><i class="fas fa-quote-left"></i></a></li>'
                + "    </ul>"
                + "  </div>"
                + '  <div class="selectionSharerPopover-clip"><span class="selectionSharerPopover-arrow"></span></div>'
                + "</div>";


            self.$popover = $(popoverHTML);

            self.$popover.find("a.quote").click(self.shareQuote);

            $("body").append(self.$popover);
        };

        this.setElements = function(elements) {
            if (typeof elements == "string") elements = $(elements);
            self.$elements = elements instanceof $ ? elements : $(elements);
            self.$elements.mouseup(self.show).mousedown(self.hide);

            self.$elements.bind("touchstart", function(e) {
                self.isMobile = true;
            });

            document.onselectionchange = self.selectionChanged;
        };

        this.selectionChanged = function(e) {
            if (!self.isMobile) return;

            if (self.lastSelectionChanged) {
                clearTimeout(self.lastSelectionChanged);
            }
        };

        this.render();
    };

    $.fn.selectedQuoting = function (options) {
        options.parentID = this.attr("id");
        var sharer = new SelectedQuoting(options);
        sharer.setElements(this);
        return this;
    };
})(jQuery);


