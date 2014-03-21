//using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Globalization;
using System.Threading;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace UniShare
{
	/// <summary>
	/// http exception
	/// </summary>	
	public class HTTPException : Exception
	{
		static public string NETWORK_ERROR = "network error";
		public HTTPException (string message) : base(message)
		{
		}
	}
	
	public enum RequestState {
		Waiting, Reading, Done	
	}
	
	/// <summary>
	/// Sockets implemented http request class wraper
	/// </summary>
	public class Request
	{
		/// <summary>
		/// request methon, "GET", "POST", default is "GET"
		/// </summary>		
		public string method = "GET";
		/// <summary>
		/// http protocol, default is "HTTP/1.1"
		/// </summary>	
		public string protocol = "HTTP/1.1";
		/// <summary>
		/// bytes of http body
		/// </summary>			
		public byte[] bytes;
		/// <summary>
		/// uri http request
		/// </summary>				
		public Uri uri;
		/// <summary>
		/// bytes represent for EOL
		/// </summary>			
		public static byte[] EOL = { (byte)'\r', (byte)'\n' };
		/// <summary>
		/// response object
		/// </summary>			
		public Response response = null;
		/// <summary>
		/// is request finished
		/// </summary>			
		public bool isDone = false;
		/// <summary>
		/// maximum retry count, default is 8
		/// </summary>		
		public int maximumRetryCount = 8;
		/// <summary>
		/// if accept gzip, default is true
		/// </summary>			
		public bool acceptGzip = true;
		/// <summary>
		/// if use cache, default is false
		/// </summary>			
		public bool useCache = false;
		/// <summary>
		/// exception object
		/// </summary>		
		public Exception exception = null;
		/// <summary>
		/// current requestState
		/// </summary>			
		public RequestState state = RequestState.Waiting;
		
		Dictionary<string, List<string>> headers = new Dictionary<string, List<string>> ();
		static Dictionary<string, string> etags = new Dictionary<string, string> ();
		/// <summary>
		/// construction function
		/// </summary>
		/// <param name="method"> get/post </param>
		/// <param name="uri"> uri of request </param>
		/// <returns></returns>				
        public Request(string method, Uri uri)
        {
            this.method = method;
            this.uri = uri;
        }
		/// <summary>
		/// construction function
		/// </summary>
		/// <param name="method"> get/post </param>
		/// <param name="uri"> url of request </param>
		/// <returns></returns>		
		public Request (string method, string uri)
		{
			this.method = method;
			this.uri = new Uri (uri);
		}
		/// <summary>
		/// construction function
		/// </summary>
		/// <param name="method"> get/post </param>
		/// <param name="uri"> uri of request </param>
		/// <param name="useCache"> should use cache </param>
		/// <returns></returns>
		public Request (string method, string uri, bool useCache)
		{
			this.method = method;
			this.uri = new Uri (uri);
			this.useCache = useCache;
		}
		/// <summary>
		/// construction function
		/// </summary>
		/// <param name="method"> get/post </param>
		/// <param name="uri"> uri of request </param>
		/// <param name="bytes"> http body </param>
		/// <returns></returns>
		public Request (string method, string uri, byte[] bytes)
		{
			this.method = method;
			this.uri = new Uri (uri);
			this.bytes = bytes;
		}
		/// <summary>
		/// add header list with key-value
		/// </summary>
		/// <param name="name"> key of header </param>
		/// <param name="value"> value of header </param>
		/// <returns>void</returns>
		public void AddHeader (string name, string value)
		{
			name = name.ToLower ().Trim ();
			value = value.Trim ();
			if (!headers.ContainsKey (name))
				headers[name] = new List<string> ();
			headers[name].Add (value);
		}
		/// <summary>
		/// get header list by name
		/// </summary>
		/// <param name="name"> key of header </param>
		/// <returns>return header</returns>
		public string GetHeader (string name)
		{
			name = name.ToLower ().Trim ();
			if (!headers.ContainsKey (name))
				return "";
			return headers[name][0];
		}
		/// <summary>
		/// get header list by name
		/// </summary>
		/// <param name="name"> key of header </param>
		/// <returns>return header list</returns>
		public List<string> GetHeaders (string name)
		{
			name = name.ToLower ().Trim ();
			if (!headers.ContainsKey (name))
				headers[name] = new List<string> ();
			return headers[name];
		}
		/// <summary>
		/// set header list with key-value
		/// </summary>
		/// <param name="name"> key of header </param>
		/// <param name="value"> value of header </param>
		/// <returns>void</returns>
		public void SetHeader (string name, string value)
		{
			name = name.ToLower ().Trim ();
			value = value.Trim ();
			if (!headers.ContainsKey (name))
				headers[name] = new List<string> ();
			headers[name].Clear ();
			headers[name].Add (value);
		}
		/// <summary>
		/// send http request
		/// </summary>
		public string Send ()
		{
			isDone = false;
			state = RequestState.Waiting;
			
			try {
				var retry = 0;
				while (++retry < maximumRetryCount) {
					if (useCache) {
						string etag = "";
						if (etags.TryGetValue (uri.AbsoluteUri, out etag)) {
							SetHeader ("If-None-Match", etag);
						}
					}
					SetHeader ("Host", uri.Host);
					var client = new TcpClient ();
					client.SendTimeout = 10000;
					client.ReceiveTimeout = 60000;
					client.Connect (uri.Host, uri.Port);
					using (var stream = client.GetStream ()) {
						var ostream = stream as Stream;
                        //Stream ostream = null;
                        if (uri.Scheme.ToLower() == "https")
                        {
                            //System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Ssl3;
                            RemoteCertificateValidationCallback callback =
                                new RemoteCertificateValidationCallback(ValidateServerCertificate);
                            SslProtocols protocol = SslProtocols.Ssl2 | SslProtocols.Ssl3 | SslProtocols.Tls;
                            //ostream = new SslStream(stream, false, callback);

                            try
                            {
                                //var ssl = ostream as SslStream;
                                var ssl = new SslStream(stream, true, callback);
                                ssl.AuthenticateAsClient(uri.Host, null, protocol, false);

                                WriteToStream(ssl);
                                response = new Response();
                                state = RequestState.Reading;
                                response.ReadFromStream(ssl);

                            }
                            catch (Exception e)
                            {
                                //Debug.LogError ("Exception: " + e.Message);
                                return string.Empty;
                            }
                            //stream.Close();
                        }
                        else
                        {
                            WriteToStream(ostream);
                            response = new Response();
                            state = RequestState.Reading;
                            response.ReadFromStream(ostream);
                        }
					}
                    
					client.Close ();
					switch (response.status) {
					case 307:
					case 302:
					case 301:
						uri = new Uri (response.GetHeader ("Location"));
						continue;
					default:
						retry = maximumRetryCount;
						break;
					}
				}
				if (useCache) {
					string etag = response.GetHeader ("etag");
					if (etag.Length > 0)
						etags[uri.AbsoluteUri] = etag;
				}
				
			} catch (Exception e) {
				Console.WriteLine ("Unhandled Exception, aborting request.");
				Console.WriteLine (e);
				exception = e;
				response = null;
				return HTTPException.NETWORK_ERROR;
			}
			state = RequestState.Done;
			isDone = true;
            return response.Text;
		}
		
		/// <summary>
		/// set bytes by text string
		/// </summary>
		public string Text {
			set { bytes = System.Text.Encoding.UTF8.GetBytes (value); }
		}

		public static bool ValidateServerCertificate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			return true;
		}

		void WriteToStream (Stream outputStream)
		{
			var stream = new BinaryWriter (outputStream);
            if (method == "get")
                stream.Write(ASCIIEncoding.ASCII.GetBytes(method.ToUpper() + " " + uri.PathAndQuery + " " + protocol));
            else if (method == "post")
                stream.Write(ASCIIEncoding.ASCII.GetBytes(method.ToUpper() + " " + uri.AbsolutePath + " " + protocol));
            stream.Write(EOL);
           
			foreach (string name in headers.Keys)
            {

                foreach (string value in headers[name])
                {
                    stream.Write(ASCIIEncoding.ASCII.GetBytes(name));
                    stream.Write(':');
                    stream.Write(ASCIIEncoding.ASCII.GetBytes(value));
                    stream.Write(EOL);
				}
				
            }
            stream.Write(EOL);
            if (method == "post")
            {
				
                if (bytes != null && bytes.Length > 0)
                {
                    stream.Write(bytes);
                }
				stream.Write(EOL);
                stream.Flush();
                //stream.Close();
            }
			
		}
		
	}
	
}

