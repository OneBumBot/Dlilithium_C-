using Dillithium.Helpers;
using Org.BouncyCastle.Crypto.Digests;

namespace Dillithium.Polynomials
{
    internal class RingPolynomial(int[] coefficients)
    {
        public int[] Coefficients { get; private set; } = ParseCoefficients(coefficients);


        public (RingPolynomial, RingPolynomial) Power2Round()
        {
            RingPolynomial r0_poly, r1_poly;

            int powerOf2 = 1 << Constants.d;
            int[] r0Coef = new int[0]; 
            int[] r1Coef = new int[0]; 

            foreach(var coef in Coefficients)
            {
                int r = coef % Constants.q;
                int r0 = DilithiumHelper.ReduceModPM(r, powerOf2);
                r1Coef = r1Coef.Append((r - r0) >> Constants.d).ToArray();
                r0Coef = r0Coef.Append(r0).ToArray();
            }

            r0_poly = new RingPolynomial(r0Coef);
            r1_poly = new RingPolynomial(r1Coef);


            return (r1_poly, r0_poly);
        }

        public (RingPolynomial, RingPolynomial) PowerRound()
        {
            RingPolynomial r0_poly = new RingPolynomial(new int[0]);
            RingPolynomial r1_poly = new RingPolynomial(new int[0]);

            for(int i =0; i < r1_poly.Coefficients.Length; ++i)
            {
                int a = this.Coefficients[i];
                r1_poly.Coefficients[i] = (a + (1 << (Constants.d - 1)) - 1) >> Constants.d;
                r0_poly.Coefficients[i] = a - (r1_poly.Coefficients[i] << Constants.d);
            }
            return (r1_poly, r0_poly);
        }

        public (RingPolynomial, RingPolynomial) Decompose(int a)
        {
            int[] highCoef = new int[0];
            int[] lowCoef = new int[0];

            foreach(var coef in Coefficients)
            {
                var (r1, r0) = DilithiumHelper.Decompose(coef, a);
                highCoef = highCoef.Append(r1).ToArray();  
                lowCoef = lowCoef.Append(r0).ToArray();  
            }

            RingPolynomial highCoefPoly = new RingPolynomial(highCoef);
            RingPolynomial lowCoefPoly = new RingPolynomial(lowCoef);

            return (highCoefPoly, lowCoefPoly);
        }

        public RingPolynomial[] decompose(int gamma2)
        {
            RingPolynomial[] pr = new RingPolynomial[2];
            pr[0] = new RingPolynomial(new int[0]);
            pr[1] = new RingPolynomial(new int[0]);

            for (int i = 0; i < this.Coefficients.Length; i++)
            {
                int a = this.Coefficients[i];

                int a1 = (a + 127) >> 7;
                if (gamma2 == (Constants.q - 1) / 32)
                {
                    a1 = (a1 * 1025 + (1 << 21)) >> 22;
                    a1 &= 15;
                }
                else if (gamma2 == (Constants.q - 1) / 88)
                {
                    a1 = (a1 * 11275 + (1 << 23)) >> 24;
                    a1 ^= ((43 - a1) >> 31) & a1;
                }
                pr[0].Coefficients[i] = a - a1 * 2 * gamma2;
                pr[0].Coefficients[i] -= (((Constants.q - 1) / 2 - pr[0].Coefficients[i]) >> 31) & Constants.q;
                pr[1].Coefficients[i] = a1;
            }

            return pr;
        }
        RingPolynomial GetHighBitsPolynomial(int a)
        {
            int[] newCoef = new int[Coefficients.Length];

            for(int i = 0; i < newCoef.Length; ++i)
            {
                newCoef[i] = DilithiumHelper.GetHighBits(Coefficients[i], a);
            }

            return new RingPolynomial(newCoef);
        }

        RingPolynomial GetLowBitsPolynomial(int a)
        {
            int[] newCoef = new int[Coefficients.Length];

            for (int i = 0; i < newCoef.Length; ++i)
            {
                newCoef[i] = DilithiumHelper.GetLowBits(Coefficients[i], a);
            }

            return new RingPolynomial(newCoef);
        }

        public static (int,RingPolynomial) MakeHintPoly(RingPolynomial r0, RingPolynomial r1)
        {
            
            int[] hintCoef = new int[Constants.n];
            int sum = 0;
            for(int i = 0; i<Constants.n; ++i)
            {
                hintCoef[i] = DilithiumHelper.MakeHint(r0.Coefficients[i], r1.Coefficients[i]);
                sum += hintCoef[i];
            }

            return (sum, new RingPolynomial(hintCoef));
        }

        public static RingPolynomial UseHintPoly(RingPolynomial h, RingPolynomial r, int a)
        {
            int[] hintCoef = new int[Constants.n];

            for(int i = 0;i < Constants.n; i++)
            {
                hintCoef[i] = DilithiumHelper.UseHint(h.Coefficients[i], r.Coefficients[i],a);
            }

            return new RingPolynomial(hintCoef);
        }

        public static RingPolynomial UseHint(RingPolynomial a, RingPolynomial h)
        {
            int[] hintCoef = new int[Constants.n];

            for (int i = 0; i < Constants.n; i++)
            {
                hintCoef[i] = DilithiumHelper.useHint(a.Coefficients[i], h.Coefficients[i]);
            }

            return new RingPolynomial(hintCoef);
        }


        private static int[] ParseCoefficients(int[] coefficients)
        {
            int len = coefficients.Length;

            if (len > Constants.n)
                throw new Exception($"Коэффициенты описывают полином степени большей, чем максимальная степень {Constants.n}");
            else if (len < Constants.n)
            {
                int[] newCoef = new int[Constants.n - len];
                for(int i = 0; i < Constants.n - len; ++i)
                {
                    newCoef[i] = 0;
                }
                coefficients = coefficients.Concat(newCoef).ToArray();
            }
            return coefficients;
        }

        public static RingPolynomial GenerateChallenge(byte[] seed)
        {
            int[] resCoef = new int[Constants.n];

            int b = 0;
            int pos;

            long signs;

            byte[] buf = new byte[Constants.shake256Rate];

            ShakeDigest s = new ShakeDigest(256);

            s.BlockUpdate(seed, 0, seed.Length);
            s.Output(buf, 0, buf.Length);

            signs = 0;

            for(int i = 0; i < 8; ++i)
            {
                signs |= (long)(buf[i] & 0xFF) << (8 * i);
            }

            pos = 8;

            for( int i = Constants.n - Constants.tau; i < Constants.n; ++i)
            {
                do
                {
                    if(pos > Constants.shake256Rate)
                    {
                        s.Output(buf, 0, buf.Length);
                        pos = 0;
                    }

                    b = (buf[pos++] & 0xFF);
                } while (b > i);

                resCoef[i] = resCoef[b];
                resCoef[b] = (int)(1 - 2 *(signs & 1));
                signs >>= 1;
            }
            return new RingPolynomial(resCoef);
        }

        public bool CheckNormBound(int bound)
        {
            for (int i = 0; i < Constants.n; ++i)
            {
                bool f = DilithiumHelper.CheckNormBound(coefficients[i], bound);
                if(f == true)
                    return true;
            }
            return false;
        }

        public void Caddq()
        {
            for (int i = 0; i < Constants.n; ++i)
            {
               this.Coefficients[i] = NTTHelper.Caddq(this.Coefficients[i]);
            }
        }

        public void Reduce()
        {
            for (int i = 0; i < Constants.n; ++i)
            {
                this.Coefficients[i] = NTTHelper.Reduce32Bits(this.Coefficients[i]);
            }
        }

        public void ShiftL()
        {
            for(int i = 0; i < this.Coefficients.Length; ++i)
            {
                this.Coefficients[i] <<= Constants.d;
            }
        }

        public RingPolynomial TransformToNTT()
        {
            RingPolynomial p = new RingPolynomial(new int[Constants.n]);
            
            for(int i =0; i < Coefficients.Length; ++i)
            {
                p.Coefficients[i] = Coefficients[i];
            }

            int[] newCoef = NTTHelper.TransformToNTT(p);
            p.Coefficients = newCoef;
            return p;
        }

        public void TransformFromNTTToMontgomery()
        {
            NTTHelper.TransformFromNTTToMontgomery(this);
        }

        public RingPolynomial PointwiseMontgomery(RingPolynomial other)
        {
            int[] resCoef = new int[Constants.n];

            for(int i = 0; i <  resCoef.Length; ++i)
            {
                resCoef[i] = NTTHelper.MontgomeryReduce((long)this.Coefficients[i] * other.Coefficients[i]);
            }

            return new RingPolynomial(resCoef);
        }

        private static int RejUniform(int[] coef, int off, int len, byte[] buf, int buflen)
        {
            int ctr, pos;
            int t;


            ctr = pos = 0;


            while(ctr < len && pos + 3 <= buflen)
            {
                t = buf[pos++];
                t |= (int)buf[pos++] << 8;
                t |= (int)buf[pos++] << 16;
                t &= 0x7FFFFF;

                if (t < Constants.q)
                    coef[off + ctr++] = t;
            }
            return ctr;
        }

        public static RingPolynomial GenUniformRandom(byte[] seed, ushort nonce)
        {
            int polyUniformNBlocks = (768 + Constants.stream128BlockBytes - 1) / Constants.stream128BlockBytes;
            int ctr, off;

            int buflen = polyUniformNBlocks * Constants.stream128BlockBytes;

            byte[] buf = new byte[buflen + 2];

            ShakeDigest s = new ShakeDigest(128);

            s.BlockUpdate(seed, 0, seed.Length);

            byte[] non = new byte[2];

            non[0] = (byte)(nonce & 0xFF);
            non[1] = (byte)((nonce >> 8) & 0xFF);

            s.BlockUpdate(non, 0, 2);
            s.Output(buf, 0, buflen);

            int[] preCoef = new int[Constants.n];

            ctr = RejUniform(preCoef, 0, Constants.n, buf, buflen);

            while(ctr < Constants.n)
            {
                off = buflen % 3;
                for(int i =0; i < off; ++i)
                {
                    buf[i] = buf[buflen - off + 1];
                }
                s.Output(buf, off, Constants.stream128BlockBytes);
                buflen = Constants.stream128BlockBytes + off;
                ctr += RejUniform(preCoef, ctr, Constants.n - ctr, buf, buflen);
            }

            return new RingPolynomial(preCoef);
        }

        private static int rej_eta(int eta, int[] coef, int off, int len, byte[] buf, int buflen)
        {
            int ctr, pos;
            int t0, t1;

            ctr = pos = 0;

            if (eta == 2)
            {
                while (ctr < len && pos < buflen)
                {
                    t0 = buf[pos] & 0x0F;
                    t1 = (buf[pos++] >> 4) & 0x0F;
                    if (t0 < 15)
                    {
                        t0 = t0 - (205 * t0 >> 10) * 5;
                        coef[off + ctr++] = 2 - t0;
                    }
                    if (t1 < 15 && ctr < len)
                    {
                        t1 = t1 - (205 * t1 >> 10) * 5;
                        coef[off + ctr++] = 2 - t1;
                    }
                }
            }
            else
            {
                while (ctr < len && pos < buflen)
                {
                    t0 = buf[pos] & 0x0F;
                    t1 = (buf[pos++] >> 4) & 0x0F;
                    if (t0 < 9)
                        coef[off + ctr++] = 4 - t0;
                    if (t1 < 9 && ctr < len)
                        coef[off + ctr++] = 4 - t1;
                }
            }

            return ctr;
        }

        public static RingPolynomial genUniformEta(byte[] seed,int eta, ushort nonce)
        {
            int polyUniformEtaNBlocks;
            if(eta == 2)
            {
                polyUniformEtaNBlocks = (136 + Constants.stream256BlockBytes - 1) / Constants.stream256BlockBytes;
            }
            else if(eta == 4)
            {
                polyUniformEtaNBlocks = (227 + Constants.stream256BlockBytes - 1) / Constants.stream256BlockBytes;
            }
            else
            {
                throw new ArgumentException("неподходящий аргумент eta: " + eta);
            }

            int ctr;

            ShakeDigest s = new ShakeDigest(256);
            s.BlockUpdate(seed, 0, seed.Length);

            byte[] non = new byte[2];

            non[0] = (byte)(nonce & 0xFF);
            non[1] = (byte)((nonce >> 8) & 0xFF);

            s.BlockUpdate(non, 0, 2);


            byte[] bb = new byte[polyUniformEtaNBlocks * Constants.stream256BlockBytes];

            s.Output(bb, 0, bb.Length);


            int[] preCoef = new int[Constants.n];

            ctr = rej_eta(eta, preCoef, 0, Constants.n, bb, bb.Length);

            while(ctr < Constants.n)
            {
                s.Output(bb, 0, Constants.stream256BlockBytes);
                ctr += rej_eta(eta, preCoef, ctr, Constants.n - ctr, bb, Constants.stream256BlockBytes);
            }

            return new RingPolynomial(preCoef);
        }

        public static RingPolynomial genRandomGamma1(byte[] seed, ushort nonce)
        {
            byte[] buf = new byte[Constants.polyUniformGamma1NBlocks * Constants.stream256BlockBytes];

            ShakeDigest s = new ShakeDigest(256);

            s.BlockUpdate(seed, 0, seed.Length);


            byte[] non = new byte[2];

            non[0] = (byte)(nonce & 0xFF);
            non[1] = (byte)((nonce >> 8) & 0xFF);

            s.BlockUpdate(non, 0, 2);

            s.Output(buf, 0, buf.Length);

            RingPolynomial pre = PackingHelper.UnPackZ(buf, 0);

            return pre;
        }
        public static RingPolynomial operator +(RingPolynomial a, RingPolynomial b)
        {
            int[] resCoef = new int[Constants.n];

            for(int i = 0; i <  resCoef.Length; ++i)
            {
                resCoef[i] = (a.Coefficients[i] + b.Coefficients[i]);
            }

            return new RingPolynomial(resCoef);
        }

        public static RingPolynomial operator -(RingPolynomial a, RingPolynomial b)
        {
            int[] resCoef = new int[Constants.n];

            for (int i = 0; i < resCoef.Length; ++i)
            {
                resCoef[i] = (a.Coefficients[i] - b.Coefficients[i]);
            }

            return new RingPolynomial(resCoef);
        }

    }
}
