using Unece.Data.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Unece.Data.Providers
{

    public interface ISiteDataProvider
    {
        public List<SiteMaster> GetSiteDetails();
        int AddorUpdateSiteDetails(SiteMaster site);
        void DeleteSiteDetails(int id);
    }
    public class SiteDataProvider : ISiteDataProvider
    {
        private readonly UNECEDbContext _context;
        public SiteDataProvider(UNECEDbContext context)
        {
            _context = context;

        }
        /// <summary>
        /// get Site Details
        /// </summary>
        /// <returns></returns>
        public List<SiteMaster> GetSiteDetails()
        {
            return _context.SiteMaster.OrderBy(x => x.Id).ToList();
        }
        /// <summary>
        /// Add or update site details
        /// </summary>
        /// <param name="site"></param>
        /// <returns></returns>
        public int AddorUpdateSiteDetails(SiteMaster site)
        {
            int saveStatus = 0;
            if (site != null)
            {

                if (site.Id == -1)
                {
                    /* for checking already exist this title  */
                    var checkIfAlreadyExist = _context.SiteMaster.FirstOrDefault(x => x.SiteName == site.SiteName);
                    if (checkIfAlreadyExist == null)
                    {
                        _context.SiteMaster.Add(new SiteMaster()
                        {
                            SiteName = site.SiteName,
                            SiteAddress = site.SiteAddress

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
                    var reportFieldToUpdate = _context.SiteMaster.SingleOrDefault(x => x.Id == site.Id);

                    if (reportFieldToUpdate != null)
                    {
                        /* for checking already exist this title in state */
                        var checkIfAlreadyExist = _context.SiteMaster.FirstOrDefault(x => x.SiteName == site.SiteName && x.Id != site.Id);
                        if (checkIfAlreadyExist == null)
                        {
                            reportFieldToUpdate.SiteName = site.SiteName;
                            reportFieldToUpdate.SiteAddress = site.SiteAddress;
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
        /// Delete site details
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void DeleteSiteDetails(int id)
        {
            if (id == -1)
                return;

            var siteDetailsToDelete = _context.SiteMaster.SingleOrDefault(x => x.Id == id);
            if (siteDetailsToDelete == null)
                throw new InvalidOperationException();

            _context.SiteMaster.Remove(siteDetailsToDelete);
            _context.SaveChanges();
        }
    }
}
