using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

using Microsoft.AspNetCore.SignalR.Client;

namespace BlazorTranslationChat.Pages
{
	public partial class Index
	{
		private HubConnection hubConnection;
		private List<string> messages = new List<string>();
		private string userInput;
		private string messageInput;
		private string language;


		protected override async Task OnInitializedAsync()
		{
			hubConnection = new HubConnectionBuilder().WithUrl(NavigationManager.ToAbsoluteUri("/chathub")).Build();
			hubConnection.On<string, string>("ReceiveMessage", (user, message) =>
			{
				var encodedMsg = $"{user}: {message}";
				messages.Add(encodedMsg);
				StateHasChanged();
			});
			await hubConnection.StartAsync();
		}

		async Task Send() => await hubConnection.SendAsync("SendMessage", userInput, messageInput, language);
		public bool IsConnected => hubConnection.State == HubConnectionState.Connected;
		public async ValueTask DisposeAsync()
		{
			await hubConnection.DisposeAsync();
		}
	}
}