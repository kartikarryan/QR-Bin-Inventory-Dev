using System.Buffers.Binary;
using System.Security.Cryptography;

namespace QrBin.Api.Services;

public interface IPasswordHasher
{
    string HashPassword(string password);
    bool VerifyPassword(string passwordHash, string password);
}

/// <summary>
/// Self-contained PBKDF2-HMACSHA256 hashing — no ASP.NET Core Identity dependency.
/// Format: base64(salt[16] + iterations[4, big-endian] + subkey[32]), iterations stored
/// per-hash so the work factor can be raised later without breaking existing hashes.
/// </summary>
public class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16;
    private const int SubkeySize = 32;
    private const int Iterations = 100_000;

    public string HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var subkey = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, SubkeySize);

        var result = new byte[SaltSize + 4 + SubkeySize];
        Buffer.BlockCopy(salt, 0, result, 0, SaltSize);
        BinaryPrimitives.WriteInt32BigEndian(result.AsSpan(SaltSize, 4), Iterations);
        Buffer.BlockCopy(subkey, 0, result, SaltSize + 4, SubkeySize);

        return Convert.ToBase64String(result);
    }

    public bool VerifyPassword(string passwordHash, string password)
    {
        byte[] bytes;
        try
        {
            bytes = Convert.FromBase64String(passwordHash);
        }
        catch (FormatException)
        {
            return false;
        }

        if (bytes.Length != SaltSize + 4 + SubkeySize)
            return false;

        var salt = bytes.AsSpan(0, SaltSize).ToArray();
        var iterations = BinaryPrimitives.ReadInt32BigEndian(bytes.AsSpan(SaltSize, 4));
        var expectedSubkey = bytes.AsSpan(SaltSize + 4, SubkeySize).ToArray();

        var actualSubkey = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, SubkeySize);

        return CryptographicOperations.FixedTimeEquals(actualSubkey, expectedSubkey);
    }
}
