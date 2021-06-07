﻿using Com.Danliris.Service.Finishing.Printing.Lib.BusinessLogic.Interfaces.Master;
using Com.Danliris.Service.Finishing.Printing.Lib.Models.Master.LossEvent;
using Com.Danliris.Service.Finishing.Printing.Lib.ViewModels.Master.LossEvent;
using Com.Danliris.Service.Finishing.Printing.Test.Controller.Utils;
using Com.Danliris.Service.Finishing.Printing.WebApi.Controllers.v1.Master;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Finishing.Printing.Test.Controllers
{
    public class LossEventControllerTest : BaseControllerTest<LossEventController, LossEventModel, LossEventViewModel, ILossEventFacade>
    {
    }
}
