using Microsoft.Extensions.Logging;
using Repository.Models;
using Repository.Repo;
using Services.DTO;
using Services.Interface;
using Services.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class MediaAssetServices : IMediaAssetServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<MediaAssetServices> _logger; 
        public MediaAssetServices(IUnitOfWork unitOfWork, ILogger<MediaAssetServices> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<List<MediaAssetDto>> GetMediaForProjectAsync(Guid projectId)
        {
            // Kiểm tra xem Project có tồn tại không (tùy chọn nhưng nên có)
            var projectExists = await _unitOfWork.ProjectRepository.GetByIdAsync(projectId);
            if (projectExists ==null)
            {
                // Có thể trả về list rỗng hoặc ném lỗi tùy logic
                return new List<MediaAssetDto>();
                // Hoặc: throw new KeyNotFoundException("Project not found.");
            }

            var mediaAssets = await _unitOfWork.MediaAssetRepository.GetMediaForProjectAsync(projectId);

            // Map thủ công bằng LINQ
            var mediaDtos = mediaAssets.Select(m => new MediaAssetDto
            {
                Id = m.Id,
                Url = m.Url,
                Type = m.Type,
                SortOrder = m.SortOrder,
                CreatedAt = m.CreatedAt
            }).ToList();

            return mediaDtos;
        }
        public async Task<MediaAssetDto> AddMediaToProjectAsync(Guid projectId, CreateMediaAssetRequestDto request, Guid currentUserId)
        {
            // === BƯỚC 1: KIỂM TRA PROJECT VÀ QUYỀN SỞ HỮU ===
            var project = await _unitOfWork.ProjectRepository.GetByIdAsync(projectId);
            if (project == null)
            {
                throw new KeyNotFoundException($"Project with ID {projectId} not found.");
            }
            if (project.CreatorId != currentUserId)
            {
                _logger.LogWarning("User {UserId} attempted to add media to project {ProjectId} owned by {OwnerId}.", currentUserId, projectId, project.CreatorId);
                throw new UnauthorizedAccessException("You are not authorized to add media to this project.");
            }

            // === BƯỚC 2: TẠO ENTITY MỚI ===
            var newMediaAsset = new MediaAsset
            {
                Id = Guid.NewGuid(),
                ProjectId = projectId,
                Url = request.Url,
                Type = request.Type,
                // Có thể tự động gán SortOrder nếu client không gửi hoặc gửi 0
                SortOrder = request.SortOrder <= 0 ? (await _unitOfWork.MediaAssetRepository.CountAsync(m => m.ProjectId == projectId) + 1) : request.SortOrder,
                CreatedAt = DateTime.UtcNow
            };

            // === BƯỚC 3: THÊM VÀO DATABASE ===
            _unitOfWork.MediaAssetRepository.Create(newMediaAsset);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("User {UserId} added new media asset {MediaId} to project {ProjectId}.", currentUserId, newMediaAsset.Id, projectId);

            // === BƯỚC 4: MAP VÀ TRẢ VỀ DTO ===
            // Map thủ công
            return new MediaAssetDto
            {
                Id = newMediaAsset.Id,
                Url = newMediaAsset.Url,
                Type = newMediaAsset.Type,
                SortOrder = newMediaAsset.SortOrder,
                CreatedAt = newMediaAsset.CreatedAt
            };
        }
    }
}
