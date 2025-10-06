using Dynamic.DataLayer;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace CentralOperatorShiftEditor
{
    public partial class ShiftEditorWindow : Window
    {
        #region Fields
        private SqlDataAccess dataAccess;

        // Data collections
        private Shifts shifts;
        private ShiftMachineAssignments assignments;
        private ShiftSettings shiftSettings;
        private OperatorSettings operatorSettings;

        // Current selections
        private Shift currentlySelectedShift = null;
        private ShiftMachineAssignments currentShiftMachines;

        // Collection views
        private ListCollectionView shiftsView;
        private ListCollectionView shiftMachinesView;

        // Change tracking flags
        private bool hasUnsavedShifts = false;
        private bool hasUnsavedShiftSettings = false;
        private bool hasUnsavedOperatorSettings = false;
        private bool hasUnsavedShiftMachines = false;
        #endregion

        #region Initialization
        public ShiftEditorWindow()
        {
            InitializeComponent();
            dataAccess = SqlDataAccess.Singleton;
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                LoadShifts();
                LoadAllAssignments(); // Load all assignments into cache
                LoadShiftSettings();
                LoadOperatorSettings();
                UpdateStatusBar();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        /// <summary>
        /// Loads all shift-machine assignments into the cache for filtering
        /// </summary>
        private void LoadAllAssignments()
        {
            assignments = dataAccess.GetShiftMachineAssignments(null, true);
        }
        #endregion

        #region Status Bar & Change Tracking
        private void UpdateStatusBar()
        {
            bool hasAnyChanges = hasUnsavedShifts || hasUnsavedShiftSettings ||
                                 hasUnsavedOperatorSettings || hasUnsavedShiftMachines;

            if (hasAnyChanges)
            {
                txtUnsavedChanges.Text = "⚠ Unsaved Changes";
                txtStatus.Text = "Modified";

                if (!this.Title.EndsWith("*"))
                    this.Title += " *";
            }
            else
            {
                txtUnsavedChanges.Text = "";
                txtStatus.Text = "Ready";
                this.Title = this.Title.TrimEnd('*', ' ');
            }
        }

        private void MarkShiftsChanged()
        {
            hasUnsavedShifts = true;
            UpdateStatusBar();
        }

        private void MarkShiftSettingsChanged()
        {
            hasUnsavedShiftSettings = true;
            UpdateStatusBar();
        }

        private void MarkOperatorSettingsChanged()
        {
            hasUnsavedOperatorSettings = true;
            UpdateStatusBar();
        }

        private void MarkShiftMachinesChanged()
        {
            hasUnsavedShiftMachines = true;
            UpdateStatusBar();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            bool hasAnyChanges = hasUnsavedShifts || hasUnsavedShiftSettings ||
                                 hasUnsavedOperatorSettings || hasUnsavedShiftMachines;

            if (hasAnyChanges)
            {
                var result = MessageBox.Show(
                    "You have unsaved changes. Are you sure you want to close without saving?",
                    "Unsaved Changes",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.No)
                {
                    e.Cancel = true;
                }
            }
        }
        #endregion

        #region Shifts Management
        private void LoadShifts()
        {
            shifts = dataAccess.GetShifts(null, true);
            shiftsView = CollectionViewSource.GetDefaultView(shifts) as ListCollectionView;
            dgShifts.ItemsSource = shiftsView;

            // Subscribe to collection changes
            if (shifts is INotifyCollectionChanged collectionChanged)
            {
                collectionChanged.CollectionChanged += Shifts_CollectionChanged;
            }

            // Subscribe to property changes for each shift
            foreach (Shift shift in shifts)
            {
                shift.PropertyChanged += Shift_PropertyChanged;
            }

            hasUnsavedShifts = false;
        }

        private void Shifts_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Shift shift in e.NewItems)
                {
                    shift.PropertyChanged += Shift_PropertyChanged;
                }
                MarkShiftsChanged();
            }
        }

        private void Shift_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            MarkShiftsChanged();
        }

        private void DgShifts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgShifts.SelectedItem is Shift selectedShift)
            {
                currentlySelectedShift = selectedShift;
                LoadMachinesForShift(selectedShift);
            }
            else
            {
                currentlySelectedShift = null;
                dgShiftMachines.ItemsSource = null;
                txtShiftMachineCount.Text = "0 assignments";
            }
        }

        private void BtnAddShift_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Shift newShift = new Shift
                {
                    ShiftName = "New Shift",
                    StartTime = new TimeSpan(8, 0, 0),
                    EndTime = new TimeSpan(16, 0, 0),
                    DeltaMinus = 5,
                    DeltaPlus = 5,
                    IsActive = true,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    ForceNew = true
                };
                shifts.Add(newShift);
                shiftsView.Refresh();
                MarkShiftsChanged();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding shift: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnDeleteShift_Click(object sender, RoutedEventArgs e)
        {
            if (dgShifts.SelectedItems.Count == 0) return;

            var result = MessageBox.Show("Delete selected shift(s)?", "Confirm Delete",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    Mouse.OverrideCursor = Cursors.Wait;

                    var itemsToDelete = dgShifts.SelectedItems.Cast<Shift>().ToList();

                    foreach (var shift in itemsToDelete)
                    {
                        shift.DeleteRecord = true;
                    }

                    dgShifts.SelectedItems.Clear();
                    shifts.UpdateToDB();
                    LoadShifts();

                    hasUnsavedShifts = false;
                    UpdateStatusBar();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting shifts: {ex.Message}", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    Mouse.OverrideCursor = null;
                }
            }
        }

        private void BtnSaveShifts_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                dgShifts.CommitEdit(DataGridEditingUnit.Row, true);
                dgShifts.CommitEdit(DataGridEditingUnit.Cell, true);

                Mouse.OverrideCursor = Cursors.Wait;
                shifts.UpdateToDB();

                MessageBox.Show("Shifts saved successfully", "Success",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                LoadShifts();
                hasUnsavedShifts = false;
                UpdateStatusBar();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving shifts: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void BtnRefreshShifts_Click(object sender, RoutedEventArgs e)
        {
            if (hasUnsavedShifts)
            {
                var result = MessageBox.Show(
                    "You have unsaved changes. Refreshing will lose these changes. Continue?",
                    "Unsaved Changes",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.No) return;
            }

            LoadShifts();
            hasUnsavedShifts = false;
            UpdateStatusBar();
        }

        // Number validation for textboxes
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        // Delta increment/decrement handlers
        private void BtnIncrementDeltaMinus_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.DataContext is Shift shift)
            {
                shift.DeltaMinus = Math.Min(shift.DeltaMinus + 1, 120);
            }
        }

        private void BtnDecrementDeltaMinus_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.DataContext is Shift shift)
            {
                shift.DeltaMinus = Math.Max(shift.DeltaMinus - 1, 0);
            }
        }

        private void BtnIncrementDeltaPlus_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.DataContext is Shift shift)
            {
                shift.DeltaPlus = Math.Min(shift.DeltaPlus + 1, 120);
            }
        }

        private void BtnDecrementDeltaPlus_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.DataContext is Shift shift)
            {
                shift.DeltaPlus = Math.Max(shift.DeltaPlus - 1, 0);
            }
        }
        #endregion

        #region Machine Assignments for Selected Shift
        /// <summary>
        /// Loads machine assignments for the currently selected shift by filtering the cached assignments
        /// </summary>
        private void LoadMachinesForShift(Shift shift)
        {
            if (shift == null || shift.ShiftID == 0)
            {
                dgShiftMachines.ItemsSource = null;
                txtShiftMachineCount.Text = "Save shift first to add machines";
                return;
            }

            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                // Filter from cached assignments instead of querying database
                currentShiftMachines = new ShiftMachineAssignments();

                if (assignments != null)
                {
                    foreach (ShiftMachineAssignment assignment in assignments)
                    {
                        if (assignment.ShiftID == shift.ShiftID)
                        {
                            // Ensure lookup fields are populated
                            PopulateAssignmentLookupFields(assignment);
                            currentShiftMachines.Add(assignment);
                        }
                    }
                }

                shiftMachinesView = CollectionViewSource.GetDefaultView(currentShiftMachines) as ListCollectionView;
                dgShiftMachines.ItemsSource = shiftMachinesView;
                txtShiftMachineCount.Text = $"{currentShiftMachines.Count} assignment(s)";

                // Subscribe to changes
                if (currentShiftMachines is INotifyCollectionChanged collectionChanged)
                {
                    collectionChanged.CollectionChanged -= ShiftMachines_CollectionChanged;
                    collectionChanged.CollectionChanged += ShiftMachines_CollectionChanged;
                }

                foreach (ShiftMachineAssignment assignment in currentShiftMachines)
                {
                    assignment.PropertyChanged -= ShiftMachine_PropertyChanged;
                    assignment.PropertyChanged += ShiftMachine_PropertyChanged;
                }

                hasUnsavedShiftMachines = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading machines for shift: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        /// <summary>
        /// Populates the lookup fields (MachineIdJensen and DayOfWeekName) for an assignment
        /// </summary>
        private void PopulateAssignmentLookupFields(ShiftMachineAssignment assignment)
        {
            // Populate MachineIdJensen if not already set
            if (assignment.MachineIdJensen > 0)
            {
                var machines = dataAccess.GetAllMachines(null, false);
                var machine = machines.Cast<Machine>().FirstOrDefault(m => m.RecNum == assignment.MachineRecNum);
                if (machine != null)
                {
                    assignment.MachineIdJensen = machine.IdJensen;
                }
            }
        }

        private void ShiftMachines_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (ShiftMachineAssignment assignment in e.NewItems)
                {
                    assignment.PropertyChanged += ShiftMachine_PropertyChanged;
                }
                MarkShiftMachinesChanged();
            }

            if (currentShiftMachines != null)
            {
                txtShiftMachineCount.Text = $"{currentShiftMachines.Count} assignment(s)";
            }
        }

        private void ShiftMachine_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            MarkShiftMachinesChanged();
        }

        private void BtnAddMachinesToShift_Click(object sender, RoutedEventArgs e)
        {
            if (currentlySelectedShift == null)
            {
                MessageBox.Show("Please select a shift first.", "No Shift Selected",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (currentlySelectedShift.ShiftID == 0)
            {
                MessageBox.Show("Please save the shift before adding machines.", "Unsaved Shift",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var dialog = new AddMachinesDialog(currentlySelectedShift, assignments);
                if (dialog.ShowDialog() == true)
                {
                    // Add to both collections
                    foreach (var assignment in dialog.NewAssignments)
                    {
                        // Ensure lookup fields are populated (they should already be set by the dialog)
                        PopulateAssignmentLookupFields(assignment);

                        currentShiftMachines.Add(assignment);
                        assignments.Add(assignment);
                    }

                    shiftMachinesView?.Refresh();
                    txtShiftMachineCount.Text = $"{currentShiftMachines.Count} assignment(s)";
                    MarkShiftMachinesChanged();

                    MessageBox.Show(
                        $"Added {dialog.NewAssignments.Count} assignment(s) to the list.\n\n" +
                        $"Click 'Save Assignments' to save to database.\n\n" +
                        $"Note: The save will fail if any assignments create overlapping shifts.",
                        "Assignments Added",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding machines to shift: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnRemoveMachineFromShift_Click(object sender, RoutedEventArgs e)
        {
            if (dgShiftMachines.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select assignment(s) to remove.", "No Selection",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show(
                $"Remove {dgShiftMachines.SelectedItems.Count} selected assignment(s)?",
                "Confirm Remove",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    Mouse.OverrideCursor = Cursors.Wait;

                    var itemsToRemove = dgShiftMachines.SelectedItems.Cast<ShiftMachineAssignment>().ToList();

                    foreach (var assignment in itemsToRemove)
                    {
                        assignment.DeleteRecord = true;
                    }

                    dgShiftMachines.SelectedItems.Clear();
                    currentShiftMachines.UpdateToDB();

                    // Reload both collections
                    LoadAllAssignments();
                    LoadMachinesForShift(currentlySelectedShift);

                    hasUnsavedShiftMachines = false;
                    UpdateStatusBar();

                    MessageBox.Show("Assignment(s) removed successfully.", "Success",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error removing assignments: {ex.Message}", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    Mouse.OverrideCursor = null;
                }
            }
        }

        private void BtnSaveShiftAssignments_Click(object sender, RoutedEventArgs e)
        {
            if (currentShiftMachines == null || currentShiftMachines.Count == 0)
            {
                MessageBox.Show("No assignments to save.", "Information",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                // Validate for overlaps BEFORE attempting to save
                var (hasOverlap, conflicts) = CheckForOverlappingAssignments(currentShiftMachines);

                if (hasOverlap)
                {
                    Mouse.OverrideCursor = null;

                    // Show detailed conflict information
                    string conflictDetails = string.Join("\n\n", conflicts);
                    MessageBox.Show(
                        "Cannot save assignments: The following conflicts were detected:\n\n" +
                        conflictDetails + "\n\n" +
                        "Please remove the conflicting assignments and try again.",
                        "Overlap Conflicts Detected",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return;
                }

                // No overlaps detected - proceed with save
                currentShiftMachines.UpdateToDB();

                // Reload from database to get any generated IDs
                LoadAllAssignments();
                LoadMachinesForShift(currentlySelectedShift);

                hasUnsavedShiftMachines = false;
                UpdateStatusBar();

                MessageBox.Show("Assignments saved successfully!", "Success",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving assignments: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }
        private void BtnRefreshShiftMachines_Click(object sender, RoutedEventArgs e)
        {
            if (hasUnsavedShiftMachines)
            {
                var result = MessageBox.Show(
                    "You have unsaved changes to machine assignments. Refreshing will lose these changes. Continue?",
                    "Unsaved Changes",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.No) return;
            }

            LoadAllAssignments();
            LoadMachinesForShift(currentlySelectedShift);
            hasUnsavedShiftMachines = false;
            UpdateStatusBar();
        }
        /// <summary>
        /// Checks if any of the new assignments would overlap with existing assignments
        /// </summary>
        private (bool hasOverlap, List<string> conflicts) CheckForOverlappingAssignments(ShiftMachineAssignments assignmentsToCheck)
        {
            var conflicts = new List<string>();

            foreach (ShiftMachineAssignment newAssignment in assignmentsToCheck)
            {
                // Skip if this assignment already exists in the database (has an ID)
                if (newAssignment.AssignmentID != 0) continue;

                // Get the shift times for this assignment
                var newShift = shifts.Cast<Shift>().FirstOrDefault(s => s.ShiftID == newAssignment.ShiftID);
                if (newShift == null) continue;

                // Check against ALL assignments in the global cache for the same machine and day
                foreach (ShiftMachineAssignment existingAssignment in assignments)
                {
                    // Skip if same assignment
                    if (existingAssignment.AssignmentID == newAssignment.AssignmentID) continue;

                    // Skip if different machine or day
                    if (existingAssignment.MachineRecNum != newAssignment.MachineRecNum) continue;
                    if (existingAssignment.DayOfWeek != newAssignment.DayOfWeek) continue;
                    if (!existingAssignment.IsActive) continue;

                    // Get the existing shift times
                    var existingShift = shifts.Cast<Shift>().FirstOrDefault(s => s.ShiftID == existingAssignment.ShiftID);
                    if (existingShift == null) continue;

                    // Check for time overlap
                    if (ShiftsOverlap(newShift.StartTime, newShift.EndTime,
                                    existingShift.StartTime, existingShift.EndTime))
                    {
                        conflicts.Add(
                            $"Machine {newAssignment.MachineIdJensen} on {newAssignment.DayOfWeekName}: " +
                            $"'{newShift.ShiftName}' ({newShift.StartTime:hh\\:mm}-{newShift.EndTime:hh\\:mm}) " +
                            $"overlaps with '{existingShift.ShiftName}' ({existingShift.StartTime:hh\\:mm}-{existingShift.EndTime:hh\\:mm})"
                        );
                    }
                }
            }

            return (conflicts.Count > 0, conflicts);
        }

        /// <summary>
        /// Determines if two time ranges overlap, handling cross-midnight shifts
        /// </summary>
        private bool ShiftsOverlap(TimeSpan start1, TimeSpan end1, TimeSpan start2, TimeSpan end2)
        {
            bool shift1CrossesMidnight = end1 < start1;
            bool shift2CrossesMidnight = end2 < start2;

            if (!shift1CrossesMidnight && !shift2CrossesMidnight)
            {
                // Normal case: neither shift crosses midnight
                // Overlap if: NOT (end1 <= start2 OR start1 >= end2)
                return !(end1 <= start2 || start1 >= end2);
            }
            else if (shift1CrossesMidnight && !shift2CrossesMidnight)
            {
                // Shift 1 crosses midnight, shift 2 doesn't
                // Shift 1 occupies [start1 to midnight) and [midnight to end1)
                // They overlap unless shift 2 is entirely in the gap
                return !(end2 <= start1 && start2 >= end1);
            }
            else if (!shift1CrossesMidnight && shift2CrossesMidnight)
            {
                // Shift 2 crosses midnight, shift 1 doesn't
                return !(end1 <= start2 && start1 >= end2);
            }
            else
            {
                // Both shifts cross midnight - they always overlap
                return true;
            }
        }
        #endregion

        #region Shift Settings
        private void LoadShiftSettings()
        {
            shiftSettings = dataAccess.GetShiftSettings(null, true);
            dgShiftSettings.ItemsSource = shiftSettings;

            if (shiftSettings is INotifyCollectionChanged collectionChanged)
            {
                collectionChanged.CollectionChanged += ShiftSettings_CollectionChanged;
            }

            foreach (ShiftSetting setting in shiftSettings)
            {
                setting.PropertyChanged += ShiftSettings_PropertyChanged;
            }

            hasUnsavedShiftSettings = false;
        }

        private void ShiftSettings_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (ShiftSetting setting in e.NewItems)
                {
                    setting.PropertyChanged += ShiftSettings_PropertyChanged;
                }
                MarkShiftSettingsChanged();
            }
        }

        private void ShiftSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            MarkShiftSettingsChanged();
        }

        private void BtnSaveShiftSettings_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                dgShiftSettings.CommitEdit(DataGridEditingUnit.Row, true);
                dgShiftSettings.CommitEdit(DataGridEditingUnit.Cell, true);

                Mouse.OverrideCursor = Cursors.Wait;
                shiftSettings.UpdateToDB();

                MessageBox.Show("Settings saved successfully", "Success",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                LoadShiftSettings();
                hasUnsavedShiftSettings = false;
                UpdateStatusBar();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving settings: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void BtnRefreshShiftSettings_Click(object sender, RoutedEventArgs e)
        {
            if (hasUnsavedShiftSettings)
            {
                var result = MessageBox.Show(
                    "You have unsaved changes. Refreshing will lose these changes. Continue?",
                    "Unsaved Changes",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.No) return;
            }

            LoadShiftSettings();
            hasUnsavedShiftSettings = false;
            UpdateStatusBar();
        }
        #endregion

        #region Operator Settings
        private void LoadOperatorSettings()
        {
            operatorSettings = dataAccess.GetOperatorSettings(null, true);
            dgOperatorSettings.ItemsSource = operatorSettings;

            if (operatorSettings is INotifyCollectionChanged collectionChanged)
            {
                collectionChanged.CollectionChanged += OperatorSettings_CollectionChanged;
            }

            foreach (OperatorSetting setting in operatorSettings)
            {
                setting.PropertyChanged += OperatorSettings_PropertyChanged;
            }

            hasUnsavedOperatorSettings = false;
        }

        private void OperatorSettings_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (OperatorSetting setting in e.NewItems)
                {
                    setting.PropertyChanged += OperatorSettings_PropertyChanged;
                }
                MarkOperatorSettingsChanged();
            }
        }

        private void OperatorSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            MarkOperatorSettingsChanged();
        }

        private void BtnAddOperatorSetting_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OperatorSetting newSetting = new OperatorSetting
                {
                    OperatorRecNum = 0,
                    CanReplaceOperator = false,
                    ModifiedDate = DateTime.Now,
                    ForceNew = true
                };
                operatorSettings.Add(newSetting);
                dgOperatorSettings.Items.Refresh();
                MarkOperatorSettingsChanged();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding operator setting: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnDeleteOperatorSetting_Click(object sender, RoutedEventArgs e)
        {
            if (dgOperatorSettings.SelectedItems.Count == 0) return;

            var result = MessageBox.Show("Delete selected operator setting(s)?", "Confirm Delete",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    Mouse.OverrideCursor = Cursors.Wait;

                    var itemsToDelete = dgOperatorSettings.SelectedItems.Cast<OperatorSetting>().ToList();

                    foreach (var setting in itemsToDelete)
                    {
                        setting.DeleteRecord = true;
                    }

                    dgOperatorSettings.SelectedItems.Clear();
                    operatorSettings.UpdateToDB();
                    LoadOperatorSettings();

                    hasUnsavedOperatorSettings = false;
                    UpdateStatusBar();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting operator settings: {ex.Message}", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    Mouse.OverrideCursor = null;
                }
            }
        }

        private void BtnSaveOperatorSettings_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                dgOperatorSettings.CommitEdit(DataGridEditingUnit.Row, true);
                dgOperatorSettings.CommitEdit(DataGridEditingUnit.Cell, true);

                Mouse.OverrideCursor = Cursors.Wait;
                operatorSettings.UpdateToDB();

                MessageBox.Show("Operator settings saved successfully", "Success",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                LoadOperatorSettings();
                hasUnsavedOperatorSettings = false;
                UpdateStatusBar();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving operator settings: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void BtnRefreshOperatorSettings_Click(object sender, RoutedEventArgs e)
        {
            if (hasUnsavedOperatorSettings)
            {
                var result = MessageBox.Show(
                    "You have unsaved changes. Refreshing will lose these changes. Continue?",
                    "Unsaved Changes",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.No) return;
            }

            LoadOperatorSettings();
            hasUnsavedOperatorSettings = false;
            UpdateStatusBar();
        }
        #endregion
    }
}