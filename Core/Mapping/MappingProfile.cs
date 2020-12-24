﻿using AutoMapper;
using Data.DTOs;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<IdentityUser, WebUserDTO>();
            CreateMap<WebUserDTO, IdentityUser>()
                .ForMember(u => u.Email, opt => opt.MapFrom(ur => ur.Username));
        }
        
    }
}
