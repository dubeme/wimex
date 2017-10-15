using WIMEX.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using Windows.ApplicationModel.Chat;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace WIMEX.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AppHub : Page
    {
        public AppHub()
        {
            this.InitializeComponent();
        }

        public async static void GetMessages(string number)
        {
            var store = await ChatMessageManager.RequestStoreAsync();
            var conversations = (await store.GetConversationReader().ReadBatchAsync(int.MaxValue))
                .Where(conv => conv.Participants.Any(str => Regex.Replace(str, "[^\\d]", "").Contains(number))).ToArray();




            foreach (var ccc in (await store.GetConversationReader().ReadBatchAsync(int.MaxValue)))
            {

                //Conversation c = ccc;
                //string json0 = JsonConvert.SerializeObject(c);
                //string json1 = JsonConvert.SerializeObject(ccc);
                Debug.WriteLine(JsonConvert.SerializeObject(ccc));
            }


            // http://stackoverflow.com/q/33082835
            // https://msdn.microsoft.com/en-us/library/windows/apps/windows.storage.pickers.filesavepicker.aspx
            //var folderPicker = new Windows.Storage.Pickers.FolderPicker();
            //var folder = await folderPicker.PickSingleFolderAsync();

            const int columnWidth = -32;

            foreach (var conversation in conversations)
            {
                //Conversation c = conversation;
                //string json0 = JsonConvert.SerializeObject(c);
                //string json1 = JsonConvert.SerializeObject(conversation);

                var messages = await conversation.GetMessageReader().ReadBatchAsync(int.MaxValue);

                foreach (var message in messages)
                {
                    Message mm = message;
                    var json1 = JsonConvert.SerializeObject(mm);

                }
            }
        }
    }
}
