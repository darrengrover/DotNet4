using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using Ge_Mac.LoggingAndExceptions;

namespace Ge_Mac.DataLayer
{
    public partial class SqlDataAccess
    {
        #region caches
        private VisioPositions visioPositionsCache = null;
        private VisioFavourites visioFavouritesCache = null;
        private VisioDiagrams visioDiagramsCache = null;
        private VisioTexts visioTextsCache = null;
        private VisioBitmaps visioBitmapsCache = null;
        private VisioDiagramBitmaps visioDiagramBitmapsCache = null;
        private VisioMachines visioMachinesCache = null;
        private VisioStations visioStationsCache = null;
        private VisioShapes visioShapesCache = null;

        private bool VisioDiagramsAreCached()
        {
            bool test = (visioDiagramsCache != null);
            if (test)
            {
                test = visioDiagramsCache.IsValid;
            }
            return test;
        }

        private bool VisioBitmapsAreCached()
        {
            bool test = (visioBitmapsCache != null);
            if (test)
            {
                test = visioBitmapsCache.IsValid;
            }
            return test;
        }

        private bool VisioDiagramBitmapsAreCached()
        {
            bool test = (visioDiagramBitmapsCache != null);
            if (test)
            {
                test = visioDiagramBitmapsCache.IsValid;
            }
            return test;
        }

        public bool VisioTextsAreCached()
        {
            bool test = (visioTextsCache != null);
            if (test)
            {
                test = visioTextsCache.IsValid;
            }
            return test;
        }

        private bool VisioPositionsAreCached()
        {
            bool test = (visioPositionsCache != null);
            if (test)
            {
                test = visioPositionsCache.IsValid;
            }
            return test;
        }

        private bool VisioFavouritesAreCached()
        {
            bool test = (visioFavouritesCache != null);
            if (test)
            {
                test = visioFavouritesCache.IsValid;
            }
            return test;
        }

        private bool VisioMachinesAreCached()
        {
            bool test = (visioMachinesCache != null);
            if (test)
            {
                test = visioMachinesCache.IsValid;
            }
            return test;
        }

        private bool VisioStationsAreCached()
        {
            bool test = (visioStationsCache != null);
            if (test)
            {
                test = visioStationsCache.IsValid;
            } 
            return test;
        }


        # endregion

        #region Select Data

        #region visioDiagrams

        public VisioDiagrams GetAllVisioDiagrams()
        {
            return GetAllVisioDiagrams(false);
        }

        public VisioDiagrams GetAllVisioDiagrams(bool noCache)
        {
            if (!noCache && VisioDiagramsAreCached())
            {
                return visioDiagramsCache;
            }
            try
            {
                const string commandString = @"SELECT [DiagramID]
                                                      ,[Title]
                                                      ,[Description]
                                                      ,[Width]
                                                      ,[Height]
                                                  FROM [dbo].[tblVisioDiagrams]
                                                    ORDER BY DiagramID ";

                using (SqlCommand command = new SqlCommand(commandString))
                {
                    if (visioDiagramsCache == null) visioDiagramsCache = new VisioDiagrams();
                    command.DataFill(visioDiagramsCache, SqlDataConnection.DBConnection.JensenGroup);
                    return visioDiagramsCache;
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

        #region visioBitmaps

        private bool readWriteVisioBitmaps = true;

        public bool ReadWriteVisioBitmaps
        {
            get { return readWriteVisioBitmaps; }
            set { readWriteVisioBitmaps = value; }
        }

        public VisioBitmaps GetAllVisioBitmaps()
        {
            return GetAllVisioBitmaps(false);
        }

        public VisioBitmaps GetAllVisioBitmaps(bool noCache)
        {
            if (!noCache && VisioBitmapsAreCached())
            {
                return visioBitmapsCache;
            }
            try
            {
                string commandString = @"SELECT [BitmapID]
                                            ,[Description]
                                            ,null as Bitmap
                                        FROM [JEGR_DB].[dbo].[tblVisioBitmaps]
                                        ORDER BY BitmapID ";
                if (readWriteVisioBitmaps)
                    commandString = @"SELECT [BitmapID]
                                            ,[Description]
                                            ,[Bitmap]
                                        FROM [JEGR_DB].[dbo].[tblVisioBitmaps]
                                        ORDER BY BitmapID ";

                using (SqlCommand command = new SqlCommand(commandString))
                {
                    if (visioBitmapsCache == null) visioBitmapsCache = new VisioBitmaps();
                    command.DataFill(visioBitmapsCache, SqlDataConnection.DBConnection.JensenGroup);
                    return visioBitmapsCache;
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

        #region visioDiagramBitmaps

        public VisioDiagramBitmaps GetAllVisioDiagramBitmaps()
        {
            return GetAllVisioDiagramBitmaps(false);
        }

        public VisioDiagramBitmaps GetAllVisioDiagramBitmaps(bool noCache)
        {
            if (!noCache && VisioDiagramBitmapsAreCached())
            {
                return visioDiagramBitmapsCache;
            }
            try
            {
                const string commandString = @"SELECT [DiagBmpID]
                                                      ,[BitmapID]
                                                      ,[DiagramID]
                                                      ,[ZIndex]
                                                  FROM [JEGR_DB].[dbo].[tblVisioDiagramBitmaps]
                                                  ORDER BY DiagBmpID ";

                using (SqlCommand command = new SqlCommand(commandString))
                {
                    if (visioDiagramBitmapsCache == null) visioDiagramBitmapsCache = new VisioDiagramBitmaps();
                    command.DataFill(visioDiagramBitmapsCache, SqlDataConnection.DBConnection.JensenGroup);
                    return visioDiagramBitmapsCache;
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

        #region visioTexts

        public VisioTexts GetAllVisioTexts()
        {
            return GetAllVisioTexts(false);
        }

        public VisioTexts GetAllVisioTexts(bool noCache)
        {
            if (!noCache && VisioTextsAreCached())
            {
                return visioTextsCache;
            }
            try
            {
                const string commandString = @"SELECT vt.[VisioTextID]
                                                      ,vt.[VisioPositionID]
                                                      ,vt.[IsDynamic]
                                                      ,vt.[Enabled]
                                                      ,vt.[RefreshRate]
                                                      ,vt.[StaticText]
                                                      ,vt.[SQLCommand]
                                                      ,vp.[DiagramID]
                                                      ,vp.[X]
                                                      ,vp.[Y]
                                                      ,vp.[Width]
                                                      ,vp.[Height]
                                                      ,vp.[Scale]
                                                      ,vp.[Rotation]
                                                      ,vp.[Mirror]
                                                 FROM [dbo].[tblVisioText] vt, dbo.tblVisioPositions vp
                                                  where vt.visiopositionid=vp.VisioPositionID
                                                 ORDER BY vp.DiagramID, vt.VisioTextID";

                using (SqlCommand command = new SqlCommand(commandString))
                {
                    if (visioTextsCache == null) visioTextsCache = new VisioTexts();
                    command.DataFill(visioTextsCache, SqlDataConnection.DBConnection.JensenGroup);
                    return visioTextsCache;
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

        #region visioPositions
        const string allVisioPositionsCommand_old =
            @"SELECT [VisioPositionID]
                  ,[DiagramID]
                  ,[X]
                  ,[Y]
              FROM [dbo].[tblVisioPositions]";

        const string allVisioPositionsCommand =
            @"SELECT [VisioPositionID]
                  ,[DiagramID]
                  ,[X]
                  ,[Y]
                  ,[Width]
                  ,[Height]
                  ,[Scale]
                  ,[Rotation]
                  ,[Mirror]
              FROM [dbo].[tblVisioPositions]";

        public VisioPositions GetAllVisioPositions()
        {
            return GetAllVisioPositions(false);
        }

        public VisioPositions GetAllVisioPositions(bool noCache) 
        {
            if (!noCache && VisioPositionsAreCached())
            {
                return visioPositionsCache;
            }
            try
            {
                string commandString= allVisioPositionsCommand_old;
                if (DatabaseVersion >= 1.7)
                    commandString = allVisioPositionsCommand;                              

                using (SqlCommand command = new SqlCommand(commandString))
                {
                    if (visioPositionsCache == null) visioPositionsCache = new VisioPositions();
                    command.DataFill(visioPositionsCache, SqlDataConnection.DBConnection.JensenGroup);
                    return visioPositionsCache;
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

        public VisioPosition GetDiagramPosition(int diagramPositionID)
        {
            VisioPositions diagramPositions = GetAllVisioPositions();
            VisioPosition diagramPosition = diagramPositions.GetById(diagramPositionID);

            return diagramPosition;
        }
        #endregion

        #region visioFavourites
        const string allVisioFavouritesCommand =
            @"SELECT [FavouriteID]
                    ,[DiagramID]
                    ,[AppID]
                    ,[AppUserID]
                    ,[FavouriteName]
                    ,[XMin]
                    ,[YMin]
                    ,[XMax]
                    ,[YMax]
                FROM [dbo].[tblVisioFavourites]";

        public VisioFavourites GetAllVisioFavourites()
        {
            return GetAllVisioFavourites(false);
        }

        public VisioFavourites GetAllVisioFavourites(bool noCache)
        {
            if (!noCache && VisioFavouritesAreCached())
            {
                return visioFavouritesCache;
            }
            try
            {
                const string commandString = allVisioFavouritesCommand;
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    if (visioFavouritesCache == null) visioFavouritesCache = new VisioFavourites();
                    command.DataFill(visioFavouritesCache, SqlDataConnection.DBConnection.JensenGroup);
                    return visioFavouritesCache;
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

        #region visioshapes
        public VisioShapes GetVisioShapes()
        {
            if (visioShapesCache == null)
            {
                visioShapesCache = new VisioShapes();
                VisioShape visioShape = new VisioShape();
                visioShape.ShapeID = 0;
                visioShape.ShapeName = "Circle";
                visioShapesCache.Add(visioShape);
                visioShape = new VisioShape();
                visioShape.ShapeID = 1;
                visioShape.ShapeName = "Rectangle";
                visioShapesCache.Add(visioShape);
            }
            return visioShapesCache;
        }

        #endregion
 
        #region visioMachines

        public VisioMachines GetAllVisioMachines()
        {
            return GetAllVisioMachines(false);
        }

        public VisioMachines GetAllVisioMachines(bool noCache)
        {
            if (!noCache && VisioMachinesAreCached())
            {
                return visioMachinesCache;
            }
            try
            {
                string commandString = @"SELECT vm.[VisioMachineID]
                                                  ,vm.[VisioPositionID]
                                                  ,vm.[MachineID]
                                                  ,vm.[MachineShape]
                                                  ,vp.[DiagramID]
                                                  ,vp.[X]
                                                  ,vp.[Y]
                                                  ,vp.[Width]
                                                  ,vp.[Height]
                                                  ,vp.[Scale]
                                                  ,vp.[Rotation]
                                                  ,vp.[Mirror]
                                             FROM [dbo].[tblVisioMachines] vm, dbo.tblVisioPositions vp
                                              where vm.visiopositionid=vp.VisioPositionID
                                                 ORDER BY vp.DiagramID, vm.VisioMachineID";
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    if (visioMachinesCache == null) visioMachinesCache = new VisioMachines();
                    command.DataFill(visioMachinesCache, SqlDataConnection.DBConnection.JensenGroup);
                    return visioMachinesCache;
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

        #region visioStations

        public VisioStations GetAllVisioStations()
        {
            return GetAllVisioStations(false);
        }

        public VisioStations GetAllVisioStations(bool noCache)
        {
            if (!noCache && VisioStationsAreCached())
            {
                return visioStationsCache;
            }
            try
            {
                string commandString = @"SELECT vs.StationID
                                                  ,vs.[VisioPositionID]
                                                  ,vs.[MachineID]
                                                  ,vs.SubID
                                                  ,vp.[DiagramID]
                                                  ,vp.[X]
                                                  ,vp.[Y]
                                                  ,vp.[Width]
                                                  ,vp.[Height]
                                                  ,vp.[Scale]
                                                  ,vp.[Rotation]
                                                  ,vp.[Mirror]
                                             FROM [dbo].[tblVisioStations] vs, dbo.tblVisioPositions vp
                                              where vs.visiopositionid=vp.VisioPositionID
                                                 ORDER BY vp.DiagramID, vs.StationID";
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    if (visioStationsCache == null) visioStationsCache = new VisioStations();
                    command.DataFill(visioStationsCache, SqlDataConnection.DBConnection.JensenGroup);
                    return visioStationsCache;
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

        #endregion

        #region Next Record

        public int NextRecord(string aTable, string aField)
        {
            string commandString = "DECLARE @return_value int " +
                "EXEC @return_value = [dbo].[FirstID] "+
		                "@TableName = N'" + aTable +
                        "', @idName = N'" + aField + 
                        "' SELECT @return_value";
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

        public int NextVisioPositionRecord()
        {
            return NextRecord("tblVisioPositions", "VisioPositionID");
        }

        public int NextVisioDiagramRecord()
        {
            return NextRecord("tblVisioDiagrams", "DiagramID");
        }

        public int NextVisioDiagBmpRecord()
        {
            return NextRecord("tblVisioDiagramBitmaps", "DiagBmpID");
        }

        public int NextVisioBitmapRecord()
        {
            return NextRecord("tblVisioBitmaps", "BitmapID");
        }

        public int NextVisioTextRecord()
        {
            return NextRecord("tblVisioText", "VisioTextID");
        }

        public int NextVisioFavouriteRecord()
        {
            return NextRecord("tblVisioFavourites", "FavouriteID");
        }

        public int NextVisioMachineRecord()
        {
            return NextRecord("tblVisioMachines", "VisioMachineID");
        }

        public int NextVisioStationRecord()
        {
            return NextRecord("tblVisioStations", "StationID");
        }

        #endregion

        #region Insert Data
        public void InsertNewVisioPosition(VisioPosition visioPosition)
        {
            string commandString =
                @"INSERT INTO [dbo].[tblVisioPositions]
                       ([VisioPositionID]
                       ,[DiagramID]
                       ,[X]
                       ,[Y])
                 VALUES
                       (@VisioPositionID
                       ,@DiagramID
                       ,@X
                       ,@Y)";
            if (DatabaseVersion >= 1.7)
                commandString = @"INSERT INTO [dbo].[tblVisioPositions]
                                                ([VisioPositionID]
                                                ,[DiagramID]
                                                ,[X]
                                                ,[Y]
                                                ,[Width]
                                                ,[Height]
                                                ,[Scale]
                                                ,[Rotation]
                                                ,[Mirror])
                                            VALUES
                                                (@VisioPositionID
                                                ,@DiagramID
                                                ,@X
                                                ,@Y
                                                ,@Width
                                                ,@Height
                                                ,@Scale
                                                ,@Rotation
                                                ,@Mirror)";
            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@VisioPositionID", visioPosition.VisioPositionID);
                    command.Parameters.AddWithValue("@DiagramID", visioPosition.DiagramID);
                    if (DatabaseVersion < 1.7)
                    {
                        command.Parameters.AddWithValue("@X", visioPosition.IntX);
                        command.Parameters.AddWithValue("@Y", visioPosition.IntY);
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@X", visioPosition.X);
                        command.Parameters.AddWithValue("@Y", visioPosition.Y);
                        command.Parameters.AddWithValue("@Width", visioPosition.Width);
                        command.Parameters.AddWithValue("@Height", visioPosition.Height);
                        command.Parameters.AddWithValue("@Scale", visioPosition.Scale);
                        command.Parameters.AddWithValue("@Rotation", visioPosition.Rotation);
                        command.Parameters.AddWithValue("@Mirror", visioPosition.Mirror);
                    }
                    try
                    {
                        command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
                        visioPosition.HasChanged = false;
                        visioPosition.ForceNew = false;
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

        public void InsertNewVisioDiagram(VisioDiagram visioDiagram)
        {
            const string commandString =
                @"INSERT INTO [dbo].[tblVisioDiagrams]
                       ([DiagramID]
                       ,[Title]
                       ,[Description]
                       --,[DiagramBitmap]
                       --,[DiagramThumb]
                       ,[Width]
                       ,[Height])
                 VALUES
                       (@DiagramID
                       ,@Title
                       ,@Description
                       --,@DiagramBitmap
                       --,@DiagramThumb
                       ,@Width
                       ,@Height)";
            try
            {
                if (visioDiagram.DiagramID < 0)
                    visioDiagram.DiagramID = NextVisioDiagramRecord();
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@DiagramID", visioDiagram.DiagramID);
                    command.Parameters.AddWithValue("@Title", visioDiagram.Title);
                    command.Parameters.AddWithValue("@Description", visioDiagram.Description);
                    command.Parameters.AddWithValue("@Width", visioDiagram.Width);
                    command.Parameters.AddWithValue("@Height", visioDiagram.Height);

                    try
                    {
                        command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
                        visioDiagram.HasChanged = false;
                        visioDiagram.ForceNew = false;
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

        public void InsertNewVisioBitmap(VisioBitmap visioBitmap)
        {
            if (readWriteVisioBitmaps && (visioBitmap.DiagBmpBytes!=null))
            {
                const string commandString = @"INSERT INTO [JEGR_DB].[dbo].[tblVisioBitmaps]
                                               ([BitmapID]
                                               ,[Description]
                                               ,[Bitmap])
                                         VALUES
                                               (@BitmapID
                                               ,@Description
                                               ,@Bitmap)";
                try
                {
                    if (visioBitmap.BitmapID < 0)
                        visioBitmap.BitmapID = NextVisioBitmapRecord();
                    using (SqlCommand command = new SqlCommand(commandString))
                    {
                        command.Parameters.AddWithValue("@BitmapID", visioBitmap.BitmapID);
                        command.Parameters.AddWithValue("@Description", visioBitmap.Description);
                        command.Parameters.AddWithValue("@Bitmap", visioBitmap.DiagBmpBytes);


                        try
                        {
                            command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
                            visioBitmap.HasChanged = false;
                            visioBitmap.ForceNew = false;
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
        }

        public void InsertNewVisioDiagramBitmap(VisioDiagramBitmap visioDiagramBitmap)
        {
            const string commandString = @"INSERT INTO [JEGR_DB].[dbo].[tblVisioDiagramBitmaps]
                                               ([DiagBmpID]
                                               ,[BitmapID]
                                               ,[DiagramID]
                                               ,[ZIndex])
                                         VALUES
                                               (@DiagBmpID
                                               ,@BitmapID
                                               ,@DiagramID
                                               ,@ZIndex)";
            try
            {
                if (visioDiagramBitmap.DiagBmpID < 0)
                    visioDiagramBitmap.DiagBmpID = NextVisioDiagBmpRecord();
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@DiagBmpID", visioDiagramBitmap.DiagBmpID);
                    command.Parameters.AddWithValue("@BitmapID", visioDiagramBitmap.BitmapID);
                    command.Parameters.AddWithValue("@DiagramID", visioDiagramBitmap.DiagramID);
                    command.Parameters.AddWithValue("@ZIndex", visioDiagramBitmap.ZIndex);

                    try
                    {
                        command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
                        visioDiagramBitmap.HasChanged = false;
                        visioDiagramBitmap.ForceNew = false;
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

        public void InsertNewVisioFavourite(VisioFavourite visioFavourite)
        {
            const string commandString = @"INSERT INTO [dbo].[tblVisioFavourites]
                                                            ([FavouriteID]
                                                            ,[DiagramID]
                                                            ,[AppID]
                                                            ,[AppUserID]
                                                            ,[FavouriteName]
                                                            ,[XMin]
                                                            ,[YMin]
                                                            ,[XMax]
                                                            ,[YMax])
                                                        VALUES
                                                            (@FavouriteID
                                                            ,@DiagramID
                                                            ,@AppID
                                                            ,@AppUserID
                                                            ,@FavouriteName
                                                            ,@XMin
                                                            ,@YMin
                                                            ,@XMax
                                                            ,@YMax)";
            try
            {
                if (visioFavourite.FavouriteID < 0)
                    visioFavourite.FavouriteID = NextVisioFavouriteRecord();
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@FavouriteID", visioFavourite.FavouriteID);
                    command.Parameters.AddWithValue("@DiagramID", visioFavourite.DiagramID);
                    command.Parameters.AddWithValue("@AppID", visioFavourite.AppID);
                    command.Parameters.AddWithValue("@AppUserID", visioFavourite.AppUserID);
                    command.Parameters.AddWithValue("@FavouriteName", visioFavourite.FavouriteName);
                    command.Parameters.AddWithValue("@XMin", visioFavourite.XMin);
                    command.Parameters.AddWithValue("@YMin", visioFavourite.YMin);
                    command.Parameters.AddWithValue("@XMax", visioFavourite.XMax);
                    command.Parameters.AddWithValue("@YMax", visioFavourite.YMax);

                    try
                    {
                        command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
                        visioFavourite.HasChanged = false;
                        visioFavourite.ForceNew = false;
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

        public void InsertNewVisioMachine(VisioMachine visioMachine)
        {
            visioMachine.VisioPositionID = this.NextVisioPositionRecord();
            VisioPosition vP = new VisioPosition();
            vP.VisioPositionID = visioMachine.VisioPositionID;
            vP.DiagramID = visioMachine.DiagramID;
            vP.X = visioMachine.X;
            vP.Y = visioMachine.Y;
            vP.Width = visioMachine.Width;
            vP.Height = visioMachine.Height;
            vP.Scale = visioMachine.Scale;
            vP.Rotation = visioMachine.Rotation;
            vP.Mirror = visioMachine.Mirror;
            InsertNewVisioPosition(vP);
            const string commandString = @"INSERT INTO [dbo].[tblVisioMachines]
                                                           ([VisioMachineID]
                                                           ,[VisioPositionID]
                                                           ,[MachineID]
                                                           ,[MachineShape])
                                                     VALUES
                                                           (@VisioMachineID
                                                           ,@VisioPositionID
                                                           ,@MachineID
                                                           ,@MachineShape)";
            try
            {
                if (visioMachine.VisioMachineID < 0)
                    visioMachine.VisioMachineID = NextVisioMachineRecord();
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@VisioMachineID", visioMachine.VisioMachineID);
                    command.Parameters.AddWithValue("@VisioPositionID", visioMachine.VisioPositionID);
                    command.Parameters.AddWithValue("@MachineID", visioMachine.MachineID);
                    command.Parameters.AddWithValue("@MachineShape", visioMachine.MachineShape);


                    try
                    {
                        command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
                        visioMachine.HasChanged = false;
                        visioMachine.ForceNew = false;
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

        public void InsertNewVisioText(VisioText visioText)
        {
            visioText.VisioPositionID = this.NextVisioPositionRecord();
            VisioPosition vP = new VisioPosition();
            vP.VisioPositionID = visioText.VisioPositionID;
            vP.DiagramID = visioText.DiagramID;
            vP.X = visioText.X;
            vP.Y = visioText.Y;
            vP.Width = visioText.Width;
            vP.Height = visioText.Height;
            vP.Scale = visioText.Scale;
            vP.Rotation = visioText.Rotation;
            vP.Mirror = visioText.Mirror;
            InsertNewVisioPosition(vP);
            const string commandString = @"INSERT INTO [dbo].[tblVisioText]
                                               ([VisioTextID]
                                               ,[VisioPositionID]
                                               ,[IsDynamic]
                                               ,[Enabled]
                                               ,[RefreshRate]
                                               ,[StaticText]
                                               ,[SQLCommand])
                                         VALUES
                                               (@VisioTextID
                                               ,@VisioPositionID
                                               ,@IsDynamic
                                               ,@Enabled
                                               ,@RefreshRate
                                               ,@StaticText
                                               ,@SQLCommand)";
            try
            {
                if (visioText.VisioTextID < 0)
                    visioText.VisioTextID = NextVisioTextRecord();
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@VisioTextID", visioText.VisioTextID);
                    command.Parameters.AddWithValue("@VisioPositionID", visioText.VisioPositionID);
                    command.Parameters.AddWithValue("@IsDynamic", visioText.IsDynamic);
                    command.Parameters.AddWithValue("@Enabled", visioText.Enabled);
                    command.Parameters.AddWithValue("@RefreshRate", visioText.RefreshRate);
                    command.Parameters.AddWithValue("@StaticText", visioText.StaticText);
                    command.Parameters.AddWithValue("@SQLCommand", visioText.SQLCommand);

                    try
                    {
                        command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
                        visioText.HasChanged = false;
                        visioText.ForceNew = false;
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

        public void InsertNewVisioStation(VisioStation visioStation)
        {
            visioStation.VisioPositionID = this.NextVisioPositionRecord();
            VisioPosition vP = new VisioPosition();
            vP.VisioPositionID = visioStation.VisioPositionID;
            vP.DiagramID = visioStation.DiagramID;
            vP.X = visioStation.X;
            vP.Y = visioStation.Y;
            vP.Width = visioStation.Width;
            vP.Height = visioStation.Height;
            vP.Scale = visioStation.Scale;
            vP.Rotation = visioStation.Rotation;
            vP.Mirror = visioStation.Mirror;
            InsertNewVisioPosition(vP);
            const string commandString = @"INSERT INTO [dbo].[tblVisioStations]
                                                           ([StationID]
                                                           ,[VisioPositionID]
                                                           ,[MachineID]
                                                           ,[SubID])
                                                     VALUES
                                                           (@StationID
                                                           ,@VisioPositionID
                                                           ,@MachineID
                                                           ,@SubID)";
            try
            {
                if (visioStation.StationID < 0)
                    visioStation.StationID = NextVisioStationRecord();
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@StationID", visioStation.StationID);
                    command.Parameters.AddWithValue("@VisioPositionID", visioStation.VisioPositionID);
                    command.Parameters.AddWithValue("@MachineID", visioStation.MachineID);
                    command.Parameters.AddWithValue("@SubID", visioStation.SubID);


                    try
                    {
                        command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
                        visioStation.HasChanged = false;
                        visioStation.ForceNew = false;
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
 
        public void UpdateVisioPosition(VisioPosition visioPosition)
        {
            string commandString =
                @"UPDATE [dbo].[tblVisioPositions]
                   SET [VisioPositionID] = @VisioPositionID
                      ,[DiagramID]= @DiagramID
                      ,[X] = @X
                      ,[Y] = @Y
                 WHERE [VisioPositionID] = @VisioPositionID";
            if (DatabaseVersion >= 1.7)
                commandString = @"UPDATE [dbo].[tblVisioPositions]
                                       SET [DiagramID] = @DiagramID
                                          ,[X] = @X
                                          ,[Y] = @Y
                                          ,[Width] = @Width
                                          ,[Height] = @Height
                                          ,[Scale] = @Scale
                                          ,[Rotation] = @Rotation
                                          ,[Mirror] = @Mirror
                                     WHERE [VisioPositionID] = @VisioPositionID";

            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@VisioPositionID", visioPosition.VisioPositionID);
                    command.Parameters.AddWithValue("@DiagramID", visioPosition.DiagramID);
                    if (DatabaseVersion < 1.7)
                    {
                        command.Parameters.AddWithValue("@X", visioPosition.IntX);
                        command.Parameters.AddWithValue("@Y", visioPosition.IntY);
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@X", visioPosition.X);
                        command.Parameters.AddWithValue("@Y", visioPosition.Y);
                        command.Parameters.AddWithValue("@Width", visioPosition.Width);
                        command.Parameters.AddWithValue("@Height", visioPosition.Height);
                        command.Parameters.AddWithValue("@Scale", visioPosition.Scale);
                        command.Parameters.AddWithValue("@Rotation", visioPosition.Rotation);
                        command.Parameters.AddWithValue("@Mirror", visioPosition.Mirror);
                    }

                    command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
                    visioPosition.HasChanged = false;
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

        public void UpdateVisioDiagram(VisioDiagram visioDiagram)
        {
            const string commandString =
                            @"UPDATE [dbo].[tblVisioDiagrams]
                               SET [Title] = @Title
                                  ,[Description] = @Description
                                  --,[DiagramBitmap] = @DiagramBitmap
                                  --,[DiagramThumb] = @DiagramThumb
                                  ,[Width] = @Width
                                  ,[Height] = @Height
                              WHERE [DiagramID] = @DiagramID";
            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@DiagramID", visioDiagram.DiagramID);
                    command.Parameters.AddWithValue("@Title", visioDiagram.Title);
                    command.Parameters.AddWithValue("@Description", visioDiagram.Description);
                    command.Parameters.AddWithValue("@Width", visioDiagram.Width);
                    command.Parameters.AddWithValue("@Height", visioDiagram.Height);

                    command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
                    visioDiagram.HasChanged = false;
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

        public void UpdateVisioFavourite(VisioFavourite visioFavourite)
        {
            const string commandString =
                            @"UPDATE [dbo].[tblVisioFavourites]
                                       SET [DiagramID] = @DiagramID
                                          ,[AppID] = @AppID
                                          ,[AppUserID] = @AppUserID
                                          ,[FavouriteName] = @FavouriteName
                                          ,[XMin] = @XMin
                                          ,[YMin] = @YMin
                                          ,[XMax] = @XMax
                                          ,[YMax] = @YMax
                                     WHERE [FavouriteID] = @FavouriteID";
            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@FavouriteID", visioFavourite.FavouriteID);
                    command.Parameters.AddWithValue("@DiagramID", visioFavourite.DiagramID);
                    command.Parameters.AddWithValue("@AppID", visioFavourite.AppID);
                    command.Parameters.AddWithValue("@AppUserID", visioFavourite.AppUserID);
                    command.Parameters.AddWithValue("@FavouriteName", visioFavourite.FavouriteName);
                    command.Parameters.AddWithValue("@XMin", visioFavourite.XMin);
                    command.Parameters.AddWithValue("@YMin", visioFavourite.YMin);
                    command.Parameters.AddWithValue("@XMax", visioFavourite.XMax);
                    command.Parameters.AddWithValue("@YMax", visioFavourite.YMax);

                    command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
                    visioFavourite.HasChanged = false;
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

        public void UpdateVisioMachine(VisioMachine visioMachine)
        {
            VisioPositions vPs = this.GetAllVisioPositions();
            if (vPs != null)
            {
                VisioPosition vP = vPs.GetById(visioMachine.VisioPositionID);
                if (vP != null)
                {
                    vP.DiagramID = visioMachine.DiagramID;
                    vP.X = visioMachine.X;
                    vP.Y = visioMachine.Y;
                    vP.Width = visioMachine.Width;
                    vP.Height = visioMachine.Height;
                    vP.Rotation = visioMachine.Rotation;
                    vP.Mirror = visioMachine.Mirror;
                    UpdateVisioPosition(vP);
                }
                try
                {
                    const string commandString =
                                   @"UPDATE [dbo].[tblVisioMachines]
                                               SET [VisioPositionID] = @VisioPositionID
                                                  ,[MachineID] = @MachineID
                                                  ,[MachineShape] = @MachineShape
                                             WHERE [VisioMachineID] = @VisioMachineID";
                    using (SqlCommand command = new SqlCommand(commandString))
                    {
                        command.Parameters.AddWithValue("@VisioPositionID", visioMachine.VisioPositionID);
                        command.Parameters.AddWithValue("@MachineID", visioMachine.MachineID);
                        command.Parameters.AddWithValue("@MachineShape", visioMachine.MachineShape);
                        command.Parameters.AddWithValue("@VisioMachineID", visioMachine.VisioMachineID);

                        command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
                        visioMachine.HasChanged = false;
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
        }

        public void UpdateVisioText(VisioText visioText)
        {
            VisioPositions vPs = this.GetAllVisioPositions();
            if (vPs != null)
            {
                VisioPosition vP = vPs.GetById(visioText.VisioPositionID);
                if (vP != null)
                {
                    vP.DiagramID = visioText.DiagramID;
                    vP.X = visioText.X;
                    vP.Y = visioText.Y;
                    vP.Width = visioText.Width;
                    vP.Height = visioText.Height;
                    vP.Rotation = visioText.Rotation;
                    vP.Mirror = visioText.Mirror;
                    UpdateVisioPosition(vP);
                }
                try
                {
                    const string commandString =
                                   @"UPDATE [dbo].[tblVisioText]
                                               SET [VisioPositionID] = @VisioPositionID
                                                  ,[IsDynamic] = @IsDynamic
                                                  ,[Enabled] = @Enabled
                                                  ,[RefreshRate] = @RefreshRate
                                                  ,[StaticText] = @StaticText
                                                  ,[SQLCommand] = @SQLCommand
                                             WHERE [VisioTextID] = @VisioTextID";
                    using (SqlCommand command = new SqlCommand(commandString))
                    {
                        command.Parameters.AddWithValue("@VisioTextID", visioText.VisioTextID);
                        command.Parameters.AddWithValue("@VisioPositionID", visioText.VisioPositionID);
                        command.Parameters.AddWithValue("@IsDynamic", visioText.IsDynamic);
                        command.Parameters.AddWithValue("@Enabled", visioText.Enabled);
                        command.Parameters.AddWithValue("@RefreshRate", visioText.RefreshRate);
                        command.Parameters.AddWithValue("@StaticText", visioText.StaticText);
                        command.Parameters.AddWithValue("@SQLCommand", visioText.SQLCommand);

                        command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
                        visioText.HasChanged = false;
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
        }

        public void UpdateVisioStation(VisioStation visioStation)
        {
            VisioPositions vPs = this.GetAllVisioPositions();
            if (vPs != null)
            {
                VisioPosition vP = vPs.GetById(visioStation.VisioPositionID);
                if (vP != null)
                {
                    vP.DiagramID = visioStation.DiagramID;
                    vP.X = visioStation.X;
                    vP.Y = visioStation.Y;
                    vP.Width = visioStation.Width;
                    vP.Height = visioStation.Height;
                    vP.Rotation = visioStation.Rotation;
                    vP.Mirror = visioStation.Mirror;
                    UpdateVisioPosition(vP);
                }
                try
                {
                    const string commandString =
                                   @"UPDATE [dbo].[tblVisioStations]
                                               SET [VisioPositionID] = @VisioPositionID
                                                  ,[MachineID] = @MachineID
                                                  ,[SubID] = @SubID
                                             WHERE [StationID] = @StationID";
                    using (SqlCommand command = new SqlCommand(commandString))
                    {
                        command.Parameters.AddWithValue("@VisioPositionID", visioStation.VisioPositionID);
                        command.Parameters.AddWithValue("@MachineID", visioStation.MachineID);
                        command.Parameters.AddWithValue("@SubID", visioStation.SubID);
                        command.Parameters.AddWithValue("@StationID", visioStation.StationID);

                        command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
                        visioStation.HasChanged = false;
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
        }

        public void UpdateVisioBitmap(VisioBitmap visioBitmap)
        {
            if (readWriteVisioBitmaps && (visioBitmap.DiagBmpBytes != null))
            {
                const string commandString =
                                @"UPDATE [JEGR_DB].[dbo].[tblVisioBitmaps]
                                   SET [Description] = @Description
                                      ,[Bitmap] = @Bitmap
                                 WHERE [BitmapID] = @BitmapID";
                try
                {
                    using (SqlCommand command = new SqlCommand(commandString))
                    {
                        command.Parameters.AddWithValue("@BitmapID", visioBitmap.BitmapID);
                        command.Parameters.AddWithValue("@Description", visioBitmap.Description);
                        command.Parameters.AddWithValue("@Bitmap", visioBitmap.DiagBmpBytes);

                        command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
                        visioBitmap.HasChanged = false;
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
        }

        public void UpdateVisioDiagramBitmap(VisioDiagramBitmap visioDiagramBitmap)
        {
            const string commandString =
                            @"UPDATE [JEGR_DB].[dbo].[tblVisioDiagramBitmaps]
                                   SET [BitmapID] = @BitmapID,
                                       [DiagramID] = @DiagramID,
                                       [ZIndex] = @ZIndex
                                 WHERE [DiagBmpID] = @DiagBmpID";
            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@DiagBmpID", visioDiagramBitmap.DiagBmpID);
                    command.Parameters.AddWithValue("@BitmapID", visioDiagramBitmap.BitmapID);
                    command.Parameters.AddWithValue("@DiagramID", visioDiagramBitmap.DiagramID);
                    command.Parameters.AddWithValue("@ZIndex", visioDiagramBitmap.ZIndex);

                    command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
                    visioDiagramBitmap.HasChanged = false;
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

        #region Delete Data

        public void DeleteTableRecord(string tblName, string fieldName, int idValue)
        {
            string commandString = "DELETE FROM [dbo].[" + tblName + "] WHERE [" + fieldName + "] = @" + fieldName;
            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@"+fieldName, idValue);
                    command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
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

        public void TruncateGroupTable(string tblName)
        {
            string commandString = "TRUNCATE TABLE [dbo].[" + tblName + "]";
            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
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


        public void DeleteVisioPosition(VisioPosition visioPosition)
        {
            DeleteTableRecord("tblVisioPositions", "VisioPositionID", visioPosition.VisioPositionID);
        }

        public void DeleteVisioDiagram(VisioDiagram visioDiagram)
        {
            DeleteTableRecord("tblVisioDiagrams", "DiagramID", visioDiagram.DiagramID);
        }

        public void DeleteVisioBitmap(VisioBitmap visioBitmap)
        {
            DeleteTableRecord("tblVisioBitmaps", "BitmapID", visioBitmap.BitmapID);
        }

        public void DeleteVisioDiagramBitmap(VisioDiagramBitmap visioDiagramBitmap)
        {
            DeleteTableRecord("tblVisioDiagramBitmaps", "VisioDiagBmpID", visioDiagramBitmap.DiagBmpID);
        }

        public void DeleteVisioText(VisioText visioText)
        {
            DeleteTableRecord("tblVisioText", "VisioTextID", visioText.VisioTextID);
        }

        public void DeleteVisioFavourite(VisioFavourite visioFavourite)
        {
            DeleteTableRecord("tblVisioFavourites", "FavouriteID", visioFavourite.FavouriteID);
        }

        public void DeleteVisioMachine(VisioMachine visioMachine)
        {
            DeleteTableRecord("tblVisioMachines", "VisioMachineID", visioMachine.VisioMachineID);
            DeleteTableRecord("tblVisioPositions", "VisioPositionID", visioMachine.VisioPositionID);
        }

        public void DeleteVisioStation(VisioStation visioStation)
        {
            DeleteTableRecord("tblVisioStations", "StationID", visioStation.StationID);
            DeleteTableRecord("tblVisioPositions", "VisioPositionID", visioStation.VisioPositionID);
        }

        #endregion
    }

    #region undo redo
    public class VisioUndoRedo : INotifyPropertyChanged
    {

        #region properties
        private List<VisioDataCopy> undoList;
        private int head;
        public int Head
        {
            get { return head; }
            set
            {
                if (head != value)
                {
                    head = value;
                    if (head < 0)
                        head += maxDepth;
                    if (head >= maxDepth)
                        head -= maxDepth;
                }
            }
        }
        private int undoIndex;
        public int UndoIndex
        {
            get { return undoIndex; }
            set
            {
                if (undoIndex != value)
                {
                    undoIndex = value;
                    if (undoIndex < 0)
                        undoIndex += maxDepth;
                    undoIndex %= maxDepth;
                    NotifyPropertyChanged("UndoIndex");
                }
            }
        }
        private int redoIndex;
        public int RedoIndex
        {
            get { return redoIndex; }
            set
            {
                if (redoIndex != value)
                {
                    redoIndex = value;
                    if (redoIndex < 0)
                        redoIndex += maxDepth;
                    redoIndex %= maxDepth;
                    NotifyPropertyChanged("RedoIndex");
                }
            }
        }        
        private int undoDepth;
        public int UndoDepth
        {
            get { return undoDepth; }
            set
            {
                if (undoDepth != value)
                {
                    undoDepth = value;
                    if (undoDepth > maxDepth) 
                        undoDepth = maxDepth;
                    if (undoDepth < 0)
                        undoDepth = 0;
                    NotifyPropertyChanged("UndoDepth");
                    CanUndo = (undoDepth > 0);
                }
            }
        }
        private int redoDepth;
        public int RedoDepth
        {
            get { return redoDepth; }
            set
            {
                if (redoDepth != value)
                {
                    redoDepth = value;
                    if (redoDepth > maxDepth)
                        redoDepth = maxDepth;
                    if (redoDepth < 0)
                        redoDepth = 0;
                    NotifyPropertyChanged("RedoDepth");
                    CanRedo = (redoDepth > 0);
                }
            }           
        }
        private int maxDepth;
        public int MaxDepth
        {
            get { return maxDepth; }
        }
        private VisioDataCopy currentCopy;
        private bool canUndo;
        public bool CanUndo
        {
            get { return canUndo; }
            set
            {
                if (canUndo != value)
                {
                    canUndo = value;
                    NotifyPropertyChanged("CanUndo");
                }
            }
        }
        private bool canRedo;
        public bool CanRedo
        {
            get { return canRedo; }
            set
            {
                if (canRedo != value)
                {
                    canRedo = value;
                    NotifyPropertyChanged("CanRedo");
                }
            }
        }

         #endregion

        #region constructor
        public VisioUndoRedo(int depth)
        {
            maxDepth = depth;
            undoList = new List<VisioDataCopy>(depth);
            for (int i = 0; i < depth; i++)
            {
                VisioDataCopy vdc = new VisioDataCopy();
                undoList.Add(vdc);
            }
            head = 0;
            undoIndex = -1;
            redoIndex = -1;
            undoDepth = -1;
            redoDepth = 0;
            currentCopy = new VisioDataCopy();
        }

        #endregion

        #region undo redo
        public void Store(string desc, VisioDiagrams vDiagrams, VisioBitmaps vBitmaps, VisioDiagramBitmaps vDiagBmps,
            VisioFavourites vFavourites, VisioPositions vPositions, VisioTexts vTexts, VisioMachines vMachines,
            VisioStations vStations)
        {
            currentCopy.Store(desc, vDiagrams, vBitmaps, vDiagBmps, vFavourites, vPositions, vTexts, vMachines, vStations);
            storeCurrent();
        }

        private void storeCurrent()
        {
            undoList[Head] = currentCopy.Copy();
            UndoIndex = Head - 1;
            RedoIndex = Head;
            UndoDepth++;
            RedoDepth = 0;
            Head++;
        }

        public void Undo(VisioDiagrams vDiagrams, VisioBitmaps vBitmaps, VisioDiagramBitmaps vDiagBmps,
            VisioFavourites vFavourites, VisioPositions vPositions, VisioTexts vTexts, VisioMachines vMachines,
            VisioStations vStations)
        {
            if ((undoIndex >= 0) && CanUndo)
            {
                int currentIndex = UndoIndex;
                currentCopy = undoList[currentIndex];
                currentCopy.Retrieve(vDiagrams, vBitmaps, vDiagBmps, vFavourites, vPositions, vTexts, vMachines, vStations);
                RedoIndex = UndoIndex + 1;
                UndoIndex--;
                UndoDepth--;
                RedoDepth++;
                Head = redoIndex;
            }
        }

        public void Redo(VisioDiagrams vDiagrams, VisioBitmaps vBitmaps, VisioDiagramBitmaps vDiagBmps,
        VisioFavourites vFavourites, VisioPositions vPositions, VisioTexts vTexts, VisioMachines vMachines,
        VisioStations vStations)
        {
            if (CanRedo)
            {
                int currentIndex = RedoIndex;
                currentCopy = undoList[currentIndex];
                currentCopy.Retrieve(vDiagrams, vBitmaps, vDiagBmps, vFavourites, vPositions, vTexts, vMachines, vStations);
                UndoIndex = currentIndex - 1;
                RedoIndex = currentIndex + 1;
                RedoDepth--;
                UndoDepth++;
                Head = currentIndex + 1;
            }
        }
        #endregion

        #region Notify
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
        #endregion


    }
    
    public class VisioDataCopy
    {
        private VisioDiagrams visioDiagrams;
        private VisioBitmaps visioBitmaps;
        private VisioDiagramBitmaps visioDiagramBitmaps;
        private VisioFavourites visioFavourites;
        private VisioPositions visioPositions;
        private VisioTexts visioTexts;
        private VisioMachines visioMachines;
        private VisioStations visioStations;

        private string description;
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        private bool canCopyDiagrams = false;
        private bool canCopyBitmaps = false;
        private bool canCopyDiagramBitmaps = false;
        private bool canCopyFavourites = false;
        private bool canCopyPositions = false;
        private bool canCopyTexts = false;
        private bool canCopyMachines = false;
        private bool canCopyStations = false;
        
        public VisioDataCopy()
        {
            visioDiagrams = new VisioDiagrams();
            visioBitmaps = new VisioBitmaps();
            visioDiagramBitmaps = new VisioDiagramBitmaps();
            visioFavourites = new VisioFavourites();
            visioPositions = new VisioPositions();
            visioTexts = new VisioTexts();
            visioMachines = new VisioMachines();
            visioStations = new VisioStations();
            description = "No Data";
        }

        private bool copy(VisioDiagrams source, VisioDiagrams dest)
        {
            bool changed = false;
            if (source != null)
            {
                if (dest == null)
                {
                    dest = new VisioDiagrams();
                }
                VisioDiagrams delList = new VisioDiagrams();
                foreach (VisioDiagram rec in dest)
                {
                    VisioDiagram rec2 = source.GetById(rec.DiagramID);
                    if (rec2 == null)
                        delList.Add(rec);
                }
                foreach (VisioDiagram del in delList)
                {
                    dest.Remove(del);
                }
                foreach (VisioDiagram rec in source)
                {
                    VisioDiagram rec2 = dest.GetById(rec.DiagramID);
                    if (rec2 == null)
                    {
                        rec2 = new VisioDiagram();
                        rec2.DiagramID = rec.DiagramID;
                        rec2.HasChanged = true;
                        dest.Add(rec2);
                    }
                    else
                    {
                        rec2.HasChanged = false;
                    }
                    rec2.Title = rec.Title;
                    rec2.Description = rec.Description;
                    rec2.Width = rec.Width;
                    rec2.Height = rec.Height;
                    rec2.DeleteRecord = rec.DeleteRecord;
                    changed |= rec2.HasChanged;
                }
            }
            return changed;
        }
        private bool copy(VisioBitmaps source, VisioBitmaps dest)
        {
            bool changed = false;
            if (source != null)
            {
                if (dest == null)
                {
                    dest = new VisioBitmaps();
                }
                VisioBitmaps delList = new VisioBitmaps();
                foreach (VisioBitmap rec in dest)
                {
                    VisioBitmap rec2 = source.GetById(rec.BitmapID);
                    if (rec2 == null)
                        delList.Add(rec);
                }
                foreach (VisioBitmap del in delList)
                {
                    dest.Remove(del);
                } 
                foreach (VisioBitmap rec in source)
                {
                    VisioBitmap rec2 = dest.GetById(rec.BitmapID);
                    if (rec2 == null)
                    {
                        rec2 = new VisioBitmap();
                        rec2.BitmapID = rec.BitmapID;
                        rec2.HasChanged = true;
                        dest.Add(rec2);
                    }
                    else
                    {
                        rec2.HasChanged = false;
                    }
                    rec2.Description = rec.Description;
                    rec2.DiagBmpBytes = rec.DiagBmpBytes;
                    rec2.DeleteRecord = rec.DeleteRecord;
                    changed |= rec2.HasChanged;
                }
            }
            return changed;
        }
        private bool copy(VisioDiagramBitmaps source, VisioDiagramBitmaps dest)
        {
            bool changed = false;
            if (source != null)
            {
                if (dest == null)
                {
                    dest = new VisioDiagramBitmaps();
                }
                VisioDiagramBitmaps delList = new VisioDiagramBitmaps();
                foreach (VisioDiagramBitmap rec in dest)
                {
                    VisioDiagramBitmap rec2 = source.GetById(rec.DiagBmpID);
                    if (rec2 == null)
                        delList.Add(rec);
                }
                foreach (VisioDiagramBitmap del in delList)
                {
                    dest.Remove(del);
                }
                foreach (VisioDiagramBitmap rec in source)
                {
                    VisioDiagramBitmap rec2 = dest.GetById(rec.DiagBmpID);
                    if (rec2 == null)
                    {
                        rec2 = new VisioDiagramBitmap();
                        rec2.DiagBmpID = rec.DiagBmpID;
                        rec2.HasChanged = true;
                        dest.Add(rec2);
                    }
                    else
                    {
                        rec2.HasChanged = false;
                    }
                    rec2.BitmapID = rec.BitmapID;
                    rec2.DiagramID = rec.DiagramID;
                    rec2.ZIndex = rec.ZIndex;
                    rec2.DeleteRecord = rec.DeleteRecord;
                    changed |= rec2.HasChanged;
                }
            }
            return changed;
        }
        private bool copy(VisioFavourites source, VisioFavourites dest)
        {
            bool changed = false;
            if (source != null)
            {
                if (dest == null)
                {
                    dest = new VisioFavourites();
                }
                VisioFavourites delList = new VisioFavourites();
                foreach (VisioFavourite rec in dest)
                {
                    VisioFavourite rec2 = source.GetById(rec.FavouriteID);
                    if (rec2 == null)
                        delList.Add(rec);
                }
                foreach (VisioFavourite del in delList)
                {
                    dest.Remove(del);
                }
                foreach (VisioFavourite rec in source)
                {
                    VisioFavourite rec2 = dest.GetById(rec.FavouriteID);
                    if (rec2 == null)
                    {
                        rec2 = new VisioFavourite();
                        rec2.FavouriteID = rec.FavouriteID;
                        rec2.HasChanged = true;
                        dest.Add(rec2);
                    }
                    else
                    {
                        rec2.HasChanged = false;
                    }
                    rec2.DiagramID = rec.DiagramID;
                    rec2.AppID = rec.AppID;
                    rec2.AppUserID = rec.AppUserID;
                    rec2.FavouriteName = rec.FavouriteName;
                    rec2.XMin = rec.XMin;
                    rec2.XMax = rec.XMax;
                    rec2.YMin = rec.YMin;
                    rec2.YMax = rec.YMax;
                    rec2.DeleteRecord = rec.DeleteRecord;
                    changed |= rec2.HasChanged;
                }
            }
            return changed;
        }
        private bool copy(VisioPositions source, VisioPositions dest)
        {
            bool changed = false;
            if (source != null)
            {
                if (dest == null)
                {
                    dest = new VisioPositions();
                }
                VisioPositions delList = new VisioPositions();
                foreach (VisioPosition rec in dest)
                {
                    VisioPosition rec2 = source.GetById(rec.VisioPositionID);
                    if (rec2 == null)
                        delList.Add(rec);
                }
                foreach (VisioPosition del in delList)
                {
                    dest.Remove(del);
                }
                foreach (VisioPosition rec in source)
                {
                    VisioPosition rec2 = dest.GetById(rec.VisioPositionID);
                    if (rec2 == null)
                    {
                        rec2 = new VisioPosition();
                        rec2.VisioPositionID = rec.VisioPositionID;
                        rec2.HasChanged = true;
                        dest.Add(rec2);
                    }
                    else
                    {
                        rec2.HasChanged = false;
                    }
                    rec2.DiagramID = rec.DiagramID;
                    rec2.X = rec.X;
                    rec2.Y = rec.Y;
                    rec2.Width = rec.Width;
                    rec2.Height = rec.Height;
                    rec2.Scale = rec.Scale;
                    rec2.Rotation = rec.Rotation;
                    rec2.Mirror = rec.Mirror;
                    rec2.DeleteRecord = rec.DeleteRecord;
                    changed |= rec2.HasChanged;
                }
            }
            return changed;
        }
        private bool copy(VisioTexts source, VisioTexts dest)
        {
            bool changed = false;
            if (source != null)
            {
                if (dest == null)
                {
                    dest = new VisioTexts();
                }
                VisioTexts delList = new VisioTexts();
                foreach (VisioText rec in dest)
                {
                    VisioText rec2 = source.GetById(rec.VisioTextID);
                    if (rec2 == null)
                        delList.Add(rec);
                }
                foreach (VisioText del in delList)
                {
                    dest.Remove(del);
                }
                foreach (VisioText rec in source)
                {
                    VisioText rec2 = dest.GetById(rec.VisioTextID);
                    if (rec2 == null)
                    {
                        rec2 = new VisioText();
                        rec2.VisioTextID = rec.VisioTextID;
                        rec2.HasChanged = true;
                        dest.Add(rec2);
                    }
                    else
                    {
                        rec2.HasChanged = false;
                    }
                    rec2.VisioPositionID = rec.VisioPositionID;
                    rec2.IsDynamic = rec.IsDynamic;
                    rec2.StaticText = rec.StaticText;
                    rec2.SQLCommand = rec.SQLCommand;
                    rec2.Enabled = rec.Enabled;
                    rec2.RefreshRate = rec.RefreshRate;
                    rec2.DiagramID = rec.DiagramID;
                    rec2.X = rec.X;
                    rec2.Y = rec.Y;
                    rec2.Width = rec.Width;
                    rec2.Height = rec.Height;
                    rec2.Scale = rec.Scale;
                    rec2.Rotation = rec.Rotation;
                    rec2.Mirror = rec.Mirror; 
                    rec2.DeleteRecord = rec.DeleteRecord;
                    changed |= rec2.HasChanged;
                }
            }
            return changed;
        }
        private bool copy(VisioMachines source, VisioMachines dest)
        {
            bool changed = false;
            if (source != null)
            {
                if (dest == null)
                {
                    dest = new VisioMachines();
                }
                VisioMachines delList = new VisioMachines();
                foreach (VisioMachine rec in dest)
                {
                    VisioMachine rec2 = source.GetById(rec.VisioMachineID);
                    if (rec2 == null)
                        delList.Add(rec);
                }
                foreach (VisioMachine del in delList)
                {
                    dest.Remove(del);
                }
                foreach (VisioMachine rec in source)
                {
                    VisioMachine rec2 = dest.GetById(rec.VisioMachineID);
                    if (rec2 == null)
                    {
                        rec2 = new VisioMachine();
                        rec2.VisioMachineID = rec.VisioMachineID;
                        rec2.HasChanged = true;
                        dest.Add(rec2);
                    }
                    else
                    {
                        rec2.HasChanged = false;
                    }
                    rec2.VisioPositionID = rec.VisioPositionID;
                    rec2.MachineID = rec.MachineID;
                    rec2.MachineShape = rec.MachineShape;
                    rec2.DiagramID = rec.DiagramID;
                    rec2.X = rec.X;
                    rec2.Y = rec.Y;
                    rec2.Width = rec.Width;
                    rec2.Height = rec.Height;
                    rec2.Scale = rec.Scale;
                    rec2.Rotation = rec.Rotation;
                    rec2.Mirror = rec.Mirror;
                    rec2.DeleteRecord = rec.DeleteRecord;
                    changed |= rec2.HasChanged;
                }
            }
            return changed;
        }
        private bool copy(VisioStations source, VisioStations dest)
        {
            bool changed = false;
            if (source != null)
            {
                if (dest == null)
                {
                    dest = new VisioStations();
                }
                VisioStations delList = new VisioStations();
                foreach (VisioStation rec in dest)
                {
                    VisioStation rec2 = source.GetById(rec.StationID);
                    if (rec2 == null)
                        delList.Add(rec);
                }
                foreach (VisioStation del in delList)
                {
                    dest.Remove(del);
                }
                foreach (VisioStation rec in source)
                {
                    VisioStation rec2 = dest.GetById(rec.StationID);
                    if (rec2 == null)
                    {
                        rec2 = new VisioStation();
                        rec2.StationID = rec.StationID;
                        rec2.HasChanged = true;
                        dest.Add(rec2);
                    }
                    else
                    {
                        rec2.HasChanged = false;
                    }
                    rec2.VisioPositionID = rec.VisioPositionID;
                    rec2.MachineID = rec.MachineID;
                    rec2.SubID = rec.SubID;
                    rec2.DiagramID = rec.DiagramID;
                    rec2.X = rec.X;
                    rec2.Y = rec.Y;
                    rec2.Width = rec.Width;
                    rec2.Height = rec.Height;
                    rec2.Scale = rec.Scale;
                    rec2.Rotation = rec.Rotation;
                    rec2.Mirror = rec.Mirror;
                    rec2.DeleteRecord = rec.DeleteRecord;
                    changed |= rec2.HasChanged;
                }
            }
            return changed;
        }

        public void Store(String desc, VisioDiagrams list)
        {
            if (list != null)
            {
                description = desc;
                canCopyDiagrams = copy(list, visioDiagrams);
            }
        }
        public void Retrieve(VisioDiagrams list)
        {
            if (list != null)
            {
                if (canCopyDiagrams)
                    copy(visioDiagrams, list);
            }
        }
        public void Store(String desc, VisioBitmaps list)
        {
            if (list != null)
            {
                description = desc;
                canCopyBitmaps = copy(list, visioBitmaps);
            }
        }
        public void Retrieve(VisioBitmaps list)
        {
            if (list != null)
            {
                if (canCopyBitmaps)
                    copy(visioBitmaps, list);
            }
        }
        public void Store(String desc, VisioDiagramBitmaps list)
        {
            if (list != null)
            {
                description = desc;
                canCopyDiagramBitmaps = copy(list, visioDiagramBitmaps);
            }
        }
        public void Retrieve(VisioDiagramBitmaps list)
        {
            if (list != null)
            {
                if (canCopyDiagramBitmaps)
                    copy(visioDiagramBitmaps, list);
            }
        }
        public void Store(String desc, VisioFavourites list)
        {
            if (list != null)
            {
                description = desc;
                canCopyFavourites = copy(list, visioFavourites);
            }
        }
        public void Retrieve(VisioFavourites list)
        {
            if (list != null)
            {
                if (canCopyFavourites)
                    copy(visioFavourites, list);
            }
        }
        public void Store(String desc, VisioPositions list)
        {
            if (list != null)
            {
                description = desc;
                canCopyPositions = copy(list, visioPositions);
            }
        }
        public void Retrieve(VisioPositions list)
        {
            if (list != null)
            {
                if (canCopyPositions)
                    copy(visioPositions, list);
            }
        }
        public void Store(String desc, VisioTexts list)
        {
            if (list != null)
            {
                description = desc;
                canCopyTexts = copy(list, visioTexts);
            }
        }
        public void Retrieve(VisioTexts list)
        {
            if (list != null)
            {
                if (canCopyTexts)
                    copy(visioTexts, list);
            }
        }
        public void Store(String desc, VisioMachines list)
        {
            if (list != null)
            {
                description = desc;
                canCopyMachines = copy(list, visioMachines);
            }
        }
        public void Retrieve(VisioMachines list)
        {
            if (list != null)
            {
                if (canCopyMachines)
                    copy(visioMachines, list);
            }
        }
        public void Store(String desc, VisioStations list)
        {
            if (list != null)
            {
                description = desc;
                canCopyStations = copy(list, visioStations);
            }
        }
        public void Retrieve(VisioStations list)
        {
            if (list != null)
            {
                if (canCopyStations)
                    copy(visioStations, list);
            }
        }

        public void Store(VisioDataCopy visioDataCopy)
        {
            Store(visioDataCopy.Description, visioDataCopy.visioDiagrams);
            Store(visioDataCopy.Description, visioDataCopy.visioBitmaps);
            Store(visioDataCopy.Description, visioDataCopy.visioDiagramBitmaps);
            Store(visioDataCopy.Description, visioDataCopy.visioFavourites);
            Store(visioDataCopy.Description, visioDataCopy.visioPositions);
            Store(visioDataCopy.Description, visioDataCopy.visioTexts);
            Store(visioDataCopy.Description, visioDataCopy.visioMachines);
            Store(visioDataCopy.Description, visioDataCopy.visioStations);
        }

        public void Store(String desc, VisioDiagrams vDiagrams, VisioBitmaps vBitmaps, VisioDiagramBitmaps vDiagBmps,
            VisioFavourites vFavourites, VisioPositions vPositions, VisioTexts vTexts, VisioMachines vMachines,
            VisioStations vStations)
        {
            Store(desc, vDiagrams);
            Store(desc, vBitmaps);
            Store(desc, vDiagBmps);
            Store(desc, vFavourites);
            Store(desc, vPositions);
            Store(desc, vTexts);
            Store(desc, vMachines);
            Store(desc, vStations);
        }

        public void Store(String desc, VisioFavourites vFavourites, VisioPositions vPositions, VisioTexts vTexts, 
            VisioMachines vMachines, VisioStations vStations)
        {
            Store(desc, vFavourites);
            Store(desc, vPositions);
            Store(desc, vTexts);
            Store(desc, vMachines);
            Store(desc, vStations);
        }

        public void Retrieve(VisioDiagrams vDiagrams, VisioBitmaps vBitmaps, VisioDiagramBitmaps vDiagBmps,
            VisioFavourites vFavourites, VisioPositions vPositions, VisioTexts vTexts, VisioMachines vMachines,
            VisioStations vStations)
        {
            Retrieve(vDiagrams);
            Retrieve(vBitmaps);
            Retrieve(vDiagBmps);
            Retrieve(vFavourites);
            Retrieve(vPositions);
            Retrieve(vTexts);
            Retrieve(vMachines);
            Retrieve(vStations);
        }

        public void Retrieve(VisioFavourites vFavourites, VisioPositions vPositions, VisioTexts vTexts,
            VisioMachines vMachines, VisioStations vStations)
        {
            Retrieve(vFavourites);
            Retrieve(vPositions);
            Retrieve(vTexts);
            Retrieve(vMachines);
            Retrieve(vStations);
        }

        public VisioDataCopy Copy()
        {
            VisioDataCopy visioDataCopy = new VisioDataCopy();
            visioDataCopy.Store(this);
            return visioDataCopy;
        }
    }

    #endregion

    #region Data Collection Classes
    public class VisioDiagrams : List<VisioDiagram>, IDataFiller, INotifyCollectionChanged
    {
        #region INotifyCollectionChanged Members

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public void Reset()
        {
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        new public void Add(VisioDiagram diag)
        {
            base.Add(diag);
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, diag));
            }
        }

        new public void Remove(VisioDiagram diag)
        {
            base.RemoveAt(this.IndexOf(diag));
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, diag));
            }
        }
        #endregion

        #region properties
        private double lifespan = 24.0;
        private string tblName = "tblVisioDiagrams";
        private DateTime lastDBUpdate;
        public double Lifespan
        {
            get { return lifespan; }
            set { lifespan = value; }
        }
        private DateTime lastRead;
        public DateTime LastRead
        {
            get { return lastRead; }
            set { lastRead = value; }
        }
        private bool neverExpire = false;
        public bool NeverExpire
        {
            get { return neverExpire; }
            set { neverExpire = value; }
        }
        private bool isValid = false;
        public bool IsValid
        {
            get
            {
                bool test = isValid && (this.Count > 0) && (lastRead != null) && (!neverExpire);
                if (test)
                {
                    SqlDataAccess da = SqlDataAccess.Singleton;
                    lastDBUpdate = da.TableLastUpdated(tblName);
                    int x = lastDBUpdate.CompareTo(lastRead.AddSeconds(0.95));
                    test = (x <= 0);
                    if (test)
                    {
                        DateTime testTime = lastRead.AddHours(lifespan);
                        test = testTime > da.ServerTime;
                    }
                }
                return test || neverExpire;
            }
            set
            {
                isValid = value;
                if (!isValid)
                    neverExpire = false;
            }
        }
        private bool forceUpdate = false;
        [XmlIgnore]
        public bool ForceUpdate
        {
            get { return forceUpdate; }
            set { forceUpdate = value; }
        }

        #endregion

        #region methods
        public int Fill(SqlDataReader dr)
        {
            SqlDataAccess da = SqlDataAccess.Singleton;
            int DiagramIDPos = dr.GetOrdinal("DiagramID");
            int TitlePos = dr.GetOrdinal("Title");
            int DescriptionPos = dr.GetOrdinal("Description");
            int WidthPos = dr.GetOrdinal("Width");
            int HeightPos = dr.GetOrdinal("Height");

            this.Clear();
            while (dr.Read())
            {
                VisioDiagram visioDiagram = new VisioDiagram()
                {
                    DiagramID = dr.GetInt32(DiagramIDPos),
                    Title=dr.GetString(TitlePos),
                    Description=dr.GetString(DescriptionPos),
                    Width=dr.GetInt32(WidthPos),
                    Height=dr.GetInt32(HeightPos),
                    HasChanged = false
                };

                this.Add(visioDiagram);
            }
            LastRead = da.ServerTime;
            IsValid = true;

            return this.Count;
        }

        public int GetNextID()
        {
            int id = 0;
            bool found;
            {
                do
                {
                    id++;
                    found = false;
                    foreach (VisioDiagram rec in this)
                    {
                        found |= (rec.ID == id);
                    }
                } while (found);
            }
            return id;
        }

        #endregion

        #region find
        public VisioDiagram GetById(int id)
        {
            return this.Find(delegate(VisioDiagram visioDiagram)
            {
                return visioDiagram.DiagramID == id;
            });
        }

        public VisioDiagram GetByName(string aName)
        {
            return this.Find(delegate(VisioDiagram visioDiagram)
            {
                return visioDiagram.Title == aName;
            });
        }

        public void DeleteAllFromDB()
        {
            SqlDataAccess da = SqlDataAccess.Singleton;
                da.TruncateGroupTable("tblVisioDiagrams");
        }

        #endregion

        #region updatedb

        public void UpdateToDB()
        {
            SqlDataAccess da = SqlDataAccess.Singleton;
            if (ForceUpdate)
            {
                da.TruncateGroupTable(tblName);
                foreach (VisioDiagram rec in this)
                {
                    if (!rec.DeleteRecord)
                        da.InsertNewVisioDiagram(rec);
                }
            }
            else
            {
                foreach (VisioDiagram diag in this)
                {
                    if (diag.IsNew)
                    {
                        if (!diag.DeleteRecord)
                            da.InsertNewVisioDiagram(diag);
                    }
                    else
                    {
                        if (diag.HasChanged)
                        {
                            if (diag.DeleteRecord)
                                da.DeleteVisioDiagram(diag);
                            else
                                da.UpdateVisioDiagram(diag);
                        }
                    }
                }
                VisioDiagrams delList = new VisioDiagrams();
                foreach (VisioDiagram rec in this)
                {
                    if (rec.DeleteRecord)
                        delList.Add(rec);
                }
                foreach (VisioDiagram rec in delList)
                {
                    this.Remove(rec);
                }
            }
        }

        #endregion
    }

    public class VisioBitmaps : List<VisioBitmap>, IDataFiller, INotifyCollectionChanged
    {
        #region INotifyCollectionChanged Members

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public void Reset()
        {
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        new public void Add(VisioBitmap bmp)
        {
            base.Add(bmp);
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, bmp));
            }
        }

        new public void Remove(VisioBitmap bmp)
        {
            base.RemoveAt(this.IndexOf(bmp));
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, bmp));
            }
        }
        #endregion
        private double lifespan = 24.0;
        private string tblName = "tblVisioBitmaps";
        private DateTime lastDBUpdate;
        public double Lifespan
        {
            get { return lifespan; }
            set { lifespan = value; }
        }
        private DateTime lastRead;
        public DateTime LastRead
        {
            get { return lastRead; }
            set { lastRead = value; }
        }
        private bool neverExpire = false;
        public bool NeverExpire
        {
            get { return neverExpire; }
            set { neverExpire = value; }
        }
        private bool isValid = false;
        public bool IsValid
        {
            get
            {
                bool test = isValid && (this.Count > 0) && (lastRead != null) && (!neverExpire);
                if (test)
                {
                    SqlDataAccess da = SqlDataAccess.Singleton;
                    lastDBUpdate = da.TableLastUpdated(tblName);
                    int x = lastDBUpdate.CompareTo(lastRead.AddSeconds(0.95));
                    test = (x <= 0);
                    if (test)
                    {
                        DateTime testTime = lastRead.AddHours(lifespan);
                        test = testTime > da.ServerTime;
                    }
                }
                return test || neverExpire;
            }
            set
            {
                isValid = value;
                if (!isValid)
                    neverExpire = false;
            }
        }
        private bool forceUpdate = false;
        [XmlIgnore]
        public bool ForceUpdate
        {
            get { return forceUpdate; }
            set { forceUpdate = value; }
        }

        public int Fill(SqlDataReader dr)
        {
            SqlDataAccess da = SqlDataAccess.Singleton;
            int BitmapIDPos = dr.GetOrdinal("BitmapID");
            int BitmapPos = dr.GetOrdinal("Bitmap");
            int DescriptionPos = dr.GetOrdinal("Description");

            this.Clear();
            while (dr.Read())
            {
                VisioBitmap visioBitmap = new VisioBitmap()
                {
                    BitmapID = dr.GetInt32(BitmapIDPos),
                    DiagBmpBytes =  dr.IsDBNull(BitmapPos) ? (byte[]) null : (byte[])dr["Bitmap"],
                    //DiagBmpBytes = (byte[])dr["Bitmap"],
                    Description = dr.GetString(DescriptionPos),
                    HasChanged = false
                };
                this.Add(visioBitmap);
            }
            LastRead = da.ServerTime;
            IsValid = true;

            return this.Count;
        }

        public int GetNextID()
        {
            int id = 0;
            bool found;
            {
                do
                {
                    id++;
                    found = false;
                    foreach (VisioBitmap rec in this)
                    {
                        found |= (rec.ID == id);
                    }
                } while (found);
            }
            return id;
        }

        public VisioBitmap GetById(int id)
        {
            return this.Find(delegate(VisioBitmap visioBitmap)
            {
                return visioBitmap.BitmapID == id;
            });
        }

        public VisioBitmap GetByName(string aName)
        {
            return this.Find(delegate(VisioBitmap visioBitmap)
            {
                return visioBitmap.Description == aName;
            });
        }

        public void DeleteAllFromDB()
        {
            SqlDataAccess da = SqlDataAccess.Singleton;
            da.TruncateGroupTable("tblVisioBitmaps");
        }

        public void UpdateToDB()
        {
            SqlDataAccess da = SqlDataAccess.Singleton;
            if (ForceUpdate)
            {
                da.TruncateGroupTable(tblName);
                foreach (VisioBitmap rec in this)
                {
                    if (!rec.DeleteRecord)
                        da.InsertNewVisioBitmap(rec);
                }
            }
            else
            {
                foreach (VisioBitmap rec in this)
                {
                    if (rec.IsNew)
                    {
                        if (rec.DiagBmpBytes != null)
                        {
                            if (!rec.DeleteRecord)
                                da.InsertNewVisioBitmap(rec);
                        }
                    }
                    else
                    {
                        if (rec.DeleteRecord)
                            da.DeleteVisioBitmap(rec);
                        else
                            da.UpdateVisioBitmap(rec);
                    }
                }
                VisioBitmaps delList = new VisioBitmaps();
                foreach (VisioBitmap rec in this)
                {
                    if (rec.DeleteRecord)
                        delList.Add(rec);
                }
                foreach (VisioBitmap rec in delList)
                {
                    this.Remove(rec);
                }
            }
        }
    }

    public class VisioDiagramBitmaps : List<VisioDiagramBitmap>, IDataFiller, INotifyCollectionChanged
    {
        #region INotifyCollectionChanged Members

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public void Reset()
        {
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        new public void Add(VisioDiagramBitmap diagbmp)
        {
            base.Add(diagbmp);
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, diagbmp));
            }
        }

        new public void Remove(VisioDiagramBitmap diagbmp)
        {
            base.RemoveAt(this.IndexOf(diagbmp));
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, diagbmp));
            }
        }
        #endregion
        private double lifespan = 24.0;
        private string tblName = "tblVisioDiagramBitmaps";
        private DateTime lastDBUpdate;
        public double Lifespan
        {
            get { return lifespan; }
            set { lifespan = value; }
        }
        private DateTime lastRead;
        public DateTime LastRead
        {
            get { return lastRead; }
            set { lastRead = value; }
        }
        private bool neverExpire = false;
        public bool NeverExpire
        {
            get { return neverExpire; }
            set { neverExpire = value; }
        }
        private bool isValid = false;
        public bool IsValid
        {
            get
            {
                bool test = isValid && (this.Count > 0) && (lastRead != null) && (!neverExpire);
                if (test)
                {
                    SqlDataAccess da = SqlDataAccess.Singleton;
                    lastDBUpdate = da.TableLastUpdated(tblName);
                    int x = lastDBUpdate.CompareTo(lastRead.AddSeconds(0.95));
                    test = (x <= 0);
                    if (test)
                    {
                        DateTime testTime = lastRead.AddHours(lifespan);
                        test = testTime > da.ServerTime;
                    }
                }
                return test || neverExpire;
            }
            set
            {
                isValid = value;
                if (!isValid)
                    neverExpire = false;
            }
        }
        private bool forceUpdate = false;
        [XmlIgnore]
        public bool ForceUpdate
        {
            get { return forceUpdate; }
            set { forceUpdate = value; }
        }

        public int Fill(SqlDataReader dr)
        {
            SqlDataAccess da = SqlDataAccess.Singleton;
            int DiagBmpIDPos = dr.GetOrdinal("DiagBmpID");
            int DiagramIDPos = dr.GetOrdinal("DiagramID");
            int BitMapIDPos = dr.GetOrdinal("BitmapID");
            int ZIndexPos = dr.GetOrdinal("ZIndex");


            this.Clear();
            while (dr.Read())
            {
                VisioDiagramBitmap visioDiagramBitmap = new VisioDiagramBitmap()
                {
                    DiagBmpID = dr.GetInt32(DiagBmpIDPos),
                    DiagramID = dr.GetInt32(DiagramIDPos),
                    BitmapID = dr.GetInt32(BitMapIDPos),
                    ZIndex = dr.GetInt32(ZIndexPos),

                    HasChanged = false
                };
                this.Add(visioDiagramBitmap);
            }
            LastRead = da.ServerTime;
            IsValid = true;

            return this.Count;
        }

        public int GetNextID()
        {
            int id = 0;
            bool found;
            {
                do
                {
                    id++;
                    found = false;
                    foreach (VisioDiagramBitmap rec in this)
                    {
                        found |= (rec.ID == id);
                    }
                } while (found);
            }
            return id;
        }
        
        public VisioDiagramBitmap GetById(int id)
        {
            return this.Find(delegate(VisioDiagramBitmap visioDiagramBitmap)
            {
                return visioDiagramBitmap.DiagBmpID == id;
            });
        }

        //public VisioDiagramBitmap GetByBmpId(int id)
        //{
        //    return this.Find(delegate(VisioDiagramBitmap visioDiagramBitmap)
        //    {
        //        return visioDiagramBitmap.BitmapID == id;
        //    });
        //}

        public VisioDiagramBitmap GetByDiagIdZ(int id, int z)
        {
            return this.Find(delegate(VisioDiagramBitmap visioDiagramBitmap)
            {
                return ((visioDiagramBitmap.DiagramID == id)
                    && (visioDiagramBitmap.ZIndex == z));
            });
        }

        public void DeleteAllFromDB()
        {
            SqlDataAccess da = SqlDataAccess.Singleton;
            da.TruncateGroupTable("tblVisioDiagramBitmaps");
        }

        public void UpdateToDB()
        {
            SqlDataAccess da = SqlDataAccess.Singleton;
            if (ForceUpdate)
            {
                da.TruncateGroupTable(tblName);
                foreach (VisioDiagramBitmap rec in this)
                {
                    if (!rec.DeleteRecord)
                        da.InsertNewVisioDiagramBitmap(rec);
                }
            }
            else
            {
                foreach (VisioDiagramBitmap diagbmp in this)
                {
                    if (diagbmp.IsNew)
                    {
                        if (!diagbmp.DeleteRecord)
                            da.InsertNewVisioDiagramBitmap(diagbmp);
                    }
                    else
                    {
                        if (diagbmp.DeleteRecord)
                            da.DeleteVisioDiagramBitmap(diagbmp);
                        else
                            da.UpdateVisioDiagramBitmap(diagbmp);
                    }
                }
                VisioDiagramBitmaps delList = new VisioDiagramBitmaps();
                foreach (VisioDiagramBitmap rec in this)
                {
                    if (rec.DeleteRecord)
                        delList.Add(rec);
                }
                foreach (VisioDiagramBitmap rec in delList)
                {
                    this.Remove(rec);
                }
            }
        }
    }

    public class VisioPositions : List<VisioPosition>, IDataFiller, INotifyCollectionChanged
    {
        #region INotifyCollectionChanged Members

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public void Reset()
        {
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        new public void Add(VisioPosition pos)
        {
            base.Add(pos);
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, pos));
            }
        }

        new public void Remove(VisioPosition pos)
        {
            base.RemoveAt(this.IndexOf(pos));
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, pos));
            }
        }
        #endregion
        private double lifespan = 24.0;
        private string tblName = "tblVisioPositions";
        private DateTime lastDBUpdate;
        public double Lifespan
        {
            get { return lifespan; }
            set { lifespan = value; }
        }
        private DateTime lastRead;
        public DateTime LastRead
        {
            get { return lastRead; }
            set { lastRead = value; }
        }
        private bool neverExpire = false;
        public bool NeverExpire
        {
            get { return neverExpire; }
            set { neverExpire = value; }
        }
        private bool isValid = false;
        public bool IsValid
        {
            get
            {
                bool test = isValid && (this.Count > 0) && (lastRead != null) && (!neverExpire);
                if (test)
                {
                    SqlDataAccess da = SqlDataAccess.Singleton;
                    lastDBUpdate = da.TableLastUpdated(tblName);
                    int x = lastDBUpdate.CompareTo(lastRead.AddSeconds(0.95));
                    test = (x <= 0);
                    if (test)
                    {
                        DateTime testTime = lastRead.AddHours(lifespan);
                        test = testTime > da.ServerTime;
                    }
                }
                return test || neverExpire;
            }
            set
            {
                isValid = value;
                if (!isValid)
                    neverExpire = false;
            }
        }
        private bool forceUpdate = false;
        [XmlIgnore]
        public bool ForceUpdate
        {
            get { return forceUpdate; }
            set { forceUpdate = value; }
        }
        public int Fill(SqlDataReader dr)
        {
            SqlDataAccess da = SqlDataAccess.Singleton;
            int VisioPositionIDPos = dr.GetOrdinal("VisioPositionID");
            int DiagramIDPos = dr.GetOrdinal("DiagramID");
            int XPos = dr.GetOrdinal("X");
            int YPos = dr.GetOrdinal("Y");
            int WidthPos = -1;
            int HeightPos = -1;
            int ScalePos = -1;
            int RotationPos = -1;
            int MirrorPos = -1;
            if (da.DatabaseVersion >= 1.7)
            {
                WidthPos = dr.GetOrdinal("Width");
                HeightPos = dr.GetOrdinal("Height");
                ScalePos = dr.GetOrdinal("Scale");
                RotationPos = dr.GetOrdinal("Rotation");
                MirrorPos = dr.GetOrdinal("Mirror");
            }

            this.Clear();
            while (dr.Read())
            {
                VisioPosition visioPosition = new VisioPosition();
                visioPosition.VisioPositionID = dr.GetInt32(VisioPositionIDPos);
                visioPosition.DiagramID = dr.GetInt32(DiagramIDPos);
                if (da.DatabaseVersion < 1.7)
                {
                    visioPosition.IntX = dr.GetInt32(XPos);
                    visioPosition.IntY = dr.GetInt32(YPos);
                }
                else
                {
                    visioPosition.X = (double)dr.GetDecimal(XPos);
                    visioPosition.Y = (double)dr.GetDecimal(YPos);
                    visioPosition.Width = dr.GetInt32(WidthPos);
                    visioPosition.Height = dr.GetInt32(HeightPos);
                    visioPosition.Scale = (double)dr.GetDecimal(ScalePos);
                    visioPosition.Rotation = dr.GetInt32(RotationPos);
                    visioPosition.Mirror = dr.GetBoolean(MirrorPos);
                }
                visioPosition.HasChanged = false;

                this.Add(visioPosition);
            }
            LastRead = da.ServerTime;
            IsValid = true;

            return this.Count;
        }

        public int GetNextID()
        {
            int id = 0;
            bool found;
            {
                do
                {
                    id++;
                    found = false;
                    foreach (VisioPosition rec in this)
                    {
                        found |= (rec.ID == id);
                    }
                } while (found);
            }
            return id;
        }
        
        public VisioPosition GetById(int id)
        {
            return this.Find(delegate(VisioPosition visioPosition)
            {
                return visioPosition.VisioPositionID == id;
            });
        }

        public void DeleteAllFromDB()
        {
            SqlDataAccess da = SqlDataAccess.Singleton;
            foreach (VisioPosition visioPosition in this)
            {
                da.DeleteVisioPosition(visioPosition);
            }
        }

        //public void UpdateToDB()
        //{
        //    SqlDataAccess da = SqlDataAccess.Singleton;
        //    foreach( VisioPosition visioPosition in this)
        //    {
        //        if (visioPosition.IsNew)
        //        {
        //            if (!visioPosition.DeleteRecord)
        //                da.InsertNewVisioPosition(visioPosition);
        //        }
        //        else
        //        {
        //            if (visioPosition.DeleteRecord)
        //                da.DeleteVisioPosition(visioPosition);
        //            else
        //                da.UpdateVisioPosition(visioPosition);
        //        }
        //    }
        //}
    }

    public class VisioTexts : List<VisioText>, IDataFiller, INotifyCollectionChanged
    {
        #region INotifyCollectionChanged Members

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public void Reset()
        {
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        new public void Add(VisioText txt)
        {
            base.Add(txt);
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, txt));
            }
        }

        new public void Remove(VisioText txt)
        {
            base.RemoveAt(this.IndexOf(txt));
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, txt));
            }
        }
        #endregion
        private double lifespan = 24.0;
        private string tblName = "tblVisioText";
        private DateTime lastDBUpdate;
        public double Lifespan
        {
            get { return lifespan; }
            set { lifespan = value; }
        }
        private DateTime lastRead;
        public DateTime LastRead
        {
            get { return lastRead; }
            set { lastRead = value; }
        }
        private bool neverExpire = false;
        public bool NeverExpire
        {
            get { return neverExpire; }
            set { neverExpire = value; }
        }
        private bool isValid = false;
        public bool IsValid
        {
            get
            {
                bool test = isValid && (this.Count > 0) && (lastRead != null) && (!neverExpire);
                if (test)
                {
                    SqlDataAccess da = SqlDataAccess.Singleton;
                    lastDBUpdate = da.TableLastUpdated(tblName);
                    int x = lastDBUpdate.CompareTo(lastRead.AddSeconds(0.95));
                    test = (x <= 0);
                    if (test)
                    {
                        DateTime testTime = lastRead.AddHours(lifespan);
                        test = testTime > da.ServerTime;
                    }
                }
                return test || neverExpire;
            }
            set
            {
                isValid = value;
                if (!isValid)
                    neverExpire = false;
            }
        }
        private bool forceUpdate = false;
        [XmlIgnore]
        public bool ForceUpdate
        {
            get { return forceUpdate; }
            set { forceUpdate = value; }
        }
        public int Fill(SqlDataReader dr)
        {
            SqlDataAccess da = SqlDataAccess.Singleton;
            int VisioTextIDPos = dr.GetOrdinal("VisioTextID");
            int VisioPositionIDPos = dr.GetOrdinal("VisioPositionID");
            int IsDynamicPos = dr.GetOrdinal("IsDynamic");
            int StaticTextPos = dr.GetOrdinal("StaticText");
            int SQLCommandPos = dr.GetOrdinal("SQLCommand");
            int RefreshRatePos = dr.GetOrdinal("RefreshRate");
            int EnabledPos = dr.GetOrdinal("Enabled");
            int DiagramIDPos = dr.GetOrdinal("DiagramID");
            int XPos = dr.GetOrdinal("X");
            int YPos = dr.GetOrdinal("Y");
            int WidthPos = dr.GetOrdinal("Width");
            int HeightPos = dr.GetOrdinal("Height");
            int ScalePos = dr.GetOrdinal("Scale");
            int RotationPos = dr.GetOrdinal("Rotation");
            int MirrorPos = dr.GetOrdinal("Mirror");

            this.Clear();
            while (dr.Read())
            {
                VisioText visioText = new VisioText()
                {
                    VisioTextID = dr.GetInt32(VisioTextIDPos),
                    VisioPositionID = dr.GetInt32(VisioPositionIDPos),
                    IsDynamic = dr.GetBoolean(IsDynamicPos),
                    StaticText = dr.GetString(StaticTextPos),
                    SQLCommand = dr.GetString(SQLCommandPos),
                    RefreshRate = dr.GetInt32(RefreshRatePos),
                    Enabled = dr.GetBoolean(EnabledPos),
                    DiagramID = dr.GetInt32(DiagramIDPos),
                    X = (double)dr.GetDecimal(XPos),
                    Y = (double)dr.GetDecimal(YPos),
                    Width = dr.GetInt32(WidthPos),
                    Height = dr.GetInt32(HeightPos),
                    Scale = (double)dr.GetDecimal(ScalePos),
                    Rotation = dr.GetInt32(RotationPos),
                    Mirror = dr.GetBoolean(MirrorPos),
                    HasChanged = false
                };

                this.Add(visioText);
            }
            LastRead = da.ServerTime;
            IsValid = true;

            return this.Count;
        }

        public int GetNextID()
        {
            int id = 0;
            bool found;
            {
                do
                {
                    id++;
                    found = false;
                    foreach (VisioText rec in this)
                    {
                        found |= (rec.ID == id);
                    }
                } while (found);
            }
            return id;
        }

        public VisioText GetById(int id)
        {
            return this.Find(delegate(VisioText visioText)
            {
                return visioText.VisioTextID == id;
            });
        }

        public void DeleteAllFromDB()
        {
            SqlDataAccess da = SqlDataAccess.Singleton;
            foreach (VisioText visioText in this)
            {
                da.DeleteVisioText(visioText);
            }
        }

        public void UpdateToDB()
        {
            SqlDataAccess da = SqlDataAccess.Singleton;
            if (ForceUpdate)
            {
                da.TruncateGroupTable(tblName);
                foreach (VisioText rec in this)
                {
                    if (!rec.DeleteRecord)
                        da.InsertNewVisioText(rec);
                }
            }
            else
            {
                foreach (VisioText rec in this)
                {
                    if (rec.IsNew)
                    {
                        if (!rec.DeleteRecord)
                            da.InsertNewVisioText(rec);
                    }
                    else
                    {
                        if (rec.HasChanged)
                        {
                            if (rec.DeleteRecord)
                                da.DeleteVisioText(rec);
                            else
                                da.UpdateVisioText(rec);
                        }
                    }
                }
                VisioTexts delList = new VisioTexts();
                foreach (VisioText rec in this)
                {
                    if (rec.DeleteRecord)
                        delList.Add(rec);
                }
                foreach (VisioText rec in delList)
                {
                    this.Remove(rec);
                }
            }
        }
    }

    public class VisioFavourites : List<VisioFavourite>, IDataFiller, INotifyCollectionChanged
    {
        #region INotifyCollectionChanged Members

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public void Reset()
        {
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        new public void Add(VisioFavourite fav)
        {
            base.Add(fav);
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, fav));
            }
        }

        new public void Remove(VisioFavourite fav)
        {
            base.RemoveAt(this.IndexOf(fav));
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, fav));
            }
        }
        #endregion

        private double lifespan = 1.0;
        private string tblName = "tblVisioFavourites";
        private DateTime lastDBUpdate;
        public double Lifespan
        {
            get { return lifespan; }
            set { lifespan = value; }
        }
        private DateTime lastRead;
        public DateTime LastRead
        {
            get { return lastRead; }
            set { lastRead = value; }
        }
        private bool neverExpire = false;
        public bool NeverExpire
        {
            get { return neverExpire; }
            set { neverExpire = value; }
        }
        private bool isValid = false;
        public bool IsValid
        {
            get
            {
                bool test = isValid && (this.Count > 0) && (lastRead != null) && (!neverExpire);
                if (test)
                {
                    SqlDataAccess da = SqlDataAccess.Singleton;
                    lastDBUpdate = da.TableLastUpdated(tblName);
                    int x = lastDBUpdate.CompareTo(lastRead.AddSeconds(0.95));
                    test = (x <= 0);
                    if (test)
                    {
                        DateTime testTime = lastRead.AddHours(lifespan);
                        test = testTime > da.ServerTime;
                    }
                }
                return test || neverExpire;
            }
            set
            {
                isValid = value;
                if (!isValid)
                    neverExpire = false;
            }
        }
        private bool forceUpdate = false;
        [XmlIgnore]
        public bool ForceUpdate
        {
            get { return forceUpdate; }
            set { forceUpdate = value; }
        }

        public int Fill(SqlDataReader dr)
        {
            int FavouriteIDPos = dr.GetOrdinal("FavouriteID");
            int DiagramIDPos = dr.GetOrdinal("DiagramID");
            int AppIDPos = dr.GetOrdinal("AppID");
            int AppUserIDPos = dr.GetOrdinal("AppUserID");
            int FavouriteNamePos = dr.GetOrdinal("FavouriteName");
            int XMinPos = dr.GetOrdinal("XMin");
            int YMinPos = dr.GetOrdinal("YMin");
            int XMaxPos = dr.GetOrdinal("XMax");
            int YMaxPos = dr.GetOrdinal("YMax");
            this.Clear();
            while (dr.Read())
            {
                VisioFavourite fav = new VisioFavourite()
                {
                    FavouriteID = dr.GetInt32(FavouriteIDPos),
                    DiagramID = dr.GetInt32(DiagramIDPos),
                    AppID = dr.GetInt32(AppIDPos),
                    AppUserID = dr.GetInt32(AppUserIDPos),
                    FavouriteName = dr.GetString(FavouriteNamePos),
                    XMin = dr.GetInt32(XMinPos),
                    YMin = dr.GetInt32(YMinPos),
                    XMax = dr.GetInt32(XMaxPos),
                    YMax = dr.GetInt32(YMaxPos),
                    HasChanged = false
                };

                // Add to collection
                this.Add(fav);
            }
            SqlDataAccess da = SqlDataAccess.Singleton;
            lastRead = da.ServerTime;
            isValid = true;

            return this.Count;
        }

        public int GetNextID()
        {
            int id = 0;
            bool found;
            {
                do
                {
                    id++;
                    found = false;
                    foreach (VisioFavourite rec in this)
                    {
                        found |= (rec.ID == id);
                    }
                } while (found);
            }
            return id;
        }

        public VisioFavourite GetById(int id)
        {
            return this.Find(delegate(VisioFavourite fav)
            {
                return fav.FavouriteID == id;
            });
        }

        public VisioFavourite GetByName(string favName)
        {
            return this.Find(delegate(VisioFavourite fav)
            {
                return (fav.FavouriteName == favName);
            });
        }

        public void UpdateToDB()
        {
            SqlDataAccess da = SqlDataAccess.Singleton;
            if (ForceUpdate)
            {
                da.TruncateGroupTable(tblName);
                foreach (VisioFavourite rec in this)
                {
                    if (!rec.DeleteRecord)
                        da.InsertNewVisioFavourite(rec);
                }
            }
            else
            {
                foreach (VisioFavourite rec in this)
                {
                    if (rec.IsNew)
                    {
                        if (!rec.DeleteRecord)
                            da.InsertNewVisioFavourite(rec);
                    }
                    else
                    {
                        if (rec.DeleteRecord)
                            da.DeleteVisioFavourite(rec);
                        else
                            da.UpdateVisioFavourite(rec);
                    }
                }
                VisioFavourites delList = new VisioFavourites();
                foreach (VisioFavourite rec in this)
                {
                    if (rec.DeleteRecord)
                        delList.Add(rec);
                }
                foreach (VisioFavourite rec in delList)
                {
                    this.Remove(rec);
                }
            }
        }
    }

    public class VisioMachines : List<VisioMachine>, IDataFiller, INotifyCollectionChanged
    {
        #region INotifyCollectionChanged Members

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public void Reset()
        {
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        new public void Add(VisioMachine vM)
        {
            base.Add(vM);
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, vM));
            }
        }

        new public void Remove(VisioMachine vM)
        {
            base.RemoveAt(this.IndexOf(vM));
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, vM));
            }
        }
        #endregion

        # region properties
        private double lifespan = 1.0;
        private string tblName = "tblVisioMachines";
        private DateTime lastDBUpdate;
        public double Lifespan
        {
            get { return lifespan; }
            set { lifespan = value; }
        }
        private DateTime lastRead;
        public DateTime LastRead
        {
            get { return lastRead; }
            set { lastRead = value; }
        }
        private bool neverExpire = false;
        public bool NeverExpire
        {
            get { return neverExpire; }
            set { neverExpire = value; }
        }
        private bool isValid = false;
        public bool IsValid
        {
            get
            {
                bool test = isValid && (this.Count > 0) && (lastRead != null) && (!neverExpire);
                if (test)
                {
                    SqlDataAccess da = SqlDataAccess.Singleton;
                    lastDBUpdate = da.TableLastUpdated(tblName);
                    int x = lastDBUpdate.CompareTo(lastRead.AddSeconds(0.95));
                    test = (x <= 0);
                    if (test)
                    {
                        DateTime testTime = lastRead.AddHours(lifespan);
                        test = testTime > da.ServerTime;
                    }
                }
                return test || neverExpire;
            }
            set
            {
                isValid = value;
                if (!isValid)
                    neverExpire = false;
            }
        }
        private bool forceUpdate = false;
        [XmlIgnore]
        public bool ForceUpdate
        {
            get { return forceUpdate; }
            set { forceUpdate = value; }
        }
        #endregion

        #region fill
        public int Fill(SqlDataReader dr)
        {
            SqlDataAccess da = SqlDataAccess.Singleton;
            int VisioMachineIDPos = dr.GetOrdinal("VisioMachineID");
            int VisioPositionIDPos = dr.GetOrdinal("VisioPositionID");
            int MachineIDPos = dr.GetOrdinal("MachineID");
            int MachineShapePos = dr.GetOrdinal("MachineShape");
            int DiagramIDPos = dr.GetOrdinal("DiagramID");
            int XPos = dr.GetOrdinal("X");
            int YPos = dr.GetOrdinal("Y");
            int WidthPos = dr.GetOrdinal("Width");
            int HeightPos = dr.GetOrdinal("Height");
            int ScalePos = dr.GetOrdinal("Scale");
            int RotationPos = dr.GetOrdinal("Rotation");
            int MirrorPos = dr.GetOrdinal("Mirror");

            this.Clear();
            while (dr.Read())
            {
                VisioMachine vM = new VisioMachine()
                {
                    VisioMachineID = dr.GetInt32(VisioMachineIDPos),
                    VisioPositionID = dr.GetInt32(VisioPositionIDPos),
                    MachineID = dr.GetInt32(MachineIDPos),
                    MachineShape = dr.GetInt32(MachineShapePos),
                    DiagramID = dr.GetInt32(DiagramIDPos),
                    X = (double)dr.GetDecimal(XPos),
                    Y = (double)dr.GetDecimal(YPos),
                    Width = dr.GetInt32(WidthPos),
                    Height = dr.GetInt32(HeightPos),
                    Scale = (double)dr.GetDecimal(ScalePos),
                    Rotation = dr.GetInt32(RotationPos),
                    Mirror = dr.GetBoolean(MirrorPos),
                    HasChanged = false
                };

                this.Add(vM);
            }
            LastRead = da.ServerTime;
            IsValid = true;

            return this.Count;
        }

        #endregion

        public int GetNextID()
        {
            int id = 0;
            bool found;
            {
                do
                {
                    id++;
                    found = false;
                    foreach (VisioMachine rec in this)
                    {
                        found |= (rec.ID == id);
                    }
                } while (found);
            }
            return id;
        }

        public VisioMachine GetById(int id)
        {
            return this.Find(delegate(VisioMachine visioMachine)
            {
                return visioMachine.VisioMachineID == id;
            });
        }

        public void UpdateToDB()
        {
            SqlDataAccess da = SqlDataAccess.Singleton;
            if (ForceUpdate)
            {
                da.TruncateGroupTable(tblName);
                foreach (VisioMachine rec in this)
                {
                    if (!rec.DeleteRecord)
                        da.InsertNewVisioMachine(rec);
                }
            }
            else
            {
                foreach (VisioMachine rec in this)
                {
                    if (rec.IsNew)
                    {
                        if (!rec.DeleteRecord)
                            da.InsertNewVisioMachine(rec);
                    }
                    else
                    {
                        if (rec.HasChanged)
                        {
                            if (rec.DeleteRecord)
                                da.DeleteVisioMachine(rec);
                            else
                                da.UpdateVisioMachine(rec);
                        }
                    }
                }
                VisioMachines delList = new VisioMachines();
                foreach (VisioMachine rec in this)
                {
                    if (rec.DeleteRecord)
                        delList.Add(rec);
                }
                foreach (VisioMachine rec in delList)
                {
                    this.Remove(rec);
                }
            }
        }
    }

    public class VisioStations : List<VisioStation>, IDataFiller, INotifyCollectionChanged
    {
        #region INotifyCollectionChanged Members

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public void Reset()
        {
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        new public void Add(VisioStation vStn)
        {
            base.Add(vStn);
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, vStn));
            }
        }

        new public void Remove(VisioStation vStn)
        {
            base.RemoveAt(this.IndexOf(vStn));
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, vStn));
            }
        }
        #endregion

        private double lifespan = 1.0;
        private string tblName = "tblVisioStations";
        private DateTime lastDBUpdate;
        public double Lifespan
        {
            get { return lifespan; }
            set { lifespan = value; }
        }
        private DateTime lastRead;
        public DateTime LastRead
        {
            get { return lastRead; }
            set { lastRead = value; }
        }
        private bool neverExpire = false;
        public bool NeverExpire
        {
            get { return neverExpire; }
            set { neverExpire = value; }
        }
        private bool isValid = false;
        public bool IsValid
        {
            get
            {
                bool test = isValid && (this.Count > 0) && (lastRead != null) && (!neverExpire);
                if (test)
                {
                    SqlDataAccess da = SqlDataAccess.Singleton;
                    lastDBUpdate = da.TableLastUpdated(tblName);
                    int x = lastDBUpdate.CompareTo(lastRead.AddSeconds(0.95));
                    test = (x <= 0);
                    if (test)
                    {
                        DateTime testTime = lastRead.AddHours(lifespan);
                        test = testTime > da.ServerTime;
                    }
                }
                return test || neverExpire;
            }
            set
            {
                isValid = value;
                if (!isValid)
                    neverExpire = false;
            }
        }
        private bool forceUpdate = false;
        [XmlIgnore]
        public bool ForceUpdate
        {
            get { return forceUpdate; }
            set { forceUpdate = value; }
        } 
        public int Fill(SqlDataReader dr)
        {
            SqlDataAccess da = SqlDataAccess.Singleton;
            int StationIDPos = dr.GetOrdinal("StationID");
            int VisioPositionIDPos = dr.GetOrdinal("VisioPositionID");
            int MachineIDPos = dr.GetOrdinal("MachineID");
            int SubIDPos = dr.GetOrdinal("SubID");
            int DiagramIDPos = dr.GetOrdinal("DiagramID");
            int XPos = dr.GetOrdinal("X");
            int YPos = dr.GetOrdinal("Y");
            int WidthPos = dr.GetOrdinal("Width");
            int HeightPos = dr.GetOrdinal("Height");
            int ScalePos = dr.GetOrdinal("Scale");
            int RotationPos = dr.GetOrdinal("Rotation");
            int MirrorPos = dr.GetOrdinal("Mirror");

            this.Clear();
            while (dr.Read())
            {
                VisioStation visioText = new VisioStation()
                {
                    StationID = dr.GetInt32(StationIDPos),
                    VisioPositionID = dr.GetInt32(VisioPositionIDPos),
                    MachineID = dr.GetInt32(MachineIDPos),
                    SubID = dr.GetInt32(SubIDPos),
                    DiagramID = dr.GetInt32(DiagramIDPos),
                    X = (double)dr.GetDecimal(XPos),
                    Y = (double)dr.GetDecimal(YPos),
                    Width = dr.GetInt32(WidthPos),
                    Height = dr.GetInt32(HeightPos),
                    Scale = (double)dr.GetDecimal(ScalePos),
                    Rotation = dr.GetInt32(RotationPos),
                    Mirror = dr.GetBoolean(MirrorPos),
                    HasChanged = false
                };

                this.Add(visioText);
            }
            LastRead = da.ServerTime;
            IsValid = true;

            return this.Count;
        }

        public int GetNextID()
        {
            int id = 0;
            bool found;
            {
                do
                {
                    id++;
                    found = false;
                    foreach (VisioStation rec in this)
                    {
                        found |= (rec.ID == id);
                    }
                } while (found);
            }
            return id;
        }

        public VisioStation GetById(int id)
        {
            return this.Find(delegate(VisioStation visioStation)
            {
                return visioStation.StationID == id;
            });
        }

        public void UpdateToDB()
        {
            SqlDataAccess da = SqlDataAccess.Singleton;
            if (ForceUpdate)
            {
                da.TruncateGroupTable(tblName);
                foreach (VisioStation rec in this)
                {
                    if (!rec.DeleteRecord)
                        da.InsertNewVisioStation(rec);
                }
            }
            else
            {
                foreach (VisioStation rec in this)
                {
                    if (rec.IsNew)
                    {
                        if (!rec.DeleteRecord)
                            da.InsertNewVisioStation(rec);
                    }
                    else
                    {
                        if (rec.HasChanged)
                        {
                            if (rec.DeleteRecord)
                                da.DeleteVisioStation(rec);
                            else
                                da.UpdateVisioStation(rec);
                        }
                    }
                }
                VisioStations delList = new VisioStations();
                foreach (VisioStation rec in this)
                {
                    if (rec.DeleteRecord)
                        delList.Add(rec);
                }
                foreach (VisioStation rec in delList)
                {
                    this.Remove(rec);
                }
            }
        }
    }

    public class VisioShapes : List<VisioShape>
    {
        public VisioShape GetById(int id)
        {
            return this.Find(delegate(VisioShape visioShape)
            {
                return visioShape.ShapeID == id;
            });
        }

        public VisioShape GetByName(string aName)
        {
            return this.Find(delegate(VisioShape visioShape)
            {
                return visioShape.ShapeName == aName;
            });
        }
    }

    #endregion

    #region Item Classes

    public class VisioDiagram : DataItem, INotifyPropertyChanged
    {        
        #region VisioDiagram Data Record
        protected class DataRecord : ICopyableObject
        {
            internal int DiagramID;
            internal string Title;
            internal string Description;
            internal int Width;
            internal int Height;
            internal bool DeleteRecord;

            public ICopyableObject ShallowCopy()
            {
                return (ICopyableObject)MemberwiseClone();
            }
        }
        #endregion

        #region Constructor
        public VisioDiagram()
        {
            ActiveData = (ICopyableObject)new DataRecord();
            activeData.DeleteRecord = false;
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
 
        #region Data Column Properties

        public int DiagramID
        {
            get
            {
                return this.activeData.DiagramID;
            }
            set
            {
                if (this.activeData.DiagramID != value)
                {
                    this.activeData.DiagramID = value;
                    NotifyPropertyChanged("DiagramID");
                }
            }
        }

        public override int ID
        {
            get
            {
                return DiagramID;
            }
            set
            {
                if (DiagramID != value)
                {
                    DiagramID = value;
                    NotifyPropertyChanged("ID");
                }
            }
        }

        public string Title
        {
            get
            {
                return this.activeData.Title;
            }
            set
            {
                if (this.activeData.Title != value)
                {
                    this.activeData.Title = value;
                    NotifyPropertyChanged("Title");
                }
            }
        }

        public string DiagramTitle
        {
            get
            {
                return Title;
            }
            set
            {
                if (Title != value)
                {
                    Title = value;
                    NotifyPropertyChanged("DiagramTitle");
                }
            }
        }

        public string Description
        {
            get
            {
                return this.activeData.Description;
            }
            set
            {
                if (this.activeData.Description != value)
                {
                    this.activeData.Description = value;
                    NotifyPropertyChanged("Description");
                }
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
                if (this.activeData.Width != value)
                {
                    this.activeData.Width = value;
                    NotifyPropertyChanged("Width");
                }
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
                if (this.activeData.Height != value)
                {
                    this.activeData.Height = value;
                    NotifyPropertyChanged("Height");
                }
            }
        }

        public bool DeleteRecord
        {
            get
            {
                return this.activeData.DeleteRecord;
            }
            set
            {
                if (this.activeData.DeleteRecord != value)
                {
                    this.activeData.DeleteRecord = value;
                    NotifyPropertyChanged("DeleteRecord");
                }
            }
        }

        #endregion
  
    }

    public class VisioBitmap : DataItem, INotifyPropertyChanged
    {
        #region VisioBitmap Data Record
        protected class DataRecord : ICopyableObject
        {
            internal int BitmapID;
            internal byte[] DiagramBitmap;
            internal string Description;
            internal bool DeleteRecord;

            public ICopyableObject ShallowCopy()
            {
                return (ICopyableObject)MemberwiseClone();
            }
        }

        #endregion

        #region Constructor
        public VisioBitmap()
        {
            ActiveData = (ICopyableObject)new DataRecord();
            activeData.DeleteRecord = false;
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
 
        #region Data Column Properties

        public int BitmapID
        {
            get
            {
                return this.activeData.BitmapID;
            }
            set
            {
                if (this.activeData.BitmapID != value)
                {
                    this.activeData.BitmapID = value;
                    NotifyPropertyChanged("BitmapID");
                }
            }
        }

        public override int ID
        {
            get
            {
                return BitmapID;
            }
            set
            {
                if (BitmapID != value)
                {
                    BitmapID = value;
                    NotifyPropertyChanged("ID");
                }
            }
        }

        public string Description
        {
            get
            {
                return this.activeData.Description;
            }
            set
            {
                if (this.activeData.Description != value)
                {
                    this.activeData.Description = value;
                    NotifyPropertyChanged("Description");
                }
            }
        }

        public string BitmapDescription
        {
            get
            {
                return Description;
            }
        }

        public string IDandDescription
        {
            get
            {
                string aString = "(" + BitmapID.ToString() + ") " + Description;
                return aString;
            }
        }

        public byte[] DiagBmpBytes
        {
            get
            {
                return this.activeData.DiagramBitmap;
            }
            set
            {
                if (this.activeData.DiagramBitmap != value)
                {
                    this.activeData.DiagramBitmap = value;
                    NotifyPropertyChanged("DiagBmp");
                    NotifyPropertyChanged("DiagBmpBytes");
                }
            }
        }

        public BitmapSource DiagBmpSource
        {
            get
            {
                return BytesToBitmapSource(this.activeData.DiagramBitmap);
            }
        }

        private static void ResizeImage(byte[] imageBytes, int width, int height)
        {
            // decode the image to the requested width and height
            BitmapSource imageSource = BytesToImageSource(imageBytes, width, height);

            // encode the image using the original format
            byte[] encodedBytes = ImageSourceToBytes(imageSource);

        }

        private static BitmapSource BytesToImageSource(byte[] imageData)
        {
            //return BytesToImageSource(imageData, 0, 0);
            return BytesToBitmapSource(imageData);
        }

        private BitmapImage BytesToBitmapImage(Byte[] imageData)
        {
            if (imageData == null) return null;
            Stream stream = new MemoryStream(imageData);

            PngBitmapDecoder decoder = new PngBitmapDecoder(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            BitmapSource bitmapSource = decoder.Frames[0];

            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = stream;
            image.EndInit();
            return image;
        }

        private static BitmapSource BytesToBitmapSource(Byte[] imageData)
        {
            if (imageData == null) return null;
            Stream stream = new MemoryStream(imageData);

            PngBitmapDecoder decoder = new PngBitmapDecoder(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            BitmapSource bitmapSource = decoder.Frames[0];
            return bitmapSource;
        }

        private static BitmapSource BytesToImageSource(byte[] imageData, int decodePixelWidth, int decodePixelHeight)
        {
            if (imageData == null) return null;
            Stream stream = new MemoryStream(imageData);
            PngBitmapDecoder decoder = new PngBitmapDecoder(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            BitmapSource bitmapSource = decoder.Frames[0];
            return bitmapSource;
        }

        private static byte[] ImageSourceToBytes(ImageSource image)
        {
            string preferredFormat=".png";
            byte[] result = null;
            BitmapEncoder encoder = null;
            switch (preferredFormat.ToLower())
            {
                case ".jpg":
                case ".jpeg":
                    encoder = new JpegBitmapEncoder();
                    break;

                case ".bmp":
                    encoder = new BmpBitmapEncoder();
                    break;

                case ".png":
                    encoder = new PngBitmapEncoder();
                    break;

                case ".tif":
                case ".tiff":
                    encoder = new TiffBitmapEncoder();
                    break;

                case ".gif":
                    encoder = new GifBitmapEncoder();
                    break;

                case ".wmp":
                    encoder = new WmpBitmapEncoder();
                    break;
            }

            if (image is BitmapSource)
            {
                MemoryStream stream = new MemoryStream();
                encoder.Frames.Add(BitmapFrame.Create(image as BitmapSource));
                encoder.Save(stream);
                stream.Seek(0, SeekOrigin.Begin);
                result = new byte[stream.Length];
                BinaryReader br = new BinaryReader(stream);
                br.Read(result, 0, (int)stream.Length);
                br.Close();
                stream.Close();
            }
            return result;
        }

        public ImageSource DiagBmp
        {
            get
            {
                return BytesToImageSource(DiagBmpBytes);
            }
        }

        public bool DeleteRecord
        {
            get
            {
                return this.activeData.DeleteRecord;
            }
            set
            {
                if (this.activeData.DeleteRecord != value)
                {
                    this.activeData.DeleteRecord = value;
                    NotifyPropertyChanged("DeleteRecord");
                }
            }
        }

        #endregion

    }

    public class VisioDiagramBitmap : DataItem, INotifyPropertyChanged
    {
        #region VisioDiagramBitmap Data Record
        protected class DataRecord : ICopyableObject
        {
            internal int DiagBmpID;
            internal int BitmapID;
            internal int DiagramID;
            internal int ZIndex;
            internal bool DeleteRecord;

            public ICopyableObject ShallowCopy()
            {
                return (ICopyableObject)MemberwiseClone();
            }
        }

        #endregion

        #region Constructor
        public VisioDiagramBitmap()
        {
            ActiveData = (ICopyableObject)new DataRecord();
            activeData.DeleteRecord = false;
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
 
        #region Data Column Properties

        public int DiagBmpID
        {
            get
            {
                return this.activeData.DiagBmpID;
            }
            set
            {
                if (this.activeData.DiagBmpID != value)
                {
                    this.activeData.DiagBmpID = value;
                    NotifyPropertyChanged("DiagBmpID");
                }
            }
        }

        public override int ID
        {
            get
            {
                return DiagBmpID;
            }
            set
            {
                if (DiagBmpID != value)
                {
                    DiagBmpID = value;
                    NotifyPropertyChanged("ID");
                }
            }
        }

        public int BitmapID
        {
            get
            {
                return this.activeData.BitmapID;
            }
            set
            {
                if (this.activeData.BitmapID != value)
                {
                    this.activeData.BitmapID = value;
                    NotifyPropertyChanged("BitmapID");
                }
            }
        }

        public string BitmapDescription
        {
            get
            {
                string aString = string.Empty;
                SqlDataAccess da = SqlDataAccess.Singleton;
                VisioBitmaps vbs = da.GetAllVisioBitmaps();
                if (vbs != null)
                {
                    VisioBitmap vb = vbs.GetById(BitmapID);
                    if (vb != null)
                    {
                        aString = vb.Description;
                    }
                }
                return aString;
            }
            set
            {
                SetBitmapDescription(value);
                NotifyPropertyChanged("BitmapDescription");
                NotifyPropertyChanged("BitmapID");
            }
        }

        private void SetBitmapDescription(string bitmapDescription)
        {
            int id = -1;
            SqlDataAccess da = SqlDataAccess.Singleton;
            VisioBitmaps vbs = da.GetAllVisioBitmaps();
            if (vbs != null)
            {
                VisioBitmap vb = vbs.GetByName(bitmapDescription);
                if (vb != null)
                {
                    id = vb.BitmapID;
                }
            }
            BitmapID = id;
        }

        public int DiagramID
        {
            get
            {
                return this.activeData.DiagramID;
            }
            set
            {
                if (this.activeData.DiagramID != value)
                {
                    this.activeData.DiagramID = value;
                    NotifyPropertyChanged("DiagramID");
                }
            }
        }

        public string DiagramTitle
        {
            get
            {
                string aString = string.Empty;
                SqlDataAccess da = SqlDataAccess.Singleton;
                VisioDiagrams vds = da.GetAllVisioDiagrams();
                if (vds != null)
                {
                    VisioDiagram vd = vds.GetById(DiagramID);
                    if (vd != null)
                    {
                        aString = vd.Title;
                    }
                }
                return aString;
            }
            set
            {
                SetDiagramTitle(value);
                NotifyPropertyChanged("DiagramID");
                NotifyPropertyChanged("DiagramTitle");
            }
        }

        private void SetDiagramTitle(string diagramTitle)
        {
            int id = -1;                
            SqlDataAccess da = SqlDataAccess.Singleton;
                VisioDiagrams vds = da.GetAllVisioDiagrams();
                if (vds != null)
                {
                    VisioDiagram vd = vds.GetByName(diagramTitle);
                    if (vd != null)
                {
                    id = vd.DiagramID;
                }
            }
                DiagramID = id;
        }

        public int ZIndex
        {
            get
            {
                return this.activeData.ZIndex;
            }
            set
            {
                if (this.activeData.ZIndex != value)
                {
                    this.activeData.ZIndex = value;
                    NotifyPropertyChanged("ZIndex");
                }
            }
        }


        public bool DeleteRecord
        {
            get
            {
                return this.activeData.DeleteRecord;
            }
            set
            {
                if (this.activeData.DeleteRecord != value)
                {
                    this.activeData.DeleteRecord = value;
                    NotifyPropertyChanged("DeleteRecord");
                }
            }
        }

        #endregion

    }

    public interface iVisioPosition
    {
        int DiagramID { get; set; }
        double X { get; set; }
        double Y { get; set; }
        int Width { get; set; }
        int Height { get; set; }
        double Scale { get; set; }
        int Rotation { get; set; }
        bool Mirror { get; set; }
    }

    public class VisioPosition : DataItem, INotifyPropertyChanged, iVisioPosition
    {
        #region VisioPosition Data Record
        protected class DataRecord : ICopyableObject
        {
            internal int VisioPositionID;
            internal int DiagramID;
            internal double X;
            internal double Y;
            internal int Width;
            internal int Height;
            internal double Scale;
            internal int Rotation;
            internal bool Mirror;
            internal bool DeleteRecord;


            public ICopyableObject ShallowCopy()
            {
                return (ICopyableObject)MemberwiseClone();
            }
        }
        #endregion

        #region Constructor
        public VisioPosition()
        {
            ActiveData = (ICopyableObject)new DataRecord();
            activeData.Width = 1;
            activeData.Height = 1;
            activeData.Scale = 1;
            activeData.Rotation = 0;
            activeData.Mirror = false;
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

        #region Data Column Properties

        public int VisioPositionID
        {
            get
            {
                return activeData.VisioPositionID;
            }
            set
            {
                if (activeData.VisioPositionID != value)
                {
                    activeData.VisioPositionID = value;
                    NotifyPropertyChanged("VisioPositionID");
                }
            }
        }

        public override int ID
        {
            get
            {
                return VisioPositionID;
            }
            set
            {
                if (VisioPositionID != value)
                {
                    VisioPositionID = value;
                    NotifyPropertyChanged("ID");
                }
            }
        }

        public int DiagramID
        {
            get
            {
                return this.activeData.DiagramID;
            }
            set
            {
                if (activeData.DiagramID != value)
                {
                    activeData.DiagramID = value;
                    NotifyPropertyChanged("DiagramID");
                }
            }
        }

        public double X
        {
            get
            {
                return this.activeData.X;
            }
            set
            {
                if (activeData.X != value)
                {
                    activeData.X = value;
                    NotifyPropertyChanged("X");
                }
            }
        }

        public double Y
        {
            get
            {
                return this.activeData.Y;
            }
            set
            {
                if (activeData.Y != value)
                {
                    activeData.Y = value;
                    NotifyPropertyChanged("Y");
                }
            }
        }        
        
        public int IntY
        {
            get
            {
                return (int)activeData.Y;
            }
            set
            {
                activeData.Y = value;
            }
        }

        public int IntX
        {
            get
            {
                return (int)activeData.X;
            }
            set
            {
                activeData.X = value;
            }
        }

        public int Width
        {
            get
            {
                return activeData.Width;
            }
            set
            {
                if (activeData.Width != value)
                {
                    activeData.Width = value;
                    NotifyPropertyChanged("Width");
                }
            }
        }

        public int Height
        {
            get
            {
                return activeData.Height;
            }
            set
            {
                if (activeData.Height != value)
                {
                    activeData.Height = value;
                    NotifyPropertyChanged("Height");
                }
            }
        }

        public double Scale
        {
            get
            {
                return activeData.Scale;
            }
            set
            {
                if (activeData.Scale != value)
                {
                    activeData.Scale = value;
                    NotifyPropertyChanged("Scale");
                }
            }
        }

        public int Rotation
        {
            get
            {
                return activeData.Rotation;
            }
            set
            {
                if (activeData.Rotation != value)
                {
                    activeData.Rotation = value;
                    NotifyPropertyChanged("Rotation");
                }
            }
        }

        public bool Mirror
        {
            get
            {
                return activeData.Mirror;
            }
            set
            {
                if (activeData.Mirror != value)
                {
                    activeData.Mirror = value;
                    NotifyPropertyChanged("Mirror");
                }
            }
        }

        public bool DeleteRecord
        {
            get
            {
                return this.activeData.DeleteRecord;
            }
            set
            {
                if (this.activeData.DeleteRecord != value)
                {
                    this.activeData.DeleteRecord = value;
                    NotifyPropertyChanged("DeleteRecord");
                }
            }
        }

        #endregion

    }

    public class VisioText : DataItem, INotifyPropertyChanged, iVisioPosition
    {
        #region VisioText Data Record
        protected class DataRecord : ICopyableObject
        {
            internal int VisioTextID;
            internal int PseudoID;
            internal int VisioPositionID;
            internal bool IsDynamic;
            internal string StaticText;
            internal string SQLCommand;
            internal int RefreshRate;
            internal bool Enabled;
            internal int DiagramID;
            internal double X;
            internal double Y;
            internal int Width;
            internal int Height;
            internal double Scale;
            internal int Rotation;
            internal bool Mirror;
            internal bool DeleteRecord;
            
            public ICopyableObject ShallowCopy()
            {
                return (ICopyableObject)MemberwiseClone();
            }
        }
        #endregion   

        #region Constructor
        public VisioText()
        {
            ActiveData = (ICopyableObject)new DataRecord();
            activeData.DeleteRecord = false;
            activeData.PseudoID = (int)DateTime.Now.Ticks % 10000000;
            if (activeData.PseudoID < 0) activeData.PseudoID *= -1; 
            lastEvaluation = DateTime.Now;
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

        #region Data Column Properties

        public int VisioTextID
        {
            get
            {
                return this.activeData.VisioTextID;
            }
            set
            {
                if (activeData.VisioTextID != value)
                {
                    activeData.VisioTextID = value;
                    NotifyPropertyChanged("VisioTextID");
                } 
            }
        }

        public override int ID
        {
            get
            {
                return VisioTextID;
            }
            set
            {
                if (VisioTextID != value)
                {
                    VisioTextID = value;
                    NotifyPropertyChanged("ID");
                }
            }
        }
        
        public bool IsDynamic
        {
            get
            {
                return this.activeData.IsDynamic;
            }
            set
            {
                if (activeData.IsDynamic != value)
                {
                    activeData.IsDynamic = value;
                    NotifyPropertyChanged("IsDynamic");
                }
            }
        }

        public bool Enabled
        {
            get
            {
                return this.activeData.Enabled;
            }
            set
            {
                if (activeData.Enabled != value)
                {
                    activeData.Enabled = value;
                    NotifyPropertyChanged("Enabled");
                }
            }
        }

        public string DynamicText
        {
            get
            {
                string aString = StaticText.Replace("$1", smartResult);
                return aString;
            }
        }

        public string StaticText
        {
            get
            {
                return this.activeData.StaticText;
            }
            set
            {
                if (activeData.StaticText != value)
                {
                    activeData.StaticText = value;
                    NotifyPropertyChanged("StaticText");
                    Width = 20 * value.Length;
                }
            }
        }

        public string SQLCommand
        {
            get
            {
                return this.activeData.SQLCommand;
            }
            set
            {
                if (activeData.SQLCommand != value)
                {
                    activeData.SQLCommand = value;
                    NotifyPropertyChanged("SQLCommand");
                }
            }
        }

        public int RefreshRate
        {
            get
            {
                return this.activeData.RefreshRate;
            }
            set
            {
                if (activeData.RefreshRate != value)
                {
                    activeData.RefreshRate = value;
                    NotifyPropertyChanged("DiagramID");
                }

            }
        }

        public int VisioPositionID
        {
            get
            {
                return this.activeData.VisioPositionID;
            }
            set
            {
                if (this.activeData.VisioPositionID != value)
                {
                    this.activeData.VisioPositionID = value;
                    NotifyPropertyChanged("VisioPositionID");
                }
            }
        }

        public int DiagramID
        {
            get
            {
                return this.activeData.DiagramID;
            }
            set
            {
                if (this.activeData.DiagramID != value)
                {
                    this.activeData.DiagramID = value;
                    NotifyPropertyChanged("DiagramID");
                }
            }
        }

        public string DiagramTitle
        {
            get
            {
                string aString = string.Empty;
                SqlDataAccess da = SqlDataAccess.Singleton;
                VisioDiagrams vds = da.GetAllVisioDiagrams();
                if (vds != null)
                {
                    VisioDiagram vd = vds.GetById(DiagramID);
                    if (vd != null)
                    {
                        aString = vd.Title;
                    }
                }
                return aString;
            }
            set
            {
                SetDiagramTitle(value);
                NotifyPropertyChanged("DiagramID");
                NotifyPropertyChanged("DiagramTitle");
            }
        }

        private void SetDiagramTitle(string diagramTitle)
        {
            int id = -1;
            SqlDataAccess da = SqlDataAccess.Singleton;
            VisioDiagrams vds = da.GetAllVisioDiagrams();
            if (vds != null)
            {
                VisioDiagram vd = vds.GetByName(diagramTitle);
                if (vd != null)
                {
                    id = vd.DiagramID;
                }
            }
            DiagramID = id;
        }

        public double X
        {
            get
            {
                return this.activeData.X;
            }
            set
            {
                if (this.activeData.X != value)
                {
                    this.activeData.X = Math.Round(value, 1);
                    NotifyPropertyChanged("X");
                }
            }
        }

        public double Y
        {
            get
            {
                return this.activeData.Y;
            }
            set
            {
                if (this.activeData.Y != value)
                {
                    this.activeData.Y = Math.Round(value, 1);
                    NotifyPropertyChanged("Y");
                }
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
                if (this.activeData.Width != value)
                {
                    this.activeData.Width = value;
                    NotifyPropertyChanged("Width");
                }
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
                if (this.activeData.Height != value)
                {
                    this.activeData.Height = value;
                    NotifyPropertyChanged("Height");
                }
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
                if (this.activeData.Scale != value)
                {
                    this.activeData.Scale = Math.Round(value, 3);
                    NotifyPropertyChanged("Scale");
                }
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
                if (this.activeData.Rotation != value)
                {
                    this.activeData.Rotation = value;
                    NotifyPropertyChanged("Rotation");
                }
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
                if (this.activeData.Mirror != value)
                {
                    this.activeData.Mirror = value;
                    NotifyPropertyChanged("Mirror");
                }
            }
        }

        public bool DeleteRecord
        {
            get
            {
                return this.activeData.DeleteRecord;
            }
            set
            {
                if (this.activeData.DeleteRecord != value)
                {
                    this.activeData.DeleteRecord = value;
                    NotifyPropertyChanged("DeleteRecord");
                }
            }
        } 

 
        #endregion

        #region smart

        private DateTime lastEvaluation;

        private bool EvalTime(DateTime evalTime)
        {
            bool test = true;
            if (lastEvaluation != null)
            {
                if (lastEvaluation.AddSeconds(RefreshRate) > evalTime)
                {
                    test = false;
                }
            } 
            return test;
        }

        private string smartResult = " ";
        private string lastResult = string.Empty;
        private bool newResult = false;

        public bool NewResult
        {
            get { return newResult && Enabled; }
            set { newResult = value; }
        }

        public void EvaluateSQL()
        {
            if (IsDynamic && Enabled)
            {
                try
                {
                    DateTime evalTime = DateTime.Now;
                    if (EvalTime(evalTime))
                    {

                        SqlDataAccess da = SqlDataAccess.Singleton;
                        SqlCommand cmd = new SqlCommand(SQLCommand);
                        object obj = SqlClientExtension.ExecuteScalar(cmd, SqlDataConnection.DBConnection.JensenGroup);
                        if (obj != null)
                        {
                            if (obj is int)
                            {
                                int x = (int)obj;
                                smartResult = x.ToString();
                            }
                            if (obj is short)
                            {
                                short x = (short)obj;
                                smartResult = x.ToString();
                            }
                            if (obj is bool)
                            {
                                bool x = (bool)obj;
                                if (x)
                                    smartResult = "True";
                                else
                                    smartResult = "False";
                            }
                            if (obj is string)
                                smartResult = (string)obj;
                        }
                        lastEvaluation = evalTime;
                        newResult = (smartResult != lastResult);
                        lastResult = smartResult;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
        }

        public string SmartResult
        {
            get
            {
                return smartResult;
            }
        }

        #endregion

    }

    public class VisioFavourite : DataItem, INotifyPropertyChanged
    {
        #region VisioFavourite Data Record
        protected class DataRecord : ICopyableObject
        {
            internal int FavouriteID;
            internal int DiagramID;
            internal int AppID;
            internal int AppUserID;
            internal string FavouriteName;
            internal double xMin;
            internal double yMin;
            internal double yMax;
            internal double xMax;
            internal bool DeleteRecord;

            public ICopyableObject ShallowCopy()
            {
                return (ICopyableObject)MemberwiseClone();
            }
        }
        #endregion

        #region Constructor
        public VisioFavourite()
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

        #region Data Column Properties

        public int FavouriteID
        {
            get
            {
                return this.activeData.FavouriteID;
            }
            set
            {
                if (this.activeData.FavouriteID != value)
                {
                    this.activeData.FavouriteID = value;
                    NotifyPropertyChanged("FavouriteID");
                }
            }
        }

        public override int ID
        {
            get
            {
                return FavouriteID;
            }
            set
            {
                if (FavouriteID != value)
                {
                    FavouriteID = value;
                    NotifyPropertyChanged("ID");
                }
            }
        }
        
        public int DiagramID
        {
            get
            {
                return this.activeData.DiagramID;
            }
            set
            {
                if (this.activeData.DiagramID != value)
                {
                    this.activeData.DiagramID = value;
                    NotifyPropertyChanged("DiagramID");
                }
            }
        }

        public string DiagramTitle
        {
            get
            {
                return GetDiagramName(DiagramID);
            }
            set
            {
                SetDiagramID(value);
                NotifyPropertyChanged("DiagramTitle");
            }
        }

        private string GetDiagramName(int id)
        {
            string aString = string.Empty;
            SqlDataAccess da = SqlDataAccess.Singleton;
            VisioDiagrams vds = da.GetAllVisioDiagrams();
            if (vds != null)
            {
                VisioDiagram vd = vds.GetById(id);
                if (vd != null)
                {
                    aString = vd.Title;
                }
            }
            return aString;
        }

        private void SetDiagramID(string aName)
        {
            int id = -1;
            SqlDataAccess da = SqlDataAccess.Singleton;
            VisioDiagrams vds = da.GetAllVisioDiagrams();
            if (vds != null)
            {
                VisioDiagram vd = vds.GetByName(aName);
                if (vd != null)
                {
                    id = vd.DiagramID;
                }
            }
            DiagramID = id;
        }

        public int AppUserID
        {
            get
            {
                return this.activeData.AppUserID;
            }
            set
            {
                if (this.activeData.AppUserID != value)
                {
                    this.activeData.AppUserID = value;
                    NotifyPropertyChanged("AppUserID");
                }
            }
        }


        public string UserName
        {
            get
            {
                return GetUserName(AppUserID);
            }
            set
            {
                SetUserID(value);
                NotifyPropertyChanged("UserName");
            }
        }

        private string GetUserName(int id)
        {
            string aString = string.Empty;
            SqlDataAccess da = SqlDataAccess.Singleton;
            AppUsers aps = da.GetAllAppUsers();
            if (aps != null)
            {
                AppUser a = aps.GetById(id);
                if (a != null)
                {
                    aString = a.UserName;
                }
            }
            return aString;
        }

        private void SetUserID(string aName)
        {
            int id = -1;
            SqlDataAccess da = SqlDataAccess.Singleton;
            AppUsers aps = da.GetAllAppUsers();
            if (aps != null)
            {
                AppUser a = aps.GetByName(aName);
                if (a != null)
                {
                    id = a.AppUserID;
                }
            }
            AppUserID = id;
        }

        public int AppID
        {
            get
            {
                return this.activeData.AppID;
            }
            set
            {
                if (this.activeData.AppID != value)
                {
                    this.activeData.AppID = value;
                    NotifyPropertyChanged("AppID");
                }
            }
        }

        public string AppDesc
        {
            get
            {
                return GetAppName(AppID);
            }
            set
            {
                SetAppID(value);
                NotifyPropertyChanged("AppDesc");            
            }
        }

        private string GetAppName(int id)
        {
            string aString = string.Empty;
            SqlDataAccess da = SqlDataAccess.Singleton;
            Apps aps = da.GetAllApps();
            if (aps != null)
            {
                App a = aps.GetById(id);
                if (a != null)
                {
                    aString = a.AppDesc;
                }
            }
            return aString;
        }

        private void SetAppID(string aName)
        {
            int id = -1;
            SqlDataAccess da = SqlDataAccess.Singleton;
            Apps aps = da.GetAllApps();
            if (aps != null)
            {
                App a = aps.GetByDesc(aName);
                if (a != null)
                {
                    id = a.AppID;
                }
            }
            AppID = id;
        }

        public string FavouriteName
        {
            get
            {
                return this.activeData.FavouriteName;
            }
            set
            {
                if (this.activeData.FavouriteName != value)
                {
                    this.activeData.FavouriteName = value;
                    NotifyPropertyChanged("FavouriteName");
                }
            }
        }

        //public double Scale
        //{
        //    get
        //    {
        //        return this.activeData.Scale;
        //    }
        //    set
        //    {
        //        this.activeData.Scale = value;
        //        HasChanged = HasChanged || this.activeData.Scale != value;
        //    }
        //}

        public double XMin
        {
            get
            {
                return this.activeData.xMin;
            }
            set
            {
                if (this.activeData.xMin != value)
                {
                    this.activeData.xMin = value;
                    NotifyPropertyChanged("xMin");
                }
            }
        }

        public double YMin
        {
            get
            {
                return this.activeData.yMin;
            }
            set
            {
                if (this.activeData.yMin != value)
                {
                    this.activeData.yMin = value;
                    NotifyPropertyChanged("yMin");
                }
            }
        }

        public double XMax
        {
            get
            {
                return this.activeData.xMax;
            }
            set
            {
                if (this.activeData.xMax != value)
                {
                    this.activeData.xMax = value;
                    NotifyPropertyChanged("xMax");
                }
            }
        }

        public double YMax
        {
            get
            {
                return this.activeData.yMax;
            }
            set
            {
                if (this.activeData.yMax != value)
                {
                    this.activeData.yMax = value;
                    NotifyPropertyChanged("yMax");
                }
            }
        }

        public bool DeleteRecord
        {
            get
            {
                return this.activeData.DeleteRecord;
            }
            set
            {
                if (this.activeData.DeleteRecord != value)
                {
                    this.activeData.DeleteRecord = value;
                    NotifyPropertyChanged("DeleteRecord");
                }
            }
        }

        #endregion

    }

    public class VisioMachine : DataItem, INotifyPropertyChanged, iVisioPosition
    {
        #region VisioMachine Data Record
        protected class DataRecord : ICopyableObject
        {
            internal int VisioMachineID;
            internal int PseudoID;
            internal int VisioPositionID;
            internal int MachineID;
            internal int MachineShape;
            internal int DiagramID;
            internal double X;
            internal double Y;
            internal int Width;
            internal int Height;
            internal double Scale;
            internal int Rotation;
            internal bool Mirror;
            internal bool DeleteRecord;

            public ICopyableObject ShallowCopy()
            {
                return (ICopyableObject)MemberwiseClone();
            }
        }
        #endregion

        #region Constructor
        public VisioMachine()
        {
            ActiveData = (ICopyableObject)new DataRecord();
            activeData.DeleteRecord = false;
            activeData.PseudoID = (int)DateTime.Now.Ticks % 10000000;
            if (activeData.PseudoID < 0) activeData.PseudoID *= -1;
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

        #region Data Column Properties

        public int VisioMachineID
        {
            get
            {
                return this.activeData.VisioMachineID;
            }
            set
            {
                this.activeData.VisioMachineID = value;
                NotifyPropertyChanged("VisioMachineID");
            }
        }

        public override int ID
        {
            get
            {
                return VisioMachineID;
            }
            set
            {
                if (VisioMachineID != value)
                {
                    VisioMachineID = value;
                    NotifyPropertyChanged("ID");
                }
            }
        }

        public int VisioPositionID
        {
            get
            {
                return this.activeData.VisioPositionID;
            }
            set
            {
                if (this.activeData.VisioPositionID != value)
                {
                    this.activeData.VisioPositionID = value;
                    NotifyPropertyChanged("VisioPositionID");
                }
            }
        }

        public int MachineID
        {
            get
            {
                return this.activeData.MachineID;
            }
            set
            {
                if (this.activeData.MachineID != value)
                {
                    this.activeData.MachineID = value;
                    NotifyPropertyChanged("MachineID");
                }
            }
        }

        public string MachineName
        {
            get
            {
                string aString = MachineID.ToString();
                SqlDataAccess da = SqlDataAccess.Singleton;
                Machines ms = da.GetAllMachines();
                if (ms != null)
                {
                    Machine m = ms.GetById(MachineID);
                    if (m != null)
                        aString = m.MachineName;
                }
                return aString;
            }
            set
            {
                SqlDataAccess da = SqlDataAccess.Singleton;
                Machines ms = da.GetAllMachines();
                if (ms != null)
                {
                    Machine m = ms.GetByNameID(value);
                    if (m != null)
                        MachineID = m.idJensen;
                }
            }
        }

        public int MachineShape
        {
            get
            {
                return this.activeData.MachineShape;
            }
            set
            {
                if (this.activeData.MachineShape != value)
                {
                    this.activeData.MachineShape = value;
                    NotifyPropertyChanged("MachineShape");
                }
            }
        }

        public string ShapeName
        {
            get
            {
                string aString = MachineShape.ToString();
                SqlDataAccess da = SqlDataAccess.Singleton;
                VisioShapes vs = da.GetVisioShapes();
                if (vs != null)
                {
                    VisioShape s = vs.GetById(MachineShape);
                    if (s != null)
                        aString = s.ShapeName;
                }
                return aString;
            }
            set
            {
                SqlDataAccess da = SqlDataAccess.Singleton;
                VisioShapes vs = da.GetVisioShapes();
                if (vs != null)
                {
                    VisioShape s = vs.GetByName(value);
                    if (s != null)
                        MachineShape = s.ShapeID;
                    NotifyPropertyChanged("ShapeName");
                }
            }
        }

        public int DiagramID
        {
            get
            {
                return this.activeData.DiagramID;
            }
            set
            {
                if (this.activeData.DiagramID != value)
                {
                    this.activeData.DiagramID = value;
                    NotifyPropertyChanged("DiagramID");
                }
            }
        }

        public string DiagramTitle
        {
            get
            {
                string aString = string.Empty;
                SqlDataAccess da = SqlDataAccess.Singleton;
                VisioDiagrams vds = da.GetAllVisioDiagrams();
                if (vds != null)
                {
                    VisioDiagram vd = vds.GetById(DiagramID);
                    if (vd != null)
                    {
                        aString = vd.Title;
                    }
                }
                return aString;
            }
            set
            {
                SetDiagramTitle(value);
                NotifyPropertyChanged("DiagramID");
                NotifyPropertyChanged("DiagramTitle");
            }
        }

        private void SetDiagramTitle(string diagramTitle)
        {
            int id = -1;
            SqlDataAccess da = SqlDataAccess.Singleton;
            VisioDiagrams vds = da.GetAllVisioDiagrams();
            if (vds != null)
            {
                VisioDiagram vd = vds.GetByName(diagramTitle);
                if (vd != null)
                {
                    id = vd.DiagramID;
                }
            }
            DiagramID = id;
        }

        public double X
        {
            get
            {
                return this.activeData.X;
            }
            set
            {
                if (this.activeData.X != value)
                {
                    this.activeData.X = Math.Round(value, 1);
                    NotifyPropertyChanged("X");
                }
            }
        }

        public double Y
        {
            get
            {
                return this.activeData.Y;
            }
            set
            {
                if (this.activeData.Y != value)
                {
                    this.activeData.Y = Math.Round(value, 1);
                    NotifyPropertyChanged("Y");
                }
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
                if (this.activeData.Width != value)
                {
                    this.activeData.Width = value;
                    NotifyPropertyChanged("Width");
                } 
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
                if (this.activeData.Height != value)
                {
                    this.activeData.Height = value;
                    NotifyPropertyChanged("Height");
                }
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
                if (this.activeData.Scale != value)
                {
                    this.activeData.Scale = Math.Round(value, 3);
                    NotifyPropertyChanged("Scale");
                }
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
                if (this.activeData.Rotation != value)
                {
                    this.activeData.Rotation = value;
                    NotifyPropertyChanged("Rotation");
                }
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
                if (this.activeData.Mirror != value)
                {
                    this.activeData.Mirror = value;
                    NotifyPropertyChanged("Mirror");
                }
            }
        }

        public bool DeleteRecord
        {
            get
            {
                return this.activeData.DeleteRecord;
            }
            set
            {
                if (this.activeData.DeleteRecord != value)
                {
                    this.activeData.DeleteRecord = value;
                    NotifyPropertyChanged("DeleteRecord");
                }
            }
        } 

        #endregion

    }

    public class VisioStation : DataItem, INotifyPropertyChanged, iVisioPosition
    {
        #region VisioStations Data Record
        protected class DataRecord : ICopyableObject
        {
            internal int StationID;
            internal int VisioPositionID;
            internal int MachineID;
            internal int SubID;
            internal int DiagramID;
            internal double X;
            internal double Y;
            internal int Width;
            internal int Height;
            internal double Scale;
            internal int Rotation;
            internal bool Mirror;
            internal bool DeleteRecord;

            public ICopyableObject ShallowCopy()
            {
                return (ICopyableObject)MemberwiseClone();
            }
        }
        #endregion

        #region Constructor
        public VisioStation()
        {
            ActiveData = (ICopyableObject)new DataRecord();
            activeData.DeleteRecord = false;
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

        #region Data Column Properties

        public int StationID
        {
            get
            {
                return this.activeData.StationID;
            }
            set
            {
                this.activeData.StationID = value;
                NotifyPropertyChanged("StationID");
            }
        }

        public override int ID
        {
            get
            {
                return StationID;
            }
            set
            {
                if (StationID != value)
                {
                    StationID = value;
                    NotifyPropertyChanged("ID");
                }
            }
        }

        public int VisioPositionID
        {
            get
            {
                return this.activeData.VisioPositionID;
            }
            set
            {
                if (this.activeData.VisioPositionID != value)
                {
                    this.activeData.VisioPositionID = value;
                    NotifyPropertyChanged("VisioPositionID");
                }
            }
        }

        public int MachineID
        {
            get
            {
                return this.activeData.MachineID;
            }
            set
            {
                if (this.activeData.MachineID != value)
                {
                    this.activeData.MachineID = value;
                    NotifyPropertyChanged("MachineID");
                }
            }
        }


        public string MachineName
        {
            get
            {
                string aString = MachineID.ToString();
                SqlDataAccess da = SqlDataAccess.Singleton;
                Machines ms = da.GetAllMachines();
                if (ms != null)
                {
                    Machine m = ms.GetById(MachineID);
                    if (m != null)
                        aString = m.MachineName;
                }
                return aString;
            }
            set
            {
                SqlDataAccess da = SqlDataAccess.Singleton;
                Machines ms = da.GetAllMachines();
                if (ms != null)
                {
                    Machine m = ms.GetByNameID(value);
                    if (m != null)
                        MachineID = m.idJensen;
                }
            }
        }
        
        public int SubID
        {
            get
            {
                return this.activeData.SubID;
            }
            set
            {
                if (this.activeData.SubID != value)
                {
                    this.activeData.SubID = value;
                    NotifyPropertyChanged("SubID");
                }
            }
        }

        public int DiagramID
        {
            get
            {
                return this.activeData.DiagramID;
            }
            set
            {
                if (this.activeData.DiagramID != value)
                {
                    this.activeData.DiagramID = value;
                    NotifyPropertyChanged("DiagramID");
                }
            }
        }

        public string DiagramTitle
        {
            get
            {
                string aString = string.Empty;
                SqlDataAccess da = SqlDataAccess.Singleton;
                VisioDiagrams vds = da.GetAllVisioDiagrams();
                if (vds != null)
                {
                    VisioDiagram vd = vds.GetById(DiagramID);
                    if (vd != null)
                    {
                        aString = vd.Title;
                    }
                }
                return aString;
            }
            set
            {
                SetDiagramTitle(value);
                NotifyPropertyChanged("DiagramID");
                NotifyPropertyChanged("DiagramTitle");
            }
        }

        private void SetDiagramTitle(string diagramTitle)
        {
            int id = -1;
            SqlDataAccess da = SqlDataAccess.Singleton;
            VisioDiagrams vds = da.GetAllVisioDiagrams();
            if (vds != null)
            {
                VisioDiagram vd = vds.GetByName(diagramTitle);
                if (vd != null)
                {
                    id = vd.DiagramID;
                }
            }
            DiagramID = id;
        }
        public double X
        {
            get
            {
                return this.activeData.X;
            }
            set
            {
                if (this.activeData.X != value)
                {
                    this.activeData.X = Math.Round(value, 1);
                    NotifyPropertyChanged("X");
                }
            }
        }

        public double Y
        {
            get
            {
                return this.activeData.Y;
            }
            set
            {
                if (this.activeData.Y != value)
                {
                    this.activeData.Y = Math.Round(value, 1);
                    NotifyPropertyChanged("Y");
                }
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
                if (this.activeData.Width != value)
                {
                    this.activeData.Width = value;
                    NotifyPropertyChanged("Width");
                }
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
                if (this.activeData.Height != value)
                {
                    this.activeData.Height = value;
                    NotifyPropertyChanged("Height");
                }
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
                if (this.activeData.Scale != value)
                {
                    this.activeData.Scale = Math.Round(value, 3);
                    NotifyPropertyChanged("Scale");
                }
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
                if (this.activeData.Rotation != value)
                {
                    this.activeData.Rotation = value;
                    NotifyPropertyChanged("Rotation");
                }
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
                if (this.activeData.Mirror != value)
                {
                    this.activeData.Mirror = value;
                    NotifyPropertyChanged("Mirror");
                }
            }
        }

        public bool DeleteRecord
        {
            get
            {
                return this.activeData.DeleteRecord;
            }
            set
            {
                if (this.activeData.DeleteRecord != value)
                {
                    this.activeData.DeleteRecord = value;
                    NotifyPropertyChanged("DeleteRecord");
                }
            }
        }

        #endregion

    }

    public class VisioShape : INotifyPropertyChanged
    {
        private int shapeID;

        public int ShapeID
        {
            get { return shapeID; }
            set
            {
                shapeID = value;
                NotifyPropertyChanged("ShapeID");
            }
        }
        private string shapeName;

        public string ShapeName
        {
            get { return shapeName; }
            set
            {
                shapeName = value;
                NotifyPropertyChanged("ShapeID");
            }
        }

        #region Notify
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
        #endregion
    }

    #endregion
}
