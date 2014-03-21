using UnityEngine;
using System.Collections;
namespace UniShare
{
	/// <summary>
	/// return code
	/// </summary>	
	public enum ReturnType{
		RETURNTYPE_DEFAULT,
		RETURNTYPE_USERCANCEL,
		RETURNTYPE_SUCCESS,
		RETURNTYPE_NEED_OAUTH,
		RETURNTYPE_OAUTH_FAILED,
		RETURNTYPE_OAUTH_TIMEOUT,
		RETURNTYPE_OTHER_ERROR
	}
	/// <summary>
	/// a ResponseResult contains return code, and original request and response text
	/// </summary>		
	public class ResponseResult  {
		public PlatformType platformType;
		public string commandType;
		public ReturnType returnType;
		public string description;
		public ResponseResult(){
			returnType = ReturnType.RETURNTYPE_DEFAULT;
			description = "";
		}
		public ResponseResult(ReturnType rt, string descrpt){
			returnType = rt;
			description = descrpt;
		}

	}
	
}