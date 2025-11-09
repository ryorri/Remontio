using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators
{
    public static class GuidValidator
    {
        public static Guid ValidateGuid(string str)
        {
            if (!Guid.TryParse(str, out Guid guid))
            {
                throw new ArgumentException($"Invalid GUID format");
            }
            return guid;
        }
    }
}
