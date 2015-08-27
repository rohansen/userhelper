using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace WebInterface.Models
{
    public class RolesDropDownViewModel
    {
        public string SelectedRoleId { get; set; }
        public IEnumerable<SelectListItem> Roles { get; set; }
    }
}