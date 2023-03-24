using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TTSW.Utils;

namespace TTSW.Utils
{
    public class StringEnumUtil
    {
        private static Hashtable _stringValues = new Hashtable();

        // StringEnumUtil.GetStringValue(DBEnum.Prename)
        public static string GetStringValue(Enum value)
        {
            string output = null;
            Type type = value.GetType();

            if (_stringValues.ContainsKey(value))
                output = (_stringValues[value] as StringValueAttribute).Value;
            else
            {
                //Look for our 'StringValueAttribute' in the field's custom attributes
                FieldInfo fi = type.GetField(value.ToString());
                StringValueAttribute[] attrs =
                    fi.GetCustomAttributes(typeof(StringValueAttribute), false) as StringValueAttribute[];
                if (attrs.Length > 0)
                {
                    _stringValues.Add(value, attrs[0]);
                    output = attrs[0].Value;
                }
            }
            return output;
        }

        public static Array GetEnumList<T>()
        {
            return Enum.GetValues(typeof(T));
        }

        // StringEnumUtil.GetListStringValues(StringEnumUtil.GetEnumList<DBEnum.Prename>())
        public static List<KeyPairDto> GetListStringValues(Array enumValues)
        {
            List<KeyPairDto> result = new List<KeyPairDto>();

            foreach (var x in enumValues)
            {
                var item = new KeyPairDto()
                {
                    Id = (int)x,
                    Name = GetStringValue((Enum)x)
                };

                result.Add(item);
            }

            return result;
        }
    }
}
