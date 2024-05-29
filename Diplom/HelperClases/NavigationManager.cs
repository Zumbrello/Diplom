using System;
using Avalonia.Controls;

namespace Diplom;

public class NavigationManager
{
    private static ContentControl? ContentControl;

    public static void Initialize(ContentControl _contentControl)
    {
        ContentControl = _contentControl ?? throw new ArgumentNullException(nameof(_contentControl));
    }

    public static void NavigateTo(UserControl userControl)
    {
        if (ContentControl == null)
            throw new InvalidOperationException("ContentControl is not initialized.");

        ContentControl.Content = userControl;
    }
}