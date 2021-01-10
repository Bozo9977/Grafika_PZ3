using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PZ3.Helpers
{
    public class LoadXMLHelper
    {
        public static T Load<T>(string XmlFilename)
        {
            T returnObject = default(T);

            if (String.IsNullOrEmpty(XmlFilename))
            {
                return default(T);
            }

            try
            {
                StreamReader xmlStream = new StreamReader(XmlFilename);
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                returnObject = (T)serializer.Deserialize(xmlStream);
            }
            catch (Exception)
            {

            }

            return returnObject;
        }
    }
}
