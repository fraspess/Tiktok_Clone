namespace Domain.Constants;

public static class ErrorCodes
{
    public const string InvalidCredentials = "INVALID_CREDENTIALS";
    public const string EmailNotConfirmed = "EMAIL_NOT_CONFIRMED";
    public const string Unauthorized = "UNAUTHORIZED";
    public const string Forbidden = "FORBIDDEN";
    
    public const string CommentNotFound = "COMMENT_NOT_FOUND";
    public const string VideoNotFound = "VIDEO_NOT_FOUND";
    public const string UserNotFound = "USER_NOT_FOUND";
    public const string ResourceNotFound = "RESOURCE_NOT_FOUND";
    
    public const string Required = "REQUIRED";
    public const string Invalid = "INVALID";
    public const string InvalidFormat = "INVALID_FORMAT";
    public const string InvalidLength = "INVALID_LENGTH";
    public const string TooShort = "TOO_SHORT";
    public const string TooLong = "TOO_LONG";
    public const string OutOfRange = "OUT_OF_RANGE";
    public const string InvalidValue = "INVALID_VALUE";
    public const string Duplicate = "DUPLICATE";
    public const string AlreadyExists = "ALREADY_EXISTS";
    
    public const string Empty = "EMPTY";
    public const string Blank = "BLANK";
    public const string InvalidCharacters = "INVALID_CHARACTERS";
    
    public const string EmailRequired = "EMAIL_REQUIRED";
    public const string InvalidEmail = "INVALID_EMAIL";
    public const string EmailAlreadyExists = "EMAIL_ALREADY_EXISTS";
    
    public const string UsernameRequired = "USERNAME_REQUIRED";
    public const string UsernameAlreadyExists = "USERNAME_ALREADY_EXISTS";
    public const string InvalidUsername = "INVALID_USERNAME";
    
    public const string PasswordRequired = "PASSWORD_REQUIRED";
    public const string InvalidPassword = "INVALID_PASSWORD";
    public const string WeakPassword = "WEAK_PASSWORD";
    
    
    public const string FileRequired = "FILE_REQUIRED";
    public const string FileTooLarge = "FILE_TOO_LARGE";
    public const string InvalidFileType = "INVALID_FILE_TYPE";
    
    public const string InvalidToken = "INVALID_TOKEN";
    public const string ExpiredToken = "EXPIRED_TOKEN";
    
    public const string InternalServerError = "INTERNAL_SERVER_ERROR";
    
    public const string ValidationError = "VALIDATION_ERROR";
    public const string EmailAlreadyConfirmed = "EMAIL_ALREADY_CONFIRMED";
    public const string TooFast = "TOO_FAST";
    public const string GoogleLoginFailed = "GOOGLE_LOGIN_FAILED";
    public const string UserBanned = "USER_IS_BANNED";
    public const string CantResetPasswordExternal = "CANT_RESET_PASSWORD_EXTERNAL";
    public const string TooManyRequests = "TOO_MANY_REQUESTS";
    public const string ExternalLoginOnly = "EXTERNAL_LOGIN_ONLY";
    public const string CooldownOnChangeUsername = "COOLDOWN_ON_CHANGE_USERNAME";
}