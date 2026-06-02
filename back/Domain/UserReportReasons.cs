using System.ComponentModel;

namespace Domain;

public enum UserReportReasons
{
    None = 0,
    [Description("Уособлення")]Impersonation = 1,
    [Description("Активність бота")]BotActivity = 2,
    [Description("Числені порушення політики")]MultiplePolicyViolations = 3,
    [Description("Підозріла активність")]SuspiciousActivity = 4,
    [Description("Неавторизований доступ")]UnauthorizedAccess = 5,
    [Description("Молодше мінімального віку")]UnderageUser = 6,
    [Description("Нелегальний контент")]IllegalContent = 7,
    [Description("Порушення правил спільноти")]CommunityGuidelinesViolation = 8,
}