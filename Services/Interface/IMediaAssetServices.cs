using Services.DTO;
using Services.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IMediaAssetServices
    {
        Task<List<MediaAssetDto>> GetMediaForProjectAsync(Guid projectId);

        Task<MediaAssetDto> AddMediaToProjectAsync(Guid projectId, CreateMediaAssetRequestDto request, Guid currentUserId);
    }
}
