using AutoMapper;
using Scheduling.Domain.Models;
using Scheduling.Application.Interfaces;
using Scheduling.Domain.Entities;
using Scheduling.Domain.Enums;
using Scheduling.Infrastructure.Interfaces;

namespace Scheduling.Application.Services;

public class ReservationService : IReservationService
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IMapper _mapper;
    public ReservationService(IReservationRepository reservationRepository, IMapper mapper)
    {
        _reservationRepository = reservationRepository;
        _mapper = mapper;
    }

    public async Task<ReservationItem> CreateAsync(ReservationItem reservation)
    {
        if(null == reservation)
        {
            throw new ArgumentNullException(nameof(reservation));
        }

        var obj = _mapper.Map<ReservationEntity>(reservation);
        var result= await _reservationRepository.CreateAsync(obj);
        return _mapper.Map<ReservationItem>(result);
    }

    public async Task<ReservationItem> GetByIdAsync(string id)
    {
        var result = await _reservationRepository.GetByIdAsync(id);
        return _mapper.Map<ReservationItem>(result);
    }

    public async Task RemoveAsync(string id) =>
        await _reservationRepository.RemoveAsync(id);

    public async Task<ReservationItem> UpdateStatusAsync(string id, ReservationStatus status)
    {
        var result = await _reservationRepository.UpdateStatusAsync(id, status);
        return _mapper.Map<ReservationItem>(result);
    }

    public async Task<List<ReservationItem>> FilterAsync()
    {
        return _mapper.Map<List<ReservationItem>>(null);
    }

}
