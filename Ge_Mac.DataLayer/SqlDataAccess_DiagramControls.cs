using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using Ge_Mac.LoggingAndExceptions;

namespace Ge_Mac.DataLayer
{
    public partial class SqlDataAccess
    {
        #region cache
        private DiagramControls diagramControlsCache = null;

        public void InvalidateDiagramControls()
        {
            if (diagramControlsCache != null)
                diagramControlsCache.IsValid = false;
        }

        private bool DiagramControlsAreCached()
        {
            bool test = (diagramControlsCache != null);
            if (test)
            {
                test = diagramControlsCache.IsValid;
            }
            return test;
        }
        # endregion

        #region Select Data
        const string allDiagramControlsCommand =
            @"SELECT [DiagramControlID]
                  ,[DiagramPositionID]
                  ,[ControlText]
                  ,[ControlImage]
                  ,[ControlShape]
                  ,[Scale]
                  ,[ScaleX]
                  ,[ScaleY]
                  ,[Width]
                  ,[Height]
                  ,[Rotation]
                  ,[Mirror]
                  ,[DisplayType]
                  ,[HideText]
                  ,[Opacity]
              FROM [dbo].[tblDiagramControls]";

        public DiagramControls GetAllDiagramControls() 
        {
            if (DiagramControlsAreCached())
            {
                return diagramControlsCache;
            }
            try
            {
                const string commandString = allDiagramControlsCommand;                  

                using (SqlCommand command = new SqlCommand(commandString))
                {
                    if (diagramControlsCache == null) diagramControlsCache = new DiagramControls();
                    command.DataFill(diagramControlsCache, SqlDataConnection.DBConnection.JensenGroup);
                    return diagramControlsCache;
                }
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached)
                {
                    ExceptionHandler.Handle(ex);
                    Debugger.Break();
                }

                throw;
            }
        }

        public DiagramControl GetDiagramControl(int diagramControlID)
        {
            DiagramControls diagramControls = GetAllDiagramControls();
            DiagramControl diagramControl = diagramControls.GetById(diagramControlID);

            return diagramControl;
         }
        #endregion

        #region Next Record

        public int NextDiagramControlRecord()
        {
            const string commandString = @"DECLARE	@return_value int
                                            EXEC	@return_value = [dbo].[FirstID]
		                                            @TableName = N'tblDiagramControls',
		                                            @idName = N'DiagramControlID'
                                         SELECT @return_value";

            int nextID = 0;
            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    object spResult = command.ExecuteScalar(SqlDataConnection.DBConnection.JensenGroup);
                    if (spResult != null)
                    {
                        if (spResult.ToString() != string.Empty)
                        {
                            nextID = (int)spResult;
                            nextID++;
                        }
                    }
                }
            }

            catch (SqlException)
            {
                throw;
            }
            return nextID;
        }

        #endregion

        #region Insert Data
        public void InsertNewDiagramControl(DiagramControl diagramControl)
        {
            const string commandString =
                @"INSERT INTO [dbo].[tblDiagramControls]
                       ([DiagramControlID]
                       ,[DiagramPositionID]
                       ,[ControlText]
                       ,[ControlImage]
                       ,[ControlShape]
                       ,[Scale]
                       ,[ScaleX]
                       ,[ScaleY]
                       ,[Width]
                       ,[Height]
                       ,[Rotation]
                       ,[Mirror]
                       ,[DisplayType]
                       ,[HideText]
                       ,[Opacity])
                 VALUES
                       (@DiagramControlID
                       ,@DiagramPositionID
                       ,@ControlText
                       ,@ControlImage
                       ,@ControlShape
                       ,@Scale
                       ,@ScaleX
                       ,@ScaleY
                       ,@Width
                       ,@Height
                       ,@Rotation
                       ,@Mirror
                       ,@DisplayType
                       ,@HideText
                       ,@Opacity)";
            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@DiagramControlID", diagramControl.DiagramControlID);
                    command.Parameters.AddWithValue("@DiagramPositionID", diagramControl.DiagramPositionID);
                    SqlParameter pCT = command.Parameters.Add("@ControlText", SqlDbType.NVarChar);
                    pCT.IsNullable = true;
                    pCT.Value = diagramControl.ControlText == string.Empty ? DBNull.Value 
                        : (object)diagramControl.ControlText;
                    SqlParameter pCI = command.Parameters.Add("@ControlImage", SqlDbType.NVarChar);
                    pCI.IsNullable = true;
                    pCI.Value = diagramControl.ControlImage == string.Empty ? DBNull.Value
                        : (object)diagramControl.ControlImage;
                    SqlParameter pCS = command.Parameters.Add("@ControlShape", SqlDbType.NVarChar);
                    pCS.IsNullable = true;
                    pCS.Value = diagramControl.ControlShape == string.Empty ? DBNull.Value
                        : (object)diagramControl.ControlShape;
                    command.Parameters.AddWithValue("@Scale", diagramControl.Scale);
                    command.Parameters.AddWithValue("@ScaleX", diagramControl.ScaleX);
                    command.Parameters.AddWithValue("@ScaleY", diagramControl.ScaleY);
                    command.Parameters.AddWithValue("@Width", diagramControl.Width);
                    command.Parameters.AddWithValue("@Height", diagramControl.Height);
                    command.Parameters.AddWithValue("@Rotation", diagramControl.Rotation);
                    command.Parameters.AddWithValue("@Mirror", diagramControl.Mirror);
                    command.Parameters.AddWithValue("@DisplayType", diagramControl.DisplayType);
                    command.Parameters.AddWithValue("@HideText", diagramControl.HideText);
                    command.Parameters.AddWithValue("@Opacity", diagramControl.Opacity);

                    try
                    {
                        command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
                        diagramControl.HasChanged = false;
                        InvalidateDiagramControls();
                    }
                    catch (SqlException ex)
                    {
                        const int insertError = 2601;

                        if (ex.Number != insertError)
                        {
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached)
                {
                    ExceptionHandler.Handle(ex);
                    Debugger.Break();
                }

                throw;
            }
        }
        #endregion

        #region Update Data
        public void UpdateDiagramControlDetails(DiagramControl diagramControl)
        {
            const string commandString =
                @"UPDATE [dbo].[tblDiagramControls]
                       SET [DiagramPositionID] = @DiagramPositionID
                          ,[ControlText] = @ControlText
                          ,[ControlImage] = @ControlImage
                          ,[Scale] = @Scale
                          ,[ScaleX] = @ScaleX
                          ,[ScaleY] = @ScaleY
                          ,[Width] = @Width
                          ,[Height] = @Height
                          ,[Rotation] = @Rotation
                          ,[Mirror] = @Mirror
                          ,[DisplayType] = @DisplayType
                          ,[HideText] = @HideText
                          ,[Opacity] = @Opacity
                    WHERE DiagramControlID = @DiagramControlID";

            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@DiagramControlID", diagramControl.DiagramControlID);
                    command.Parameters.AddWithValue("@DiagramPositionID", diagramControl.DiagramPositionID);
                    SqlParameter pCT = command.Parameters.Add("@ControlText", SqlDbType.NVarChar);
                    pCT.IsNullable = true;
                    pCT.Value = diagramControl.ControlText == string.Empty ? DBNull.Value
                        : (object)diagramControl.ControlText;
                    SqlParameter pCI = command.Parameters.Add("@ControlImage", SqlDbType.NVarChar);
                    pCI.IsNullable = true;
                    pCI.Value = diagramControl.ControlImage == string.Empty ? DBNull.Value
                        : (object)diagramControl.ControlImage;
                    command.Parameters.AddWithValue("@Scale", diagramControl.Scale);
                    command.Parameters.AddWithValue("@ScaleX", diagramControl.ScaleX);
                    command.Parameters.AddWithValue("@ScaleY", diagramControl.ScaleY);
                    command.Parameters.AddWithValue("@Width", diagramControl.Width);
                    command.Parameters.AddWithValue("@Height", diagramControl.Height);
                    command.Parameters.AddWithValue("@Rotation", diagramControl.Rotation);
                    command.Parameters.AddWithValue("@Mirror", diagramControl.Mirror);
                    command.Parameters.AddWithValue("@DisplayType", diagramControl.DisplayType);
                    command.Parameters.AddWithValue("@HideText", diagramControl.HideText);
                    command.Parameters.AddWithValue("@Opacity", diagramControl.Opacity);
                    command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
                    diagramControl.HasChanged = false;
                }
                InvalidateDiagramControls();
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached)
                {
                    ExceptionHandler.Handle(ex);
                    Debugger.Break();
                }

                throw;
            }
        }
        #endregion

        #region Delete Data
        public void DeleteDiagramControl(DiagramControl diagramControl)
        {
            const string commandString =
                @"DELETE FROM [dbo].[tblDiagramControls]
                    WHERE DiagramControlID = @DiagramControlID";
            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@DiagramControlID", diagramControl.DiagramControlID);
                    command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
                }
                InvalidateDiagramControls();
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached)
                {
                    ExceptionHandler.Handle(ex);
                    Debugger.Break();
                }

                throw;
            }
        }
        #endregion
    }


    #region Data Collection Class
    public class DiagramControls : DataList<DiagramControl>, IDataFiller
    {

        public int Fill(SqlDataReader dr)
        {
            int DiagramControlIDPos = dr.GetOrdinal("DiagramControlID");
            int DiagramPositionIDPos = dr.GetOrdinal("DiagramPositionID");
            int ControlTextPos = dr.GetOrdinal("ControlText");
            int ControlImagePos = dr.GetOrdinal("ControlImage");
            int ControlShapePos = dr.GetOrdinal("ControlShape");
            int ScalePos = dr.GetOrdinal("Scale");
            int ScaleXPos = dr.GetOrdinal("ScaleX");
            int ScaleYPos = dr.GetOrdinal("ScaleY");
            int WidthPos = dr.GetOrdinal("Width");
            int HeightPos = dr.GetOrdinal("Height");
            int RotationPos = dr.GetOrdinal("Rotation");
            int MirrorPos = dr.GetOrdinal("Mirror");
            int DisplayTypePos = dr.GetOrdinal("DisplayType");
            int HideTextPos = dr.GetOrdinal("HideText");
            int OpacityPos = dr.GetOrdinal("Opacity");

            this.Clear();
            while (dr.Read())
            {
                DiagramControl diagramControl = new DiagramControl()
                {
                    DiagramControlID = dr.GetInt32(DiagramControlIDPos),
                    DiagramPositionID = dr.GetInt32(DiagramPositionIDPos),
                    ControlText = dr.IsDBNull(ControlTextPos) ? string.Empty : dr.GetString(ControlTextPos),
                    ControlImage = dr.IsDBNull(ControlImagePos) ? string.Empty : dr.GetString(ControlImagePos),
                    ControlShape = dr.IsDBNull(ControlShapePos) ? string.Empty : dr.GetString(ControlShapePos),
                    Scale = dr.GetDouble(ScalePos),
                    ScaleX = dr.GetDouble(ScaleXPos),
                    ScaleY = dr.GetDouble(ScaleYPos),
                    Width = dr.GetInt32(WidthPos),
                    Height = dr.GetInt32(HeightPos),
                    Rotation = dr.GetInt32(RotationPos),
                    Mirror = dr.GetBoolean(MirrorPos),
                    DisplayType = dr.GetString(DisplayTypePos),
                    HideText = dr.GetBoolean(HideTextPos),
                    Opacity = dr.GetInt32(OpacityPos),
                    HasChanged = false
                };

                this.Add(diagramControl);
            }
            LastRead = DateTime.Now;
            IsValid = true;

            return this.Count;
        }

        public DiagramControl GetById(int id)
        {
            return this.Find(delegate(DiagramControl diagramControl)
            {
                return diagramControl.DiagramControlID == id;
            });
        }
    }
    #endregion

    #region Item Class
    public class DiagramControl : DataItem
    {
        #region DiagramControl Data Record
        protected class DataRecord : ICopyableObject
        {
            internal int DiagramControlID;
            internal int DiagramPositionID;
            internal string ControlText;
            internal string ControlImage;
            internal string ControlShape;
            internal double Scale;
            internal double ScaleX;
            internal double ScaleY;
            internal int Width;
            internal int Height;
            internal int Rotation;
            internal bool Mirror;
            internal string DisplayType;
            internal bool HideText;
            internal int Opacity;

            public ICopyableObject ShallowCopy()
            {
                return (ICopyableObject)MemberwiseClone();
            }
        }
        #endregion

        #region Constructor
        public DiagramControl()
        {
            ActiveData = (ICopyableObject)new DataRecord();
        }
        #endregion

        #region Abstract Member Variable Properties
        private DataRecord activeData
        {
            get
            {
                return (DataRecord)ActiveData;
            }
            set
            {
                ActiveData = (ICopyableObject)value;
            }
        }

        private DataRecord backupData
        {
            get
            {
                return (DataRecord)BackupData;
            }
            set
            {
                BackupData = (ICopyableObject)value;
            }
        }
        #endregion

        #region Record Status Properties

        /// <summary>This is a new record, ie Not yet created in the database</summary>
        public override bool IsNew
        {
            get
            {
                return this.activeData.DiagramControlID == -1;
            }
        }

        /// <summary>The record exists in the database</summary>
        public override bool IsExisting
        {
            get
            {
                return this.activeData.DiagramControlID != -1;
            }
        }
        #endregion

        #region Data Column Properties
        public int DiagramControlID
        {
            get
            {
                return this.activeData.DiagramControlID;
            }
            set
            {
                this.activeData.DiagramControlID = value;
                HasChanged = true;
            }
        }

        public int DiagramPositionID
        {
            get
            {
                return this.activeData.DiagramPositionID;
            }
            set
            {
                this.activeData.DiagramPositionID = value;
                HasChanged = true;
            }
        }

        public string ControlText
        {
            get
            {
                return this.activeData.ControlText;
            }
            set
            {
                this.activeData.ControlText = value;
                HasChanged = true;
            }
        }

        public string ControlImage
        {
            get
            {
                return this.activeData.ControlImage;
            }
            set
            {
                this.activeData.ControlImage = value;
                HasChanged = true;
            }
        }

        public string ControlShape
        {
            get
            {
                return this.activeData.ControlShape;
            }
            set
            {
                this.activeData.ControlShape = value;
                HasChanged = true;
            }
        }

        public double Scale
        {
            get
            {
                return this.activeData.Scale;
            }
            set
            {
                this.activeData.Scale = value;
                HasChanged = true;
            }
        }

        public double ScaleX
        {
            get
            {
                return this.activeData.ScaleX;
            }
            set
            {
                this.activeData.ScaleX = value;
                HasChanged = true;
            }
        }

        public double ScaleY
        {
            get
            {
                return this.activeData.ScaleY;
            }
            set
            {
                this.activeData.ScaleY = value;
                HasChanged = true;
            }
        }

        public int Width
        {
            get
            {
                return this.activeData.Width;
            }
            set
            {
                this.activeData.Width = value;
                HasChanged = true;
            }
        }

        public int Height
        {
            get
            {
                return this.activeData.Height;
            }
            set
            {
                this.activeData.Height = value;
                HasChanged = true;
            }
        }

        public int Rotation
        {
            get
            {
                return this.activeData.Rotation;
            }
            set
            {
                this.activeData.Rotation = value;
                HasChanged = true;
            }
        }

        public bool Mirror
        {
            get
            {
                return this.activeData.Mirror;
            }
            set
            {
                this.activeData.Mirror = value;
                HasChanged = true;
            }
        }

        public string DisplayType
        {
            get
            {
                return this.activeData.DisplayType;
            }
            set
            {
                this.activeData.DisplayType = value;
                HasChanged = true;
            }
        }

        public bool HideText
        {
            get
            {
                return this.activeData.HideText;
            }
            set
            {
                this.activeData.HideText = value;
                HasChanged = true;
            }
        }

        public int Opacity
        {
            get
            {
                return this.activeData.Opacity;
            }
            set
            {
                this.activeData.Opacity = value;
                HasChanged = true;
            }
        }

        #endregion
    }
    #endregion
}
