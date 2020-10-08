using System;
using System.Runtime.InteropServices;


namespace RWGameProtocol.Serializer.Marshal
{
    [Serializable] 
    [StructLayout(LayoutKind.Sequential, Pack=1)]
    public class Serializer<T> where T : class
    {
        public byte[] Serialize()
        {
            var size = System.Runtime.InteropServices.Marshal.SizeOf(typeof(T)); 
            var array = new byte[size]; 
            var ptr = System.Runtime.InteropServices.Marshal.AllocHGlobal(size);
            System.Runtime.InteropServices.Marshal.StructureToPtr(this, ptr, true);
            System.Runtime.InteropServices.Marshal.Copy(ptr, array, 0, size);
            System.Runtime.InteropServices.Marshal.FreeHGlobal(ptr); 
            return array;
        }

        public static T Deserialize(byte[] array)
        {
            var size = System.Runtime.InteropServices.Marshal.SizeOf(typeof(T)); 
            var ptr = System.Runtime.InteropServices.Marshal.AllocHGlobal(size);
            System.Runtime.InteropServices.Marshal.Copy(array, 0, ptr, size); 
            var s = (T)System.Runtime.InteropServices.Marshal.PtrToStructure(ptr, typeof(T));
            System.Runtime.InteropServices.Marshal.FreeHGlobal(ptr); 
            return s;
        }
    }
}