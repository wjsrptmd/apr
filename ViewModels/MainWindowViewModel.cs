using Avalonia.Controls;
using Avalonia.Platform.Storage;

using System;
using System.IO;
using System.Threading.Tasks;

namespace apr.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public async Task<CopyResult> OpenAndSaveImageAsync(Window parent)
    {
        CopyResult result = new CopyResult();

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

            string baseDir = AppContext.BaseDirectory;
            string targetDir = Path.Combine(baseDir, "imageData");

            Directory.CreateDirectory(targetDir);

            string fileName = Path.GetFileName(sourcePath);
            string targetPath = Path.Combine(targetDir, fileName);

            File.Copy(sourcePath, targetPath, overwrite: true);
            result.path = targetPath;
        }
        catch (Exception ex)
        {
            result.message = ex.Message;
        }

        return result;
    }
}
