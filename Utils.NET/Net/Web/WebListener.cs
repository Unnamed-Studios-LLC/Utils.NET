using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Serialization;
using Utils.NET.IO;
using Utils.NET.Logging;

namespace Utils.NET.Net.Web
{
    public class WebListener
    {
        /// <summary>
        /// The http listener used to handle incoming http requests
        /// </summary>
        private HttpListener listener;

        /// <summary>
        /// Dictionary containing handlers
        /// </summary>
        private Dictionary<string, Func<HttpListenerContext, NameValueCollection, Task<object>>> handlers = new Dictionary<string, Func<HttpListenerContext, NameValueCollection, Task<object>>>();

        private ConcurrentDictionary<Type, XmlSerializer> xmlSerializers = new ConcurrentDictionary<Type, XmlSerializer>();

        public WebListener(params string[] prefixes)
        {
            listener = new HttpListener();
            foreach (var prefix in prefixes)
                listener.Prefixes.Add(prefix);
        }

        public void Start()
        {
            listener.Start();
            listener.BeginGetContext(OnGetContext, null);

            Log.Write("Web server running on prefix: " + listener.Prefixes.First());
        }

        public void Stop()
        {
            listener.Stop();
        }

        public void AddHandler(string subPath, Func<HttpListenerContext, NameValueCollection, Task<object>> handler)
        {
            handlers.Add(subPath.ToLower(), handler);
        }

        private async void OnGetContext(IAsyncResult ar)
        {
            HttpListenerContext context;
            try
            {
                context = listener.EndGetContext(ar);
            }
            catch (Exception)
            {
                return;
            }
            finally
            {
                if (listener.IsListening)
                {
                    listener.BeginGetContext(OnGetContext, null);
                }
            }

            var query = new NameValueCollection();
            using (StreamReader rdr = new StreamReader(context.Request.InputStream))
                query = HttpUtility.ParseQueryString(rdr.ReadToEnd());

            if (query.AllKeys.Length == 0)
            {
                string url = context.Request.RawUrl;
                int indexOfQuery = url.IndexOf('?');
                if (indexOfQuery >= 0)
                    query = HttpUtility.ParseQueryString((indexOfQuery < url.Length - 1) ? url.Substring(indexOfQuery + 1) : string.Empty);
            }

            var localPath = context.Request.Url.LocalPath.Substring(1);
            if (!handlers.TryGetValue(localPath, out var handler)) // no handler found
            {
                context.Response.StatusCode = 404;
                context.Response.Close();
                return;
            }

            try
            {
                using (var wr = new StreamWriter(context.Response.OutputStream))
                {
                    var obj = await handler.Invoke(context, query);
                    GetSerializer(obj.GetType()).Serialize(wr, obj);
                    wr.Flush();
                }
            }
            finally
            {
                context.Response.Close();
            }
        }

        private XmlSerializer GetSerializer(Type type)
        {
            if (!xmlSerializers.TryGetValue(type, out var serializer))
            {
                serializer = new XmlSerializer(type);
                xmlSerializers.TryAdd(type, serializer);
            }
            return serializer;
        }
    }
}
