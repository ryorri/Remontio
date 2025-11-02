using Application.Objects.DTOs.AlertsDTO;
using Application.Objects.DTOs.PhotoDTO;
using Application.Objects.DTOs.Photos;
using Application.Objects.DTOs.ProjectDTO;
using Application.Objects.DTOs.RoomDTO;
using Application.Objects.DTOs.TaskDTO;
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
            CreateMap<Project, ProjectDataDTO>();
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

            #region RoomMapping

            CreateMap<CreateRoomDTO, Room>();
            CreateMap<RoomDataDTO, Room>();
            CreateMap<Room, RoomDataDTO>();
            CreateMap<List<Room>, List<RoomDataDTO>>()
                    .ConvertUsing((src, dest, context) =>
                    {
                        var result = new List<RoomDataDTO>();
                        foreach (var proj in src)
                        {
                            result.Add(context.Mapper.Map<RoomDataDTO>(proj));
                        }
                        return result;
                    });


            #endregion

            #region WallMapping

            CreateMap<WallDTO, Wall>();
            CreateMap<Wall, WallDTO>();
            CreateMap<List<Wall>, List<WallDTO>>()
                    .ConvertUsing((src, dest, context) =>
                    {
                        var result = new List<WallDTO>();
                        foreach (var proj in src)
                        {
                            result.Add(context.Mapper.Map<WallDTO>(proj));
                        }
                        return result;
                    });


            #endregion

            #region FloorMapping

            CreateMap<FloorDTO, Floor>();
            CreateMap<Floor, FloorDTO>();
            CreateMap<List<Floor>, List<FloorDTO>>()
                    .ConvertUsing((src, dest, context) =>
                    {
                        var result = new List<FloorDTO>();
                        foreach (var proj in src)
                        {
                            result.Add(context.Mapper.Map<FloorDTO>(proj));
                        }
                        return result;
                    });


            #endregion

            #region AlertsMapping

            CreateMap<CreateAlertDTO, Alerts>();
            CreateMap<AlertDataDTO, Alerts>();
            CreateMap<Alerts, AlertDataDTO>();
            CreateMap<List<Alerts>, List<AlertDataDTO>>()
                    .ConvertUsing((src, dest, context) =>
                    {
                        var result = new List<AlertDataDTO>();
                        foreach (var proj in src)
                        {
                            result.Add(context.Mapper.Map<AlertDataDTO>(proj));
                        }
                        return result;
                    });


            #endregion

            #region PhotosMapping

            CreateMap<CreatePhotoDTO, Photos>();
            CreateMap<PhotoDataDTO, Photos>();
            CreateMap<Photos, PhotoDataDTO>();
            CreateMap<List<Photos>, List<PhotoDataDTO>>()
                    .ConvertUsing((src, dest, context) =>
                    {
                        var result = new List<PhotoDataDTO>();
                        foreach (var proj in src)
                        {
                            result.Add(context.Mapper.Map<PhotoDataDTO>(proj));
                        }
                        return result;
                    });


            #endregion

            #region PhotosMapping

            CreateMap<CreateTaskDTO, Tasks>();
            CreateMap<TaskDataDTO, Tasks>();
            CreateMap<Tasks, TaskDataDTO>();
            CreateMap<List<Tasks>, List<TaskDataDTO>>()
                    .ConvertUsing((src, dest, context) =>
                    {
                        var result = new List<TaskDataDTO>();
                        foreach (var proj in src)
                        {
                            result.Add(context.Mapper.Map<TaskDataDTO>(proj));
                        }
                        return result;
                    });


            #endregion
        }
    }
}
