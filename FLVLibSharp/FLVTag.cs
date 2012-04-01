using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FLVLibSharp.Data;

namespace FLVLibSharp
{
    public class FLVTag
    {
        public enum TagTypeEnum : byte { audio = 8, video = 9, scriptData = 18 }

        #region Header
        public TagTypeEnum tagtype;
        public uint tagSize;
        public FLVTagBody body;
        #endregion

        public FLVTag(FileStream fs)
        {
            byte[] headerbytes = new byte[11];

           
            fs.Read(headerbytes, 0, 11);

            tagtype = (TagTypeEnum)headerbytes[0];

            var tmpSize = headerbytes.Skip(1).Take(3).ToList();
            tmpSize.Insert(0, 0);
            tmpSize.Reverse();

            tagSize = BitConverter.ToUInt32(tmpSize.ToArray(), 0);

        }
        
        public void parseData(FileStream fs)
        {
            switch (tagtype)
            {

                case TagTypeEnum.scriptData:
                    body = new FLVTagScriptBody(fs);
                    break;
                case TagTypeEnum.audio:
                case TagTypeEnum.video:
                default:
                    break;
            }
        }

    }

    public class FLVTagBody
    { 
    }

    public class FLVTagScriptBody : FLVTagBody
    {
        public string name;
        public FLVArray value;

        public FLVTagScriptBody(FileStream fs)
        {
            name = (new FLVString(fs)).text;
            value = new FLVArray(fs);
        }
    }
    
    
}
