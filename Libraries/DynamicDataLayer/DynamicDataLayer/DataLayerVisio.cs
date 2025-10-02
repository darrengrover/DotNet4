using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Dynamic.DataLayer
{

    #region VisioDiagrams
    public class VisioDiagrams : DataList
    {
        public VisioDiagrams()
        {
            Lifespan = 1.0;
            ListType = typeof(VisioDiagram);
            TblName = "tblVisioDiagrams";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("DiagramID", "DiagramID", true, false);
            dBFieldMappings.AddMapping("Title", "Title");
            dBFieldMappings.AddMapping("Description", "Description");
            dBFieldMappings.AddMapping("Width", "Width");
            dBFieldMappings.AddMapping("Height", "Height");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }

    }


    public class VisioDiagram : DataItem
    {
        #region private fields

        private int diagramID;
        private string title;
        //private string description;
        private int width;
        private int height;

        #endregion

        #region Data Column Properties

        public int DiagramID
        {
            get { return diagramID; }
            set
            {
                diagramID = AssignNotify(ref diagramID, value, "DiagramID");
                ID = diagramID;
                PrimaryKey = diagramID;
            }
        }

        public string Title
        {
            get { return title; }
            set
            {
                title = AssignNotify(ref title, value, "Title");
                ItemName = title;
            }
        }

        public string DiagramTitle
        {
            get { return Title; }
            set { title = AssignNotify(ref title, value, "DiagramTitle"); }
        }

        //public string Description
        //{
        //    get { return description; }
        //    set { description = AssignNotify(ref description, value, "Description"); }
        //}

        public int Width
        {
            get { return width; }
            set { width = AssignNotify(ref width, value, "Width"); }
        }

        public int Height
        {
            get { return height; }
            set { height = AssignNotify(ref height, value, "Height"); }
        }

        #endregion
    }
    #endregion

    #region VisioFavourites
    public class VisioFavourites : DataList
    {
        public VisioFavourites()
        {
            Lifespan = 1.0;
            ListType = typeof(VisioFavourite);
            TblName = "tblVisioFavourites";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("FavouriteID", "FavouriteID", true, false);
            dBFieldMappings.AddMapping("DiagramID", "DiagramID");
            dBFieldMappings.AddMapping("AppID", "AppID");
            dBFieldMappings.AddMapping("AppUserID", "AppUserID");
            dBFieldMappings.AddMapping("FavouriteName", "FavouriteName");
            dBFieldMappings.AddMapping("XMin", "XMin");
            dBFieldMappings.AddMapping("YMin", "YMin");
            dBFieldMappings.AddMapping("XMax", "XMax");
            dBFieldMappings.AddMapping("YMax", "YMax");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }

    }


    public class VisioFavourite : DataItem
    {
        #region private fields

        private int favouriteID;
        private int diagramID;
        private int appID;
        private int appUserID;
        private string favouriteName;
        private int xMin;
        private int yMin;
        private int xMax;
        private int yMax;

        #endregion

        #region Data Column Properties

        public int FavouriteID
        {
            get { return favouriteID; }
            set
            {
                favouriteID = AssignNotify(ref favouriteID, value, "FavouriteID");
                ID = favouriteID;
                PrimaryKey = favouriteID;
            }
        }

        public int DiagramID
        {
            get { return diagramID; }
            set { diagramID = AssignNotify(ref diagramID, value, "DiagramID"); }
        }

        public int AppID
        {
            get { return appID; }
            set { appID = AssignNotify(ref appID, value, "AppID"); }
        }

        public int AppUserID
        {
            get { return appUserID; }
            set { appUserID = AssignNotify(ref appUserID, value, "AppUserID"); }
        }

        public string FavouriteName
        {
            get { return favouriteName; }
            set
            {
                favouriteName = AssignNotify(ref favouriteName, value, "FavouriteName");
                ItemName = favouriteName;
            }
        }

        public int XMin
        {
            get { return xMin; }
            set { xMin = AssignNotify(ref xMin, value, "XMin"); }
        }

        public int YMin
        {
            get { return yMin; }
            set { yMin = AssignNotify(ref yMin, value, "YMin"); }
        }

        public int XMax
        {
            get { return xMax; }
            set { xMax = AssignNotify(ref xMax, value, "XMax"); }
        }

        public int YMax
        {
            get { return yMax; }
            set { yMax = AssignNotify(ref yMax, value, "YMax"); }
        }

        #endregion
    }
    #endregion


    #region VisioBitmaps

    public class VisioBitmaps : DataList
    {
        public VisioBitmaps()
        {
            Lifespan = 1.0;
            ListType = typeof(VisioBitmap);
            TblName = "tblVisioBitmaps";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("BitmapID", "BitmapID", true, false);
            dBFieldMappings.AddMapping("Bitmap", "Bitmap");
            dBFieldMappings.AddMapping("Description", "Description");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }
    }

    public class VisioBitmap : DataItem
    {
        private int bitmapID;
        private byte[] bitmap;
        //private string description;

        public int BitmapID
        {
            get { return bitmapID; }
            set
            {
                bitmapID = AssignNotify(ref bitmapID, value, "BitmapID");
                ID = bitmapID;
                PrimaryKey = bitmapID;
            }
        }

        public byte[] Bitmap
        {
            get { return bitmap; }
            set { bitmap = AssignNotify(ref bitmap, value, "Bitmap"); }
        }

        //public string Description
        //{
        //    get { return description; }
        //    set { description = AssignNotify(ref description, value, "Description"); }
        //}

    }

    #endregion

    #region VisioDiagramBitmaps

    public class VisioDiagramBitmaps : DataList
    {
        public VisioDiagramBitmaps()
        {
            Lifespan = 1.0;
            ListType = typeof(VisioDiagramBitmap);
            TblName = "tblVisioDiagramBitmaps";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("DiagBitmapID", "DiagBitmapID", true, false);
            dBFieldMappings.AddMapping("BitmapID", "BitmapID");
            dBFieldMappings.AddMapping("DiagramID", "DiagramID");
            dBFieldMappings.AddMapping("ZIndex", "ZIndex");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }
    }

    public class VisioDiagramBitmap : DataItem
    {
        private int diagBitmapID;
        private int bitmapID;
        private int diagramID;
        private int zIndex;

        public int DiagBitmapID
        {
            get { return diagBitmapID; }
            set
            {
                diagBitmapID = AssignNotify(ref diagBitmapID, value, "DiagBitmapID");
                ID = diagBitmapID;
                PrimaryKey = diagBitmapID;
            }
        }

        public int BitmapID
        {
            get { return bitmapID; }
            set { bitmapID = AssignNotify(ref bitmapID, value, "BitmapID"); }
        }

        public int DiagramID
        {
            get { return diagramID; }
            set { diagramID = AssignNotify(ref diagramID, value, "DiagramID"); }
        }

        public int ZIndex
        {
            get { return zIndex; }
            set { zIndex = AssignNotify(ref zIndex, value, "ZIndex"); }
        }



    }

    #endregion

    #region VisioPositions

    public class VisioPositions : DataList
    {
        public VisioPositions()
        {
            Lifespan = 1.0;
            ListType = typeof(VisioPosition);
            TblName = "tblVisioPositions";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("VisioPositionID", "VisioPositionID", true, false);
            dBFieldMappings.AddMapping("DiagramID", "DiagramID");
            dBFieldMappings.AddMapping("X", "X");
            dBFieldMappings.AddMapping("Y", "Y");
            dBFieldMappings.AddMapping("Width", "Width");
            dBFieldMappings.AddMapping("Height", "Height");
            dBFieldMappings.AddMapping("Scale", "Scale");
            dBFieldMappings.AddMapping("Rotation", "Rotation");
            dBFieldMappings.AddMapping("Mirror", "Mirror");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }
    }

    public class VisioPosition : DataItem
    {
        private int visioPositionID;
        private int diagramID;
        private double x;
        private double y;
        private int width;
        private int height;
        private double scale;
        private double rotation;
        private bool mirror;

        public int VisioPositionID
        {
            get { return visioPositionID; }
            set
            {
                visioPositionID = AssignNotify(ref visioPositionID, value, "VisioPositionID");
                ID = visioPositionID;
                PrimaryKey = visioPositionID;
            }
        }

        public int DiagramID
        {
            get { return diagramID; }
            set { diagramID = AssignNotify(ref diagramID, value, "DiagramID"); }
        }

        public double X
        {
            get { return x; }
            set { x = AssignNotify(ref x, value, "X"); }
        }

        public double Y
        {
            get { return y; }
            set { y = AssignNotify(ref y, value, "Y"); }
        }

        public int Width
        {
            get { return width; }
            set { width = AssignNotify(ref width, value, "Width"); }
        }

        public int Height
        {
            get { return height; }
            set { height = AssignNotify(ref height, value, "Height"); }
        }

        public double Scale
        {
            get { return scale; }
            set { scale = AssignNotify(ref scale, value, "Scale"); }
        }

        public double Rotation
        {
            get { return rotation; }
            set { rotation = AssignNotify(ref rotation, value, "Rotation"); }
        }

        public bool Mirror
        {
            get { return mirror; }
            set { mirror = AssignNotify(ref mirror, value, "Mirror"); }
        }

    }

    public class VisioPositionLookup : DataItem
    {
        /* this is in effect an abstract class that is not instanciated in itself
         * it has no collection.  Used as a descendant to visio items that have a
         * Position to provide wrappers for the position and diagram records.
         * It may require notify elements as these may not apply from the unbound 
         * items.  Update comment when testing! */

        private int visioPositionID;
        private VisioPosition visioPosition;
        private DateTime positionTime;
        private VisioDiagram visioDiagram;
        private DateTime diagramTime;
        public SqlDataAccess da;


        private int timeoutSeconds = 2;

        public VisioPositionLookup()
        {
            da = SqlDataAccess.Singleton;
        }

        private bool isTimedOut(DateTime a, int timeLimitSeconds)
        {
            bool test = true;
            if (a != null)
            {
                TimeSpan ts = DateTime.Now.Subtract(a);
                test = (ts.TotalSeconds > timeLimitSeconds);
            }
            return test;
        }

        private bool positionValid()
        {
            bool test = (visioPosition != null) && (!isTimedOut(positionTime, timeoutSeconds));
            return test;
        }

        private bool diagramValid()
        {
            bool test = (visioPosition != null) && (!isTimedOut(positionTime, timeoutSeconds));
            return test;
        }

        private void getPosition()
        {
            if (!positionValid())
            {
                VisioPositions vps = new VisioPositions();
                vps = (VisioPositions)da.DBDataListSelect(vps);
                visioPosition = (VisioPosition)vps.GetById(visioPositionID);
                positionTime = DateTime.Now;
            }
        }

        private void getDiagram()
        {
            if (!diagramValid())
            {
                VisioDiagrams vds = new VisioDiagrams();
                vds = (VisioDiagrams)da.DBDataListSelect(vds);
                visioDiagram = (VisioDiagram)vds.GetById(DiagramID);
                diagramTime = DateTime.Now;
            }
        }

        public int VisioPositionID
        {
            get { return visioPositionID; }
            set
            {
                visioPositionID = AssignNotify(ref visioPositionID, value, "VisioPositionID");
                ID = visioPositionID;
                PrimaryKey = visioPositionID;
            }
        }

        public int DiagramID
        {
            get
            {
                getPosition();
                if (visioPosition != null)
                    return visioPosition.DiagramID;
                else
                    return -1;
            }
            set
            {
                getPosition();
                if (visioPosition != null)
                {
                    visioPosition.DiagramID = value;
                }
            }
        }

        public double X
        {
            get
            {
                getPosition();
                if (visioPosition != null)
                    return visioPosition.X;
                else
                    return 0;
            }
            set
            {
                getPosition();
                if (visioPosition != null)
                {
                    visioPosition.X = value;
                }
            }
        }

        public double Y
        {
            get
            {
                getPosition();
                if (visioPosition != null)
                    return visioPosition.Y;
                else
                    return 0;
            }
            set
            {
                getPosition();
                if (visioPosition != null)
                {
                    visioPosition.Y = value;
                }
            }
        }

        public double Scale
        {
            get
            {
                getPosition();
                if (visioPosition != null)
                    return visioPosition.Scale;
                else
                    return 0;
            }
            set
            {
                getPosition();
                if (visioPosition != null)
                {
                    visioPosition.Scale = value;
                }
            }
        }

        public double Rotation
        {
            get
            {
                getPosition();
                if (visioPosition != null)
                    return visioPosition.Rotation;
                else
                    return 0;
            }
            set
            {
                getPosition();
                if (visioPosition != null)
                {
                    visioPosition.Rotation = value;
                }
            }
        }

        public int Width
        {
            get
            {
                getPosition();
                if (visioPosition != null)
                    return visioPosition.Width;
                else
                    return 0;
            }
            set
            {
                getPosition();
                if (visioPosition != null)
                {
                    visioPosition.Width = value;
                }
            }
        }

        public int Height
        {
            get
            {
                getPosition();
                if (visioPosition != null)
                    return visioPosition.Height;
                else
                    return 0;
            }
            set
            {
                getPosition();
                if (visioPosition != null)
                {
                    visioPosition.Height = value;
                }
            }
        }

        public bool Mirror
        {
            get
            {
                getPosition();
                if (visioPosition != null)
                    return visioPosition.Mirror;
                else
                    return false;
            }
            set
            {
                getPosition();
                if (visioPosition != null)
                {
                    visioPosition.Mirror = value;
                }
            }
        }

        public string DiagTitle
        {
            get
            {
                getDiagram();
                if (visioDiagram != null)
                    return visioDiagram.Title;
                else
                    return string.Empty;
            }
            set
            {
                getDiagram();
                if (visioDiagram != null)
                {
                    visioDiagram.Title = value;
                }
            }
        }

        public string DiagDesc
        {
            get
            {
                getDiagram();
                if (visioDiagram != null)
                    return visioDiagram.Description;
                else
                    return string.Empty;
            }
            set
            {
                getDiagram();
                if (visioDiagram != null)
                {
                    visioDiagram.Description = value;
                }
            }
        }

        public int DiagWidth
        {
            get
            {
                getDiagram();
                if (visioDiagram != null)
                    return visioDiagram.Width;
                else
                    return 0;
            }
            set
            {
                getDiagram();
                if (visioDiagram != null)
                {
                    visioDiagram.Width = value;
                }
            }
        }

        public int DiagHeight
        {
            get
            {
                getDiagram();
                if (visioDiagram != null)
                    return visioDiagram.Height;
                else
                    return 0;
            }
            set
            {
                getDiagram();
                if (visioDiagram != null)
                {
                    visioDiagram.Height = value;
                }
            }
        }

    }

    #endregion

    #region VisioMachines

    public class VisioMachines : DataList
    {
        public VisioMachines()
        {
            Lifespan = 1.0;
            ListType = typeof(VisioMachine);
            TblName = "tblVisioMachines";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("VisioMachineID", "VisioMachineID", true, false);
            dBFieldMappings.AddMapping("VisioPositionID", "VisioPositionID");
            dBFieldMappings.AddMapping("MachineID", "MachineID");
            dBFieldMappings.AddMapping("MachineShape", "MachineShape");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }
    }

    public class VisioMachine : VisioPositionLookup
    {
        private int visioMachineID;
        //private int visioPositionID;
        private int machineID;
        private int machineShape;

        public int VisioMachineID
        {
            get { return visioMachineID; }
            set
            {
                visioMachineID = AssignNotify(ref visioMachineID, value, "VisioMachineID");
                ID = visioMachineID;
                PrimaryKey = visioMachineID;
            }
        }

        //public int VisioPositionID from lookup

        public int MachineID
        {
            get { return machineID; }
            set { machineID = AssignNotify(ref machineID, value, "MachineID"); }
        }

        public int MachineShape
        {
            get { return machineShape; }
            set { machineShape = AssignNotify(ref machineShape, value, "MachineShape"); }
        }


    }

    #endregion

    #region VisioStations

    public class VisioStations : DataList
    {
        public VisioStations()
        {
            Lifespan = 1.0;
            ListType = typeof(VisioStation);
            TblName = "tblVisioStations";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("VisioStationID", "VisioStationID", true, false);
            dBFieldMappings.AddMapping("VisioPositionID", "VisioPositionID");
            dBFieldMappings.AddMapping("MachineID", "MachineID");
            dBFieldMappings.AddMapping("SubID", "SubID");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }
    }

    public class VisioStation : VisioPositionLookup
    {
        private int visioStationID;
        //private int visioPositionID;
        private int machineID;
        private int subID;

        public int VisioStationID
        {
            get { return visioStationID; }
            set
            {
                visioStationID = AssignNotify(ref visioStationID, value, "VisioStationID");
                ID = visioStationID;
                PrimaryKey = visioStationID;
            }
        }

        //public int VisioPositionID from lookup

        public int MachineID
        {
            get { return machineID; }
            set { machineID = AssignNotify(ref machineID, value, "MachineID"); }
        }

        public int SubID
        {
            get { return subID; }
            set { subID = AssignNotify(ref subID, value, "SubID"); }
        }


    }

    #endregion

    #region VisioBatches

    public class VisioBatches : DataList
    {
        public VisioBatches()
        {
            Lifespan = 1.0;
            ListType = typeof(VisioBatch);
            TblName = "tblVisioStations";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("VisioBatchID", "VisioBatchID", true, false);
            dBFieldMappings.AddMapping("VisioPositionID", "VisioPositionID");
            dBFieldMappings.AddMapping("MachineID", "MachineID");
            dBFieldMappings.AddMapping("PlcPosition", "PlcPosition");
            dBFieldMappings.AddMapping("BatchShape", "BatchShape");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }
    }

    public class VisioBatch : VisioPositionLookup
    {
        private int visioBatchID;
        //private int visioPositionID;
        private int machineID;
        private int plcPosition;
        private int batchShape;

        public int VisioBatchID
        {
            get { return visioBatchID; }
            set
            {
                visioBatchID = AssignNotify(ref visioBatchID, value, "VisioBatchID");
                ID = visioBatchID;
                PrimaryKey = visioBatchID;
            }
        }

        //public int VisioPositionID from lookup

        public int MachineID
        {
            get { return machineID; }
            set { machineID = AssignNotify(ref machineID, value, "MachineID"); }
        }

        public int PlcPosition
        {
            get { return plcPosition; }
            set { plcPosition = AssignNotify(ref plcPosition, value, "PlcPosition"); }
        }

        public int BatchShape
        {
            get { return batchShape; }
            set { batchShape = AssignNotify(ref batchShape, value, "BatchShape"); }
        }


    }

    #endregion

    #region VisioSequences

    public class VisioSequences : DataList
    {
        public VisioSequences()
        {
            Lifespan = 1.0;
            ListType = typeof(VisioSequence);
            TblName = "tblVisioSequences";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("VisioSequenceID", "VisioSequenceID", true, false);
            dBFieldMappings.AddMapping("VisioPositionID", "VisioPositionID");
            dBFieldMappings.AddMapping("MachineID", "MachineID");
            dBFieldMappings.AddMapping("PlcPosition", "PlcPosition");
            dBFieldMappings.AddMapping("Title", "Title");
            dBFieldMappings.AddMapping("IsShortTrip", "IsShortTrip");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }
    }

    public class VisioSequence : VisioPositionLookup
    {
        private int visioSequenceID;
        //private int visioPositionID;
        private int machineID;
        private int plcPosition;
        private int title;
        private bool isShortTrip;

        public int VisioSequenceID
        {
            get { return visioSequenceID; }
            set
            {
                visioSequenceID = AssignNotify(ref visioSequenceID, value, "VisioSequenceID");
                ID = visioSequenceID;
                PrimaryKey = visioSequenceID;
            }
        }

        //public int VisioPositionID from lookup

        public int MachineID
        {
            get { return machineID; }
            set { machineID = AssignNotify(ref machineID, value, "MachineID"); }
        }

        public int PlcPosition
        {
            get { return plcPosition; }
            set { plcPosition = AssignNotify(ref plcPosition, value, "PlcPosition"); }
        }

        public int Title
        {
            get { return title; }
            set { title = AssignNotify(ref title, value, "Title"); }
        }

        public bool IsShortTrip
        {
            get { return isShortTrip; }
            set { isShortTrip = AssignNotify(ref isShortTrip, value, "IsShortTrip"); }
        }


    }

    #endregion

    #region VisioTexts

    public class VisioTexts : DataList
    {
        public VisioTexts()
        {
            Lifespan = 1.0;
            ListType = typeof(VisioText);
            TblName = "tblVisioText";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("VisioTextID", "VisioTextID", true, false);
            dBFieldMappings.AddMapping("VisioPositionID", "VisioPositionID");
            dBFieldMappings.AddMapping("IsDynamic", "IsDynamic");
            dBFieldMappings.AddMapping("StaticText", "StaticText");
            dBFieldMappings.AddMapping("SqlCommand", "SqlCommand");
            dBFieldMappings.AddMapping("IsMultiLine", "IsMultiLine");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }
    }

    public class VisioText : VisioPositionLookup
    {
        private int visioTextID;
        //private int visioPositionID;
        private bool isDynamic;
        private string staticText;
        private string sqlCommand;
        private bool isMultiLine;

        public int VisioTextID
        {
            get { return visioTextID; }
            set
            {
                visioTextID = AssignNotify(ref visioTextID, value, "VisioTextID");
                ID = visioTextID;
                PrimaryKey = visioTextID;
            }
        }

        //public int VisioPositionID from lookup

        public bool IsDynamic
        {
            get { return isDynamic; }
            set { isDynamic = AssignNotify(ref isDynamic, value, "IsDynamic"); }
        }

        public string StaticText
        {
            get { return staticText; }
            set { staticText = AssignNotify(ref staticText, value, "StaticText"); }
        }

        public string SqlCommand
        {
            get { return sqlCommand; }
            set { sqlCommand = AssignNotify(ref sqlCommand, value, "SqlCommand"); }
        }

        public bool IsMultiLine
        {
            get { return isMultiLine; }
            set { isMultiLine = AssignNotify(ref isMultiLine, value, "IsMultiLine"); }
        }


    }

    #endregion


}

