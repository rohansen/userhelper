$(document).ready(function (e) {

   //Set the correct selected value for dropdown.

    $('.chkRole').change(function () {
        var cb = $(this);

        if (!cb.is(":checked")) {
           var roleId = $(this).attr("name");
           var userId = $(this).data("chkFor");
           console.log("UserId: " + userId + " roleid: " + roleId);
           RemoveRole(userId, roleId, $(this));
        } else {
           var roleId = $(this).attr("name");
           var userId = $(this).data("chkFor");
           console.log("UserId: " + userId + " roleid: " + roleId);
           AddRole(userId, roleId,$(this));
       }
   });
    
    function RemoveRole(userId, roleId,sender) {
        console.log("removing");
       console.log("UserId: " + userId + " roleid: " + roleId);
       $.ajax({
           url: '/Manage/RemoveUserRole',
           type: 'POST',
           data: { id: userId, value: roleId },
           success: function (e) {
               sender.css({ "text-decoration": "underline" });
           },
           error: function (e) {
               console.log($(this).data("chkFor") + " " + $(this).val());
               console.log(e);
               alert("error");
           }

       });
   }

   function AddRole(userId, roleId,sender) {
       console.log("adding");
           $.ajax({
               url: '/Manage/AddUserRole',
               type: 'POST',
               data: { id: userId, value: roleId },
               success: function (e) {
                   sender.css({ "text-decoration": "underline" });
               },
               error: function (e) {
                   console.log($(this).data("chkFor") + " " + $(this).val());
                   console.log(e);
                   alert("error");
               }
       

       });
   }
  

  


});