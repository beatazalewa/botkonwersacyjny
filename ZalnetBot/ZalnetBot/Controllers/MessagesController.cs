using Microsoft.Bot.Connector;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace ZalnetBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Otrzymanie wiadomości od użytkownika i udzielenie odpowiedzi na nią 
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                // wyliczenie, co ma być zwrócone 
                int length = (activity.Text ?? string.Empty).Length;
                Activity reply = activity.CreateReply("");
                // zwrócenie odpowiedzi do użytkownika 
                switch (activity.Text)
                {
                    case "Hej": 
                    case "Cześć": 
                        reply = activity.CreateReply($"{activity.Text}. Jak mogę Ci pomóc?"); 
                        break;
                    case "Chciałabym porozmawiać": 
                        reply = activity.CreateReply($"Ok, nie ma problemu."); 
                        break;
                    case "Beata": 
                        reply = activity.CreateReply($"{ activity.Text}, miło mi Cię poznać"); 
                        break;
                    case "Jak się masz?": 
                        reply = activity.CreateReply($"Dobrze, a jak u Ciebie ?"); 
                        break;
                    case "Skąd jesteś?": 
                        reply = activity.CreateReply($"Wola Radzięcka , a Ty?"); 
                        break;
                    case "Warszawa": 
                        reply = activity.CreateReply($"Jesteś ze stolicy? To super!!"); 
                        break;
                    case "Ile masz lat?": 
                        reply = activity.CreateReply($"30 , a Ty?"); 
                        break;
                    case "Jesteś studentem?":
                        reply = activity.CreateReply($"Nie, już pracuję, a Ty?");
                        break;
                    case "Do zobaczenia": 
                        reply = activity.CreateReply($"Do zobaczenia , dziękuję !!"); 
                        break;
                    default:
                        reply = activity.CreateReply($"Takiej odpowiedzi chat bot stworzony przy użyciu Bot Framework nie rozumie!!"); 
                        break;
                }
                await connector.Conversations.ReplyToActivityAsync(reply);
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }
        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Tutaj dodajemy implementację usunięcia użytkownika 
                // Jeśli obsługujemy usunięcie użytkownika, zwracamy prawdziwą wiadomość 
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Obsługujemy zmianę stanów konwersacji, jak dodawanie lub usuwanie 
                // Używamy Activity.MembersAdded, Activity.MembersRemoved i Activity.Action w celach informacyjnych 
                // Nie jest dostępne dla wszystkich kanałów 
                IConversationUpdateActivity update = message;
                var client = new ConnectorClient(new Uri(message.ServiceUrl), new MicrosoftAppCredentials());
                if (update.MembersAdded != null && update.MembersAdded.Any())
                {
                    foreach (var newMember in update.MembersAdded)
                    {
                        if (newMember.Id != message.Recipient.Id)
                        {
                            var reply = message.CreateReply();
                            reply.Text = $"Witaj {newMember.Name}!"; 
                            client.Conversations.ReplyToActivityAsync(reply);
                        }
                    }
                }
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Obsługujemy dodawanie / usuwanie z listy kontaktów (Handle add/remove from contact lists)
                // Activity.From + Activity.Action reprezentują akcje, które się wydarzyły 
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Obsługa akcji, kiedy użytkownik coś wpisuje 
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }
            return null;
        }
    }
}
