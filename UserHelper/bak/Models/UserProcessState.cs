using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserHelper.Models
{
    public enum UserProcessState
    {
        INITIAL,
        All_OK,
        WIN_USER_OK,
        WIN_USER_ERROR,
        WIN_GROUP_OK,
        WIN_GROUP_ERROR,
        WIN_DELETE_OK,
        WIN_DELETE_ERROR,
        IIS_WEBSITE_OK,
        IIS_WEBSITE_ERROR,
        IIS_DELETE_WEBSITE_OK,
        IIS_DELETE_WEBSITE_ERROR,
        VIRTUAL_DIRECTORY_OK,
        VIRTUAL_DIRECTORY_ERROR,
        VIRTUAL_DIRECTORY_REMOVE_OK,
        VIRTUAL_DIRECTORY_REMOVE_ERROR,
        SQL_LOGIN_OK,
        SQL_LOGIN_ERROR,
        SQL_LOGIN_DELETE_OK,
        SQL_LOGIN_DELETE_ERROR, 
        SQL_DB_OK, 
        SQL_DB_ERROR,
        SQL_DELETE_DB_ERROR,
        SQL_DELETE_DB_OK,
        SQL_USER_OK,
        SQL_ROLE_OK,
        SQL_USER_ERROR,
        SQL_ROLE_ERROR,
       
        


    }
}
