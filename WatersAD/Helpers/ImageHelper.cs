namespace WatersAD.Helpers
{
    public class ImageHelper : IImageHelper
    {
        public async Task<string> UploadImageAsync(IFormFile imageFile, string folder, string oldPath = null)
        {
            if (!string.IsNullOrEmpty(oldPath))
            {
                var oldFilePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                oldPath.TrimStart('~', '/').Replace('/', '\\'));

                if (File.Exists(oldFilePath))
                {
                    File.Delete(oldFilePath);
                }
            }

            string guid = Guid.NewGuid().ToString();
            string file = $"{guid}.jpg";

            string path = Path.Combine(
                Directory.GetCurrentDirectory(),
                $"wwwroot\\image\\{folder}",
                file);

            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            return $"~/image/{folder}/{file}";
        }
    }
}
