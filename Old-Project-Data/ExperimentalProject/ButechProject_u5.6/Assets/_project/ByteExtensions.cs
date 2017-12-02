using System;
using System.Linq;
using System.Net;
public static class ByteExtensions
{
    public static Int16[] ToInt16Array(this byte[] bytes)
    {
        var size = sizeof(Int16);

        if (bytes == null)
            throw new ArgumentNullException("bytes");

        if (bytes.Length % size != 0)
            throw new ArgumentException
                  ("bytes does not represent a sequence of Int16");

        return Enumerable.Range(0, bytes.Length / size)
                         .Select(i => {
                             var chunk = bytes.Skip(i * size).Take(size).ToArray();
                             var conv = BitConverter.ToInt16(chunk, 0);
                             return IPAddress.NetworkToHostOrder(conv);
                                 })
                         .ToArray();
    }
}
