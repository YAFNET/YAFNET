/*
 * BBCode Plugin v1.0 for CKEditor - http://www.site-top.com/
 * Copyright (c) 2010, PitBult.
 * - GNU Lesser General Public License Version 2.1 or later (the "LGPL")
 */

CKEDITOR.plugins.add( 'bbcode',
{
	requires : [ 'htmlwriter' ],
	init : function( editor )
	{
		editor.dataProcessor = new CKEDITOR.htmlDataProcessor( editor );
	}
});

CKEDITOR.htmlDataProcessor.prototype =
{
	toHtml : function( data, fixForBody )
	{
		
		// Convert < and > to their HTML entities.
		data = data.replace( /</g, '&lt;' ) ;
		data = data.replace( />/g, '&gt;' ) ;

		// Convert line breaks to <br>.
		data = data.replace( /(?:\r\n|\n|\r)/g, '<br>' ) ;

		// [url]
		data = data.replace( /\[url\](.+?)\[\/url]/gi, '<a href="$1">$1</a>' ) ;
		data = data.replace( /\[url\=([^\]]+)](.+?)\[\/url]/gi, '<a href="$1">$2</a>' ) ;

		// [b]
		data = data.replace( /\[b\](.+?)\[\/b]/gi, '<b>$1</b>' ) ;

		// [i]
		data = data.replace( /\[i\](.+?)\[\/i]/gi, '<i>$1</i>' ) ;

		// [u]
		data = data.replace( /\[u\](.+?)\[\/u]/gi, '<u>$1</u>' ) ;
		
		// [img]
		data = data.replace(/\[img\](.*?)\[\/img\]/gi,'<img src="$1" />');
		data = data.replace( /\[img\=([^\]]+)](.+?)\[\/img]/gi, '<img src="$1" alt="$2" title="$2" />' ) ;
		
		// [quote=username;1234]
		data = data.replace(/\[quote=(.*?)\]/gi,'<blockquote title="$1">');
		
		// [quote]
		data = data.replace( /\[quote\]/gi, '<blockquote>' ) ;
		data = data.replace( /\[\/quote]/gi, "</blockquote> \n" ) ;
		
		// [code]
		data = data.replace(/\[code\]/gi,'<code>');
		// [code=language]
		data = data.replace(/\[code=(.+?)\]/gi,'<code title="$1">');
		data = data.replace(/\[\/code\]/gi,'</code>');
		
		// [color]
		data = data.replace(/\[color=(.*?)\](.*?)\[\/color\]/gi, '<span style="color: $1">$2</span>');

		// [youtube]
/*		data = data.replace(/\[youtube\](.*?)\[\/youtube\]/gi, '<object width="425" height="350"><param name="movie" value="$1"></param><param name="wmode" value="transparent" /><embed src="$1" type="application/x-shockwave-flash" width="425" height="350" wmode="transparent"></embed></object>');*/

        // [left]
		data = data.replace(/\[left\](.*?)\[\/left\]/gi,'<div style="text-align:left">$1</div>');
		
		// [center]
		data = data.replace(/\[center\](.*?)\[\/center\]/gi,'<div style="text-align:center">$1</div>');
		
		// [right]
		data = data.replace(/\[right\](.*?)\[\/right\]/gi,'<div style="text-align:right">$1</div>');
		
		// [list]
		data = data.replace(/\[list\](.*?)\[\/list\]/gi,'<ul>$1</ul>');
		
		// [list=1]
		data = data.replace(/\[list=1\](.*?)\[\/list\]/gi,'<ol>$1</ol>');
		
		// [*]
		data = data.replace(/\[\*]/gi,'<li>');
			
		return data;
	},

	toDataFormat : function( html, fixForBody )
	{
		// Convert <br> to line breaks.
		html = html.replace( /<br(?=[ \/>]).*?>/gi, '\r\n') ;
		html = html.replace(/<p>/gi,"");
		html = html.replace(/<\/p>/gi,"\n");
		html = html.replace(/&nbsp;/gi," ");

		// [url]
		html = html.replace( /<a .*?href=(["'])(.+?)\1.*?>(.+?)<\/a>/gi, '[url=$2]$3[/url]') ;
		//html = html.replace(/<a.*?href=\"(.*?)\".*?>(.*?)<\/a>/gi,"[url=$1]$2[/url]");

		// [b]
		html = html.replace( /<(?:b|strong)>/gi, '[b]') ;
		html = html.replace( /<\/(?:b|strong)>/gi, '[/b]') ;

		// [i]
		html = html.replace( /<(?:i|em)>/gi, '[i]') ;
		html = html.replace( /<\/(?:i|em)>/gi, '[/i]') ;

		// [u]
		html = html.replace( /<u>/gi, '[u]') ;
		html = html.replace( /<\/u>/gi, '[/u]') ;
		
		// [img]
		html = html.replace( /<img .*?src=(["'])(.+?)\1.*?alt=(["'])(.+?)\1.*?>/gi, '[img=$2]$4[/img]') ;		
		html = html.replace( /<img .*?src=(["'])(.+?)\1.*?\/>/gi, '[img]$2[/img]') ;
		html = html.replace( /<img .*?src=(["'])(.+?)\1.*?>/gi, '[img]$2[/img]') ;
		
		
		// [quote=username;1234]
		html = html.replace(/<blockquote title=\"(.*?)\">/gi,"[quote=$1]");
		
		// [quote]
		html = html.replace( /<blockquote>/gi, '[quote]') ;
		
	    html = html.replace( /<\/blockquote>/gi, '[/quote]') ;
		
		// [code]
		html = html.replace( /<code>/gi, '[code]') ;
		// [code=language]
        html = html.replace(/<code title=\"(.*?)\">/gi,"[code=$1]");
		html = html.replace( /<\/code>/gi, '[/code]') ;
		
		// [color]
		html = html.replace(/<span style=\"color: ?(.*?);\">(.*?)<\/span>/gi,"[color=$1]$2[/color]");
		html = html.replace(/<font.*?color=\"(.*?)\".*?>(.*?)<\/font>/gi, "[color=$1]$2[/color]");
		
		// [youtube]
		//html = html.replace(/<object.*?><param name="movie" value="(.*?)">.*?<\/object>/gi, "[youtube]$1[/youtube]");
		
        // [left], [center] and [right] 
        html = html.replace(/<p style=\"text-align:(.*?)\">(.*?)<\/p>/gi, '[$1]$2[/$1]')
		html = html.replace(/<div style=\"text-align:(.*?)\">(.*?)<\/div>/gi, '[$1]$2[/$1]')
		
		// [list=1]
		html = html.replace(/<ol>(.*?)<\/ol>/gi, '[list=1]$1[/list]')
		
		// [list]
		html = html.replace(/<ul>(.*?)<\/ul>/gi, '[list]$1[/list]')
		
		// [*]
		html = html.replace(/<li>(.*?)<\/li>/gi, '[*]$1')
		html = html.replace(/<li>/gi, '[*]')
		
		
		// Remove remaining tags.
		html = html.replace( /<[^>]+>/g, '') ;

		return html;
	}
};