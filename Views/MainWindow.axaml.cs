using System;
using apr.ViewModels;
using Avalonia.Controls;

namespace apr.Views;

public partial class MainWindow : Window
{
    private readonly ViewModels.MainWindowViewModel _vm = new();

    public MainWindow()
    {
        InitializeComponent();
    }

    // 버튼 클릭 이벤트 핸들러
    private async void OnPickImageClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        CopyResult ret = await _vm.OpenAndSaveImageAsync(this);

        if (ret.message is not null)
        {
            Console.WriteLine(ret.message);
        }

        if (ret.path is not null)
        {
            Console.WriteLine(ret.path);
        }
    }
}