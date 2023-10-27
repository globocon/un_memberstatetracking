
using Unece.Data.Helpers;
using Unece.Data.Models;
using Unece.Data.Providers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System;
using System.Linq;

namespace Unece.Mro.Pages.Admin
{
    public class SettingsModel : PageModel
    {


        private readonly IUserDataProvider _userDataProvider;
        private readonly IDepartmentDataProvider _departmentDataProvider;
        private readonly IApplicationDataProvider _applicationDataProvider;
        private readonly ISiteDataProvider _siteDataProvider;
        private readonly IFieldDataProvider _fieldDataProvider;
        public SettingsModel(IWebHostEnvironment webHostEnvironment,
            IUserDataProvider userDataProvider,
            IDepartmentDataProvider DepartmentDataProvider,
            IApplicationDataProvider applicationDataProvider,
            ISiteDataProvider siteDataProvider,
            IFieldDataProvider fieldDataProvider
            )
        {

            _userDataProvider = userDataProvider;
            _departmentDataProvider = DepartmentDataProvider;
            _applicationDataProvider = applicationDataProvider;
            _siteDataProvider = siteDataProvider;
            _fieldDataProvider = fieldDataProvider;
        }
        public void OnGet()
        {
        }

        #region Department
        public JsonResult OnGetDepartmentDetails()
        {
            var fields = _departmentDataProvider.GetDepartmentDetails();
            return new JsonResult(fields);
        }

        public JsonResult OnPostDepartmentDetailsUpdate(DepartmentMaster department)
        {
            var status = true;
            var message = "Success";
            var success = 1;
            try
            {
                success = _departmentDataProvider.AddorUpdateDepartmentDetails(department);
                if (success != 1)
                {
                    if (success == 2)
                        message = "The department name you have entered is already exists . Please Use different department name .";
                    else if (success == 3)
                        message = "The department name you have entered is already exists . Please Use different department name.";
                    status = false;
                }


            }
            catch (Exception ex)
            {
                status = false;
                message = "Error " + ex.Message;
            }

            return new JsonResult(new { status = status, message = message });
        }

        public JsonResult OnPostDeleteDepartmentDetails(int id)
        {
            var status = true;
            var message = "Success";
            try
            {
                _departmentDataProvider.DeleteDepartmentDetails(id);
            }
            catch (Exception ex)
            {
                status = false;
                message = "Error " + ex.Message;
            }

            return new JsonResult(new { status = status, message = message });
        }

        public IFieldDataProvider FieldDataProvider { get { return _fieldDataProvider; } }
        #endregion


        #region Application
        public JsonResult OnGetApplicationDetails()
        {
            var fields = _applicationDataProvider.GetApplicationDetails();
            return new JsonResult(fields);
        }

        public JsonResult OnPostApplicationDetailsUpdate(ApplicationMaster application)
        {
            var status = true;
            var message = "Success";
            var success = 1;
            try
            {
                success = _applicationDataProvider.AddorUpdateApplicationDetails(application);
                if (success != 1)
                {
                    if (success == 2)
                        message = "The application name you have entered is already exists . Please Use different application name .";
                    else if (success == 3)
                        message = "The application name you have entered is already exists . Please Use different application name.";
                    status = false;
                }


            }
            catch (Exception ex)
            {
                status = false;
                message = "Error " + ex.Message;
            }

            return new JsonResult(new { status = status, message = message });
        }

        public JsonResult OnPostDeleteApplicationDetails(int id)
        {
            var status = true;
            var message = "Success";
            try
            {
                _applicationDataProvider.DeleteApplicationDetails(id);
            }
            catch (Exception ex)
            {
                status = false;
                message = "Error " + ex.Message;
            }

            return new JsonResult(new { status = status, message = message });
        }
        #endregion


        #region User
        public JsonResult OnGetUsers()
        {
            var users = _userDataProvider.GetUsers(true).Select(x => new { x.Id, x.UserName, x.IsDeleted });
            return new JsonResult(users);
        }

        public JsonResult OnPostShowPassword(USR_Users user)
        {
            var value = string.Empty;
            try
            {
                var currUser = _userDataProvider.GetUsers().SingleOrDefault(x => x.Id == user.Id);
                if (currUser != null)
                    value = PasswordHelper.DecryptPassword(currUser.Password);
            }
            catch
            {
            }

            return new JsonResult(value);
        }

        public JsonResult OnPostUser(USR_Users record)
        {
            var status = true;
            var message = "Success";
            try
            {
                if (record != null)
                {
                    record.Password = PasswordHelper.EncryptPassword(record.Password);
                    _userDataProvider.SaveUser(record);
                }
            }
            catch (Exception ex)
            {
                status = false;
                message = "Error " + ex.Message;

                if (ex.InnerException != null &&
                    ex.InnerException is SqlException &&
                    ex.InnerException.Message.StartsWith("Violation of UNIQUE KEY constraint"))
                {
                    message = "A user with this username already exists";
                }
            }

            return new JsonResult(new { status = status, message = message });
        }




        public JsonResult OnPostUpdateUserStatus(int id, bool deleted)
        {
            var status = true;
            var message = "Success";
            try
            {
                _userDataProvider.UpdateUserStatus(id, deleted);
            }
            catch (Exception ex)
            {
                status = false;
                message = "Error " + ex.Message;
            }

            return new JsonResult(new { status, message });
        }
        #endregion User

        #region Site
        public JsonResult OnGetSiteDetails()
        {
            var fields = _siteDataProvider.GetSiteDetails();
            return new JsonResult(fields);
        }

        public JsonResult OnPostSiteDetailsUpdate(SiteMaster site)
        {
            var status = true;
            var message = "Success";
            var success = 1;
            try
            {
                success = _siteDataProvider.AddorUpdateSiteDetails(site);
                if (success != 1)
                {
                    if (success == 2)
                        message = "The application name you have entered is already exists . Please Use different application name .";
                    else if (success == 3)
                        message = "The application name you have entered is already exists . Please Use different application name.";
                    status = false;
                }


            }
            catch (Exception ex)
            {
                status = false;
                message = "Error " + ex.Message;
            }

            return new JsonResult(new { status = status, message = message });
        }

        public JsonResult OnPostDeleteSiteDetails(int id)
        {
            var status = true;
            var message = "Success";
            try
            {
                _siteDataProvider.DeleteSiteDetails(id);
            }
            catch (Exception ex)
            {
                status = false;
                message = "Error " + ex.Message;
            }

            return new JsonResult(new { status = status, message = message });
        }
        #endregion

        #region field 
        public IActionResult OnGetFieldTypeList()
        {
            return new JsonResult(_fieldDataProvider.GetFieldTypes());
        }
        public JsonResult OnGetFieldDetails(int typeId)
        {
            var fields = _fieldDataProvider.GetFieldDetails(typeId);
            return new JsonResult(fields);
        }

        public JsonResult OnPostFieldDetails(FieldDetails reportfield)
        {
            var status = true;
            var message = "Success";
            var success = 1;
            try
            {
                success = _fieldDataProvider.AddorUpdateFieldDetails(reportfield);
                if (success != 1)
                {
                    if (success == 2)
                        message = "The title you have entered is already exists for this button. Please Use different Title or button.";
                    else if (success == 3)
                        message = "The title you have entered is already exists for this button. Please Use different Title or button.";
                    status = false;
                }


            }
            catch (Exception ex)
            {
                status = false;
                message = "Error " + ex.Message;
            }

            return new JsonResult(new { status = status, message = message });
        }


        public JsonResult OnPostDeleteFieldDetails(int id)
        {
            var status = true;
            var message = "Success";
            try
            {
                _fieldDataProvider.DeleteFieldDetails(id);
            }
            catch (Exception ex)
            {
                status = false;
                message = "Error " + ex.Message;
            }

            return new JsonResult(new { status = status, message = message });
        }

        public JsonResult OnPostFieldType(FieldTypeMaster ClientSiteLinksPageTyperecord)
        {
            var status = 0;
            var message = "Success";
            try
            {
                if (ClientSiteLinksPageTyperecord != null)
                {

                    status = _fieldDataProvider.SaveFieldType(ClientSiteLinksPageTyperecord);
                    if (status == -1)
                    {

                        message = "Same button name already exist";


                    }
                }
            }
            catch (Exception ex)
            {
                status = 0;
                message = "Error " + ex.Message;


            }

            return new JsonResult(new { status = status, message = message });
        }

        public JsonResult OnPostDeleteFieldType(int TypeId)
        {
            var status = 0;
            var message = "Success";
            try
            {
                if (TypeId != 0)
                {

                    status = _fieldDataProvider.DeleteFieldType(TypeId);

                }
            }
            catch (Exception ex)
            {
                status = 0;
                message = "Error " + ex.Message;


            }

            return new JsonResult(new { status = status, message = message });
        }
        #endregion

    }
}
