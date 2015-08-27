namespace UserHelperCL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedpasswordattributestomodel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Credentials", "AllowPasswordChange", c => c.Boolean(nullable: false));
            AddColumn("dbo.Credentials", "PasswordExpires", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Credentials", "PasswordExpires");
            DropColumn("dbo.Credentials", "AllowPasswordChange");
        }
    }
}
