#region Disclaimer / License

// Copyright (C) 2009, Kenneth Skovhede
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

#endregion Disclaimer / License

using Maestro.Shared.UI;
using OSGeo.MapGuide.MaestroAPI;
using OSGeo.MapGuide.MaestroAPI.Commands;
using OSGeo.MapGuide.MaestroAPI.Exceptions;
using OSGeo.MapGuide.MaestroAPI.Tile;
using OSGeo.MapGuide.ObjectModels;
using OSGeo.MapGuide.ObjectModels.Common;
using OSGeo.MapGuide.ObjectModels.MapDefinition;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace MgCooker
{
    public partial class SetupRun : Form
    {
        private IServerConnection m_connection;
        private IDictionary<string, string> m_commandlineargs;
        private IDictionary<string, IEnvelope> m_coordinateOverrides;
        private bool m_isUpdating = false;

        private SetupRun()
        {
            InitializeComponent();
            saveFileDialog.Filter = string.Format(OSGeo.MapGuide.MaestroAPI.Strings.GenericFilter, OSGeo.MapGuide.MaestroAPI.Strings.PickBat, "bat") + "|" + //NOXLATE
                                     OSGeo.MapGuide.MaestroAPI.StringConstants.AllFilesFilter; //NOXLATE
        }

        internal SetupRun(string username, string password, IServerConnection connection, string[] maps, IDictionary<string, string> args)
            : this(connection, maps, args)
        {
            var cloneP = connection.CloneParameters;
            //HACK: Provider-specific information, but there isn't too many providers for this to be a problem
            if (cloneP["SessionId"] != null)
                cloneP.Remove("SessionId");
            cloneP["Username"] = username;
            cloneP["Password"] = password;
            txtConnectionString.Text = Utility.ToConnectionString(cloneP);
        }

        public SetupRun(IServerConnection connection, string[] maps, IDictionary<string, string> args)
            : this()
        {
            m_connection = connection;

            m_commandlineargs = args;
            m_coordinateOverrides = new Dictionary<string, IEnvelope>();
            IEnvelope overrideExtents = null;

            if (m_commandlineargs.ContainsKey(TileRunParameters.MAPDEFINITIONS)) //NOXLATE
                m_commandlineargs.Remove(TileRunParameters.MAPDEFINITIONS); //NOXLATE

            if (m_commandlineargs.ContainsKey(TileRunParameters.PROVIDER) && m_commandlineargs.ContainsKey(TileRunParameters.CONNECTIONPARAMS))
            {
                txtProvider.Text = m_commandlineargs[TileRunParameters.PROVIDER];
                txtConnectionString.Text = m_commandlineargs[TileRunParameters.CONNECTIONPARAMS];
            }
            else
            {
                txtProvider.Text = connection.ProviderName;
                txtConnectionString.Text = Utility.ToConnectionString(connection.CloneParameters);
            }
            if (m_commandlineargs.ContainsKey(TileRunParameters.LIMITROWS)) //NOXLATE
            {
                int i;
                if (int.TryParse(m_commandlineargs[TileRunParameters.LIMITROWS], out i) && i > 0) //NOXLATE
                {
                    MaxRowLimit.Value = i;
                    TilesetLimitPanel.Enabled = true;
                }
            }

            if (m_commandlineargs.ContainsKey(TileRunParameters.LIMITCOLS)) //NOXLATE
            {
                int i;
                if (int.TryParse(m_commandlineargs[TileRunParameters.LIMITCOLS], out i) && i > 0) //NOXLATE
                {
                    MaxColLimit.Value = i;
                    TilesetLimitPanel.Enabled = true;
                }
            }

            if (m_commandlineargs.ContainsKey(TileRunParameters.EXTENTOVERRIDE)) //NOXLATE
            {
                string[] parts = m_commandlineargs[TileRunParameters.EXTENTOVERRIDE].Split(',');
                if (parts.Length == 4)
                {
                    double minx;
                    double miny;
                    double maxx;
                    double maxy;
                    if (
                        double.TryParse(parts[0], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out minx) &&
                        double.TryParse(parts[1], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out miny) &&
                        double.TryParse(parts[2], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out maxx) &&
                        double.TryParse(parts[3], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out maxy)
                        )
                    {
                        overrideExtents = ObjectFactory.CreateEnvelope(minx, miny, maxx, maxy);
                    }
                }
            }

            if (m_commandlineargs.ContainsKey(TileRunParameters.METERSPERUNIT)) //NOXLATE
            {
                double d;
                if (
                    double.TryParse(m_commandlineargs[TileRunParameters.METERSPERUNIT], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.CurrentUICulture, out d) //NOXLATE
                    || double.TryParse(m_commandlineargs[TileRunParameters.METERSPERUNIT], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out d) //NOXLATE
                    )
                    if (d >= (double)MetersPerUnit.Minimum && d <= (double)MetersPerUnit.Maximum)
                    {
                        MetersPerUnit.Value = (decimal)d;
                    }
            }

            if (maps == null || maps.Length == 0 || (maps.Length == 1 && maps[0].Trim().Length == 0))
            {
                List<string> tmp = new List<string>();
                foreach (ResourceListResourceDocument doc in m_connection.ResourceService.GetRepositoryResources(StringConstants.RootIdentifier, ResourceTypes.MapDefinition.ToString()).Items)
                    tmp.Add(doc.ResourceId);
                maps = tmp.ToArray();
            }

            var basegroupsSelected = new List<string>();
            if (m_commandlineargs.ContainsKey(TileRunParameters.BASEGROUPS))//NOXLATE
            {
                basegroupsSelected = new List<string>(m_commandlineargs[TileRunParameters.BASEGROUPS].Split(','));//NOXLATE
                m_commandlineargs.Remove(TileRunParameters.BASEGROUPS); //NOXLATE
            }

            var scalesSelected = new List<int>();
            if (m_commandlineargs.ContainsKey(TileRunParameters.SCALEINDEX)) //NOXLATE
            {
                foreach (string scaleIndex in m_commandlineargs[TileRunParameters.SCALEINDEX].Split(','))//NOXLATE
                {
                    scalesSelected.Add(int.Parse(scaleIndex));
                }
                m_commandlineargs.Remove(TileRunParameters.SCALEINDEX); //NOXLATE
            }

            MapTree.Nodes.Clear();
            foreach (string m in maps)
            {
                IMapDefinition mdef = m_connection.ResourceService.GetResource(m) as IMapDefinition;
                if (mdef == null) //Skip unknown Map Definition version (which would be returned as UntypedResource objects)
                    continue;

                IBaseMapDefinition baseMap = mdef.BaseMap;
                if (baseMap != null &&
                    baseMap.ScaleCount > 0 &&
                    baseMap.HasGroups())
                {
                    TreeNode mn = MapTree.Nodes.Add(m);

                    mn.ImageIndex = mn.SelectedImageIndex = 0;
                    mn.Tag = mdef;
                    foreach (var g in baseMap.BaseMapLayerGroups)
                    {
                        TreeNode gn = mn.Nodes.Add(g.Name);
                        gn.Tag = g;
                        if (basegroupsSelected.Contains(g.Name))
                        {
                            mn.Checked = true;
                            gn.Checked = true;
                            if (overrideExtents != null && !m_coordinateOverrides.ContainsKey(m))
                            {
                                m_coordinateOverrides.Add(m, overrideExtents);
                            }
                        }

                        gn.ImageIndex = gn.SelectedImageIndex = 1;

                        int counter = 0;
                        foreach (double d in baseMap.FiniteDisplayScale)
                        {
                            TreeNode sn = gn.Nodes.Add(d.ToString(System.Globalization.CultureInfo.CurrentUICulture));
                            if (gn.Checked && scalesSelected.Contains(counter))
                            {
                                sn.Checked = true;
                            }
                            sn.ImageIndex = sn.SelectedImageIndex = 3;
                            counter++;
                        }
                    }

                    mn.Expand();
                }
            }
            MapTree_AfterSelect(null, null);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnBuild_Click(object sender, EventArgs e)
        {
            IServerConnection con = m_connection;
            try
            {
                TilingRunCollection bx = new TilingRunCollection(con);

                if (LimitTileset.Checked)
                {
                    if (MaxRowLimit.Value > 0)
                        bx.LimitRows((int)MaxRowLimit.Value);
                    if (MaxColLimit.Value > 0)
                        bx.LimitCols((int)MaxColLimit.Value);
                }

                bx.Config.MetersPerUnit = (double)MetersPerUnit.Value;

                bx.Config.ThreadCount = (int)ThreadCount.Value;
                bx.Config.RandomizeTileSequence = RandomTileOrder.Checked;

                foreach (Config c in ReadTree())
                {
                    MapTilingConfiguration bm = new MapTilingConfiguration(bx, c.MapDefinition);
                    bm.SetGroups(new string[] { c.Group });
                    bm.SetScalesAndExtend(c.ScaleIndexes, c.ExtentOverride);

                    bx.Maps.Add(bm);
                }

                Progress p = new Progress(bx);
                if (p.ShowDialog(this) != DialogResult.Cancel)
                {
                    var ts = p.TotalTime;
                    MessageBox.Show(string.Format(Strings.TileGenerationCompleted, ((ts.Days * 24) + ts.Hours), ts.Minutes, ts.Seconds));
                }
            }
            catch (Exception ex)
            {
                string msg = NestedExceptionMessageProcessor.GetFullMessage(ex);
                MessageBox.Show(this, string.Format(Strings.InternalError, msg), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private List<Config> ReadTree()
        {
            List<Config> lst = new List<Config>();
            foreach (TreeNode mn in MapTree.Nodes)
            {
                if (mn.Checked)
                {
                    foreach (TreeNode gn in mn.Nodes)
                        if (gn.Checked)
                        {
                            List<int> ix = new List<int>();
                            foreach (TreeNode sn in gn.Nodes)
                                if (sn.Checked)
                                    ix.Add(sn.Index);

                            if (ix.Count > 0)
                                lst.Add(new Config(mn.Text, gn.Text, ix.ToArray(), (m_coordinateOverrides.ContainsKey(mn.Text) ? m_coordinateOverrides[mn.Text] : null)));
                        }
                }
            }

            return lst;
        }

        private class Config
        {
            public string MapDefinition;
            public string Group;
            public int[] ScaleIndexes;
            public IEnvelope ExtentOverride = null;

            public Config(string MapDefinition, string Group, int[] ScaleIndexes, IEnvelope ExtentOverride)
            {
                this.MapDefinition = MapDefinition;
                this.Group = Group;
                this.ScaleIndexes = ScaleIndexes;
                this.ExtentOverride = ExtentOverride;
            }
        }

        private void btnSaveScript_Click(object sender, EventArgs e)
        {
            if (System.Environment.OSVersion.Platform == PlatformID.Unix)
                saveFileDialog.Filter =
                    string.Format(Strings.FileTypeShellScript + "|{0}", "*.sh") + //NOXLATE
                    string.Format(Strings.FileTypeAllFiles + "|{0}", "*.*"); //NOXLATE

            if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                //Common args for all map defintions to be tiled
                List<string> args = new List<string>();

                args.Add("--" + TileRunParameters.PROVIDER + "=\"" + txtProvider.Text + "\""); //NOXLATE
                args.Add("--" + TileRunParameters.CONNECTIONPARAMS + "=\"" + txtConnectionString.Text + "\""); //NOXLATE

                if (LimitTileset.Checked)
                {
                    if (MaxRowLimit.Value > 0)
                        args.Add("--" + TileRunParameters.LIMITROWS + "=\"" + ((int)MaxRowLimit.Value).ToString() + "\""); //NOXLATE
                    if (MaxColLimit.Value > 0)
                        args.Add("--" + TileRunParameters.LIMITCOLS + "=\"" + ((int)MaxColLimit.Value).ToString() + "\""); //NOXLATE
                }

                args.Add("--" + TileRunParameters.METERSPERUNIT + "=" + ((double)MetersPerUnit.Value).ToString(System.Globalization.CultureInfo.InvariantCulture)); //NOXLATE

                args.Add("--" + TileRunParameters.THREADCOUNT + "=" + ((int)ThreadCount.Value).ToString()); //NOXLATE
                if (RandomTileOrder.Checked)
                    args.Add("--" + TileRunParameters.RANDOMTILEORDER + ""); //NOXLATE

                string executable = System.IO.Path.GetFileName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                string cmdExecutable = "MgCookerCmd.exe"; //NOXLATE

                //Windows has problems with console output from GUI applications...
                if (System.Environment.OSVersion.Platform != PlatformID.Unix && executable == "MgCooker.exe" && System.IO.File.Exists(System.IO.Path.Combine(Application.StartupPath, cmdExecutable))) //NOXLATE
                    executable = System.IO.Path.Combine(Application.StartupPath, cmdExecutable); //NOXLATE
                else
                    executable = System.IO.Path.Combine(Application.StartupPath, executable);

                string exeName = System.IO.Path.GetFileName(executable);
                string exePath = System.IO.Path.GetDirectoryName(executable);

                executable = "\"" + executable + "\""; //NOXLATE

                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(saveFileDialog.FileName))
                {
                    if (System.Environment.OSVersion.Platform == PlatformID.Unix)
                    {
                        sw.WriteLine("#!/bin/sh"); //NOXLATE
                        executable = "mono " + executable; //NOXLATE
                    }
                    else
                    {
                        sw.WriteLine("@echo off"); //NOXLATE
                    }

                    //If on windows, wrap the exe call in a pushd/popd so that the executable is
                    //executed from its own directory

                    if (System.Environment.OSVersion.Platform != PlatformID.MacOSX ||
                        System.Environment.OSVersion.Platform != PlatformID.Unix)
                    {
                        sw.WriteLine("pushd \"" + exePath + "\""); //NOXLATE
                    }

                    foreach (Config c in ReadTree())
                    {
                        //Map-specific args
                        List<string> argsMap = new List<string>();
                        if (System.Environment.OSVersion.Platform != PlatformID.MacOSX ||
                            System.Environment.OSVersion.Platform != PlatformID.Unix)
                        {
                            argsMap.Add(exeName);
                        }
                        else
                        {
                            argsMap.Add(executable);
                        }

                        argsMap.Add("batch"); //NOXLATE
                        argsMap.Add("--" + TileRunParameters.MAPDEFINITIONS + "=\"" + c.MapDefinition + "\"");
                        argsMap.Add("--" + TileRunParameters.BASEGROUPS + "=\"" + c.Group + "\"");
                        StringBuilder si = new StringBuilder("--" + TileRunParameters.SCALEINDEX + "="); //NOXLATE
                        for (int i = 0; i < c.ScaleIndexes.Length; i++)
                        {
                            if (i != 0)
                                si.Append(","); //NOXLATE
                            si.Append(c.ScaleIndexes[i].ToString());
                        }
                        argsMap.Add(si.ToString());

                        if (c.ExtentOverride != null)
                        {
                            StringBuilder ov = new StringBuilder("--" + TileRunParameters.EXTENTOVERRIDE + "="); //NOXLATE
                            ov.Append(c.ExtentOverride.MinX.ToString(System.Globalization.CultureInfo.InvariantCulture));
                            ov.Append(","); //NOXLATE
                            ov.Append(c.ExtentOverride.MinY.ToString(System.Globalization.CultureInfo.InvariantCulture));
                            ov.Append(","); //NOXLATE
                            ov.Append(c.ExtentOverride.MaxX.ToString(System.Globalization.CultureInfo.InvariantCulture));
                            ov.Append(","); //NOXLATE
                            ov.Append(c.ExtentOverride.MaxY.ToString(System.Globalization.CultureInfo.InvariantCulture));
                            argsMap.Add(ov.ToString());
                        }

                        string[] argsFinal = new string[args.Count + argsMap.Count];
                        int a = 0;
                        //Map-specific args first (as this contains the executable name)
                        foreach (string arg in argsMap)
                        {
                            argsFinal[a] = arg;
                            a++;
                        }
                        //Then the common args
                        foreach (string arg in args)
                        {
                            argsFinal[a] = arg;
                            a++;
                        }

                        sw.Write(string.Join(" ", argsFinal));
                        sw.WriteLine();
                    }

                    if (System.Environment.OSVersion.Platform != PlatformID.MacOSX ||
                        System.Environment.OSVersion.Platform != PlatformID.Unix)
                    {
                        sw.WriteLine("popd"); //NOXLATE
                    }
                }
            }
        }

        private void treeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            bool byuser = e.Action == TreeViewAction.ByKeyboard || e.Action == TreeViewAction.ByMouse;

            if (e.Node == null)
                return;

            if (byuser)
            {
                foreach (TreeNode n in e.Node.Nodes)
                {
                    foreach (TreeNode tn in n.Nodes)
                        tn.Checked = e.Node.Checked;

                    n.Checked = e.Node.Checked;
                }

                if (e.Node.Parent != null)
                {
                    int c = 0;

                    foreach (TreeNode n in e.Node.Parent.Nodes)
                        if (n.Checked)
                            c++;

                    if (c > 0)
                    {
                        e.Node.Parent.Checked = true;
                        if (e.Node.Parent.Parent != null)
                            e.Node.Parent.Parent.Checked = true;
                    }
                }
            }
        }

        private void LimitTileset_CheckedChanged(object sender, EventArgs e)
        {
            TilesetLimitPanel.Enabled = LimitTileset.Checked;
        }

        private void MapTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (m_isUpdating)
                return;

            if (MapTree.SelectedNode == null)
            {
                BoundsOverride.Enabled = false;
                BoundsOverride.Tag = null;
            }
            else
            {
                BoundsOverride.Enabled = true;
                TreeNode root = MapTree.SelectedNode;
                while (root.Parent != null)
                    root = root.Parent;

                IEnvelope box;
                if (m_coordinateOverrides.ContainsKey(root.Text))
                    box = m_coordinateOverrides[root.Text];
                else
                    box = ((IMapDefinition)root.Tag).Extents;

                BoundsOverride.Tag = root;

                try
                {
                    m_isUpdating = true;
                    txtLowerX.Text = box.MinX.ToString(System.Globalization.CultureInfo.CurrentUICulture);
                    txtLowerY.Text = box.MinY.ToString(System.Globalization.CultureInfo.CurrentUICulture);
                    txtUpperX.Text = box.MaxX.ToString(System.Globalization.CultureInfo.CurrentUICulture);
                    txtUpperY.Text = box.MaxY.ToString(System.Globalization.CultureInfo.CurrentUICulture);
                }
                finally
                {
                    m_isUpdating = false;
                }

                ModfiedOverrideWarning.Visible = m_coordinateOverrides.ContainsKey(root.Text);
            }
        }

        private void ResetBounds_Click(object sender, EventArgs e)
        {
            if (BoundsOverride.Tag as TreeNode == null)
                return;

            TreeNode root = BoundsOverride.Tag as TreeNode;

            if (m_coordinateOverrides.ContainsKey(root.Text))
                m_coordinateOverrides.Remove(root.Text);

            MapTree_AfterSelect(null, null);
        }

        private void CoordinateItem_TextChanged(object sender, EventArgs e)
        {
            if (BoundsOverride.Tag as TreeNode == null || m_isUpdating)
                return;

            TreeNode root = BoundsOverride.Tag as TreeNode;

            if (!m_coordinateOverrides.ContainsKey(root.Text))
            {
                //IEnvelope newbox = new OSGeo.MapGuide.IEnvelope();
                IEnvelope origbox = ((IMapDefinition)root.Tag).Extents;
                IEnvelope newbox = origbox.Clone();

                m_coordinateOverrides.Add(root.Text, newbox);
            }

            double d;
            if (double.TryParse(txtLowerX.Text, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.CurrentUICulture, out d))
                m_coordinateOverrides[root.Text].MinX = d;
            if (double.TryParse(txtLowerY.Text, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.CurrentUICulture, out d))
                m_coordinateOverrides[root.Text].MinY = d;
            if (double.TryParse(txtUpperX.Text, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.CurrentUICulture, out d))
                m_coordinateOverrides[root.Text].MaxX = d;
            if (double.TryParse(txtUpperY.Text, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.CurrentUICulture, out d))
                m_coordinateOverrides[root.Text].MaxY = d;
        }

        private void lnkCalcMpu_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var configs = ReadTree();
            var mapDefs = configs.Select(x => x.MapDefinition).Distinct().ToArray();

            if (mapDefs.Length == 0)
            {
                MessageBox.Show(Strings.NoMapSelected);
            }
            else if (mapDefs.Length == 1)
            {
                TryCalcMpu(mapDefs[0]);
            }
            else
            {
                MessageBox.Show(Strings.CannotCalculateMpuForMultipleMaps);
            }
        }

        private enum MpuMethod
        {
            CreateRuntimeMap,
            MpuCalcExe,
            BuiltIn
        }

        private class MpuCalcResult
        {
            public MpuMethod Method;
            public decimal Result;
        }

        private void TryCalcMpu(string mapDef)
        {
            BusyWaitDialog.Run(Strings.CalculatingMpu, () =>
            {
                var currentPath = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
                var mpuCalc = Path.Combine(currentPath, "AddIns/Local/MpuCalc.exe");
                if (!File.Exists(mpuCalc))
                {
                    int[] cmdTypes = m_connection.Capabilities.SupportedCommands;
                    if (Array.IndexOf(cmdTypes, (int)OSGeo.MapGuide.MaestroAPI.Commands.CommandType.CreateRuntimeMap) < 0)
                    {
                        IMapDefinition mdf = (IMapDefinition)m_connection.ResourceService.GetResource(mapDef);
                        var calc = m_connection.GetCalculator();
                        return new MpuCalcResult()
                        {
                            Method = MpuMethod.BuiltIn,
                            Result = Convert.ToDecimal(calc.Calculate(mdf.CoordinateSystem, 1.0))
                        };
                    }
                    else
                    {
                        ICreateRuntimeMap create = (ICreateRuntimeMap)m_connection.CreateCommand((int)OSGeo.MapGuide.MaestroAPI.Commands.CommandType.CreateRuntimeMap);
                        create.MapDefinition = mapDef;
                        create.RequestedFeatures = (int)RuntimeMapRequestedFeatures.None;
                        var info = create.Execute();
                        return new MpuCalcResult()
                        {
                            Method = MpuMethod.CreateRuntimeMap,
                            Result = Convert.ToDecimal(info.CoordinateSystem.MetersPerUnit)
                        };
                    }
                }
                else
                {
                    IMapDefinition mdf = (IMapDefinition)m_connection.ResourceService.GetResource(mapDef);
                    var proc = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = mpuCalc,
                            Arguments = mdf.CoordinateSystem,
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            CreateNoWindow = true
                        }
                    };
                    proc.Start();
                    StringBuilder sb = new StringBuilder();
                    while (!proc.StandardOutput.EndOfStream)
                    {
                        string line = proc.StandardOutput.ReadLine();
                        // do something with line
                        sb.AppendLine(line);
                    }
                    string output = sb.ToString();
                    double mpu;
                    if (double.TryParse(output, out mpu))
                        return new MpuCalcResult() { Method = MpuMethod.MpuCalcExe, Result = Convert.ToDecimal(mpu) };
                    else
                        return string.Format(Strings.FailedToCalculateMpu, output);
                }
            }, (res, ex) =>
            {
                if (ex != null)
                {
                    ErrorDialog.Show(ex);
                }
                else
                {
                    var mres = res as MpuCalcResult;
                    if (mres != null)
                    {
                        MetersPerUnit.Value = mres.Result;
                        if (mres.Method == MpuMethod.BuiltIn)
                        {
                            MessageBox.Show(Strings.ImperfectMpuCalculation);
                        }
                    }
                    else
                    {
                        MessageBox.Show(res.ToString());
                    }
                }
            });
        }
    }
}