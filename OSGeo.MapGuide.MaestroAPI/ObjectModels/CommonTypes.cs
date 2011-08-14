﻿#region Disclaimer / License
// Copyright (C) 2011, Jackie Ng
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
using System.ComponentModel;
using System.IO;
using OSGeo.MapGuide.MaestroAPI.Schema;

namespace OSGeo.MapGuide.ObjectModels.Common
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2Code", "3.3.0.33572")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class ExtendedDataType : System.ComponentModel.INotifyPropertyChanged
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        private System.Xml.XmlElement[] anyField;

        private static System.Xml.Serialization.XmlSerializer serializer;

        [System.Xml.Serialization.XmlAnyElementAttribute()]
        public System.Xml.XmlElement[] Any
        {
            get
            {
                return this.anyField;
            }
            set
            {
                if ((this.anyField != null))
                {
                    if ((anyField.Equals(value) != true))
                    {
                        this.anyField = value;
                        this.OnPropertyChanged("Any");
                    }
                }
                else
                {
                    this.anyField = value;
                    this.OnPropertyChanged("Any");
                }
            }
        }

        private static System.Xml.Serialization.XmlSerializer Serializer
        {
            get
            {
                if ((serializer == null))
                {
                    serializer = new System.Xml.Serialization.XmlSerializer(typeof(ExtendedDataType));
                }
                return serializer;
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged(string info)
        {
            System.ComponentModel.PropertyChangedEventHandler handler = this.PropertyChanged;
            if ((handler != null))
            {
                handler(this, new System.ComponentModel.PropertyChangedEventArgs(info));
            }
        }

        #region Serialize/Deserialize
        /// <summary>
        /// Serializes current ExtendedDataType object into an XML document
        /// </summary>
        /// <returns>string XML value</returns>
        public virtual string Serialize()
        {
            System.IO.StreamReader streamReader = null;
            System.IO.MemoryStream memoryStream = null;
            try
            {
                memoryStream = new System.IO.MemoryStream();
                Serializer.Serialize(memoryStream, this);
                memoryStream.Seek(0, System.IO.SeekOrigin.Begin);
                streamReader = new System.IO.StreamReader(memoryStream);
                return streamReader.ReadToEnd();
            }
            finally
            {
                if ((streamReader != null))
                {
                    streamReader.Dispose();
                }
                if ((memoryStream != null))
                {
                    memoryStream.Dispose();
                }
            }
        }

        /// <summary>
        /// Deserializes workflow markup into an ExtendedDataType object
        /// </summary>
        /// <param name="xml">string workflow markup to deserialize</param>
        /// <param name="obj">Output ExtendedDataType object</param>
        /// <param name="exception">output Exception value if deserialize failed</param>
        /// <returns>true if this XmlSerializer can deserialize the object; otherwise, false</returns>
        public static bool Deserialize(string xml, out ExtendedDataType obj, out System.Exception exception)
        {
            exception = null;
            obj = default(ExtendedDataType);
            try
            {
                obj = Deserialize(xml);
                return true;
            }
            catch (System.Exception ex)
            {
                exception = ex;
                return false;
            }
        }

        public static bool Deserialize(string xml, out ExtendedDataType obj)
        {
            System.Exception exception = null;
            return Deserialize(xml, out obj, out exception);
        }

        public static ExtendedDataType Deserialize(string xml)
        {
            System.IO.StringReader stringReader = null;
            try
            {
                stringReader = new System.IO.StringReader(xml);
                return ((ExtendedDataType)(Serializer.Deserialize(System.Xml.XmlReader.Create(stringReader))));
            }
            finally
            {
                if ((stringReader != null))
                {
                    stringReader.Dispose();
                }
            }
        }

        /// <summary>
        /// Serializes current ExtendedDataType object into file
        /// </summary>
        /// <param name="fileName">full path of outupt xml file</param>
        /// <param name="exception">output Exception value if failed</param>
        /// <returns>true if can serialize and save into file; otherwise, false</returns>
        public virtual bool SaveToFile(string fileName, out System.Exception exception)
        {
            exception = null;
            try
            {
                SaveToFile(fileName);
                return true;
            }
            catch (System.Exception e)
            {
                exception = e;
                return false;
            }
        }

        public virtual void SaveToFile(string fileName)
        {
            System.IO.StreamWriter streamWriter = null;
            try
            {
                string xmlString = Serialize();
                System.IO.FileInfo xmlFile = new System.IO.FileInfo(fileName);
                streamWriter = xmlFile.CreateText();
                streamWriter.WriteLine(xmlString);
                streamWriter.Close();
            }
            finally
            {
                if ((streamWriter != null))
                {
                    streamWriter.Dispose();
                }
            }
        }

        /// <summary>
        /// Deserializes xml markup from file into an ExtendedDataType object
        /// </summary>
        /// <param name="fileName">string xml file to load and deserialize</param>
        /// <param name="obj">Output ExtendedDataType object</param>
        /// <param name="exception">output Exception value if deserialize failed</param>
        /// <returns>true if this XmlSerializer can deserialize the object; otherwise, false</returns>
        public static bool LoadFromFile(string fileName, out ExtendedDataType obj, out System.Exception exception)
        {
            exception = null;
            obj = default(ExtendedDataType);
            try
            {
                obj = LoadFromFile(fileName);
                return true;
            }
            catch (System.Exception ex)
            {
                exception = ex;
                return false;
            }
        }

        public static bool LoadFromFile(string fileName, out ExtendedDataType obj)
        {
            System.Exception exception = null;
            return LoadFromFile(fileName, out obj, out exception);
        }

        public static ExtendedDataType LoadFromFile(string fileName)
        {
            System.IO.FileStream file = null;
            System.IO.StreamReader sr = null;
            try
            {
                file = new System.IO.FileStream(fileName, FileMode.Open, FileAccess.Read);
                sr = new System.IO.StreamReader(file);
                string xmlString = sr.ReadToEnd();
                sr.Close();
                file.Close();
                return Deserialize(xmlString);
            }
            finally
            {
                if ((file != null))
                {
                    file.Dispose();
                }
                if ((sr != null))
                {
                    sr.Dispose();
                }
            }
        }
        #endregion

        #region Clone method
        /// <summary>
        /// Create a clone of this ExtendedDataType object
        /// </summary>
        public virtual ExtendedDataType Clone()
        {
            return ((ExtendedDataType)(this.MemberwiseClone()));
        }
        #endregion
    }

    public interface IExpressionPropertySource
    {
        /// <summary>
        /// Gets the name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the description
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets the expression data type
        /// </summary>
        ExpressionDataType ExpressionType { get; }
    }

    public enum ExpressionDataType
    {
        Data_Blob,
        Data_Boolean,
        Data_Byte,
        Data_Clob,
        Data_DateTime,
        Data_Double,
        Data_Int16,
        Data_Int32,
        Data_Int64,
        Data_Single,
        Data_String,
        Geometry,
        Raster,
        Association,
        Sym_String,
        Sym_Boolean,
        Sym_Integer,
        Sym_Real,
        Sym_Color,
        Sym_Angle,
        Sym_FillColor,
        Sym_LineColor,
        Sym_LineWeight,
        Sym_Content,
        Sym_Markup,
        Sym_FontName,
        Sym_Bold,
        Sym_Italic,
        Sym_Underlined,
        Sym_Overlined,
        Sym_ObliqueAngle,
        Sym_TrackSpacing,
        Sym_FontHeight,
        Sym_HorizontalAlignment,
        Sym_VerticalAlignment,
        Sym_Justification,
        Sym_LineSpacing,
        Sym_TextColor,
        Sym_GhostColor,
        Sym_FrameLineColor,
        Sym_FrameFillColor,
        Sym_StartOffset,
        Sym_EndOffset,
        Sym_RepeatX,
        Sym_RepeatY
    }

    /// <summary>
    /// Represents a resource id reference
    /// </summary>
    public interface IResourceIdReference : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets the resource id.
        /// </summary>
        /// <value>The resource id.</value>
        string ResourceId { get; set; }
    }
}
