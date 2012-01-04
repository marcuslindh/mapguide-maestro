﻿#region Disclaimer / License
// Copyright (C) 2010, Jackie Ng
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
using System;
using System.Collections.Generic;
using System.Text;
using Maestro.Base.Templates;
using Res = Maestro.AddIn.ExtendedObjectModels.Properties.Resources;
using OSGeo.MapGuide.MaestroAPI;
using Maestro.Editors.Generic;
using OSGeo.MapGuide.ObjectModels;
using OSGeo.MapGuide.ObjectModels.LayerDefinition;
using OSGeo.MapGuide.ObjectModels.WatermarkDefinition;
using OSGeo.MapGuide.ObjectModels.SymbolDefinition;

namespace Maestro.AddIn.ExtendedObjectModels.Templates
{
    internal class WatermarkDefinitionSimple240ItemTemplate : ItemTemplate
    {
        public WatermarkDefinitionSimple240ItemTemplate()
        {
            Category = Res.TPL_CATEGORY_MGOS24;
            Icon = Res.edit;
            Description = Res.TPL_WDFS_240_DESC;
            Name = Res.TPL_WDFS_240_NAME;
            ResourceType = ResourceTypes.LayerDefinition.ToString();
        }

        public override Version MinimumSiteVersion
        {
            get
            {
                return new Version(2, 4);
            }
        }

        public override OSGeo.MapGuide.MaestroAPI.Resource.IResource CreateItem(string startPoint, OSGeo.MapGuide.MaestroAPI.IServerConnection conn)
        {
            return ObjectFactory.CreateWatermark(conn, SymbolDefinitionType.Simple, new Version(2, 4, 0));
        }
    }

    internal class WatermarkDefinitionCompound240ItemTemplate : ItemTemplate
    {
        public WatermarkDefinitionCompound240ItemTemplate()
        {
            Category = Res.TPL_CATEGORY_MGOS24;
            Icon = Res.edit;
            Description = Res.TPL_WDFC_240_DESC;
            Name = Res.TPL_WDFC_240_NAME;
            ResourceType = ResourceTypes.LayerDefinition.ToString();
        }

        public override Version MinimumSiteVersion
        {
            get
            {
                return new Version(2, 4);
            }
        }

        public override OSGeo.MapGuide.MaestroAPI.Resource.IResource CreateItem(string startPoint, OSGeo.MapGuide.MaestroAPI.IServerConnection conn)
        {
            return ObjectFactory.CreateWatermark(conn, SymbolDefinitionType.Compound, new Version(2, 4, 0));
        }
    }
}