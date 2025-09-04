namespace Ge_Mac.Logging
{
    #region RailEventType Enumeration
    // Type of events
    public enum RailEventType
    {
        Alarm = 1,
        Batch = 2,
        Sequence = 3,
        Trip = 4,
        User = 5,
        SystemConfiguration = 6,
        Settings = 7,
        CategorySetup = 8
    }
    #endregion

    #region RailEventAction Enumeration
    // Actions within the event
    public enum RailEventAction
    {
        System = 0,
        AlarmOn = 1,
        AlarmOff = 2,
        UserLogon = 11,
        UserLogoff = 12,
        UserLogonFail = 13,
        UserSetupChange = 14,
        UserRightsChange = 15,
        BatchCustomerChange = 21,
        BatchCategoryChange = 22,
        BatchWeightChange = 23,
        BatchStorageChange = 24,
        BatchDropChange = 25,
        BatchLoopChange = 26,
        BatchAdd = 31,
        BatchEdit = 35,
        BatchDelete = 41,
        SequenceEdit = 51,
        TripSet = 61,
        TripReset = 62,
        CalibrateLow = 71,
        CalibrateHigh = 72,
        CustomerSetup = 81,
        QuickRef = 91,
        ExtRef = 92,
        ShortDescription = 93,
        LongDescription = 94,
        BackColour = 95,
        ForeColour = 96,
        ReleaseWeight = 97,
        MinimumWeight = 98,
        WeightApproach = 99,
        WeightOver = 100,
        UseDefault = 101,
        BatchSize = 102,
        Express = 103,
        FixedRail_1 = 104,
        FixedRail_2 = 105,
        FixedRail_3 = 106,
        FixedRail_4 = 107,
        FixedRail_5 = 108,
        UseDynamicRails = 109,
        DynamicPercent = 110,
        DynamicDryTime = 111,
        AllowedRail = 112,
        WeightPerPiece = 113,
        WeightSettings = 121,
        CallOff = 131,
        SortingSetup = 141
    }
    #endregion
}
