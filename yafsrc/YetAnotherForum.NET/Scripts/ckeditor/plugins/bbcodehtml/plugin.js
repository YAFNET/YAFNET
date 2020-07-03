/*
 * BBCode Html to fix issue when using bb codes in the html Mode
 */

CKEDITOR.plugins.add( 'bbcodehtml',
{
	requires : [ 'htmlwriter' ],
	init : function( editor ) {
		editor.dataProcessor = new CKEDITOR.htmlDataProcessor( editor );
	},
	beforeInit:function(editor) {
		addEventOn(editor);
	}
});

function addEventOn(editor) {
  editor.on('paste', function (evt){
	evt.data['html'] = ConvertHtmlToBBCode(evt.data.dataValue);
  });
}

function ConvertHtmlToBBCode(html) {
		// Convert <br> to line breaks.
        html = html.replace(/<br(?=[ \/>]).*?>/gi, '\r\n');

    return html;
}

CKEDITOR.htmlDataProcessor.prototype =
{
    toHtml: function (data, fixForBody) {

        // Convert line breaks to <br>.
        data = data.replace(/(?:\r\n|\n|\r)/g, '<br>');
		
		return data;
    },

    toDataFormat: function (html, fixForBody) {
        return ConvertHtmlToBBCode(html);
    }
};