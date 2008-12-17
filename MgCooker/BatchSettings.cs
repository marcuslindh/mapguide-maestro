#region Disclaimer / License
// Copyright (C) 2008, Kenneth Skovhede
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
using System.Collections.Generic;
using System.Text;

namespace OSGeo.MapGuide.MgCooker
{
    /// <summary>
    /// This delegate is used to monitor progress on tile rendering
    /// </summary>
    /// <param name="map">The map currently being processed</param>
    /// <param name="group">The group being processed</param>
    /// <param name="scaleindex">The scaleindex being processed</param>
    /// <param name="row">The row being processed</param>
    /// <param name="column">The column being processed</param>
    /// <param name="cancel">A control flag to stop the tile rendering</param>
    /// <param name="state">The state that invoked the callback</param>
    public delegate void ProgressCallback(CallbackStates state, BatchMap map, string group, int scaleindex, int row, int column, ref bool cancel);

    /// <summary>
    /// This delegate is used to monitor progress on tile rendering
    /// </summary>
    /// <param name="map">The map currently being processed</param>
    /// <param name="group">The group being processed</param>
    /// <param name="scaleindex">The scaleindex being processed</param>
    /// <param name="row">The row being processed</param>
    /// <param name="column">The column being processed</param>
    /// <param name="state">The state that invoked the callback</param>
    /// <param name="exception">The exception from the last attempt, set this to null to ignore the exception</param>
    public delegate void ErrorCallback(CallbackStates state, BatchMap map, string group, int scaleindex, int row, int column, ref Exception exception);

    /// <summary>
    /// These are the avalible states for callbacks
    /// </summary>
    public enum CallbackStates
    {
        /// <summary>
        /// All maps are being rendered
        /// </summary>
        StartRenderAllMaps,
        /// <summary>
        /// A map is being rendered
        /// </summary>
        StartRenderMap,
        /// <summary>
        /// A group is being rendered
        /// </summary>
        StartRenderGroup,
        /// <summary>
        /// A scale is being rendered
        /// </summary>
        StartRenderScale,
        /// <summary>
        /// A tile is being rendered
        /// </summary>
        StartRenderTile,
        /// <summary>
        /// A tile has been rendered
        /// </summary>
        FinishRenderTile,
        /// <summary>
        /// A scale has been rendered
        /// </summary>
        FinishRenderScale,
        /// <summary>
        /// A group has been rendered
        /// </summary>
        FinishRenderGroup,
        /// <summary>
        /// A map has been rendered
        /// </summary>
        FinishRenderMap,
        /// <summary>
        /// All maps have been rendered
        /// </summary>
        FinishRenderAllMaps,
        /// <summary>
        /// A tile has failed to render
        /// </summary>
        FailedRenderingTile,
    }

    /// <summary>
    /// Class to hold settings for a batch run of tile building
    /// </summary>
    public class BatchSettings
    {
        /// <summary>
        /// A reference to the connection
        /// </summary>
        private MaestroAPI.ServerConnectionI m_connection;
        /// <summary>
        /// The list of maps
        /// </summary>
        private List<BatchMap> m_maps;
        /// <summary>
        /// A default set of tile settings
        /// </summary>
        private TileSettings m_tileSettings = new TileSettings();

        /// <summary>
        /// A flag that indicates the rendering should stop
        /// </summary>
        private bool m_cancel;

        #region Events
        /// <summary>
        /// All maps are being rendered
        /// </summary>
        public event ProgressCallback BeginRenderingMaps;
        /// <summary>
        /// A map is being rendered
        /// </summary>
        public event ProgressCallback BeginRenderingMap;
        /// <summary>
        /// A group is being rendered
        /// </summary>
        public event ProgressCallback BeginRenderingGroup;
        /// <summary>
        /// A scale is being rendered
        /// </summary>
        public event ProgressCallback BeginRenderingScale;
        /// <summary>
        /// A tile is being rendered
        /// </summary>
        public event ProgressCallback BeginRenderingTile;
        /// <summary>
        /// All maps have been rendered
        /// </summary>
        public event ProgressCallback FinishRenderingMaps;
        /// <summary>
        /// A map has been rendered
        /// </summary>
        public event ProgressCallback FinishRenderingMap;
        /// <summary>
        /// A group has been rendered
        /// </summary>
        public event ProgressCallback FinishRenderingGroup;
        /// <summary>
        /// A scale has been rendered
        /// </summary>
        public event ProgressCallback FinishRenderingScale;
        /// <summary>
        /// A tile has been rendered
        /// </summary>
        public event ProgressCallback FinishRenderingTile;
        /// <summary>
        /// A tile has failed to render
        /// </summary>
        public event ErrorCallback FailedRenderingTile;

        internal void InvokeBeginRendering(BatchMap batchMap)
        {
            if (this.BeginRenderingMap != null)
                this.BeginRenderingMap(CallbackStates.StartRenderMap, batchMap, null, -1, -1, -1, ref m_cancel);
        }

        internal void InvokeFinishRendering(BatchMap batchMap)
        {
            if (this.FinishRenderingMap != null)
                this.FinishRenderingMap(CallbackStates.FinishRenderMap, batchMap, null, -1, -1, -1, ref m_cancel);
        }

        internal void InvokeBeginRendering(BatchMap batchMap, string group)
        {
            if (this.BeginRenderingGroup != null)
                this.BeginRenderingGroup(CallbackStates.StartRenderGroup, batchMap, group, -1, -1, -1, ref m_cancel);
        }

        internal void InvokeFinishRendering(BatchMap batchMap, string group)
        {
            if (this.FinishRenderingGroup != null)
                this.FinishRenderingGroup(CallbackStates.FinishRenderGroup, batchMap, group, -1, -1, -1, ref m_cancel);
        }

        internal void InvokeBeginRendering(BatchMap batchMap, string group, int scaleindex)
        {
            if (this.BeginRenderingScale != null)
                this.BeginRenderingScale(CallbackStates.StartRenderScale, batchMap, group, scaleindex, -1, -1, ref m_cancel);
        }

        internal void InvokeFinishRendering(BatchMap batchMap, string group, int scaleindex)
        {
            if (this.FinishRenderingScale != null)
                this.FinishRenderingScale(CallbackStates.FinishRenderScale, batchMap, group, scaleindex, -1, -1, ref m_cancel);
        }

        internal void InvokeBeginRendering(BatchMap batchMap, string group, int scaleindex, int row, int col)
        {
            if (this.BeginRenderingTile != null)
                this.BeginRenderingTile(CallbackStates.StartRenderTile, batchMap, group, scaleindex, row, col, ref m_cancel);
        }

        internal void InvokeFinishRendering(BatchMap batchMap, string group, int scaleindex, int row, int col)
        {
            if (this.FinishRenderingTile != null)
                this.FinishRenderingTile(CallbackStates.FinishRenderTile, batchMap, group, scaleindex, row, col, ref m_cancel);
        }

        internal void InvokeError(BatchMap batchMap, string group, int scaleindex, int row, int col, ref Exception exception)
        {
            if (this.FailedRenderingTile != null)
                this.FailedRenderingTile(CallbackStates.FailedRenderingTile, batchMap, group, scaleindex, row, col, ref exception);
        }

        #endregion

        /// <summary>
        /// Constructs a new batch setup. If no maps are supplied, all maps in the repository is assumed.
        /// </summary>
        /// <param name="mapagent">The url to the mapagent.fcgi</param>
        /// <param name="username">The username to connect with</param>
        /// <param name="password">The password to connect with</param>
        /// <param name="maps">A list of maps to process, leave empty to process all layers</param>
        public BatchSettings(string mapagent, string username, string password, params string[] maps)
            : this(new MaestroAPI.HttpServerConnection(new Uri(mapagent), username, password, null, true), maps)
        {
        }

        public BatchSettings(MaestroAPI.ServerConnectionI connection)
        {
            m_connection = connection;
            m_maps = new List<BatchMap>();
        }

        public BatchSettings(MaestroAPI.ServerConnectionI connection, params string[] maps)
        {
            m_connection = connection;
            m_maps = new List<BatchMap>();

            if (maps == null || maps.Length == 0 || (maps.Length == 1 && maps[0].Trim().Length == 0))
            {
                List<string> tmp = new List<string>();
                foreach (MaestroAPI.ResourceListResourceDocument doc in m_connection.GetRepositoryResources("Library://", "MapDefinition").Items)
                    tmp.Add(doc.ResourceId);
                maps = tmp.ToArray();
            }

            foreach (string s in maps)
            {
                BatchMap bm = new BatchMap(this, s);
                if (bm.Resolutions > 0)
                    m_maps.Add(bm);
            }
        }

        public void SetScales(int[] scaleindexes)
        {
            foreach (BatchMap bm in m_maps)
                bm.SetScales(scaleindexes);
        }

        public void SetGroups(string[] groups)
        {
            foreach (BatchMap bm in m_maps)
                bm.SetGroups(groups);
        }

        public void LimitRows(long limit)
        {
            foreach (BatchMap bm in m_maps)
                bm.LimitRows(limit);
        }

        public void LimitCols(long limit)
        {
            foreach (BatchMap bm in m_maps)
                bm.LimitCols(limit);
        }

        /// <summary>
        /// Renders all tiles in all maps
        /// </summary>
        public void RenderAll()
        {
            m_cancel = false;

            if (this.BeginRenderingMaps != null)
                this.BeginRenderingMaps(CallbackStates.StartRenderAllMaps, null, null, -1, -1, -1, ref m_cancel);

            foreach (BatchMap bm in this.Maps)
                if (m_cancel)
                    break;
                else
                    bm.Render();

            if (this.FinishRenderingMaps != null)
                this.FinishRenderingMaps(CallbackStates.FinishRenderAllMaps, null, null, -1, -1, -1, ref m_cancel);
        }

        /// <summary>
        /// The connection to the server
        /// </summary>
        public MaestroAPI.ServerConnectionI Connection { get { return m_connection; } }
        /// <summary>
        /// The list of maps to proccess
        /// </summary>
        public List<BatchMap> Maps { get { return m_maps; } }

        /// <summary>
        /// The tile settings
        /// </summary>
        public TileSettings Config { get { return m_tileSettings; } }

        /// <summary>
        /// Gets a flag indicating if the rendering process is cancelled
        /// </summary>
        public bool Cancel { get { return m_cancel; } }
    }

    /// <summary>
    /// Class that represents a single map to build tiles for
    /// </summary>
    public class BatchMap
    {
        /// <summary>
        /// A reference to the parent, and thus the connection
        /// </summary>
        private BatchSettings m_parent;
        /// <summary>
        /// The map read from MapGuide
        /// </summary>
        private MaestroAPI.MapDefinition m_mapdefinition;

        /// <summary>
        /// The max extent of the map
        /// </summary>
        private MaestroAPI.Box2DType m_maxExtent;

        /// <summary>
        /// The list of baselayer group names
        /// </summary>
        private string[] m_groups;

        /// <summary>
        /// For each entry there is two longs, row and column
        /// </summary>
        private long[][] m_dimensions;

        /// <summary>
        /// Constructs a new map to be processed
        /// </summary>
        /// <param name="parent">The parent entry</param>
        /// <param name="map">The resource id for the mapdefinition</param>
        public BatchMap(BatchSettings parent, string map)
        {
            m_parent = parent;
            m_mapdefinition = parent.Connection.GetMapDefinition(map);

            if (m_mapdefinition.BaseMapDefinition != null && m_mapdefinition.BaseMapDefinition.FiniteDisplayScale != null && m_mapdefinition.BaseMapDefinition.FiniteDisplayScale.Count != 0)
            {
                m_groups = new string[m_mapdefinition.BaseMapDefinition.BaseMapLayerGroup.Count];
                for (int i = 0; i < m_mapdefinition.BaseMapDefinition.BaseMapLayerGroup.Count; i++)
                    m_groups[i] = m_mapdefinition.BaseMapDefinition.BaseMapLayerGroup[i].Name;

                CalculateDimensions();
            }
        }

        public void CalculateDimensions()
        {
            if (m_mapdefinition.BaseMapDefinition.FiniteDisplayScale.Count == 0)
            {
                m_dimensions = new long[0][];
                return;
            }

            MaestroAPI.Box2DType extents = m_maxExtent == null ? m_mapdefinition.Extents : m_maxExtent;
            double maxscale = m_mapdefinition.BaseMapDefinition.FiniteDisplayScale[m_mapdefinition.BaseMapDefinition.FiniteDisplayScale.Count - 1];

            m_dimensions = new long[this.Resolutions][];
            for (int i = this.Resolutions - 1; i >= 0; i--)
            {
                double scale = m_mapdefinition.BaseMapDefinition.FiniteDisplayScale[i];
                long pw = (long)(m_parent.Config.DisplayResolutionWidth * (1 / (scale / maxscale)));
                long ph = (long)(m_parent.Config.DisplayResolutionHeight * (1 / (scale / maxscale)));

                long rows = (pw + (m_parent.Config.TileWidth - 1)) / m_parent.Config.TileWidth;
                long cols = (ph + (m_parent.Config.TileHeight - 1)) / m_parent.Config.TileHeight;
                rows += rows % 2;
                cols += cols % 2;

                m_dimensions[i] = new long[] {rows, cols};

            }
        }

        public void SetGroups(string[] groups)
        {
            List<string> g = new List<string>();
            for(int i = 0; i < m_groups.Length; i++)
                if (Array.IndexOf<string>(groups, m_groups[i]) >= 0)
                    g.Add(m_groups[i]);

            m_groups = g.ToArray();
        }

        public void SetScales(int[] scaleindexes)
        {
            //TODO: Re-read scales from mapdef?
            SortedList<int, int> s = new SortedList<int, int>();
            foreach (int i in scaleindexes)
                if (!s.ContainsKey(i))
                    s.Add(i, i);

            List<int> keys = new List<int>(s.Keys);
            keys.Reverse();

            for (int i = m_mapdefinition.BaseMapDefinition.FiniteDisplayScale.Count - 1; i >= 0; i--)
                if (!keys.Contains(i))
                    m_mapdefinition.BaseMapDefinition.FiniteDisplayScale.RemoveAt(i);

            CalculateDimensions();
        }

        public void LimitCols(long limit)
        {
            foreach (long[] d in m_dimensions)
                d[1] = Math.Min(limit, d[1]);
        }

        public void LimitRows(long limit)
        {
            foreach (long[] d in m_dimensions)
                d[0] = Math.Min(limit, d[0]);
        }

        public long TotalTiles
        {
            get
            {
                long t = 0;
                foreach (long[] d in m_dimensions)
                    t += d[0] * d[1];
                return t;
            }
        }

        /// <summary>
        /// Gets the number of resolutions for the map
        /// </summary>
        public int Resolutions
        {
            get
            {
                if (m_mapdefinition.BaseMapDefinition == null || m_mapdefinition.BaseMapDefinition.FiniteDisplayScale == null)
                    return 0;
                else
                    return m_mapdefinition.BaseMapDefinition.FiniteDisplayScale.Count;
            }
        }

        /// <summary>
        /// Renders a single tile
        /// </summary>
        /// <param name="row">The row index of the tile</param>
        /// <param name="col">The column index of the tile</param>
        /// <param name="scaleindex">The scale index of the tile</param>
        /// <param name="group">The name of the baselayer group</param>
        public void RenderTile(long row, long col, int scaleindex, string group)
        {
            m_parent.InvokeBeginRendering(this, group, scaleindex, (int)row, (int)col);

            int c = m_parent.Config.RetryCount;
            while (c > 0)
            {
                c--;
                try
                {
                    if (!m_parent.Cancel)
                        m_parent.Connection.GetTile(m_mapdefinition.ResourceId, group, (int)col, (int)row, scaleindex, "PNG").Dispose();

                    break;
                }
                catch (Exception ex)
                {
                    if (c == 0)
                    {
                        Exception pex = ex;
                        m_parent.InvokeError(this, group, scaleindex, (int)row, (int)col, ref ex);

                        if (ex == null)
                            break;

                        if (pex == ex)
                            throw;
                        else
                            throw ex;
                    }
                }
            }

            m_parent.InvokeFinishRendering(this, group, scaleindex, (int)row, (int)col);
        }

        /// <summary>
        /// Renders all tiles in a given scale
        /// </summary>
        /// <param name="scaleindex">The scale to render</param>
        /// <param name="group">The name of the baselayer group</param>
        public void RenderScale(int scaleindex, string group)
        {
            m_parent.InvokeBeginRendering(this, group, scaleindex);

            if (!m_parent.Cancel)
            {
                int rows = (int)m_dimensions[scaleindex][0];
                int cols = (int)m_dimensions[scaleindex][1];

                for (int r = 0; r < rows; r++)
                    if (m_parent.Cancel)
                        break;
                    else
                        for (int c = 0; c < cols; c++)
                            if (m_parent.Cancel)
                                break;
                            else
                                RenderTile(r - (rows / 2), c - (cols / 2), scaleindex, group);

            }

            m_parent.InvokeFinishRendering(this, group, scaleindex);

        }

        /// <summary>
        /// Renders all tiles in all scales
        /// </summary>
        /// <param name="group">The name of the baselayer group</param>
        public void RenderGroup(string group)
        {
            m_parent.InvokeBeginRendering(this, group);

            if (!m_parent.Cancel)
            {

                for (int i = this.Resolutions - 1; i >= 0; i--)
                    if (m_parent.Cancel)
                        break;
                    else
                        RenderScale(i, group);
            }

            m_parent.InvokeFinishRendering(this, group);

        }

        /// <summary>
        /// Renders all tiles in all groups in all scales
        /// </summary>
        public void Render()
        {
            m_parent.InvokeBeginRendering(this);

            if (!m_parent.Cancel)
                foreach (string s in m_groups)
                    if (m_parent.Cancel)
                        break;
                    else
                        RenderGroup(s);

            m_parent.InvokeFinishRendering(this);
        }

        /// <summary>
        /// Gets or sets the maximum extent used to calculate the tiles
        /// </summary>
        public MaestroAPI.Box2DType MaxExtent
        {
            get
            {
                return m_maxExtent;
            }
            set
            {
                m_maxExtent = value;
                CalculateDimensions();
            }
        }

        /// <summary>
        /// Gets the resourceId for the map
        /// </summary>
        public string ResourceId { get { return m_mapdefinition.ResourceId; } }

        /// <summary>
        /// Gets the MapDefintion
        /// </summary>
        public MaestroAPI.MapDefinition Map { get { return m_mapdefinition; } }

        /// <summary>
        /// Gets a reference to the parent
        /// </summary>
        public BatchSettings Parent { get { return m_parent; } }
    }

    public class TileSettings
    {
        public double MetersPerUnit = 1;
        public double DPI = 96;
        public int TileWidth = 300;
        public int TileHeight = 300;
        public int RetryCount = 5;
        public int DisplayResolutionWidth = 1920;
        public int DisplayResolutionHeight = 1280;

    }
}
