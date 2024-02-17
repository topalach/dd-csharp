using DomainDrivers.SmartSchedule.Availability.Segment;
using DomainDrivers.SmartSchedule.Shared;
using static DomainDrivers.SmartSchedule.Availability.Segment.SegmentInMinutes;

namespace DomainDrivers.SmartSchedule.Availability;

public class AvailabilityFacade
{
    private readonly ResourceAvailabilityRepository _availabilityRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AvailabilityFacade(ResourceAvailabilityRepository availabilityRepository, IUnitOfWork unitOfWork)
    {
        _availabilityRepository = availabilityRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task CreateResourceSlots(ResourceAvailabilityId resourceId, TimeSlot timeslot)
    {
        var groupedAvailability = ResourceGroupedAvailability.Of(resourceId, timeslot);
        await _availabilityRepository.SaveNew(groupedAvailability);
    }

    public async Task CreateResourceSlots(ResourceAvailabilityId resourceId, ResourceAvailabilityId parentId,
        TimeSlot timeslot)
    {
        var groupedAvailability = ResourceGroupedAvailability.Of(resourceId, timeslot, parentId);
        await _availabilityRepository.SaveNew(groupedAvailability);
    }

    public async Task<bool> Block(ResourceAvailabilityId resourceId, TimeSlot timeSlot, Owner requester)
    {
        return await _unitOfWork.InTransaction(async () =>
        {
            var toBlock = await FindGrouped(resourceId, timeSlot);
            return await Block(requester, toBlock);
        });
    }

    private async Task<bool> Block(Owner requester, ResourceGroupedAvailability toBlock)
    {
        var result = toBlock.Block(requester);

        if (result)
        {
            return await _availabilityRepository.SaveCheckingVersion(toBlock);
        }

        return result;
    }

    public async Task<bool> Release(ResourceAvailabilityId resourceId, TimeSlot timeSlot, Owner requester)
    {
        return await _unitOfWork.InTransaction(async () =>
        {
            var toRelease = await FindGrouped(resourceId, timeSlot);
            var result = toRelease.Release(requester);

            if (result)
            {
                return await _availabilityRepository.SaveCheckingVersion(toRelease);
            }

            return result;
        });
    }

    public async Task<bool> Disable(ResourceAvailabilityId resourceId, TimeSlot timeSlot, Owner requester)
    {
        return await _unitOfWork.InTransaction(async () =>
        {
            var toDisable = await FindGrouped(resourceId, timeSlot);
            var result = toDisable.Disable(requester);

            if (result)
            {
                result = await _availabilityRepository.SaveCheckingVersion(toDisable);
            }

            return result;
        });
    }

    private async Task<ResourceGroupedAvailability> FindGrouped(ResourceAvailabilityId resourceId, TimeSlot within)
    {
        var normalized = Segments.NormalizeToSegmentBoundaries(within, DefaultSegment());
        return new ResourceGroupedAvailability(await _availabilityRepository.LoadAllWithinSlot(resourceId, normalized));
    }

    public async Task<ResourceGroupedAvailability> Find(ResourceAvailabilityId resourceId, TimeSlot within)
    {
        var normalized = Segments.NormalizeToSegmentBoundaries(within, DefaultSegment());
        return new ResourceGroupedAvailability(await _availabilityRepository.LoadAllWithinSlot(resourceId, normalized));
    }

    public async Task<ResourceGroupedAvailability> FindByParentId(ResourceAvailabilityId parentId, TimeSlot within)
    {
        var normalized = Segments.NormalizeToSegmentBoundaries(within, DefaultSegment());
        return new ResourceGroupedAvailability(
            await _availabilityRepository.LoadAllByParentIdWithinSlot(parentId, normalized));
    }
}