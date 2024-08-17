namespace Shop.PL.Helper
{
    public static class DocumentSettings
    {
        public static string? Upload(IFormFile? file, string folderName)
        {
            if (file is null)
                return null;

            string fileName = $"{Guid.NewGuid()}{file.FileName}";

            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Files", folderName);

            string fullFilePath = Path.Combine(folderPath, fileName);

            using var fileStream = new FileStream(fullFilePath, FileMode.Create);
            file.CopyTo(fileStream);

            return fileName;
        }

        public static void Delete(string? fileName, string folderName)
        {
            if (fileName is null) return;

            string fullFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Files", folderName, fileName);

            if (File.Exists(fullFilePath))
                File.Delete(fullFilePath);
        }
    }
}
