namespace WatersAD.Helpers
{
    public interface IImageHelper
    {
        /// <summary>
        /// Asynchronously uploads an image file to the specified folder and returns the path of the uploaded image.
        /// </summary>
        /// <param name="imageFile">The <see cref="IFormFile"/> object representing the image file to be uploaded.</param>
        /// <param name="folder">The destination folder where the image will be stored.</param>
        /// <param name="oldPath">An optional parameter representing the path of an existing image that can be deleted or replaced; defaults to null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the string path of the uploaded image.</returns>
        Task<string> UploadImageAsync(IFormFile imageFile, string folder, string oldPath = null);
    }
}
