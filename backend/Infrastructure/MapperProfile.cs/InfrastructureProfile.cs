using AutoMapper;
using Domain.Models;
using TaskEntity = global::Task;

public class InfrastructureProfile : Profile
{
    public InfrastructureProfile()
    {
        CreateMap<ActivityLog, GetActivityLogDto>();
        CreateMap<InsertActivityLogDto, ActivityLog>();
        CreateMap<UpdateActivityLogDto, ActivityLog>();
        CreateMap<RetroActionItem, GetRetroActionItemDto>();
        CreateMap<InsertRetroActionItemDto, RetroActionItem>();
        CreateMap<UpdateRetroActionItemDto, RetroActionItem>();
        CreateMap<Sprint, GetSprintDto>();
        CreateMap<InsertSprintDto, Sprint>();
        CreateMap<UpdateSprintDto, Sprint>();
        CreateMap<SprintRetro, GetSprintRetroDto>();
        CreateMap<InsertSprintRetroDto, SprintRetro>();
        CreateMap<UpdateSprintRetroDto, SprintRetro>();
        CreateMap<Subtask, GetSubtaskDto>();
        CreateMap<InsertSubtaskDto, Subtask>();
        CreateMap<UpdateSubtaskDto, Subtask>();
        CreateMap<Tag, GetTagDto>();
        CreateMap<InsertTagDto, Tag>();
        CreateMap<UpdateTagDto, Tag>();
        CreateMap<TaskEntity, GetTaskDto>();
        CreateMap<InsertTaskDto, TaskEntity>();
        CreateMap<UpdateTaskDto, TaskEntity>();
        CreateMap<TaskTag, GetTaskTagDto>();
        CreateMap<InsertTaskTagDto, TaskTag>();
        CreateMap<UpdateTaskTagDto, TaskTag>();
        CreateMap<TeamMember, GetTeamMemberDto>();
        CreateMap<InsertTeamMemberDto, TeamMember>();
        CreateMap<UpdateTeamMemberDto, TeamMember>();
    }
}
