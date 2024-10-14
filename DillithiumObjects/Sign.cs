using Dillithium.Polynomials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dillithium.DillithiumObjects
{
    internal class Sign
    {
        public readonly byte[] Challenge;
        public readonly PolynomialVector z;
        public readonly PolynomialVector h;

        public Sign(byte[] challenge, PolynomialVector z, PolynomialVector h)
        {
            Challenge = challenge;
            this.z = z;
            this.h = h;
        }
    }
}
