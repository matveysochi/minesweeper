﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace minesweeper.Components
{
    public class NavigationPanel : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
