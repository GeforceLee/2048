using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UniShare.Json;

namespace UniShare
{
	/// <summary>
	/// a Task contains request informations such as request method, request paramters, etc
	/// </summary>
	public struct Task{
		/// <summary>
		/// a commandType is url path raltive to base url
		/// </summary>		
		public string commandType;
		/// <summary>
		/// get/post
		/// </summary>		
		public RequestMethod requestMethod;
		/// <summary>
		/// request parameters
		/// </summary>				
		public List<HttpParameter> parameters;
		/// <summary>
		/// construction function
		/// </summary>			
		/// <param name="cmdType"> command type </param>
		/// <param name="pms"> request parameters </param>
		public Task(string cmdType, List<HttpParameter> pms){
			commandType = cmdType;
			parameters = pms;
			requestMethod = RequestMethod.Post;
		}	
	
	}
}

