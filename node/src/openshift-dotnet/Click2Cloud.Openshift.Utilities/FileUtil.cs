﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Click2Cloud.Openshift.Utilities
{
    public class FileUtil
    {
        public static string GetSymlinkTargetLocation(string source)
        {
            return Alphaleonis.Win32.Filesystem.File.GetLinkTargetInfo(source).PrintName;
        }
    }
}
