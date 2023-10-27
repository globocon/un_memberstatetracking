using Unece.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Unece.Data.Providers
{

    public interface IModuleDataProvider
    {

        List<ModuleOne> GetModuleOneDetails();
        List<ModuleTwo> GetModuleTwoDetails();
        void SaveModuleOneDetails(ModuleOne moduleOne);
        void DeleteModuleOneDetails(int id);
    }

    public class ModuleDataProvider : IModuleDataProvider
    {
        private readonly UNECEDbContext _context;
        public ModuleDataProvider(UNECEDbContext context)
        {
            _context = context;

        }
        /// <summary>
        /// Get Module One deatils
        /// </summary>
        /// <returns></returns>
        public List<ModuleOne> GetModuleOneDetails()
        {
            return _context.Tbl_ModuleOne.OrderBy(x => x.Id).ToList();
        }
        /// <summary>
        /// Get Module two deatils
        /// </summary>
        /// <returns></returns>
        public List<ModuleTwo> GetModuleTwoDetails()
        {
            return _context.Tbl_ModuleTwo.OrderBy(x => x.Id).ToList();
        }
        /// <summary>
        /// Save or Update moduleOne details
        /// </summary>
        /// <param name="moduleOne"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void SaveModuleOneDetails(ModuleOne moduleOne)
        {
            if (moduleOne == null)
                throw new ArgumentNullException();

            if (moduleOne.Id == 0)
            {
                _context.Tbl_ModuleOne.Add(moduleOne);
            }
            else
            {
                var moduleOneDataToUpdate = _context.Tbl_ModuleOne.SingleOrDefault(x => x.Id == moduleOne.Id);
                if (moduleOneDataToUpdate != null)
                {
                    moduleOneDataToUpdate.FieldOne = moduleOne.FieldOne;
                    moduleOneDataToUpdate.FieldTwo = moduleOne.FieldTwo;
                }
            }
            _context.SaveChanges();
        }
        /// <summary>
        /// Delete Module one Details
        /// </summary>
        /// <param name="id"></param>
        public void DeleteModuleOneDetails(int id)
        {
            var moduleOneDataToDelete = _context.Tbl_ModuleOne.SingleOrDefault(x => x.Id == id);
            if (moduleOneDataToDelete != null)
            {
                _context.Tbl_ModuleOne.Remove(moduleOneDataToDelete);
                _context.SaveChanges();
            }
        }

    }

}
