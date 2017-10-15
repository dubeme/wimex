using Newtonsoft.Json;
using System;
using System.IO;
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
        /// Gets or sets the base64 encoded data unique identifier of this Attachment.
        /// </summary>
        [JsonProperty(PropertyName = "a")]
        public Guid DataGUID { get; set; }

        /// <summary>
        /// Gets or sets the base64 encoded thumbnail unique identifier of this Attachment.
        /// </summary>
        [JsonProperty(PropertyName = "b")]
        public Guid ThumbnailGUID { get; set; }

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
        public Attachment(ChatMessageAttachment attachment)
        {
            GroupId = attachment.GroupId;
            MimeType = attachment.MimeType;
            OriginalFileName = attachment.OriginalFileName;
            Text = attachment.Text;
            TransferProgress = attachment.TransferProgress;

            DataGUID = Guid.NewGuid();
            ThumbnailGUID = Guid.Empty;

            _Attachment = attachment;
        }

        /// <summary>
        /// Parse a <see cref="ChatMessageAttachment" /> into <see cref="Attachment" />.
        /// </summary>
        /// <param name="attachment">The attachment.</param>
        /// <returns></returns>
        public static Attachment Parse(ChatMessageAttachment attachment)
        {
            return new Attachment(attachment);
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