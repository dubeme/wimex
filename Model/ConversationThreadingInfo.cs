using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.Chat;

namespace WIMEX.Model
{
    /// <summary>
    ///
    /// </summary>
    public class ConversationThreadingInfo
    {
        /// <summary>
        ///  Gets or sets the Contact.Id for the remote participant.
        /// </summary>
        [JsonProperty(PropertyName = "a")]
        public string ContactId { get; set; }

        /// <summary>
        /// Gets or sets the ID of the ChatConversation.
        /// </summary>
        [JsonProperty(PropertyName = "b")]
        public string ConversationId { get; set; }

        /// <summary>
        /// Gets or sets a string where you can store your own custom threading info.
        /// </summary>
        [JsonProperty(PropertyName = "c")]
        public string Custom { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates the type of threading info, such as participant,
        /// contact ID, conversation ID, and so on.
        /// </summary>
        [JsonProperty(PropertyName = "d")]
        public ChatConversationThreadingKind Kind { get; set; }

        /// <summary>
        /// Gets the list of participants in the ChatConversation.
        /// </summary>
        [JsonProperty(PropertyName = "e")]
        public IEnumerable<string> Participants { get; set; }

        public static implicit operator ConversationThreadingInfo(ChatConversationThreadingInfo threadingInfo)
        {
            return new ConversationThreadingInfo
            {
                ContactId = threadingInfo.ContactId,
                ConversationId = threadingInfo.ConversationId,
                Custom = threadingInfo.Custom,
                Kind = threadingInfo.Kind,
                Participants = threadingInfo.Participants.Select(p => p)
            };
        }
    }
}