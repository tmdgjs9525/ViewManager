using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Chrome;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using System;

namespace Avalonia.Container.Test;

public partial class TestDialog : UserControl
{
    public TestDialog()
    {
        InitializeComponent();

        if (border != null)
        {
            // 2. �ش� ��Ʈ�ѿ� PointerPressed(���콺 ����) �̺�Ʈ�� �����մϴ�.
            border.PointerPressed += TitleBar_PointerPressed;
        }
    }

    private void TitleBar_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        var parentWindow = this.VisualRoot as Window;

        // 2. ã�� Window���� �޼��带 ȣ���մϴ�.
        parentWindow?.BeginMoveDrag(e);
    }
}