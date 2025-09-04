using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Security.Cryptography;
using Ge_Mac.DataLayer;

namespace OperatorLogin
{
    /// <summary>
    /// Interaction logic for TagEditWindow.xaml
    /// </summary>
    public partial class TagEditWindow : Window
    {
        public string EditCaption_trans = "Caption";
        public string version_trans = "Version";
        public string operator_trans = "Operator";
        public string tag_trans = "Tag";
        public string close_trans = "Close";
        public string update_trans = "Update";
        public string add_trans = "Add";
        public string refresh_trans = "Refresh";
        public string password_trans = "Password";
        public string store_trans = "Store";
        public string tagdata_trans = "Tag Data";
        public string tagid_trans = "Tag ID";
        public string delete_trans = "Delete";

        public string Password = string.Empty;

        public Operators operators;
        public Tags tags;

        public TagEditWindow()
        {
            InitializeComponent();
        }

        public void SetTagEdit(string aTag)
        {
            txtTagEdit.Text = aTag;
        }

        public void SetOperators(Operators ops)
        {
            operators = ops;
            cbxOperators.ItemsSource = operators;
            cbxOperators.DisplayMemberPath = "ShortDescription";
            cbxOperators.IsReadOnly = true;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string tagData = txtTagEdit.Text;
            Operator op = (Operator)cbxOperators.SelectedItem;
            if (op != null)
            {
                Tag tag = tags.GetByRefs("tblOperators", op.idJensen);
                if (tag == null)
                {
                    tag = new Tag();
                    tag.TagID = -1;
                    tag.ReferenceTable = "tblOperators";
                    tag.ReferenceID = op.idJensen;
                }
                tag.TagData = tagData;
                SqlDataAccess da=SqlDataAccess.Singleton;
                if (tag.TagID == -1)
                {
                    da.InsertNewTag(tag);
                }
                else
                {
                    da.UpdateTag(tag);
                }
            }
        }

        private void cbxOperators_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Operator op = (Operator)cbxOperators.SelectedItem;
            if (op != null)
            {
                btnAdd.IsEnabled = true;
                //Tag tag = tags.GetByRefs("tblOperators", op.idJensen);
                //if (tag != null)
                //{
                //    txtTagEdit.Text = tag.TagData;
                //}
            }
            else
            {
                btnAdd.IsEnabled = false;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dgTags.ItemsSource = tags;
            lblHeader.Content = EditCaption_trans;
            lblOperator.Content = operator_trans;
            dgColOperator.Header = operator_trans;
            lblTag.Content = tag_trans;
            lblPassword.Content = password_trans;
            btnClose.Content = close_trans;
            btnUpdate.Content = update_trans;
            btnAdd.Content = add_trans;
            btnRefresh.Content = refresh_trans;
            btnStore.Content = store_trans;
            dgColDelete.Header = delete_trans;
            dgColRFIDTagData.Header = tagdata_trans;
            dgColTagID.Header = tagid_trans;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            Operator op = (Operator)cbxOperators.SelectedItem;
            if (op != null)
            {
                btnAdd.IsEnabled = true;
                Tag tag = tags.GetByRefs("tblOperators", op.idJensen);
                if (tag != null)
                {
                    dgTags.SelectedItem = tag;
                }
                else
                {
                    string testdata = txtTagEdit.Text.Trim();
                    bool alreadyExists = false;
                    foreach (Tag t in tags)
                    {
                        alreadyExists |= t.TagData == testdata;
                    }
                    tag = new Tag();
                    tag.TagID = -1;
                    tag.ReferenceTable = "tblOperators";
                    tag.ReferenceID = op.idJensen;
                    if (!alreadyExists)
                    {
                        tag.TagData = txtTagEdit.Text;
                    }
                    else
                    {
                        tag.TagData = string.Empty;
                    }
                    tags.Add(tag);
                    tags.Reset();
                    dgTags.SelectedItem = tag;
                }
            }
            e.Handled = true;
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            SqlDataAccess da = SqlDataAccess.Singleton;
            cbxOperators.ItemsSource = null;
            cbxOperators.Items.Clear();
            operators = da.GetAllActiveOperators();
            operators.Sort();
            cbxOperators.ItemsSource = operators;
            tags = da.GetAllTags(true);
            tags.Reset();
            e.Handled = true;
        }

        private void btnStore_Click(object sender, RoutedEventArgs e)
        {
            foreach (Tag t in tags)
            {
                if (t.TagData == string.Empty)
                {
                    t.DeleteRecord = true;
                }
            }
            tags.UpdateToDB();
            txtPassword.Text = string.Empty;
            tags.Reset();
            e.Handled = true;
        }

        private void btnAdd_TouchDown(object sender, TouchEventArgs e)
        {
            btnAdd_Click(sender, e);
        }

        private void btnRefresh_TouchDown(object sender, TouchEventArgs e)
        {
            btnRefresh_Click(sender, e);
        }

        private void btnStore_TouchDown(object sender, TouchEventArgs e)
        {
            btnStore_Click(sender, e);
        }

        private void txtTagEdit_TextChanged(object sender, TextChangedEventArgs e)
        {
            btnUpdate.IsEnabled = ((dgTags.SelectedIndex != -1) && (txtTagEdit.Text.Length > 0));
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            string testdata = txtTagEdit.Text.Trim();
            bool alreadyExists = false;
            foreach (Tag tag in tags)
            {
                alreadyExists |= tag.TagData == testdata;
            }
            if (!alreadyExists)
            {
                Tag tag = (Tag)dgTags.SelectedItem;
                if (tag != null)
                {
                    tag.TagData = txtTagEdit.Text.Trim();
                    tags.Reset();
                }
            }
        }

        private void dgTags_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnUpdate.IsEnabled = ((dgTags.SelectedIndex != -1) && (txtTagEdit.Text.Length > 0));
        }

        public static string EncodePasswordToBase64(string password)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(password);
            //byte[] inArray = HashAlgorithm.Create("SHA1").ComputeHash(bytes);
            byte[] inArray = HashAlgorithm.Create("MD5").ComputeHash(bytes);
            return Convert.ToBase64String(inArray);
        }

        private bool validatePassword()
        {
            string test = EncodePasswordToBase64(txtPassword.Text);
            return (test == Password);
        }

        private void passwordBox1_PasswordChanged(object sender, RoutedEventArgs e)
        {
        }

        private void Password_TextChanged(object sender, TextChangedEventArgs e)
        {
            btnStore.IsEnabled = validatePassword();
        }

    }
}
