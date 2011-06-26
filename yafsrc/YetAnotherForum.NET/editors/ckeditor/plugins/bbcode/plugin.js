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
		
		// [h]
		data = data.replace( /\[h\](.+?)\[\/h]/gi, '<span style="background:yellow">$1</span>' ) ;
		
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
		data = data.replace(/\[left\]/gi,'<div style="text-align:left">');
		data = data.replace(/\[\/left\]/gi,'</div>');
		
		// [center]
		data = data.replace(/\[center\]/gi,'<div style="text-align:center">');
		data = data.replace(/\[\/center\]/gi,'</div>');
		
		// [right]
		data = data.replace(/\[right\]/gi,'<div style="text-align:right">');
		data = data.replace(/\[\/right\]/gi,'</div>');
		
		// [list]
		data = data.replace(/\[list\](.*?)\[\/list\]/gi, '<ul>$1</ul>');

		// [list=1]
		data = data.replace(/\[list=1\](.*?)\[\/list\]/gi, '<ol style="list-style-type:number">$1</ol>');
		
		// [list=a]
		data = data.replace(/\[list=a\](.*?)\[\/list\]/gi, '<ol style="list-style-type:lower-alpha">$1</ol>');

		// [list=A]
		data = data.replace(/\[list=A\](.*?)\[\/list\]/gi, '<ol style="list-style-type:upper-alpha">$1</ol>');

		// [list=i]
		data = data.replace(/\[list=i\](.*?)\[\/list\]/gi, '<ol style="list-style-type:lower-roman">$1</ol>');

		// [list=I]
		data = data.replace(/\[list=I\](.*?)\[\/list\]/gi, '<ol style="list-style-type:upper-roman">$1</ol>');

		// [list=1]
		data = data.replace(/\[list=1\](.*?)\[\/list\]/gi, '<ol>$1</ol>');
		
		// [*]
		data = data.replace(/\[\*]/gi,'<li>');
		
		// [size=1]
		data = data.replace(/\[size=(.*?)\](((\n|.)*).*?)\[\/size\]/gi,'<span style="font-size:$1">$2</span>');
		
		// [font=?]
		data = data.replace(/\[font=(.*?)\](((\n|.)*).*?)\[\/font\]/gi,'<span style="font-family:$1">$2</span>');
			
		return data;
	},

	toDataFormat : function( html, fixForBody )
	{
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
		
		// [h]
		html = html.replace( /<span style=\"background:yellow\">(.+?)<\/span>/gi, '[h]$1[/h]' ) ;
		html = html.replace( /<span class=\"highlight\">(.+?)<\/span>/gi, '[h]$1[/h]' ) ;
		
		// [img]
		html = html.replace( /<img .*?alt=(["'])(.+?)\1.*?src=(["'])(.+?)\1.*?.*?>/gi, '[img=$4]$2[/img]') ;		
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
        html = html.replace(/<p style=\"text-align:(.+?)\">(((\n|.)*).*?)<\/p>/gi, '[$1]$2[/$1]');
		html = html.replace(/<div style=\"text-align:(.+?)\">(((\n|.)*).*?)<\/div>/gi, '[$1]$2[/$1]');
		
		//html = html.replace(/<div style=\"text-align:(.*?)\">/gi, '[$1]');
		
		// [list=1]
		html = html.replace(/<ol style=\"list-style-type:number\">(.*?)<\/ol>/gi, '[list=1]$1[/list]');
		html = html.replace(/<ol type=\"1\">(.*?)<\/ol>/gi, '[list=1]$1[/list]');
		html = html.replace(/<ol>(.*?)<\/ol>/gi, '[list=1]$1[/list]');

		// [list=a]
		html = html.replace(/<ol style=\"list-style-type:lower-alpha\">(.*?)<\/ol>/gi, '[list=a]$1[/list]');
		html = html.replace(/<ol type=\"a\">(.*?)<\/ol>/gi, '[list=a]$1[/list]');

		// [list=A]
		html = html.replace(/<ol style=\"list-style-type:upper-alpha\">(.*?)<\/ol>/gi, '[list=A]$1[/list]');
		html = html.replace(/<ol type=\"A\">(.*?)<\/ol>/gi, '[list=A]$1[/list]');

		// [list=i]
		html = html.replace(/<ol style=\"list-style-type:lower-roman\">(.*?)<\/ol>/gi, '[list=i]$1[/list]');
		html = html.replace(/<ol type=\"i\">(.*?)<\/ol>/gi, '[list=i]$1[/list]');

		// [list=I]
		html = html.replace(/<ol style=\"list-style-type:upper-roman\">(.*?)<\/ol>/gi, '[list=I]$1[/list]');
		html = html.replace(/<ol type=\"I\">(.*?)<\/ol>/gi, '[list=I]$1[/list]');
		
		// [list]
		html = html.replace(/<ul>(.*?)<\/ul>/gi, '[list]$1[/list]');
		
		// [*]
		html = html.replace(/<li>(.*?)<\/li>/gi, '[*]$1');
		html = html.replace(/<li>/gi, '[*]');
		
        // [size=1]
	    html = html.replace(/<span style=\"font-size:(.*?);\">(((\n|.)*).*?)<\/span>/gi,"[size=$1]$2[/size]");
		html = html.replace(/<p style=\"font-size:(.*?);\">(((\n|.)*).*?)<\/p>/gi,"[size=$1]$2[/size]");
		html = html.replace(/<span style=\"font-size:(.*?)\">(((\n|.)*).*?)<\/span>/gi,"[size=$1]$2[/size]");
		html = html.replace(/<p style=\"font-size:(.*?)\">(((\n|.)*).*?)<\/p>/gi,"[size=$1]$2[/size]");
		
		// [font]
	    html = html.replace(/<span style=\"font-family:(.*?);\">(((\n|.)*).*?)<\/span>/gi,"[font=$1]$2[/font]");
		html = html.replace(/<p style=\"font-family:(.*?);\">(((\n|.)*).*?)<\/p>/gi,"[font=$1]$2[/font]");
		html = html.replace(/<span style=\"font-family:(.*?)\">(((\n|.)*).*?)<\/span>/gi,"[font=$1]$2[/font]");
		html = html.replace(/<p style=\"font-family:(.*?)\">(((\n|.)*).*?)<\/p>/gi,"[font=$1]$2[/font]");
		
		
		// Convert <br> to line breaks.
		html = html.replace( /<br(?=[ \/>]).*?>/gi, '\r\n') ;
		
		// Remove remaining tags.
		html = html.replace(/<p>/gi,"");
		html = html.replace(/<\/p>/gi,"\n");
		html = html.replace(/&nbsp;/gi," ");

		html = html.replace( /<[^>]+>/g, '') ;

		return html;
	}
};