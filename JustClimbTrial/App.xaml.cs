﻿using JustClimbTrial.DataAccess;
using JustClimbTrial.DataAccess.Entities;
using JustClimbTrial.Globals;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace JustClimbTrial
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Wall newestValidWall = WallDataAccess.NewestValidWall;
            if (newestValidWall != null)
            {
                AppGlobal.WallID = newestValidWall.WallID;
            }
        }
    }
}
