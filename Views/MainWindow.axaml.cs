using System;
using apr.ViewModels;
using System.ComponentModel;
using System.IO;
using Avalonia.Controls;
using System.Threading.Tasks;

namespace apr.Views;

public partial class MainWindow : Window
{
    private readonly MainWindowViewModel _vm = new();
    private readonly NoseOverlayService _ns = new();

    private string? imageServiceDir = "";
    public MainWindow(string[] args)
    {
        InitializeComponent();
        DataContext = _vm;

        _vm.PropertyChanged += VmOnPropertyChanged;

        if (args.Length > 0)
        {
            imageServiceDir = args[0];
            // ViewModel 등에 전달 가능
        }
        else
        {
            string baseDir = AppContext.BaseDirectory;
            imageServiceDir = Path.Combine(baseDir, "ImageService");
        }

        Console.WriteLine("IMAGESERVICE_DIR: " + imageServiceDir);
    }

    private void VmOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MainWindowViewModel.SelectedImagePath))
        {
            // 예: UI에 직접 접근하여 값 적용
            var textBlock = this.FindControl<TextBlock>("TextBlock");
            if (textBlock is not null)
            {
                textBlock.IsVisible = (_vm.SelectedImagePath == null);
            }

            var image = this.FindControl<Image>("Image");
            if (image is not null && _vm.SelectedImagePath is not null)
            {
                var localPath = new Uri(_vm.SelectedImagePath).LocalPath;
                image.Source = new Avalonia.Media.Imaging.Bitmap(localPath);
            }
        }

        if (e.PropertyName == nameof(MainWindowViewModel.Message))
        {
            var messageBlock = this.FindControl<TextBlock>("MessageBlock");
            if (messageBlock is not null)
            {
                messageBlock.Text = _vm.Message;
            }
        }

        if (e.PropertyName == nameof(MainWindowViewModel.Loding))
        {
            var loding = this.FindControl<Border>("loding");
            if (loding is not null)
            {
                loding.IsVisible = _vm.Loding;
            }
        }
    }

    // 버튼 클릭 이벤트 핸들러
    private async void OnPickImageClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(imageServiceDir))
        {
            Console.WriteLine("IMAGESERVICE_DIR is null");
            return;
        }

        Result cpRet = await _vm.OpenAndSaveImageAsync(this, imageServiceDir);

        if (cpRet.message is not null)
        {
            _vm.Message = cpRet.message;
            return;
        }

        if (cpRet.path is not null)
        {
            await ConvertImage(cpRet.path, imageServiceDir);
        }
    }

    private async Task<bool> ConvertImage(string imagePath, string imageServiceDir)
    {
        _vm.Loding = true;
        Result ret = await _ns.AddRudolphNoseAsync(imagePath, imageServiceDir);
        if (ret.message is not null)
        {
            _vm.Message = ret.message;
        }

        if (ret.path is not null)
        {
            _vm.SelectedImagePath = new Uri(ret.path).AbsoluteUri;
        }
        _vm.Loding = false;

        return string.IsNullOrEmpty(ret.message);
    }
}