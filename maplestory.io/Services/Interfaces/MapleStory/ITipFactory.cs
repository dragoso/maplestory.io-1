﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PKG1;
using maplestory.io.Data;

namespace maplestory.io.Services.Interfaces.MapleStory
{
    public interface ITipFactory
    {
        IEnumerable<Tips> GetTips();
    }
}
