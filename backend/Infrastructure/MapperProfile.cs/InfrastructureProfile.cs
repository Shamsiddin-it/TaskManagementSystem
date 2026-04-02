using AutoMapper;
using Domain.Models;
using Domain.Enums;
using TaskEntity = Domain.Models.Task;

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
        CreateMap<TaskEntity, Application.DTOs.GetTaskDto>()
            .ForMember(dest => dest.Team, opt => opt.Ignore())
            .ForMember(dest => dest.AssignedTo, opt => opt.MapFrom(src => src.AssignedToId))
            .ForMember(dest => dest.AssignedToUser, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedById))
            .ForMember(dest => dest.CreatedByUser, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority.ToString()));
        CreateMap<Application.DTOs.CreateTaskDto, TaskEntity>()
            .ForMember(dest => dest.AssignedToId, opt => opt.MapFrom(src => src.AssignedTo ?? src.AssignedToId ?? string.Empty))
            .ForMember(dest => dest.CreatedById, opt => opt.MapFrom(src => src.CreatedById ?? src.CreatedBy))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => ParseTaskStatus(src.Status)))
            .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => ParseTaskPriority(src.Priority)))
            .ForMember(dest => dest.TicketType, opt => opt.MapFrom(src => ParseTicketType(src.TicketType)))
            .ForMember(dest => dest.SprintId, opt => opt.MapFrom(src => src.SprintId))
            .ForMember(dest => dest.StoryPoints, opt => opt.MapFrom(src => src.StoryPoints))
            .ForMember(dest => dest.Team, opt => opt.Ignore())
            .ForMember(dest => dest.AssignedTo, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Sprint, opt => opt.Ignore())
            .ForMember(dest => dest.Subtasks, opt => opt.Ignore())
            .ForMember(dest => dest.Comments, opt => opt.Ignore())
            .ForMember(dest => dest.TaskTags, opt => opt.Ignore())
            .ForMember(dest => dest.Attachments, opt => opt.Ignore())
            .ForMember(dest => dest.TimeLogs, opt => opt.Ignore())
            .ForMember(dest => dest.ActivityLogs, opt => opt.Ignore())
            .ForMember(dest => dest.IsBlocked, opt => opt.Ignore())
            .ForMember(dest => dest.BlockedReason, opt => opt.Ignore())
            .ForMember(dest => dest.TotalTimeMinutes, opt => opt.Ignore())
            .ForMember(dest => dest.IsArchived, opt => opt.Ignore())
            .ForMember(dest => dest.TicketCode, opt => opt.Ignore());
        CreateMap<Application.DTOs.UpdateTaskDto, TaskEntity>()
            .ForMember(dest => dest.AssignedToId, opt => opt.MapFrom(src => src.AssignedTo))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => ParseTaskStatus(src.Status)))
            .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => ParseTaskPriority(src.Priority)))
            .ForMember(dest => dest.TeamId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedById, opt => opt.Ignore())
            .ForMember(dest => dest.SprintId, opt => opt.Ignore())
            .ForMember(dest => dest.TicketType, opt => opt.Ignore())
            .ForMember(dest => dest.StoryPoints, opt => opt.Ignore())
            .ForMember(dest => dest.IsBlocked, opt => opt.Ignore())
            .ForMember(dest => dest.BlockedReason, opt => opt.Ignore())
            .ForMember(dest => dest.TotalTimeMinutes, opt => opt.Ignore())
            .ForMember(dest => dest.IsArchived, opt => opt.Ignore())
            .ForMember(dest => dest.TicketCode, opt => opt.Ignore());
        CreateMap<TaskTag, GetTaskTagDto>();
        CreateMap<InsertTaskTagDto, TaskTag>();
        CreateMap<UpdateTaskTagDto, TaskTag>();
        CreateMap<TeamMember, GetTeamMemberDto>();
        CreateMap<InsertTeamMemberDto, TeamMember>();
        CreateMap<UpdateTeamMemberDto, TeamMember>();
    }

    private static TaskStatus ParseTaskStatus(string? value)
    {
        return Enum.TryParse<TaskStatus>(value, true, out var status)
            ? status
            : TaskStatus.Todo;
    }

    private static TaskPriority ParseTaskPriority(string? value)
    {
        return Enum.TryParse<TaskPriority>(value, true, out var priority)
            ? priority
            : TaskPriority.Medium;
    }

    private static TicketType ParseTicketType(string? value)
    {
        return Enum.TryParse<TicketType>(value, true, out var ticketType)
            ? ticketType
            : TicketType.Task;
    }
}
