using Dillithium.Polynomials;

namespace Dillithium.DillithiumObjects
{
    internal class DillithiumPublicKey
    {
        public readonly byte[] Seed;
        public readonly PolynomialVector t1;

        public DillithiumPublicKey(byte[] Seed, PolynomialVector t1)
        {
            this.Seed = Seed;
            this.t1 = t1;
        }

    }
}
