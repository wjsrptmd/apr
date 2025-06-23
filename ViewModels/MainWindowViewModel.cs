using Avalonia.Controls;
using Avalonia.Platform.Storage;

using System;
using System.IO;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace apr.ViewModels;

public class MainWindowViewModel : INotifyPropertyChanged
{
    private string? selectedImagePath;

    public string? SelectedImagePath
    {
        get => selectedImagePath;
        set
        {
            if (selectedImagePath != value)
            {
                selectedImagePath = value;
                OnPropertyChanged();
            }
        }
    }

    private string? message;

    public string? Message
    {
        get => message;
        set
        {
            if (message != value)
            {
                message = value;
                OnPropertyChanged();
            }
        }
    }

    private bool loding = false;

    public bool Loding
    {
        get => loding;
        set
        {
            if (loding != value)
            {
                loding = value;
                OnPropertyChanged();
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        if (PropertyChanged is not null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
        else
        {
            Console.WriteLine("PropertyChanged is null");
        }
    }

    public async Task<Result> SaveImageAsync(string iamgePath, string ImageServiceDir)
    {
        Result result = new Result();

        try
        {
            string targetDir = Path.Combine(ImageServiceDir, "Data");

            Directory.CreateDirectory(targetDir);

            string extension = Path.GetExtension(iamgePath);
            string fileName = $"input{extension}";
            string targetPath = Path.Combine(targetDir, fileName);

            File.Copy(iamgePath, targetPath, overwrite: true);
            result.path = targetPath;
        }
        catch (Exception ex)
        {
            result.message = ex.Message;
        }

        return result;
    }

    public async Task<Result> OpenAndSaveImageAsync(Window parent, string imageServiceDir)
    {
        Result result = new Result();

        try
        {
            var files = await parent.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "이미지 선택",
                FileTypeFilter = new[]
            {
                    new FilePickerFileType("Image Files")
                    {
                        Patterns = new[] { "*.jpg", "*.jpeg", "*.png" }
                    }
                },
                AllowMultiple = false
            });

            if (files.Count == 0)
            {
                result.message = "파일이 선택되지 않았습니다.";
                return result;
            }

            var sourcePath = files[0].TryGetLocalPath();
            if (string.IsNullOrEmpty(sourcePath))
            {
                result.message = "로컬 경로를 가져올 수 없습니다.";
                return result;
            }

            result = await SaveImageAsync(sourcePath, imageServiceDir);
        }
        catch (Exception ex)
        {
            result.message = ex.Message;
        }

        return result;
    }
}
