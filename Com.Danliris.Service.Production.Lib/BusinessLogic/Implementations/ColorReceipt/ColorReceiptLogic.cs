﻿using Com.Danliris.Service.Finishing.Printing.Lib.Models.ColorReceipt;
using Com.Danliris.Service.Production.Lib;
using Com.Danliris.Service.Production.Lib.Services.IdentityService;
using Com.Danliris.Service.Production.Lib.Utilities.BaseClass;
using Com.Moonlay.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Finishing.Printing.Lib.BusinessLogic.Implementations.ColorReceipt
{
    public class ColorReceiptLogic : BaseLogic<ColorReceiptModel>
    {
        private readonly ProductionDbContext dbContext;
        private readonly DbSet<ColorReceiptItemModel> dbSetItem;

        public ColorReceiptLogic(IIdentityService identityService, ProductionDbContext dbContext) : base(identityService, dbContext)
        {
            this.dbContext = dbContext;
            this.dbSetItem = dbContext.Set<ColorReceiptItemModel>();
        }

        public override void CreateModel(ColorReceiptModel model)
        {
            foreach (var item in model.ColorReceiptItems)
            {
                EntityExtension.FlagForCreate(item, IdentityService.Username, UserAgent);
            }

            foreach(var item in model.DyeStuffReactives)
            {
                EntityExtension.FlagForCreate(item, IdentityService.Username, UserAgent);
            }

            base.CreateModel(model);
        }

        public override async Task DeleteModel(int id)
        {
            var model = await ReadModelById(id);
            EntityExtension.FlagForDelete(model, IdentityService.Username, UserAgent, true);
            foreach (var item in model.ColorReceiptItems)
            {
                EntityExtension.FlagForDelete(item, IdentityService.Username, UserAgent);
            }

            foreach (var item in model.DyeStuffReactives)
            {
                EntityExtension.FlagForDelete(item, IdentityService.Username, UserAgent);
            }
            DbSet.Update(model);
        }

        public override async Task UpdateModelAsync(int id, ColorReceiptModel model)
        {
            var dbModel = await ReadModelById(id);
            dbModel.ColorCode = model.ColorCode;
            dbModel.ColorName = model.ColorName;
            dbModel.Remark = model.Remark;
            dbModel.TechnicianId = model.TechnicianId;
            dbModel.TechnicianName = model.TechnicianName;

            EntityExtension.FlagForUpdate(dbModel, IdentityService.Username, UserAgent);

            var addedItems = model.ColorReceiptItems.Where(x => !dbModel.ColorReceiptItems.Any(y => y.Id == x.Id)).ToList();
            var updatedItems = model.ColorReceiptItems.Where(x => dbModel.ColorReceiptItems.Any(y => y.Id == x.Id)).ToList();
            var deletedItems = dbModel.ColorReceiptItems.Where(x => !model.ColorReceiptItems.Any(y => y.Id == x.Id)).ToList();
            foreach (var item in updatedItems)
            {
                var dbItem = dbModel.ColorReceiptItems.FirstOrDefault(x => x.Id == item.Id);

                dbItem.ProductCode = item.ProductCode;
                dbItem.ProductId = item.ProductId;
                dbItem.ProductName = item.ProductName;
                dbItem.Quantity = item.Quantity;

                EntityExtension.FlagForUpdate(dbItem, IdentityService.Username, UserAgent);
            }


            foreach (var item in deletedItems)
            {
                EntityExtension.FlagForDelete(item, IdentityService.Username, UserAgent);
            }

            foreach (var item in addedItems)
            {
                item.ColorReceiptId = id;
                EntityExtension.FlagForCreate(item, IdentityService.Username, UserAgent);
                dbModel.ColorReceiptItems.Add(item);
            }


            var addedReactive = model.DyeStuffReactives.Where(x => !dbModel.DyeStuffReactives.Any(y => y.Name == x.Name)).ToList();
            var updatedReactive = model.DyeStuffReactives.Where(x => dbModel.DyeStuffReactives.Any(y => y.Name == x.Name)).ToList();
            var deletedReactive = dbModel.DyeStuffReactives.Where(x => !model.DyeStuffReactives.Any(y => y.Name == x.Name)).ToList();
            foreach (var item in updatedReactive)
            {
                var dbItem = dbModel.DyeStuffReactives.FirstOrDefault(x => x.Name == item.Name);

                dbItem.Name = item.Name;
                dbItem.Quantity = item.Quantity;

                EntityExtension.FlagForUpdate(dbItem, IdentityService.Username, UserAgent);
            }


            foreach (var item in deletedReactive)
            {
                EntityExtension.FlagForDelete(item, IdentityService.Username, UserAgent);
            }

            foreach (var item in addedReactive)
            {
                item.ColorReceiptId = id;
                EntityExtension.FlagForCreate(item, IdentityService.Username, UserAgent);
                dbModel.DyeStuffReactives.Add(item);
            }

        }

        public override Task<ColorReceiptModel> ReadModelById(int id)
        {
            return DbSet.Include(s => s.ColorReceiptItems).Include(s => s.DyeStuffReactives).FirstOrDefaultAsync(s => s.Id == id);
        }

        public void SetDefaultAllTechnician(bool flag)
        {
            var dbTechnician = dbContext.Technicians;
            foreach (var item in dbTechnician)
            {
                item.IsDefault = flag;
                EntityExtension.FlagForUpdate(item, IdentityService.Username, UserAgent);
            }
        }

        public TechnicianModel CreateTechnician(string name)
        {
            TechnicianModel technician = new TechnicianModel
            {
                Name = name,
                IsDefault = true
            };
            EntityExtension.FlagForCreate(technician, IdentityService.Username, UserAgent);
            dbContext.Technicians.Add(technician);
            return technician;
        }


        public Task<TechnicianModel> GetDefaultTechnician()
        {
            return dbContext.Technicians.OrderByDescending(s => s.LastModifiedUtc).FirstOrDefaultAsync(s => s.IsDefault);
        }
    }
}
