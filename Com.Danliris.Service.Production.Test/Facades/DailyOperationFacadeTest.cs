﻿using AutoMapper;
using Com.Danliris.Service.Finishing.Printing.Lib.AutoMapperProfiles.DailyOperation;
using Com.Danliris.Service.Finishing.Printing.Lib.BusinessLogic.Facades.DailyOperation;
using Com.Danliris.Service.Finishing.Printing.Lib.BusinessLogic.Facades.Kanban;
using Com.Danliris.Service.Finishing.Printing.Lib.BusinessLogic.Facades.Master;
using Com.Danliris.Service.Finishing.Printing.Lib.BusinessLogic.Implementations.DailyOperation;
using Com.Danliris.Service.Finishing.Printing.Lib.BusinessLogic.Implementations.Kanban;
using Com.Danliris.Service.Finishing.Printing.Lib.BusinessLogic.Implementations.Master.Machine;
using Com.Danliris.Service.Finishing.Printing.Lib.Models.Daily_Operation;
using Com.Danliris.Service.Finishing.Printing.Lib.ViewModels.Daily_Operation;
using Com.Danliris.Service.Finishing.Printing.Test.DataUtils;
using Com.Danliris.Service.Finishing.Printing.Test.DataUtils.MasterDataUtils;
using Com.Danliris.Service.Finishing.Printing.Test.Utils;
using Com.Danliris.Service.Production.Lib;
using Com.Danliris.Service.Production.Lib.Services.IdentityService;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Com.Danliris.Service.Finishing.Printing.Test.Facades
{
    public class DailyOperationFacadeTest : BaseFacadeTest<ProductionDbContext, DailyOperationFacade, DailyOperationLogic, DailyOperationModel, DailyOperationDataUtil>
    {
        private const string ENTITY = "DailyOperation";

        public DailyOperationFacadeTest() : base(ENTITY)
        {
        }

        protected override DailyOperationDataUtil DataUtil(DailyOperationFacade facade, ProductionDbContext dbContext = null)
        {
            IServiceProvider serviceProvider = GetServiceProviderMock(dbContext).Object;

            MachineFacade machineFacade = new MachineFacade(serviceProvider, dbContext);
            MachineDataUtil machineDataUtil = new MachineDataUtil(machineFacade);

            KanbanFacade kanbanFacade = new KanbanFacade(serviceProvider, dbContext);
            KanbanDataUtil kanbanDataUtil = new KanbanDataUtil(machineDataUtil, kanbanFacade);

            DailyOperationDataUtil dataUtil = new DailyOperationDataUtil(kanbanDataUtil, facade);

            return dataUtil;
        }

        protected override Mock<IServiceProvider> GetServiceProviderMock(ProductionDbContext dbContext)
        {
            var serviceProviderMock = new Mock<IServiceProvider>();

            IIdentityService identityService = new IdentityService { Username = "Username" };

            serviceProviderMock
                .Setup(x => x.GetService(typeof(IdentityService)))
                .Returns(identityService);

            MachineEventLogic machineEventLogic = new MachineEventLogic(identityService, dbContext);
            MachineStepLogic machineStepLogic = new MachineStepLogic(identityService, dbContext);

            serviceProviderMock
                .Setup(x => x.GetService(typeof(MachineLogic)))
                .Returns(new MachineLogic(machineEventLogic, machineStepLogic, identityService, dbContext));

            serviceProviderMock
                .Setup(x => x.GetService(typeof(KanbanLogic)))
                .Returns(new KanbanLogic(identityService, dbContext));

            DailyOperationBadOutputReasonsLogic dailyOperationBadOutputReasonsLogic = new DailyOperationBadOutputReasonsLogic(identityService, dbContext);

            serviceProviderMock
                .Setup(x => x.GetService(typeof(DailyOperationLogic)))
                .Returns(new DailyOperationLogic(dailyOperationBadOutputReasonsLogic, identityService, dbContext));

            return serviceProviderMock;
        }

        [Fact]
        public async Task GetJoinKanban_WithoutNo()
        {
            var dbContext = DbContext(GetCurrentMethod());
            var serviceProvider = GetServiceProviderMock(dbContext).Object;

            DailyOperationFacade facade = Activator.CreateInstance(typeof(DailyOperationFacade), serviceProvider, dbContext) as DailyOperationFacade;

            var data = await DataUtil(facade, dbContext).GetTestData();

            var Response = await facade.GetJoinKanban(null);

            Assert.NotNull(Response);
        }

        [Fact]
        public async Task GetJoinKanban_WithNo()
        {
            var dbContext = DbContext(GetCurrentMethod());
            var serviceProvider = GetServiceProviderMock(dbContext).Object;

            DailyOperationFacade facade = Activator.CreateInstance(typeof(DailyOperationFacade), serviceProvider, dbContext) as DailyOperationFacade;

            var data = await DataUtil(facade, dbContext).GetTestData();

            var Response = await facade.GetJoinKanban("a");

            Assert.NotNull(Response);
        }

        [Fact]
        public async Task GetReport()
        {
            var dbContext = DbContext(GetCurrentMethod());
            var serviceProvider = GetServiceProviderMock(dbContext).Object;

            DailyOperationFacade facade = Activator.CreateInstance(typeof(DailyOperationFacade), serviceProvider, dbContext) as DailyOperationFacade;

            var data = await DataUtil(facade, dbContext).GetTestData();

            var Response = facade.GetReport(1, 25, -1, -1, DateTime.UtcNow.AddDays(-30), DateTime.UtcNow.AddDays(30), 7);

            Assert.NotEmpty(Response.Data);
        }

        [Fact]
        public async Task HasOutput_False()
        {
            var dbContext = DbContext(GetCurrentMethod());
            var serviceProvider = GetServiceProviderMock(dbContext).Object;

            DailyOperationFacade facade = Activator.CreateInstance(typeof(DailyOperationFacade), serviceProvider, dbContext) as DailyOperationFacade;

            var data = await DataUtil(facade, dbContext).GetTestData();

            var Response = await facade.HasOutput(data.KanbanId, data.StepProcess, data.MachineId, data.KanbanStepIndex);

            Assert.False(Response);
        }

        [Fact]
        public async Task Should_Success_Set_Kanban_When_Create_Daily_Operation()
        {
            var dbContext = DbContext(GetCurrentMethod());
            IIdentityService identityService = new IdentityService { Username = "Username" };

            var dailyOperationBadOutputReasonsLogic = new DailyOperationBadOutputReasonsLogic(identityService, dbContext);
            var dailyOperationLogic = new DailyOperationLogic(dailyOperationBadOutputReasonsLogic, identityService, dbContext);

            var serviceProvider = GetServiceProviderMock(dbContext).Object;
            DailyOperationFacade facade = Activator.CreateInstance(typeof(DailyOperationFacade), serviceProvider, dbContext) as DailyOperationFacade;
            var data = await DataUtil(facade, dbContext).GetTestData();

            var outputData = DataUtil(facade, dbContext).GetNewDataOut(data);

            await facade.CreateAsync(outputData);
            Assert.NotNull(data);
        }

        [Fact]
        public async Task Should_Success_Delete()
        {
            var dbContext = DbContext(GetCurrentMethod());
            IIdentityService identityService = new IdentityService { Username = "Username" };

            var dailyOperationBadOutputReasonsLogic = new DailyOperationBadOutputReasonsLogic(identityService, dbContext);
            var dailyOperationLogic = new DailyOperationLogic(dailyOperationBadOutputReasonsLogic, identityService, dbContext);

            var serviceProvider = GetServiceProviderMock(dbContext).Object;
            DailyOperationFacade facade = Activator.CreateInstance(typeof(DailyOperationFacade), serviceProvider, dbContext) as DailyOperationFacade;
            var data = await DataUtil(facade, dbContext).GetTestData();

            var outputData = DataUtil(facade, dbContext).GetNewDataOut(data);

            await facade.CreateAsync(outputData);
            await facade.DeleteAsync(outputData.Id);
            Assert.NotNull(data);
        }

        [Fact]
        public async Task Should_Success_Update()
        {
            var dbContext = DbContext(GetCurrentMethod());
            IIdentityService identityService = new IdentityService { Username = "Username" };

            var dailyOperationBadOutputReasonsLogic = new DailyOperationBadOutputReasonsLogic(identityService, dbContext);
            var dailyOperationLogic = new DailyOperationLogic(dailyOperationBadOutputReasonsLogic, identityService, dbContext);

            var serviceProvider = GetServiceProviderMock(dbContext).Object;
            DailyOperationFacade facade = Activator.CreateInstance(typeof(DailyOperationFacade), serviceProvider, dbContext) as DailyOperationFacade;
            var data = await DataUtil(facade, dbContext).GetTestData();

            var outputData = DataUtil(facade, dbContext).GetNewDataOut(data);

            await facade.CreateAsync(outputData);

            outputData.BadOutput = 10;
            await facade.UpdateAsync(outputData.Id, outputData);
            Assert.NotNull(data);
        }

        [Fact]
        public async Task Should_Success_Update_Add()
        {
            var dbContext = DbContext(GetCurrentMethod() + "Should_Success_Update_Add");
            IIdentityService identityService = new IdentityService { Username = "Username" };

            var dailyOperationBadOutputReasonsLogic = new DailyOperationBadOutputReasonsLogic(identityService, dbContext);
            var dailyOperationLogic = new DailyOperationLogic(dailyOperationBadOutputReasonsLogic, identityService, dbContext);

            var serviceProvider = GetServiceProviderMock(dbContext).Object;
            DailyOperationFacade facade = Activator.CreateInstance(typeof(DailyOperationFacade), serviceProvider, dbContext) as DailyOperationFacade;
            var data = await DataUtil(facade, dbContext).GetTestData();

            var outputData = DataUtil(facade, dbContext).GetNewDataOut(data);

            await facade.CreateAsync(outputData);

            outputData.BadOutput = 10;
            outputData.BadOutputReasons = new List<DailyOperationBadOutputReasonsModel>()
            {
                new DailyOperationBadOutputReasonsModel()
                {
                    MachineCode = "c"
                }
            };
            await facade.UpdateAsync(outputData.Id, outputData);
            Assert.NotNull(data);
        }

        [Fact]
        public virtual async void Should_Success_Read()
        {
            var dbContext = DbContext(GetCurrentMethod());
            var serviceProvider = GetServiceProviderMock(dbContext).Object;

            var facade = new DailyOperationFacade(serviceProvider, dbContext);

            var data = await DataUtil(facade, dbContext).GetTestData();

            var Response = facade.Read(1, 25, "{}", new System.Collections.Generic.List<string>(), null, null, null, null, null, null, DateTime.UtcNow.AddDays(-30), DateTime.UtcNow.AddDays(30));

            Assert.NotEmpty(Response.Data);
        }

        [Fact]
        public async Task GenerateExcel()
        {
            var dbContext = DbContext(GetCurrentMethod());
            var serviceProvider = GetServiceProviderMock(dbContext).Object;

            DailyOperationFacade facade = Activator.CreateInstance(typeof(DailyOperationFacade), serviceProvider, dbContext) as DailyOperationFacade;

            var data = await DataUtil(facade, dbContext).GetTestData();

            var Response = facade.GenerateExcel(-1, -1, DateTime.UtcNow.AddDays(-30), DateTime.UtcNow.AddDays(30), 7);

            Assert.NotNull(Response);
        }

        [Fact]
        public void  GenerateExcel_with_Empty_Data()
        {
            var dbContext = DbContext(GetCurrentMethod());
            var serviceProvider = GetServiceProviderMock(dbContext).Object;

            DailyOperationFacade facade = Activator.CreateInstance(typeof(DailyOperationFacade), serviceProvider, dbContext) as DailyOperationFacade;

            var Response = facade.GenerateExcel(-1, -1, DateTime.UtcNow.AddDays(-30), DateTime.UtcNow.AddDays(30), 7);

            Assert.NotNull(Response);
        }

        [Fact]
        public async Task Should_Success_GetOutputBadDesc()
        {
            var dbContext = DbContext(GetCurrentMethod());
            IIdentityService identityService = new IdentityService { Username = "Username" };

            var dailyOperationBadOutputReasonsLogic = new DailyOperationBadOutputReasonsLogic(identityService, dbContext);
            var dailyOperationLogic = new DailyOperationLogic(dailyOperationBadOutputReasonsLogic, identityService, dbContext);

            var serviceProvider = GetServiceProviderMock(dbContext).Object;
            DailyOperationFacade facade = Activator.CreateInstance(typeof(DailyOperationFacade), serviceProvider, dbContext) as DailyOperationFacade;
            var data = await DataUtil(facade, dbContext).GetTestData();

            var outputData = DataUtil(facade, dbContext).GetNewDataOut(data);
            outputData.BadOutputReasons.Add(new DailyOperationBadOutputReasonsModel()
            {
                MachineName = "name",
                Action = "ac",
                BadOutputReason = "reas"
            });
            await facade.CreateAsync(outputData);

            outputData.BadOutput = 10;
            var response = facade.GetOutputBadDescription(outputData);

            Assert.NotNull(response);
        }

        [Fact]
        public void Validate_VM_NULL()
        {
            var dbContext = DbContext(GetCurrentMethod());
            var serviceProvider = GetServiceProviderMock(dbContext).Object;

            DailyOperationViewModel vm = new DailyOperationViewModel()
            {
                IsEdit = false,
                UId = "a",
                BadOutputDescription = "a"
            };

            var fla = vm.IsChangeable;
            var desc = vm.BadOutputDescription;
            System.ComponentModel.DataAnnotations.ValidationContext context = new System.ComponentModel.DataAnnotations.ValidationContext(vm, serviceProvider, null);
            Assert.NotEmpty(vm.Validate(context));

            vm.Type = "s";
            var result = vm.Validate(context);
            Assert.NotEmpty(result);
            Assert.True(0 < result.Count());

            vm.Shift = "s";
             result = vm.Validate(context);
            Assert.NotEmpty(result);
            Assert.True(0 < result.Count());

            vm.Machine = new Lib.ViewModels.Master.Machine.MachineViewModel();
            Assert.NotEmpty(vm.Validate(context));

            vm.Step = new Lib.ViewModels.Master.Machine.MachineStepViewModel();

            result = vm.Validate(context);
            Assert.NotEmpty(result);
            Assert.True(0 < result.Count());

            vm.Kanban = new Lib.ViewModels.Kanban.KanbanViewModel()
            {
                CurrentStepIndex = 0,
                Instruction = new Lib.ViewModels.Kanban.KanbanInstructionViewModel()
                {
                    Steps = new List<Lib.ViewModels.Kanban.KanbanStepViewModel>()
                    {
                        new Lib.ViewModels.Kanban.KanbanStepViewModel()
                        {
                            Process = "process"
                        }
                    }
                }
            };

            result = vm.Validate(context);
            Assert.NotEmpty(result);
            Assert.True(0 < result.Count());


            vm.Kanban = new Lib.ViewModels.Kanban.KanbanViewModel();

            vm.Type = "input";
            result = vm.Validate(context);
            Assert.NotEmpty(result);
            Assert.True(0 < result.Count());

            vm.DateInput = DateTime.Now.AddDays(1);
            result = vm.Validate(context);
            Assert.NotEmpty(result);
            Assert.True(0 < result.Count());

            vm.DateInput = null;
            result = vm.Validate(context);
            Assert.NotEmpty(result);
            Assert.True(0 < result.Count());

            vm.Input = 0;
            result = vm.Validate(context);
            Assert.NotEmpty(result);
            Assert.True(0 < result.Count());

            vm.TimeInput = 0;
            result = vm.Validate(context);
            Assert.NotEmpty(result);
            Assert.True(0 < result.Count());

            vm.Type = "output";
            vm.BadOutputReasons = new List<DailyOperationBadOutputReasonsViewModel>();
            result = vm.Validate(context);
            Assert.NotEmpty(result);
            Assert.True(0 < result.Count());

            vm.DateOutput = DateTime.Now;
            vm.BadOutputReasons = new List<DailyOperationBadOutputReasonsViewModel>();
            vm.BadOutput = 1;
            result = vm.Validate(context);
            Assert.NotEmpty(result);
            Assert.True(0 < result.Count());

            vm.DateOutput = DateTime.Now;
            vm.BadOutputReasons = new List<DailyOperationBadOutputReasonsViewModel>();
            vm.BadOutput = 1;
            result = vm.Validate(context);
            Assert.NotEmpty(result);
            Assert.True(0 < result.Count());

            vm.BadOutputReasons = new List<DailyOperationBadOutputReasonsViewModel>()
            {
                new DailyOperationBadOutputReasonsViewModel()
                {
                    Length = 0
                }
            };
            vm.BadOutput = 1;
            result = vm.Validate(context);
            Assert.NotEmpty(result);
            Assert.True(0 < result.Count());

            vm.DateOutput = DateTime.Now.AddDays(1);
            result = vm.Validate(context);
            Assert.NotEmpty(result);
            Assert.True(0 < result.Count());

            vm.DateOutput = null;
            result = vm.Validate(context);
            Assert.NotEmpty(result);
            Assert.True(0 < result.Count());

            vm.GoodOutput = -1;
            result = vm.Validate(context);
            Assert.NotEmpty(result);
            Assert.True(0 < result.Count());

            vm.TimeOutput = 0; result = vm.Validate(context);
            Assert.NotEmpty(result);
            Assert.True(0 < result.Count());
        }

        [Fact]
        public async Task Validate_VM_Exist()
        {
            var dbContext = DbContext(GetCurrentMethod());
            var serviceProvider = GetServiceProviderMock(dbContext).Object;

            DailyOperationFacade facade = Activator.CreateInstance(typeof(DailyOperationFacade), serviceProvider, dbContext) as DailyOperationFacade;

            var data = await DataUtil(facade, dbContext).GetTestData();
            DailyOperationViewModel vm = new DailyOperationViewModel()
            {
                Type = data.Type,
                Shift = data.Shift,
                Kanban = new Lib.ViewModels.Kanban.KanbanViewModel()
                {
                    Id = data.KanbanId
                },
                Step = new Lib.ViewModels.Master.Machine.MachineStepViewModel()
                {
                    StepId = data.StepId
                },
                Machine = new Lib.ViewModels.Master.Machine.MachineViewModel(),

            };
            System.ComponentModel.DataAnnotations.ValidationContext context = new System.ComponentModel.DataAnnotations.ValidationContext(vm, serviceProvider, null);
            Assert.NotEmpty(vm.Validate(context));
        }

        [Fact]
        public async Task ValidateVM_Input()
        {
            var dbContext = DbContext(GetCurrentMethod());
            var serviceProvider = GetServiceProviderMock(dbContext).Object;
            DailyOperationFacade facade = Activator.CreateInstance(typeof(DailyOperationFacade), serviceProvider, dbContext) as DailyOperationFacade;

            var data = await DataUtil(facade, dbContext).GetNewDataAsync();
            data.DateInput = DateTimeOffset.UtcNow.AddDays(-2);
            var resI = await facade.CreateAsync(data);
            var dataO = DataUtil(facade, dbContext).GetNewDataOut(data);
            dataO.DateOutput = DateTime.UtcNow.AddDays(-1);
            var result = await facade.CreateAsync(dataO);
            DailyOperationViewModel vm = new DailyOperationViewModel()
            {
                Type = "input",
                Shift = "s",
                Machine = new Lib.ViewModels.Master.Machine.MachineViewModel(),
                Step = new Lib.ViewModels.Master.Machine.MachineStepViewModel()
            };
            System.ComponentModel.DataAnnotations.ValidationContext context = new System.ComponentModel.DataAnnotations.ValidationContext(vm, serviceProvider, null);
            Assert.NotEmpty(vm.Validate(context));

            vm.TimeInput = 1;
            Assert.NotEmpty(vm.Validate(context));
            vm.Input = 1;
            Assert.NotEmpty(vm.Validate(context));
            vm.DateInput = DateTimeOffset.UtcNow.AddDays(1);
            Assert.NotEmpty(vm.Validate(context));
            vm.DateInput = DateTimeOffset.UtcNow.AddDays(-1);
            Assert.NotEmpty(vm.Validate(context));

            vm.DateInput = dataO.DateOutput.Value.AddHours(-1);
            vm.Kanban = new Lib.ViewModels.Kanban.KanbanViewModel()
            {
                CurrentStepIndex = data.KanbanStepIndex,
                Id = data.KanbanId,
                Instruction = new Lib.ViewModels.Kanban.KanbanInstructionViewModel()
                {
                    Steps = new List<Lib.ViewModels.Kanban.KanbanStepViewModel>()
                }
            };
            Assert.NotEmpty(vm.Validate(context));

            vm.Kanban = new Lib.ViewModels.Kanban.KanbanViewModel()
            {
                CurrentStepIndex = 2,
            };
            Assert.NotEmpty(vm.Validate(context));

            vm.Kanban = new Lib.ViewModels.Kanban.KanbanViewModel()
            {
                CurrentStepIndex = 0,
                Instruction = new Lib.ViewModels.Kanban.KanbanInstructionViewModel()
                {
                    Steps = new List<Lib.ViewModels.Kanban.KanbanStepViewModel>()
                    {
                        new Lib.ViewModels.Kanban.KanbanStepViewModel()
                        {
                            Process = "a",
                            StepIndex = 1
                        }
                    }
                }
            };
            Assert.NotEmpty(vm.Validate(context));


        }

        [Fact]
        public async Task ValidateVM_Output()
        {
            var dbContext = DbContext(GetCurrentMethod());
            var serviceProvider = GetServiceProviderMock(dbContext).Object;
            DailyOperationFacade facade = Activator.CreateInstance(typeof(DailyOperationFacade), serviceProvider, dbContext) as DailyOperationFacade;

            var data = await DataUtil(facade, dbContext).GetTestData();
            var dataO = DataUtil(facade, dbContext).GetNewDataOut(data);
            dataO.KanbanId = data.KanbanId;
            await facade.CreateAsync(dataO);
            DailyOperationViewModel vm = new DailyOperationViewModel()
            {
                Type = "output",
                Shift = "s",
                Machine = new Lib.ViewModels.Master.Machine.MachineViewModel(),
                Step = new Lib.ViewModels.Master.Machine.MachineStepViewModel()
            };
            System.ComponentModel.DataAnnotations.ValidationContext context = new System.ComponentModel.DataAnnotations.ValidationContext(vm, serviceProvider, null);
            Assert.NotEmpty(vm.Validate(context));

            vm.TimeOutput = 1;
            Assert.NotEmpty(vm.Validate(context));
            vm.GoodOutput = 1;
            Assert.NotEmpty(vm.Validate(context));
            vm.DateOutput = DateTimeOffset.UtcNow.AddDays(1);
            Assert.NotEmpty(vm.Validate(context));
            vm.DateOutput = DateTimeOffset.UtcNow.AddDays(-1);
            vm.BadOutputReasons = new List<DailyOperationBadOutputReasonsViewModel>()
            {
            };
            Assert.NotEmpty(vm.Validate(context));

            vm.BadOutput = 1;

            Assert.NotEmpty(vm.Validate(context));

            vm.BadOutput = 0;
            Assert.NotEmpty(vm.Validate(context));

            vm.Kanban = new Lib.ViewModels.Kanban.KanbanViewModel()
            {
                CurrentStepIndex = data.KanbanStepIndex,
                Id = data.KanbanId
            };
            Assert.NotEmpty(vm.Validate(context));



            vm.Kanban = new Lib.ViewModels.Kanban.KanbanViewModel()
            {
                CurrentStepIndex = dataO.KanbanStepIndex,
                Id = dataO.KanbanId
            };
            Assert.NotEmpty(vm.Validate(context));

            vm.Kanban = new Lib.ViewModels.Kanban.KanbanViewModel()
            {
                CurrentStepIndex = 0,
                Instruction = new Lib.ViewModels.Kanban.KanbanInstructionViewModel()
                {
                    Steps = new List<Lib.ViewModels.Kanban.KanbanStepViewModel>()
                    {
                        new Lib.ViewModels.Kanban.KanbanStepViewModel()
                        {
                            Process = "a",
                            StepIndex = 1
                        }
                    }
                }
            };
            Assert.NotEmpty(vm.Validate(context));

            vm.GoodOutput = 0;
            vm.BadOutput = 0;
            Assert.NotEmpty(vm.Validate(context));

        }

        [Fact]
        public void Mapping_With_AutoMapper_Profiles()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<DailyOperationProfile>();
            });
            var mapper = configuration.CreateMapper();

            DailyOperationViewModel vm = new DailyOperationViewModel { Id = 1 };
            DailyOperationModel model = mapper.Map<DailyOperationModel>(vm);

            Assert.Equal(vm.Id, model.Id);

        }

        [Fact]
        public async void Create_ThrowException()
        {
            var dbContext = DbContext(GetCurrentMethod());
            var serviceProvider = GetServiceProviderMock(dbContext);

            var facade = new DailyOperationFacade(serviceProvider.Object, dbContext);

            await Assert.ThrowsAnyAsync<Exception>(() => facade.CreateAsync(null));
        }

        [Fact]
        public async void Update_ThrowException()
        {
            var dbContext = DbContext(GetCurrentMethod());
            var serviceProvider = GetServiceProviderMock(dbContext);

            var facade = new DailyOperationFacade(serviceProvider.Object, dbContext);

            await Assert.ThrowsAnyAsync<Exception>(() => facade.UpdateAsync(0, null));
        }

        [Fact]
        public async void Delete_ThrowException()
        {
            var dbContext = DbContext(GetCurrentMethod());
            var serviceProvider = GetServiceProviderMock(dbContext);

            var facade = new DailyOperationFacade(serviceProvider.Object, dbContext);

            await Assert.ThrowsAnyAsync<Exception>(() => facade.DeleteAsync(0));
        }


      

        [Fact]
        public async void CreateDO_Type_Output_AreaPreTreatment_When_DataExist()
        {
            var dbContext = DbContext(GetCurrentMethod());

            int result = 0;
            var serviceProvider = GetServiceProviderMock(dbContext);
            var facade = new DailyOperationFacade(serviceProvider.Object, dbContext);
            var kanban = await DataUtil(facade, dbContext).GetKanban();

            var kanbanSnapShot = DataUtil(facade, dbContext).GetKanbanSnapshot();
            dbContext.KanbanSnapshots.Add(kanbanSnapShot);
            dbContext.SaveChanges();


            var ptData = DataUtil(facade, dbContext).GetNewDataInputPreTreatment(kanban);
            result = await facade.CreateAsync(ptData);
         

            var outPTData = DataUtil(facade, dbContext).GetNewDataOut(ptData);
            result = await facade.CreateAsync(outPTData);
            Assert.NotEqual(0, result);


        }

        [Fact]
        public async void CreateDOFullKanban()
        {
            var dbContext = DbContext(GetCurrentMethod());
            
            int result = 0;
            var serviceProvider = GetServiceProviderMock(dbContext);

            var facade = new DailyOperationFacade(serviceProvider.Object, dbContext);

            var kanban = await DataUtil(facade, dbContext).GetKanban();

            
            var ptData = DataUtil(facade, dbContext).GetNewDataInputPreTreatment(kanban);
            result = await facade.CreateAsync(ptData);

            Assert.NotEqual(0, result);
           

            var outPTData = DataUtil(facade, dbContext).GetNewDataOut(ptData);
            
            result = await facade.CreateAsync(outPTData);
            Assert.NotEqual(0, result);
            

            var dData = DataUtil(facade, dbContext).GetNewDataInputDyeing(kanban);

            result = await facade.CreateAsync(dData);
            Assert.NotEqual(0, result);

            var outDData = DataUtil(facade, dbContext).GetNewDataOut(dData);
            result = await facade.CreateAsync(outDData);
            Assert.NotEqual(0, result);

            var pData = DataUtil(facade, dbContext).GetNewDataInputPrinting(kanban);
            result = await facade.CreateAsync(pData);
            Assert.NotEqual(0, result);

            var outPData = DataUtil(facade, dbContext).GetNewDataOut(pData);
            result = await facade.CreateAsync(outPData);
            Assert.NotEqual(0, result);

            var fData = DataUtil(facade, dbContext).GetNewDataInputFinishing(kanban);
            result = await facade.CreateAsync(fData);
            Assert.NotEqual(0, result);

            var outFData = DataUtil(facade, dbContext).GetNewDataOut(fData);
            result = await facade.CreateAsync(outFData);
            Assert.NotEqual(0, result);

            var qData = DataUtil(facade, dbContext).GetNewDataInputQC(kanban);
            result = await facade.CreateAsync(qData);
            Assert.NotEqual(0, result);

            var outQData = DataUtil(facade, dbContext).GetNewDataOut(qData);
            result = await facade.CreateAsync(outQData);
            Assert.NotEqual(0, result);
        }


        [Fact]
        public async void CreateDataOutputAreaPretreatmentYesterday()
        {
            var dbContext = DbContext(GetCurrentMethod());

            int result = 0;
            var serviceProvider = GetServiceProviderMock(dbContext);

            var facade = new DailyOperationFacade(serviceProvider.Object, dbContext);
            var kanbanSnapShot = DataUtil(facade, dbContext).GetKanbanSnapshotPreTreatmentOutputDateYesterday();
            dbContext.KanbanSnapshots.Add(kanbanSnapShot);
            dbContext.SaveChanges();

            var kanban = await DataUtil(facade, dbContext).GetKanban();

            var ptData = DataUtil(facade, dbContext).GetNewDataInputPreTreatment(kanban);
            result = await facade.CreateAsync(ptData);
            Assert.NotEqual(0, result);

            var outPTData = DataUtil(facade, dbContext).GetNewDataOut(ptData);
            result = await facade.CreateAsync(outPTData);
            Assert.NotEqual(0, result);

        }


        [Fact]
        public async void CreateDataInputAreaDyeing()
        {
            var dbContext = DbContext(GetCurrentMethod());

            int result = 0;
            var serviceProvider = GetServiceProviderMock(dbContext);

            var facade = new DailyOperationFacade(serviceProvider.Object, dbContext);

            var kanbanSnapShot = DataUtil(facade, dbContext).GetKanbanSnapshot();
            dbContext.KanbanSnapshots.Add(kanbanSnapShot);
            dbContext.SaveChanges();

            var kanban = await DataUtil(facade, dbContext).GetKanban();

            var dData = DataUtil(facade, dbContext).GetNewDataInputDyeing(kanban);
            dData.KanbanStepIndex = 3;
            result = await facade.CreateAsync(dData);


            var outDData = DataUtil(facade, dbContext).GetNewDataOut(dData);
            result = await facade.CreateAsync(outDData);
            Assert.NotEqual(0, result);

            Assert.NotEqual(0, result);


        }

        [Fact]
        public async void EditDOFullKanban()
        {
            var dbContext = DbContext(GetCurrentMethod());
            int result = 0;
            var serviceProvider = GetServiceProviderMock(dbContext);

            var facade = new DailyOperationFacade(serviceProvider.Object, dbContext);

            var kanban = await DataUtil(facade, dbContext).GetKanban();

            var ptData = DataUtil(facade, dbContext).GetNewDataInputPreTreatment(kanban);
            await facade.CreateAsync(ptData);
            result = await facade.UpdateAsync(ptData.Id, ptData);
            Assert.NotEqual(0, result);
            var outPTData = DataUtil(facade, dbContext).GetNewDataOut(ptData);
            await facade.CreateAsync(outPTData);
            result = await facade.UpdateAsync(outPTData.Id, outPTData);
            Assert.NotEqual(0, result);


            var dData = DataUtil(facade, dbContext).GetNewDataInputDyeing(kanban);
            await facade.CreateAsync(dData);
            result = await facade.UpdateAsync(dData.Id, dData);
            Assert.NotEqual(0, result);

            var outDData = DataUtil(facade, dbContext).GetNewDataOut(dData);
            await facade.CreateAsync(outDData);
            result = await facade.UpdateAsync(outDData.Id, outDData);
            Assert.NotEqual(0, result);

            var pData = DataUtil(facade, dbContext).GetNewDataInputPrinting(kanban);
            await facade.CreateAsync(pData);
            result = await facade.UpdateAsync(pData.Id, pData);
            Assert.NotEqual(0, result);

            var outPData = DataUtil(facade, dbContext).GetNewDataOut(pData);
            await facade.CreateAsync(outPData);
            result = await facade.UpdateAsync(outPData.Id, outPData);
            Assert.NotEqual(0, result);

            var fData = DataUtil(facade, dbContext).GetNewDataInputFinishing(kanban);
            await facade.CreateAsync(fData);
            result = await facade.UpdateAsync(fData.Id, fData);
            Assert.NotEqual(0, result);

            var outFData = DataUtil(facade, dbContext).GetNewDataOut(fData);
            await facade.CreateAsync(outFData);
            result = await facade.UpdateAsync(outFData.Id, outFData);
            Assert.NotEqual(0, result);

            var qData = DataUtil(facade, dbContext).GetNewDataInputQC(kanban);
            await facade.CreateAsync(qData);
            result = await facade.UpdateAsync(qData.Id, qData);
            Assert.NotEqual(0, result);

            var outQData = DataUtil(facade, dbContext).GetNewDataOut(qData);
            await facade.CreateAsync(outQData);
            result = await facade.UpdateAsync(outQData.Id, outQData);
            Assert.NotEqual(0, result);
        }

        [Fact]
        public async void DeleteDOFullKanban()
        {
            var dbContext = DbContext(GetCurrentMethod());
            int result = 0;
            var serviceProvider = GetServiceProviderMock(dbContext);

            var facade = new DailyOperationFacade(serviceProvider.Object, dbContext);

            var kanban = await DataUtil(facade, dbContext).GetKanban();

            var ptData = DataUtil(facade, dbContext).GetNewDataInputPreTreatment(kanban);
            await facade.CreateAsync(ptData);

            var outPTData = DataUtil(facade, dbContext).GetNewDataOut(ptData);
            await facade.CreateAsync(outPTData);


            var dData = DataUtil(facade, dbContext).GetNewDataInputDyeing(kanban);
            await facade.CreateAsync(dData);

            var outDData = DataUtil(facade, dbContext).GetNewDataOut(dData);
            await facade.CreateAsync(outDData);

            var pData = DataUtil(facade, dbContext).GetNewDataInputPrinting(kanban);
            await facade.CreateAsync(pData);

            var outPData = DataUtil(facade, dbContext).GetNewDataOut(pData);
            await facade.CreateAsync(outPData);

            var fData = DataUtil(facade, dbContext).GetNewDataInputFinishing(kanban);
            await facade.CreateAsync(fData);

            var outFData = DataUtil(facade, dbContext).GetNewDataOut(fData);
            await facade.CreateAsync(outFData);

            var qData = DataUtil(facade, dbContext).GetNewDataInputQC(kanban);
            await facade.CreateAsync(qData);

            var outQData = DataUtil(facade, dbContext).GetNewDataOut(qData);
            await facade.CreateAsync(outQData);


            result = await facade.DeleteAsync(outQData.Id);
            Assert.NotEqual(0, result);

            result = await facade.DeleteAsync(qData.Id);
            Assert.NotEqual(0, result);

            result = await facade.DeleteAsync(outFData.Id);
            Assert.NotEqual(0, result);

            result = await facade.DeleteAsync(fData.Id);
            Assert.NotEqual(0, result);

            result = await facade.DeleteAsync(outPData.Id);
            Assert.NotEqual(0, result);

            result = await facade.DeleteAsync(pData.Id);
            Assert.NotEqual(0, result);

            result = await facade.DeleteAsync(outDData.Id);
            Assert.NotEqual(0, result);

            result = await facade.DeleteAsync(dData.Id);
            Assert.NotEqual(0, result);

            result = await facade.DeleteAsync(outPTData.Id);
            Assert.NotEqual(0, result);

            result = await facade.DeleteAsync(ptData.Id);
            Assert.NotEqual(0, result);
        }

        //[Fact]
        //public async Task ETLKanbanStep()
        //{
        //    var dbContext = DbContext(GetCurrentMethod());
        //    var serviceProvider = GetServiceProviderMock(dbContext).Object;

        //    var facade = new DailyOperationFacade(serviceProvider, dbContext);

        //    var data = await DataUtil(facade, dbContext).GetTestData();
        //    var dataO = await DataUtil(facade, dbContext).GetNewDataOutAsync();
        //    dataO.KanbanId = data.KanbanId;
        //    dataO.StepProcess = data.StepProcess;
        //    await facade.CreateAsync(dataO);

        //    await facade.ETLKanbanStepIndex(1);
        //    Assert.True(true);
        //}

    }
}
