using Unece.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Unece.Data.Providers
{
    public interface IFieldDataProvider
    {
        List<FieldTypeMaster> GetFieldTypes();
        List<FieldDetails> GetFieldDetails(int type);
        int AddorUpdateFieldDetails(FieldDetails fieldDetails);
        void DeleteFieldDetails(int id);
        int SaveFieldType(FieldTypeMaster fieldTypeMaster);
        int DeleteFieldType(int typeId);
    }


    public class FieldDataProvider : IFieldDataProvider
    {


        private readonly UNECEDbContext _context;
        public FieldDataProvider(UNECEDbContext context)
        {
            _context = context;

        }

        /// <summary>
        /// get Field Types
        /// </summary>
        /// <returns>Field Type List</returns>
        public List<FieldTypeMaster> GetFieldTypes()
        {
            return _context.FieldTypeMaster.OrderBy(x => x.FieldTypeName).ToList();
        }
        /// <summary>
        /// Get Field Details using type 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<FieldDetails> GetFieldDetails(int type)
        {
            return _context.FieldDetails.Where(x => x.FieldTypeMasterId == type).OrderBy(x => x.FieldName).ToList();

        }
        /// <summary>
        /// Add or update Field Deatils
        /// </summary>
        /// <param name="fieldDetails"></param>
        /// <returns>saveStatus 1,2,and 3</returns>
        public int AddorUpdateFieldDetails(FieldDetails fieldDetails)
        {
            int saveStatus = 0;
            if (fieldDetails != null)
            {

                if (fieldDetails.Id == -1)
                {
                    /* for checking already exist this title  */
                    var checkIfAlreadyExist = _context.FieldDetails.FirstOrDefault(x => x.FieldName == fieldDetails.FieldName);
                    if (checkIfAlreadyExist == null)
                    {
                        _context.FieldDetails.Add(new FieldDetails()
                        {
                            FieldTypeMasterId = fieldDetails.FieldTypeMasterId,
                            FieldName = fieldDetails.FieldName,


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
                    var reportFieldToUpdate = _context.FieldDetails.SingleOrDefault(x => x.Id == fieldDetails.Id);

                    if (reportFieldToUpdate != null)
                    {
                        /* for checking already exist this title in state */
                        var checkIfAlreadyExist = _context.FieldDetails.FirstOrDefault(x => x.FieldName == fieldDetails.FieldName && x.Id != fieldDetails.Id);
                        if (checkIfAlreadyExist == null)
                        {
                            reportFieldToUpdate.FieldName = fieldDetails.FieldName;
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
        /// Delete  Field Details
        /// </summary>
        /// <param name="id">Field Id</param>
        /// <exception cref="InvalidOperationException"></exception>
        public void DeleteFieldDetails(int id)
        {
            if (id == -1)
                return;

            var fieldDetailsToDelete = _context.FieldDetails.SingleOrDefault(x => x.Id == id);
            if (fieldDetailsToDelete == null)
                throw new InvalidOperationException();

            _context.FieldDetails.Remove(fieldDetailsToDelete);
            _context.SaveChanges();
        }
        /// <summary>
        /// Save Field Type
        /// </summary>
        /// <param name="fieldTypeMaster"></param>
        /// <returns>saveStatus -1 or 1</returns>
        public int SaveFieldType(FieldTypeMaster fieldTypeMaster)
        {
            int saveStatus = -1;
            if (fieldTypeMaster != null)
            {

                if (fieldTypeMaster.Id == 0)
                {
                    var ClientSiteLinksPageTypeToUpdate = _context.FieldTypeMaster.SingleOrDefault(x => x.FieldTypeName == fieldTypeMaster.FieldTypeName);

                    if (ClientSiteLinksPageTypeToUpdate == null)
                    {
                        _context.FieldTypeMaster.Add(new FieldTypeMaster() { FieldTypeName = fieldTypeMaster.FieldTypeName });

                        saveStatus = 1;

                    }
                    else
                    {
                        saveStatus = -1;
                    }

                }
                else
                {
                    var ClientSiteLinksPageTypeToUpdate = _context.FieldTypeMaster.SingleOrDefault(x => x.Id == fieldTypeMaster.Id);
                    if (ClientSiteLinksPageTypeToUpdate != null)
                    {

                        ClientSiteLinksPageTypeToUpdate.FieldTypeName = fieldTypeMaster.FieldTypeName;
                        saveStatus = 1;
                    }


                }


                _context.SaveChanges();
                if (saveStatus != -1)
                {
                    var lastInsertedId = _context.FieldTypeMaster.SingleOrDefault(x => x.FieldTypeName == fieldTypeMaster.FieldTypeName);
                    saveStatus = lastInsertedId.Id;

                }
            }

            return saveStatus;
        }

        /// <summary>
        /// Delete Field Type
        /// </summary>
        /// <param name="typeId">typeId</param>
        /// <returns> 0 or 1</returns>
        public int DeleteFieldType(int typeId)
        {
            if (typeId == -1)
                return 0;

            var feedBackTypeToDelete = _context.FieldTypeMaster.SingleOrDefault(x => x.Id == typeId);
            if (feedBackTypeToDelete == null)
            {

                return 0;
            }
            else
            {
                _context.FieldTypeMaster.Remove(feedBackTypeToDelete);
                _context.SaveChanges();
                return 1;
            }
        }
    }
}
