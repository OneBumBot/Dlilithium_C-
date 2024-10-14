using Dillithium.Helpers;

namespace Dillithium.Polynomials
{
    internal class PolynomialVector
    {
        public RingPolynomial[] Polynomials { get; private set; }

        public PolynomialVector(int size)
        {
            this.Polynomials = new RingPolynomial[size];
            for(int i =0; i < this.Polynomials.Length; ++i)
            {
                this.Polynomials[i] = new RingPolynomial(new int[0]);
            }
        }

        public bool CheckNorm(int bound)
        {
            foreach(RingPolynomial p in Polynomials)
                if(p.CheckNormBound(bound))
                    return true;
            return false;
        }

        public PolynomialVector TransformToNTT()
        {
            RingPolynomial[] newPolys = new RingPolynomial[Polynomials.Length];

            for(int i = 0; i <  newPolys.Length; ++i)
            {
                newPolys[i] = this.Polynomials[i].TransformToNTT();
            }

            PolynomialVector res = new PolynomialVector(Polynomials.Length);
            res.Polynomials = newPolys;
            return res;
        }

        public void TransformFromNTTToMontgomery()
        {
            foreach(var p in Polynomials)
            {
                p.TransformFromNTTToMontgomery();
            }
        }

        public void Reduce()
        {
            foreach( var p in Polynomials)
            {
                p.Reduce();
            }
        }
        public void CAddQ()
        {
            foreach(var p in this.Polynomials)
            {
                p.Caddq();
            }
        }

        public void ShiftL()
        {
            foreach(var p in this.Polynomials)
            {
                p.ShiftL();
            }
        }

        public static PolynomialVector[] ExpandAMatrix(byte[] seed)
        {
            PolynomialVector[] A = new PolynomialVector[Constants.k];

            for(int i = 0; i < Constants.k; ++i)
            {
                A[i] = new PolynomialVector(Constants.l);
                for(int j = 0; j < Constants.l; j++)
                {
                    A[i].Polynomials[j] = RingPolynomial.GenUniformRandom(seed, (ushort)((i << 8) + j));
                }
            }
            return A;
        }



        public static PolynomialVector UseHintVector(PolynomialVector h, PolynomialVector r,int a) 
        {
            PolynomialVector res = new PolynomialVector(r.Polynomials.Length);

            for(int i =0; i < res.Polynomials.Length; ++i)
            {

                res.Polynomials[i] = RingPolynomial.UseHintPoly(h.Polynomials[i], r.Polynomials[i], a);
            }
            return res; 
        }

        public static PolynomialVector UseHint(PolynomialVector a, PolynomialVector h)
        {
            PolynomialVector res = new PolynomialVector(a.Polynomials.Length);
            for (int i = 0; i < res.Polynomials.Length; ++i)
            {

                res.Polynomials[i] = RingPolynomial.UseHint(a.Polynomials[i], h.Polynomials[i]);
            }
            return res;
        }
        public static (int, PolynomialVector) MakeHint(PolynomialVector v0, PolynomialVector v1)
        {
            int sum = 0;
            PolynomialVector res = new PolynomialVector(v0.Polynomials.Length);

            for(int i = 0; i < v0.Polynomials.Length; ++i)
            {
                var (cnt, v) = RingPolynomial.MakeHintPoly(v0.Polynomials[i], v1.Polynomials[i]);
                res.Polynomials[i] = v;
                sum += cnt;
            }

            return (sum, res);
        }

        public static PolynomialVector generateRandomVectorEta(byte[] seed, int len, ushort nonce)
        {
            PolynomialVector pv = new PolynomialVector(len);
            for(int i = 0; i < len; ++i)
            {
                pv.Polynomials[i] = RingPolynomial.genUniformEta(seed, Constants.eta, nonce++);

            }

            return pv;
        }

        public static PolynomialVector generateRandomVectorGamma1(byte[] seed, int len, ushort nonce)
        {
            PolynomialVector z = new PolynomialVector(len);

            for(int i = 0; i < len; ++i)
            {
                z.Polynomials[i] = RingPolynomial.genRandomGamma1(seed, (ushort)(len * nonce + i));
            }

            return z;
        }

        public (PolynomialVector, PolynomialVector) Decompose(int a)
        {
            PolynomialVector r0 = new PolynomialVector(this.Polynomials.Length);
            PolynomialVector r1 = new PolynomialVector(this.Polynomials.Length);

            for(int i =0; i < this.Polynomials.Length; ++i)
            {
                var (rr1, rr0) = Polynomials[i].Decompose(a);
                r0.Polynomials[i] = rr0;
                r1.Polynomials[i] = rr1;
            }

            return (r1, r0);
        }

        public PolynomialVector[] decompose(int gamma2)
        {
            PolynomialVector[] res = new PolynomialVector[2];
            res[0] = new PolynomialVector(this.Polynomials.Length);
            res[1] = new PolynomialVector(this.Polynomials.Length);
            for (int i = 0; i < this.Polynomials.Length; i++)
            {
                RingPolynomial[] r = Polynomials[i].decompose(gamma2);
                res[0].Polynomials[i] = r[0];
                res[1].Polynomials[i] = r[1];
            }
            return res;
        }


        public (PolynomialVector, PolynomialVector) Power2Round()
        {
            PolynomialVector r0VecPoly = new PolynomialVector(this.Polynomials.Length);
            PolynomialVector r1VecPoly = new PolynomialVector(this.Polynomials.Length);

            for(int i = 0; i <  Polynomials.Length; ++i)
            {
                var (r1, r0) = Polynomials[i].Power2Round();

                r0VecPoly.Polynomials[i] = r0;
                r1VecPoly.Polynomials[i] = r1;
            }

            return (r1VecPoly, r0VecPoly);
        }
        public (PolynomialVector, PolynomialVector) PowerRound()
        {
            PolynomialVector r0VecPoly = new PolynomialVector(this.Polynomials.Length);
            PolynomialVector r1VecPoly = new PolynomialVector(this.Polynomials.Length);
            for (int i = 0; i < Polynomials.Length; ++i)
            {
                var (r1, r0) = Polynomials[i].PowerRound();

                r0VecPoly.Polynomials[i] = r0;
                r1VecPoly.Polynomials[i] = r1;
            }
            return (r1VecPoly, r0VecPoly);
        }

        public PolynomialVector PointwiseMontgomery(RingPolynomial other)
        {
            PolynomialVector res = new PolynomialVector(Polynomials.Length);
            for (int i = 0; i < res.Polynomials.Length; ++i)
            {
                res.Polynomials[i] = res.Polynomials[i].PointwiseMontgomery(other);
            }

            return res;
        }

        public RingPolynomial PointwiseAccMontgomery(PolynomialVector a, PolynomialVector b)
        {
            RingPolynomial res = a.Polynomials[0].PointwiseMontgomery(b.Polynomials[0]);
            for (int i = 1; i <  b.Polynomials.Length; ++i)
            {
                RingPolynomial t = a.Polynomials[i].PointwiseMontgomery(b.Polynomials[i]);
                res += t;
            }
            
            
            return res;
        }
        
        public PolynomialVector MultiplyMatrixPointwiseMontgomery(PolynomialVector[] M)
        {
            PolynomialVector res = new PolynomialVector(M.Length);

            for(int i =0; i < M.Length; ++i)
            {
                res.Polynomials[i] = PointwiseAccMontgomery(M[i], this);
            }

            return res;
        }

        public static PolynomialVector operator +(PolynomialVector a, PolynomialVector b)
        {
            PolynomialVector res = new PolynomialVector(a.Polynomials.Length);
            for(int i =0; i < a.Polynomials.Length; ++i)
            {
                res.Polynomials[i] = new RingPolynomial(new int[0]);
            }

            for(int i = 0; i < a.Polynomials.Length; ++i)
            {
                res.Polynomials[i] = a.Polynomials[i] + b.Polynomials[i];
            }

            return res;
        }

        public static PolynomialVector operator -(PolynomialVector a, PolynomialVector b)
        {


            PolynomialVector res = new PolynomialVector(a.Polynomials.Length);
            for (int i = 0; i < a.Polynomials.Length; ++i)
            {
                res.Polynomials[i] = new RingPolynomial(new int[0]);
            }


            for (int i = 0; i < a.Polynomials.Length; ++i)
            {
                res.Polynomials[i] = a.Polynomials[i] - b.Polynomials[i];
            }
            return res;
        }

        public int Length()
        {
            return Polynomials.Length;
        }

    }
}
