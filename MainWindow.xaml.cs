using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;
using Gma.System.MouseKeyHook;
using Hardcodet.Wpf.TaskbarNotification;
using Newtonsoft.Json;
using KeyEventArgs = System.Windows.Forms.KeyEventArgs;

namespace fazo_client_cs
{
    public partial class MainWindow
    {
        private readonly Conf _conf = JsonConvert.DeserializeObject<Conf>(File.ReadAllText(@".\conf.json"));

        public MainWindow()
        {
            InitializeComponent();

            var tbi = new TaskbarIcon
            {
                Icon = Properties.Resources.Icon,
                ToolTipText = "Fazo",
                LeftClickCommand = new ExitCommand(),
                LeftClickCommandParameter = this
            };

            var mGlobalHook = Hook.GlobalEvents();
            mGlobalHook.KeyDown += M_GlobalHook_KeyDown;

            Hide();
        }

        private void M_GlobalHook_KeyDown(object sender, KeyEventArgs e)
        {
            var key = _conf.Shortcut.Key;
            var modifiers = _conf.Shortcut.Modifiers;

            var isModifiersPushed = modifiers
                .All(m => e.Modifiers.HasFlag(m));

            if (e.KeyCode == key && isModifiersPushed)
            {
                var window = new CaptureWindow(_conf);
                window.ShowDialog();
                window.Activate();
            }
        }
    }

    public class ExitCommand : ICommand
    {
        public void Execute(object parameter)
        {
            ((MainWindow) parameter).Close();
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;
    }
}