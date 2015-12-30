using Windows.UI.Xaml.Controls;
using Pospa.NET.TurrisGadgets;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TG_Manager
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            TurrisDongle dongle = new TurrisDongle();
            dongle.MessageReceived += Dongle_MessageReceived;
            dongle.Initialize(true);
        }

        private void Dongle_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            string message = e.Message;
        }
    }
}