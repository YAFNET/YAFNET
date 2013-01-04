         var get =
new function(){

	var mergeObjs = function(obj1, obj2){

		for (var k in obj1)
			obj2[k] = obj1[k];

		return obj2;
	};
	var removeWhitespaces = function (s){
		return s.replace(new RegExp("^ +| +$"),"");
	};


	var addEvent = function ( el ,sEventName, fTodo ) {
		if (el.addEventListener) {
			el.addEventListener (sEventName,fTodo,false);

		} else if (el.attachEvent) {
			el.attachEvent ("on"+sEventName,fTodo);

		} else {
			el["on"+sEventName] = fTodo;
		}
	};
	var getElementsByClassName = function (node,classname ,strTag) {
		strTag = strTag || "*";
	  	node = node || document;
		if (node.getElementsByClassName)
			return node.getElementsByClassName(classname);
		else {
			var objColl = node.getElementsByTagName(strTag);
			if (!objColl.length &&  strTag == "*" &&  node.all) objColl = node.all;
			var arr = new Array();
			var delim = classname.indexOf('|') != -1  ? '|' : ' ';
			var arrClass = classname.split(delim);
			for (var i = 0, j = objColl.length; i < j; i++) {
				var arrObjClass = objColl[i].className.split(' ');
				if (delim == ' ' && arrClass.length > arrObjClass.length) continue;
				var c = 0;
				comparisonLoop:
				for (var k = 0, l = arrObjClass.length; k < l; k++) {
				  for (var m = 0, n = arrClass.length; m < n; m++) {
				    if (arrClass[m] == arrObjClass[k]) c++;
				    if (( delim == '|' && c == 1) || (delim == ' ' && c == arrClass.length)) {
				      arr.push(objColl[i]);
				      break comparisonLoop;
				    }
				  }
				}
			}
			return arr;
		}
	};

	var hasClassName = function ( sClassName, elem ) {
		//.split(/\s+/);
		var aCnames = elem.className.split(/\s+/) || [];
		for (var i=0, l=aCnames.length; i<l ; i++){
			if (sClassName == aCnames[i])
				return true;
		}
		return false;
	};

	var single = {
		addClass 	: function ( sClassName ) {
			//console.info( sClassName, this.className, );
			if ( hasClassName(sClassName , this) )
				return this;
			this.className = removeWhitespaces(this.className + " " +sClassName);

			return this;

		},
		removeClass : function ( sClassName ) {
			this.className = removeWhitespaces(this.className.replace(sClassName,""));

			return this;
		},
		setStyle	: function ( oStyles ) {
            for (var style in oStyles)
                this.style[style] = oStyles[style];
			return this;
		},
		bindOnclick		: function ( handler ) {
			//addEvent( this, "click" , handler);
			this.onclick = handler;
			return this;
		},
		bindOnchange	: function ( handler ) {
			//addEvent( this, "change" , handler);
			this.onchange = handler;
			return this;
		},
		getAttr: function ( sAttrName ){
			if ( !sAttrName ) return null;

			return this[sAttrName];
		},
		setAttr: function ( sAttrName , attrVal ){
			if ( !sAttrName || !attrVal ) return null;
			this[sAttrName] = attrVal;
			return this;

		},
		remAttr: function ( sAttrName ){
			if ( !sAttrName ) return null;

		}
	};

	var singleCaller = function ( sMethod,args ) {
		for ( var i=0, l=this.length; i<l ; i++ ){
			var oItem = mergeObjs( single, this[i] );
			oItem[sMethod].apply(this[i],args);
		}
	};


	var collection = {

		addClass 	: function ( sClassName ){
			singleCaller.call(this, "addClass", [sClassName]);
			return this;
		},
		removeClass 	: function ( sClassName ) {
			singleCaller.call(this, "removeClass", [sClassName]);
			return this;
		},
		setStyle		: function ( oStyles ) {
			singleCaller.call(this, "setStyle", [oStyles]);
			return this;
		},
		bindOnclick		: function ( f ) {
			singleCaller.call(this, "bindOnclick", [f]);
			return this;
		},
		bindOnchange	: function ( f ) {
			singleCaller.call(this, "bindOnchange", [f]);
			return this;
		},

		forEach : function ( fTodo ) {
			//el,i
			for (var i=0, l=this.length; i<l ; i++){
				fTodo.apply(this[i], [this[i],i ]);
			}
			return this;
		}

	};



	this.byClass = function( sClassName ){
		var o = getElementsByClassName(document, sClassName );
		return o ? mergeObjs( collection, o ) : o;
	};

	this.byId = function( sId ){
		var o = document.getElementById( sId );
		return o ? mergeObjs( single, o ) : o;
	};

	this.gup = function ( name ){
        name = name.replace( /[\[]/, '\\\[' ).replace( /[\]]/, '\\\]' ) ;
        var regexS = '[\\?&]' + name + '=([^&#]*)' ;
        var regex = new RegExp( regexS ) ;
        var results = regex.exec( window.location.href ) ;

        if( results == null )
            return '' ;
        else
            return results[ 1 ] ;
	};
	this.wrap = function ( o ) {
		return o ? mergeObjs( single, o ) : o;
	};
	this.forEach = function ( oScope, fTodo ){
		collection.forEach.apply( oScope,[fTodo] );
	};

 };
            
            var _s = tinyMCEPopup.getWindowArg('s');
            var scayt = _s._SCAYT;
            var scayt_control = tinyMCEPopup.getWin().tinyMCE.activeEditor.plugins.scayt._s._SCAYT_control;// _s._SCAYT_control;
            var userDicActive = false;
            var plugin = tinyMCEPopup.getWin().tinyMCE.activeEditor.plugins.scayt;
            var uiTabs = plugin.uiTabs;
            
            function Ok() {
				// set upp options if any was set
                var c = false;
				//console.info(options)
                var o = scayt_control.option();
                //setup languge if it was change
                var csLang = chosed_lang.split("::")[1];
				
                for ( var oN in options ) {
                    if ( o[oN] != options[oN] && c != true){
                        //scayt_control.option( options );
                        c = true;
                    }
                }
				
				if (c) {
					scayt_control.option( options );
				}
				
				if ( csLang && sLang != csLang ) {
					scayt_control.setLang( csLang );
					//console.info(sLang+" -> "+csLang , scayt_control);
				}
				//else {
					//scayt_control.refresh();
				//}
                
                return tinyMCEPopup.close();
            }
            
            function Cancel () {
                return tinyMCEPopup.close();
            };
            
            
            
            var buttons = [ 'dic_create','dic_delete','dic_rename','dic_restore' ],
                labels  = [ 'mixedCase','mixedWithDigits','allCaps','ignoreDomainNames' ];
                
                
            function apllyCaptions ( ) {
                
                // fill tabs headers
                // add missing captions
                
                get.byClass("PopupTab").forEach(function(el,i){
                    
                    
                    if ( tabs[i] == 1 ){
                        el.style.display = "block";
                    }
                    el.innerHTML = "<span>" + captions['tab_'+el.id] + "</span>";
                    
                });
                    
                // Fill options labels.
                for ( i in labels )
                {
                    var label = 'label_' + labels[ i ],
                        labelElement = document.getElementById( label );
                    
                    if (  'undefined' != typeof labelElement
                        && 'undefined' != typeof captions[ label ] && captions[ label ] !== ""
                        && 'undefined' != typeof options[labels[ i ]] )
                    {
                        labelElement.innerHTML = captions[ label ];
                        var labelParent = labelElement.parentNode;
                        labelParent.style.display = "block";
                    }
                }
                // fill dictionary section
                for ( var i in buttons )
                {
                    var button = buttons[ i ];
                    //get.byId( button ).innerHTML = '<span>' + captions[ 'button_' + button]  +'</span>' ;
                    get.byId( button ).value = captions[ 'button_' + button];
                }
                get.byId("dname").innerHTML = captions['label_dname'];
                get.byId( 'dic_info' ).innerHTML = captions[ 'dic_info' ];
                    
                // fill about tab
                var about = '<p><img src="' + window.scayt.getAboutInfo().logoURL + '" /></p>'+
                    '<p>' + captions[ 'version' ]  + scayt.version.toString() + '</p>' +
                    '<p>' + captions[ 'about_throwt_copy' ] + '</p>';

                get.byId( 'scayt_about' ).innerHTML = about;	
                
            }
            
            
        
            var lang_list = {},
                sLang,
                fckLang,
                chosed_lang,
                options,
                //tabs = [1,1,1,1],
                captions,
                getCaptionRun = false;
            
            //onload runner
            window.onload = function(){
                
                    //scayt = oEditor.scayt;
                    //scayt_control = oEditor.scayt_control;
                    
                    if (!scayt) throw "SCAYT is undefined";
                    if (!scayt_control) throw "SCAYT_CONTROL is undefined";
                    
                    // show alowed tabs
                    tabs = uiTabs;
                    
                    if (tabs.length > 0 && typeof tabs[2] != 'undefined' && tabs[2] == 1)
                        userDicActive = true;
                    
                    sLang = scayt_control.getLang();
                    fckLang = "en";
                    options = scayt_control.option();
                    // apply captions
                    scayt.getCaption( fckLang, function( caps )
                    {
                        
                        if (getCaptionRun == true) return;
                        //console.info("get caption")
                        getCaptionRun = true;
                        //console.info( "scayt.getCaption runned" )
                        captions = caps;
                        apllyCaptions();
                        //lang_list = scayt.getLangList();
                        lang_list = scayt.getLangList() ;//|| {ltr: {"en_US" : "English","en_GB" : "British English","pt_BR" : "Brazilian Portuguese","da_DK" : "Danish","nl_NL" : "Dutch","en_CA" : "English Canadian","fi_FI" : "Finnish","fr_FR" : "French","fr_CA" : "French Canadian","de_DE" : "German","el_GR" : "Greek","hu_HU" : "Hungarian","it_IT" : "Italian","nb_NO" : "Norwegian","pl_PL" : "Polish","pt_PT" : "Portuguese","ru_RU" : "Russian","es_ES" : "Spanish","sv_SE" : "Swedish","tr_TR" : "Turkish","uk_UA" : "Ukrainian","cy_GB" : "Welsh"},rtl: {"ar_EG" : "Arabic"}};
                        
                        
                        var active_tab = get.gup('ui') || "options";
                        var class_selected_tab = "PopupTabSelected current";
                        var class_usual_tab = "PopupTab";
                        
                        // * DHTML tabs
                        // ** select active tab 
                        get.byId(active_tab).addClass( class_selected_tab ).removeClass( class_usual_tab );
                        get.byId("inner_"+active_tab).setStyle({display:"block"});
                        // ** make all tabs clickable
                        get.byClass("TabfixedClass").bindOnclick(function(){
                            
                            if (this.className.indexOf(class_selected_tab) != -1) return false;
                            var subClass = this.id;
                            var contenttabId = 'inner_' + subClass;
                            
                            get.byClass("tab_container").setStyle({display:"none"});
                            get.byId(contenttabId).setStyle({display:"block"});
                            get.byClass("TabfixedClass").addClass( class_usual_tab ).removeClass( class_selected_tab );
                            get.wrap(this).addClass( class_selected_tab );
                            
                        });     
                        
                        // ** animate options
                        get.byClass("_scayt_option").forEach(function(el,i){
                            
                            if ('undefined' != typeof(options[el.name])) {
                                // *** set default values
                                
                                if ( 1 == options[ el.name ] ){
                                    
                                    get.wrap(el).setAttr("checked" ,true)
                                    
                                }
                                //console.info(options)
                                // *** bind events
                                get.wrap(el).bindOnclick( function(ev){
                                    
                                    var that = get.wrap(this);
                                    var isCheck = that.getAttr("checked");
                                    //console.info(isCheck)									
                                    if ( isCheck == false ) {
                                        
                                        //that.setAttr("checked",false);
                                        options[ this.name ] = 0;
                                    }else{
                                        //that.setAttr("checked",true);
                                        options[ this.name ] = 1;
                                    }
                                    //console.info(options)
                                });
                            }
                        });
                        
                        
                        // * Create languages tab
                        // ** convert langs obj to array
                        var lang_arr = [];
                        
                        for (var k in lang_list.rtl){
                            // find curent lang
                            if ( k == sLang)
                                chosed_lang = lang_list.rtl[k] + "::" + k;
                            lang_arr[lang_arr.length] = lang_list.rtl[k] + "::" + k;
                                
                        }
                        for (var k in lang_list.ltr){
                            // find curent lang
                            if ( k == sLang)
                                chosed_lang = lang_list.ltr[k] + "::" + k;
                            lang_arr[lang_arr.length] = lang_list.ltr[k] + "::" + k;
                        }
                        lang_arr.sort();
                        
                        // ** find lang containers
                        
                        var lcol = get.byId("lcolid");
                        var rcol = get.byId("rcolid");
                        // ** place langs in DOM
                        
                        get.forEach(lang_arr , function( l , i ){
                            
                            //console.info( l,i );
                            
                            var l_arr = l.split('::');
                            var l_name = l_arr[0];
                            var l_code = l_arr[1];
                            var row = document.createElement('div');
                            row.id = l_code;
                            row.className = "li";
                            // split langs on half
                            var col = ( i < lang_arr.length/2 ) ? lcol:rcol ;
                            
                            // append row
                            //console.dir( col )
                            col.appendChild(row);
                            var row_dom = get.byId( l_code )
                            row_dom.innerHTML = l_name;
                                                    
                            var checkActiveLang = function( id ){
                                return chosed_lang.split("::")[1] == id;
                            };
                            // bind click
                            row_dom.bindOnclick(function(ev){
                                
                                if ( checkActiveLang(this.id) ) return false;
                                var elId = this.id;
                                get.byId(this.id)
                                    .addClass("Button")
                                    .removeClass("DarkBackground");
                                    
                                window.setTimeout( function (){ 
                                        get.byId(elId).setStyle({opacity:"0.5",cursor:"no-drop"});  
                                    } 
                                ,300 );
                                
                                get.byId(chosed_lang.split("::")[1])
                                    .addClass("DarkBackground")
                                    .removeClass("Button")
                                    .setStyle({opacity:"1",cursor:"pointer"});
                                    
                                chosed_lang = this.innerHTML + "::" + this.id;
                                return true;
                            })
                            .setStyle({
                                cursor:"pointer"
                            });
                            // select current lang
                            if (l == chosed_lang)
                                row_dom.addClass("Button").setStyle({opacity:"0.5",cursor:"no-drop"});
                            else 
                                row_dom.addClass("DarkBackground").setStyle({opacity:"1"});
                                
                        });
                        
                        // * user dictionary    
                        
                        // ** customize buttons
                        var dic_name ="mydictionary";
                        var dic_buttons = [
                            // [0] contains buttons for creating
                            "dic_create,dic_restore",
                            // [1] contains buton for manipulation 
                            "dic_rename,dic_delete"
                        ];
                        
                        if ( userDicActive ){
                            
                            scayt.getNameUserDictionary(
                                function( o )
                                {
                                    var dic_name = o.dname;
                                    if ( dic_name )
                                    {
                                        get.byId( 'dic_name' ).value = dic_name;
                                        display_dic_buttons( dic_buttons[1] );
                                    }
                                    else
                                        display_dic_buttons( dic_buttons[0] );
                                    display_dic_tab();
                    
                                },
                                function ()
                                {
                                    get.byId( 'dic_name' ).value ="";
                                    dic_error_message(captions["err_dic_enable"] || "Used dictionary are unaveilable now.")
                                });
                            dic_success_message("");
                        }
                        
                        
                        // ** bind event listeners
                        get.byClass("button").bindOnclick(function( ){
                            
                            if ( userDicActive && typeof window[this.id] == 'function'  ){
                                // get dic name
                                var dic_name = get.byId('dic_name').value ;
                                // check common dictionary rules
                                if (!dic_name) {
                                    dic_error_message(" Dictionary name should not be empty. ");
                                    return false;
                                }
                                //apply handler
                                window[this.id].apply( window, [this, dic_name, dic_buttons ] );
                            }
                                
                            //console.info( typeof window[this.id], window[this.id].calle )
                            return false;
                        });
                        
                    });
                    
            };
            
            window.dic_create = function( el, dic_name , dic_buttons )
            {
                // comma separated button's ids include repeats if exists
                var all_buttons = dic_buttons[0] + ',' + dic_buttons[1];

                var err_massage = captions["err_dic_create"];
                var suc_massage = captions["succ_dic_create"];
                //console.info("--plugin ");
                dic_empty_message();
                scayt.createUserDictionary(dic_name,
                    function(arg)
                        {
                            //console.info( "dic_create callback called with args" , arg );
                            hide_dic_buttons ( all_buttons );
                            display_dic_buttons ( dic_buttons[1] );
                            suc_massage = suc_massage.replace("%s" , arg.dname );
                            dic_success_message (suc_massage);
                        },
                    function(arg)
                        {
                            //console.info( "dic_create errorback called with args" , arg )
                            err_massage = err_massage.replace("%s" ,arg.dname );
                            dic_error_message ( err_massage + "( "+ (arg.message || "") +")");
                        });

            };
            
            
            window.dic_rename = function( el, dic_name , dic_buttons )
            {
                //
                // try to rename dictionary
                var err_massage = captions["err_dic_rename"] || "";
                var suc_massage = captions["succ_dic_rename"] || "";
                
                dic_empty_message();
                scayt.renameUserDictionary(dic_name,
                    function(arg)
                        {
                            //console.info( "dic_rename callback called with args" , arg );
                            suc_massage = suc_massage.replace("%s" , arg.dname );
                            set_dic_name( dic_name );
                            dic_success_message ( suc_massage );
                        },
                    function(arg)
                        {
                            //console.info( "dic_rename errorback called with args" , arg )
                            err_massage = err_massage.replace("%s" , arg.dname  );
                            set_dic_name( dic_name );
                            dic_error_message( err_massage + "( " + ( arg.message || "" ) + " )" );
                        });
            };

            window.dic_delete = function ( el, dic_name , dic_buttons )
            {
                var all_buttons = dic_buttons[0] + ',' + dic_buttons[1];
                var err_massage = captions["err_dic_delete"];
                var suc_massage = captions["succ_dic_delete"];
                
                dic_empty_message();
                scayt.deleteUserDictionary(
                    function(arg)
                        {
                            //console.info( "dic_delete callback " , dic_name ,arg );
                            suc_massage = suc_massage.replace("%s" , arg.dname );
                            hide_dic_buttons ( all_buttons );
                            display_dic_buttons ( dic_buttons[0] );
                            set_dic_name( "" ); // empty input field
                            dic_success_message( suc_massage );
                        },
                    function(arg)
                        {
                            //console.info( " dic_delete errorback called with args" , arg )
                            err_massage = err_massage.replace("%s" , arg.dname );
                            dic_error_message(err_massage);
                        });
            };

            window.dic_restore =  function ( el, dic_name , dic_buttons )
            {
                // try to restore existing dictionary
                var all_buttons = dic_buttons[0] + ',' + dic_buttons[1];
                var err_massage = captions["err_dic_restore"];
                var suc_massage = captions["succ_dic_restore"];
                
                dic_empty_message();
                scayt.restoreUserDictionary(dic_name,
                    function(arg)
                        {
                            //console.info( "dic_restore callback called with args" , arg );
                            suc_massage = suc_massage.replace("%s" , arg.dname );
                            hide_dic_buttons ( all_buttons );
                            display_dic_buttons(dic_buttons[1]);
                            dic_success_message( suc_massage );
                        },
                    function(arg)
                        {
                            //console.info( " dic_restore errorback called with args" , arg )
                            err_massage = err_massage.replace("%s" , arg.dname );
                            dic_error_message( err_massage );
                        });
            };

            function dic_error_message ( m ){
                if (m)
                    get.byId('dic_message').innerHTML =  '<span class="error">' + m + '</span>' ;
                return "";

            }
            function dic_success_message ( m ){
                if (m)
                    get.byId('dic_message').innerHTML = '<span class="success">' + m + '</span>' ;
                return "";
            }
            function dic_empty_message (){
                return get.byId('dic_message').innerHTML = ' ' ;
            }
            function display_dic_buttons ( sIds ){
                sIds = new String( sIds );

                get.forEach( sIds.split(','), function ( id,i) {
                    
                    get.byId(id ).setStyle({display:"inline"});
                })
            }
            function hide_dic_buttons ( sIds ){
                sIds = new String( sIds );
                get.forEach( sIds.split(','), function ( id,i) {
                    get.byId(id ).setStyle({display:"none"});
                })
            }
            function set_dic_name ( dic_name ) {
                get.byId( 'dic_name' ).value = dic_name;
            }
            function display_dic_tab () {
                get.byId( 'dic_tab' ).setStyle({display:"block"});
            }