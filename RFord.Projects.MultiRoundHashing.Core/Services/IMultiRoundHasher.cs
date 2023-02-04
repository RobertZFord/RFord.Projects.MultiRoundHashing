using System.Security.Cryptography;

namespace RFord.Projects.MultiRoundHashing.Core.Services
{
    public interface IMultiRoundHasher
    {
        byte[] ComputeHash(Stream source, HashAlgorithm hashAlgorithm, int rounds, Delegate? onIncrement = default);
    }
}