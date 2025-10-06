using Dynamic.DataLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CentralOperatorShiftEditor
{
    public partial class AddMachinesDialog : Window, INotifyPropertyChanged
    {
        private SqlDataAccess dataAccess;
        private Shift selectedShift;
        private ShiftMachineAssignments existingAssignments; // Add this field
        private List<MachineSelectionItem> allMachines;
        private List<MachineSelectionItem> filteredMachines;

        public event PropertyChangedEventHandler PropertyChanged;

        public List<ShiftMachineAssignment> NewAssignments { get; private set; }

        public AddMachinesDialog(Shift shift, ShiftMachineAssignments assignments)
        {
            InitializeComponent();
            dataAccess = SqlDataAccess.Singleton;
            selectedShift = shift;
            existingAssignments = assignments; // Store the passed collection
            NewAssignments = new List<ShiftMachineAssignment>();

            LoadData();
        }

        private void LoadData()
        {
            // Display shift information
            txtShiftName.Text = selectedShift.ShiftName;
            txtShiftTime.Text = $"{selectedShift.StartTime:hh\\:mm} - {selectedShift.EndTime:hh\\:mm}";

            // Load all machines
            LoadMachines();

            // Set default days (weekdays)
            chkMonday.IsChecked = true;
            chkTuesday.IsChecked = true;
            chkWednesday.IsChecked = true;
            chkThursday.IsChecked = true;
            chkFriday.IsChecked = true;

            UpdateSelectionSummary();
        }

        private void LoadMachines()
        {
            try
            {
                // Get all machines from database
                var machines = dataAccess.GetAllMachines(null, true);

                // Filter existing assignments for this shift from the passed collection
                var assignedMachineIds = existingAssignments
                    .Cast<ShiftMachineAssignment>()
                    .Where(a => a.ShiftID == selectedShift.ShiftID && a.IsActive)
                    .Select(a => a.MachineRecNum)
                    .Distinct()
                    .ToHashSet();

                // Create selection items for machines
                allMachines = new List<MachineSelectionItem>();
                foreach (var machine in machines)
                {
                    var item = new MachineSelectionItem
                    {
                        RecNum = ((Machine)machine).RecNum,
                        IdJensen = ((Machine)machine).IdJensen,
                        Description = ((Machine)machine).ShortDescription ?? "",
                        IsSelected = false,
                        IsAlreadyAssigned = assignedMachineIds.Contains(((Machine)machine).RecNum)
                    };

                    // Subscribe to selection changes
                    item.PropertyChanged += MachineSelectionItem_PropertyChanged;
                    allMachines.Add(item);
                }

                filteredMachines = new List<MachineSelectionItem>(allMachines);
                dgAvailableMachines.ItemsSource = filteredMachines;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading machines: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void MachineSelectionItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsSelected")
            {
                UpdateSelectionSummary();
            }
        }
        private void TxtSearchMachines_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterMachines();
        }

        private void FilterMachines()
        {
            string searchText = txtSearchMachines.Text?.ToLower() ?? "";

            if (string.IsNullOrWhiteSpace(searchText))
            {
                filteredMachines = new List<MachineSelectionItem>(allMachines);
            }
            else
            {
                filteredMachines = allMachines.Where(m =>
                    m.IdJensen.ToString().Contains(searchText) ||
                    m.Description.ToLower().Contains(searchText) ||
                    m.RecNum.ToString().Contains(searchText)
                ).ToList();
            }

            dgAvailableMachines.ItemsSource = null;
            dgAvailableMachines.ItemsSource = filteredMachines;
            UpdateSelectionSummary();
        }

        private void BtnClearSearch_Click(object sender, RoutedEventArgs e)
        {
            txtSearchMachines.Text = "";
        }

        private void ChkSelectAll_Checked(object sender, RoutedEventArgs e)
        {
            foreach (var machine in filteredMachines)
            {
                machine.IsSelected = true;
            }
            dgAvailableMachines.Items.Refresh();
            UpdateSelectionSummary();
        }

        private void ChkSelectAll_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (var machine in filteredMachines)
            {
                machine.IsSelected = false;
            }
            dgAvailableMachines.Items.Refresh();
            UpdateSelectionSummary();
        }

        // Day selection quick buttons
        private void BtnSelectAllDays_Click(object sender, RoutedEventArgs e)
        {
            chkSunday.IsChecked = true;
            chkMonday.IsChecked = true;
            chkTuesday.IsChecked = true;
            chkWednesday.IsChecked = true;
            chkThursday.IsChecked = true;
            chkFriday.IsChecked = true;
            chkSaturday.IsChecked = true;
        }

        private void BtnSelectWeekdays_Click(object sender, RoutedEventArgs e)
        {
            chkSunday.IsChecked = false;
            chkMonday.IsChecked = true;
            chkTuesday.IsChecked = true;
            chkWednesday.IsChecked = true;
            chkThursday.IsChecked = true;
            chkFriday.IsChecked = true;
            chkSaturday.IsChecked = false;
        }

        private void BtnSelectWeekends_Click(object sender, RoutedEventArgs e)
        {
            chkSunday.IsChecked = true;
            chkMonday.IsChecked = false;
            chkTuesday.IsChecked = false;
            chkWednesday.IsChecked = false;
            chkThursday.IsChecked = false;
            chkFriday.IsChecked = false;
            chkSaturday.IsChecked = true;
        }

        private void BtnClearDays_Click(object sender, RoutedEventArgs e)
        {
            chkSunday.IsChecked = false;
            chkMonday.IsChecked = false;
            chkTuesday.IsChecked = false;
            chkWednesday.IsChecked = false;
            chkThursday.IsChecked = false;
            chkFriday.IsChecked = false;
            chkSaturday.IsChecked = false;
        }

        private void UpdateSelectionSummary()
        {
            int selectedCount = filteredMachines.Count(m => m.IsSelected);
            txtSelectionSummary.Text = $"{selectedCount} machine(s) selected";
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedMachines = allMachines.Where(m => m.IsSelected).ToList();

                if (selectedMachines.Count == 0)
                {
                    MessageBox.Show("Please select at least one machine.", "Validation",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var selectedDays = GetSelectedDays();

                if (selectedDays.Count == 0)
                {
                    MessageBox.Show("Please select at least one day of the week.", "Validation",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                NewAssignments.Clear();
                foreach (var machine in selectedMachines)
                {
                    foreach (var day in selectedDays)
                    {
                        var assignment = new ShiftMachineAssignment
                        {
                            // DON'T set AssignmentID - let the database assign it
                            ShiftID = selectedShift.ShiftID,
                            MachineRecNum = machine.RecNum,
                            MachineIdJensen = machine.IdJensen,
                            DayOfWeek = day.Key,
                            IsActive = true,
                            CreatedDate = DateTime.Now,
                            ForceNew = true
                        };
                        NewAssignments.Add(assignment);
                    }
                }

                int totalAssignments = NewAssignments.Count;
                var result = MessageBox.Show(
                    $"Add {totalAssignments} assignment(s) to the shift?\n" +
                    $"({selectedMachines.Count} machine(s) × {selectedDays.Count} day(s))\n\n" +
                    $"You'll need to click 'Save Assignments' to save them to the database.",
                    "Confirm",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    this.DialogResult = true;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating assignments: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private Dictionary<int, string> GetSelectedDays()
        {
            var days = new Dictionary<int, string>();

            if (chkSunday.IsChecked == true) days.Add(0, "Sunday");
            if (chkMonday.IsChecked == true) days.Add(1, "Monday");
            if (chkTuesday.IsChecked == true) days.Add(2, "Tuesday");
            if (chkWednesday.IsChecked == true) days.Add(3, "Wednesday");
            if (chkThursday.IsChecked == true) days.Add(4, "Thursday");
            if (chkFriday.IsChecked == true) days.Add(5, "Friday");
            if (chkSaturday.IsChecked == true) days.Add(6, "Saturday");

            return days;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }

    // Helper class for machine selection
    public class MachineSelectionItem : INotifyPropertyChanged
    {
        private bool isSelected;

        public int RecNum { get; set; }
        public int IdJensen { get; set; }
        public string Description { get; set; }
        public bool IsAlreadyAssigned { get; set; }

        public bool IsSelected
        {
            get => isSelected;
            set
            {
                if (isSelected != value)
                {
                    isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}