﻿#region Disclaimer / License
// Copyright (C) 2014, Jackie Ng
// http://trac.osgeo.org/mapguide/wiki/maestro, jumpinjackie@gmail.com
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// 
#endregion
using ICSharpCode.Core;
using Maestro.Base.Services;
using Maestro.Editors.Diff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maestro.Base.Commands.SiteExplorer
{
    internal class CompareResourceCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            var wb = Workbench.Instance;
            var siteExp = wb.ActiveSiteExplorer;
            var connMgr = ServiceRegistry.GetService<ServerConnectionManager>();
            var conn = connMgr.GetConnection(wb.ActiveSiteExplorer.ConnectionName);

            var items = siteExp.SelectedItems;
            if (items.Length == 1)
            {
                using (var diag = new CompareResourceDialog(conn.ResourceService))
                {
                    diag.Source = items[0].ResourceId;
                    diag.ShowDialog();
                }
            }
        }
    }
}