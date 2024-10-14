using Dillithium.DillithiumObjects;
using Dillithium.Polynomials;

namespace Dillithium.Helpers
{
    internal static class PackingHelper
    {

        public static void PackZ(byte[] buf, int off, RingPolynomial a)
        {
            long[] t = new long[4];

            if (Constants.gamma1 == 1 << 17)
            {
                for (int i = 0; i < Constants.n / 4; i++)
                {
                    t[0] = Constants.gamma1 - a.Coefficients[4 * i + 0] & 0xFFFFFFFFL;
                    t[1] = Constants.gamma1 - a.Coefficients[4 * i + 1] & 0xFFFFFFFFL;
                    t[2] = Constants.gamma1 - a.Coefficients[4 * i + 2] & 0xFFFFFFFFL;
                    t[3] = Constants.gamma1 - a.Coefficients[4 * i + 3] & 0xFFFFFFFFL;

                    buf[off + 9 * i + 0] = (byte)t[0];
                    buf[off + 9 * i + 1] = (byte)(t[0] >> 8);
                    buf[off + 9 * i + 2] = (byte)(t[0] >> 16);
                    buf[off + 9 * i + 2] |= (byte)(t[1] << 2);
                    buf[off + 9 * i + 3] = (byte)(t[1] >> 6);
                    buf[off + 9 * i + 4] = (byte)(t[1] >> 14);
                    buf[off + 9 * i + 4] |= (byte)(t[2] << 4);
                    buf[off + 9 * i + 5] = (byte)(t[2] >> 4);
                    buf[off + 9 * i + 6] = (byte)(t[2] >> 12);
                    buf[off + 9 * i + 6] |= (byte)(t[3] << 6);
                    buf[off + 9 * i + 7] = (byte)(t[3] >> 2);
                    buf[off + 9 * i + 8] = (byte)(t[3] >> 10);
                }
            }
            else if (Constants.gamma1 == 1 << 19)
            {
                for (int i = 0; i < Constants.n / 2; i++)
                {
                    t[0] = Constants.gamma1 - a.Coefficients[2 * i + 0];
                    t[1] = Constants.gamma1 - a.Coefficients[2 * i + 1];

                    buf[off + 5 * i + 0] = (byte)t[0];
                    buf[off + 5 * i + 1] = (byte)(t[0] >> 8);
                    buf[off + 5 * i + 2] = (byte)(t[0] >> 16);
                    buf[off + 5 * i + 2] |= (byte)(t[1] << 4);
                    buf[off + 5 * i + 3] = (byte)(t[1] >> 4);
                    buf[off + 5 * i + 4] = (byte)(t[1] >> 12);
                }
            }
            else
            {
                throw new ArgumentException("Недопустимый аргумент gamma1: " + Constants.gamma1);
            }
        }

        public static RingPolynomial UnPackZ(byte[] packed, int off)
        {
            int[] newCoef = new int[Constants.n];

            if (Constants.gamma1 == 1 << 17)
            {
                for (int i = 0; i < Constants.n / 4; i++)
                {
                    newCoef[4 * i + 0] = packed[off + 9 * i + 0] & 0xFF;
                    newCoef[4 * i + 0] |= (packed[off + 9 * i + 1] & 0xFF) << 8;
                    newCoef[4 * i + 0] |= (packed[off + 9 * i + 2] & 0xFF) << 16;
                    newCoef[4 * i + 0] &= 0x3FFFF;

                    newCoef[4 * i + 1] = (packed[off + 9 * i + 2] & 0xFF) >> 2;
                    newCoef[4 * i + 1] |= (packed[off + 9 * i + 3] & 0xFF) << 6;
                    newCoef[4 * i + 1] |= (packed[off + 9 * i + 4] & 0xFF) << 14;
                    newCoef[4 * i + 1] &= 0x3FFFF;

                    newCoef[4 * i + 2] = (packed[off + 9 * i + 4] & 0xFF) >> 4;
                    newCoef[4 * i + 2] |= (packed[off + 9 * i + 5] & 0xFF) << 4;
                    newCoef[4 * i + 2] |= (packed[off + 9 * i + 6] & 0xFF) << 12;
                    newCoef[4 * i + 2] &= 0x3FFFF;

                    newCoef[4 * i + 3] = (packed[off + 9 * i + 6] & 0xFF) >> 6;
                    newCoef[4 * i + 3] |= (packed[off + 9 * i + 7] & 0xFF) << 2;
                    newCoef[4 * i + 3] |= (packed[off + 9 * i + 8] & 0xFF) << 10;
                    newCoef[4 * i + 3] &= 0x3FFFF;

                    newCoef[4 * i + 0] = Constants.gamma1 - newCoef[4 * i + 0];
                    newCoef[4 * i + 1] = Constants.gamma1 - newCoef[4 * i + 1];
                    newCoef[4 * i + 2] = Constants.gamma1 - newCoef[4 * i + 2];
                    newCoef[4 * i + 3] = Constants.gamma1 - newCoef[4 * i + 3];
                }
            }
            else if (Constants.gamma1 == 1 << 19)
            {
                for (int i = 0; i < Constants.n / 2; ++i)
                {
                    newCoef[2 * i + 0] = packed[off + 5 * i + 0] & 0xFF;
                    newCoef[2 * i + 0] |= (packed[off + 5 * i + 1] & 0xFF) << 8;
                    newCoef[2 * i + 0] |= (packed[off + 5 * i + 2] & 0xFF) << 16;
                    newCoef[2 * i + 0] &= 0xFFFFF;

                    newCoef[2 * i + 1] = (packed[off + 5 * i + 2] & 0xFF) >> 4;
                    newCoef[2 * i + 1] |= (packed[off + 5 * i + 3] & 0xFF) << 4;
                    newCoef[2 * i + 1] |= (packed[off + 5 * i + 4] & 0xFF) << 12;
                    newCoef[2 * i + 0] &= 0xFFFFF;

                    newCoef[2 * i + 0] = Constants.gamma1 - newCoef[2 * i + 0];
                    newCoef[2 * i + 1] = Constants.gamma1 - newCoef[2 * i + 1];
                }
            }
            else
            {
                throw new ArgumentException("Недопустимый аргумент gamma1: " + Constants.gamma1);
            }

            return new RingPolynomial(newCoef);
        }

        public static void PackEta(byte[] buf, int off, RingPolynomial a)
        {
            byte[] t = new byte[8];

            if (Constants.eta == 2)
            {
                for (int i = 0; i < Constants.n / 8; ++i)
                {
                    t[0] = (byte)(Constants.eta - a.Coefficients[8 * i + 0]);
                    t[1] = (byte)(Constants.eta - a.Coefficients[8 * i + 1]);
                    t[2] = (byte)(Constants.eta - a.Coefficients[8 * i + 2]);
                    t[3] = (byte)(Constants.eta - a.Coefficients[8 * i + 3]);
                    t[4] = (byte)(Constants.eta - a.Coefficients[8 * i + 4]);
                    t[5] = (byte)(Constants.eta - a.Coefficients[8 * i + 5]);
                    t[6] = (byte)(Constants.eta - a.Coefficients[8 * i + 6]);
                    t[7] = (byte)(Constants.eta - a.Coefficients[8 * i + 7]);

                    buf[off + 3 * i + 0] = (byte)(t[0] >> 0 | t[1] << 3 | t[2] << 6);
                    buf[off + 3 * i + 1] = (byte)(t[2] >> 2 | t[3] << 1 | t[4] << 4 | t[5] << 7);
                    buf[off + 3 * i + 2] = (byte)(t[5] >> 1 | t[6] << 2 | t[7] << 5);
                }
            }
            else if (Constants.eta == 4)
            {
                for (int i = 0; i < Constants.n / 2; ++i)
                {
                    t[0] = (byte)(Constants.eta - a.Coefficients[2 * i + 0]);
                    t[1] = (byte)(Constants.eta - a.Coefficients[2 * i + 1]);
                    buf[off + i] = (byte)(t[0] | t[1] << 4);
                }
            }
            else
            {
                throw new ArgumentException("Недопустимый аргумент eta: " + Constants.eta);
            }

        }

        public static RingPolynomial UnPackEta(byte[] packed, int off)
        {
            int[] newCoef = new int[Constants.n];
            if (Constants.eta == 2)
            {
                for (int i = 0; i < Constants.n / 8; ++i)
                {
                    newCoef[8 * i + 0] = (packed[off + 3 * i + 0] & 0xFF) >> 0 & 7;
                    newCoef[8 * i + 1] = (packed[off + 3 * i + 0] & 0xFF) >> 3 & 7;
                    newCoef[8 * i + 2] = ((packed[off + 3 * i + 0] & 0xFF) >> 6 | (packed[off + 3 * i + 1] & 0xFF) << 2) & 7;
                    newCoef[8 * i + 3] = (packed[off + 3 * i + 1] & 0xFF) >> 1 & 7;
                    newCoef[8 * i + 4] = (packed[off + 3 * i + 1] & 0xFF) >> 4 & 7;
                    newCoef[8 * i + 5] = ((packed[off + 3 * i + 1] & 0xFF) >> 7 | (packed[off + 3 * i + 2] & 0xFF) << 1) & 7;
                    newCoef[8 * i + 6] = (packed[off + 3 * i + 2] & 0xFF) >> 2 & 7;
                    newCoef[8 * i + 7] = (packed[off + 3 * i + 2] & 0xFF) >> 5 & 7;

                    newCoef[8 * i + 0] = Constants.eta - newCoef[8 * i + 0];
                    newCoef[8 * i + 1] = Constants.eta - newCoef[8 * i + 1];
                    newCoef[8 * i + 2] = Constants.eta - newCoef[8 * i + 2];
                    newCoef[8 * i + 3] = Constants.eta - newCoef[8 * i + 3];
                    newCoef[8 * i + 4] = Constants.eta - newCoef[8 * i + 4];
                    newCoef[8 * i + 5] = Constants.eta - newCoef[8 * i + 5];
                    newCoef[8 * i + 6] = Constants.eta - newCoef[8 * i + 6];
                    newCoef[8 * i + 7] = Constants.eta - newCoef[8 * i + 7];
                }
            }
            else if (Constants.eta == 4)
            {
                for (int i = 0; i < Constants.n / 2; ++i)
                {
                    newCoef[2 * i + 0] = packed[off + i] & 0xFF & 0x0F;
                    newCoef[2 * i + 1] = (packed[off + i] & 0xFF) >> 4;

                    newCoef[2 * i + 0] = Constants.eta - newCoef[2 * i + 0];
                    newCoef[2 * i + 1] = Constants.eta - newCoef[2 * i + 1];
                }
            }
            else
            {
                throw new ArgumentException("Недопустимый аргумент eta: " + Constants.eta);
            }

            return new RingPolynomial(newCoef);
        }

        public static void PackT1(byte[] buf, int off, RingPolynomial a)
        {
            for (int i = 0; i < Constants.n / 4; ++i)
            {
                buf[5 * i + 0 + off] = (byte)(a.Coefficients[4 * i + 0] >> 0);
                buf[5 * i + 1 + off] = (byte)(a.Coefficients[4 * i + 0] >> 8 | a.Coefficients[4 * i + 1] << 2);
                buf[5 * i + 2 + off] = (byte)(a.Coefficients[4 * i + 1] >> 6 | a.Coefficients[4 * i + 2] << 4);
                buf[5 * i + 3 + off] = (byte)(a.Coefficients[4 * i + 2] >> 4 | a.Coefficients[4 * i + 3] << 6);
                buf[5 * i + 4 + off] = (byte)(a.Coefficients[4 * i + 3] >> 2);
            }
        }

        public static RingPolynomial UnPackT1(byte[] packed, int off)
        {
            int[] newCoef = new int[Constants.n];

            for (int i = 0; i < Constants.n / 4; i++)
            {
                newCoef[4 * i + 0] = ((packed[off + 5 * i + 0] & 0xFF) >> 0 | (packed[off + 5 * i + 1] & 0xFF) << 8) & 0x3FF;
                newCoef[4 * i + 1] = ((packed[off + 5 * i + 1] & 0xFF) >> 2 | (packed[off + 5 * i + 2] & 0xFF) << 6) & 0x3FF;
                newCoef[4 * i + 2] = ((packed[off + 5 * i + 2] & 0xFF) >> 4 | (packed[off + 5 * i + 3] & 0xFF) << 4) & 0x3FF;
                newCoef[4 * i + 3] = ((packed[off + 5 * i + 3] & 0xFF) >> 6 | (packed[off + 5 * i + 4] & 0xFF) << 2) & 0x3FF;
            }

            return new RingPolynomial(newCoef);
        }

        public static void PackT0(byte[] buf, int off, RingPolynomial a)
        {
            int[] t = new int[8];

            for (int i = 0; i < Constants.n / 8; ++i)
            {
                t[0] = (1 << Constants.d - 1) - a.Coefficients[8 * i + 0];
                t[1] = (1 << Constants.d - 1) - a.Coefficients[8 * i + 1];
                t[2] = (1 << Constants.d - 1) - a.Coefficients[8 * i + 2];
                t[3] = (1 << Constants.d - 1) - a.Coefficients[8 * i + 3];
                t[4] = (1 << Constants.d - 1) - a.Coefficients[8 * i + 4];
                t[5] = (1 << Constants.d - 1) - a.Coefficients[8 * i + 5];
                t[6] = (1 << Constants.d - 1) - a.Coefficients[8 * i + 6];
                t[7] = (1 << Constants.d - 1) - a.Coefficients[8 * i + 7];

                buf[off + 13 * i + 0] = (byte)t[0];
                buf[off + 13 * i + 1] = (byte)(t[0] >> 8);
                buf[off + 13 * i + 1] |= (byte)(t[1] << 5);
                buf[off + 13 * i + 2] = (byte)(t[1] >> 3);
                buf[off + 13 * i + 3] = (byte)(t[1] >> 11);
                buf[off + 13 * i + 3] |= (byte)(t[2] << 2);
                buf[off + 13 * i + 4] = (byte)(t[2] >> 6);
                buf[off + 13 * i + 4] |= (byte)(t[3] << 7);
                buf[off + 13 * i + 5] = (byte)(t[3] >> 1);
                buf[off + 13 * i + 6] = (byte)(t[3] >> 9);
                buf[off + 13 * i + 6] |= (byte)(t[4] << 4);
                buf[off + 13 * i + 7] = (byte)(t[4] >> 4);
                buf[off + 13 * i + 8] = (byte)(t[4] >> 12);
                buf[off + 13 * i + 8] |= (byte)(t[5] << 1);
                buf[off + 13 * i + 9] = (byte)(t[5] >> 7);
                buf[off + 13 * i + 9] |= (byte)(t[6] << 6);
                buf[off + 13 * i + 10] = (byte)(t[6] >> 2);
                buf[off + 13 * i + 11] = (byte)(t[6] >> 10);
                buf[off + 13 * i + 11] |= (byte)(t[7] << 3);
                buf[off + 13 * i + 12] = (byte)(t[7] >> 5);
            }
        }

        public static RingPolynomial UnPackT0(byte[] packed, int off)
        {
            int[] newCoef = new int[Constants.n];

            for (int i = 0; i < Constants.n / 8; ++i)
            {
                newCoef[8 * i + 0] = packed[off + 13 * i + 0] & 0xFF;
                newCoef[8 * i + 0] |= (packed[off + 13 * i + 1] & 0xFF) << 8;
                newCoef[8 * i + 0] &= 0x1FFF;

                newCoef[8 * i + 1] = (packed[off + 13 * i + 1] & 0xFF) >> 5;
                newCoef[8 * i + 1] |= (packed[off + 13 * i + 2] & 0xFF) << 3;
                newCoef[8 * i + 1] |= (packed[off + 13 * i + 3] & 0xFF) << 11;
                newCoef[8 * i + 1] &= 0x1FFF;

                newCoef[8 * i + 2] = (packed[off + 13 * i + 3] & 0xFF) >> 2;
                newCoef[8 * i + 2] |= (packed[off + 13 * i + 4] & 0xFF) << 6;
                newCoef[8 * i + 2] &= 0x1FFF;

                newCoef[8 * i + 3] = (packed[off + 13 * i + 4] & 0xFF) >> 7;
                newCoef[8 * i + 3] |= (packed[off + 13 * i + 5] & 0xFF) << 1;
                newCoef[8 * i + 3] |= (packed[off + 13 * i + 6] & 0xFF) << 9;
                newCoef[8 * i + 3] &= 0x1FFF;

                newCoef[8 * i + 4] = (packed[off + 13 * i + 6] & 0xFF) >> 4;
                newCoef[8 * i + 4] |= (packed[off + 13 * i + 7] & 0xFF) << 4;
                newCoef[8 * i + 4] |= (packed[off + 13 * i + 8] & 0xFF) << 12;
                newCoef[8 * i + 4] &= 0x1FFF;

                newCoef[8 * i + 5] = (packed[off + 13 * i + 8] & 0xFF) >> 1;
                newCoef[8 * i + 5] |= (packed[off + 13 * i + 9] & 0xFF) << 7;
                newCoef[8 * i + 5] &= 0x1FFF;

                newCoef[8 * i + 6] = (packed[off + 13 * i + 9] & 0xFF) >> 6;
                newCoef[8 * i + 6] |= (packed[off + 13 * i + 10] & 0xFF) << 2;
                newCoef[8 * i + 6] |= (packed[off + 13 * i + 11] & 0xFF) << 10;
                newCoef[8 * i + 6] &= 0x1FFF;

                newCoef[8 * i + 7] = (packed[off + 13 * i + 11] & 0xFF) >> 3;
                newCoef[8 * i + 7] |= (packed[off + 13 * i + 12] & 0xFF) << 5;
                newCoef[8 * i + 7] &= 0x1FFF;

                newCoef[8 * i + 0] = (1 << Constants.d - 1) - newCoef[8 * i + 0];
                newCoef[8 * i + 1] = (1 << Constants.d - 1) - newCoef[8 * i + 1];
                newCoef[8 * i + 2] = (1 << Constants.d - 1) - newCoef[8 * i + 2];
                newCoef[8 * i + 3] = (1 << Constants.d - 1) - newCoef[8 * i + 3];
                newCoef[8 * i + 4] = (1 << Constants.d - 1) - newCoef[8 * i + 4];
                newCoef[8 * i + 5] = (1 << Constants.d - 1) - newCoef[8 * i + 5];
                newCoef[8 * i + 6] = (1 << Constants.d - 1) - newCoef[8 * i + 6];
                newCoef[8 * i + 7] = (1 << Constants.d - 1) - newCoef[8 * i + 7];
            }

            return new RingPolynomial(newCoef);
        }

        public static void PackW1(byte[] buf, int off, RingPolynomial a)
        {
            if (Constants.gamma2 == (Constants.q - 1) / 88)
            {
                for (int i = 0; i < Constants.n / 4; ++i)
                {
                    buf[off + 3 * i + 0] = (byte)a.Coefficients[4 * i + 0];
                    buf[off + 3 * i + 0] |= (byte)(a.Coefficients[4 * i + 1] << 6);
                    buf[off + 3 * i + 1] = (byte)(a.Coefficients[4 * i + 1] >> 2);
                    buf[off + 3 * i + 1] |= (byte)(a.Coefficients[4 * i + 2] << 4);
                    buf[off + 3 * i + 2] = (byte)(a.Coefficients[4 * i + 2] >> 4);
                    buf[off + 3 * i + 2] |= (byte)(a.Coefficients[4 * i + 3] << 2);
                }
            }
            else if (Constants.gamma2 == (Constants.q - 1) / 32)
            {
                for (int i = 0; i < Constants.n / 2; ++i)
                    buf[off + i] = (byte)(a.Coefficients[2 * i + 0] | a.Coefficients[2 * i + 1] << 4);

            }
            else
            {
                throw new ArgumentException("Недопустимый аргумент gamma2: " + Constants.gamma2);
            }
        }
        
        public static void PackW1Sign(PolynomialVector w1, byte[] sign)
        {
            int off = 0;
            for(int i = 0; i < w1.Polynomials.Length; ++i)
            {
                PackW1(sign, off, w1.Polynomials[i]);
                off += Constants.polyW1PacketBytes;
            }
        }
        public static byte[] PackPublicKey(byte[] rho, PolynomialVector t1)
        {
            byte[] pubKey = new byte[Constants.cryptoPublicKeyBytes];

            for(int i = 0; i < Constants.seedBytes; ++i)
            {
                pubKey[i] = rho[i];
            }

            for(int i = 0; i < t1.Length(); ++i)
            {
                PackT1(pubKey, Constants.seedBytes + i * Constants.polyT1PacketBytes, t1.Polynomials[i]);
            }

            return pubKey;
        }

        public static DillithiumPublicKey UnPackPublicKey(byte[] bytes)
        {
            byte[] rho = new byte[Constants.seedBytes];

            for(int i = 0; i < Constants.seedBytes; ++i)
            {
                rho[i] = bytes[i];
            }

            int off = Constants.seedBytes;

            PolynomialVector t1 = new PolynomialVector(Constants.k);

            for(int i = 0; i < Constants.k; ++i)
            {
                t1.Polynomials[i] = UnPackT1(bytes, off);
                off += Constants.polyT1PacketBytes;
            }

            return new DillithiumPublicKey(rho, t1);
        }

        public static byte[] PackSecretKey(byte[] rho, byte[] K, byte[] tr, PolynomialVector s1, PolynomialVector s2, PolynomialVector t0)
        {
            byte[] buf = new byte[Constants.cryptoSecretKeyBytes];

            for(int i = 0; i < Constants.seedBytes; ++i)
            {
                buf[i] = rho[i];
            }
            int off = Constants.seedBytes;

            for (int i = 0; i < Constants.seedBytes; ++i)
            {
                buf[off + i] = K[i];
            }
            off += Constants.seedBytes;
            for (int i = 0; i < Constants.seedBytes; ++i)
            {
                buf[off + i] = tr[i];
            }
            off += Constants.seedBytes;

            for(int i = 0; i < s1.Polynomials.Length; ++i)
            {
                PackEta(buf, off, s1.Polynomials[i]);
                off += Constants.polyEtaPacketBytes;
            }

            for (int i = 0; i < s2.Polynomials.Length; ++i)
            {
                PackEta(buf, off, s2.Polynomials[i]);
                off += Constants.polyEtaPacketBytes;
            }
            
            for (int i = 0; i < t0.Polynomials.Length; ++i)
            {
                PackT0(buf, off, t0.Polynomials[i]);
                off += Constants.polyT0PacketBytes;
            }

            return buf;

        }
    
        public static DillithiumPrivateKey UnPackSecretKey(byte[] bytes)
        {
            byte[] rho = new byte[Constants.seedBytes];
            for(int i =0; i < Constants.seedBytes; ++i)
            {
                rho[i] = bytes[i];
            }
            int off = Constants.seedBytes;

            byte[] K = new byte[Constants.seedBytes];
            for (int i = 0; i < Constants.seedBytes; ++i)
            {
                K[i] = bytes[off + i];
            }
            off += Constants.seedBytes;
            
            byte[] tr = new byte[Constants.seedBytes];
            for (int i = 0; i < Constants.seedBytes; ++i)
            {
                tr[i] = bytes[off + i];
            }
            off += Constants.seedBytes;

            PolynomialVector s1 = new PolynomialVector(Constants.l);
            for (int i = 0; i < Constants.l; ++i)
            {
                s1.Polynomials[i] = UnPackEta(bytes, off);
                off += Constants.polyEtaPacketBytes;
            }

            PolynomialVector s2 = new PolynomialVector(Constants.k);
            for (int i = 0; i < Constants.k; ++i)
            {
                s2.Polynomials[i] = UnPackEta(bytes, off);
                off += Constants.polyEtaPacketBytes;
            }

            PolynomialVector t0 = new PolynomialVector(Constants.k);
            for (int i = 0; i < Constants.k; ++i)
            {
                t0.Polynomials[i] = UnPackT0(bytes, off);
                off += Constants.polyT0PacketBytes;
            }

            return new DillithiumPrivateKey(rho, K, tr, s1, s2, t0);
        }

        public static void PackSignature(byte[] sign, byte[] c, PolynomialVector z, PolynomialVector h)
        {
            for (int i = 0; i < Constants.seedBytes; ++i)
            {
                sign[i] = c[i];
            }

            int off = Constants.seedBytes;

            for(int i = 0; i < z.Polynomials.Length; ++i)
            {
                PackZ(sign, off, z.Polynomials[i]);
                off += Constants.polyZPacketBytes;
            }

            for (int i = 0; i < Constants.omega + h.Polynomials.Length; ++i)
                sign[off + i] = 0;

            int k = 0;

            for (int i = 0; i < h.Polynomials.Length; i++)
            {
                for (int j = 0; j < Constants.n; j++)
                {
                    if (h.Polynomials[i].Coefficients[j] != 0)
                    {
                        sign[off + k++] = (byte)(j);
                    }
                }

                sign[off + Constants.omega + i] = (byte)(k);
            }


        }

        public static Sign UnPackSignature(byte[] bytes)
        {
            byte[] challenge = new byte[Constants.seedBytes];

            for(int i = 0; i < Constants.seedBytes; ++i)
            {
                challenge[i] = bytes[i];
            }

            PolynomialVector z = new PolynomialVector(Constants.l);

            int off = Constants.seedBytes;
            for(int i =0; i < Constants.l; ++i)
            {
                z.Polynomials[i] = UnPackZ(bytes, off);
                off += Constants.polyZPacketBytes;
            }

                
            int k = 0;
            PolynomialVector h = new PolynomialVector(Constants.k);
            for (int i =0; i < h.Polynomials.Length; ++i)
            {
                h.Polynomials[i] = new RingPolynomial(new int[0]);

                if (bytes[off + Constants.omega + i] < k || bytes[off + Constants.omega + i] > Constants.omega)
                    throw new Exception("Неверная подпись");

                for(int j = k; j < bytes[off + Constants.omega + i]; ++j)
                {
                    if(j > k && (bytes[off + j] <= bytes[off + j - 1]))
                        throw new Exception("Неверная подпись");
                    h.Polynomials[i].Coefficients[bytes[off + j]] = 1;
                }

                k = bytes[off + Constants.omega + i];
            }

            for(int j =k; j < Constants.omega; ++j)
            {
                if (bytes[off + j] != 0)
                {
                    throw new Exception("Неверная подпись");
                }
            }

            return new Sign(challenge, z, h);
        }
    }
}

