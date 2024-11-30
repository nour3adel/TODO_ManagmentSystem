using AutoMapper;
using ToDoList.DTOs;
using ToDoList.Models;

namespace ToDoList.Configurations
{
    public class TaskMapperConfig : Profile
    {
        public TaskMapperConfig()
        {
            CreateMap<ToDoTask, TodoDTO>()
                .AfterMap((src, dest) =>
                {
                    dest.Status = src.Status.ToString();
                    dest.Priority = src.Priority.ToString();
                }).ReverseMap();
        }
    }
}
