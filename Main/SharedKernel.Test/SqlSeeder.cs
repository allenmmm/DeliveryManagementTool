using DMT.GeneratingOrderBooks.Domain.Entities;
using DMT.SharedKernel.ValueObjects;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace SharedKernel.Test
{
    public static class SqlSeeder
    {
        public static List<List<SqlParameter>> SetSupplierSQLValues(List<PlannedAndOverdueOrder> plannedAndOverdueOrders)
        {
            List<List<SqlParameter>> sqlParameters = new List<List<SqlParameter>>();
            plannedAndOverdueOrders.ForEach(o => {
                sqlParameters.Add(new
                    List<SqlParameter>()
                {
                    new SqlParameter("Id",o.SupplierId),
                    new SqlParameter("Details_Name", o.SupplierName)
                });

            });
            return sqlParameters;
        }

        public static List<List<SqlParameter>> SetOrderbookSQLValues(List<PlannedAndOverdueOrder> plannedAndOverdueOrders,
                                                                     DateTime currentDateTime)
        {
            Int64 val = (Int64)Orderbook.CalculateOrderbookId(  currentDateTime, 
                                                                plannedAndOverdueOrders.First().SupplierId);
            List<List<SqlParameter>> sqlParameters = new List<List<SqlParameter>>();
            plannedAndOverdueOrders.ForEach(o =>
            {
                sqlParameters.Add(new
                    List<SqlParameter>()
                {
                    new SqlParameter("Id",val),
                    new SqlParameter("DateStamp_DatePulled",o.DatePulled),
                    new SqlParameter("DateStamp_DateCreated",currentDateTime),
                    new SqlParameter("DateStamp_OrderbookWeek", OrderbookWeek_VO.Create(currentDateTime ).FormatString()),
                    new SqlParameter("SupplierId",o.SupplierId)
                });
            });
            return sqlParameters;
        }

        public static List<List<SqlParameter>> SetPandOSQLValues(List<PlannedAndOverdueOrder> plannedAndOverdueOrders)
        {
            List<List<SqlParameter>> sqlParameters = new List<List<SqlParameter>>();
            plannedAndOverdueOrders.ForEach(p =>
            {

                sqlParameters.Add(new
                    List<SqlParameter>()
                    {
                        new SqlParameter("Id",p.Id),
                        new SqlParameter("SupplierId",p.SupplierId),
                        new SqlParameter("SupplierName",p.SupplierName),
                        new SqlParameter("PartNumber", p.PartNumber),
                        new SqlParameter("PartDescription",p.PartDescription),
                        new SqlParameter("PurchaseOrder",p.PurchaseOrder),
                        new SqlParameter("POLineItem",p.POLineItem),
                        new SqlParameter("POSchedLine",p.POSchedLine),
                        new SqlParameter("OpenPOQty",p.OpenPOQty),
                        new SqlParameter("ItemDeliveryDate",p.ItemDeliveryDate),
                        new SqlParameter("StatDeliverySchedule",p.StatDeliverySchedule),
                        new SqlParameter("DatePulled",p.DatePulled)
                    });
            });
            return sqlParameters;
        }


        public static List<List<SqlParameter>> SetOrderSQLValues(List<PlannedAndOverdueOrder> plannedAndOverdueOrders,
                                                                DateTime currentDateTime)
        {
            Int64 val = (Int64)  Orderbook.CalculateOrderbookId(currentDateTime,
                                                                plannedAndOverdueOrders.First().SupplierId);
            List<List<SqlParameter>> sqlParameters = new List<List<SqlParameter>>();
            plannedAndOverdueOrders.ForEach(p =>
            {
                sqlParameters.Add(new
                    List<SqlParameter>()
                    {
                        new SqlParameter("PandOId",  p.Id),
                        new SqlParameter("OrderbookId", val),
                        new SqlParameter("Details_PurchaseOrder", p.PurchaseOrder),
                        new SqlParameter("Details_POLineItem", p.POLineItem),
                        new SqlParameter("Details_POSchedLine", p.POSchedLine),
                        new SqlParameter("Details_OpenPOQty", p.OpenPOQty),
                        new SqlParameter("Details_ItemDeliveryDate", p.ItemDeliveryDate),
                        new SqlParameter("Details_StatDeliverySchedule", p.StatDeliverySchedule),
                        new SqlParameter("Part_Number", p.PartNumber),
                        new SqlParameter("Part_Description", p.PartDescription)
                });
            });       
            return sqlParameters;
        }

        public static List<string> CreateSQLParameters(List<SqlParameter> sqlValues)
        {
            List<string> parameters = new List<string>();
            sqlValues.ForEach(p => parameters.Add("@" + p.ParameterName));
            return parameters;
        }
    }
}
