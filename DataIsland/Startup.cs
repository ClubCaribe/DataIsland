using DataIsland.Classes.signalr;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;
using System.IO;
using System.Text;
using System.Web.Http;
using System.Web;
using System;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Http.Internal;
using Autofac.Integration.WebApi;
using DataIsland.App_Start;
using Autofac;
using Autofac.Integration.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

[assembly: OwinStartup(typeof(DataIsland.Startup))]
namespace DataIsland
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {

            HttpConfiguration config = new HttpConfiguration();

            ConfigureAuth(app);

            app.Use((context, next) =>
            {
                if (context.Request.Uri.OriginalString.IndexOf("signalr") < 0)
                {
                    context.Response.Body = new ResponseCaptureStream(context.Response.Body, context.Response);
                }
                return next.Invoke();
            });

            
            IContainer container = AutofacRegistrationConfig.GetContainer();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
            

            WebApiConfig.Register(config);

            app.UseAutofacMiddleware(container);
            app.UseAutofacWebApi(config);
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            app.UseWebApi(config);
            // Any connection or hub wire up and configuration should go here
            //app.MapSignalR("/signalr", hubConfig);
            app.MapSignalR();

            GlobalHost.DependencyResolver.Register(typeof(IUserIdProvider), () => new DiUserIdProvider());
            GlobalHost.HubPipeline.AddModule(new DiPipelineModule());
        }
    }

    public class DiPipelineModule : HubPipelineModule
    {
        public DiPipelineModule()
        {

        }

        protected override bool OnBeforeOutgoing(IHubOutgoingInvokerContext context)
        {
            if (context.Invocation.Args != null && context.Invocation.Args.Length > 0)
            {
                for (int i = 0; i < context.Invocation.Args.Length; i++)
                {
                    this.Translate(context.Invocation.Args[i]);
                }
            }

            return base.OnBeforeOutgoing(context);
        }

        public void Translate(object arg)
        {
            try
            {
                if (arg != null)
                {
                    Type argType = arg.GetType();
                    BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
                    foreach (PropertyInfo prop in argType.GetProperties(flags))
                    {
                        if (prop.MemberType == MemberTypes.Property)
                        {
                            if (prop.PropertyType == typeof(string))
                            {
                                    string translatedVal = (string)prop.GetValue(arg);
                                    translatedVal = translatedVal.Replace("[tr]", "").Replace("[/tr]", "");
                                    prop.SetValue(arg, translatedVal);

                            }
                            else if (typeof(IDictionary).IsAssignableFrom(prop.PropertyType))
                            {
                                object oDict = prop.GetValue(arg);
                                IDictionary dict = (IDictionary)oDict;
                                if (dict != null)
                                {
                                    Dictionary<object, object> valsToChange = new Dictionary<object, object>();
                                    ICollection keys = dict.Keys;
                                    var itemPropertyInfo = oDict.GetType().GetProperty("Item");
                                    foreach (var elm in keys)
                                    {
                                        object val = itemPropertyInfo.GetValue(oDict, new[] { elm });
                                        if (val.GetType() == typeof(string))
                                        {
                                            string translatedVal = (string)val;
                                            translatedVal = translatedVal.Replace("[tr]", "").Replace("[/tr]", "");
                                            valsToChange[elm] = translatedVal;
                                            
                                        }
                                        else if (val.GetType().IsClass)
                                        {
                                            this.Translate(val);
                                        }
                                    }
                                    if(valsToChange.Count>0)
                                    {
                                        foreach(var elm in valsToChange)
                                        {
                                            itemPropertyInfo.SetValue(oDict, elm.Value, new[] { elm.Key });
                                        }
                                    }
                                }
                            }
                            else if (typeof(IEnumerable).IsAssignableFrom(prop.PropertyType))
                            {
                                object oList = prop.GetValue(arg);
                                IEnumerable enumerable = (IEnumerable)oList;
                                if (enumerable != null)
                                {
                                    Dictionary<object, object> valsToChange = new Dictionary<object, object>();
                                    int count = 0;
                                    var itemPropertyInfo = oList.GetType().GetProperty("Item");
                                    foreach (object child in enumerable)
                                    {
                                        object val = itemPropertyInfo.GetValue(oList, new object[] { count });
                                        if (val.GetType() == typeof(string))
                                        {
                                            string translatedVal = (string)val;
                                            translatedVal = translatedVal.Replace("[tr]", "").Replace("[/tr]", "");
                                            valsToChange[count] = translatedVal;
                                        }
                                        else if(val.GetType().IsClass)
                                        {
                                            this.Translate(val);
                                        }

                                        count++;
                                    }
                                    if (valsToChange.Count > 0)
                                    {
                                        foreach (var elm in valsToChange)
                                        {
                                            itemPropertyInfo.SetValue(oList, elm.Value, new[] { elm.Key });
                                        }
                                    }
                                }
                            }
                            else if (prop.PropertyType.IsClass || typeof(IEnumerable).IsAssignableFrom(prop.PropertyType))
                            {
                                this.Translate(prop.GetValue(arg));
                            }
                        }
                    }
                }
            }
            catch
            {
            }
        }
        
    }

    public class ResponseCaptureStream : Stream
    {
        private Stream _streamToCapture;
        private Encoding _responseEncoding;

        MemoryStream _internalStream = null;
        IOwinResponse _response = null;

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

        public ResponseCaptureStream(Stream streamToCapture, IOwinResponse response )
        {
            try
            {
                _internalStream = new MemoryStream();
                _responseEncoding = Encoding.UTF8;
                _streamToCapture = streamToCapture;
                _response = response;
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
                            _streamContent = TranslateResource(_responseEncoding.GetString(_internalStream.ToArray()));

                            byte[] toflush = _responseEncoding.GetBytes(StreamContent);
                            _response.ContentLength = toflush.Length;
                            _streamToCapture.Write(toflush, 0, toflush.Length);
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

        public async override System.Threading.Tasks.Task FlushAsync(System.Threading.CancellationToken cancellationToken)
        {
            try
                {
                    if (canSaveAsText())
                    {
                            _streamContent = TranslateResource(_responseEncoding.GetString(_internalStream.ToArray()));

                            byte[] toflush = _responseEncoding.GetBytes(StreamContent);

                            _response.ContentLength = toflush.Length;
                            await _streamToCapture.WriteAsync(toflush, 0, toflush.Length, cancellationToken);
                    }
                    else
                    {
                        await _streamToCapture.WriteAsync(_internalStream.ToArray(), 0, (int)_internalStream.Length, cancellationToken);
                    }
                }
                catch
                {
                    _streamToCapture.Write(_internalStream.ToArray(), 0, (int)_internalStream.Length);
                }
                _internalStream.SetLength(0);
                _internalStream.Dispose();
                await _streamToCapture.FlushAsync(cancellationToken);
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
            Flush();
        }

        public async override System.Threading.Tasks.Task WriteAsync(byte[] buffer, int offset, int count, System.Threading.CancellationToken cancellationToken)
        {
            await _internalStream.WriteAsync(buffer, 0, count,cancellationToken);
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

                
                string ct = _response.ContentType;
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
}