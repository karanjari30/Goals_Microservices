﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessAccessLayer.Common
{
    public class Enums
    {
        public enum ResponseStatus
        {
            Success = 1, Error = 0, Unauthorized = 401, BadRequest = 400
        }

        public enum ERPCompany
        {
            QuickBooks = 1,
            Xero = 2
        }
    }
}
