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
using WIMEX.Model;
using Windows.ApplicationModel.Chat;
using Windows.ApplicationModel.Contacts;
using Windows.ApplicationModel.Core;
using Windows.Storage;

namespace WIMEX.ViewModel
{
    public class AppHubViewModel : ViewModelBase
    {
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

        private bool _ExportInProgress;

        public bool ExportInProgress
        {
            get { return _ExportInProgress; }
            set
            {
                _ExportInProgress = value;
                RaisePropertyChanged(nameof(ExportInProgress));
            }
        }


        private bool _ExportLocationSet;
        public bool ExportLocationSet
        {
            get { return _ExportLocationSet; }
            set
            {
                _ExportLocationSet = value;
                RaisePropertyChanged(nameof(ExportLocationSet));
            }
        }

        public StorageFolder ExportFolder { get; set; }

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

        private ObservableCollection<ExportStepViewModel> _ExportSteps;

        public ObservableCollection<ExportStepViewModel> ExportSteps
        {
            get { return _ExportSteps; }
            set
            {
                _ExportSteps = value;
                RaisePropertyChanged(nameof(ExportSteps));
            }
        }

        public ICommand PickExportFolderCommand { get; set; }
        public ICommand StartExportingCommand { get; set; }
        public ICommand CancelExportingCommand { get; set; }

        public AppHubViewModel()
        {
            Init();
        }

        private void Init()
        {
            var tokenSource = new CancellationTokenSource();

            PickExportFolderCommand = new RelayCommand(async () =>
            {
                var folderPicker = new Windows.Storage.Pickers.FolderPicker();
                ExportFolder = await folderPicker.PickSingleFolderAsync();
                ExportLocationSet = ExportFolder?.Name?.Length > 0;
            });

            StartExportingCommand = new RelayCommand(async () =>
            {
                await GetMessages(tokenSource.Token);
            });

            CancelExportingCommand = new RelayCommand(() =>
            {
                tokenSource.Cancel();
            });
        }

        public async Task GetMessages(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested == false)
            {
                try
                {
                    this.ExportInProgress = true;

                    var store = (await ChatMessageManager.RequestStoreAsync());
                    var convoReader = store.GetConversationReader();
                    var chatConversations = (await convoReader.ReadBatchAsync(int.MaxValue));

                    this.CurrentConverstaionIndex = 0;
                    this.CurrentMessageIndex = 0;
                    this.NumberOfConversations = chatConversations.Count;

                    var contactStore = await ContactManager.RequestStoreAsync();
                    var clBatch = (await contactStore.FindContactListsAsync()).ToList();
                    var cBatch = (await contactStore.FindContactsAsync()).ToList();
                    var cJSON = await Task.Run(() => JsonConvert.SerializeObject(cBatch), cancellationToken);

                    //Conversations = await ChatConversationsToConversationViewModelsAsync(chatConversations, cancellationToken);

                    // FontAwesome.UWP.FontAwesomeIcon.Check

                    var conversations = await Task.WhenAll(chatConversations.Select((convo) => Task.Run(
                        async () =>
                        {
                            RunOnUIThread(() => this.CurrentConverstaionIndex++);

                            var messages = await convo.GetMessageReader().ReadBatchAsync(int.MaxValue);

                            RunOnUIThread(() => this.NumberOfMessages += messages.Count);
                            RunOnUIThread(() => this.CurrentMessageIndex++);

                            var conversation = (Conversation)convo;
                            conversation.Messages = messages.Distinct(new Message()).Select(msg => (Message)msg);

                            return conversation;
                        }, cancellationToken)));

                    await WriteBackup(conversations, cancellationToken);
                }
                catch(FileLoadException ex)
                {
                    if (System.Diagnostics.Debugger.IsAttached)
                    {
                        System.Diagnostics.Debugger.Break();
                    }
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
                    this.ExportInProgress = false;
                }
            }
        }

        private static async Task<ObservableCollection<ConversationViewModel>> ChatConversationsToConversationViewModelsAsync(IReadOnlyList<ChatConversation> chatConversations, CancellationToken cancellationToken)
        {
            var contactStore = await ContactManager.RequestStoreAsync();

            return new ObservableCollection<ConversationViewModel>(await Task.WhenAll(
                chatConversations
                .Select(convo => Task.Run(async () =>
                {
                    var allParticipants = await Task.WhenAll(convo.Participants.Select(participant => Task.Run(async () =>
                    {
                        try
                        {
                            return (await contactStore.FindContactsAsync(participant)).First().FullName;
                        }
                        catch (Exception)
                        {
                        }

                        return participant;
                    }, cancellationToken)));

                    return new ConversationViewModel
                    {
                        Conversation = convo,
                        AllParticipants = string.Join(", ", allParticipants)
                    };
                }))));
        }

        private async Task WriteBackup(IEnumerable<Conversation> conversations, CancellationToken cancellationToken)
        {
            var exportFolderName = $"backup-{DateTime.Now:yyyyMMddHHmmssfff}";
            var exportFolder = await ExportFolder.CreateFolderAsync(exportFolderName);
            var attachmentFolder = await exportFolder.CreateFolderAsync("attachments");

            await FileIO.WriteTextAsync(
                await exportFolder.CreateFileAsync($"conversations.csv"),
                await Task.Run(() =>
                    ServiceStack.Text.CsvSerializer.SerializeToCsv(conversations), cancellationToken));
            
            await FileIO.WriteTextAsync(
                await exportFolder.CreateFileAsync($"messages.csv"),
                await Task.Run(() =>
                    ServiceStack.Text.CsvSerializer.SerializeToCsv(conversations.SelectMany(convo => convo.FlattenedMessages)), cancellationToken));


            await FileIO.WriteTextAsync(
                await exportFolder.CreateFileAsync($"attachments.csv"),
                await Task.Run(() =>
                    ServiceStack.Text.CsvSerializer.SerializeToCsv(conversations.SelectMany(conversation => conversation.Attachments)), cancellationToken));


            

            //await Task.WhenAll(conversations
            //    .Where(conversation => conversation.Attachments.Any())
            //    .SelectMany(conversation => conversation.Attachments)
            //    .Select(attachment => Task.Run(async () =>
            //{
            //    var attachmentFile = await attachmentFolder.CreateFileAsync($"{attachment.DataGUID}.{attachment.GuessedExtension}");

            //    using (var fileStream = await attachmentFile.OpenStreamForWriteAsync())
            //    {
            //        await Attachment.WriteDataToStream(attachment, fileStream);
            //    }
            //})));

            //await Task.Run(() =>
            //{
            //    ZipFile.CreateFromDirectory(
            //        backupFolder.Path,
            //        $"{backupFolder.Path}.zip",
            //        CompressionLevel.Optimal,
            //        false);

            //    Directory.Delete(backupFolder.Path, true);
            //});
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