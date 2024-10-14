using Dillithium.Polynomials;

namespace Dillithium.DillithiumObjects
{
    internal class DillithiumPrivateKey
    {
        public readonly byte[] Seed;
        public readonly byte[] K;
        public readonly byte[] Tr;
        public readonly PolynomialVector S1;
        public readonly PolynomialVector S2;
        public readonly PolynomialVector T0;
    
        public DillithiumPrivateKey(byte[] seed, byte[] k, byte[] tr, PolynomialVector s1, PolynomialVector s2, PolynomialVector t0)
        {
            Seed = seed;
            K = k;
            Tr = tr;
            S1 = s1;
            S2 = s2;
            T0 = t0;
        }
    }
}
