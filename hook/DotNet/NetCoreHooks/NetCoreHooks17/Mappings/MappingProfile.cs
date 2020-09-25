using AutoMapper;
using NetCoreHooks.DTOs;
using NetCoreHooks.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreHooks.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Add as many of these lines as you need to map your objects
            CreateMap<Registrant, RegistrantDTO>().ReverseMap();
        }
    }
}
