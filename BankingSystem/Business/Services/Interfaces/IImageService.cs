using Domain.Entities.Concretes;

namespace Business.Services
{
    public interface IImageService
    {
        Task<string> TryToGetFaceImageAsync(User user);
    }
}