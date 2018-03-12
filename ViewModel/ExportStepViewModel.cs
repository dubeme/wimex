using GalaSoft.MvvmLight;
using WIMEX.Model;

namespace WIMEX.ViewModel
{
    public class ExportStepViewModel : ViewModelBase
    {
        private ExportStepState _State;

        public ExportStepState State
        {
            get { return _State; }
            set
            {
                _State = value;
                RaisePropertyChanged(nameof(State));
            }
        }

        private string _Label;

        public string Label
        {
            get { return _Label; }
            set
            {
                _Label = value;
                RaisePropertyChanged(nameof(Label));
            }
        }
    }
}