using Newtonsoft.Json;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Chat;

namespace WIMEX.Model
{
    /// <summary>
    ///
    /// </summary>
    public class Attachment
    {
        private ChatMessageAttachment _Attachment;

        private string _Base64EncodedData;
        private string _Base64EncodedThumbnail;

        /// <summary>
        /// Get's the ID for this attachment
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Gets the guessed extension, from the Mime type.
        /// </summary>
        [JsonIgnore]
        public string GuessedExtension
        {
            get
            {
                return this.MimeType.Split('/')[1];
            }
        }

        /// <summary>
        /// Gets or sets the identifier for the attachment group to which this attachment belongs.
        /// </summary>
        [JsonProperty(PropertyName = "c")]
        public uint GroupId { get; set; }

        /// <summary>
        /// Gets or sets the MIME type of the attachment.
        /// </summary>
        /// <value>
        /// The type of the MIME.
        /// </value>
        [JsonProperty(PropertyName = "d")]
        public string MimeType { get; set; }

        /// <summary>
        /// Gets or sets the original file name of the attachment.
        /// </summary>
        /// <value>
        /// The name of the original file.
        /// </value>
        [JsonProperty(PropertyName = "e")]
        public string OriginalFileName { get; set; }

        /// <summary>
        /// Gets or sets the text encoded representation of the attachment object.
        /// </summary>
        [JsonProperty(PropertyName = "f")]
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the progress of transferring the attachment.
        /// </summary>
        [JsonProperty(PropertyName = "g")]
        public double TransferProgress { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Attachment"/> class.
        /// </summary>
        /// <param name="attachment">The attachment.</param>
        /// <param name="message">The message the attachment belongs to.</param>
        public Attachment(ChatMessageAttachment attachment, Message message)
        {
            GroupId = attachment.GroupId;
            MimeType = attachment.MimeType;
            OriginalFileName = attachment.OriginalFileName;
            Text = attachment.Text;
            TransferProgress = attachment.TransferProgress;
            Id = $"{message.Id}-{attachment.GroupId}-{attachment.MimeType}-{attachment.OriginalFileName}";

            _Attachment = attachment;
        }

        /// <summary>
        /// Parse a <see cref="ChatMessageAttachment" /> into <see cref="Attachment" />.
        /// </summary>
        /// <param name="attachment">The attachment.</param>
        /// <param name="message">The message the attachment belongs to.</param>
        /// <returns></returns>
        public static Attachment Parse(ChatMessageAttachment attachment, Message message)
        {
            return new Attachment(attachment, message);
        }

        private static string HashString(string str)
        {
            var uniqueNameHash = SHA256
                .Create()
                .ComputeHash(Encoding.UTF8.GetBytes(str));

            var uniqueNameHashString = new StringBuilder();

            foreach (byte b in uniqueNameHash)
            {
                uniqueNameHashString.Append(b);
            }

            return uniqueNameHashString.ToString();
        }

        public async static Task<string> EncodeDataToBase64Async(Attachment attachment)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var readStream = (await attachment._Attachment.DataStreamReference.OpenReadAsync()))
                {
                    if (!Equals(readStream, null))
                    {
                        using (var datastream = readStream.AsStreamForRead())
                        {
                            datastream.CopyTo(memoryStream);
                        }
                    }
                }

                return Convert.ToBase64String(memoryStream.ToArray());
            }
        }

        public async static Task<string> EncodeThumbnailToBase64Async(Attachment attachment)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var readStream = (await attachment._Attachment.Thumbnail.OpenReadAsync()))
                {
                    if (!Equals(readStream, null))
                    {
                        using (var datastream = readStream.AsStreamForRead())
                        {
                            datastream.CopyTo(memoryStream);
                        }
                    }
                }

                return Convert.ToBase64String(memoryStream.ToArray());
            }
        }

        public async static Task WriteDataToStream(Attachment attachment, Stream stream)
        {
            using (var readStream = (await attachment._Attachment.DataStreamReference.OpenReadAsync()))
            {
                if (!Equals(readStream, null))
                {
                    using (var datastream = readStream.AsStreamForRead())
                    {
                        await datastream.CopyToAsync(stream);
                    }
                }
            }
        }

        public async static Task WriteThumbnailToStream(Attachment attachment, Stream stream)
        {
            using (var readStream = (await attachment._Attachment.Thumbnail.OpenReadAsync()))
            {
                if (!Equals(readStream, null))
                {
                    using (var datastream = readStream.AsStreamForRead())
                    {
                        await datastream.CopyToAsync(stream);
                    }
                }
            }
        }
    }
}