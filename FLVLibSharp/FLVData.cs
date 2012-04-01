using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLVLibSharp.Data
{
    public class FLVData
    {
        public enum TypeEnum : byte
        {
            Number = 0,
            Boolean = 1,
            String = 2,
            Object = 3,
            MovieClip = 4,
            Null = 5,
            Undefined = 6,
            Reference = 7,
            ECMAarray = 8,
            Objectendmarker = 9,
            strictarray = 10,
            Date = 11,
            Longstring = 12
        }

        public TypeEnum FLVType=TypeEnum.Undefined;


        public static FLVData GetObject(FileStream fs)
        {
            byte[] cache = new byte[1];
            fs.Read(cache, 0, 1);
            TypeEnum type = (TypeEnum)cache[0];

            switch (type)
            {
                case TypeEnum.String:
                    return new FLVString(fs);
                case TypeEnum.ECMAarray:
                    return new FLVArray(fs);
                case TypeEnum.Number:
                    return new FLVNumber(fs);
                case TypeEnum.Boolean:
                case TypeEnum.Object:
                case TypeEnum.MovieClip:
                case TypeEnum.Null:
                case TypeEnum.Undefined:
                case TypeEnum.Reference:
                case TypeEnum.strictarray:
                case TypeEnum.Date:
                case TypeEnum.Longstring:
                default:
                    return new FLVData();
            }

        }
    }

    public class FLVObjectEndMarker : FLVData
    {
        public FLVObjectEndMarker(FileStream fs)
        {
            FLVType = TypeEnum.Objectendmarker;
            fs.Position += 3;
        }
    }
    public class FLVNumber : FLVData
    {
        public double value;
        public FLVNumber(FileStream fs)
        {
            FLVType = TypeEnum.Number;
            byte[] cache = new byte[8];
            fs.Read(cache, 0, 8);
            Array.Reverse(cache);
            value = BitConverter.ToDouble(cache, 0);
        }
    }
    public class FLVString : FLVData
    {
        public string text;
        public FLVString(FileStream fs)
        {
            FLVType = TypeEnum.String;
            byte[] cache = new byte[2];

            fs.Read(cache, 0, 2);
            Array.Reverse(cache);
            cache = new byte[BitConverter.ToUInt16(cache, 0)];
            fs.Read(cache, 0, cache.Length);
            text = Encoding.ASCII.GetString(cache);
        }

        public override string ToString()
        {
            return text;
        }
    }

    /// <summary>
    /// ECMA array
    /// </summary>
    public class FLVArray : FLVData
    {
        public FLVObjectProperty[] array;
        public Dictionary<string, FLVObjectProperty> dic;
        public FLVArray(FileStream fs)
        {
            FLVType = TypeEnum.ECMAarray;
            byte[] cache = new byte[4];
            fs.Read(cache, 0, 4);
            Array.Reverse(cache);
            uint length = BitConverter.ToUInt32(cache, 0);

            dic = new Dictionary<string, FLVObjectProperty>();
            array = new FLVObjectProperty[length];
            for (uint i = 0; i < length; i++)
            {
                array[i] = new FLVObjectProperty(fs);
                dic[array[i].name.ToString()] = array[i];
            }

            FLVObjectEndMarker end = new FLVObjectEndMarker(fs);
        }

        /// <summary>
        /// key-value set
        /// </summary>
        public class FLVObjectProperty
        {
            public FLVString name;
            public FLVData value;
            public FLVObjectProperty(FileStream fs)
            {
                name = new FLVString(fs);
                value = FLVData.GetObject(fs);
            }
        }
    }

    
}
