using AutoMapper;
using Scheduling.Application.Dto;
using Scheduling.Domain.Entities;
using Scheduling.Domain.Models;

namespace Scheduling.Application.Mappings;

public class ApplicationMappingsProfile : Profile
{
    public ApplicationMappingsProfile()
    {
        CreateMap<ReservationItem, ReservationResponse>();
        CreateMap<CreateReservationRequest, ReservationItem>();
        CreateMap<ReservationItem, ReservationEntity>();
        CreateMap<ReservationEntity, ReservationItem>();
    }
}
