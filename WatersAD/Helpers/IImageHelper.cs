﻿namespace WatersAD.Helpers
{
    public interface IImageHelper
    {
        Task<string> UploadImageAsync(IFormFile imageFile, string folder, string oldPath = null);
    }
}
