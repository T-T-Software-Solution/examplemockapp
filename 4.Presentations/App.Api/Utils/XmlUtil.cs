using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TTSW.Constant;
using MimeTypes;
using System.Net.Http;
using System.Xml.Serialization;
using System.Xml;

namespace TTSW.Utils
{
    public class XmlUtil
    {
        public XmlUtil()
        {

        }

        public static string Serialize<T>(T dataToSerialize)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                using (var stringwriter = new System.IO.StringWriter())
                {
                    serializer.Serialize(stringwriter, dataToSerialize);
                    return stringwriter.ToString();
                }
            }
            catch
            {
                throw;
            }
        }

        public static string SerializeWithExcludingRootElement<T>(T dataToSerialize)
        {
            var result = Serialize<T>(dataToSerialize);

            // Remove first 2 lines
            result = StringUtil.DeleteLines(result, 2);
            result = StringUtil.RemoveLastLine(result);

            return result;
        }

        public static T Deserialize<T>(string xmlText)
        {
            try
            {
                using (var stringReader = new System.IO.StringReader(xmlText))
                {
                    var serializer = new XmlSerializer(typeof(T));
                    return (T)serializer.Deserialize(stringReader);
                }
            }
            catch
            {
                throw;
            }
        }

    }
}
