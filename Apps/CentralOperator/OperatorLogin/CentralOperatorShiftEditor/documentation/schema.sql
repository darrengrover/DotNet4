-- =============================================
-- Central Operator Shift Management Schema
-- Database: JEGR_Utils
-- =============================================

-- Core Shifts Table
CREATE TABLE tblShifts (
    ShiftID int IDENTITY(1,1) PRIMARY KEY,
    ShiftName varchar(50) NOT NULL,
    StartTime time NOT NULL,
    EndTime time NOT NULL,
    DeltaMinus int NOT NULL DEFAULT 0, -- minutes before shift end operator can logout
    DeltaPlus int NOT NULL DEFAULT 0,  -- minutes after shift end operator can logout
    CrossesMidnight AS (CASE WHEN EndTime < StartTime THEN 1 ELSE 0 END), -- computed column
    IsActive bit NOT NULL DEFAULT 1,
    CreatedDate datetime NOT NULL DEFAULT GETDATE(),
    ModifiedDate datetime NOT NULL DEFAULT GETDATE(),
    
    CONSTRAINT CK_tblShifts_DeltaValues CHECK (DeltaMinus >= 0 AND DeltaPlus >= 0),
    CONSTRAINT CK_tblShifts_ShiftName CHECK (LEN(LTRIM(RTRIM(ShiftName))) > 0)
);

-- Machine-Shift Assignments with Day of Week
CREATE TABLE tblShiftMachineAssignments (
    AssignmentID int IDENTITY(1,1) PRIMARY KEY,
    ShiftID int NOT NULL,
    MachineRecNum int NOT NULL, -- FK to JEGR_DB machine table RecNum
    DayOfWeek tinyint NOT NULL, -- 1=Sunday, 2=Monday, 3=Tuesday, etc.
    IsActive bit NOT NULL DEFAULT 1,
    CreatedDate datetime NOT NULL DEFAULT GETDATE(),
    
    CONSTRAINT FK_tblShiftMachineAssignments_ShiftID 
        FOREIGN KEY (ShiftID) REFERENCES tblShifts(ShiftID),
    CONSTRAINT CK_tblShiftMachineAssignments_DayOfWeek 
        CHECK (DayOfWeek BETWEEN 1 AND 7),
    
    -- Unique constraint to prevent duplicate assignments
    CONSTRAINT UQ_tblShiftMachineAssignments_Machine_Day_Shift 
        UNIQUE (MachineRecNum, DayOfWeek, ShiftID)
);

-- System-wide Settings
CREATE TABLE tblShiftSettings (
    SettingID int IDENTITY(1,1) PRIMARY KEY,
    SettingName varchar(50) NOT NULL UNIQUE,
    SettingValue varchar(100) NOT NULL,
    Detail varchar(255) NULL,
    ModifiedDate datetime NOT NULL DEFAULT GETDATE(),
    
    CONSTRAINT CK_tblShiftSettings_SettingName 
        CHECK (LEN(LTRIM(RTRIM(SettingName))) > 0)
);

-- Per-Operator Settings
CREATE TABLE tblOperatorSettings (
    OperatorSettingID int IDENTITY(1,1) PRIMARY KEY,
    OperatorRecNum int NOT NULL UNIQUE, -- FK to JEGR_DB operator table RecNum
    CanReplaceOperator bit NOT NULL DEFAULT 0, -- the "operator replace" setting for supervisors
    ModifiedDate datetime NOT NULL DEFAULT GETDATE()
);

-- =============================================
-- INDEXES FOR PERFORMANCE
-- =============================================

-- Index for quick shift lookups by time
CREATE INDEX IX_tblShifts_Times ON tblShifts (StartTime, EndTime, IsActive);

-- Index for machine assignment lookups
CREATE INDEX IX_tblShiftMachineAssignments_Machine_Day 
    ON tblShiftMachineAssignments (MachineRecNum, DayOfWeek, IsActive);

-- Index for operator settings lookups
CREATE INDEX IX_tblOperatorSettings_OperatorRecNum 
    ON tblOperatorSettings (OperatorRecNum);
GO
-- =============================================
-- VALIDATION FUNCTION FOR SHIFT OVERLAPS
-- =============================================

CREATE FUNCTION fn_CheckShiftOverlap
(
    @MachineRecNum int,
    @DayOfWeek tinyint,
    @StartTime time,
    @EndTime time,
    @ExcludeAssignmentID int = NULL
)
RETURNS bit
AS
BEGIN
    DECLARE @HasOverlap bit = 0;
    
    -- Check for overlaps with existing shifts for the same machine and day
    IF EXISTS (
        SELECT 1 
        FROM tblShiftMachineAssignments sma
        INNER JOIN tblShifts s ON sma.ShiftID = s.ShiftID
        WHERE sma.MachineRecNum = @MachineRecNum 
          AND sma.DayOfWeek = @DayOfWeek
          AND sma.IsActive = 1 
          AND s.IsActive = 1
          AND (@ExcludeAssignmentID IS NULL OR sma.AssignmentID != @ExcludeAssignmentID)
          AND (
              -- Case 1: Neither shift crosses midnight
              (s.StartTime <= s.EndTime AND @StartTime <= @EndTime AND 
               NOT (@EndTime <= s.StartTime OR @StartTime >= s.EndTime))
              OR
              -- Case 2: Existing shift crosses midnight, new shift doesn't
              (s.StartTime > s.EndTime AND @StartTime <= @EndTime AND 
               NOT (@EndTime <= s.StartTime AND @StartTime >= s.EndTime))
              OR  
              -- Case 3: New shift crosses midnight, existing doesn't
              (s.StartTime <= s.EndTime AND @StartTime > @EndTime AND 
               NOT (s.EndTime <= @StartTime AND s.StartTime >= @EndTime))
              OR
              -- Case 4: Both shifts cross midnight
              (s.StartTime > s.EndTime AND @StartTime > @EndTime)
          )
    )
    BEGIN
        SET @HasOverlap = 1;
    END
    
    RETURN @HasOverlap;
END;
GO

-- =============================================
-- VALIDATION TRIGGER FOR SHIFT OVERLAPS
-- =============================================

CREATE TRIGGER tr_tblShiftMachineAssignments_ValidateOverlap
ON tblShiftMachineAssignments
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Check each inserted/updated record
    IF EXISTS (
        SELECT 1
        FROM inserted i
        INNER JOIN tblShifts s ON i.ShiftID = s.ShiftID
        WHERE dbo.fn_CheckShiftOverlap(i.MachineRecNum, i.DayOfWeek, s.StartTime, s.EndTime, i.AssignmentID) = 1
    )
    BEGIN
        RAISERROR('Shift assignment would create overlapping shifts for the same machine on the same day.', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
END;
GO

-- =============================================
-- INITIAL SYSTEM SETTINGS
-- =============================================

INSERT INTO tblShiftSettings (SettingName, SettingValue, Detail)
VALUES 
    ('AutoLogoutAtShiftEnd', 'True', 'Automatically log out operators when their shift ends');

-- =============================================
-- UTILITY VIEWS FOR EASY QUERYING
-- =============================================

-- View to get current active shifts for all machines
CREATE VIEW vw_CurrentActiveShifts
AS
SELECT 
    sma.MachineRecNum,
    s.ShiftID,
    s.ShiftName,
    s.StartTime,
    s.EndTime,
    s.DeltaMinus,
    s.DeltaPlus,
    s.CrossesMidnight,
    sma.DayOfWeek,
    CASE 
        WHEN s.StartTime <= s.EndTime THEN
            -- Normal shift
            CASE WHEN CAST(GETDATE() AS time) BETWEEN s.StartTime AND s.EndTime THEN 1 ELSE 0 END
        ELSE
            -- Cross-midnight shift
            CASE WHEN CAST(GETDATE() AS time) >= s.StartTime OR CAST(GETDATE() AS time) <= s.EndTime THEN 1 ELSE 0 END
    END AS IsCurrentlyActive
FROM tblShiftMachineAssignments sma
INNER JOIN tblShifts s ON sma.ShiftID = s.ShiftID
WHERE sma.DayOfWeek = DATEPART(weekday, GETDATE())
  AND sma.IsActive = 1 
  AND s.IsActive = 1;
GO

-- View to get all machine-shift assignments with shift details
CREATE VIEW vw_MachineShiftDetails
AS
SELECT 
    sma.AssignmentID,
    sma.MachineRecNum,
    s.ShiftID,
    s.ShiftName,
    s.StartTime,
    s.EndTime,
    s.DeltaMinus,
    s.DeltaPlus,
    s.CrossesMidnight,
    sma.DayOfWeek,
    CASE sma.DayOfWeek 
        WHEN 1 THEN 'Sunday'
        WHEN 2 THEN 'Monday'
        WHEN 3 THEN 'Tuesday'
        WHEN 4 THEN 'Wednesday'
        WHEN 5 THEN 'Thursday'
        WHEN 6 THEN 'Friday'
        WHEN 7 THEN 'Saturday'
    END AS DayOfWeekName,
    sma.IsActive AS AssignmentActive,
    s.IsActive AS ShiftActive,
    sma.CreatedDate
FROM tblShiftMachineAssignments sma
INNER JOIN tblShifts s ON sma.ShiftID = s.ShiftID;