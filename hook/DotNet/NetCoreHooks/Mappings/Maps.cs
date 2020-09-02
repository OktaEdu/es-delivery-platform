using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using NetCoreHooks.DTOs;
using NetCoreHooks.model;

namespace NetCoreHooks.Mappings
{
    public class Maps : Profile
    {
        public Maps()
        {
            CreateMap<Employee, EmployeeDTO>().ReverseMap();
        }
    }
}
