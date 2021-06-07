﻿using System.Collections.Generic;

namespace Com.Danliris.Service.Production.Lib.Utilities
{
    public class ReadResponse<TModel>
    {
        public List<TModel> Data { get; set; }
        public int Count { get; set; }
        public Dictionary<string, string> Order { get; set; }
        public List<string> Selected { get; set; }
        public int Total { get; set; }
        public ReadResponse(List<TModel> data, int count, Dictionary<string, string> order, List<string> selected)
        {
            this.Data = data;
            this.Count = count;
            this.Order = order;
            this.Selected = selected;
        }
        public ReadResponse(List<TModel> data, int count, int total, Dictionary<string, string> order, List<string> selected)
        {
            this.Data = data;
            this.Count = count;
            this.Order = order;
            this.Selected = selected;
            Total = total;
        }
    }
}
