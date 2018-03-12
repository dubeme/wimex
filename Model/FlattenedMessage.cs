using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Chat;

namespace WIMEX.Model
{
    public class FlattenedMessage
    {
        // Conversation
        public bool CanModifyParticipants { get; set; }
        public bool HasUnreadMessages { get; set; }
        public bool IsConversationMuted { get; set; }
        public ChatItemKind ItemKind { get; set; }
        public ConversationThreadingInfo ConversationThreadingInfo { get; set; }
        public IEnumerable<string> Participants { get; set; }
        public string ConversationId { get; set; }
        public string ConversationSubject { get; set; }
        public string MostRecentMessageId { get; set; }

        // Message
        public bool IsAutoReply { get; set; }
        public bool IsForwardingDisabled { get; set; }
        public bool IsIncoming { get; set; }
        public bool IsRead { get; set; }
        public bool IsReceivedDuringQuietHours { get; set; }
        public bool IsReplyDisabled { get; set; }
        public bool IsSeen { get; set; }
        public bool IsSimMessage { get; set; }
        public bool ShouldSuppressNotification { get; set; }
        public ChatMessageKind MessageKind { get; set; }
        public ChatMessageOperatorKind MessageOperatorKind { get; set; }
        public ChatMessageStatus Status { get; set; }
        public ConversationThreadingInfo MessageThreadingInfo { get; set; }
        public DateTimeOffset LocalTimestamp { get; set; }
        public DateTimeOffset NetworkTimestamp { get; set; }
        public IEnumerable<Attachment> MessageAttachments { get; set; }
        public IList<ChatRecipientDeliveryInfo> RecipientsDeliveryInfos { get; set; }
        public IList<string> Recipients { get; set; }
        public IReadOnlyDictionary<string, ChatMessageStatus> RecipientSendStatuses { get; set; }
        public string Body { get; set; }
        public string From { get; set; }
        public string MessageId { get; set; }
        public string MessageSubject { get; set; }
        public string RemoteId { get; set; }
        public string TransportFriendlyName { get; set; }
        public string TransportId { get; set; }
        public ulong EstimatedDownloadSize { get; set; }
    }
}