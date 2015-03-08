using System;
using System.Collections.Specialized;
namespace dataislandcommon.Services.Utilities
{
	public interface IUtilitiesSingleton
	{
		string GetProperContentType(string ext, bool ispreview = false);
		string HtmlDecode(string text);
		string HtmlEncode(string text);
		NameValueCollection ParseStyleLikeDocument(string styletxt);
        object ParseObjectFromString(string value);
        string EscapeUserId(string userId);
        string UnescapeUserId(string userId);
	}
}
