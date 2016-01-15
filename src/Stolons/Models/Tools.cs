using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stolons.Models
{
    public static class Tools
    {
        public static IList<string> SerializeStringToList( string stringToSerialize)
        {
            if (String.IsNullOrWhiteSpace(stringToSerialize))
            {
                return null;
            }
            else
            {
                return stringToSerialize.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }
        }

        public static string SerializeListToString(IList<string> listToSerialize)
        {
            return String.Join(";", listToSerialize);
        }
    }
}
