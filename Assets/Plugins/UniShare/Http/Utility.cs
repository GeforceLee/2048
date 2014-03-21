using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UniShare.Json;
using System.Security.Cryptography;
namespace UniShare
{
	/// <summary>
	/// RequestMethod, current get, post only
	/// </summary>
	public enum RequestMethod
	{
		Get,
		Post
	}
	/// <summary>
	/// http Utility, contains encoder, converter, etc.
	/// </summary>
	public static class Utility
	{
		private static string UnreservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";
		/// <summary>
		/// convert string time to DateTime
		/// </summary>
		/// <param name="dateString">date string</param>
		/// <returns>DateTime</returns>
		public static DateTime ParseUTCDate(string dateString)
		{
			System.Globalization.CultureInfo provider = System.Globalization.CultureInfo.InvariantCulture;

			DateTime dt = DateTime.ParseExact(dateString, "ddd MMM dd HH:mm:ss zzz yyyy", provider);

			return dt;
		}
		internal static Dictionary<string, string> GetDictionaryFromJSON(string json)
		{
            //modified by chren
			var result = JsonConvert.DeserializeObject<object[]>(json);

			Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (Dictionary<string, string> loc in result)
            {
                foreach (var x in loc)
                {
                    dict.Add(x.Key, x.Value);
                }

            }
			return dict;
		}

		internal static IEnumerable<string> GetStringListFromJSON(string json)
		{
            // modified by chren
			var result = JsonConvert.DeserializeObject<object[]>(json);
			List<string> list = new List<string>();
            foreach (Dictionary<string, object> loc in result)
			{
				foreach (var x in loc)
				{
					list.Add(x.Value.ToString());
				}

			}
			return list;
		}

		/// <summary>
		/// Build query string
		/// </summary>
		/// <param name="parameters">Query parameters dictonary</param>
		/// <returns>query string</returns>
		internal static string BuildQueryString(Dictionary<string, string> parameters)
		{
			List<string> pairs = new List<string>();
			foreach (KeyValuePair<string, string> item in parameters)
			{
				if (string.IsNullOrEmpty(item.Value))
					continue;
				
				pairs.Add(string.Format("{0}={1}", Uri.EscapeDataString(item.Key), Uri.EscapeDataString(item.Value)));
			}

			return string.Join("&", pairs.ToArray());
		}
		
		
		/// <summary>
		/// Build query string
		/// </summary>
		/// <param name="parameters">Query parameters array</param>
		/// <returns>query string</returns>
		internal static string BuildQueryString(params HttpParameter[] parameters)
		{
			List<string> pairs = new List<string>();
			foreach (var item in parameters)
			{
				if (item.IsBinaryData)
					continue;

				var value = string.Format("{0}", item.Value);
				if (!string.IsNullOrEmpty(value))
				{
					pairs.Add(string.Format("{0}={1}", Uri.EscapeDataString(item.Name), Uri.EscapeDataString(value)));
				}
				//Debug.Log(Uri.EscapeDataString(value));
			}

			return string.Join("&", pairs.ToArray());
		}
		
		internal static string BuildJsonString(params HttpParameter[] parameters)
		{
			
			Dictionary<string, object> share = new Dictionary<string, object>();

			foreach (var item in parameters)
			{
				if (item.IsBinaryData)
					continue;
				
				
				var value = string.Format("{0}", item.Value);
				if (!string.IsNullOrEmpty(value))
				{
					share.Add(item.Name, value);
					
				}
				//Debug.Log(Uri.EscapeDataString(value));
			}
			string shareContentString = JsonWriter.Serialize(share);
			return shareContentString;
		}
		
		internal static string GetBoundary()
		{
			string pattern = "abcdefghijklmnopqrstuvwxyz0123456789";
			StringBuilder boundaryBuilder = new StringBuilder();
			System.Random rnd = new System.Random();
			for (int i = 0; i < 10; i++)
			{
				var index = rnd.Next(pattern.Length);
				boundaryBuilder.Append(pattern[index]);
			}
			return boundaryBuilder.ToString();
		}
		/// <summary>
		/// create post body
		/// </summary>
		/// <param name="boundary">boundary</param>
		/// <param name="parameters">parameters</param>
		/// <returns></returns>
		internal static byte[] BuildPostData(string boundary, params HttpParameter[] parameters)
		{
			List<HttpParameter> pairs = new List<HttpParameter>(parameters);
			pairs.Sort(new HttpParameterComparer());
			MemoryStream buff = new MemoryStream();
			byte[] EOL = Encoding.ASCII.GetBytes("\r\n");
			byte[] headerBuff = Encoding.ASCII.GetBytes(string.Format("--{0}\r\n", boundary));
			byte[] footerBuff = Encoding.ASCII.GetBytes(string.Format("\r\n--{0}--\r\n", boundary));


			StringBuilder contentBuilder = new StringBuilder();

			foreach (HttpParameter p in pairs)
			{
				if (!p.IsBinaryData)
				{
					var value = string.Format("{0}", p.Value);
					if (string.IsNullOrEmpty(value))
					{
						continue;
					}


					buff.Write(headerBuff, 0, headerBuff.Length);
					
					byte[] dispositonBuff = Encoding.UTF8.GetBytes(string.Format("content-disposition: form-data; name=\"{0}\"\r\n\r\n{1}", p.Name, p.Value.ToString()));
					buff.Write(dispositonBuff, 0, dispositonBuff.Length);
					
				}
				else
				{
					buff.Write(headerBuff, 0, headerBuff.Length);
					//string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: \"image/jpeg\"\r\n\r\n";
					string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: \"image/unknow\"\r\nContent-Transfer-Encoding: binary\r\n\r\n";
					byte[] fileBuff = System.Text.Encoding.UTF8.GetBytes(string.Format(headerTemplate, p.Name, string.Format("upload{0}", BitConverter.ToInt64(Guid.NewGuid().ToByteArray(), 0))));
					buff.Write(fileBuff, 0, fileBuff.Length);
					byte[] file = (byte[])p.Value;
					buff.Write(file, 0, file.Length);
				}
				buff.Write(EOL, 0, EOL.Length);
			}

			buff.Write(footerBuff, 0, footerBuff.Length);
			buff.Position = 0;

			byte[] contentBuff = new byte[buff.Length];
			buff.Read(contentBuff, 0, contentBuff.Length);
			buff.Close();
			buff.Dispose();
			return contentBuff;
		}
		
		/// <summary>
		/// md5 encode
		/// </summary>
        public static string MD5Encrpt(string plainText)
        {
            MD5 md5Hash = MD5.Create();
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(plainText));
            StringBuilder sbList = new StringBuilder();
            foreach (byte d in data)
            {
                sbList.Append(d.ToString("x2"));
            }
            return sbList.ToString();
        }
		/// <summary>
		/// Url Encode
		/// </summary>		
		public static string UrlEncode(string Input)
		{
			StringBuilder Result = new StringBuilder();
			for (int x = 0; x < Input.Length; ++x)
			{
			 if (UnreservedChars.IndexOf(Input[x]) != -1)
			     Result.Append(Input[x]);
			 else
			     Result.Append("%").Append(String.Format("{0:X2}", (int)Input[x]));
			}
			return Result.ToString();
		}		
		
	}
	
}