using Wpf.Container.Extensions;
using CommunityToolkit.Mvvm.DependencyInjection;
using System.Runtime.InteropServices;
using Wpf.Container.Extensions;
using EyeContainer.Core.Interfaces;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;

namespace Eye.Base.Dialog
{
    /// <summary>
    /// DialogBase.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class DialogBase : Window
    {
        private bool _positioned = false;
        private readonly StartPosition? _startPosition;

        #region Win32 Monitor API

        [DllImport("user32.dll")]
        private static extern IntPtr MonitorFromWindow(IntPtr hwnd, int dwFlags);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

        private const int MONITOR_DEFAULTTONEAREST = 2;

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct MONITORINFO
        {
            public int cbSize;
            public RECT rcMonitor; // 전체 모니터 영역
            public RECT rcWork;    // 작업 영역 (작업표시줄 제외)
            public int dwFlags;
        }

        #endregion

        public DialogBase(StartPosition? startPosition = null)
        {
            InitializeComponent();

            Owner = WindowUtil.GetActiveWindow();
            WindowStartupLocation = WindowStartupLocation.CenterOwner;

            _startPosition = startPosition;
        }


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Control content = dialogContent.Content as Control ?? throw new ArgumentNullException("ContentControls Content is not UserControl. should be UserControl");
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            if (!_positioned && _startPosition is not null)
            {
                if (_startPosition == StartPosition.Center)
                    CenterToOwner();           // 주모니터 고정 금지, 오너 기준
                else
                    PositionFromCursor(_startPosition.Value);

                _positioned = true;
            }

            Opacity = 1;
        }

        private void CenterToOwner()
        {
            //기준 owner 찾기
            var owner = Owner ?? Application.Current.MainWindow;
            if (owner is null || ReferenceEquals(owner, this))
                return;

            //다이얼로그 크기 계산
            double dialogWidth = ActualWidth <= 0 || double.IsNaN(ActualWidth) ? Width : ActualWidth;
            double dialogHeight = ActualHeight <= 0 || double.IsNaN(ActualHeight) ? Height : ActualHeight;

            if (dialogWidth <= 0) dialogWidth = MinWidth;
            if (dialogHeight <= 0) dialogHeight = MinHeight;

            //최대화 아닐 때 프로그램 중앙 배치
            if (owner.WindowState != WindowState.Maximized)
            {
                double ownerWidth = owner.ActualWidth > 0 ? owner.ActualWidth : owner.Width;
                double ownerHeight = owner.ActualHeight > 0 ? owner.ActualHeight : owner.Height;

                Left = owner.Left + (ownerWidth - dialogWidth) / 2;
                Top = owner.Top + (ownerHeight - dialogHeight) / 2;
                return;
            }

            //최대화일 때 모니터 중앙 위치
            var ownerHandle = new WindowInteropHelper(owner).Handle;
            if (ownerHandle == IntPtr.Zero)
            {
                // 핸들 못 구하면 그냥 owner 기준으로
                Left = owner.Left + (owner.ActualWidth - dialogWidth) / 2;
                Top = owner.Top + (owner.ActualHeight - dialogHeight) / 2;
                return;
            }

            IntPtr hMonitor = MonitorFromWindow(ownerHandle, MONITOR_DEFAULTTONEAREST);
            if (hMonitor == IntPtr.Zero)
            {
                // 모니터 정보 못 구하면 return
                Left = owner.Left + (owner.ActualWidth - dialogWidth) / 2;
                Top = owner.Top + (owner.ActualHeight - dialogHeight) / 2;
                return;
            }

            var mi = new MONITORINFO();
            mi.cbSize = Marshal.SizeOf(typeof(MONITORINFO));

            if (!GetMonitorInfo(hMonitor, ref mi))
            {                
                Left = owner.Left + (owner.ActualWidth - dialogWidth) / 2;
                Top = owner.Top + (owner.ActualHeight - dialogHeight) / 2;
                return;
            }

            // 작업영역(픽셀 좌표)
            var work = mi.rcWork;

            // 픽셀 - WPF 좌표 변환 (DPI 고려)
            var source = PresentationSource.FromVisual(owner);
            if (source?.CompositionTarget is null)
            {                
                double areaLeft = work.Left;
                double areaTop = work.Top;
                double areaWidth = work.Right - work.Left;
                double areaHeight = work.Bottom - work.Top;

                Left = areaLeft + (areaWidth - dialogWidth) / 2;
                Top = areaTop + (areaHeight - dialogHeight) / 2;
                return;
            }

            var transform = source.CompositionTarget.TransformFromDevice;
            var topLeft = transform.Transform(new Point(work.Left, work.Top));
            var bottomRight = transform.Transform(new Point(work.Right, work.Bottom));

            double monitorLeft = topLeft.X;
            double monitorTop = topLeft.Y;
            double monitorWidth = bottomRight.X - topLeft.X;
            double monitorHeight = bottomRight.Y - topLeft.Y;

            Left = monitorLeft + (monitorWidth - dialogWidth) / 2;
            Top = monitorTop + (monitorHeight - dialogHeight) / 2;
        }


        private void Before_CenterToOwner()
        {
            var owner = Owner ?? WindowUtil.GetActiveWindow() ?? Application.Current.MainWindow;

            if (owner is null || ReferenceEquals(owner, this))
                owner = Application.Current.MainWindow;

            if (Owner is null) return;

            var dialogWidth = ActualWidth;
            var dialogHeight = ActualHeight;
            double ownerLeft = 0;
            double ownerTop = 0;

            if (dialogWidth <= 0 || double.IsNaN(dialogWidth)) dialogWidth = Width;
            if (dialogHeight <= 0 || double.IsNaN(dialogHeight)) dialogHeight = Height;

            if (Owner.WindowState == WindowState.Maximized)
            {
                ownerLeft = 0;
                ownerTop = 0;
            }
            else
            {
                ownerLeft = Owner.Left;
                ownerTop = Owner.Top;
            }

            Left = ownerLeft + (Owner.ActualWidth - dialogWidth) / 2;
            Top = ownerTop + (Owner.ActualHeight - dialogHeight) / 2;
        }

        private void PositionFromCursor(StartPosition pos)
        {
            var (cursor, workArea) = MonitorUtil.GetCursorAndWorkAreaWpf(this);

            // 기본값: 가운데 스케일 anchor
            dialogContent.RenderTransformOrigin = new Point(0.5, 0.5);

            switch (pos)
            {
                case StartPosition.Left:
                    dialogContent.RenderTransformOrigin = new Point(0, 0.5);
                    Left = cursor.X;
                    Top = cursor.Y - Height / 2;
                    break;

                case StartPosition.Right:
                    dialogContent.RenderTransformOrigin = new Point(1, 0.5);
                    Left = cursor.X - Width;
                    Top = cursor.Y - Height / 2;
                    break;

                case StartPosition.Top:
                    dialogContent.RenderTransformOrigin = new Point(0.5, 0);
                    Left = cursor.X - Width / 2;
                    Top = cursor.Y;
                    break;

                case StartPosition.Bottom:
                    dialogContent.RenderTransformOrigin = new Point(0.5, 1);
                    Left = cursor.X - Width / 2;
                    Top = cursor.Y - Height;
                    break;

                case StartPosition.LeftUp:
                    dialogContent.RenderTransformOrigin = new Point(0, 0);
                    Left = cursor.X;
                    Top = cursor.Y;
                    break;

                case StartPosition.RightUp:
                    dialogContent.RenderTransformOrigin = new Point(1, 0);
                    Left = cursor.X - Width;
                    Top = cursor.Y;
                    break;

                case StartPosition.LeftDown:
                    dialogContent.RenderTransformOrigin = new Point(0, 1);
                    Left = cursor.X;
                    Top = cursor.Y - Height;
                    break;

                case StartPosition.RightDown:
                    dialogContent.RenderTransformOrigin = new Point(1, 1);
                    Left = cursor.X - Width;
                    Top = cursor.Y - Height;
                    break;

                default:
                    CenterToOwner();
                    break;
            }

            // 해당 모니터 워킹영역 안으로 클램프
            Left = Math.Min(Math.Max(Left, workArea.Left), workArea.Right - Width);
            Top = Math.Min(Math.Max(Top, workArea.Top), workArea.Bottom - Height);
        }

        public void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MaxiMizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
            else
            {
                WindowState = WindowState.Maximized;
            }
        }

        private void AnimateWindowSize(double targetWidth, double targetHeight)
        {
            var scaleXAnim = new DoubleAnimation
            {
                From = 0.5,
                To = 1.0,
                Duration = TimeSpan.FromSeconds(0.3),
                EasingFunction = new CircleEase { EasingMode = EasingMode.EaseOut }
            };

            var scaleYAnim = new DoubleAnimation
            {
                From = 0.5,
                To = 1.0,
                Duration = TimeSpan.FromSeconds(0.3),
                EasingFunction = new CircleEase { EasingMode = EasingMode.EaseOut }
            };

            var transform = dialogContent.RenderTransform as ScaleTransform;
            transform?.BeginAnimation(ScaleTransform.ScaleXProperty, scaleXAnim);
            transform?.BeginAnimation(ScaleTransform.ScaleYProperty, scaleYAnim);
        }


    }
}
