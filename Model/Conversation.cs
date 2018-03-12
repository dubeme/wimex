using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using Windows.ApplicationModel.Chat;
using System;

/// <summary>
///
/// </summary>
namespace WIMEX.Model
{
    /// <summary>
    ///
    /// </summary>
    public class Conversation
    {
        [JsonIgnore]
        public IEnumerable<Attachment> Attachments
        {
            get
            {
                return Messages.SelectMany(message => message.Attachments);
            }
        }

        /// <summary>
        /// Gets or sets a boolean that indicates whether participants can be modified or not.
        /// </summary>
        [JsonProperty(PropertyName = "a")]
        public bool CanModifyParticipants { get; set; }

        public string CleanId
        {
            get
            {
                return Regex.Replace(this.Id, "[^\\w]", "");
            }
        }

        /// <summary>
        /// Gets or sets a Boolean value indicating if there are unread messages in the ChatConversation.
        /// </summary>
        [JsonProperty(PropertyName = "b")]
        public bool HasUnreadMessages { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the ChatConversation.
        /// </summary>
        [JsonProperty(PropertyName = "c")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets a Boolean value indicating if the ChatConversation is muted.
        /// </summary>
        [JsonProperty(PropertyName = "d")]
        public bool IsConversationMuted { get; set; }

        /// <summary>
        /// Gets or sets the item kind.
        /// </summary>
        [JsonProperty(PropertyName = "e")]
        public ChatItemKind ItemKind { get; set; }

        /// <summary>
        /// Gets or sets the messages of this Conversation.
        /// </summary>
        [JsonProperty(PropertyName = "f")]
        [IgnoreDataMember]
        public IEnumerable<Message> Messages { get; set; }
        
        /// <summary>
        /// Gets or Sets the flattened messages in this conversation
        /// </summary>
        [IgnoreDataMember]
        public IEnumerable<FlattenedMessage> FlattenedMessages
        {
            get => this.Messages.Select(msg => FlattenMessage(this, msg));
        }

        /// <summary>
        /// Gets or sets the ID of the most recent message in the conversation.
        /// </summary>
        [JsonProperty(PropertyName = "g")]
        public string MostRecentMessageId { get; set; }

        /// <summary>
        /// Gets or sets a list of all the participants in the conversation.
        /// </summary>
        [JsonProperty(PropertyName = "h")]
        public IEnumerable<string> Participants { get; set; }

        /// <summary>
        /// Gets or sets the subject of a group conversation.
        /// </summary>
        [JsonProperty(PropertyName = "i")]
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the threading info for the ChatConversation.
        /// </summary>
        [JsonProperty(PropertyName = "j")]
        public ConversationThreadingInfo ThreadingInfo { get; set; }

        /// <summary>
        /// Performs an implicit conversion from <see cref="ChatConversation"/> to <see cref="Conversation"/>.
        /// </summary>
        /// <param name="conversation">The conversation.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator Conversation(ChatConversation conversation)
        {
            return new Conversation
            {
                CanModifyParticipants = conversation.CanModifyParticipants,
                HasUnreadMessages = conversation.HasUnreadMessages,
                Id = conversation.Id,
                IsConversationMuted = conversation.IsConversationMuted,
                ItemKind = conversation.ItemKind,
                Messages = new List<Message>(),
                MostRecentMessageId = conversation.MostRecentMessageId,
                Participants = conversation.Participants.Select(p => p),
                Subject = conversation.Subject,
                ThreadingInfo = conversation.ThreadingInfo
            };
        }


        private FlattenedMessage FlattenMessage(Conversation conversation, Message message)
        {
            return new FlattenedMessage
            {
                // Conversation
                ConversationId = conversation.Id,
                ConversationSubject = conversation.Subject,
                ConversationThreadingInfo = conversation.ThreadingInfo,
                HasUnreadMessages = conversation.HasUnreadMessages,
                IsConversationMuted = conversation.IsConversationMuted,
                ItemKind = conversation.ItemKind,
                MostRecentMessageId = conversation.MostRecentMessageId,
                Participants = conversation.Participants,
                CanModifyParticipants = conversation.CanModifyParticipants,
                // Message
                Body = message.BodyUTF8,
                EstimatedDownloadSize = message.EstimatedDownloadSize,
                From = message.From,
                IsAutoReply = message.IsAutoReply,
                IsForwardingDisabled = message.IsForwardingDisabled,
                IsIncoming = message.IsIncoming,
                IsRead = message.IsRead,
                IsReceivedDuringQuietHours = message.IsReceivedDuringQuietHours,
                IsReplyDisabled = message.IsReplyDisabled,
                IsSeen = message.IsSeen,
                IsSimMessage = message.IsSimMessage,
                LocalTimestamp = message.LocalTimestamp,
                MessageAttachments = message.Attachments,
                MessageId = message.Id,
                MessageKind = message.MessageKind,
                MessageOperatorKind = message.MessageOperatorKind,
                MessageSubject = message.Subject,
                MessageThreadingInfo = message.ThreadingInfo,
                NetworkTimestamp = message.NetworkTimestamp,
                Recipients = message.Recipients,
                RecipientsDeliveryInfos = message.RecipientsDeliveryInfos,
                RecipientSendStatuses = message.RecipientSendStatuses,
                RemoteId = message.RemoteId,
                ShouldSuppressNotification = message.ShouldSuppressNotification,
                Status = message.Status,
                TransportFriendlyName = message.TransportFriendlyName,
                TransportId = message.TransportId,
            };
        }
    }
}