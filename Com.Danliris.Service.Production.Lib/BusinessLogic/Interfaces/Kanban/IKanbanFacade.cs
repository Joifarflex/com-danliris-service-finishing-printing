﻿using Com.Danliris.Service.Finishing.Printing.Lib.Models.Kanban;
using Com.Danliris.Service.Finishing.Printing.Lib.ViewModels.Kanban;
using Com.Danliris.Service.Production.Lib.Utilities;
using Com.Danliris.Service.Production.Lib.Utilities.BaseInterface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Finishing.Printing.Lib.BusinessLogic.Interfaces.Kanban
{
    public interface IKanbanFacade : IBaseFacade<KanbanModel>
    {
        ReadResponse<KanbanViewModel> GetReport(int page, int size, bool? proses, int orderTypeId, int processTypeId, string orderNo, DateTime? dateFrom, DateTime? dateTo, int offSet);

        MemoryStream GenerateExcel(bool? proses, int orderTypeId, int processTypeId, string orderNo, DateTime? dateFrom, DateTime? dateTo, int offSet);

        List<KanbanViewModel> GetReport(bool? proses, int orderTypeId, int processTypeId, string orderNo, DateTime? dateFrom, DateTime? dateTo, int offSet);

        Task<int> CompleteKanban(int id);

        Task<KanbanModel> ReadOldKanbanByIdAsync(int id);

        MemoryStream GenerateKanbanSnapshotExcel(int month, int year);
        ReadResponse<KanbanVisualizationViewModel> ReadVisualization(string order, string filter);
    }
}
