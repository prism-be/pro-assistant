namespace Prism.ProAssistant.Business.Users;

public record User(string Id, string Login, string PasswordHash, string Name);