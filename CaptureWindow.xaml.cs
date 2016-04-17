using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Reactive.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Point = System.Windows.Point;

namespace fazo_client_cs
{
    public partial class CaptureWindow
    {
        private readonly Conf _conf;

        public CaptureWindow(Conf conf)
        {
            InitializeComponent();

            _conf = conf;

            Cursor = Cursors.Cross;
            var origin = new Point();

            var mouseDown = Observable.FromEventPattern<MouseEventArgs>(this, "MouseLeftButtonDown");
            var mouseMove = Observable.FromEventPattern<MouseEventArgs>(this, "MouseMove");
            var mouseUp = Observable.FromEventPattern<MouseEventArgs>(this, "MouseLeftButtonUp");
            var keyDown = Observable.FromEventPattern<KeyEventArgs>(this, "KeyDown")
                .Where(x => x.EventArgs.Key == Key.Escape);

            keyDown.Subscribe(x =>
            {
                Cursor = Cursors.Arrow;
                Close();
            });

            mouseDown
                .Do(e => { origin = e.EventArgs.GetPosition(LayoutRoot); })
                .SelectMany(mouseMove)
                .TakeUntil(mouseUp)
                .Do(e =>
                {
                    var rect = BoundsRect(origin.X, origin.Y, e.EventArgs.GetPosition(LayoutRoot).X,
                        e.EventArgs.GetPosition(LayoutRoot).Y);
                    SelectionRect.Margin = new Thickness(rect.Left, rect.Top, Width - rect.Right, Height - rect.Bottom);
                    SelectionRect.Width = rect.Width;
                    SelectionRect.Height = rect.Height;
                })
                .LastAsync()
                .Subscribe(e =>
                {
                    Hide();

                    CaptureScreen(
                        Rect.Offset(
                            BoundsRect(origin.X, origin.Y, e.EventArgs.GetPosition(LayoutRoot).X,
                                e.EventArgs.GetPosition(LayoutRoot).Y), Left, Top));

                    Cursor = Cursors.Arrow;
                    Close();
                });
        }

        public void CaptureScreen(Rect rect)
        {
            using (var bmp = new Bitmap((int) rect.Width, (int) rect.Height, PixelFormat.Format32bppArgb))
            {
                using (var bmpGraphics = Graphics.FromImage(bmp))
                {
                    bmpGraphics.CopyFromScreen((int) rect.X, (int) rect.Y, 0, 0, bmp.Size);

                    var fileName = "fazo_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".png";

                    using (var ms = new MemoryStream())
                    {
                        bmp.Save(ms, ImageFormat.Png);
                        var img = ms.GetBuffer();

                        var url = _conf.Host;
                        var userName = Environment.UserName;

                        var wc = new WebClient();
                        wc.Headers.Add("userName:" + userName);
                        wc.Headers.Add("fileName:" + fileName);
                        var resData = wc.UploadData(url, img);
                        wc.Dispose();

                        var imageUrl = Encoding.UTF8.GetString(resData);
                        Clipboard.SetText("!" + imageUrl + "!");
                        Process.Start(imageUrl);
                    }
                }
            }
        }

        private static Rect BoundsRect(double left, double top, double right, double bottom)
        {
            return new Rect(Math.Min(left, right), Math.Min(top, bottom), Math.Abs(right - left), Math.Abs(bottom - top));
        }
    }
}