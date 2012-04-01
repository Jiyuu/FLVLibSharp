using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLVLibSharp
{
    public class FLVFile
    {
        public FLVHeader Header;

        public FLVTag[] Tags;

        private FLVTagScriptBody _onMetaData;
        public FLVTagScriptBody onMetaData { get { return onMetaData; } }

        public FLVFile(FileStream fs)
        {
            readHeader(fs);
            readTags(fs);

            if (Tags.Length > 0 && Tags[0].tagtype == FLVTag.TagTypeEnum.scriptData && (Tags[0].body as FLVTagScriptBody).name.ToLower() == "onmetadata")
                _onMetaData = (Tags[0].body as FLVTagScriptBody);

        }

        private void readHeader(FileStream fs)
        {
            fs.Seek(13, SeekOrigin.Begin);//skip the flv HEADER and first sizeof previuos tag
        }
        /// <summary>
        /// only reads the first tag for now
        /// </summary>
        /// <param name="fs"></param>
        private void readTags(FileStream fs)
        {
            List<FLVTag> tags = new List<FLVTag>();

            bool continueReading = true;
            while (continueReading)
            {
                tags.Add(new FLVTag(fs));
                continueReading = false;
            }

        }


    }

    public class FLVHeader
    { 
    
    
    }

}
