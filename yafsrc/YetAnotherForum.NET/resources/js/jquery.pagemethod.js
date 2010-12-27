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
