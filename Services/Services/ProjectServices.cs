using Repository.Models;
using Repository.Repo;
using Services.Interface;
using Services.Request;
using Services.Response;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Services.Services
{
    public class ProjectServices : IProjectServices
    {
        private readonly IUnitOfWork _unitOfWork;

        // Thêm constructor để inject IUnitOfWork
        public ProjectServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task CreateAsync(Project p)
        {
            // Bước 1: Chuẩn bị hành động Create
            _unitOfWork.ProjectRepository.Create(p);
            // Bước 2: Lưu tất cả thay đổi vào DB
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            // Bước 1: Tìm đối tượng cần xóa
            var project = await _unitOfWork.ProjectRepository.GetByIdAsync(id);
            if (project == null)
            {
                return false;
            }

            // Bước 2: Chuẩn bị hành động Remove
            _unitOfWork.ProjectRepository.Remove(project);

            // Bước 3: Lưu thay đổi vào DB
            var result = await _unitOfWork.SaveChangesAsync();
            return result > 0;
        }
        public async Task<List<Project>> GetAllAsync()
        {
            return await _unitOfWork.ProjectRepository.GetAllAsync();
        }

        public async Task<Project> GetByIdAsync(Guid id)
        {
            // Giả sử ProjectRepository có GetByIdAsync(Guid id)
            return await _unitOfWork.ProjectRepository.GetByIdAsync(id);
        }

        public async Task<PaginatedProjectResponse> QueryProjectsAsync(QueryProjectsRequest request)
        {
            // Gọi phương thức Repository với các tham số đã được chuẩn hóa
            var (projects, totalCount) = await _unitOfWork.ProjectRepository.QueryProjectsAsync(
                request.SearchQuery,
                request.CategoryId,
                request.Status,
                request.SortBy,
                request.Page,
                request.PageSize
            );

            // Chuyển đổi từ Model `Project` sang `ProjectSummaryResponse` DTO
            var projectSummaries = projects.Select(p => new ProjectSummaryResponse
            {
                Id = p.Id,
                Title = p.Title,
                Subtitle = p.Summary,
                CoverImageUrl = p.MediaCoverUrl,
                CurrentPledgeAmount = p.CurrentAmount,
                GoalAmount = p.Goal,
                EndDate = p.EndAt,
                CreatorName = p.Creator.FullName 
            }).ToList();

            // Tính toán thông tin phân trang
            var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

            // Tạo và trả về đối tượng response cuối cùng
            return new PaginatedProjectResponse
            {
                Projects = projectSummaries,
                CurrentPage = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
        }
        public async Task<ProjectDetailResponseDto> CreateDraftProjectAsync(CreateProjectRequestDto request, Guid creatorId)
        {
            
            var slug = Regex.Replace(request.Title.ToLower(), @"[^a-z0-9\s-]", "")
                            .Replace(" ", "-");
           

            // Bước 2: Chuyển đổi từ Request DTO sang Model (với tên thuộc tính đã cập nhật)
            var project = new Project
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Slug = slug, // Gán slug vừa tạo
                Summary = request.Summary, // Dùng tên mới
                Goal = request.Goal, // Dùng tên mới
                EndAt = request.EndAt, // Dùng tên mới
                CategoryId = request.CategoryId,
                CreatorId = creatorId,
                Status = "Draft",
                CreatedAt = DateTime.UtcNow,
                CurrentAmount = 0 // Tên trong model của bạn là CurrentAmount
            };

            // Bước 3: Lưu vào DB
            _unitOfWork.ProjectRepository.Create(project);
            await _unitOfWork.SaveChangesAsync();

            // Bước 4: Chuyển đổi từ Model sang Response DTO để trả về (với tên thuộc tính đã cập nhật)
            return new ProjectDetailResponseDto
            {
                Id = project.Id,
                Title = project.Title,
                Slug = project.Slug, // Trả về slug
                Summary = project.Summary, // Dùng tên mới
                Goal = project.Goal, // Dùng tên mới
                EndAt = project.EndAt, // Dùng tên mới
                Status = project.Status,
                CategoryId = project.CategoryId,
                CreatorId = project.CreatorId,
                CreatedAt = project.CreatedAt
            };
        }
        public async Task<bool> UpdateProjectAsync(Guid projectId, UpdateProjectRequest request, Guid currentUserId)
        {
            // Bước 1: Tìm dự án trong DB
            var project = await _unitOfWork.ProjectRepository.GetByIdAsync(projectId);
            if (project == null)
            {
                return false; // Không tìm thấy
            }

            // Bước 2: Kiểm tra quyền sở hữu
            if (project.CreatorId != currentUserId)
            {
                throw new UnauthorizedAccessException("Bạn không có quyền chỉnh sửa dự án này.");
            }

            // Bước 3: Kiểm tra trạng thái (chỉ cho sửa khi là Draft hoặc Pending)
            if (project.Status != "Draft" && project.Status != "Pending")
            {
                throw new InvalidOperationException("Chỉ có thể chỉnh sửa dự án khi đang ở trạng thái Draft hoặc Pending.");
            }

            // Bước 4: Cập nhật thông tin từ DTO vào model
            project.Title = request.Title;
            project.Summary = request.Summary;
            project.Description = request.Description;       // <-- Cập nhật trường mới
            project.MediaCoverUrl = request.MediaCoverUrl;   // <-- Cập nhật trường mới
            project.Goal = request.Goal;
            project.EndAt = request.EndAt;
            project.CategoryId = request.CategoryId;
            project.UpdatedAt = DateTime.UtcNow;

            // TODO: Xử lý tạo lại slug nếu Title thay đổi

            // Bước 5: Chuẩn bị và lưu thay đổi
            _unitOfWork.ProjectRepository.Update(project);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
        public async Task<RewardTierResponseDto> CreateTierForProjectAsync(Guid projectId, CreateRewardTierRequestDto request, Guid creatorId)
        {
            
            var project = await _unitOfWork.ProjectRepository.GetByIdAsync(projectId);
            if (project == null)
            {
                throw new KeyNotFoundException("Không tìm thấy dự án.");
            }
            if (project.CreatorId != creatorId)
            {
                throw new UnauthorizedAccessException("Bạn không có quyền thêm gói thưởng cho dự án này.");
            }

            // Step 2: Map from DTO to Model (with updated properties)
            var newTier = new RewardTier
            {
                Id = Guid.NewGuid(),
                ProjectId = projectId,
                Title = request.Title,
                Description = request.Description,
                Amount = request.Amount, // Use 'Amount'
                Quantity = request.Quantity,
                DeliveryDate = request.DeliveryDate, // Add 'DeliveryDate'
                CreatedAt = DateTime.UtcNow
            };

            // Step 3: Save to DB (no change here)
            _unitOfWork.RewardTierRepository.Create(newTier);
            await _unitOfWork.SaveChangesAsync();

            // Step 4: Map to Response DTO (with updated properties)
            return new RewardTierResponseDto
            {
                Id = newTier.Id,
                ProjectId = newTier.ProjectId,
                Title = newTier.Title,
                Description = newTier.Description,
                Amount = newTier.Amount, // Use 'Amount'
                Quantity = newTier.Quantity,
                DeliveryDate = newTier.DeliveryDate // Add 'DeliveryDate'
            };
        }

        public async Task<bool> SubmitProjectForApprovalAsync(Guid projectId, Guid creatorId)
        {
            // Bước 1: Tìm dự án trong DB
            var project = await _unitOfWork.ProjectRepository.GetByIdAsync(projectId);
            if (project == null)
            {
                // Ném lỗi để controller trả về 404 Not Found
                throw new KeyNotFoundException("Không tìm thấy dự án.");
            }

            // Bước 2: Kiểm tra quyền sở hữu
            if (project.CreatorId != creatorId)
            {
                // Ném lỗi để controller trả về 403 Forbidden
                throw new UnauthorizedAccessException("Bạn không có quyền gửi duyệt dự án này.");
            }

            // Bước 3: Kiểm tra trạng thái của dự án
            // Chỉ cho phép gửi duyệt khi dự án đang ở trạng thái "Draft"
            if (project.Status != "Draft")
            {
                // Ném lỗi để controller trả về 400 Bad Request
                throw new InvalidOperationException("Chỉ có thể gửi duyệt dự án khi đang ở trạng thái Draft.");
            }

            // Bước 4: Cập nhật trạng thái
            project.Status = "Submitted"; // Hoặc "Pending" tùy theo quy ước của bạn
            project.UpdatedAt = DateTime.UtcNow;

            // Bước 5: Chuẩn bị và lưu thay đổi
            _unitOfWork.ProjectRepository.Update(project);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
