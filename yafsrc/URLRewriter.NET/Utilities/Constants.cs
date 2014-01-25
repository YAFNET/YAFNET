// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2007 Intelligencia
// Copyright 2007 Seth Yates
// 

using System;

namespace Intelligencia.UrlRewriter.Utilities
{
	/// <summary>
	/// Constants for the parser.
	/// </summary>
	internal sealed class Constants
	{
		private Constants()
		{
		}

		public const string Messages = "Intelligencia.UrlRewriter.Messages";
		public const string RewriterNode = "rewriter";
		public const string RemoteAddressHeader = "REMOTE_ADDR";
		public const string AttributeAction = "{0}-{1}";
		public const string HeaderXPoweredBy = "X-Powered-By";

		public const string AttrID = "id";
		public const string AttrAction = "action";
		public const string AttrExists = "exists";
		public const string AttrFile = "file";
		public const string AttrAddress = "address";
		public const string AttrHeader = "header";
		public const string AttrMethod = "method";
		public const string AttrMatch = "match";
		public const string AttrValue = "value";
		public const string AttrProperty = "property";
		public const string AttrStatus = "status";
		public const string AttrCookie = "cookie";
		public const string AttrRewrite = "rewrite";
		public const string AttrRedirect = "redirect";
		public const string AttrProcessing = "processing";
		public const string AttrPermanent = "permanent";
		public const string AttrValueContinue = "continue";
		public const string AttrValueRestart = "restart";
		public const string AttrValueStop = "stop";
		public const string AttrFrom = "from";
		public const string AttrName = "name";
		public const string AttrTo = "to";
		public const string AttrType = "type";
		public const string AttrCode = "code";
		public const string AttrUrl = "url";
		public const string AttrParser = "parser";
		public const string AttrTransform = "transform";
		public const string AttrLogger = "logger";

		public const string ElementIf = "if";
		public const string ElementUnless = "unless";
		public const string ElementAnd = "and";
		public const string ElementAdd = "add";
		public const string ElementSet = "set";
		public const string ElementErrorHandler = "error-handler";
		public const string ElementForbidden = "forbidden";
		public const string ElementNotImplemented = "not-implemented";
		public const string ElementNotAllowed = "not-allowed";
		public const string ElementGone = "gone";
		public const string ElementNotFound = "not-found";
		public const string ElementRewrite = "rewrite";
		public const string ElementRedirect = "redirect";
		public const string ElementMap = "map";
		public const string ElementMapping = "mapping";
		public const string ElementRegister = "register";
		public const string ElementDefaultDocuments = "default-documents";
		public const string ElementDocument = "document";

		public const string TransformDecode = "decode";
		public const string TransformEncode = "encode";
		public const string TransformBase64 = "base64";
		public const string TransformBase64Decode = "base64decode";
		public const string TransformLower = "lower";
		public const string TransformUpper = "upper";
	}
}
