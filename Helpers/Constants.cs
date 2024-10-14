using System.Text.Json;

namespace Dillithium.Helpers
{
    internal static class Constants
    {
        public static int n = 256;
        public static int q = 8380417;
        public static int qInv = 58728449; //q^-1 mod 2^32
        public static int d = 13;
        public static int rootOfUnity = 1753;
        public static int seedBytes = 32;
        public static int crhBytes = 64;
        public static int polyT0PacketBytes = 416;
        public static int polyT1PacketBytes = 320;
        public static int shake128Rate = 168;
        public static int shake256Rate = 136;
        public static int stream128BlockBytes = shake128Rate;
        public static int stream256BlockBytes = shake256Rate;
        public static int polyUniformGamma1NBlocks = (576 + stream256BlockBytes - 1) / stream256BlockBytes;

        public static int polyVecHPackedBytes, polyZPacketBytes, polyW1PacketBytes, polyEtaPacketBytes, cryptoPublicKeyBytes, cryptoSecretKeyBytes, cryptoBytes;
        public static int k, l, eta, tau, beta, gamma1, gamma2, omega;

        public static void InitConstants(int secLevel = 3)
        {
            switch (secLevel)
            {
                case 2:
                    k = 4;
                    l = 4;
                    eta = 2;
                    tau = 39;
                    beta = 79;
                    gamma1 = 1 << 17;
                    gamma2 = (q - 1) / 88;
                    omega = 80;
                        break;
                case 3:
                    k = 6;
                    l = 5;
                    eta = 4;
                    tau = 49;
                    beta = 196;
                    gamma1 = 1 << 19;
                    gamma2 = (q - 1) / 32;
                    omega = 55;
                        break;
                case 5:
                    k = 8;
                    l = 7;
                    eta = 2;
                    tau = 60;
                    beta = 120;
                    gamma1 = 1 << 19;
                    gamma2 = (q-1) / 32;
                    omega = 75;
                        break;
            }

            if (gamma1 == 1 << 17)
                polyZPacketBytes = 576;
            else
                polyZPacketBytes = 640;

            if (gamma2 == (q-1)/88)
                polyW1PacketBytes = 192;
            else
                polyW1PacketBytes = 128;

            if (eta == 2)
                polyEtaPacketBytes = 96;
            else
                polyEtaPacketBytes = 128;

            cryptoPublicKeyBytes = seedBytes + k * polyT1PacketBytes;
            cryptoSecretKeyBytes = 3 * seedBytes + l * polyEtaPacketBytes + k * polyEtaPacketBytes + k * polyT0PacketBytes;
            cryptoBytes = seedBytes + l * polyZPacketBytes + omega + k;
        }

        
    }

}

