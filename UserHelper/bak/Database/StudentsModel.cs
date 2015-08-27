namespace UserHelper.Database
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using UserHelper.Models;

    public class StudentsModel : DbContext
    {
        // Your context has been configured to use a 'StudentsModel' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // 'UserHelper.Database.StudentsModel' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'StudentsModel' 
        // connection string in the application configuration file.
        public StudentsModel()
            : base("name=StudentsModel")
        {
            Database.SetInitializer<StudentsModel>(new DropCreateDatabaseIfModelChanges<StudentsModel>());
        }

        // Add a DbSet for each entity type that you want to include in your model. For more information 
        // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

        public virtual DbSet<Student> Students { get; set; }
        public virtual DbSet<Team> Teams { get; set; }
        public virtual DbSet<Credentials> Credentials { get; set; }





    }



}