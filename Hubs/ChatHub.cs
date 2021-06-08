using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace BlazorTranslationChat.Hubs
{
	public class ChatHub : Hub
	{
		private readonly TranslationService _translationService;
		public ChatHub(TranslationService translationService)
		{
			_translationService = translationService;
		}
		public async Task SendMessage(string user, string message, string language)
		{
			var translatedMessage = await _translationService.TranslatorAsync(message, language);
			await Clients.All.SendAsync("ReceiveMessage", user, translatedMessage);
		}
	}
}