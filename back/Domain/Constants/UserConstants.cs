namespace Domain.Constants;

public static class UserConstants
{
    public const int UsernameMinLength = 3;
    public const int UsernameMaxLength = 50;
    public const string UsernameRegex = "^[a-zA-Z0-9._]+$";

    public const string UsernameRegexMessage =
        "Ім'я користувача може містити літери, числа, крапки і нижні підкреслювання";

    public const int BioMaxLength = 500;
    public const int PasswordMinLength = 8;

    public const int UsernameChangeCooldownDays = 7;
}