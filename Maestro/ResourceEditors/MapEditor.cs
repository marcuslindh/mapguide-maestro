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

namespace OSGeo.MapGuide.Maestro.ResourceEditors
{
	/// <summary>
	/// Summary description for MapEditor.
	/// </summary>
	public class MapEditor : System.Windows.Forms.UserControl, ResourceEditor
	{
		private System.ComponentModel.IContainer components;

		private EditorInterface m_editor;
		private System.Windows.Forms.Label lable1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.TextBox txtLowerX;
		private System.Windows.Forms.TextBox txtLowerY;
		private System.Windows.Forms.TextBox txtUpperX;
		private System.Windows.Forms.TextBox txtUpperY;
		private System.Windows.Forms.TextBox txtDescription;
		private System.Windows.Forms.TextBox txtCoordsys;
		private System.Windows.Forms.TabControl tabLayers;
		private System.Windows.Forms.TabPage tabLayerGroups;
		private System.Windows.Forms.TabPage tabDrawOrder;
		private System.Windows.Forms.TreeView trvLayerGroups;
		private System.Windows.Forms.ListView lstDrawOrder;
        private System.Windows.Forms.Button btnSelectCoordSys;
		private System.Windows.Forms.Panel panel3;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.Panel panel4;
		private ResourceEditors.MapLayerProperties ctlLayerProperties;
		private ResourceEditors.MapLayerGroupProperties ctlGroupProperties;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Panel panel1;
		private ResourceEditors.GeometryStyleEditors.ColorComboBox bgColor;
        private System.Windows.Forms.ImageList LayerToolbarImages;
		private System.Windows.Forms.ImageList TreeImages;
		
		private OSGeo.MapGuide.MaestroAPI.MapDefinition m_map;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private bool m_isUpdating = false;
		private System.Windows.Forms.Button SetZoom;
        private ToolStrip tlbLayerGroups;
        private ToolStripButton AddGroupButton;
        private ToolStripButton RemoveGroupButton;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton AddLayerButton;
        private ToolStripButton RemoveLayerButton;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripButton MoveLayerUpButton;
        private ToolStripButton MoveLayerDownButton;
        private ToolStrip toolStrip1;
        private ToolStripButton RemoveLayerOrderButton;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripButton MoveLayerUpOrderButton;
        private ToolStripButton AddLayerOrderButton;
        private ToolStripButton MoveLayerDownOrderButton;
		private Globalizator.Globalizator m_globalizor = null;

		public MapEditor(EditorInterface editor, string resourceID)
			: this()
		{
			m_editor = editor;
			m_map = m_editor.CurrentConnection.GetMapDefinition(resourceID);
			UpdateDisplay();
		}

		public MapEditor(EditorInterface editor)
			: this()
		{
			m_editor = editor;
			m_map = new OSGeo.MapGuide.MaestroAPI.MapDefinition();
			UpdateDisplay();
		}

		private MapEditor()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			ctlLayerProperties.Visible = false;
			ctlLayerProperties.Dock = DockStyle.Fill;
			ctlGroupProperties.Visible = false;
			ctlGroupProperties.Dock = DockStyle.Fill;

			ctlLayerProperties.LayerPropertiesChanged += new EventHandler(ctlLayerProperties_LayerPropertiesChanged);
			ctlGroupProperties.LayerPropertiesChanged +=new EventHandler(ctlGroupProperties_LayerPropertiesChanged);

			m_globalizor = new  Globalizator.Globalizator(this);
            bgColor.ResetColors();
        }

		public void UpdateDisplay()
		{
			if (m_isUpdating)
				return;

			try
			{
				m_isUpdating = true;

				txtDescription.Text = m_map.Metadata.Replace("<MapDescription>", "").Replace("</MapDescription>", "");
				if (m_editor.CurrentConnection.CoordinateSystem == null || m_map.CoordinateSystem == null || m_map.CoordinateSystem.Length == 0)
					txtCoordsys.Text = m_map.CoordinateSystem;
				else
				{
					try
					{
						string coordcode = m_editor.CurrentConnection.CoordinateSystem.ConvertWktToCoordinateSystemCode(m_map.CoordinateSystem);
						txtCoordsys.Text = m_editor.CurrentConnection.CoordinateSystem.FindCoordSys(coordcode).ToString();
					}
					catch
					{
						txtCoordsys.Text = m_map.CoordinateSystem;
					}
				}
				bgColor.CurrentColor = m_map.BackgroundColor;

				txtLowerX.Text = m_map.Extents.MinX.ToString(System.Globalization.CultureInfo.CurrentUICulture);
				txtLowerY.Text = m_map.Extents.MinY.ToString(System.Globalization.CultureInfo.CurrentUICulture);
				txtUpperX.Text = m_map.Extents.MaxX.ToString(System.Globalization.CultureInfo.CurrentUICulture);
				txtUpperY.Text = m_map.Extents.MaxY.ToString(System.Globalization.CultureInfo.CurrentUICulture);

				lstDrawOrder.BeginUpdate();
				trvLayerGroups.BeginUpdate();
				trvLayerGroups.Nodes.Clear();

				ArrayList unmapped = new ArrayList(m_map.LayerGroups);
				while(unmapped.Count > 0)
				{
					ArrayList toRemove = new ArrayList();
					foreach(OSGeo.MapGuide.MaestroAPI.MapLayerGroupType group in unmapped)
					{
						TreeNodeCollection parent = FindParentNode(group.GetFullPath("/", m_map));
						if (parent != null)
						{
							TreeNode tn = new TreeNode(group.Name, m_editor.ResourceEditorMap.FolderIcon, m_editor.ResourceEditorMap.FolderIcon);
							tn.Tag = group;
							tn.ImageIndex = tn.SelectedImageIndex = 1;
							parent.Add(tn);
							toRemove.Add(group);
						}
					}
				
					foreach(OSGeo.MapGuide.MaestroAPI.MapLayerGroupType group in toRemove)
						unmapped.Remove(group);

					//Prevent infinite loops
					if (toRemove.Count == 0 && unmapped.Count != 0)
					{
						((OSGeo.MapGuide.MaestroAPI.MapLayerGroupType)unmapped[0]).Group = "";
						m_editor.HasChanged();
					}
				}


				lstDrawOrder.Items.Clear();
				foreach(OSGeo.MapGuide.MaestroAPI.MapLayerType layer in m_map.Layers)
				{
					ListViewItem lvi = new ListViewItem(layer.Name, m_editor.ResourceEditorMap.GetImageIndexFromResourceType("LayerDefinition"));
					lvi.Tag = layer;
					lvi.ImageIndex = 0;
					lstDrawOrder.Items.Add(lvi);

					TreeNode tn = new TreeNode(layer.Name, m_editor.ResourceEditorMap.GetImageIndexFromResourceType("LayerDefinition"), m_editor.ResourceEditorMap.GetImageIndexFromResourceType("LayerDefinition"));
					tn.Tag = layer;
					tn.ImageIndex = tn.SelectedImageIndex = 0;

					TreeNodeCollection parent = FindParentNode(layer.GetFullPath("/", m_map));
					if (parent == null)
						parent = trvLayerGroups.Nodes;

					parent.Add(tn);
				}

				trvLayerGroups.EndUpdate();
				lstDrawOrder.EndUpdate();

				if (tabLayers.SelectedIndex == 0)
				{
					if (trvLayerGroups.Nodes.Count > 0)
						trvLayerGroups.SelectedNode = trvLayerGroups.Nodes[0];
				}
				else if (tabLayers.SelectedIndex == 1)
				{
					if (lstDrawOrder.Items.Count > 0)
					{
						lstDrawOrder.SelectedItems.Clear();
						lstDrawOrder.Items[0].Selected = true;
					}
				}
			}
			finally
			{
				m_isUpdating = false;
			}
		}

		private TreeNodeCollection FindParentNode(string fullpath)
		{
			string[] path = fullpath.Split('/');
			TreeNodeCollection nodes = trvLayerGroups.Nodes;
			bool found = true;
			for(int i = 0; i < path.Length - 1; i++)
			{
				if (path[i].Length != 0)
				{
					found = false;
					foreach(TreeNode n in nodes)
						if (n.Text == path[i])
						{
							nodes = n.Nodes;
							found = true;
							break;
						}

					if (!found)
						break;
				}
			}

			return found ? nodes : null;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MapEditor));
            this.lable1 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.bgColor = new OSGeo.MapGuide.Maestro.ResourceEditors.GeometryStyleEditors.ColorComboBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.SetZoom = new System.Windows.Forms.Button();
            this.txtUpperY = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtUpperX = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtLowerY = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtLowerX = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnSelectCoordSys = new System.Windows.Forms.Button();
            this.txtCoordsys = new System.Windows.Forms.TextBox();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.panel4 = new System.Windows.Forms.Panel();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.ctlGroupProperties = new OSGeo.MapGuide.Maestro.ResourceEditors.MapLayerGroupProperties();
            this.ctlLayerProperties = new OSGeo.MapGuide.Maestro.ResourceEditors.MapLayerProperties();
            this.label9 = new System.Windows.Forms.Label();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.panel3 = new System.Windows.Forms.Panel();
            this.tabLayers = new System.Windows.Forms.TabControl();
            this.tabLayerGroups = new System.Windows.Forms.TabPage();
            this.trvLayerGroups = new System.Windows.Forms.TreeView();
            this.TreeImages = new System.Windows.Forms.ImageList(this.components);
            this.tlbLayerGroups = new System.Windows.Forms.ToolStrip();
            this.AddGroupButton = new System.Windows.Forms.ToolStripButton();
            this.RemoveGroupButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.AddLayerButton = new System.Windows.Forms.ToolStripButton();
            this.RemoveLayerButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.MoveLayerUpButton = new System.Windows.Forms.ToolStripButton();
            this.MoveLayerDownButton = new System.Windows.Forms.ToolStripButton();
            this.tabDrawOrder = new System.Windows.Forms.TabPage();
            this.lstDrawOrder = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.AddLayerOrderButton = new System.Windows.Forms.ToolStripButton();
            this.RemoveLayerOrderButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.MoveLayerUpOrderButton = new System.Windows.Forms.ToolStripButton();
            this.MoveLayerDownOrderButton = new System.Windows.Forms.ToolStripButton();
            this.LayerToolbarImages = new System.Windows.Forms.ImageList(this.components);
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.panel3.SuspendLayout();
            this.tabLayers.SuspendLayout();
            this.tabLayerGroups.SuspendLayout();
            this.tlbLayerGroups.SuspendLayout();
            this.tabDrawOrder.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lable1
            // 
            this.lable1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lable1.Location = new System.Drawing.Point(16, 24);
            this.lable1.Name = "lable1";
            this.lable1.Size = new System.Drawing.Size(120, 16);
            this.lable1.TabIndex = 0;
            this.lable1.Text = "Description";
            // 
            // label1
            // 
            this.label1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label1.Location = new System.Drawing.Point(16, 56);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(120, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "Coordinate system";
            // 
            // label2
            // 
            this.label2.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label2.Location = new System.Drawing.Point(16, 88);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(120, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "Background color";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.bgColor);
            this.groupBox1.Controls.Add(this.panel1);
            this.groupBox1.Controls.Add(this.btnSelectCoordSys);
            this.groupBox1.Controls.Add(this.txtCoordsys);
            this.groupBox1.Controls.Add(this.txtDescription);
            this.groupBox1.Controls.Add(this.lable1);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(576, 224);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Settings";
            // 
            // bgColor
            // 
            this.bgColor.Location = new System.Drawing.Point(144, 88);
            this.bgColor.Name = "bgColor";
            this.bgColor.Size = new System.Drawing.Size(216, 21);
            this.bgColor.TabIndex = 10;
            this.bgColor.SelectedIndexChanged += new System.EventHandler(this.bgColor_SelectedIndexChanged);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.groupBox2);
            this.panel1.Location = new System.Drawing.Point(16, 120);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(544, 88);
            this.panel1.TabIndex = 9;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.SetZoom);
            this.groupBox2.Controls.Add(this.txtUpperY);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.txtUpperX);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.txtLowerY);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.txtLowerX);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(544, 88);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Initial map view";
            // 
            // SetZoom
            // 
            this.SetZoom.Enabled = false;
            this.SetZoom.Location = new System.Drawing.Point(408, 24);
            this.SetZoom.Name = "SetZoom";
            this.SetZoom.Size = new System.Drawing.Size(104, 48);
            this.SetZoom.TabIndex = 10;
            this.SetZoom.Text = "Set view to current layers extent";
            this.SetZoom.Click += new System.EventHandler(this.SetZoom_Click);
            // 
            // txtUpperY
            // 
            this.txtUpperY.Location = new System.Drawing.Point(280, 56);
            this.txtUpperY.Name = "txtUpperY";
            this.txtUpperY.Size = new System.Drawing.Size(96, 20);
            this.txtUpperY.TabIndex = 9;
            this.txtUpperY.TextChanged += new System.EventHandler(this.txtUpperY_TextChanged);
            // 
            // label8
            // 
            this.label8.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label8.Location = new System.Drawing.Point(256, 56);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(16, 16);
            this.label8.TabIndex = 8;
            this.label8.Text = "Y";
            // 
            // txtUpperX
            // 
            this.txtUpperX.Location = new System.Drawing.Point(144, 56);
            this.txtUpperX.Name = "txtUpperX";
            this.txtUpperX.Size = new System.Drawing.Size(96, 20);
            this.txtUpperX.TabIndex = 7;
            this.txtUpperX.TextChanged += new System.EventHandler(this.txtUpperX_TextChanged);
            // 
            // label7
            // 
            this.label7.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label7.Location = new System.Drawing.Point(120, 56);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(16, 16);
            this.label7.TabIndex = 6;
            this.label7.Text = "X";
            // 
            // txtLowerY
            // 
            this.txtLowerY.Location = new System.Drawing.Point(280, 24);
            this.txtLowerY.Name = "txtLowerY";
            this.txtLowerY.Size = new System.Drawing.Size(96, 20);
            this.txtLowerY.TabIndex = 5;
            this.txtLowerY.TextChanged += new System.EventHandler(this.txtLowerY_TextChanged);
            // 
            // label6
            // 
            this.label6.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label6.Location = new System.Drawing.Point(256, 24);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(16, 16);
            this.label6.TabIndex = 4;
            this.label6.Text = "Y";
            // 
            // txtLowerX
            // 
            this.txtLowerX.Location = new System.Drawing.Point(144, 24);
            this.txtLowerX.Name = "txtLowerX";
            this.txtLowerX.Size = new System.Drawing.Size(96, 20);
            this.txtLowerX.TabIndex = 3;
            this.txtLowerX.TextChanged += new System.EventHandler(this.txtLowerX_TextChanged);
            // 
            // label5
            // 
            this.label5.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label5.Location = new System.Drawing.Point(120, 24);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(16, 16);
            this.label5.TabIndex = 2;
            this.label5.Text = "X";
            // 
            // label4
            // 
            this.label4.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label4.Location = new System.Drawing.Point(16, 56);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(80, 16);
            this.label4.TabIndex = 1;
            this.label4.Text = "Upper right";
            // 
            // label3
            // 
            this.label3.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label3.Location = new System.Drawing.Point(16, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(88, 16);
            this.label3.TabIndex = 0;
            this.label3.Text = "Lower left";
            // 
            // btnSelectCoordSys
            // 
            this.btnSelectCoordSys.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelectCoordSys.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnSelectCoordSys.Location = new System.Drawing.Point(536, 56);
            this.btnSelectCoordSys.Name = "btnSelectCoordSys";
            this.btnSelectCoordSys.Size = new System.Drawing.Size(24, 20);
            this.btnSelectCoordSys.TabIndex = 8;
            this.btnSelectCoordSys.Text = "...";
            this.btnSelectCoordSys.Click += new System.EventHandler(this.btnSelectCoordSys_Click);
            // 
            // txtCoordsys
            // 
            this.txtCoordsys.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCoordsys.Location = new System.Drawing.Point(144, 56);
            this.txtCoordsys.Name = "txtCoordsys";
            this.txtCoordsys.ReadOnly = true;
            this.txtCoordsys.Size = new System.Drawing.Size(392, 20);
            this.txtCoordsys.TabIndex = 5;
            // 
            // txtDescription
            // 
            this.txtDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDescription.Location = new System.Drawing.Point(144, 24);
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(416, 20);
            this.txtDescription.TabIndex = 4;
            this.txtDescription.TextChanged += new System.EventHandler(this.txtDescription_TextChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.panel4);
            this.groupBox3.Controls.Add(this.splitter1);
            this.groupBox3.Controls.Add(this.panel3);
            this.groupBox3.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupBox3.Location = new System.Drawing.Point(0, 232);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(576, 392);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Layers";
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.groupBox4);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(280, 16);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(293, 373);
            this.panel4.TabIndex = 4;
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.ctlGroupProperties);
            this.groupBox4.Controls.Add(this.ctlLayerProperties);
            this.groupBox4.Controls.Add(this.label9);
            this.groupBox4.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupBox4.Location = new System.Drawing.Point(8, 0);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(280, 368);
            this.groupBox4.TabIndex = 1;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Layer properties";
            // 
            // ctlGroupProperties
            // 
            this.ctlGroupProperties.Location = new System.Drawing.Point(80, 96);
            this.ctlGroupProperties.Name = "ctlGroupProperties";
            this.ctlGroupProperties.Size = new System.Drawing.Size(176, 240);
            this.ctlGroupProperties.TabIndex = 1;
            // 
            // ctlLayerProperties
            // 
            this.ctlLayerProperties.AutoScroll = true;
            this.ctlLayerProperties.AutoScrollMinSize = new System.Drawing.Size(458, 304);
            this.ctlLayerProperties.Location = new System.Drawing.Point(16, 88);
            this.ctlLayerProperties.Name = "ctlLayerProperties";
            this.ctlLayerProperties.Size = new System.Drawing.Size(176, 184);
            this.ctlLayerProperties.TabIndex = 0;
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(16, 24);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(256, 56);
            this.label9.TabIndex = 2;
            this.label9.Text = "When a layer or a group is selected, properties for that item will be displayed h" +
                "ere";
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(272, 16);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(8, 373);
            this.splitter1.TabIndex = 3;
            this.splitter1.TabStop = false;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.tabLayers);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel3.Location = new System.Drawing.Point(3, 16);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(269, 373);
            this.panel3.TabIndex = 2;
            // 
            // tabLayers
            // 
            this.tabLayers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabLayers.Controls.Add(this.tabLayerGroups);
            this.tabLayers.Controls.Add(this.tabDrawOrder);
            this.tabLayers.ImageList = this.LayerToolbarImages;
            this.tabLayers.Location = new System.Drawing.Point(8, 8);
            this.tabLayers.Name = "tabLayers";
            this.tabLayers.SelectedIndex = 0;
            this.tabLayers.Size = new System.Drawing.Size(256, 352);
            this.tabLayers.TabIndex = 0;
            // 
            // tabLayerGroups
            // 
            this.tabLayerGroups.Controls.Add(this.trvLayerGroups);
            this.tabLayerGroups.Controls.Add(this.tlbLayerGroups);
            this.tabLayerGroups.Location = new System.Drawing.Point(4, 23);
            this.tabLayerGroups.Name = "tabLayerGroups";
            this.tabLayerGroups.Size = new System.Drawing.Size(248, 325);
            this.tabLayerGroups.TabIndex = 0;
            this.tabLayerGroups.Text = "Layers by group";
            // 
            // trvLayerGroups
            // 
            this.trvLayerGroups.AllowDrop = true;
            this.trvLayerGroups.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trvLayerGroups.ImageIndex = 0;
            this.trvLayerGroups.ImageList = this.TreeImages;
            this.trvLayerGroups.Location = new System.Drawing.Point(0, 25);
            this.trvLayerGroups.Name = "trvLayerGroups";
            this.trvLayerGroups.SelectedImageIndex = 0;
            this.trvLayerGroups.Size = new System.Drawing.Size(248, 300);
            this.trvLayerGroups.TabIndex = 0;
            this.trvLayerGroups.DragDrop += new System.Windows.Forms.DragEventHandler(this.trvLayerGroups_DragDrop);
            this.trvLayerGroups.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.trvLayerGroups_AfterSelect);
            this.trvLayerGroups.DragEnter += new System.Windows.Forms.DragEventHandler(this.trvLayerGroups_DragEnter);
            this.trvLayerGroups.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.trvLayerGroups_ItemDrag);
            // 
            // TreeImages
            // 
            this.TreeImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("TreeImages.ImageStream")));
            this.TreeImages.TransparentColor = System.Drawing.Color.Transparent;
            this.TreeImages.Images.SetKeyName(0, "");
            this.TreeImages.Images.SetKeyName(1, "");
            // 
            // tlbLayerGroups
            // 
            this.tlbLayerGroups.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tlbLayerGroups.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.AddGroupButton,
            this.RemoveGroupButton,
            this.toolStripSeparator1,
            this.AddLayerButton,
            this.RemoveLayerButton,
            this.toolStripSeparator2,
            this.MoveLayerUpButton,
            this.MoveLayerDownButton});
            this.tlbLayerGroups.Location = new System.Drawing.Point(0, 0);
            this.tlbLayerGroups.Name = "tlbLayerGroups";
            this.tlbLayerGroups.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.tlbLayerGroups.Size = new System.Drawing.Size(248, 25);
            this.tlbLayerGroups.TabIndex = 2;
            this.tlbLayerGroups.Text = "toolStrip1";
            // 
            // AddGroupButton
            // 
            this.AddGroupButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.AddGroupButton.Image = ((System.Drawing.Image)(resources.GetObject("AddGroupButton.Image")));
            this.AddGroupButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.AddGroupButton.Name = "AddGroupButton";
            this.AddGroupButton.Size = new System.Drawing.Size(23, 22);
            this.AddGroupButton.Text = "toolStripButton1";
            this.AddGroupButton.ToolTipText = "Add a new group";
            this.AddGroupButton.Click += new System.EventHandler(this.AddGroupButton_Click);
            // 
            // RemoveGroupButton
            // 
            this.RemoveGroupButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.RemoveGroupButton.Image = ((System.Drawing.Image)(resources.GetObject("RemoveGroupButton.Image")));
            this.RemoveGroupButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.RemoveGroupButton.Name = "RemoveGroupButton";
            this.RemoveGroupButton.Size = new System.Drawing.Size(23, 22);
            this.RemoveGroupButton.Text = "toolStripButton2";
            this.RemoveGroupButton.ToolTipText = "Remove the selected group and contents";
            this.RemoveGroupButton.Click += new System.EventHandler(this.RemoveGroupButton_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // AddLayerButton
            // 
            this.AddLayerButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.AddLayerButton.Image = ((System.Drawing.Image)(resources.GetObject("AddLayerButton.Image")));
            this.AddLayerButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.AddLayerButton.Name = "AddLayerButton";
            this.AddLayerButton.Size = new System.Drawing.Size(23, 22);
            this.AddLayerButton.Text = "toolStripButton3";
            this.AddLayerButton.ToolTipText = "Add a new layer to the map";
            this.AddLayerButton.Click += new System.EventHandler(this.AddLayerButton_Click);
            // 
            // RemoveLayerButton
            // 
            this.RemoveLayerButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.RemoveLayerButton.Image = ((System.Drawing.Image)(resources.GetObject("RemoveLayerButton.Image")));
            this.RemoveLayerButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.RemoveLayerButton.Name = "RemoveLayerButton";
            this.RemoveLayerButton.Size = new System.Drawing.Size(23, 22);
            this.RemoveLayerButton.Text = "toolStripButton4";
            this.RemoveLayerButton.ToolTipText = "Remove a layer from the group";
            this.RemoveLayerButton.Click += new System.EventHandler(this.RemoveLayerButton_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // MoveLayerUpButton
            // 
            this.MoveLayerUpButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.MoveLayerUpButton.Image = ((System.Drawing.Image)(resources.GetObject("MoveLayerUpButton.Image")));
            this.MoveLayerUpButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.MoveLayerUpButton.Name = "MoveLayerUpButton";
            this.MoveLayerUpButton.Size = new System.Drawing.Size(23, 22);
            this.MoveLayerUpButton.Text = "toolStripButton5";
            this.MoveLayerUpButton.ToolTipText = "Move the selected layer up";
            this.MoveLayerUpButton.Click += new System.EventHandler(this.MoveLayerUpButton_Click);
            // 
            // MoveLayerDownButton
            // 
            this.MoveLayerDownButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.MoveLayerDownButton.Image = ((System.Drawing.Image)(resources.GetObject("MoveLayerDownButton.Image")));
            this.MoveLayerDownButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.MoveLayerDownButton.Name = "MoveLayerDownButton";
            this.MoveLayerDownButton.Size = new System.Drawing.Size(23, 22);
            this.MoveLayerDownButton.Text = "toolStripButton6";
            this.MoveLayerDownButton.ToolTipText = "Move the selected layer down";
            this.MoveLayerDownButton.Click += new System.EventHandler(this.MoveLayerDownButton_Click);
            // 
            // tabDrawOrder
            // 
            this.tabDrawOrder.Controls.Add(this.lstDrawOrder);
            this.tabDrawOrder.Controls.Add(this.toolStrip1);
            this.tabDrawOrder.Location = new System.Drawing.Point(4, 23);
            this.tabDrawOrder.Name = "tabDrawOrder";
            this.tabDrawOrder.Size = new System.Drawing.Size(248, 325);
            this.tabDrawOrder.TabIndex = 1;
            this.tabDrawOrder.Text = "Layers by drawing order";
            // 
            // lstDrawOrder
            // 
            this.lstDrawOrder.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.lstDrawOrder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstDrawOrder.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lstDrawOrder.Location = new System.Drawing.Point(0, 25);
            this.lstDrawOrder.Name = "lstDrawOrder";
            this.lstDrawOrder.Size = new System.Drawing.Size(248, 300);
            this.lstDrawOrder.SmallImageList = this.TreeImages;
            this.lstDrawOrder.TabIndex = 0;
            this.lstDrawOrder.UseCompatibleStateImageBehavior = false;
            this.lstDrawOrder.View = System.Windows.Forms.View.Details;
            this.lstDrawOrder.SelectedIndexChanged += new System.EventHandler(this.lstDrawOrder_SelectedIndexChanged);
            this.lstDrawOrder.SizeChanged += new System.EventHandler(this.lstDrawOrder_SizeChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Width = 200;
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.AddLayerOrderButton,
            this.RemoveLayerOrderButton,
            this.toolStripSeparator3,
            this.MoveLayerUpOrderButton,
            this.MoveLayerDownOrderButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(248, 25);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // AddLayerOrderButton
            // 
            this.AddLayerOrderButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.AddLayerOrderButton.Image = ((System.Drawing.Image)(resources.GetObject("AddLayerOrderButton.Image")));
            this.AddLayerOrderButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.AddLayerOrderButton.Name = "AddLayerOrderButton";
            this.AddLayerOrderButton.Size = new System.Drawing.Size(23, 22);
            this.AddLayerOrderButton.ToolTipText = "Add a new layer to the map";
            this.AddLayerOrderButton.Click += new System.EventHandler(this.AddLayerOrderButton_Click);
            // 
            // RemoveLayerOrderButton
            // 
            this.RemoveLayerOrderButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.RemoveLayerOrderButton.Image = ((System.Drawing.Image)(resources.GetObject("RemoveLayerOrderButton.Image")));
            this.RemoveLayerOrderButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.RemoveLayerOrderButton.Name = "RemoveLayerOrderButton";
            this.RemoveLayerOrderButton.Size = new System.Drawing.Size(23, 22);
            this.RemoveLayerOrderButton.ToolTipText = "Remove the selected layers from the map";
            this.RemoveLayerOrderButton.Click += new System.EventHandler(this.RemoveLayerOrderButton_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // MoveLayerUpOrderButton
            // 
            this.MoveLayerUpOrderButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.MoveLayerUpOrderButton.Image = ((System.Drawing.Image)(resources.GetObject("MoveLayerUpOrderButton.Image")));
            this.MoveLayerUpOrderButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.MoveLayerUpOrderButton.Name = "MoveLayerUpOrderButton";
            this.MoveLayerUpOrderButton.Size = new System.Drawing.Size(23, 22);
            this.MoveLayerUpOrderButton.Text = "toolStripButton3";
            this.MoveLayerUpOrderButton.ToolTipText = "Move the selected layers up";
            this.MoveLayerUpOrderButton.Click += new System.EventHandler(this.MoveLayerUpOrderButton_Click);
            // 
            // MoveLayerDownOrderButton
            // 
            this.MoveLayerDownOrderButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.MoveLayerDownOrderButton.Image = ((System.Drawing.Image)(resources.GetObject("MoveLayerDownOrderButton.Image")));
            this.MoveLayerDownOrderButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.MoveLayerDownOrderButton.Name = "MoveLayerDownOrderButton";
            this.MoveLayerDownOrderButton.Size = new System.Drawing.Size(23, 22);
            this.MoveLayerDownOrderButton.Text = "toolStripButton3";
            this.MoveLayerDownOrderButton.ToolTipText = "Move the selected layers down";
            this.MoveLayerDownOrderButton.Click += new System.EventHandler(this.MoveLayerDownOrderButton_Click);
            // 
            // LayerToolbarImages
            // 
            this.LayerToolbarImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("LayerToolbarImages.ImageStream")));
            this.LayerToolbarImages.TransparentColor = System.Drawing.Color.Transparent;
            this.LayerToolbarImages.Images.SetKeyName(0, "");
            this.LayerToolbarImages.Images.SetKeyName(1, "");
            this.LayerToolbarImages.Images.SetKeyName(2, "");
            this.LayerToolbarImages.Images.SetKeyName(3, "");
            this.LayerToolbarImages.Images.SetKeyName(4, "");
            this.LayerToolbarImages.Images.SetKeyName(5, "");
            // 
            // MapEditor
            // 
            this.AutoScroll = true;
            this.AutoScrollMinSize = new System.Drawing.Size(576, 624);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Name = "MapEditor";
            this.Size = new System.Drawing.Size(576, 624);
            this.Load += new System.EventHandler(this.MapEditor_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.tabLayers.ResumeLayout(false);
            this.tabLayerGroups.ResumeLayout(false);
            this.tabLayerGroups.PerformLayout();
            this.tlbLayerGroups.ResumeLayout(false);
            this.tlbLayerGroups.PerformLayout();
            this.tabDrawOrder.ResumeLayout(false);
            this.tabDrawOrder.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		private void trvLayerGroups_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			if (trvLayerGroups.SelectedNode == null)
				SelectLayerItem(null);
			else
				SelectLayerItem(trvLayerGroups.SelectedNode.Tag);
		}

		private void lstDrawOrder_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (lstDrawOrder.SelectedItems.Count != 1)
				SelectLayerItem(null);
			else
				SelectLayerItem(lstDrawOrder.SelectedItems[0].Tag);
		}

		private void SelectLayerItem(object item)
		{
			if (item == null)
			{
				ctlLayerProperties.Visible = false;
				ctlGroupProperties.Visible = false;
				RemoveGroupButton.Enabled = false;
				RemoveLayerButton.Enabled = false;
				SetZoom.Enabled = false;
			}
			else
			{
				if (item.GetType() == typeof(OSGeo.MapGuide.MaestroAPI.MapLayerType))
				{

					ctlLayerProperties.SelectLayerItem((OSGeo.MapGuide.MaestroAPI.MapLayerType)item, m_editor);
					RemoveGroupButton.Enabled = false;
					RemoveLayerButton.Enabled = true;
					SetZoom.Enabled = true;

					ctlLayerProperties.Visible = true;
					ctlGroupProperties.Visible = false;
				}
				else if (item.GetType() == typeof(OSGeo.MapGuide.MaestroAPI.MapLayerGroupType))
				{
					ctlGroupProperties.SelectLayerItem((OSGeo.MapGuide.MaestroAPI.MapLayerGroupType)item, m_editor);
					RemoveGroupButton.Enabled = true;
					RemoveLayerButton.Enabled = false;
					SetZoom.Enabled = false;

					ctlLayerProperties.Visible = false;
					ctlGroupProperties.Visible = true;
				}
				else
				{
					ctlLayerProperties.Visible = false;
					ctlGroupProperties.Visible = false;
					RemoveGroupButton.Enabled = false;
					RemoveLayerButton.Enabled = false;
					SetZoom.Enabled = false;
				}
			}
		}

		private void MapEditor_Load(object sender, System.EventArgs e)
		{
		}

		private void txtDescription_TextChanged(object sender, System.EventArgs e)
		{
			if (m_isUpdating)
				return;

			m_map.Metadata = "<MapDescription>" + txtDescription.Text + "</MapDescription>";
			m_editor.HasChanged();
		}

		private void txtLowerX_TextChanged(object sender, System.EventArgs e)
		{
			if (m_isUpdating)
				return;

			double v;
			if (double.TryParse(txtLowerX.Text, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.CurrentUICulture , out v))
			{
				m_map.Extents.MinX = v;
				m_editor.HasChanged();
			}
		}

		private void bgColor_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (m_isUpdating)
				return;

			m_map.BackgroundColor = bgColor.CurrentColor;
			m_editor.HasChanged();
		}

		private void btnSelectCoordSys_Click(object sender, System.EventArgs e)
		{
			SelectCoordinateSystem dlg = new SelectCoordinateSystem(m_editor.CurrentConnection);
			dlg.SetWKT(m_map.CoordinateSystem);

			if (dlg.ShowDialog() == DialogResult.OK)
			{
				txtCoordsys.Text = dlg.SelectedCoordSys.ToString();
				m_map.CoordinateSystem = dlg.SelectedCoordSys.WKT;
				m_editor.HasChanged();
			}
		}

		private void RemoveGroup()
		{
			if (trvLayerGroups.SelectedNode == null || trvLayerGroups.SelectedNode.Tag == null)
				return;

			int index = m_map.LayerGroups.IndexOf((OSGeo.MapGuide.MaestroAPI.MapLayerGroupType)trvLayerGroups.SelectedNode.Tag);
			if (index >= 0)
			{
				m_map.LayerGroups.RemoveAt(index);
				trvLayerGroups.SelectedNode.Remove();
				m_editor.HasChanged();
				return;
			}
			
		}

		private void RemoveLayer()
		{
			ArrayList items = new ArrayList();
			if (tabLayers.SelectedTab == tabLayerGroups)
			{
				if (trvLayerGroups.SelectedNode == null || trvLayerGroups.SelectedNode.Tag == null)
					return;

				items.Add(trvLayerGroups.SelectedNode.Tag);
			}
			else
			{
				if (lstDrawOrder.SelectedItems.Count == 0)
					return;

				foreach(ListViewItem lvi in lstDrawOrder.SelectedItems)
					items.Add(lvi.Tag);
			}

			bool hasChanged = false;
			foreach(OSGeo.MapGuide.MaestroAPI.MapLayerType layer in items)
			{
				int index = m_map.Layers.IndexOf(layer);
				if (index >= 0)
				{
					hasChanged = true;
					m_map.Layers.RemoveAt(index);

					TreeNode n = FindItemByTag(trvLayerGroups.Nodes, layer);
					if (n != null)
						n.Remove();

					foreach(ListViewItem lvi in lstDrawOrder.Items)
						if (lvi.Tag == layer)
						{
							lstDrawOrder.Items.Remove(lvi);
							break;
						}

					hasChanged = true;
				}
			}

			if (hasChanged)
				m_editor.HasChanged();

		}

		private void MoveLayerUp()
		{
			if (tabLayers.SelectedTab == tabLayerGroups)
			{
				if (trvLayerGroups.SelectedNode == null || trvLayerGroups.SelectedNode.Tag == null)
					return;

				int index = -1;
				IList list = null;

				if (trvLayerGroups.SelectedNode.Tag.GetType() == typeof(OSGeo.MapGuide.MaestroAPI.MapLayerGroupType))
				{
					index = m_map.LayerGroups.IndexOf((OSGeo.MapGuide.MaestroAPI.MapLayerGroupType)trvLayerGroups.SelectedNode.Tag);
					list = m_map.LayerGroups;
				}
				else if (trvLayerGroups.SelectedNode.Tag.GetType() == typeof(OSGeo.MapGuide.MaestroAPI.MapLayerType))
				{
					index = m_map.Layers.IndexOf((OSGeo.MapGuide.MaestroAPI.MapLayerType)trvLayerGroups.SelectedNode.Tag);
					list = m_map.Layers;
				}

				if (list != null && index > 0)
				{
					object o = list[index];
					list.RemoveAt(index);
					index--;
					list.Insert(index, o);
					m_editor.HasChanged();
					UpdateDisplay();
					SelectItemByTag(trvLayerGroups.Nodes, o);
				}
			}
			else
			{
				if (lstDrawOrder.SelectedItems.Count == 0)
					return;

				ArrayList indices = new ArrayList();
				bool changed = false;

				foreach(ListViewItem lvi in lstDrawOrder.SelectedItems)
				{
					int index = m_map.Layers.IndexOf((OSGeo.MapGuide.MaestroAPI.MapLayerType)lvi.Tag);
					if (index == 0)
						return;

					indices.Add(index);
				}

				foreach(ListViewItem lvi in lstDrawOrder.SelectedItems)
				{
					int index = m_map.Layers.IndexOf((OSGeo.MapGuide.MaestroAPI.MapLayerType)lvi.Tag);
					if (index > 0)
					{
						changed = true;
						m_map.Layers.RemoveAt(index);
						index--;
						((IList)m_map.Layers).Insert(index, (OSGeo.MapGuide.MaestroAPI.MapLayerType)lvi.Tag);
					}
				}

				if (changed)
				{
					m_editor.HasChanged();
					UpdateDisplay();

					lstDrawOrder.SelectedItems.Clear();

					foreach(int i in indices)
						lstDrawOrder.Items[i - 1].Selected = true;
					lstDrawOrder.Items[((int)indices[indices.Count - 1])- 1].EnsureVisible();
				}
			}
		}

		private void MoveLayerDown()
		{
			if (tabLayers.SelectedTab == tabLayerGroups)
			{
				if (trvLayerGroups.SelectedNode == null || trvLayerGroups.SelectedNode.Tag == null)
					return;

				int index = -1;
				IList list = null;

				if (trvLayerGroups.SelectedNode.Tag.GetType() == typeof(OSGeo.MapGuide.MaestroAPI.MapLayerGroupType))
				{
					index = m_map.LayerGroups.IndexOf((OSGeo.MapGuide.MaestroAPI.MapLayerGroupType)trvLayerGroups.SelectedNode.Tag);
					list = m_map.LayerGroups;
				}
				else if (trvLayerGroups.SelectedNode.Tag.GetType() == typeof(OSGeo.MapGuide.MaestroAPI.MapLayerType))
				{
					index = m_map.Layers.IndexOf((OSGeo.MapGuide.MaestroAPI.MapLayerType)trvLayerGroups.SelectedNode.Tag);
					list = m_map.Layers;
				}

				if (list != null && index >= 0 && index < list.Count - 1)
				{
					object o = list[index];
					list.RemoveAt(index);
					index++;
					list.Insert(index, o);
					m_editor.HasChanged();
					UpdateDisplay();
					SelectItemByTag(trvLayerGroups.Nodes, o);
				}
			}
			else
			{
				if (lstDrawOrder.SelectedItems.Count == 0)
					return;

				ArrayList indices = new ArrayList();
				foreach(ListViewItem lvi in lstDrawOrder.SelectedItems)
				{
					int index = m_map.Layers.IndexOf((OSGeo.MapGuide.MaestroAPI.MapLayerType)lvi.Tag);
					if (index < 0 || index >= m_map.Layers.Count - 1)
						return;

					indices.Add(index);
				}

				bool changed = false;
				for(int i = lstDrawOrder.SelectedItems.Count - 1; i >= 0; i--)
				{
					int index = m_map.Layers.IndexOf((OSGeo.MapGuide.MaestroAPI.MapLayerType)lstDrawOrder.SelectedItems[i].Tag);
					if (index >= 0 && index < m_map.Layers.Count)
					{
						changed = true;
						m_map.Layers.RemoveAt(index);
						index++;
						((IList)m_map.Layers).Insert(index, (OSGeo.MapGuide.MaestroAPI.MapLayerType)lstDrawOrder.SelectedItems[i].Tag);
					}
				}

				if (changed)
				{
					m_editor.HasChanged();
					UpdateDisplay();

					lstDrawOrder.SelectedItems.Clear();

					foreach(int i in indices)
						lstDrawOrder.Items[i+1].Selected = true;
					lstDrawOrder.Items[((int)indices[0]) + 1].EnsureVisible();
				}
			}
		}

		private void AddGroup()
		{
			Hashtable ht = new Hashtable();
			foreach(OSGeo.MapGuide.MaestroAPI.MapLayerGroupType g in m_map.LayerGroups)
				ht.Add(g.Name, g);
            
			OSGeo.MapGuide.MaestroAPI.MapLayerGroupType group = new OSGeo.MapGuide.MaestroAPI.MapLayerGroupType();
			int i = 1;
			string groupName = m_globalizor.Translate("New group");

			while(ht.ContainsKey(groupName))
				groupName = string.Format(m_globalizor.Translate("New group {0}"), + (i++));

			group.Name = groupName;
			m_map.LayerGroups.Add(group);
			m_editor.HasChanged();
			UpdateDisplay();
			SelectItemByTag(trvLayerGroups.Nodes, group);
		}

		private void AddLayer()
		{
			string resource = m_editor.BrowseResource("LayerDefinition");
			if (resource != null)
			{
				foreach(OSGeo.MapGuide.MaestroAPI.MapLayerType layer in m_map.Layers)
					if (layer.ResourceId == resource)
					{
						if (MessageBox.Show(this, m_globalizor.Translate("That layer is already in the map, do you want it to appear twice?"), Application.ProductName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) != DialogResult.Yes)
							return;
					}

				OSGeo.MapGuide.MaestroAPI.MapLayerType maplayer = new OSGeo.MapGuide.MaestroAPI.MapLayerType();
				maplayer.ResourceId = resource;
				maplayer.Name = resource.Substring(resource.LastIndexOf("/") + 1, resource.LastIndexOf(".") - (resource.LastIndexOf("/") + 1));
				maplayer.Visible = true;
				maplayer.ShowInLegend = true;
				maplayer.ExpandInLegend = true;
				m_map.Layers.Add(maplayer);
				m_editor.HasChanged();
				UpdateDisplay();
				if (tabLayers.SelectedTab == tabLayerGroups)
					SelectItemByTag(trvLayerGroups.Nodes, maplayer);
				else
					foreach(ListViewItem lvi in lstDrawOrder.Items)
						if (lvi.Tag == maplayer)
						{
							lstDrawOrder.SelectedItems.Clear();
							lvi.Selected = true;
							lvi.EnsureVisible();
							break;
						}
			}
		}

		public object Resource
		{
			get { return m_map; }
			set 
			{
				m_map = (OSGeo.MapGuide.MaestroAPI.MapDefinition)value;
				UpdateDisplay();
			}
		}

		private TreeNode FindItemByTag(TreeNodeCollection nodes, object tag)
		{
			foreach(TreeNode n in nodes)
				if (n.Tag == tag)
					return n;
				else if (n.Nodes.Count > 0)
				{
					TreeNode t = FindItemByTag(n.Nodes, tag);
					if (t != null)
						return t;
				}
			return null;
		}

		private void SelectItemByTag(TreeNodeCollection nodes, object tag)
		{
			TreeNode t = FindItemByTag(nodes, tag);
			if (t != null && t.TreeView != null)
				t.TreeView.SelectedNode = t;
		}

		private void ctlLayerProperties_LayerPropertiesChanged(object sender, EventArgs e)
		{
			if (m_isUpdating)
				return;

			if (trvLayerGroups.SelectedNode == null || trvLayerGroups.SelectedNode.Tag == null)
				return;

			m_editor.HasChanged();
			if (trvLayerGroups.SelectedNode.Tag as OSGeo.MapGuide.MaestroAPI.MapLayerGroupType != null)
				trvLayerGroups.SelectedNode.Text = ((OSGeo.MapGuide.MaestroAPI.MapLayerGroupType)trvLayerGroups.SelectedNode.Tag).Name;
			else if (trvLayerGroups.SelectedNode.Tag as OSGeo.MapGuide.MaestroAPI.MapLayerType != null)
				trvLayerGroups.SelectedNode.Text = ((OSGeo.MapGuide.MaestroAPI.MapLayerType)trvLayerGroups.SelectedNode.Tag).Name;
		}

		private void ctlGroupProperties_LayerPropertiesChanged(object sender, EventArgs e)
		{
			ctlLayerProperties_LayerPropertiesChanged(sender, e);
		}

		public string ResourceId
		{
			get { return m_map.ResourceId; }
			set { m_map.ResourceId = value; }
		}

		public bool Preview()
		{
			try
			{
				string id = m_editor.CurrentConnection.GetResourceIdentifier(Guid.NewGuid().ToString(), OSGeo.MapGuide.MaestroAPI.ResourceTypes.RuntimeMap, true);
				m_editor.CurrentConnection.CreateRuntimeMap(id, m_map);

				RasterViewer.PreviewMap pv = new RasterViewer.PreviewMap(m_editor.CurrentConnection, id);
				pv.ShowDialog(this);
			}
			catch(Exception ex)
			{
				MessageBox.Show(this, string.Format(m_globalizor.Translate("Failed while creating map preview: {0}"), ex.Message), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

			return false;
		}

		private void lstDrawOrder_SizeChanged(object sender, System.EventArgs e)
		{
			lstDrawOrder.Columns[0].Width = lstDrawOrder.Width - 40;
		}

		private void txtUpperX_TextChanged(object sender, System.EventArgs e)
		{
			if (m_isUpdating)
				return;

			double v;
			if (double.TryParse(txtUpperX.Text, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.CurrentUICulture, out v))
			{
				m_map.Extents.MaxX = v;
				m_editor.HasChanged();
			}
		}

		private void txtLowerY_TextChanged(object sender, System.EventArgs e)
		{
			if (m_isUpdating)
				return;

			double v;
			if (double.TryParse(txtLowerY.Text, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.CurrentUICulture, out v))
			{
				m_map.Extents.MinY = v;
				m_editor.HasChanged();
			}
		
		}

		private void txtUpperY_TextChanged(object sender, System.EventArgs e)
		{
			if (m_isUpdating)
				return;

			double v;
			if (double.TryParse(txtUpperY.Text, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.CurrentUICulture, out v))
			{
				m_map.Extents.MaxY = v;
				m_editor.HasChanged();
			}
		
		}

		public bool Save(string savename)
		{
			return false;
		}

		private void trvLayerGroups_ItemDrag(object sender, System.Windows.Forms.ItemDragEventArgs e)
		{
			trvLayerGroups.DoDragDrop(e.Item, DragDropEffects.Move);
		}

		private void trvLayerGroups_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
		{
			TreeNode source = e.Data.GetData(typeof(TreeNode)) as TreeNode;

			if (source == null || source.TreeView != trvLayerGroups || source.Tag == null)
				e.Effect = DragDropEffects.None;
			else
				e.Effect = DragDropEffects.Move;
		}

		private void trvLayerGroups_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
		{
			TreeNode source = e.Data.GetData(typeof(TreeNode)) as TreeNode;

			Point pt = ((TreeView)sender).PointToClient(new Point(e.X, e.Y));
			TreeNode target = ((TreeView)sender).GetNodeAt(pt);

			if (source == null || source.TreeView != trvLayerGroups)
				return;

			source.Remove();
			
			string group;
			if (target == null || target.Tag as OSGeo.MapGuide.MaestroAPI.MapLayerGroupType == null)
			{
				group = "";
				trvLayerGroups.Nodes.Add(source);
			}
			else
			{
				group = target.Text;
				target.Nodes.Add(source);
			}

			if (source.Tag as OSGeo.MapGuide.MaestroAPI.MapLayerGroupType != null)
				(source.Tag as OSGeo.MapGuide.MaestroAPI.MapLayerGroupType).Group = group;
			else if (source.Tag as OSGeo.MapGuide.MaestroAPI.MapLayerType != null)
				(source.Tag as OSGeo.MapGuide.MaestroAPI.MapLayerType).Group = group;

			m_editor.HasChanged();
		}

		private void SetZoom_Click(object sender, System.EventArgs e)
		{
			try
			{
				OSGeo.MapGuide.MaestroAPI.MapLayerType ml = null;
				if (tabLayers.SelectedTab == tabLayerGroups && trvLayerGroups.SelectedNode != null)
					ml = trvLayerGroups.SelectedNode.Tag as OSGeo.MapGuide.MaestroAPI.MapLayerType;
				else if (tabLayers.SelectedTab == tabDrawOrder && lstDrawOrder.SelectedItems.Count == 1)
					ml = lstDrawOrder.SelectedItems[0].Tag as OSGeo.MapGuide.MaestroAPI.MapLayerType;

				if (ml == null)
					throw new Exception(m_globalizor.Translate("No layer selected"));

				OSGeo.MapGuide.MaestroAPI.LayerDefinition ldef = m_editor.CurrentConnection.GetLayerDefinition(ml.ResourceId);
				OSGeo.MapGuide.MaestroAPI.FdoSpatialContextList lst = m_editor.CurrentConnection.GetSpatialContextInfo(ldef.Item.ResourceId, false);
				if (lst.SpatialContext != null && lst.SpatialContext.Count >= 1)
				{
					txtLowerX.Text = double.Parse(lst.SpatialContext[0].Extent.LowerLeftCoordinate.X, System.Globalization.CultureInfo.InvariantCulture).ToString(System.Globalization.CultureInfo.CurrentUICulture);
					txtLowerY.Text = double.Parse(lst.SpatialContext[0].Extent.LowerLeftCoordinate.Y, System.Globalization.CultureInfo.InvariantCulture).ToString(System.Globalization.CultureInfo.CurrentUICulture);
					txtUpperX.Text = double.Parse(lst.SpatialContext[0].Extent.UpperRightCoordinate.X, System.Globalization.CultureInfo.InvariantCulture).ToString(System.Globalization.CultureInfo.CurrentUICulture);
					txtUpperY.Text = double.Parse(lst.SpatialContext[0].Extent.UpperRightCoordinate.Y, System.Globalization.CultureInfo.InvariantCulture).ToString(System.Globalization.CultureInfo.CurrentUICulture);
				}
				else
					throw new Exception("No spatial data found in layer");

			}
			catch (Exception ex)
			{
				MessageBox.Show(this, string.Format(m_globalizor.Translate("Failed to get layer extent: {0}"), ex.Message), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

        private void AddGroupButton_Click(object sender, EventArgs e)
        {
            AddGroup();
        }

        private void RemoveGroupButton_Click(object sender, EventArgs e)
        {
            RemoveGroup();
        }

        private void AddLayerButton_Click(object sender, EventArgs e)
        {
            AddLayer();
        }

        private void RemoveLayerButton_Click(object sender, EventArgs e)
        {
            RemoveLayer();
        }

        private void MoveLayerUpButton_Click(object sender, EventArgs e)
        {
            MoveLayerUp();
        }

        private void MoveLayerDownButton_Click(object sender, EventArgs e)
        {
            MoveLayerDown();
        }

        private void AddLayerOrderButton_Click(object sender, EventArgs e)
        {
            AddLayer();
        }

        private void RemoveLayerOrderButton_Click(object sender, EventArgs e)
        {
            RemoveLayer();
        }

        private void MoveLayerUpOrderButton_Click(object sender, EventArgs e)
        {
            MoveLayerUp();
        }

        private void MoveLayerDownOrderButton_Click(object sender, EventArgs e)
        {
            MoveLayerDown();
        }
	}
}