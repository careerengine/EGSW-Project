﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EGSW.Web.ActionFilters
{
    public enum ChangeFrequency
    {
        Always,
        Hourly,
        Daily,
        Weekly,
        Monthly,
        Yearly,
        Never
    }
}