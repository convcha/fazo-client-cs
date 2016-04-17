using System;
using System.Windows.Forms;
using System.Windows.Input;
using Gma.System.MouseKeyHook;
using Hardcodet.Wpf.TaskbarNotification;
using KeyEventArgs = System.Windows.Forms.KeyEventArgs;

namespace fazo_client_cs
{
    public partial class MainWindow
    {
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
            if (e.KeyCode == Keys.E && e.Modifiers == Keys.Control)
            {
                var window = new CaptureWindow();
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