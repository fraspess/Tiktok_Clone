using System.ComponentModel;

namespace Domain;

public enum UserReportReasons
{
    Impersonation,
    BotActivity,
    MultiplePolicyViolations,
    SuspiciousActivity,
    UnauthorizedAccess,
    UnderageUser,
    IllegalContent,
    CommunityGuidelinesViolation
}