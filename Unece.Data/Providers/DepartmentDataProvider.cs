using Unece.Data.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Unece.Data.Providers
{


    public interface IDepartmentDataProvider
    {

        List<DepartmentMaster> GetDepartmentDetails();
        int AddorUpdateDepartmentDetails(DepartmentMaster department);
        public void DeleteDepartmentDetails(int id);
    }
    public class DepartmentDataProvider : IDepartmentDataProvider
    {

        private readonly UNECEDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;


        public DepartmentDataProvider(IWebHostEnvironment webHostEnvironment,
            IConfiguration configuration,
            UNECEDbContext context)
        {
            _context = context;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }
        /// <summary>
        /// Get Department Details
        /// </summary>
        /// <returns>Department List</returns>
        public List<DepartmentMaster> GetDepartmentDetails()
        {
            return _context.DepartmentMaster.OrderBy(x => x.Id).ToList();
        }
        /// <summary>
        /// Add New Department or Update Existing Department
        /// </summary>
        /// <param name="department">department object</param>
        /// <returns>saveStatus 1,2 and 3</returns>
        public int AddorUpdateDepartmentDetails(DepartmentMaster department)
        {
            int saveStatus = 0;
            if (department != null)
            {

                if (department.Id == -1)
                {
                    /* for checking already exist this title  */
                    var checkIfAlreadyExist = _context.DepartmentMaster.FirstOrDefault(x => x.DepartmentName == department.DepartmentName);
                    if (checkIfAlreadyExist == null)
                    {
                        _context.DepartmentMaster.Add(new DepartmentMaster()
                        {
                            DepartmentName = department.DepartmentName,
                            DepartmentDescription = department.DepartmentDescription

                        });
                        saveStatus = 1;
                    }
                    else
                    {
                        saveStatus = 2;
                    }

                }
                else
                {
                    var reportFieldToUpdate = _context.DepartmentMaster.SingleOrDefault(x => x.Id == department.Id);

                    if (reportFieldToUpdate != null)
                    {
                        /* for checking already exist this title in state */
                        var checkIfAlreadyExist = _context.DepartmentMaster.FirstOrDefault(x => x.DepartmentName == department.DepartmentName && x.Id != department.Id);
                        if (checkIfAlreadyExist == null)
                        {
                            reportFieldToUpdate.DepartmentName = department.DepartmentName;
                            reportFieldToUpdate.DepartmentDescription = department.DepartmentDescription;
                            saveStatus = 1;
                        }
                        else
                        {

                            saveStatus = 3;
                        }
                    }
                }
                _context.SaveChanges();

            }

            return saveStatus;
        }
        /// <summary>
        /// Delete Department Details
        /// </summary>
        /// <param name="id">Department id</param>
        /// <exception cref="InvalidOperationException"></exception>
        public void DeleteDepartmentDetails(int id)
        {
            if (id == -1)
                return;

            var departmentDetailsToDelete = _context.DepartmentMaster.SingleOrDefault(x => x.Id == id);
            if (departmentDetailsToDelete == null)
                throw new InvalidOperationException();
            _context.DepartmentMaster.Remove(departmentDetailsToDelete);
            _context.SaveChanges();
        }
    }

}
