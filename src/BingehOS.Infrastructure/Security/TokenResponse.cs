namespace BingehOS.Infrastructure.Security;

public record TokenResponse(string AccessToken, string TokenType = "Bearer", int ExpiresIn = 3600);
