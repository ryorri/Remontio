﻿using Application.Objects.DTOs.ProjectDTO;
using Application.Objects.DTOs.UserDTO;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            #region UserMapping
            CreateMap<CreateUserDTO, User>();
            CreateMap<UserDataDTO, User>();
            CreateMap<User, UserDataDTO>();
            CreateMap<List<User>, List<UserDataDTO>>()
                    .ConvertUsing((src, dest, context) =>
                    {
                        var result = new List<UserDataDTO>();
                        foreach (var user in src)
                        {
                            result.Add(context.Mapper.Map<UserDataDTO>(user));
                        }
                        return result;
                    });
            #endregion

            #region ProjectMapping

            CreateMap<CreateProjectDTO, Project>();
            CreateMap<ProjectDataDTO, Project>();
            CreateMap<Project,  ProjectDataDTO>();
            CreateMap<List<Project>, List<ProjectDataDTO>>()
                    .ConvertUsing((src, dest, context) =>
                    {
                        var result = new List<ProjectDataDTO>();
                        foreach (var proj in src)
                        {
                            result.Add(context.Mapper.Map<ProjectDataDTO>(proj));
                        }
                        return result;
                    });


            #endregion
        }
    }
}
