using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Scheduling.Application.Dto;
using Scheduling.Application.Interfaces;
using Scheduling.Domain.Enums;
using Scheduling.Domain.Models;
using Scheduling.Reservation.Filters;

namespace Scheduling.Reservation.Controllers;

[ApiController]
[Route("[controller]")]
public class ReservationController : ControllerBase
{
    private readonly ILogger<ReservationController> _logger;
    private readonly IReservationService _reservationService;
    private readonly IMapper _mapper;

    public ReservationController(ILogger<ReservationController> logger,
        IReservationService reservationService,
        IMapper mapper)
    {
        _reservationService = reservationService;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpPost]
    [ValidateModel]
    public async Task<ReservationResponse> Create(CreateReservationRequest request)
    {
        _logger.LogInformation($"Creating reservation for {request.ProductGroup}:{request.Account}");
        var result = await _reservationService.CreateAsync(_mapper.Map<ReservationItem>(request));
        return _mapper.Map<ReservationResponse>(result);
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<ReservationResponse> GetById(string id)
    {
        _logger.LogInformation($"Get reservation by id '{id}'");
        var result = await  _reservationService.GetByIdAsync(id);
        return _mapper.Map<ReservationResponse>(result);
    }

    [HttpPost]
    [Route("query")]
    [ValidateModel]
    public async Task<List<ReservationResponse>> Query(ReservationFilteredRequest request)
    {
        _logger.LogInformation($"Listing reservations");
        var result = await _reservationService.FilterAsync();
        return _mapper.Map<List<ReservationItem>, List<ReservationResponse>>(result);
    }

    [HttpDelete]    
    [Route("{id}")]
    public async Task Delete(string id)
    {
        _logger.LogInformation($"Deleting reservation for '{id}'");
        await _reservationService.RemoveAsync(id);
    }

    [HttpPut]
    [Route("{id}/status/{status}")]
    public async Task<ReservationResponse> UpdateStatus(string id, ReservationStatus status)
    {
        var reservation = await _reservationService.GetByIdAsync(id);
        _logger.LogInformation(
            $"Updating reservation status for '{reservation?.ProductGroup}:{reservation?.Account}:{id}' to {status}");
        var result = await _reservationService.UpdateStatusAsync(id, status);
        return _mapper.Map<ReservationResponse>(result);
    }
}