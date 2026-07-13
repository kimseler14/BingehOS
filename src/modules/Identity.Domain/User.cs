using BingehOS.Shared;

namespace BingehOS.Modules.Identity.Domain;

public class User : BaseEntity
{
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string FullName { get; private set; } = string.Empty;
    public bool IsActive { get; private set; } = true;
    public AuthProvider AuthProvider { get; private set; } = AuthProvider.Local;

    public ICollection<UserRole> UserRoles { get; private set; } = new List<UserRole>();

    public static User Create(Guid tenantId, string email, string passwordHash, string fullName)
        => new()
        {
            TenantId = tenantId,
            Email = email,
            PasswordHash = passwordHash,
            FullName = fullName,
            IsActive = true,
            AuthProvider = AuthProvider.Local
        };

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
    public void SetPasswordHash(string hash) => PasswordHash = hash;
}
