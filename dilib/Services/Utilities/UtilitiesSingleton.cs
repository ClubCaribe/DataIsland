using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Web;
using System.Globalization;
using System.Data;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Threading.Tasks;

namespace dataislandcommon.Services.Utilities
{
    public class UtilitiesSingleton : dataislandcommon.Services.Utilities.IUtilitiesSingleton
	{

		public string GetProperContentType(string ext, bool ispreview = false)
		{
			if ((ext != "png") && (ext != "jpg") && (ext != "tif") && (ext != "gif") && ispreview)
			{
				return "image/png";
			}
			switch (ext.ToLower())
			{
				case "ez ": return "application/andrew-inset";
				case "hqx": return "application/mac-binhex40";
				case "cpt": return "application/mac-compactpro";
				case "mathml": return "application/mathml+xml";
				case "doc": return "application/msword";
				case "lha": return "application/octet-stream";
				case "lzh": return "application/octet-stream";
				case "oda": return "application/oda";
				case "ogg": return "application/ogg";
				case "pdf": return "application/pdf";
				case "ai": return "application/postscript";
				case "eps": return "application/postscript";
				case "ps": return "application/postscript";
				case "rdf": return "application/rdf+xml";
				case "smi": return "application/smil";
				case "smil": return "application/smil";
				case "gram": return "application/srgs";
				case "grxml": return "application/srgs+xml";
				case "mif": return "application/vnd.mif";
				case "xul": return "application/vnd.mozilla.xul+xml";
				case "xls": return "application/vnd.ms-excel";
				case "ppt": return "application/vnd.ms-powerpoint";
				case "wbxml": return "application/vnd.wap.wbxml";
				case ".wmlc": return "application/vnd.wap.wmlc";
				case "wmlc": return "application/vnd.wap.wmlc";
				case "wmlsc": return "application/vnd.wap.wmlscriptc";
				case ".wmlsc": return "application/vnd.wap.wmlscriptc";
				case "vxml": return "application/voicexml+xml";
				case "bcpio": return "application/x-bcpio";
				case "vcd": return "application/x-cdlink";
				case "pgn": return "application/x-chess-pgn";
				case "cpio": return "application/x-cpio";
				case "csh": return "application/x-csh";
				case "dcr": return "application/x-director";
				case "dir": return "application/x-director";
				case "dxr": return "application/x-director";
				case "dvi": return "application/x-dvi";
				case "spl": return "application/x-futuresplash";
				case "gtar": return "application/x-gtar";
				case "hdf": return "application/x-hdf";
				case "skp": return "application/x-koan";
				case "skd": return "application/x-koan";
				case "skt": return "application/x-koan";
				case "skm": return "application/x-koan";
				case "latex": return "application/x-latex";
				case "nc": return "application/x-netcdf";
				case "cdf": return "application/x-netcdf";
				case ".crl": return "application/x-pkcs7-crl";
				case "sh": return "application/x-sh";
				case "shar": return "application/x-shar";
				case "swf": return "application/x-shockwave-flash";
				case "sit": return "application/x-stuffit";
				case "sv4cpio": return "application/x-sv4cpio";
				case "sv4crc": return "application/x-sv4crc";
				case "tar": return "application/x-tar";
				case "tcl": return "application/x-tcl";
				case "tex": return "application/x-tex";
				case "texinfo": return "application/x-texinfo";
				case "texi": return "application/x-texinfo";
				case "t": return "application/x-troff";
				case "tr": return "application/x-troff";
				case "roff": return "application/x-troff";
				case "man": return "application/x-troff-man";
				case "me": return "application/x-troff-me";
				case "ms": return "application/x-troff-ms";
				case "ustar": return "application/x-ustar";
				case "src": return "application/x-wais-source";
				case ".crt": return "application/x-x509-ca-cert";
				case "xht": return "application/xhtml+xml";
				case "xsl": return "application/xml";
				case "xml": return "application/xml";
				case "dtd": return "application/xml-dtd";
				case "xslt": return "application/xslt+xml";
				case "zip": return "application/zip";
				case "snd": return "audio/basic";
				case "au": return "audio/basic";
				case "mid": return "audio/midi";
				case "midi": return "audio/midi";
				case "kar": return "audio/midi";
				case "mpga": return "audio/mpeg";
				case "mp2": return "audio/mpeg";
				case "mp3": return "audio/mpeg";
				case "aif": return "audio/x-aiff";
				case "aiff": return "audio/x-aiff";
				case "aifc": return "audio/x-aiff";
				case "m3u": return "audio/x-mpegurl";
				case "rm": return "audio/x-pn-realaudio";
				case "ram": return "audio/x-pn-realaudio";
				case "rpm": return "audio/x-pn-realaudio-plugin";
				case "ra": return "audio/x-realaudio";
				case "wav": return "audio/x-wav";
				case "pdb": return "chemical/x-pdb";
				case "xyz": return "chemical/x-xyz";
				case "bmp": return "image/bmp";
				case "cgm": return "image/cgm";
				case "gif": return "image/gif";
				case "ief": return "image/ief";
				case "jpeg": return "image/jpeg";
				case "jpg": return "image/jpeg";
				case "jpe": return "image/jpeg";
				case "png": return "image/png";
				case "svg": return "image/svg+xml";
				case "tif": return "image/tiff";
				case "tiff": return "image/tiff";
				case "djv": return "image/vnd.djvu";
				case "djvu": return "image/vnd.djvu";
				case "wbmp": return "image/vnd.wap.wbmp";
				case ".wbmp": return "image/vnd.wap.wbmp";
				case "ras": return "image/x-cmu-raster";
				case "ico": return "image/x-icon";
				case "pnm": return "image/x-portable-anymap";
				case "pbm": return "image/x-portable-bitmap";
				case "pgm": return "image/x-portable-graymap";
				case "ppm": return "image/x-portable-pixmap";
				case "rgb": return "image/x-rgb";
				case "xbm": return "image/x-xbitmap";
				case "xpm": return "image/x-xpixmap";
				case "xwd": return "image/x-xwindowdump";
				case "iges": return "model/iges";
				case "igs": return "model/iges";
				case "msh": return "model/mesh";
				case "mesh": return "model/mesh";
				case "silo": return "model/mesh";
				case "vrml": return "model/vrml";
				case "wrl": return "model/vrml";
				case "ifb": return "text/calendar";
				case "ics": return "text/calendar";
				case "css": return "text/css";
				case "js": return "text/javascript";
				case "asc": return "text/plain";
				case "txt": return "text/plain";
				case "rtx": return "text/richtext";
				case "rtf": return "text/rtf";
				case "sgm": return "text/sgml";
				case "sgml": return "text/sgml";
				case "tsv": return "text/tab-separated-values";
				case "wml": return "text/vnd.wap.wml";
				case ".wml": return "text/vnd.wap.wml";
				case "wmls": return "text/vnd.wap.wmlscript";
				case ".wmls": return "text/vnd.wap.wmlscript";
				case "etx": return "text/x-setext";
				case "mpeg": return "video/mpeg";
				case "mpg": return "video/mpeg";
				case "mpe": return "video/mpeg";
				case "mov": return "video/quicktime";
				case "qt": return "video/quicktime";
				case "mxu": return "video/vnd.mpegurl";
				case "avi": return "video/x-msvideo";
				case "movie": return "video/x-sgi-movie";
				case "flv": return "video/x-flv";
				default: return "image/png";
			}
		}

		public string HtmlDecode(string text)
		{
			return text.ToString().Replace("\"", "&quot;").Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
		}

		public string HtmlEncode(string text)
		{
			return text.ToString().Replace("&amp;", "&").Replace("&quot;", "\"").Replace("&lt;", "<").Replace("&gt;", ">");
		}

		public NameValueCollection ParseStyleLikeDocument(string styletxt)
		{
			try
			{
				NameValueCollection coll = new NameValueCollection();
				string[] args = styletxt.Split(new char[] { ':', ';' }, StringSplitOptions.RemoveEmptyEntries);
				for (int i = 0; i < args.Length; i++)
				{
					coll.Add(args[i].Trim(), args[i + 1].Trim());
					i++;
				}
				return coll;
			}
			catch
			{
			}
			return new NameValueCollection();
		}

        public object ParseObjectFromString(string value)
        {
            bool bValue;
            if (bool.TryParse(value,out bValue))
            {
                return bValue;
            }

            double dValue;
            if (double.TryParse(value, out dValue))
            {
                return dValue;   
            }

            int iValue;
            if(int.TryParse(value,out iValue))
            {
                return iValue;
            }


            return value;
        }

        public string EscapeUserId(string userId)
        {
            string escaped = userId.Replace("/", "(2F)").Replace("+","(plus)");
            return escaped;
        }

        public string UnescapeUserId(string userId)
        {
            string unescaped = userId.Replace("(2F)","/").Replace("(plus)","+");
            return unescaped;
        }
	}
}
