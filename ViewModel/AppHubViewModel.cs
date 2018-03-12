using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using WIMEX.Model;
using Windows.ApplicationModel.Chat;
using Windows.ApplicationModel.Core;
using Windows.Storage;

namespace WIMEX.ViewModel
{
    public class AppHubViewModel : ViewModelBase
    {
        private Dictionary<ExportStep, ExportStepViewModel> _ExportStepsDictionary;

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
                ResetExportSteps();
                await GetMessages(tokenSource.Token);
            });

            CancelExportingCommand = new RelayCommand(() =>
            {
                tokenSource.Cancel();
            });

            SetupExportSteps();
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

                    UpdateExportStepState(ExportStep.DiscoverConversations, ExportStepState.InProgess);

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

                    UpdateExportStepState(ExportStep.DiscoverConversations, ExportStepState.Done);

                    await WriteBackup(conversations, cancellationToken);
                }
                catch (FileLoadException ex)
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

        private async Task WriteBackup(IEnumerable<Conversation> conversations, CancellationToken cancellationToken)
        {
            var exportFolderName = $"backup-{DateTime.Now:yyyyMMddHHmmssfff}";
            var exportFolder = await ExportFolder.CreateFolderAsync(exportFolderName);
            var attachmentFolder = await exportFolder.CreateFolderAsync("attachments");

            UpdateExportStepState(ExportStep.StoreConversations, ExportStepState.InProgess);
            await FileIO.WriteTextAsync(
                await exportFolder.CreateFileAsync($"conversations.csv"),
                await Task.Run(() =>
                    ServiceStack.Text.CsvSerializer.SerializeToCsv(conversations), cancellationToken));
            UpdateExportStepState(ExportStep.StoreConversations, ExportStepState.Done);

            UpdateExportStepState(ExportStep.StoreMessages, ExportStepState.InProgess);
            await FileIO.WriteTextAsync(
                await exportFolder.CreateFileAsync($"messages.csv"),
                await Task.Run(() =>
                    ServiceStack.Text.CsvSerializer.SerializeToCsv(conversations.SelectMany(convo => convo.FlattenedMessages)), cancellationToken));
            UpdateExportStepState(ExportStep.StoreMessages, ExportStepState.Done);

            UpdateExportStepState(ExportStep.StoreAttachementsMetadata, ExportStepState.InProgess);
            await FileIO.WriteTextAsync(
                await exportFolder.CreateFileAsync($"attachments.csv"),
                await Task.Run(() =>
                    ServiceStack.Text.CsvSerializer.SerializeToCsv(conversations.SelectMany(conversation => conversation.Attachments)), cancellationToken));
            UpdateExportStepState(ExportStep.StoreAttachementsMetadata, ExportStepState.Done);

            UpdateExportStepState(ExportStep.StroeAttachmentFiles, ExportStepState.InProgess);
            await Task.WhenAll(conversations
                .Where(conversation => conversation.Attachments.Any())
                .SelectMany(conversation => conversation.Attachments)
                .Select(attachment => Task.Run(async () =>
                {
                    var attachmentFile = await attachmentFolder.CreateFileAsync($"{attachment.Id}.{attachment.GuessedExtension}");

                    using (var fileStream = await attachmentFile.OpenStreamForWriteAsync())
                    {
                        await Attachment.WriteDataToStream(attachment, fileStream);
                    }
                })));
            UpdateExportStepState(ExportStep.StroeAttachmentFiles, ExportStepState.Done);
        }

        private void SetupExportSteps()
        {
            _ExportStepsDictionary = new Dictionary<ExportStep, ExportStepViewModel>
            {
                {
                    ExportStep.DiscoverConversations,
                    new ExportStepViewModel
                    {
                        State = ExportStepState.Idle,
                        Label = "Discover conversations"
                    }
                },
                {
                    ExportStep.StoreConversations,
                    new ExportStepViewModel
                    {
                        State = ExportStepState.Idle,
                        Label = "Export conversations (conversation structure)"
                    }
                },
                {
                    ExportStep.StoreMessages,
                    new ExportStepViewModel
                    {
                        State = ExportStepState.Idle,
                        Label = "Export messages (within conversations)"
                    }
                },
                {
                    ExportStep.StoreAttachementsMetadata,
                    new ExportStepViewModel
                    {
                        State = ExportStepState.Idle,
                        Label = "Export Attachment Metadata"
                    }
                },
                {
                    ExportStep.StroeAttachmentFiles,
                    new ExportStepViewModel
                    {
                        State = ExportStepState.Idle,
                        Label = "Export Attachments (individual files/content)"
                    }
                }
            };

            ExportSteps = new ObservableCollection<ExportStepViewModel>(new ExportStepViewModel[]
            {
                _ExportStepsDictionary[ExportStep.DiscoverConversations],
                _ExportStepsDictionary[ExportStep.StoreConversations],
                _ExportStepsDictionary[ExportStep.StoreMessages],
                _ExportStepsDictionary[ExportStep.StoreAttachementsMetadata],
                _ExportStepsDictionary[ExportStep.StroeAttachmentFiles],
            });
        }

        private void ResetExportSteps()
        {
            _ExportStepsDictionary[ExportStep.DiscoverConversations].State = ExportStepState.Idle;
            _ExportStepsDictionary[ExportStep.StoreConversations].State = ExportStepState.Idle;
            _ExportStepsDictionary[ExportStep.StoreMessages].State = ExportStepState.Idle;
            _ExportStepsDictionary[ExportStep.StoreAttachementsMetadata].State = ExportStepState.Idle;
            _ExportStepsDictionary[ExportStep.StroeAttachmentFiles].State = ExportStepState.Idle;
        }

        private void UpdateExportStepState(ExportStep step, ExportStepState state)
        {
            _ExportStepsDictionary[step].State = state;
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