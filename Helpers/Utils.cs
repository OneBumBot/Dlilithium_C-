using Org.BouncyCastle.Crypto.Digests;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dillithium.Helpers
{
    internal static class Utils
    {

        public static void ClearByteArray(byte[] arr)
        {
            for(int i = 0; i < arr.Length; ++i)
            {
                arr[i] = 0;
            }
        }

        public static byte[] ConcatByteArrays(params byte[][] arrays)
        {
            byte[] res = arrays[0];
            foreach(byte[] x in arrays[1..^0])
            {
                res = res.Concat(x).ToArray();
            }

            return res;
        }


        public static byte[] GetSHAKE256Digest(int size, params byte[][] arrays)
        {
            byte[] con = ConcatByteArrays(arrays);
            ShakeDigest s = new ShakeDigest(256);
            s.BlockUpdate(con, 0, con.Length);

            byte[] output = new byte[size];
            s.Output(output, 0, size);

            return output;
        }
    }
}
