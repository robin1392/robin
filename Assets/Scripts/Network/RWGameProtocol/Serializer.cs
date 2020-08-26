using System;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;


namespace RWGameProtocol
{
    [Serializable] 
    [StructLayout(LayoutKind.Sequential, Pack=1)]
    public class Serializer<T> where T : class
    {
        public byte[] Serialize()
        {
            var size = Marshal.SizeOf(typeof(T)); 
            var array = new byte[size]; 
            var ptr = Marshal.AllocHGlobal(size); 
            Marshal.StructureToPtr(this, ptr, true); 
            Marshal.Copy(ptr, array, 0, size); 
            Marshal.FreeHGlobal(ptr); 
            return array;
        }

        public static T Deserialize(byte[] array)
        {
            var size = Marshal.SizeOf(typeof(T)); 
            var ptr = Marshal.AllocHGlobal(size); 
            Marshal.Copy(array, 0, ptr, size); 
            var s = (T)Marshal.PtrToStructure(ptr, typeof(T)); 
            Marshal.FreeHGlobal(ptr); 
            return s;
        }
    }
}