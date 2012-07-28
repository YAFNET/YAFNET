<%

Dim ByteCount, BinRead
ByteCount = Request.TotalBytes
BinRead = Request.BinaryRead(ByteCount)
rawData = RSBinaryToString(BinRead)

'get language
if instr(rawData, """params"":[""") > 1 then
    lang = mid(rawData, InStr(rawData,"[")+2, 2)
else
    lang = "en"
end if

if instr(rawData, """method"":""checkWords"",") > 1 then
'return mispelled words
 json = mid(rawData, InStrRev(rawData,"["))
 json = mid(json, 1, instr(json, "]"))
 json = replace(json, """,""", " ")
 json = replace(json, """", "")
 t = json

    r = "<?xml version=""1.0"" encoding=""utf-8"" ?><spellrequest textalreadyclipped=""0"" ignoredups=""0"" ignoredigits=""1"" ignoreallcaps=""1""><text>"_
        &t&"</text></spellrequest>"

    r = getURL("https://www.google.com/tbproxy/spell?lang="&lang, r, "","")
    out = "{""id"":null,""result"":["
    wrds = ""
    for each c in filter(split(r,"<c "),"</c>")
        'response.write "<br>"&server.htmlencode(c)
        o = cint(split(split(c,"o=",2)(1),"""")(1))+1
        l = cint(split(split(c,"l=",2)(1),"""")(1))
        s = cint(split(split(c,"s=",2)(1),"""")(1))
        out = out & """" & mid(t,o,l)& """, " 
        wrds = "1"
    next
    if wrds = "" then
        out = "{""id"":null,""result"":[],""error"":null}"
    else
        out = mid(out, 1, len(out)-2) & "],""error"":null}"
    end if
    
    response.write out
    response.end

else
 'return single word corrections
 json = mid(rawData, InStrRev(rawData,"["))
 json = mid(json, 1, instr(json, "]"))
 json = replace(json, """,""", " ")
 json = replace(json, "en ", "")
 json = replace(json, """", "")
 t = json
     
    r = "<?xml version=""1.0"" encoding=""utf-8"" ?><spellrequest textalreadyclipped=""0"" ignoredups=""0"" ignoredigits=""1"" ignoreallcaps=""1""><text>"_
        &t&"</text></spellrequest>"

    r = getURL("https://www.google.com/tbproxy/spell?lang="&lang, r, "","")

    for each c in filter(split(r,"<c "),"</c>")
        'response.write "<br>"&server.htmlencode(c)
        o = cint(split(split(c,"o=",2)(1),"""")(1))+1
        l = cint(split(split(c,"l=",2)(1),"""")(1))
        s = cint(split(split(c,"s=",2)(1),"""")(1))
        c = textbetween(">", c, "<")
        '{"id":null,"result":["Titmice","Times","Tines","Tinnies","Timmy\'s"],"error":null}
        out =  "{""id"":null,""result"":["
        wrds = ""
        for each w in split(c,vbTab)
            out = out & """" & w & """, "
            wrds = "1"
        next
        if wrds = "" then
            out = "{""id"":null,""result"":[],""error"":null}"
        else
            out = mid(out, 1, len(out)-2) & "],""error"":null}"
        end if
    next
    response.write out
    response.end
end if

 if t=empty then t = request.form()    'GoogieSpell is going to put the text in the POST data.

'show the reply from google for the POST data.     
 response.write getURL("https://www.google.com/tbproxy/spell?lang="&lang, t, "","")



Function TextBetween(sThis, sAnd, sThat)
    on error resume next
    TextBetween = split(split(sAnd,sThis,2,1)(1),sThat,2,1)(0)
end function

Function RSBinaryToString(xBinary)
  Dim Binary
  If vartype(xBinary)=8 Then Binary = MultiByteToBinary(xBinary) Else Binary = xBinary
  Dim RS, LBinary
  Const adLongVarChar = 201
  Set RS = CreateObject("ADODB.Recordset")
  LBinary = LenB(Binary)

  If LBinary>0 Then
    RS.Fields.Append "mBinary", adLongVarChar, LBinary
    RS.Open
    RS.AddNew
    RS("mBinary").AppendChunk Binary
    RS.Update
    RSBinaryToString = RS("mBinary")
  Else
    RSBinaryToString = ""
  End If
End Function

function getURL(aURL, anyPostData, anyUserName, anyPassword) 
DIM objSrvHTTP,web,method,s
    on error resume next
    s=""
    set objSrvHTTP = Server.CreateObject ("Msxml2.ServerXMLHTTP.3.0")
    if anyPostData=empty then
        objSrvHTTP.open "GET",aURL, true, anyUsername, anyPassword
    else
        objSrvHTTP.open "POST",aURL, true, anyUsername, anyPassword
        objSrvHTTP.setRequestHeader "Content-Type", "application/x-www-form-urlencoded"
    end if
    objSrvHTTP.setRequestHeader "User-Agent", "Mozilla/4.0 (compatible; MSIE 5.5; Windows NT 5.0)"
    objSrvHTTP.send anyPostData
    objSrvHTTP.waitForResponse 7
    select case objSrvHTTP.readyState
        case 0 'object created, but no URL opened
            debug "getURL("&aURL&", "&anyPostData&", "&anyUserName&", "&anyPassword&"): Object Created, no URL opened"
            err.raise 1, "Object Created, no URL opened"
            exit function
        case 1    'loading: URL opened, but no data sent
            debug "getURL("&aURL&", "&anyPostData&", "&anyUserName&", "&anyPassword&"):URL opened, no data sent"
            err.raise 2, "URL opened, no data sent"
            exit function
        case 2    'loaded: data sent, status and headers available, no response recieved.
            debug "getURL("&aURL&", "&anyPostData&", "&anyUserName&", "&anyPassword&"):No response from remote host"
            err.raise 3, "No response from remote host"
            exit function
        case 3    'interactive: some data recieved. responseBody and responseText will return partial results.
            debug "getURL("&aURL&", "&anyPostData&", "&anyUserName&", "&anyPassword&"):Partial response recieved:"
            debug server.htmlencode(objSrvHTTP.responseText)
            s = objSrvHTTP.responseText
            err.raise 4, "Partial response recieved"
        case 4    'complete: 
            s = objSrvHTTP.responseText
        end select
    getURL = s
end function

%>