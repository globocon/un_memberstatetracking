using Unece.Data.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Unece.Data.Providers
{


    public interface IApplicationDataProvider
    {

        List<ApplicationMaster> GetApplicationDetails();
        int AddorUpdateApplicationDetails(ApplicationMaster department);
        public void DeleteApplicationDetails(int id);
    }
    public class ApplicationDataProvider : IApplicationDataProvider
    {

        private readonly UNECEDbContext _context;
        public ApplicationDataProvider(UNECEDbContext context)
        {
            _context = context;

        }
        /// <summary>
        /// Get all ApplicationDetails
        /// </summary>
        /// <returns>Application List</returns>
        public List<ApplicationMaster> GetApplicationDetails()
        {
            return _context.ApplicationMaster.OrderBy(x => x.Id).ToList();
        }
        /// <summary>
        /// Add New Application or Update Existing Application 
        /// </summary>
        /// <param name="application"></param>
        /// <returns>saveStatus 1,2,3</returns>
        public int AddorUpdateApplicationDetails(ApplicationMaster application)
        {
            int saveStatus = 0;
            if (application != null)
            {

                if (application.Id == -1)
                {
                    /* for checking already exist this title  */
                    var checkIfAlreadyExist = _context.ApplicationMaster.FirstOrDefault(x => x.ApplicationName == application.ApplicationName);
                    if (checkIfAlreadyExist == null)
                    {
                        _context.ApplicationMaster.Add(new ApplicationMaster()
                        {
                            ApplicationName = application.ApplicationName,
                            ApplicationDescription = application.ApplicationDescription

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
                    var reportFieldToUpdate = _context.ApplicationMaster.SingleOrDefault(x => x.Id == application.Id);

                    if (reportFieldToUpdate != null)
                    {
                        /* for checking already exist this title in state */
                        var checkIfAlreadyExist = _context.ApplicationMaster.FirstOrDefault(x => x.ApplicationName == application.ApplicationName && x.Id != application.Id);
                        if (checkIfAlreadyExist == null)
                        {
                            reportFieldToUpdate.ApplicationName = application.ApplicationName;
                            reportFieldToUpdate.ApplicationDescription = application.ApplicationDescription;
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
        /// Delete Application Details
        /// </summary>
        /// <param name="id">Application id</param>
        /// <exception cref="InvalidOperationException"></exception>
        public void DeleteApplicationDetails(int id)
        {
            if (id == -1)
                return;

            var applicationDetailsToDelete = _context.ApplicationMaster.SingleOrDefault(x => x.Id == id);
            if (applicationDetailsToDelete == null)
                throw new InvalidOperationException();
            _context.ApplicationMaster.Remove(applicationDetailsToDelete);
            _context.SaveChanges();
        }
    }

}
