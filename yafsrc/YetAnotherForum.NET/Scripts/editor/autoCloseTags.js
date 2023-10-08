/**
 * Auto Close BBCode tags
 * 
 * Original author... 
 * @author itsmikita@gmail.com
 */

var AutoCloseTags = function(textarea) {
        this.textarea = textarea;

        // Auto-close tags
        this.autoClosingTags = [
            "b", "i", "u", "h", "code", "img", "quote", "left", "center", "right",
            "indent", "list", "color",
            "size", "albumimg", "attach", "youtube", "vimeo",
            "instagram", "twitter", "facebook", "googlewidget", "spoiler", "userlink", "googlemaps",
            "hide", "group-hide", "hide-thanks", "hide-reply-thanks", "hide-reply", "hide-posts", "dailymotion",
            "audio", "media"
        ];
        this.enableAutoCloseTags();
    };

AutoCloseTags.prototype = {
    enableAutoCloseTags: function() {
        var self = this;

        this.textarea.addEventListener("keydown",
            function(event) {
                const keyCode = event.key;

                // Close tag ']'
                if (keyCode === "]") {
                    const position = this.selectionStart;
                    const before = this.value.substr(0, position);
                    const after = this.value.substr(this.selectionEnd, this.value.length);
                    let tagName;

                    try {
                        tagName = before.match(/\[([^\]]+)$/)[1].match(/^([a-z1-6]+)/)[1];
                    } catch (e) {
                        return;
                    }

                    // Not an auto-closing tag
                    if (-1 === self.autoClosingTags.indexOf(tagName))
                        return;

                    const closeTag = `[/${tagName}]`;

                    this.value = before + closeTag + after;
                    this.selectionStart =
                        this.selectionEnd = position;
                    this.focus();
                }
            });
    }
};

document.addEventListener("DOMContentLoaded", function () {
    const autoCloseTags = new AutoCloseTags(document.querySelector(".BBCodeEditor"));
})