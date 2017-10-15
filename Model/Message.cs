using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.Chat;

namespace WIMEX.Model
{
    /// <summary>
    /// Represents a chat message.
    /// </summary>
    public class Message : IEqualityComparer<Message>, IEqualityComparer<ChatMessage>
    {
        private const int THRESHOLD_SECONDS = 5;

        private IList<ChatMessageAttachment> _Attachments;

        /// <summary>
        /// Gets the attachments of this Message.
        /// </summary>
        [JsonProperty(PropertyName = "a")]
        public IEnumerable<Attachment> Attachments
        {
            get
            {
                if (!Equals(_Attachments, null))
                {
                    return _Attachments.Select(Attachment.Parse);
                }

                return Enumerable.Empty<Attachment>();
            }
        }

        /// <summary>
        /// Gets or sets the body of this Message.
        /// </summary>
        [JsonProperty(PropertyName = "b")]
        public string Body { get; set; }

        /// <summary>
        /// Gets or sets the size of the estimated download.
        /// </summary>
        [JsonProperty(PropertyName = "c")]
        public ulong EstimatedDownloadSize { get; set; }

        /// <summary>
        /// Gets or sets who this message is from.
        /// </summary>
        [JsonProperty(PropertyName = "d")]
        public string From { get; set; }

        /// <summary>
        /// Gets or sets the identifier of this Message.
        /// </summary>
        [JsonProperty(PropertyName = "e")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets if this message is automatic reply.
        /// </summary>
        [JsonProperty(PropertyName = "f")]
        public bool IsAutoReply { get; set; }

        /// <summary>
        /// Gets or sets if this message has forwarding disabled.
        /// </summary>
        [JsonProperty(PropertyName = "g")]
        public bool IsForwardingDisabled { get; set; }

        /// <summary>
        /// Gets or sets if this message is incoming.
        /// </summary>
        [JsonProperty(PropertyName = "h")]
        public bool IsIncoming { get; set; }

        /// <summary>
        /// Gets or sets if this message is read.
        /// </summary>
        [JsonProperty(PropertyName = "i")]
        public bool IsRead { get; set; }

        /// <summary>
        /// Gets or sets if this message was received during quiet hours.
        /// </summary>
        [JsonProperty(PropertyName = "j")]
        public bool IsReceivedDuringQuietHours { get; set; }

        /// <summary>
        /// Gets or sets if this message has reply disabled.
        /// </summary>
        [JsonProperty(PropertyName = "k")]
        public bool IsReplyDisabled { get; set; }

        /// <summary>
        /// Gets or sets if this message has seen.
        /// </summary>
        [JsonProperty(PropertyName = "l")]
        public bool IsSeen { get; set; }

        /// <summary>
        /// Gets or sets if this message is sim message.
        /// </summary>
        [JsonProperty(PropertyName = "m")]
        public bool IsSimMessage { get; set; }

        /// <summary>
        /// Gets or sets the local timestamp of this Message.
        /// </summary>
        [JsonProperty(PropertyName = "n")]
        public DateTimeOffset LocalTimestamp { get; set; }

        /// <summary>
        /// Gets or sets the kind of the message.
        /// </summary>
        [JsonProperty(PropertyName = "o")]
        public ChatMessageKind MessageKind { get; set; }

        /// <summary>
        /// Gets or sets the kind of the message operator.
        /// </summary>
        [JsonProperty(PropertyName = "p")]
        public ChatMessageOperatorKind MessageOperatorKind { get; set; }

        /// <summary>
        /// Gets or sets the network timestamp of this Message.
        /// </summary>
        [JsonProperty(PropertyName = "q")]
        public DateTimeOffset NetworkTimestamp { get; set; }

        /// <summary>
        /// Gets or sets the recipients of this Message.
        /// </summary>
        [JsonProperty(PropertyName = "r")]
        public IList<string> Recipients { get; set; }

        /// <summary>
        /// Gets or sets the recipients delivery infos of this Message.
        /// </summary>
        [JsonProperty(PropertyName = "s")]
        public IList<ChatRecipientDeliveryInfo> RecipientsDeliveryInfos { get; set; }

        /// <summary>
        /// Gets or sets the recipient send statuses of this Message.
        /// </summary>
        [JsonProperty(PropertyName = "t")]
        public IReadOnlyDictionary<string, ChatMessageStatus> RecipientSendStatuses { get; set; }

        /// <summary>
        /// Gets or sets the remote identifier of this Message.
        /// </summary>
        [JsonProperty(PropertyName = "u")]
        public string RemoteId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [should suppress notification].
        /// </summary>
        [JsonProperty(PropertyName = "v")]
        public bool ShouldSuppressNotification { get; set; }

        /// <summary>
        /// Gets or sets the status of this Message.
        /// </summary>
        [JsonProperty(PropertyName = "w")]
        public ChatMessageStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the subject of this Message.
        /// </summary>
        [JsonProperty(PropertyName = "x")]
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the threading information of this Message.
        /// </summary>
        [JsonProperty(PropertyName = "y")]
        public ConversationThreadingInfo ThreadingInfo { get; set; }

        /// <summary>
        /// Gets or sets the name of the transport friendly.
        /// </summary>
        [JsonProperty(PropertyName = "z")]
        public string TransportFriendlyName { get; set; }

        /// <summary>
        /// Gets or sets the transport identifier of this Message.
        /// </summary>
        [JsonProperty(PropertyName = "$")]
        public string TransportId { get; set; }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Windows.ApplicationModel.Chat.ChatMessage" /> to <see cref="Message" />.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator Message(Windows.ApplicationModel.Chat.ChatMessage message)
        {
            return new Message
            {
                _Attachments = message.Attachments,
                Body = message.Body,
                EstimatedDownloadSize = message.EstimatedDownloadSize,
                From = message.From,
                Id = message.Id,
                IsAutoReply = message.IsAutoReply,
                IsForwardingDisabled = message.IsForwardingDisabled,
                IsIncoming = message.IsIncoming,
                IsRead = message.IsRead,
                IsReceivedDuringQuietHours = message.IsReceivedDuringQuietHours,
                IsReplyDisabled = message.IsReplyDisabled,
                IsSeen = message.IsSeen,
                IsSimMessage = message.IsSimMessage,
                LocalTimestamp = message.LocalTimestamp,
                MessageKind = message.MessageKind,
                MessageOperatorKind = message.MessageOperatorKind,
                NetworkTimestamp = message.NetworkTimestamp,
                Recipients = message.Recipients,
                RecipientsDeliveryInfos = message.RecipientsDeliveryInfos,
                RecipientSendStatuses = message.RecipientSendStatuses,
                RemoteId = message.RemoteId,
                ShouldSuppressNotification = message.ShouldSuppressNotification,
                Status = message.Status,
                Subject = message.Subject,
                ThreadingInfo = message.ThreadingInfo,
                TransportFriendlyName = message.TransportFriendlyName,
                TransportId = message.TransportId
            };
        }

        public override string ToString()
        {
            return $"{this.Body}";
        }

        public override bool Equals(object obj)
        {
            return this.Equals(this, obj as Message);
        }

        public bool Equals(Message msg)
        {
            return this.Equals(this, msg);
        }

        public bool Equals(Message msg1, Message msg2)
        {
            if (msg1 == null && msg2 == null)
            {
                return true;
            }

            if (msg1 == null || msg2 == null)
            {
                return false;
            }

            return
                msg1.Body.Equals(msg2.Body) &&
                msg1.From.Equals(msg2.From) &&
                Math.Abs((msg1.LocalTimestamp - msg2.LocalTimestamp).TotalSeconds) <= THRESHOLD_SECONDS;
        }

        public bool Equals(ChatMessage msg1, ChatMessage msg2)
        {
            return this.Equals((Message)msg1, (Message)msg2);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                // http://www.aaronstannard.com/overriding-equality-in-dotnet/
                var hashCode = 13;

                hashCode = (hashCode * 397) ^ this.Body.GetHashCode();
                hashCode = (hashCode * 397) ^ this.From.GetHashCode();
                hashCode = (hashCode * 397) ^ this.LocalTimestamp.GetHashCode();

                return hashCode;
            }
        }

        public int GetHashCode(Message msg)
        {
            return msg.GetHashCode();
        }

        public int GetHashCode(ChatMessage msg)
        {
            return this.GetHashCode((Message)msg);
        }
    }
}