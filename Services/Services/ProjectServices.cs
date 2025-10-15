using Repository.Models;
using Repository.Repo;
using Services.DTO;
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
            // Gọi phương thức Repository (không thay đổi)
            var (projects, totalCount) = await _unitOfWork.ProjectRepository.QueryProjectsAsync(
                request.SearchQuery,
                request.CategoryId,
                request.Status,
                request.SortBy,
                request.Page,
                request.PageSize
            );

           
            var projectSummaries = projects.Select(p => new ProjectSummaryResponse
            {
                Id = p.Id,
                Title = p.Title,
                Summary = p.Summary,
                MediaCoverUrl = p.MediaCoverUrl, 
                CurrentAmount = p.CurrentAmount, 
                Goal = p.Goal, 
                EndAt = p.EndAt, 
                CreatorName = p.Creator?.FullName ?? "N/A" ,
                CategoryName = p.Category?.Name ?? "N/A"
            }).ToList();

           
            var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

           
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
           

           
            var project = new Project
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Slug = slug, 
                Summary = request.Summary, 
                Goal = request.Goal, 
                EndAt = request.EndAt, 
                CategoryId = request.CategoryId,
                CreatorId = creatorId,
                Status = "Draft",
                CreatedAt = DateTime.UtcNow,
                CurrentAmount = 0,
                MediaCoverUrl = request.MediaCoverUrl,
                Description = request.Description
            };

           
            _unitOfWork.ProjectRepository.Create(project);
            await _unitOfWork.SaveChangesAsync();

            
            return new ProjectDetailResponseDto
            {
                Id = project.Id,
                Title = project.Title,
                Slug = project.Slug, 
                Summary = project.Summary,
                Goal = project.Goal,
                EndAt = project.EndAt, 
                Status = project.Status,
                CategoryId = project.CategoryId,
                CreatorId = project.CreatorId,
                CreatedAt = project.CreatedAt,
                MediaCoverUrl =project.MediaCoverUrl,
                Description =project.Description
            };
        }
        public async Task<bool> UpdateProjectAsync(Guid projectId, UpdateProjectRequest request, Guid currentUserId)
        {
            
            var project = await _unitOfWork.ProjectRepository.GetByIdAsync(projectId);
            if (project == null)
            {
                return false; 
            }

           
            if (project.CreatorId != currentUserId)
            {
                throw new UnauthorizedAccessException("Bạn không có quyền chỉnh sửa dự án này.");
            }

            
            if (project.Status != "Draft" && project.Status != "Submitted")
            {
                throw new InvalidOperationException("Chỉ có thể chỉnh sửa dự án khi đang ở trạng thái Draft hoặc Submmitted .");
            }

           
            project.Title = request.Title;
            project.Summary = request.Summary;
            project.Description = request.Description;     
            project.MediaCoverUrl = request.MediaCoverUrl;  
            project.Goal = request.Goal;
            project.EndAt = request.EndAt;
            project.CategoryId = request.CategoryId;
            project.UpdatedAt = DateTime.UtcNow;



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
        public async Task<bool> ApproveProjectAsync(Guid projectId, Guid adminId)
        {
            var project = await _unitOfWork.ProjectRepository.GetByIdAsync(projectId);
            if (project == null)
            {
                throw new KeyNotFoundException("Không tìm thấy dự án.");
            }
            if (project.Status != "Submitted")
            {
                throw new InvalidOperationException("Chỉ có thể duyệt dự án khi đang ở trạng thái Submitted.");
            }

            // --- PHẦN CẬP NHẬT ---

            // Bước 1: Tạo bản ghi ProjectApproval với đúng các thuộc tính
            var approvalRecord = new ProjectApproval
            {
                Id = Guid.NewGuid(),
                ProjectId = projectId,
                AdminId = adminId,           // <-- Sửa đổi: Khớp với model
                Decision = "Published",       // <-- Sửa đổi: Khớp với model
                DecidedAt = DateTime.UtcNow, // <-- Sửa đổi: Khớp với model
                Note = "Dự án đã được chấp thuận.",
                CreatedAt = DateTime.UtcNow
            };
            _unitOfWork.ProjectApprovalRepository.Create(approvalRecord);

            // Bước 2: Cập nhật trạng thái của dự án
            project.Status = "Published";
            project.UpdatedAt = DateTime.UtcNow;
            project.StartAt = DateTime.UtcNow;
            _unitOfWork.ProjectRepository.Update(project);

            // Bước 3: Lưu tất cả thay đổi vào DB
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
        public async Task<PaginatedPendingProjectResponse> GetPendingProjectsAsync(AdminQueryPendingProjectsRequest request)
        {
            var (projects, totalCount) = await _unitOfWork.ProjectRepository.GetPendingProjectsAsync(request.Page, request.PageSize);

            // Bước 2: Chuyển đổi từ Model sang DTO
            var pendingProjectsDto = projects.Select(p => new PendingProjectResponseDto
            {
                Id = p.Id,
                Title = p.Title,
                CreatorName = p.Creator?.FullName ?? "N/A", 
                SubmittedAt = p.UpdatedAt ?? p.CreatedAt 
            }).ToList();

           
            var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

            return new PaginatedPendingProjectResponse
            {
                Projects = pendingProjectsDto,
                CurrentPage = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
        }

        public async Task<ProjectDetailResponse> GetPublishedProjectBySlugAsync(string slug)
        {
            // Bước 1: Gọi Repository để lấy dữ liệu thô
            var project = await _unitOfWork.ProjectRepository.GetPublishedBySlugWithDetailsAsync(slug);

            if (project == null)
            {
                return null; // Trả về null để controller xử lý thành 404 Not Found
            }

            // Bước 2: Thực hiện tính toán và chuyển đổi (map)
            var projectDetailDto = new ProjectDetailResponse
            {
                Id = project.Id,
                Title = project.Title,
                Slug = project.Slug,
                Summary = project.Summary,
                Description = project.Description,
                Goal = project.Goal,
                CurrentAmount = project.CurrentAmount,
                MediaCoverUrl = project.MediaCoverUrl,
                EndAt = project.EndAt,
                Status = project.Status,
                CreatedAt = project.CreatedAt,

                // Tính toán tiến độ
                 ProgressPercentage = (double)(project.Goal > 0 ? Math.Round((project.CurrentAmount / project.Goal) * 100, 2) : 0),

                // Đếm số người ủng hộ (chỉ đếm các pledge thành công nếu có)
                BackerCount = project.Pledges.Count(),

                // Xử lý an toàn nếu Creator bị null
                CreatorName = project.Creator?.FullName ?? "N/A",

                // Map danh sách các gói thưởng
                Tiers = project.RewardTiers.Select(t => new RewardTierDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    Amount = t.Amount,
                    Quantity = t.Quantity
                }).ToList(),

                // Map danh sách media
                Media = project.MediaAssets.Select(m => new MediaAssetDto
                {
                    Id = m.Id,
                    Url = m.Url,
                    Type = m.Type,
                    SortOrder = m.SortOrder
                }).OrderBy(m => m.SortOrder).ToList() // Sắp xếp media theo thứ tự
            };

            return projectDetailDto;
        }

        public async Task<bool> RejectProjectAsync(Guid projectId, Guid adminId, RejectProjectRequest request)
        {
            // Bước 1: Tìm dự án
            var project = await _unitOfWork.ProjectRepository.GetByIdAsync(projectId);
            if (project == null)
            {
                throw new KeyNotFoundException("Không tìm thấy dự án.");
            }

            // Bước 2: Kiểm tra trạng thái
            if (project.Status != "Submitted")
            {
                throw new InvalidOperationException("Chỉ có thể từ chối dự án khi đang ở trạng thái Submitted.");
            }

            // Bước 3: Tạo bản ghi ProjectApproval để lưu lịch sử
            var approvalRecord = new ProjectApproval
            {
                Id = Guid.NewGuid(),
                ProjectId = projectId,
                AdminId = adminId,
                Decision = "Rejected", // Quyết định là "Rejected"
                Note = request.Note, // Lý do từ chối từ request
                DecidedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };
            _unitOfWork.ProjectApprovalRepository.Create(approvalRecord);

            // Bước 4: Cập nhật trạng thái của dự án
            project.Status = "Rejected";
            project.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.ProjectRepository.Update(project);

            // Bước 5: Lưu tất cả thay đổi vào DB
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

    }
}
