//MicrosoftAutomaticTranslator.cs
//
// Written by Niklas Borglund and Jakob Hillerström
//
using System.Collections;


namespace SmartLocalization.Editor
{

    using UnityEngine;
    using System.Security.Cryptography.X509Certificates;
    using System.Net.Security;
    using System.Net;
    using System;
    using System.IO;
    using System.Collections.Generic;
    using SmartLocalization.Editor.MiniJSON;
    using System.Xml;
    using UnityEditor;
    using System.Text;

    [System.Serializable]
internal class AdmAccessToken
{
	public AdmAccessToken(){}
    public string AccessToken = null;
    public string TokenType = null;
    public string ExpiresIn = null;
    public string Scope = null;
}

[System.Serializable]
internal class TranslateTextData
{
	public TranslateTextData(){}
	public WebRequest translationWebRequest = null;
	public string dictionaryKey = null;	
	public TranslateTextEventHandler eventHandler = null;
}

[System.Serializable]
internal class TranslateTextArrayData
{
	public TranslateTextArrayData(){}
	public WebRequest translationWebRequest = null;
	public List<string> keys = null;	
	public TranslateTextArrayEventHandler eventHandler = null;
}

/// <summary>
/// Handles the connection with the Microsoft Translation API
/// </summary>
[System.Serializable]
public class MicrosoftAutomaticTranslator : IAutomaticTranslator
{
#region Members
	static string AuthenticationURL = "https://datamarket.accesscontrol.windows.net/v2/OAuth2-13";
	static string GetAvailableLanguagesURL = "http://api.microsofttranslator.com/v2/Http.svc/GetLanguagesForTranslate";
	static string TranslateArrayURL = "http://api.microsofttranslator.com/v2/Http.svc/TranslateArray";
	static int MaxArrayTranslationCount = 100;
	static float MaxLetterTranslationCount = 1000;

	[SerializeField]
	string clientID = string.Empty;
	[SerializeField]
	string clientSecret = string.Empty;
	[SerializeField]
	string headerValue = string.Empty;
	[SerializeField]
	bool isInitialized = false;
	[SerializeField]
	bool isInitializing = false;
	[SerializeField]
	bool didExpire = false;
	[SerializeField]
	AdmAccessToken accessToken = null;
	[SerializeField]
	DateTime timeForTokenToExpire = new DateTime(1,1,1);
	[SerializeField]
	List<string> availableLanguages = null;

	InitializeTranslatorEventHandler initializationEventHandler = null;

#endregion

#region Properties
	/// <inheritdoc />
	public bool IsInitialized
	{
		get
		{
			if(isInitialized)
			{
				if(timeForTokenToExpire.Year == 1)
				{
					isInitialized = false;
				}

				if(timeForTokenToExpire <= DateTime.UtcNow)
				{
					isInitialized = false;
				}

				if(!isInitialized)
				{
					didExpire = true;
				}
			}

			return isInitialized;
		}
	}

	 /// <inheritdoc />
	public bool InitializationDidExpire
	{
		get
		{
			if(IsInitialized)
			{
				return false;
			}

			return didExpire;
		}
	}

	 /// <inheritdoc />
	public bool IsInitializing
	{
		get
		{
			return isInitializing;
		}
	}
#endregion

#region Construction / Desctruction

	/// <summary>
	/// ctor
	/// </summary>
	public MicrosoftAutomaticTranslator(){}

#endregion

#region IAutomaticTranslator Methods

	 /// <inheritdoc />
	public void Initialize(InitializeTranslatorEventHandler eventHandler, string clientID, string clientSecret)
	{
		if(eventHandler == null)
		{
			Debug.LogError("There's no valid event handler set to initialize! Aborting process");
			return;
		}
		if(clientID == null || clientSecret == null)
		{
			Debug.LogError("There's no valid clientID & Secret set to initialize! Aborting process");
			eventHandler(false);
			return;
		}

		if(initializationEventHandler != null)
		{
			Debug.LogError("There's already an ongoing initialization process. Aborting..");
			eventHandler(false);
			return;
		}

		timeForTokenToExpire = new DateTime(1,1,1);

		initializationEventHandler = eventHandler;
		isInitialized = false;
		isInitializing = true;
		didExpire = false;
		this.clientID = clientID.RemoveWhitespace();
		this.clientSecret = clientSecret.RemoveWhitespace();

		ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(AcceptAllCertifications);

		WebRequest request = System.Net.WebRequest.Create(AuthenticationURL);
		request.Method = "POST";
		request.ContentType = "application/x-www-form-urlencoded";

		request.BeginGetRequestStream(new AsyncCallback(PrepareAuthenticationRequest), request);
	}

	/// <inheritdoc />
	public void GetAvailableTranslationLanguages(GetAvailableLanguagesEventHandler eventHandler)
	{
		if(IsInitialized == false)
		{
			Debug.LogError("The translator is not initialized! Aborting process");
			return;
		}
		if(eventHandler == null)
		{
			Debug.LogError("There's no valid event handler set to get available languages! Aborting process");
			return;
		}

		if(availableLanguages != null)
		{
			eventHandler(true, availableLanguages);
			return;
		}

		availableLanguages = new List<string>();
        HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(GetAvailableLanguagesURL);
        httpWebRequest.Headers.Add("Authorization", headerValue);
        WebResponse response = null;
        try
        {
            response = httpWebRequest.GetResponse();
            using (Stream stream = response.GetResponseStream())
            {
				ReadAvailableLanguagesResponse(stream);
            }

			eventHandler(true, availableLanguages);
        }
        catch
        {
			eventHandler(false, availableLanguages);
			availableLanguages = null;
        }
        finally
        {
            if (response != null)
            {
                response.Close();
                response = null;
            }
        }
	}

	 /// <inheritdoc />
	public void TranslateText(TranslateTextEventHandler eventHandler, string key, string textToTranslate, string languageFrom, string languageTo)
	{
		if(IsInitialized == false)
		{
			Debug.LogError("The translator is not initialized! Aborting process");
			return;
		}
		if(eventHandler == null)
		{
			Debug.LogError("There's no valid event handler set to translate text! Aborting process");
			return;
		}
		if(textToTranslate == string.Empty || languageFrom == string.Empty || languageTo == string.Empty)
		{
			Debug.LogError("Invalid values to translate! Aborting process");
			return;
		}

		Debug.Log("Translating key:" + key + ", from:" + languageFrom + ", to:" + languageTo);

		string url = "http://api.microsofttranslator.com/v2/Http.svc/Translate?text=" + WWW.EscapeURL(EscapeLinebreaks(textToTranslate)) + "&from=" + languageFrom + "&to=" + languageTo;
	   	WebRequest translationWebRequest = HttpWebRequest.Create(url);
	    translationWebRequest.Headers["Authorization"] = headerValue;
		
		
		TranslateTextData translateTextData = new TranslateTextData();
		translateTextData.dictionaryKey = key;
		translateTextData.translationWebRequest = translationWebRequest;
		translateTextData.eventHandler = eventHandler;
			
		translationWebRequest.BeginGetResponse(new AsyncCallback(ReadTranslateTextResponse), translateTextData);
	}

	/// <inheritdoc />
	public void TranslateTextArray(TranslateTextArrayEventHandler eventHandler, List<string> keys, List<string> textsToTranslate, string languageFrom, string languageTo)
	{
		if(isInitialized == false)
		{
			Debug.LogError("The translator is not initialized! Aborting process");
			return;
		}
		if(eventHandler == null)
		{
			Debug.LogError("There's no valid event handler set to translate text! Aborting process");
			return;
		}
		if(textsToTranslate == null || keys == null || textsToTranslate.Count == 0 || keys.Count == 0 || keys.Count != textsToTranslate.Count || languageFrom == string.Empty || languageTo == string.Empty)
		{
			Debug.LogError("Invalid values to translate! Aborting process");
			return;
		}

		Debug.Log("Translating keys, count:" + keys.Count + ", from:" + languageFrom + ", to:" + languageTo);

		List<string> splittedKeys = new List<string>();
		List<string> splittedTexts = new List<string>();
		int characterCount = 0;
		for(int i = 0; i < keys.Count; i++)
		{
			int currentCharacterCount = characterCount + textsToTranslate[i].Length;
			if(currentCharacterCount >= (MaxLetterTranslationCount * 0.7f) || (splittedKeys.Count + 1) >= (MaxArrayTranslationCount -1))
			{
				PrepareToTranslateArray(eventHandler, new List<string>(splittedKeys), new List<string>(splittedTexts), languageFrom, languageTo);
				currentCharacterCount = textsToTranslate[i].Length;
				splittedKeys.Clear();
				splittedTexts.Clear();
			}

			splittedKeys.Add(keys[i]);
            var textToTranslate = EscapeLinebreaks(textsToTranslate[i]);
			splittedTexts.Add(textToTranslate);
			characterCount = currentCharacterCount;
		}

		if(splittedKeys.Count > 0)
		{
			PrepareToTranslateArray(eventHandler, new List<string>(splittedKeys), new List<string>(splittedTexts), languageFrom, languageTo);
		}
	}

    string EscapeLinebreaks(string textToTranslate)
    {
        if(!string.IsNullOrEmpty(textToTranslate) && textToTranslate.Contains("\n"))
        {
            var convertedString = new StringBuilder();
            for(int charIndex = 0; charIndex < textToTranslate.Length; charIndex++)
            {
                char currentChar = textToTranslate[charIndex];
                if(currentChar == '\n')
                {
                    convertedString.Append("\\n");
                }
                else
                {
                    convertedString.Append(currentChar);
                }
            }
            return convertedString.ToString();
        }
        return textToTranslate;
    }



#endregion

#region Microsoft Translator Methods -> Initialization

	void PrepareAuthenticationRequest(IAsyncResult asyncResult)
	{
		string requestDetails = string.Format("grant_type=client_credentials&client_id={0}&client_secret={1}&scope=http://api.microsofttranslator.com", 
											WWW.EscapeURL(clientID), WWW.EscapeURL(clientSecret));	
		
		 HttpWebRequest request = (HttpWebRequest)asyncResult.AsyncState;
		
		 //now that we have the working request, write the request details into it
		 byte[] bytes = System.Text.Encoding.UTF8.GetBytes(requestDetails);
		 using(Stream postStream = request.EndGetRequestStream(asyncResult))
		 {
			postStream.Write(bytes, 0, bytes.Length);
		 }
			
		 request.BeginGetResponse(new AsyncCallback(ReadAuthenticationResponse), request);
	}

	void ReadAuthenticationResponse(IAsyncResult asyncResult)
	{
		HttpWebRequest request = (HttpWebRequest)asyncResult.AsyncState;
		HttpWebResponse response = null;

		try
		{
	   		response = (HttpWebResponse)request.EndGetResponse(asyncResult);
		}
		catch (WebException e)
        {
			response = GetErrorMessage(response, e);
			
			InvokeInitializationHandlerAndDeleteReference(false);
        }

		try
		{		
			string jsonObject = string.Empty;
			using(StreamReader streamReader = new StreamReader(response.GetResponseStream()))
			{
				jsonObject = streamReader.ReadToEnd();
			}
					
			accessToken = new AdmAccessToken();
			Dictionary<string,object> accessTokenValues = Json.Deserialize(jsonObject) as Dictionary<string,object>;

			if(accessTokenValues.ContainsKey("access_token"))
			{
				accessToken.AccessToken = accessTokenValues["access_token"].ToString();
			}
			if(accessTokenValues.ContainsKey("token_type"))
			{
				accessToken.TokenType = accessTokenValues["token_type"].ToString();
			}
			if(accessTokenValues.ContainsKey("expires_in"))
			{
				accessToken.ExpiresIn = accessTokenValues["expires_in"].ToString();
			}
			if(accessTokenValues.ContainsKey("scope"))
			{
				accessToken.Scope = accessTokenValues["scope"].ToString();
			}
	
			headerValue = "Bearer " + accessToken.AccessToken;
			
			isInitialized = true;

			timeForTokenToExpire = DateTime.UtcNow.AddSeconds(Convert.ToDouble(accessToken.ExpiresIn));
			InvokeInitializationHandlerAndDeleteReference(true);
		}
		catch (Exception ex)
		{
			Debug.LogError("MicrosoftTranslatorManager.cs:" + ex.Message);
			InvokeInitializationHandlerAndDeleteReference(false);
		}
		finally
		{
			if (response != null)
            {
                response.Close();
                response = null;
            }
		}
	}

	void InvokeInitializationHandlerAndDeleteReference(bool success)
	{
		isInitializing = false;
		if(initializationEventHandler != null)
		{
			initializationEventHandler(success);
		}
		initializationEventHandler = null;
	}

	static HttpWebResponse GetErrorMessage(HttpWebResponse response, WebException e)
	{
		using(response = (HttpWebResponse)e.Response)
		{
			HttpWebResponse httpResponse = (HttpWebResponse)response;
			Debug.LogError("Error code:" + httpResponse.StatusCode.ToString());
			using(Stream data = response.GetResponseStream())
			{
				string text = new StreamReader(data).ReadToEnd();
				Debug.LogError(text);
			}
		}
		return response;
	}

	bool AcceptAllCertifications(object sender, X509Certificate certification, X509Chain chain, SslPolicyErrors sslPolicyErrors)
	{
    	return true;
	}

#endregion

#region Microsoft Translator Methods -> Get Available Languages

	private void ReadAvailableLanguagesResponse(Stream stream)
	{
		using(XmlReader reader = XmlReader.Create(stream))
		{
			while(reader.Read())
			{
				if(reader.NodeType == XmlNodeType.Element && 
				   reader.Name == "ArrayOfstring" && 
				   reader.ReadToDescendant("string"))
				{
					do
					{
						availableLanguages.Add(reader.ReadString());
					}
					while(reader.ReadToNextSibling("string"));
				}
			}
		}
	}
#endregion

#region Microsoft Translator Methods -> Translation

	void ReadTranslateTextResponse(IAsyncResult asyncResult)
	{
		TranslateTextData translationData = null;
		HttpWebRequest request = null;
		HttpWebResponse response = null;
		try
		{
	  		translationData = asyncResult.AsyncState as TranslateTextData;
			request = translationData.translationWebRequest as HttpWebRequest;
	  		response = request.EndGetResponse(asyncResult) as HttpWebResponse;

	  		using(Stream streamResponse = response.GetResponseStream())
			{
				string translatedValue = string.Empty;
				using(XmlReader reader = XmlReader.Create(streamResponse))
				{
					reader.Read();
					reader.Read();
					translatedValue = reader.Value;
				}
		
				Debug.Log("Successfully translated key:" + translationData.dictionaryKey + "!");
				if(translationData.eventHandler != null)
				{
					translationData.eventHandler(true, translationData.dictionaryKey, translatedValue);
				}
				translationData.eventHandler = null;
			}
		}
		catch(WebException exception)
		{
			Debug.LogError("Failed to translate text! error: " + exception.Message);
			using(Stream streamResponse = exception.Response.GetResponseStream())
			{
				using(StreamReader reader = new StreamReader(streamResponse))
		      	{
					Debug.LogError(reader.ReadToEnd());
				}
			}
			if(translationData.eventHandler != null)
			{
				translationData.eventHandler(false, string.Empty, string.Empty);
			}
			translationData.eventHandler = null;
		}
		finally
		{	
			if (response != null)
			{
				response.Close();
				response = null;
			}
		}
	}

	void PrepareToTranslateArray(TranslateTextArrayEventHandler eventHandler, List<string> keys, List<string> textsToTranslate, string languageFrom, string languageTo)
	{
        string body = "<TranslateArrayRequest>" +
                            "<AppId />" +
                            "<From>";
		body += languageFrom;
		body += "</From>" +
                "<Options>" +
                " <Category xmlns=\"http://schemas.datacontract.org/2004/07/Microsoft.MT.Web.Service.V2\" />" +
                "<ContentType xmlns=\"http://schemas.datacontract.org/2004/07/Microsoft.MT.Web.Service.V2\">text/plain</ContentType>" +
                "<ReservedFlags xmlns=\"http://schemas.datacontract.org/2004/07/Microsoft.MT.Web.Service.V2\" />" +
                "<State xmlns=\"http://schemas.datacontract.org/2004/07/Microsoft.MT.Web.Service.V2\" />" +
                "<Uri xmlns=\"http://schemas.datacontract.org/2004/07/Microsoft.MT.Web.Service.V2\" />" +
                "<User xmlns=\"http://schemas.datacontract.org/2004/07/Microsoft.MT.Web.Service.V2\" />" +
                "</Options>" +
                "<Texts>";

		foreach(string text in textsToTranslate)
		{
			body += "<string xmlns=\"http://schemas.microsoft.com/2003/10/Serialization/Arrays\">" + text + "</string>";
		}
        body += "</Texts>" +"<To>";
		body += languageTo;
		body += "</To>" + "</TranslateArrayRequest>";

		WebRequest translationWebRequest = HttpWebRequest.Create(TranslateArrayURL);
	    translationWebRequest.Headers["Authorization"] = headerValue;
		translationWebRequest.ContentType = "text/xml";
        translationWebRequest.Method = "POST";

		using (Stream stream = translationWebRequest.GetRequestStream())
        {
            byte[] arrBytes = System.Text.Encoding.UTF8.GetBytes(body);
            stream.Write(arrBytes, 0, arrBytes.Length);
        }

		TranslateTextArrayData translateTextData = new TranslateTextArrayData();
		translateTextData.keys = keys;
		translateTextData.translationWebRequest = translationWebRequest;
		translateTextData.eventHandler = eventHandler;

		translationWebRequest.BeginGetResponse(new AsyncCallback(ReadTranslateArrayResponse), translateTextData);
	}

	void ReadTranslateArrayResponse(IAsyncResult asyncResult)
	{
		TranslateTextArrayData translationData = null;
		HttpWebRequest request = null;
		HttpWebResponse response = null;

		try
        {
			translationData = asyncResult.AsyncState as TranslateTextArrayData;
			request = translationData.translationWebRequest as HttpWebRequest;
			response = request.EndGetResponse(asyncResult) as HttpWebResponse;
            using (Stream stream = response.GetResponseStream())
            {		
				List<string> translatedTexts = new List<string>();
				using(XmlReader reader = XmlReader.Create(stream))
				{
					while (reader.Read())
					{
						if(reader.NodeType == XmlNodeType.Element)
						{
							if (reader.Name == "TranslateArrayResponse")
							{
								if (reader.ReadToDescendant("TranslatedText"))
								{
									do
									{
										translatedTexts.Add(reader.ReadString());
									}
									while (reader.ReadToNextSibling("TranslatedText"));
								}
							}
						}
					}
				}

				Debug.Log("Successfully translated keys!");
				if(translationData.eventHandler != null)
				{
					translationData.eventHandler(true,	translationData.keys, translatedTexts);
				}
				translationData.eventHandler = null;
            }
        }
        catch(WebException exception)
        {
			Debug.LogError("Failed to translate text array! error: " + exception.Message);
			using(Stream streamResponse = exception.Response.GetResponseStream())
			{
				using(StreamReader reader = new StreamReader(streamResponse))
				{
					Debug.LogError(reader.ReadToEnd());
				}
			}
			if(translationData.eventHandler != null)
			{
				translationData.eventHandler(false,	null, null);
			}
			translationData.eventHandler = null;
        }
        finally
        {
            if (response != null)
            {
                response.Close();
                response = null;
            }
        }
	}

#endregion

}
}//namespace SmartLocalization.Editor
