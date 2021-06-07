﻿using Com.Danliris.Service.Finishing.Printing.Lib.ModelConfigs.FabricQualityControl;
using Com.Danliris.Service.Finishing.Printing.Lib.ModelConfigs.Kanban;
using Com.Danliris.Service.Finishing.Printing.Lib.Models.ColorReceipt;
using Com.Danliris.Service.Finishing.Printing.Lib.Models.CostCalculation;
using Com.Danliris.Service.Finishing.Printing.Lib.Models.Daily_Operation;
using Com.Danliris.Service.Finishing.Printing.Lib.Models.DailyMonitoringEvent;
using Com.Danliris.Service.Finishing.Printing.Lib.Models.DyestuffChemicalUsageReceipt;
using Com.Danliris.Service.Finishing.Printing.Lib.Models.FabricQualityControl;
using Com.Danliris.Service.Finishing.Printing.Lib.Models.Kanban;
using Com.Danliris.Service.Finishing.Printing.Lib.Models.Master.BadOutput;
using Com.Danliris.Service.Finishing.Printing.Lib.Models.Master.DirectLaborCost;
using Com.Danliris.Service.Finishing.Printing.Lib.Models.Master.LossEvent;
using Com.Danliris.Service.Finishing.Printing.Lib.Models.Master.LossEventCategory;
using Com.Danliris.Service.Finishing.Printing.Lib.Models.Master.LossEventRemark;
using Com.Danliris.Service.Finishing.Printing.Lib.Models.Master.Machine;
using Com.Danliris.Service.Finishing.Printing.Lib.Models.Master.MachineType;
using Com.Danliris.Service.Finishing.Printing.Lib.Models.Master.OperationalCost;
using Com.Danliris.Service.Finishing.Printing.Lib.Models.Monitoring_Event;
using Com.Danliris.Service.Finishing.Printing.Lib.Models.Monitoring_Specification_Machine;
using Com.Danliris.Service.Finishing.Printing.Lib.Models.NewShipmentDocument;
using Com.Danliris.Service.Finishing.Printing.Lib.Models.Packing;
using Com.Danliris.Service.Finishing.Printing.Lib.Models.PackingReceipt;
using Com.Danliris.Service.Finishing.Printing.Lib.Models.ReturToQC;
using Com.Danliris.Service.Finishing.Printing.Lib.Models.ShipmentDocument;
using Com.Danliris.Service.Finishing.Printing.Lib.Models.StrikeOff;
using Com.Danliris.Service.Finishing.Printing.WebApi.Controllers.v1.UploadExcel.Excel_InventoryGreige;
using Com.Danliris.Service.Finishing.Printing.WebApi.Controllers.v1.UploadExcel.Excel_Preparing;
using Com.Danliris.Service.Finishing.Printing.WebApi.Controllers.v1.UploadExcel.Excel_Pretreatment;
using Com.Danliris.Service.Production.Lib.ModelConfigs.Master.DurationEstimation;
using Com.Danliris.Service.Production.Lib.ModelConfigs.Master.Instruction;
using Com.Danliris.Service.Production.Lib.ModelConfigs.Master.Step;
using Com.Danliris.Service.Production.Lib.Models.Master.DurationEstimation;
using Com.Danliris.Service.Production.Lib.Models.Master.Instruction;
using Com.Danliris.Service.Production.Lib.Models.Master.Step;
using Com.Moonlay.Data.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Com.Danliris.Service.Production.Lib
{
    public class ProductionDbContext : StandardDbContext
    {
        public ProductionDbContext(DbContextOptions<ProductionDbContext> options) : base(options)
        {
        }

        /* Master */
        public DbSet<StepIndicatorModel> StepIndicators { get; set; }
        public DbSet<StepModel> Steps { get; set; }
        public DbSet<InstructionStepIndicatorModel> InstructionStepIndicators { get; set; }
        public DbSet<InstructionStepModel> InstructionSteps { get; set; }
        public DbSet<InstructionModel> Instructions { get; set; }
        public DbSet<DurationEstimationModel> DurationEstimations { get; set; }
        public DbSet<DurationEstimationAreaModel> DurationEstimationAreas { get; set; }
        public DbSet<MachineTypeModel> MachineType { get; set; }
        public DbSet<MachineTypeIndicatorsModel> MachineTypeIndicators { get; set; }
        public DbSet<MachineModel> Machine { get; set; }
        public DbSet<MachineEventsModel> MachineEvents { get; set; }
        public DbSet<MachineStepModel> MachineSteps { get; set; }
        public DbSet<MonitoringSpecificationMachineModel> MonitoringSpecificationMachine { get; set; }
        public DbSet<MonitoringSpecificationMachineDetailsModel> MonitoringSpecificationMachineDetails { get; set; }
        public DbSet<KanbanModel> Kanbans { get; set; }
        public DbSet<KanbanInstructionModel> KanbanInstructions { get; set; }
        public DbSet<KanbanStepModel> KanbanSteps { get; set; }
        public DbSet<KanbanStepIndicatorModel> KanbanStepIndicators { get; set; }
        public DbSet<MonitoringEventModel> MonitoringEvent { get; set; }
        public DbSet<BadOutputModel> BadOutput { get; set; }
        public DbSet<BadOutputMachineModel> BadOutputMachine { get; set; }
        public DbSet<DailyOperationModel> DailyOperation { get; set; }
        public DbSet<DailyOperationBadOutputReasonsModel> DailyOperationBadOutputReasons { get; set; }
        public DbSet<PackingModel> Packings { get; set; }
        public DbSet<PackingDetailModel> PackingDetails { get; set; }
        public DbSet<FabricQualityControlModel> FabricQualityControls { get; set; }
        public DbSet<FabricGradeTestModel> FabricGradeTests { get; set; }
        public DbSet<CriteriaModel> Criterion { get; set; }
        public DbSet<PackingReceiptModel> PackingReceipt { get; set; }
        public DbSet<ReturToQCModel> ReturToQCs { get; set; }
        public DbSet<ReturToQCItemModel> ReturToQCItems { get; set; }
        public DbSet<ReturToQCItemDetailModel> ReturToQCItemDetails { get; set; }
        public DbSet<ShipmentDocumentModel> ShipmentDocuments { get; set; }
        public DbSet<ShipmentDocumentDetailModel> ShipmentDocumentDetails { get; set; }
        public DbSet<ShipmentDocumentItemModel> ShipmentDocumentItems { get; set; }
        public DbSet<ShipmentDocumentPackingReceiptItemModel> ShipmentDocumentPackingReceiptItems { get; set; }
        public DbSet<NewShipmentDocumentModel> NewShipmentDocuments { get; set; }
        public DbSet<NewShipmentDocumentDetailModel> NewShipmentDocumentDetails { get; set; }
        public DbSet<NewShipmentDocumentItemModel> NewShipmentDocumentItems { get; set; }
        public DbSet<NewShipmentDocumentPackingReceiptItemModel> NewShipmentDocumentPackingReceiptItems { get; set; }

        public DbSet<DirectLaborCostModel> DirectLaborCosts { get; set; }
        public DbSet<OperationalCostModel> OperationalCosts { get; set; }

        public DbSet<CostCalculationModel> CostCalculations { get; set; }
        public DbSet<CostCalculationMachineModel> CostCalculationMachines { get; set; }
        public DbSet<CostCalculationChemicalModel> CostCalculationChemicals { get; set; }

        public DbSet<KanbanSnapshotModel> KanbanSnapshots { get; set; }
        public DbSet<ColorReceiptModel> ColorReceipts { get; set; }
        public DbSet<ColorReceiptItemModel> ColorReceiptItems { get; set; }
        public DbSet<ColorReceiptDyeStuffReactiveModel> ColorReceiptDyeStuffReactives { get; set; }
        public DbSet<TechnicianModel> Technicians { get; set; }

        public DbSet<StrikeOffModel> StrikeOffs { get; set; }
        public DbSet<StrikeOffItemModel> StrikeOffItems { get; set; }
        public DbSet<StrikeOffItemChemicalItemModel> StrikeOffItemChemicalItems { get; set; }
        public DbSet<StrikeOffItemDyeStuffItemModel> StrikeOffItemDyeStuffItems { get; set; }

        public DbSet<DyestuffChemicalUsageReceiptModel> DyestuffChemicalUsageReceipts { get; set; }
        public DbSet<DyestuffChemicalUsageReceiptItemModel> DyestuffChemicalUsageReceiptItems { get; set; }
        public DbSet<DyestuffChemicalUsageReceiptItemDetailModel> DyestuffChemicalUsageReceiptItemDetails { get; set; }

        public DbSet<LossEventModel> LossEvents { get; set; }
        public DbSet<LossEventCategoryModel> LossEventCategories { get; set; }
        public DbSet<LossEventRemarkModel> LossEventRemarks { get; set; }

        public DbSet<DailyMonitoringEventModel> DailyMonitoringEvents { get; set; }
        public DbSet<DailyMonitoringEventLossEventItemModel> DailyMonitoringEventLossEventItems { get; set; }
        public DbSet<DailyMonitoringEventProductionOrderItemModel> DailyMonitoringEventProductionOrderItems { get; set; }

        public DbSet<Excel_AreaInventoryGreigeModel> Excel_AreaInventoryGreiges { get; set; }
        public DbSet<Excel_AreaInventoryGreigeMovementModel> Excel_AreaInventoryGreigeMovements { get; set; }
        public DbSet<Excel_AreaPreparingModel> Excel_AreaPreparings { get; set; }
        public DbSet<Excel_AreaPretreatmentChemicalUsedModel> Excel_AreaPretreatmentChemicalUseds { get; set; }
        public DbSet<Excel_AreaPretreatmentConditionCheckModel> Excel_AreaPretreatmentConditionChecks { get; set; }
        public DbSet<Excel_AreaPretreatmentDiaryMachineModel> Excel_AreaPretreatmentDiaryMachines { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new StepConfig());
            modelBuilder.ApplyConfiguration(new StepIndicatorConfig());
            modelBuilder.ApplyConfiguration(new InstructionConfig());
            modelBuilder.ApplyConfiguration(new InstructionStepConfig());
            modelBuilder.ApplyConfiguration(new InstructionStepIndicatorConfig());
            modelBuilder.ApplyConfiguration(new DurationEstimationConfig());
            modelBuilder.ApplyConfiguration(new DurationEstimationAreaConfig());
            modelBuilder.ApplyConfiguration(new KanbanConfig());
            modelBuilder.ApplyConfiguration(new KanbanInstructionConfig());
            modelBuilder.ApplyConfiguration(new KanbanStepConfig());
            modelBuilder.ApplyConfiguration(new KanbanStepIndicatorConfig());
            modelBuilder.ApplyConfiguration(new FabricQualityControlConfig());
            modelBuilder.ApplyConfiguration(new FabricGradeTestConfig());
            modelBuilder.ApplyConfiguration(new CriteriaConfig());
            base.OnModelCreating(modelBuilder);
        }
    }
}