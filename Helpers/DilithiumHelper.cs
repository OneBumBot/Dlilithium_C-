namespace Dillithium.Helpers
{
    internal static class DilithiumHelper
    {

        public static int ReduceModPM(int n, int a)
        {
            int r = n % a;
            if (r > (a >> 1))
                r -= a;
            return r;
        }

        public static (int, int) Decompose(int r, int a)
        {
            r = r % a;
            int r0 = ReduceModPM(r, a);
            int r1 = r - r0;

            if (r1 == Constants.q - 1)
                return (0, r0 - 1);
            r1 /= a;
            return (r1, r0);
        }

        public static int GetHighBits(int r, int a)
        {
            var (r1, _) = Decompose(r, a);
            return r1;
        }

        public static int GetLowBits(int r, int a)
        {
            var (_, r0) = Decompose(r, a);
            return r0;
        }

        public static int MakeHint(int z0, int r1)
        {
            if (z0 <= Constants.gamma2 || z0 > Constants.q - Constants.gamma2 || (z0 == Constants.q - Constants.gamma2 && r1 == 0))
                return 0;

            return 1;
        }

        public static int UseHint(int hint ,int r,int a)
        {
            int m = (Constants.q - 1) / a;
            var (r1, r0) = Decompose(r, a);

            if (hint == 1)
                if (r0 > 0)
                    return (r1 + 1) % m;
                else if (r <= 0)
                    return (r1 - 1) % m;
            return r1;
        }

        public static int useHint(int a, int h)
        {
            int a0, a1;
            a1 = (a + 127) >> 7;
            if (Constants.gamma2 == (Constants.q - 1) / 32)
            {
                a1 = (a1 * 1025 + (1 << 21)) >> 22;
                a1 &= 15;
            }
            else if (Constants.gamma2 == (Constants.q - 1) / 88)
            {
                a1 = (a1 * 11275 + (1 << 23)) >> 24;
                a1 ^= ((43 - a1) >> 31) & a1;
            }
            a0 = a - a1 * 2 * Constants.gamma2;
            a0 -= (((Constants.q - 1) / 2 - a0) >> 31) & Constants.q;

            if (h == 0)
            {
                return a1;
            }
            if (Constants.gamma2 == (Constants.q - 1) / 32)
            {
                if (a0 > 0)
                    return (a1 + 1) & 15;
                else
                    return (a1 - 1) & 15;
            }
            else if (Constants.gamma2 == (Constants.q - 1) / 88)
            {
                if (a0 > 0)
                    return (a1 == 43) ? 0 : a1 + 1;
                else
                    return (a1 == 0) ? 43 : a1 - 1;

            }
            else
            {
                throw new Exception("Плохая gamma2");
            }
        }
        public static bool CheckNormBound(int n, int b)
        {
            int x = n % Constants.q;
            x = ((Constants.q - 1) >> 1) - x;
            x = x ^ (x >> 31);
            x = ((Constants.q - 1) >> 1) - x;
            return x >= b;
        }
    }
}
