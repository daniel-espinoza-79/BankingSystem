
using Domain.Entities.Concretes;

namespace Business.Services
{
    public class ImageService : IImageService
    {
        private static readonly string _facesDirectory = Environment.GetEnvironmentVariable("DIRECTORY_IMAGE_FACES") ?? throw new NullReferenceException("DIRECTORY_IMAGE_FACES not set");
     
        private static readonly string _faceImageName = "face_1.jpg";
        public async Task<string> TryToGetFaceImageAsync(User user)
        {
            var separator = Path.DirectorySeparatorChar.ToString();
            string imagePath = _facesDirectory + separator + user.Id + separator + _faceImageName;

            if (string.IsNullOrEmpty(imagePath) || !File.Exists(imagePath))
            {
                return string.Empty;
            }
            byte[] imageBytes = File.ReadAllBytes(imagePath);
            return Convert.ToBase64String(imageBytes);
        }
    }
}
