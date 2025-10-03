USE [JEGR_Utils]
GO

-- Insert sample shifts (3 shifts covering 24 hours)
SET IDENTITY_INSERT [dbo].[tblShifts] ON
GO

INSERT INTO [dbo].[tblShifts] 
    ([ShiftID], [ShiftName], [StartTime], [EndTime], [DeltaMinus], [DeltaPlus], [IsActive], [CreatedDate], [ModifiedDate])
VALUES
    (1, 'Day Shift', '06:00:00', '14:00:00', 15, 15, 1, GETDATE(), GETDATE()),
    (2, 'Evening Shift', '14:00:00', '22:00:00', 15, 15, 1, GETDATE(), GETDATE()),
    (3, 'Night Shift', '22:00:00', '06:00:00', 15, 15, 1, GETDATE(), GETDATE())
GO

SET IDENTITY_INSERT [dbo].[tblShifts] OFF
GO

-- Insert the single shift setting
SET IDENTITY_INSERT [dbo].[tblShiftSettings] ON
GO

INSERT INTO [dbo].[tblShiftSettings] 
    ([SettingID], [SettingName], [SettingValue], [Detail], [ModifiedDate])
VALUES
    (1, 'AutoLogoutAtShiftEnd', 'true', 'Automatically log out operators at the end of their shift', GETDATE())
GO

SET IDENTITY_INSERT [dbo].[tblShiftSettings] OFF
GO

-- Assign machines to shifts for all days of week
-- Day Shift (6am-2pm) - IL1 Folder (RecNum 14)
INSERT INTO [dbo].[tblShiftMachineAssignments] 
    ([ShiftID], [MachineRecNum], [DayOfWeek], [IsActive], [CreatedDate])
SELECT 1, 14, DayNum, 1, GETDATE()
FROM (VALUES (1),(2),(3),(4),(5)) AS Days(DayNum); -- Mon-Fri

-- Evening Shift (2pm-10pm) - IL1 Folder (RecNum 14)
INSERT INTO [dbo].[tblShiftMachineAssignments] 
    ([ShiftID], [MachineRecNum], [DayOfWeek], [IsActive], [CreatedDate])
SELECT 2, 14, DayNum, 1, GETDATE()
FROM (VALUES (1),(2),(3),(4),(5)) AS Days(DayNum);

-- Night Shift (10pm-6am) - IL1 Folder (RecNum 14)
INSERT INTO [dbo].[tblShiftMachineAssignments] 
    ([ShiftID], [MachineRecNum], [DayOfWeek], [IsActive], [CreatedDate])
SELECT 3, 14, DayNum, 1, GETDATE()
FROM (VALUES (1),(2),(3),(4),(5)) AS Days(DayNum);

-- Assign multiple towel fold machines to shifts
-- TEMATIC SPF 01-03 (RecNum 34-36) for Day shift
INSERT INTO [dbo].[tblShiftMachineAssignments] 
    ([ShiftID], [MachineRecNum], [DayOfWeek], [IsActive], [CreatedDate])
SELECT 1, MachineRec, DayNum, 1, GETDATE()
FROM (VALUES (34),(35),(36)) AS Machines(MachineRec)
CROSS JOIN (VALUES (1),(2),(3),(4),(5)) AS Days(DayNum);

-- TEMATIC SPF 04-06 (RecNum 37-39) for Evening shift
INSERT INTO [dbo].[tblShiftMachineAssignments] 
    ([ShiftID], [MachineRecNum], [DayOfWeek], [IsActive], [CreatedDate])
SELECT 2, MachineRec, DayNum, 1, GETDATE()
FROM (VALUES (37),(38),(39)) AS Machines(MachineRec)
CROSS JOIN (VALUES (1),(2),(3),(4),(5)) AS Days(DayNum);

-- TEMATIC SPF 07-09 (RecNum 40-42) for Night shift
INSERT INTO [dbo].[tblShiftMachineAssignments] 
    ([ShiftID], [MachineRecNum], [DayOfWeek], [IsActive], [CreatedDate])
SELECT 3, MachineRec, DayNum, 1, GETDATE()
FROM (VALUES (40),(41),(42)) AS Machines(MachineRec)
CROSS JOIN (VALUES (1),(2),(3),(4),(5)) AS Days(DayNum);

-- Configure some operators with replace capability
INSERT INTO [dbo].[tblOperatorSettings] 
    ([OperatorRecNum], [CanReplaceOperator], [ModifiedDate])
VALUES
    (4, 1, GETDATE()),   -- Alvardo, Daysi (1002) can replace
    (6, 0, GETDATE()),   -- Canales, Miguel (1004) cannot replace
    (7, 1, GETDATE()),   -- Carrion, Rosa (1005) can replace
    (9, 0, GETDATE()),   -- Chamorro, Antonia (1007) cannot replace
    (10, 1, GETDATE());  -- Codutty, Neidy (1008) can replace

GO