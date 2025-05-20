using UploadProjects.Model;

namespace UploadProjects.Service.Interface
{
    public interface IProjectUploadService
    {
        Task<(int success, int fail)> EmbedAndUploadAsync(List<ProjectDocument> projects);
    }
}
