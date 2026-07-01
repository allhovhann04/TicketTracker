using System.Security.Cryptography;
using System.Text;
using Konscious.Security.Cryptography;

namespace TicketTracker.Api.Services.Auth;

// Encodes params + salt + hash together so future tuning doesn't break existing hashes.
// Format: argon2id$v=19$m=<memoryKb>,t=<iterations>,p=<parallelism>$<saltBase64>$<hashBase64>
public class Argon2idPasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 4;
    private const int MemorySizeKb = 65536;
    private const int DegreeOfParallelism = 2;

    public string HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Hash(password, salt, MemorySizeKb, Iterations, DegreeOfParallelism, HashSize);

        return string.Join(
            '$',
            "argon2id",
            "v=19",
            $"m={MemorySizeKb},t={Iterations},p={DegreeOfParallelism}",
            Convert.ToBase64String(salt),
            Convert.ToBase64String(hash));
    }

    public bool VerifyPassword(string password, string encodedHash)
    {
        var parts = encodedHash.Split('$');
        if (parts.Length != 5 || parts[0] != "argon2id")
        {
            return false;
        }

        var parameters = parts[2].Split(',');
        var memorySize = int.Parse(parameters[0].Split('=')[1]);
        var iterations = int.Parse(parameters[1].Split('=')[1]);
        var parallelism = int.Parse(parameters[2].Split('=')[1]);

        var salt = Convert.FromBase64String(parts[3]);
        var expectedHash = Convert.FromBase64String(parts[4]);

        var actualHash = Hash(password, salt, memorySize, iterations, parallelism, expectedHash.Length);

        return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
    }

    private static byte[] Hash(string password, byte[] salt, int memorySizeKb, int iterations, int parallelism, int hashSize)
    {
        using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
        {
            Salt = salt,
            DegreeOfParallelism = parallelism,
            Iterations = iterations,
            MemorySize = memorySizeKb
        };

        return argon2.GetBytes(hashSize);
    }
}
