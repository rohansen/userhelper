using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.DirectoryServices.AccountManagement;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UserHelperCL;
using UserHelperCL.Database;
using UserHelperCL.Models;
using UserHelperCL.UserFunctions;


namespace UserHelper
{
    public partial class UserHelperMain : Form
    {
        //IISManagement iis = new IISManagement();
        //SQLManagement sql = new SQLManagement();
        //PrincipalContext context = new PrincipalContext(ContextType.Machine);
        
        private UserInteraction _userInteraction = new UserInteraction();
        private StudentsModel db;
        public UserHelperMain()
        {
            InitializeComponent();
            db = _userInteraction.GetDbContext();
            BatchState.State = UserProcessState.INITIAL;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void UserHelperMain_Load(object sender, EventArgs e)
        {
            //add teams to comboboxes and listbox in team management
            lbExistingTeams.DisplayMember = "Name";
            cbTeam.DisplayMember = "Name";
            cbTeam.ValueMember = "TeamId";
            cbTeamMany.DisplayMember = "Name";
            cbTeamMany.ValueMember = "TeamId";
            comboCredentialsTeam.DisplayMember = "Name";
            comboCredentialsTeam.ValueMember = "TeamId";
            comboCredentialsTeam.Items.Add("Alle");
            lbStudentList.DisplayMember = "Name";
            lbStudentList.ValueMember = "StudentId";

            Team[] teams = db.Teams.ToArray();
            Student[] studentObjs = db.Students.Include("Team").Include("Credentials").ToArray();
            lbExistingTeams.Items.AddRange(teams);
            cbTeam.Items.AddRange(teams);
            cbTeamMany.Items.AddRange(teams);
            comboCredentialsTeam.Items.AddRange(teams);
            lbStudentList.Items.AddRange(studentObjs);



            GroupPrincipal group = new GroupPrincipal(_userInteraction.GetPrincipalContext());
            PrincipalSearcher src = new PrincipalSearcher(group);

            PrincipalSearchResult<Principal> groups = src.FindAll();

            //Add windows usergroups to listboxes
            lbUserGroup.Items.AddRange(groups.ToArray());
            lbUserGroupMany.Items.AddRange(groups.ToArray());
            int defaultPort = 0;
            if (db.Credentials.Any())
            {
                defaultPort = db.Credentials.Max(x => x.WebsitePort) + 1;
            }


            txtPort.Text = defaultPort + "";
            txtPortMany.Text = defaultPort + "";


        }


        private void btnCreateMany_Click(object sender, EventArgs e)
        {

            if (txtUserNamePrefix.Text != "" && txtUserNameSuffix.Text != "" && cbTeamMany.SelectedItem != null && txtPasswordSuffix.Text != "" && txtPortMany.Text != "" && lbUserGroupMany.SelectedItem != null && txtUserNames.Text != "")
            {

                bool disablepwchange = chkDisablePWChangeMany.Checked;
                bool pwneverexpires = chkNoPWExpirationMany.Checked;


                int port = Convert.ToInt32(txtPortMany.Text);
                int teamId = (int)((Team)cbTeamMany.SelectedItem).TeamId;

                _userInteraction.CreateMany(txtUserNamePrefix.Text, Convert.ToInt32(txtUserNameSuffix.Text), teamId, txtPasswordSuffix.Text, port, lbUserGroupMany.Text, txtUserNames.Text, disablepwchange, pwneverexpires);
                MessageBox.Show("Done");

            }
        }

        



        private void btnCreateSingleUser_Click(object sender, EventArgs e)
        {
            if (txtUserName.Text != "" && txtPassword.Text != "" && txtPort.Text != "" && txtName.Text != "" && cbTeam.SelectedItem != null && lbUserGroup.SelectedItem != null)
            {
                Team team = db.Teams.Where(x=>x.TeamId==((Team)cbTeam.SelectedItem).TeamId).FirstOrDefault();
                bool disablepwchange = chkPWChange.Checked;
                bool pwneverexpires = chkNoPWExpiration.Checked;
                int port = Convert.ToInt32(txtPort.Text);//


                _userInteraction.CreateSingleUser(txtUserName.Text, txtPassword.Text, port, txtName.Text, team.TeamId, lbUserGroup.Text, disablepwchange, pwneverexpires);
                MessageBox.Show("Done");
            }
        }
     

        private void btnCreateTeam_Click(object sender, EventArgs e)
        {
            try
            {
                
                Team t = _userInteraction.CreateTeam(txtTeamName.Text, dtpStartDate.Value, dtpEndDate.Value);
                //update checkboxes and lists
                lbExistingTeams.Items.Add(t);
                cbTeam.Items.Add(t);
                cbTeamMany.Items.Add(t);
                comboCredentialsTeam.Items.Add(t);
            }
            catch (Exception ex)
            {
                
                MessageBox.Show("Der er sket en fejl:\n" + ex.Message, "Fejl", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void comboCredentialsTeam_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ComboBox cb = (ComboBox)sender;
                lbStudentList.Items.Clear();

                if (cb.SelectedItem.ToString() == "Alle")
                {
                    Student[] students = db.Students.OrderBy(x => x.Name).ToArray();

                    lbStudentList.Items.AddRange(students);
                }
                else
                {
                    Team t = (Team)cb.SelectedItem;
                    Student[] students = db.Students.Where(x => x.Team.TeamId == t.TeamId).OrderBy(x => x.Name).ToArray();

                    lbStudentList.Items.AddRange(students);
                }
            }
            catch (Exception ex)
            {
                
                MessageBox.Show("Der er sket en fejl:\n" + ex.Message, "Fejl", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }

        private void txtQuickSearchUser_TextChanged(object sender, EventArgs e)
        {
            try
            {
                TextBox tb = (TextBox)sender;
                comboCredentialsTeam.SelectedItem = "Alle";
                if (tb.Text.Length >= 2)
                {
                    lbStudentList.Items.Clear();
                    Student[] students = db.Students.Where(x => x.Name.Contains(tb.Text)).OrderBy(x => x.Name).ToArray();

                    lbStudentList.Items.AddRange(students);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Der er sket en fejl:\n" + ex.Message, "Fejl", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (lbExistingTeams.SelectedItem != null)
            {
                try { 
                Team teamToPrint = (Team)lbExistingTeams.SelectedItem;
                _userInteraction.CreatePDF(teamToPrint);
                System.Diagnostics.Process.Start(@"StudentList.pdf");

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Der er sket en fejl:\n" + ex.Message, "Fejl", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }

        }

        private void onFormClose(object sender, FormClosingEventArgs e)
        {
            db.Dispose();
        }

        private void btnDeleteTeam(object sender, EventArgs e)
        {
            try
            {
                Team t = db.Teams.Find(((Team)lbExistingTeams.SelectedItem).TeamId);
                string warningText = "Er du sikker på, at du vil slette holdet: " + t.Name + " og alle studerende tilknyttet?\nDenne operation sletter også deres Websites, FTP Directory og Database, samt login til alle disse";
                DialogResult result = MessageBox.Show(warningText, "Advarsel!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {

            
                    
                    //Find oprettede brugere på det angivne hold, der skal laves en ToList, da DeleteUserWebsiteFTPDatabase ellers ændrer i collectionen, der køres foreach over(må man ikke )
                    var students = t.Students.ToList();
                    //Sletter Windows bruger, IIS Website, IIS FTP (VirtualDirectory), Database, samt databaseserver login, for hver funden bruger.
                    foreach (var item in students)
                    {
                        _userInteraction.DeleteUserWebsiteFTPDatabase(item.StudentId);
                    }

                    //sletter herefter holdet i databasen, og gemmer ændringerne til databasen.
                    db.Teams.Remove(t);
                    db.SaveChanges();

                    //opdaterer GUI, så det fjernede hold ikke længere er der.
                    cbTeam.Items.Remove(t);
                    cbTeamMany.Items.Remove(t);
                    lbExistingTeams.Items.Remove(t);
                    comboCredentialsTeam.Items.Remove(t);

                    BatchState.State = UserProcessState.INITIAL;
                    MessageBox.Show("Team, Students and credentials removed, aswell as FTP Virtual directory, Website and Windows User Credentials");
                }
            }
            catch (Exception ex)
            {
                
                MessageBox.Show("Der er sket en fejl:\n" + ex.Message, "Fejl", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDeleteStudentAndCredentials_Click(object sender, EventArgs e)
        {
            Student s = (Student)lbStudentList.SelectedItem;
            try
            {
                
                string warningText = "Er du sikker på, at du vil slette eleven: " + s.Name + "?\nDenne operation sletter også Website, FTP Directory og Database, samt logins";
                DialogResult result = MessageBox.Show(warningText, "Advarsel!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    
                    _userInteraction.DeleteUserWebsiteFTPDatabase(s.StudentId);
                    db.SaveChanges();
                    lbStudentList.Items.Remove(lbStudentList.SelectedItem);
                   
                 
                    
                    MessageBox.Show("Students and credentials removed, aswell as FTP Virtual directory, Website and Windows User Credentials");
                }
            }
            catch (Exception ex)
            {
                _userInteraction.RollbackOnError(BatchState.State, s.Credentials.FTPUserName);
                MessageBox.Show("Der er sket en fejl:\n" + ex.Message, "Fejl", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void lbStudentList_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (lbStudentList.SelectedItem != null) { 
                ListBox lb = (ListBox)sender;
                Student stud = (Student)lb.SelectedItem;
                txtStudentNameShow.Text = stud.Name;
                txtTeamNameShow.Text = stud.Team.Name;
                txtDbUserNameShow.Text = stud.Credentials.DatabaseUserName;
                txtDbPasswordShow.Text = stud.Credentials.DatabasePassword;
                txtFTPUsernameShow.Text = stud.Credentials.FTPUserName;
                txtFTPPasswordShow.Text = stud.Credentials.FTPPassword;
                txtUrlShow.Text = stud.Credentials.WebsitePort.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Der er sket en fejl:\n" + ex.Message, "Fejl", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void DeleteStudentAndCredentials(int studentId)
        {
            Student s = new Student { StudentId = studentId };
            try
            {
                s = _userInteraction.DeleteUserWebsiteFTPDatabase(studentId);
                db.SaveChanges();

            }
            catch (Exception)
            {
                _userInteraction.RollbackOnError(BatchState.State, s.Credentials.FTPUserName);
                throw;
            }
        }

        private void hjælpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Help helperForm = new Help();
            helperForm.ShowDialog();
        }







    }
}
