using System.Security.Cryptography;

namespace RFord.Projects.MultiRoundHashing.Core.Services
{
    public class MultiRoundHasher : IMultiRoundHasher
    {
        public byte[] ComputeHash(Stream source, HashAlgorithm hash, int roundCount, Delegate? onIncrement = default)
        {
            byte[] result = new byte[0];
            for (int i = 0; i < roundCount; i++)
            {
                result = i == 0 ? hash.ComputeHash(source) : hash.ComputeHash(result);
                onIncrement?.DynamicInvoke();
            }

            return result;
        }
    }
}
