namespace UserHelperCL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedpasswordattributestomodel2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Credentials", "DisablePasswordChange", c => c.Boolean(nullable: false));
            AddColumn("dbo.Credentials", "PasswordNeverExpires", c => c.Boolean(nullable: false));
            DropColumn("dbo.Credentials", "AllowPasswordChange");
            DropColumn("dbo.Credentials", "PasswordExpires");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Credentials", "PasswordExpires", c => c.Boolean(nullable: false));
            AddColumn("dbo.Credentials", "AllowPasswordChange", c => c.Boolean(nullable: false));
            DropColumn("dbo.Credentials", "PasswordNeverExpires");
            DropColumn("dbo.Credentials", "DisablePasswordChange");
        }
    }
}
