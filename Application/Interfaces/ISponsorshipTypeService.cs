using SponsorshipApproval.Api.Application.DTOs;

namespace SponsorshipApproval.Api.Application.Interfaces;

public interface ISponsorshipTypeService
{
    Task<IReadOnlyCollection<SponsorshipTypeDto>> GetAllAsync(bool includeInactive, CancellationToken cancellationToken);
    Task<SponsorshipTypeDto> CreateAsync(SponsorshipTypeCreateDto dto, CancellationToken cancellationToken);
    Task<SponsorshipTypeDto> UpdateAsync(int id, SponsorshipTypeUpdateDto dto, CancellationToken cancellationToken);
    Task DeleteAsync(int id, CancellationToken cancellationToken);
}
