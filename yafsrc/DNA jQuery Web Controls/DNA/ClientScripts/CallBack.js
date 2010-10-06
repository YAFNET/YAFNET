/// <reference name="MicrosoftAjax.js"/>
///  
/// Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html
///  
Type.registerNamespace("DNA.UI");

DNA.UI.CallBack = function() {
    DNA.UI.CallBack.initializeBase(this);
    this._targetId = null;
    this._async = true;
    this._updateViewState = true;
    this._callbackDelegates = [];
}

DNA.UI.CallBack.prototype = {
    initialize: function() {
        DNA.UI.CallBack.callBaseMethod(this, 'initialize');
    },
    registerMethod: function(methodName, returnType, successfulDelegate, errorDelegate) {
        Array.add(this._callbackDelegates,
         {
             "method": methodName,
             "returnType": returnType,
             "onsuccess": successfulDelegate,
             "onerror": errorDelegate
         });
        this[methodName] = function() {
            var args = [];
            for (var i = 0; i < arguments.length; i++)
                Array.add(args, arguments[i]);
            this._invokeServerCall({ "method": methodName, "params": args });
        }
    },
    _invokeServerCall: function(_serverArgs) {
        if (String.isNullOrEmpty(this.get_targetId()))
            throw "targetId must not be null!";
        var _registedMethod = null;
        for (var i = 0; i < this._callbackDelegates.length; i++) {
            if (this._callbackDelegates[i].method == _serverArgs.method) {
                _registedMethod = this._callbackDelegates[i];
                break;
            }
        }
        if (_registedMethod == null)
            throw "No server method found!";

        uniqueID = this.get_targetId().replace(/\_/g, '$');
        jsonStr = "";
        if (_serverArgs != null)
            jsonStr = Sys.Serialization.JavaScriptSerializer.serialize(_serverArgs);
        __theFormPostData = ''; //reinit webform callback
        WebForm_InitCallback();
        _registedMethod.updateViewState = this.get_updateViewState();
        WebForm_DoCallback(uniqueID, jsonStr, this._callbackSuccessfulHandler, _registedMethod, this._callbackErrorHandler, this._anync);
    },
    _callbackErrorHandler: function(response, context) {
        context.onerror(response);
    },
    _callbackSuccessfulHandler: function(response, context) {
        data = response.split(";");
        if (context.updateViewState)
            $get("__VIEWSTATE").value = data[0];
        var _result = null;

        if (context.returnType == "json") {
            if (!String.isNullOrEmpty(data[1]))
                _result = Sys.Serialization.JavaScriptSerializer.deserialize(data[1]);
        }
        else
            _result == data[1];

        context.onsuccess(_result);
    },
    makeServerCall: function(_method, _params) {
        if ((_method == "") || (_method == undefined) || (_method == null))
            throw "Server Method could be null";
        if ((_params != undefined) && (_params != null))
            this._invokeServerCall({ "method": _method, "params": Sys.Serialization.JavaScriptSerializer.serialize(_params) });
        else
            this._invokeServerCall({ "method": _method });
    },
    get_targetId: function() { return this._targetId; },
    set_targetId: function(value) { this._targetId = value; },
    get_async: function() { return this._async; },
    set_async: function(value) { this._async = value; },
    get_updateViewState: function() { return this._updateViewState; },
    set_updateViewState: function(value) { this._updateViewState = value; },
    dispose: function() {
        DNA.UI.CallBack.callBaseMethod(this, 'dispose');
    }
}

DNA.UI.CallBack.registerClass('DNA.UI.CallBack', Sys.Component);

if (typeof (Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();
