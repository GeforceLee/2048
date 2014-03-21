using System;
using System.Collections.Generic;
using System.Text;
namespace UniShare
{
	/// <summary>
	/// http参数
	/// </summary>
	public class HttpParameter
	{
		/// <summary>
		/// 参数名称
		/// </summary>
		public string Name
		{
			get;
			set;
		}
		/// <summary>
		/// 参数值
		/// </summary>
		public object Value
		{
			get;
			set;
		}

		/// <summary>
		/// 是否为二进制参数（如图片、文件等）
		/// </summary>
		public bool IsBinaryData
		{
			get
			{
				if (Value != null && Value.GetType() == typeof(byte[]))
					return true;
				else
					return false;
			}
		}

		/// <summary>
		/// construction function
		/// </summary>
		public HttpParameter()
		{ 
		
		}

		/// <summary>
		/// construction function
		/// </summary>
		/// <param name="name">key</param>
		/// <param name="value">value</param>
		public HttpParameter(string name, string value)
		{
			this.Name = name;
			this.Value = value;
		}
		/// <summary>
		/// construction function
		/// </summary>
		/// <param name="name">key</param>
		/// <param name="value">value</param>
		public HttpParameter(string name, bool value)
		{
			this.Name = name;
			this.Value = value ? "1" : "0";
		}
		/// <summary>
		/// construction function
		/// </summary>
		/// <param name="name">key</param>
		/// <param name="value">value</param>
		public HttpParameter(string name, int value)
		{
			this.Name = name;
			this.Value = string.Format("{0}", value);
		}
		/// <summary>
		/// construction function
		/// </summary>
		/// <param name="name">key</param>
		/// <param name="value">value</param>
		public HttpParameter(string name, long value)
		{
			this.Name = name;
			this.Value = string.Format("{0}", value);
		}
		/// <summary>
		/// construction function
		/// </summary>
		/// <param name="name">key</param>
		/// <param name="value">value</param>
		public HttpParameter(string name, byte[] value)
		{
			this.Name = name;
			this.Value = value;
		}

		/// <summary>
		/// construction function
		/// </summary>
		/// <param name="name">key</param>
		/// <param name="value">value</param>
		public HttpParameter(string name, object value)
		{
			this.Name = name;
			this.Value = value;
		}
	}
	/// <summary>
	/// IComparer interface for WeiboParameter 
	/// </summary>
	internal class HttpParameterComparer : IComparer<HttpParameter>
	{

		public int Compare(HttpParameter x, HttpParameter y)
		{
			return StringComparer.CurrentCulture.Compare(x.Name, y.Name);
		}
	}
}
