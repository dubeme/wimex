using GalaSoft.MvvmLight;

namespace WIMEX.ViewModel
{
    public class ExportStepViewModel : ViewModelBase
    {
        private bool _InProgress;

        public bool InProgress
        {
            get { return _InProgress; }
            set
            {
                _InProgress = value;
                RaisePropertyChanged(nameof(InProgress));
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