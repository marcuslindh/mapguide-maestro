#region Disclaimer / License
// Copyright (C) 2006, Kenneth Skovhede
// http://www.hexad.dk, opensource@hexad.dk
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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using OSGeo.MapGuide.Maestro;

namespace OSGeo.MapGuide.Maestro.ResourceEditors.FeatureSourceEditors.OGR
{
	/// <summary>
	/// Control for handling settings with a managed file
	/// </summary>
	public class Managed : System.Windows.Forms.UserControl
	{
		private ResourceEditors.FeatureSourceEditors.ManagedFileControl managedFileControl;
		private System.ComponentModel.IContainer components;

		private ResourceEditors.EditorInterface m_editor = null;
		private System.Windows.Forms.ImageList ToolbarImages;
		private OSGeo.MapGuide.MaestroAPI.FeatureSource m_item = null;

		public Managed()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
            managedFileControl.toolStrip.Items.Insert(3, new ToolStripButton("", ToolbarImages.Images[0], new EventHandler(FileToolbar_ButtonClick)));
            managedFileControl.toolStrip.Items[3].ToolTipText = "Use all files in the feature source as data source";

			managedFileControl.NewDefaultSelected += new ResourceEditors.FeatureSourceEditors.ManagedFileControl.NewDefaultSelectedDelegate(managedFileControl_NewDefaultSelected);

			System.Collections.Specialized.NameValueCollection nv = new System.Collections.Specialized.NameValueCollection();
			nv.Add(".shp", "Shape file (*.shp)");
			nv.Add(".mif", "MapInfo Interchange (*.mif)");
			nv.Add(".tab", "MapInfo Native (*.tab)");
			nv.Add(".sqlite", "SQLite database (*.sqlite)");
			nv.Add(".gml", "GML file (*.gml)");
			nv.Add(".dgn", "DGN file (*.dgn)");
			nv.Add(".s57", "S-57 (*.s57)");
			nv.Add("", "All files (*.*)");
			managedFileControl.FileTypes = nv;
		}

		public void SetItem(ResourceEditors.EditorInterface editor, OSGeo.MapGuide.MaestroAPI.FeatureSource item)
		{
			m_item = item;
			m_editor = editor;
			managedFileControl.SetItem(editor, item.ResourceId, new ManagedFileControl.IsDefaultItemDelegate(IsDefaultItem));
			UpdateDisplay();
		}

		private bool IsDefaultItem(string filename)
		{
			if (m_item.Parameter == null || m_item.Parameter["DataSource"] == null || m_item.Parameter["DataSource"] == "%MG_DATA_FILE_PATH%")
				return false;

			return m_item.Parameter["DataSource"].ToLower().IndexOf(filename.ToLower()) >= 0;
		}

		public void UpdateDisplay()
		{
			managedFileControl.UpdateDisplay();
		}


		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(Managed));
			this.managedFileControl = new ResourceEditors.FeatureSourceEditors.ManagedFileControl();
			this.ToolbarImages = new System.Windows.Forms.ImageList(this.components);
			this.SuspendLayout();
			// 
			// managedFileControl
			// 
			this.managedFileControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.managedFileControl.FileTypes = null;
			this.managedFileControl.Location = new System.Drawing.Point(0, 0);
			this.managedFileControl.Name = "managedFileControl";
			this.managedFileControl.Size = new System.Drawing.Size(200, 96);
			this.managedFileControl.TabIndex = 1;
			this.managedFileControl.NewDefaultSelected += new ResourceEditors.FeatureSourceEditors.ManagedFileControl.NewDefaultSelectedDelegate(this.managedFileControl_NewDefaultSelected);
			// 
			// ToolbarImages
			// 
			this.ToolbarImages.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
			this.ToolbarImages.ImageSize = new System.Drawing.Size(16, 16);
			this.ToolbarImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ToolbarImages.ImageStream")));
			this.ToolbarImages.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// Managed
			// 
			this.AutoScroll = true;
			this.AutoScrollMinSize = new System.Drawing.Size(200, 96);
			this.Controls.Add(this.managedFileControl);
			this.Name = "Managed";
			this.Size = new System.Drawing.Size(200, 96);
			this.ResumeLayout(false);

		}
		#endregion

		private void FileToolbar_ButtonClick(object sender, EventArgs e)
		{
			m_item.Parameter["DataSource"] = "%MG_DATA_FILE_PATH%";
			m_editor.HasChanged();
			managedFileControl.UpdateDisplay();
		}

		private void managedFileControl_NewDefaultSelected(string filename)
		{
			m_item.Parameter["DataSource"] = "%MG_DATA_FILE_PATH%" + filename;
		}
	}
}