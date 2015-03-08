using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Threading.Tasks;

namespace dataislandcommon
{
    public class TranslationHandler : IHttpModule
    {
        private class ResponseCaptureStream : Stream
        {
            private Stream _streamToCapture;
            private Encoding _responseEncoding;
            HttpContext _context;

            
            MemoryStream _internalStream = null;

            private string _streamContent = "";
            public string StreamContent
            {
                get
                {
                    try
                    {
                        return _streamContent;
                    }
                    catch
                    {
                    }
                    return null;
                }
                private set
                {
                    _streamContent = value;
                }
            }

            public ResponseCaptureStream(Stream streamToCapture, Encoding responseEncoding, HttpContext ctx)
            {
                try
                {
                    _internalStream = new MemoryStream();
                    _responseEncoding = responseEncoding;
                    _streamToCapture = streamToCapture;
                    _context = ctx;
                }
                catch
                {
                }

            }

            public override bool CanRead
            {
                get
                {
                    try
                    {
                        return _streamToCapture.CanRead;
                    }
                    catch
                    {
                    }
                    return false;
                }
            }

            public override bool CanSeek
            {
                get
                {
                    try
                    {
                        return _streamToCapture.CanSeek;
                    }
                    catch
                    {
                    }
                    return false;
                }
            }

            public override bool CanWrite
            {
                get
                {
                    try
                    {
                        return _streamToCapture.CanWrite;
                    }
                    catch
                    {
                    }
                    return false;
                }
            }

            public override void Flush()
            {
                try
                {
                    try
                    {
                        if (canSaveAsText())
                        {

                            if (!string.IsNullOrEmpty(_context.Request.Headers["x-microsoftajax"]))
                            {
                                string toTranslate;
                                toTranslate = _responseEncoding.GetString(_internalStream.ToArray());
                                string endsequence = _context.Request.Url.PathAndQuery;
                                if (endsequence.IndexOf("/") > -1)
                                {
                                    endsequence = endsequence.Substring(endsequence.LastIndexOf("/") + 1);
                                }
                                if (_streamContent.IndexOf(endsequence + "|") > -1)
                                {
                                    List<string> parts = new List<string>();

                                    int delimiterIndex, len;
                                    string type, id, content;
                                    int replyIndex = 0;


                                    while (replyIndex < _streamContent.Length)
                                    {
                                        delimiterIndex = _streamContent.IndexOf('|', replyIndex);

                                        len = int.Parse(_streamContent.Substring(replyIndex, delimiterIndex - replyIndex));
                                        parts.Add(len.ToString());

                                        replyIndex = delimiterIndex + 1;
                                        delimiterIndex = _streamContent.IndexOf('|', replyIndex);

                                        type = _streamContent.Substring(replyIndex, delimiterIndex - replyIndex);
                                        parts.Add(type);
                                        replyIndex = delimiterIndex + 1;
                                        delimiterIndex = _streamContent.IndexOf('|', replyIndex);

                                        id = _streamContent.Substring(replyIndex, delimiterIndex - replyIndex);
                                        parts.Add(id);
                                        replyIndex = delimiterIndex + 1;

                                        content = _streamContent.Substring(replyIndex, len);
                                        parts.Add(content);
                                        replyIndex += len;

                                        replyIndex++;
                                    }

                                    //List<string> parts = _streamContent.Split(new char[] { '|' }).ToList();
                                    for (int i = 0; i < parts.Count; i++)
                                    {
                                        if (parts[i] == "updatePanel")
                                        {
                                            string cnt = parts[i + 2];
                                            int cl = cnt.Length;
                                            cnt = TranslateResource(cnt);
                                            parts[i - 1] = cnt.Count().ToString();
                                            parts[i + 2] = cnt;
                                        }
                                    }
                                    string output = "";
                                    foreach (string str in parts)
                                    {
                                        output += str + "|";
                                    }
                                    //output = output.Substring(0, output.Length - 1);
                                    byte[] toflush = _responseEncoding.GetBytes(output);
                                    _streamToCapture.Write(toflush, 0, toflush.Length);
                                }
                            }
                            else
                            {
                                _streamContent = TranslateResource(_responseEncoding.GetString(_internalStream.ToArray()));

                                byte[] toflush = _responseEncoding.GetBytes(StreamContent);

                                _streamToCapture.Write(toflush, 0, toflush.Length);
                            }
                        }
                        else
                        {
                            _streamToCapture.Write(_internalStream.ToArray(), 0, (int)_internalStream.Length);
                        }
                    }
                    catch
                    {
                        _streamToCapture.Write(_internalStream.ToArray(), 0, (int)_internalStream.Length);
                    }
                    _internalStream.SetLength(0);
                    _internalStream.Dispose();
                    _streamToCapture.Flush();
                }
                catch
                {
                }
            }

            public override long Length
            {
                get
                {
                    try
                    {
                        return _streamToCapture.Length;
                    }
                    catch
                    {
                    }
                    return 0;
                }
            }

            public override long Position
            {
                get
                {
                    try
                    {
                        return _streamToCapture.Position;
                    }
                    catch
                    {
                    }
                    return 0;
                }
                set
                {
                    _streamToCapture.Position = value;
                }
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                try
                {
                    return _streamToCapture.Read(buffer, offset, count);
                }
                catch
                {
                }
                return -1;
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                try
                {
                    return _streamToCapture.Seek(offset, origin);
                }
                catch
                {
                }
                return 0;
            }

            public override void SetLength(long value)
            {
                //_streamToCapture.SetLength(value);
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                _internalStream.Write(buffer, 0, count);
            }

            public string TranslateResource(string input)
            {
                try
                {
                    string _streamContent = input;
                    //_streamContent = TranslationOperations.Translate(_streamContent);
                    _streamContent = _streamContent.Replace("[tr]", "");
                    _streamContent = _streamContent.Replace("[/tr]", "");
                    return _streamContent;
                }
                catch
                {
                }
                return "";
            }

            public override void Close()
            {
                try
                {
                    _streamToCapture.Close();
                    base.Close();
                }
                catch
                {
                }
            }

            public bool canSaveAsText()
            {
                try
                {

                    string ct = _context.Response.ContentType;
                    if (ct.IndexOf("text") > -1)
                    {
                        return true;
                    }
                    if (ct.IndexOf("html") > -1)
                    {
                        return true;
                    }
                    if (ct.IndexOf("javascript") > -1)
                    {
                        return true;
                    }
                    if (ct.IndexOf("json") > -1)
                    {
                        return true;
                    }

                }
                catch
                {
                }
                return false;
            }
        }

        SessionStateModule sessionstate;

        public void Init(HttpApplication app)
        {
            sessionstate = app.Modules["Session"] as SessionStateModule;
            app.PreRequestHandlerExecute += new EventHandler(app_PreRequestHandlerExecute);
            //app.resp
            //app.BeginRequest += app_BeginRequest;

        }

        //void app_BeginRequest(object sender, EventArgs e)
        //{
        //    try
        //    {

        //        HttpApplication application = (HttpApplication)sender;
        //        HttpRequest request = application.Context.Request;
        //        string lang = (request.UserLanguages ?? Enumerable.Empty<string>()).FirstOrDefault();
        //        if (string.IsNullOrEmpty(lang))
        //        {
        //            acc.UILanguage = "default";
        //        }
        //        else
        //        {
        //            if (lang.IndexOf("-") > -1)
        //            {
        //                lang = lang.Substring(0, lang.IndexOf("-"));
        //            }
        //            if (lang.IndexOf(";") > -1)
        //            {
        //                lang = lang.Substring(0, lang.IndexOf(";"));
        //            }
        //            acc.UILanguage = lang;
        //        }
        //    }
        //    catch
        //    {
        //    }
        //}

        void app_PreRequestHandlerExecute(object sender, EventArgs e)
        {

            if ((HttpContext.Current.Request.Url.OriginalString.ToLower().IndexOf(".ogg") < 0) && (HttpContext.Current.Request.Url.OriginalString.ToLower().IndexOf(".mp4") < 0) && (HttpContext.Current.Request.Url.OriginalString.ToLower().IndexOf("scriptresource.axd") < 0) && (HttpContext.Current.Request.Url.OriginalString.ToLower().IndexOf("webresource.axd") < 0))
            {
                ResponseCaptureStream filter = new ResponseCaptureStream(HttpContext.Current.Response.Filter, HttpContext.Current.Response.ContentEncoding, HttpContext.Current);
                HttpContext.Current.Response.Filter = filter;
                
            }
        }


        public void Dispose() { }
    }
}
