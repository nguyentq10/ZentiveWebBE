using Repository.Models;
using Services.Request;
using Services.Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IProjectServices
    {
        Task<List<Project>> GetAllAsync();
        Task<Project> GetByIdAsync(Guid id);
        Task CreateAsync(Project p);
       // Task<bool> UpdateAsync(Project p);
        Task<bool> DeleteAsync(Guid id);
        Task<PaginatedProjectResponse> QueryProjectsAsync(QueryProjectsRequest request);
        Task<ProjectDetailResponseDto> CreateDraftProjectAsync(CreateProjectRequestDto request, Guid creatorId);

        Task<bool> UpdateProjectAsync(Guid projectId, UpdateProjectRequest request, Guid currentUserId);
        Task<RewardTierResponseDto> CreateTierForProjectAsync(Guid projectId, CreateRewardTierRequestDto request, Guid creatorId);
        Task<bool> SubmitProjectForApprovalAsync(Guid projectId, Guid creatorId);
    }
}