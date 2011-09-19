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