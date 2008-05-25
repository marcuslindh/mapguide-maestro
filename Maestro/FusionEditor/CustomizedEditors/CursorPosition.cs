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
using System.Windows.Forms;
using OSGeo.MapGuide.MaestroAPI.ApplicationDefinition;

namespace OSGeo.MapGuide.Maestro.FusionEditor.CustomizedEditors
{
	public class CursorPosition : FusionEditor.BasisWidgetEditor
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox Precision;
		private System.Windows.Forms.TextBox Units;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox Template;
		private System.Windows.Forms.Label label3;
		private System.ComponentModel.IContainer components = null;

		public CursorPosition()
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		public override void SetItem(WidgetType w)
		{
			try
			{
				m_isUpdating = true;
				m_w = w;
				this.Enabled = m_w != null;
				SetSettingValue("Precision", Precision.Text);
				SetSettingValue("Units", Units.Text);
				SetSettingValue("Template", Template.Text);
			} 
			finally
			{
				m_isUpdating = false;
			}
		}


		#region Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Precision = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.Units = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.Template = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// Precision
			// 
			this.Precision.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.Precision.Location = new System.Drawing.Point(104, 8);
			this.Precision.Name = "Precision";
			this.Precision.Size = new System.Drawing.Size(504, 20);
			this.Precision.TabIndex = 3;
			this.Precision.Text = "";
			this.Precision.TextChanged += new System.EventHandler(this.Precision_TextChanged);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(72, 16);
			this.label1.TabIndex = 2;
			this.label1.Text = "Precision";
			// 
			// Units
			// 
			this.Units.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.Units.Location = new System.Drawing.Point(104, 56);
			this.Units.Name = "Units";
			this.Units.Size = new System.Drawing.Size(504, 20);
			this.Units.TabIndex = 5;
			this.Units.Text = "";
			this.Units.TextChanged += new System.EventHandler(this.Units_TextChanged);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 56);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(72, 16);
			this.label2.TabIndex = 4;
			this.label2.Text = "Units";
			// 
			// Template
			// 
			this.Template.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.Template.Location = new System.Drawing.Point(104, 32);
			this.Template.Name = "Template";
			this.Template.Size = new System.Drawing.Size(504, 20);
			this.Template.TabIndex = 7;
			this.Template.Text = "";
			this.Template.TextChanged += new System.EventHandler(this.Template_TextChanged);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(8, 32);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(72, 16);
			this.label3.TabIndex = 6;
			this.label3.Text = "Template";
			// 
			// CursorPosition
			// 
			this.Controls.Add(this.Template);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.Units);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.Precision);
			this.Controls.Add(this.label1);
			this.Name = "CursorPosition";
			this.Size = new System.Drawing.Size(616, 88);
			this.ResumeLayout(false);

		}
		#endregion

		private void Precision_TextChanged(object sender, System.EventArgs e)
		{
			if (m_isUpdating || m_w == null)
				return;

			SetSettingValue("Precision", Precision.Text);
		}

		private void Template_TextChanged(object sender, System.EventArgs e)
		{
			if (m_isUpdating || m_w == null)
				return;

			SetSettingValue("Template", Template.Text);
		}

		private void Units_TextChanged(object sender, System.EventArgs e)
		{
			if (m_isUpdating || m_w == null)
				return;

			SetSettingValue("Units", Units.Text);
		}
	}
}
