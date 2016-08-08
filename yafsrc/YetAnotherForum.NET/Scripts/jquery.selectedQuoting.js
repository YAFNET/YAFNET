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

        this.textSelection = '';
        this.htmlSelection = '';

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

        this.showPopunder = function () {
            self.popunder = self.popunder || document.getElementById('selectionSharerPopunder');

            var sel = window.getSelection();
            var selection = self.getSelectionText(sel);

            if (sel.isCollapsed || selection.length < 10 || !selection.match(/ /))
                return self.hidePopunder();

            if (self.popunder.classList.contains("fixed"))
                return self.popunder.style.bottom = 0;

            var range = sel.getRangeAt(0);
            var node = range.endContainer.parentNode; // The <p> where the selection ends

            // If the popunder is currently displayed
            if (self.popunder.classList.contains('show')) {
                // If the popunder is already at the right place, we do nothing
                if (Math.ceil(self.popunder.getBoundingClientRect().top) == Math.ceil(node.getBoundingClientRect().bottom))
                    return;

                // Otherwise, we first hide it and the we try again
                return self.hidePopunder(self.showPopunder);
            }

            if (node.nextElementSibling) {
                // We need to push down all the following siblings
                self.pushSiblings(node);
            } else {
                // We need to append a new element to push all the content below
                if (!self.placeholder) {
                    self.placeholder = document.createElement('div');
                    self.placeholder.className = 'selectionSharerPlaceholder';
                }

                // If we add a div between two <p> that have a 1em margin, the space between them
                // will become 2x 1em. So we give the placeholder a negative margin to avoid that
                var margin = window.getComputedStyle(node).marginBottom;
                self.placeholder.style.height = margin;
                self.placeholder.style.marginBottom = (-2 * parseInt(margin, 10)) + 'px';
                node.parentNode.insertBefore(self.placeholder);
            }

            // scroll offset
            var offsetTop = window.pageYOffset + node.getBoundingClientRect().bottom;
            self.popunder.style.top = Math.ceil(offsetTop) + 'px';

            setTimeout(function() {
                if (self.placeholder) self.placeholder.classList.add('show');
                self.popunder.classList.add('show');
            }, 0);

        };

        this.pushSiblings = function(el) {
            while (el = el.nextElementSibling) {
                el.classList.add('selectionSharer');
                el.classList.add('moveDown');
            }
        };

        this.hidePopunder = function(cb) {
            cb = cb || function() {};

            if (self.popunder == "fixed") {
                self.popunder.style.bottom = '-50px';
                return cb();
            }

            self.popunder.classList.remove('show');
            if (self.placeholder) self.placeholder.classList.remove('show');
            // We need to push back up all the siblings
            var els = document.getElementsByClassName('moveDown');
            while (el = els[0]) {
                el.classList.remove('moveDown');
            }

            // CSS3 transition takes 0.6s
            setTimeout(function() {
                if (self.placeholder) document.body.insertBefore(self.placeholder);
                cb();
            }, 600);

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
                    case 'forward':
                        left -= self.$popover.width();
                        break;
                    case 'backward':
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
            s_ = toLong ? s_.substr(0, s_.lastIndexOf(' ')) : s_;
            return toLong ? s_ + '...' : s_;
        };

        this.shareQuote = function (e) {
            e.preventDefault();

            var messageID = $(this).attr("id").replace("Message", "");

            var text = self.htmlSelection.replace(/<p[^>]*>/ig, '\n').replace(/<\/p>|  /ig, '').trim();
            var url = self.URL + '&q=' + messageID + '&text=' + encodeURIComponent(text);

            window.location.href = url;
        };

        this.render = function () {
            var popoverHTML = '<div class="selectionSharer" id="selectionSharerPopover" style="position:absolute;">'
                + '  <div id="selectionSharerPopover-inner">'
                + '    <ul>'
                + '      <li><a class="action quote" id="Message' + self.parentID + '" href="" title="' + self.ToolTip + '"><svg width="20" height="20"><path d="M1920 1024q0 174-120 321.5t-326 233-450 85.5q-70 0-145-8-198 175-460 242-49 14-114 22-17 2-30.5-9t-17.5-29v-1q-3-4-.5-12t2-10 4.5-9.5l6-9 7-8.5 8-9q7-8 31-34.5t34.5-38 31-39.5 32.5-51 27-59 26-76q-157-89-247.5-220t-90.5-281q0-130 71-248.5t191-204.5 286-136.5 348-50.5q244 0 450 85.5t326 233 120 321.5z" fill="#fff"/></svg></a></li>'
                + '    </ul>'
                + '  </div>'
                + '  <div class="selectionSharerPopover-clip"><span class="selectionSharerPopover-arrow"></span></div>'
                + '</div>';

            var popunderHTML = '<div id="selectionSharerPopunder" class="selectionSharer">'
                + '  <div id="selectionSharerPopunder-inner">'
                + '    <label>' + self.ToolTip + '</label>'
                + '    <ul>'
                + '      <li><a class="action quote" id="Message' + self.parentID + '" href="" title="' + self.ToolTip + '"><svg width="20" height="20"><path d="M1920 1024q0 174-120 321.5t-326 233-450 85.5q-70 0-145-8-198 175-460 242-49 14-114 22-17 2-30.5-9t-17.5-29v-1q-3-4-.5-12t2-10 4.5-9.5l6-9 7-8.5 8-9q7-8 31-34.5t34.5-38 31-39.5 32.5-51 27-59 26-76q-157-89-247.5-220t-90.5-281q0-130 71-248.5t191-204.5 286-136.5 348-50.5q244 0 450 85.5t326 233 120 321.5z" fill="#fff"/></svg></a></li>'
                + '    </ul>'
                + '  </div>'
                + '</div>';


            self.$popover = $(popoverHTML);

            self.$popover.find('a.quote').click(self.shareQuote);

            $('body').append(self.$popover);

            self.$popunder = $(popunderHTML);
            self.$popunder.find('a.quote').click(self.shareQuote);
            $('body').append(self.$popunder);
        };

        this.setElements = function(elements) {
            if (typeof elements == 'string') elements = $(elements);
            self.$elements = elements instanceof $ ? elements : $(elements);
            self.$elements.mouseup(self.show).mousedown(self.hide);

            self.$elements.bind('touchstart', function(e) {
                self.isMobile = true;
            });

            document.onselectionchange = self.selectionChanged;
        };

        this.selectionChanged = function(e) {
            if (!self.isMobile) return;

            if (self.lastSelectionChanged) {
                clearTimeout(self.lastSelectionChanged);
            }
            self.lastSelectionChanged = setTimeout(function() {
                self.showPopunder(e);
            }, 300);
        };

        this.render();
    };

    $.fn.selectedQuoting = function (options) {
        options.parentID = this.attr('id');
        var sharer = new SelectedQuoting(options);
        sharer.setElements(this);
        return this;
    };
})(jQuery);


