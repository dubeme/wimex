using WIMEX.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel.Chat;
using Windows.ApplicationModel.Contacts;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.Resources;
using Windows.Storage;

namespace WIMEX.ViewModel
{
    public class AppHubViewModel : ViewModelBase
    {
        private static ResourceLoader ResourceLoader = new ResourceLoader();

        private int _CurrentConverstaionIndex;

        public int CurrentConverstaionIndex
        {
            get { return _CurrentConverstaionIndex; }
            set
            {
                _CurrentConverstaionIndex = value;
                RaisePropertyChanged(nameof(CurrentConverstaionIndex));
            }
        }

        private int _NumberOfConversations;

        public int NumberOfConversations
        {
            get { return _NumberOfConversations; }
            set
            {
                _NumberOfConversations = value;
                RaisePropertyChanged(nameof(NumberOfConversations));
            }
        }

        private int _CurrentMessageIndex;

        public int CurrentMessageIndex
        {
            get { return _CurrentMessageIndex; }
            set
            {
                _CurrentMessageIndex = value;
                RaisePropertyChanged(nameof(CurrentMessageIndex));
            }
        }

        private int _NumberOfMessages;

        public int NumberOfMessages
        {
            get { return _NumberOfMessages; }
            set
            {
                _NumberOfMessages = value;
                RaisePropertyChanged(nameof(NumberOfMessages));
            }
        }

        private bool _BackupInProgress;

        public bool BackupInProgress
        {
            get { return _BackupInProgress; }
            set
            {
                _BackupInProgress = value;
                RaisePropertyChanged(nameof(BackupInProgress));
            }
        }

        private ObservableCollection<ConversationViewModel> _Conversations;

        public ObservableCollection<ConversationViewModel> Conversations
        {
            get { return _Conversations; }
            set
            {
                _Conversations = value;
                RaisePropertyChanged(nameof(Conversations));
            }
        }

        public ICommand StartBackupCommand { get; set; }
        public ICommand CancelBackupCommand { get; set; }

        public AppHubViewModel()
        {
            Init();
        }

        private void Init()
        {
            var loader = new ResourceLoader();
            var str = loader.GetString("discovering");

            var tokenSource = new CancellationTokenSource();
            StartBackupCommand = new RelayCommand(async () =>
            {
                await GetMessages(tokenSource.Token);
            });
            CancelBackupCommand = new RelayCommand(() => { });
        }

        public async Task GetMessages(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested == false)
            {
                var store = (await ChatMessageManager.RequestStoreAsync());
                var convoReader = store.GetConversationReader();
                var chatConversations = (await convoReader.ReadBatchAsync(int.MaxValue));

                try
                {
                    this.BackupInProgress = true;

                    this.CurrentConverstaionIndex = 0;
                    this.CurrentMessageIndex = 0;
                    this.NumberOfConversations = chatConversations.Count;

                    var contactStore = await ContactManager.RequestStoreAsync();
                    var clBatch = (await contactStore.FindContactListsAsync()).ToList();
                    var cBatch = (await contactStore.FindContactsAsync()).ToList();
                    var cJSON = await Task.Run(() => JsonConvert.SerializeObject(cBatch), cancellationToken);

                    Conversations = new ObservableCollection<ConversationViewModel>(await Task.WhenAll(
                        chatConversations
                        .Select(convo => Task.Run(async () =>
                       {
                           var allParticipants = await Task.WhenAll(convo.Participants.Select(part =>
                               Task.Run(async () =>
                               {
                                   try
                                   {
                                       return (await contactStore.FindContactsAsync(part)).First().FullName;
                                   }
                                   catch (Exception)
                                   {
                                   }

                                   return part;
                               })));

                           return new ConversationViewModel
                           {
                               Conversation = convo,
                               AllParticipants = string.Join(", ", allParticipants)
                           };
                       }))));

                    //var conversations = await Task.WhenAll(chatConversations.Select((convo) => Task.Run(
                    //    async () =>
                    //    {
                    //        RunOnUIThread(() => this.CurrentConverstaionIndex++);

                    //        var messages = await convo.GetMessageReader().ReadBatchAsync(int.MaxValue);

                    //        RunOnUIThread(() => this.NumberOfMessages += messages.Count);
                    //        RunOnUIThread(() => this.CurrentMessageIndex++);

                    //        var conversation = (Conversation)convo;
                    //        conversation.Messages = messages.Distinct(new Message()).Select(msg => (Message)msg);

                    //        return conversation;
                    //    }, cancellationToken)));

                    //await WriteBackup(conversations, cancellationToken);
                }
                catch (Exception ex)
                {
                    if (System.Diagnostics.Debugger.IsAttached)
                    {
                        System.Diagnostics.Debugger.Break();
                    }
                    else
                    {
                        throw;
                    }
                }
                finally
                {
                    this.BackupInProgress = false;
                }
            }
        }

        private async Task WriteBackup(IEnumerable<Conversation> conversations, CancellationToken cancellationToken)
        {
            var folderPicker = new Windows.Storage.Pickers.FolderPicker();
            var pickedFolder = await folderPicker.PickSingleFolderAsync();

            var backupFolderName = $"backup-{DateTime.Now:yyyyMMddHHmmssfff}";
            var backupFolder = await pickedFolder.CreateFolderAsync(backupFolderName);
            var attachmentFolder = await backupFolder.CreateFolderAsync("attachments");

            await FileIO.WriteTextAsync(
                await backupFolder.CreateFileAsync($"conversations.json"),
                await Task.Run(() => JsonConvert.SerializeObject(conversations), cancellationToken));

            await Task.WhenAll(conversations
                .Where(conversation => conversation.Attachments.Any())
                .SelectMany(conversation => conversation.Attachments)
                .Select(attachment => Task.Run(async () =>
            {
                var attachmentFile = await attachmentFolder.CreateFileAsync($"{attachment.DataGUID}.{attachment.GuessedExtension}");

                using (var fileStream = await attachmentFile.OpenStreamForWriteAsync())
                {
                    await Attachment.WriteDataToStream(attachment, fileStream);
                }
            })));

            await Task.Run(() =>
            {
                ZipFile.CreateFromDirectory(
                    backupFolder.Path,
                    $"{backupFolder.Path}.zip",
                    CompressionLevel.Optimal,
                    false);

                Directory.Delete(backupFolder.Path, true);
            });
        }

        private async static void RunOnUIThread(Action action)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                action();
            });
        }

        private void Log(object obj)
        {
            System.Diagnostics.Debug.WriteLine(obj);
        }
    }
}