using AutoMapper;
using App.Domain;

namespace App.Database
{
    public class alienMappingProfile : Profile
    {
        public alienMappingProfile()
        {
            CreateMap<alienInputModel, alienEntity>();
            CreateMap<alienEntity, alienInputModel>();
            CreateMap<alienEntity, alienViewModel>();
            CreateMap<alienEntity, alienWithSelectionViewModel>();
        }
    }
}
