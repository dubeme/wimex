using GalaSoft.MvvmLight;
using Windows.ApplicationModel.Chat;

namespace WIMEX.ViewModel
{
    public class ConversationViewModel : ObservableObject
    {
        private bool _IsSelected;

        public bool IsSelected
        {
            get { return _IsSelected; }
            set
            {
                _IsSelected = value;
                RaisePropertyChanged(nameof(IsSelected));
            }
        }

        private string _AllParticipants;

        public string AllParticipants
        {
            get { return _AllParticipants; }
            set
            {
                _AllParticipants = value;
                RaisePropertyChanged(nameof(AllParticipants));
            }
        }

        private ChatConversation _Conversation;

        public ChatConversation Conversation
        {
            get { return _Conversation; }
            set
            {
                _Conversation = value;
                RaisePropertyChanged(nameof(Conversation));
            }
        }
    }
}