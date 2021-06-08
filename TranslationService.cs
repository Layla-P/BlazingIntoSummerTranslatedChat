using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace BlazorTranslationChat
{
	public class TranslationService
	{
		private readonly Secrets _secrets;
		private readonly HttpClient _client;
		public TranslationService(IOptions<Secrets> secrets, HttpClient client)
		{
			_secrets = secrets.Value ?? throw new ArgumentNullException(nameof(secrets));
			_client = client ?? throw new ArgumentNullException(nameof(client));
		}
		public async Task<string> TranslatorAsync(string text, string language)
		{
			var key = GetLanguage(language);
			if (key is null) return text;


			var textEncoded = HttpUtility.UrlEncode(text, System.Text.Encoding.UTF8);
			var endpoint = $"https://api.funtranslations.com/translate/{language}.json?text={textEncoded}";
			HttpResponseMessage response;
			string translatedText = "default";


			_client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			if (!string.IsNullOrEmpty(key))
			{ _client.DefaultRequestHeaders.Add("X-Funtranslations-Api-Secret", key); }
			var responseMessage = await
				_client
					.GetAsync(endpoint);

			if (responseMessage.IsSuccessStatusCode)
			{
				string jsonContent = await responseMessage.Content.ReadAsStringAsync();
				dynamic data = JsonConvert.DeserializeObject(jsonContent);
				translatedText = data.contents.translated;
			}
			else if (responseMessage.StatusCode == (HttpStatusCode)429)
			{
				//Fun Translations API rate limits to 5 calls an hour unless you are using the paid for service.
				translatedText = "too much yoda speak, the force be with you for an hour";
			}



			return translatedText;
		}

		private string GetLanguage(string lang)
		{
			return lang switch
			{
				"cockney" => _secrets.CockneyApiKey,
				"redneck" => string.Empty,
				_ => null
			};
		}
	}
}
