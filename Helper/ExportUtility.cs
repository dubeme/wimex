using Newtonsoft.Json;
using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WIMEX.Model;
using Windows.Storage;

namespace WIMEX.Helper
{
    public static class ExportUtility
    {
        private static async Task ExportConversations(IEnumerable<Conversation> conversations, StorageFolder destinationFolder, CancellationToken cancellationToken)
        {
            var exportFolderName = $"export-{DateTime.Now:yyyyMMddHHmmssfff}";
            var exportFolder = await destinationFolder.CreateFolderAsync(exportFolderName);
            var attachmentFolder = await exportFolder.CreateFolderAsync("attachments");

            var file = await exportFolder.CreateFileAsync($"conversations.csv");
            var conversationCSVStream = await file.OpenStreamForWriteAsync();

            CsvSerializer.SerializeToStream(conversations, conversationCSVStream);


            await FileIO.WriteTextAsync(
                await exportFolder.CreateFileAsync($"conversations.json"),
                await Task.Run(() => JsonConvert.SerializeObject(conversations), cancellationToken));

            await Task.WhenAll(conversations
                .Where(conversation => conversation.Attachments.Any())
                .SelectMany(conversation => conversation.Attachments)
                .Select(attachment => Task.Run(async () =>
                {
                    var attachmentFile = await attachmentFolder.CreateFileAsync($"{attachment.OriginalFileName}.{attachment.GuessedExtension}");

                    using (var fileStream = await attachmentFile.OpenStreamForWriteAsync())
                    {
                        await Attachment.WriteDataToStream(attachment, fileStream);
                    }
                })));

            await Task.Run(() =>
            {
                ZipFile.CreateFromDirectory(
                    exportFolder.Path,
                    $"{exportFolder.Path}.zip",
                    CompressionLevel.Optimal,
                    false);

                Directory.Delete(exportFolder.Path, true);
            });
        }
    }
}
