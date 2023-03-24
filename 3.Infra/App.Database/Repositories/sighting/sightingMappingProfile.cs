using AutoMapper;
using App.Domain;

namespace App.Database
{
    public class sightingMappingProfile : Profile
    {
        public sightingMappingProfile()
        {
            CreateMap<sightingInputModel, sightingEntity>();
            CreateMap<sightingEntity, sightingInputModel>();
            CreateMap<sightingEntity, sightingViewModel>();
            CreateMap<sightingEntity, sightingWithSelectionViewModel>();
        }
    }
}
