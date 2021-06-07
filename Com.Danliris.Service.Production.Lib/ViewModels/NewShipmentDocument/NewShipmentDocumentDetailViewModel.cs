﻿using Com.Danliris.Service.Production.Lib.Utilities.BaseClass;
using Com.Danliris.Service.Production.Lib.ViewModels.Integration.Sales.FinishingPrinting;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Com.Danliris.Service.Finishing.Printing.Lib.ViewModels.NewShipmentDocument
{
    public class NewShipmentDocumentDetailViewModel : BaseViewModel, IValidatableObject
    {
        public string ProductionOrderColorType { get; set; }
        public string ProductionOrderDesignCode { get; set; }
        public string ProductionOrderDesignNumber { get; set; }
        public int? ProductionOrderId { get; set; }
        public string ProductionOrderNo { get; set; }
        public string ProductionOrderType { get; set; }
        public ProductionOrderIntegrationViewModel ProductionOrder { get; set; }
        public List<NewShipmentDocumentItemViewModel> Items { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            throw new System.NotImplementedException();
        }
    }
}