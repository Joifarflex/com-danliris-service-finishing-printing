﻿using Com.Danliris.Service.Finishing.Printing.Lib.BusinessLogic.Implementations.Master.Machine;
using Com.Danliris.Service.Finishing.Printing.Lib.BusinessLogic.Interfaces.Master;
using Com.Danliris.Service.Finishing.Printing.Lib.Models.Master.Machine;
using Com.Danliris.Service.Production.Lib;
using Com.Danliris.Service.Production.Lib.Utilities;
using Com.Moonlay.NetCore.Lib;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Finishing.Printing.Lib.BusinessLogic.Facades.Master
{
    public class MachineFacade : IMachineFacade
    {
        private readonly ProductionDbContext DbContext;
        private readonly DbSet<MachineModel> DbSet;
        private readonly MachineLogic MachineLogic;
        public MachineFacade(IServiceProvider serviceProvider, ProductionDbContext dbContext)
        {
            this.DbContext = dbContext;
            this.DbSet = DbContext.Set<MachineModel>();
            this.MachineLogic = serviceProvider.GetService<MachineLogic>();
        }

        public async Task<int> CreateAsync(MachineModel model)
        {
            do
            {
                model.Code = CodeGenerator.Generate();
            }
            while (DbSet.Any(d => d.Code.Equals(model.Code)));
            this.MachineLogic.CreateModel(model);
            return await DbContext.SaveChangesAsync();
        }

        public async Task<int> DeleteAsync(int id)
        {
            await MachineLogic.DeleteModel(id);
            return await DbContext.SaveChangesAsync();
        }

        public ReadResponse<MachineModel> GetDyeingPrintingMachine(int page, int size, string order, List<string> select, string keyword, string filter)
        {
            IQueryable<MachineModel> query = DbSet.Include(x => x.MachineEvents).Include(x => x.MachineSteps)
                .Where(x => x.UnitDivisionName.ToUpper() == "FINISHING & PRINTING" || x.UnitDivisionName.ToUpper() == "DYEING & PRINTING");

            List<string> searchAttributes = new List<string>()
            {
                "Code", "Name"
            };
            query = QueryHelper<MachineModel>.Search(query, searchAttributes, keyword);

            Dictionary<string, object> filterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(filter);
            query = QueryHelper<MachineModel>.Filter(query, filterDictionary);

            List<string> selectedFields = new List<string>()
                {
                    "Id", "Name", "Code", "Process", "Manufacture","Year","Condition","Unit","MachineType","MonthlyCapacity", "Electric", "Steam",
                    "Water", "Solar", "LPG", "LastModifiedUtc", "MachineEvents", "Steps", "UseBQBS"
                };

            Dictionary<string, string> orderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            query = QueryHelper<MachineModel>.Order(query, orderDictionary);

            Pageable<MachineModel> pageable = new Pageable<MachineModel>(query, page - 1, size);
            List<MachineModel> data = pageable.Data.ToList();
            int totalData = pageable.TotalCount;

            return new ReadResponse<MachineModel>(data, totalData, orderDictionary, selectedFields);
        }

        public ReadResponse<MachineModel> Read(int page, int size, string order, List<string> select, string keyword, string filter)
        {
            IQueryable<MachineModel> query = DbSet.Include(x => x.MachineEvents).Include(x => x.MachineSteps);

            List<string> searchAttributes = new List<string>()
            {
                "Code", "Name"
            };
            query = QueryHelper<MachineModel>.Search(query, searchAttributes, keyword);

            Dictionary<string, object> filterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(filter);
            query = QueryHelper<MachineModel>.Filter(query, filterDictionary);

            List<string> selectedFields = new List<string>()
                {
                    "Id", "Name", "Code", "Process", "Manufacture","Year","Condition","Unit","MachineType","MonthlyCapacity", "Electric", "Steam",
                    "Water", "Solar", "LPG", "LastModifiedUtc", "MachineEvents", "Steps", "UseBQBS"
                };

            //query = query
            //        .Select(field => new MachineModel
            //        {
            //            Id = field.Id,
            //            Name = field.Name,
            //            Code = field.Code,
            //            Process = field.Process,
            //            Manufacture = field.Manufacture,
            //            Year = field.Year,
            //            Condition = field.Condition,
            //            UnitName = field.UnitName,
            //            UnitDivisionName = field.UnitDivisionName,
            //            MachineTypeId = field.MachineTypeId,
            //            MachineTypeName = field.MachineTypeName,
            //            MonthlyCapacity = field.MonthlyCapacity,
            //            Electric = field.Electric,
            //            LPG = field.LPG,
            //            Solar = field.Solar,
            //            MachineTypeCode = field.MachineTypeCode,
            //            Water = field.Water,
            //            Steam = field.Steam,
            //            UnitCode = field.UnitCode,
            //            UnitDivisionId = field.UnitDivisionId,
            //            UnitId = field.UnitId,
            //            LastModifiedUtc = field.LastModifiedUtc,
            //            MachineEvents = new List<MachineEventsModel>(field.MachineEvents.Select(i => new MachineEventsModel
            //            {
            //                Id = i.Id,
            //                Code = i.Code,
            //                Name = i.Name,
            //                No = i.No,
            //                Category = i.Category,
            //                MachineId = i.MachineId
            //            })),
            //            MachineSteps = new List<MachineStepModel>(field.MachineSteps.Select(d => new MachineStepModel
            //            {
            //                Id = d.Id,
            //                Code = d.Code,
            //                Alias = d.Alias,
            //                Process = d.Process,
            //                ProcessArea = d.ProcessArea,
            //                StepId = d.StepId,
            //            }))
            //        });

            Dictionary<string, string> orderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            query = QueryHelper<MachineModel>.Order(query, orderDictionary);

            Pageable<MachineModel> pageable = new Pageable<MachineModel>(query, page - 1, size);
            List<MachineModel> data = pageable.Data.ToList();
            int totalData = pageable.TotalCount;

            return new ReadResponse<MachineModel>(data, totalData, orderDictionary, selectedFields);
        }

        public async Task<MachineModel> ReadByIdAsync(int id)
        {
            return await MachineLogic.ReadModelById(id);
        }

        public async Task<int> UpdateAsync(int id, MachineModel model)
        {
            await this.MachineLogic.UpdateModelAsync(id, model);
            return await DbContext.SaveChangesAsync();
        }
    }
}
