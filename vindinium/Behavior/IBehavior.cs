﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vindinium.Behavior
{
    interface IBehavior
    {
        void Do();
        void CheckTransitions();
    }
}
