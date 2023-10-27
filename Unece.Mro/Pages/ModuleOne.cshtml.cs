using Microsoft.AspNetCore.Mvc;
using Unece.Data.Providers;
using Unece.Data.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System;

namespace Unece.Mro.Pages
{
    public class ModuleOneModel : PageModel
    {
        private readonly IModuleDataProvider _moduleData;
        public ModuleOneModel(IModuleDataProvider moduleData)
        {

            _moduleData = moduleData;
        }
        public void OnGet()
        {
        }
        public IActionResult OnGetModuleOneData()
        {

            return new JsonResult(_moduleData.GetModuleOneDetails());
        }
        public JsonResult OnPostSaveModuleOneData(ModuleOne moduleOne)
        {
            var success = false;
            var message = string.Empty;
            try
            {
                _moduleData.SaveModuleOneDetails(moduleOne);
                success = true;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                if (ex.InnerException != null &&
                    ex.InnerException is SqlException &&
                    ex.InnerException.Message.StartsWith("Violation of UNIQUE KEY constraint"))
                {
                    message = "Key number already exists";
                }
            }

            return new JsonResult(new { success, message });
        }

        public JsonResult OnPostDeleteModuleOneDetails(int id)
        {
            var success = false;
            var message = string.Empty;
            try
            {
                _moduleData.DeleteModuleOneDetails(id);
                success = true;
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            return new JsonResult(new { success, message });
        }
    }
}
