using InterviewPlatform.Core.Enums;

namespace InterviewPlatform.Application.Interfaces;

public interface ICurrentUserContext
{
    Guid UserId { get; }
    Role Role { get; }
    bool IsAdmin { get; }
    bool IsCreator { get; }
}
