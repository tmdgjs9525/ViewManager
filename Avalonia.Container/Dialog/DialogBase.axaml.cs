using Avalonia.Container.Util;
using EyeContainer.Core.Interfaces;
using Avalonia;
using Avalonia.Animation; // Animation, KeyFrame
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input; // PointerPressedEventArgs
using Avalonia.Interactivity;
using Avalonia.Media; // ScaleTransform
using Avalonia.Styling;

namespace Avalonia.Container;

public partial class DialogBase : Window
{
    private bool _positioned = false;
    private readonly StartPosition? _startPosition;
    public DialogBase(StartPosition? startPosition = null)
    {
        InitializeComponent();

        WindowStartupLocation = WindowStartupLocation.CenterOwner;

        _startPosition = startPosition;
    }


}