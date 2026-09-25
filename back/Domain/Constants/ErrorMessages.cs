namespace Domain.Constants;

public static class ErrorMessages
{
    public static string For(string code) => code switch
    {
        ErrorCodes.InvalidCredentials => "Invalid email or password.",
        ErrorCodes.EmailNotConfirmed => "Please confirm your email address.",
        ErrorCodes.Unauthorized => "Authentication is required.",
        ErrorCodes.Forbidden => "You do not have permission to perform this action.",
        ErrorCodes.MessagesNotAccepted => "This user does not accept messages from you.",
        ErrorCodes.CommentNotFound => "Comment not found.",
        ErrorCodes.VideoNotFound => "Video not found.",
        ErrorCodes.UserNotFound => "User not found.",
        ErrorCodes.ResourceNotFound => "Resource not found.",
        ErrorCodes.Invalid => "The request is invalid.",
        ErrorCodes.InvalidFormat => "The value has an invalid format.",
        ErrorCodes.InvalidLength => "The value has an invalid length.",
        ErrorCodes.TooShort => "The value is too short.",
        ErrorCodes.TooLong => "The value is too long.",
        ErrorCodes.OutOfRange => "The value is out of range.",
        ErrorCodes.InvalidValue => "The value is invalid.",
        ErrorCodes.AlreadyExists => "The resource already exists.",
        ErrorCodes.EmailAlreadyExists => "An account with this email already exists.",
        ErrorCodes.UsernameAlreadyExists => "This username is already taken.",
        ErrorCodes.InvalidUsername => "The username is invalid.",
        ErrorCodes.InvalidFileType => "The uploaded file type is not allowed.",
        ErrorCodes.InvalidToken => "The token is invalid or expired.",
        ErrorCodes.InternalServerError => "An internal server error occurred.",
        ErrorCodes.ValidationError => "Validation failed.",
        ErrorCodes.EmailAlreadyConfirmed => "This email address has already been confirmed.",
        ErrorCodes.TooFast => "Please wait before trying again.",
        ErrorCodes.GoogleLoginFailed => "Google sign-in failed.",
        ErrorCodes.UserBanned => "This account has been banned.",
        ErrorCodes.CantResetPasswordExternal => "Password reset is unavailable for external-login accounts.",
        ErrorCodes.TooManyRequests => "Too many requests. Please try again later.",
        ErrorCodes.ExternalLoginOnly => "This account can only sign in through its external provider.",
        ErrorCodes.CooldownOnChangeUsername => "You cannot change your username yet.",
        ErrorCodes.InvalidContentType => "The content type is invalid.",
        ErrorCodes.MessageNotFound => "Message not found.",
        ErrorCodes.ConversationNotFound => "Conversation not found.",
        _ => "An unexpected error occurred."
    };
}
