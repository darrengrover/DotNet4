using Dynamic.DataLayer;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace CentralOperatorShiftEditor
{
    public partial class ShiftEditorWindow : Window
    {
        private SqlDataAccess dataAccess;
        private Shifts shifts;
        private ShiftMachineAssignments assignments;
        private ShiftSettings shiftSettings;
        private OperatorSettings operatorSettings;

        private ListCollectionView shiftsView;
        private ListCollectionView assignmentsView;

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
                LoadAssignments();
                LoadShiftSettings();
                LoadOperatorSettings();
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

        #region Shifts
        private void LoadShifts()
        {
            shifts = dataAccess.GetShifts(null, true);
            shiftsView = CollectionViewSource.GetDefaultView(shifts) as ListCollectionView;
            dgShifts.ItemsSource = shiftsView;
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
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding shift: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnDeleteShift_Click(object sender, RoutedEventArgs e)
        {
            if (dgShifts.SelectedItems.Count > 0)
            {
                var result = MessageBox.Show("Delete selected shift(s)?", "Confirm Delete",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        Mouse.OverrideCursor = Cursors.Wait;

                        // Copy selected items to a separate list to avoid collection modification issues
                        var itemsToDelete = new System.Collections.Generic.List<Shift>();
                        foreach (Shift shift in dgShifts.SelectedItems)
                        {
                            itemsToDelete.Add(shift);
                        }

                        // Mark all for deletion
                        foreach (var shift in itemsToDelete)
                        {
                            shift.DeleteRecord = true;
                        }

                        // Clear selection before updating to prevent DataGrid issues
                        dgShifts.SelectedItems.Clear();

                        // Delete from database
                        shifts.UpdateToDB();

                        // Reload from database
                        LoadShifts();
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
        }

        private void BtnSaveShifts_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Commit any pending edits
                dgShifts.CommitEdit(DataGridEditingUnit.Row, true);
                dgShifts.CommitEdit(DataGridEditingUnit.Cell, true);

                Mouse.OverrideCursor = Cursors.Wait;
                shifts.UpdateToDB();
                MessageBox.Show("Shifts saved successfully", "Success",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                LoadShifts();
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
            LoadShifts();
        }
        #endregion

        #region Assignments
        private void LoadAssignments()
        {
            assignments = dataAccess.GetShiftMachineAssignments(null, true);
            assignmentsView = CollectionViewSource.GetDefaultView(assignments) as ListCollectionView;
            dgAssignments.ItemsSource = assignmentsView;
        }

        private void BtnAddAssignment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ShiftMachineAssignment newAssignment = new ShiftMachineAssignment
                {
                    ShiftID = 1,
                    MachineRecNum = 0,
                    MachineIdJensen = "",
                    DayOfWeek = 2,
                    DayOfWeekName = "Monday",
                    IsActive = true,
                    CreatedDate = DateTime.Now,
                    ForceNew = true
                };
                assignments.Add(newAssignment);
                assignmentsView.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding assignment: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnDeleteAssignment_Click(object sender, RoutedEventArgs e)
        {
            if (dgAssignments.SelectedItems.Count > 0)
            {
                var result = MessageBox.Show("Delete selected assignment(s)?", "Confirm Delete",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        Mouse.OverrideCursor = Cursors.Wait;

                        // Copy selected items to a separate list
                        var itemsToDelete = new System.Collections.Generic.List<ShiftMachineAssignment>();
                        foreach (ShiftMachineAssignment assignment in dgAssignments.SelectedItems)
                        {
                            itemsToDelete.Add(assignment);
                        }

                        // Mark all for deletion
                        foreach (var assignment in itemsToDelete)
                        {
                            assignment.DeleteRecord = true;
                        }

                        // Clear selection before updating
                        dgAssignments.SelectedItems.Clear();

                        // Delete from database
                        assignments.UpdateToDB();

                        // Reload from database
                        LoadAssignments();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting assignments: {ex.Message}", "Error",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    finally
                    {
                        Mouse.OverrideCursor = null;
                    }
                }
            }
        }

        private void BtnSaveAssignments_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Commit any pending edits
                dgAssignments.CommitEdit(DataGridEditingUnit.Row, true);
                dgAssignments.CommitEdit(DataGridEditingUnit.Cell, true);

                Mouse.OverrideCursor = Cursors.Wait;
                assignments.UpdateToDB();
                MessageBox.Show("Assignments saved successfully", "Success",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                LoadAssignments();
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

        private void BtnRefreshAssignments_Click(object sender, RoutedEventArgs e)
        {
            LoadAssignments();
        }
        #endregion

        #region Shift Settings
        private void LoadShiftSettings()
        {
            shiftSettings = dataAccess.GetShiftSettings(null, true);
            dgShiftSettings.ItemsSource = shiftSettings;
        }

        private void BtnSaveShiftSettings_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Commit any pending edits
                dgShiftSettings.CommitEdit(DataGridEditingUnit.Row, true);
                dgShiftSettings.CommitEdit(DataGridEditingUnit.Cell, true);

                Mouse.OverrideCursor = Cursors.Wait;
                shiftSettings.UpdateToDB();
                MessageBox.Show("Settings saved successfully", "Success",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                LoadShiftSettings();
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
            LoadShiftSettings();
        }
        #endregion

        #region Operator Settings
        private void LoadOperatorSettings()
        {
            operatorSettings = dataAccess.GetOperatorSettings(null, true);
            dgOperatorSettings.ItemsSource = operatorSettings;
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
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding operator setting: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnDeleteOperatorSetting_Click(object sender, RoutedEventArgs e)
        {
            if (dgOperatorSettings.SelectedItems.Count > 0)
            {
                var result = MessageBox.Show("Delete selected operator setting(s)?", "Confirm Delete",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        Mouse.OverrideCursor = Cursors.Wait;

                        // Copy selected items to a separate list
                        var itemsToDelete = new System.Collections.Generic.List<OperatorSetting>();
                        foreach (OperatorSetting setting in dgOperatorSettings.SelectedItems)
                        {
                            itemsToDelete.Add(setting);
                        }

                        // Mark all for deletion
                        foreach (var setting in itemsToDelete)
                        {
                            setting.DeleteRecord = true;
                        }

                        // Clear selection before updating
                        dgOperatorSettings.SelectedItems.Clear();

                        // Delete from database
                        operatorSettings.UpdateToDB();

                        // Reload from database
                        LoadOperatorSettings();
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
        }

        private void BtnSaveOperatorSettings_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Commit any pending edits
                dgOperatorSettings.CommitEdit(DataGridEditingUnit.Row, true);
                dgOperatorSettings.CommitEdit(DataGridEditingUnit.Cell, true);

                Mouse.OverrideCursor = Cursors.Wait;
                operatorSettings.UpdateToDB();
                MessageBox.Show("Operator settings saved successfully", "Success",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                LoadOperatorSettings();
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
            LoadOperatorSettings();
        }
        #endregion
    }
}