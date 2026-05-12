using Microsoft.EntityFrameworkCore;
using SponsorshipApproval.Api.Application.DTOs;
using SponsorshipApproval.Api.Application.Interfaces;
using SponsorshipApproval.Api.Domain.Entities;
using SponsorshipApproval.Api.Infrastructure.Data;

namespace SponsorshipApproval.Api.Application.Services;

public sealed class SponsorshipTypeService(ApplicationDbContext dbContext) : ISponsorshipTypeService
{
    public async Task<IReadOnlyCollection<SponsorshipTypeDto>> GetAllAsync(bool includeInactive, CancellationToken cancellationToken)
    {
        var query = dbContext.SponsorshipTypes.AsNoTracking();
        if (!includeInactive)
        {
            query = query.Where(x => x.IsActive);
        }

        return await query.OrderBy(x => x.Name).Select(x => ToDto(x)).ToListAsync(cancellationToken);
    }

    public async Task<SponsorshipTypeDto> CreateAsync(SponsorshipTypeCreateDto dto, CancellationToken cancellationToken)
    {
        await EnsureUniqueNameAsync(dto.Name, null, cancellationToken);
        var entity = new SponsorshipType { Name = dto.Name.Trim(), Description = dto.Description?.Trim(), IsActive = dto.IsActive };
        dbContext.SponsorshipTypes.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToDto(entity);
    }

    public async Task<SponsorshipTypeDto> UpdateAsync(int id, SponsorshipTypeUpdateDto dto, CancellationToken cancellationToken)
    {
        var entity = await dbContext.SponsorshipTypes.FindAsync(new object[] { id }, cancellationToken) ?? throw new KeyNotFoundException("Sponsorship type was not found.");
        await EnsureUniqueNameAsync(dto.Name, id, cancellationToken);
        entity.Name = dto.Name.Trim();
        entity.Description = dto.Description?.Trim();
        entity.IsActive = dto.IsActive;
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToDto(entity);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await dbContext.SponsorshipTypes.FindAsync(new object[] { id }, cancellationToken) ?? throw new KeyNotFoundException("Sponsorship type was not found.");
        var inUse = await dbContext.SponsorshipRequests.AnyAsync(x => x.SponsorshipTypeId == id, cancellationToken);
        if (inUse)
        {
            entity.IsActive = false;
        }
        else
        {
            dbContext.SponsorshipTypes.Remove(entity);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureUniqueNameAsync(string name, int? existingId, CancellationToken cancellationToken)
    {
        var normalizedName = name.Trim().ToLower();
        var exists = await dbContext.SponsorshipTypes.AnyAsync(x => x.Name.ToLower() == normalizedName && x.Id != existingId, cancellationToken);
        if (exists)
        {
            throw new InvalidOperationException("A sponsorship type with this name already exists.");
        }
    }

    private static SponsorshipTypeDto ToDto(SponsorshipType entity) => new(entity.Id, entity.Name, entity.Description, entity.IsActive);
}
