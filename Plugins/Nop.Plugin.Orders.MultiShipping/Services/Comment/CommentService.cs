using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Data;
using Nop.Plugin.Orders.MultiShipping.Domain;
using Nop.Plugin.Orders.MultiShipping.Models;
using Nop.Plugin.Orders.MultiShipping.Models.Comment;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Orders.MultiShipping.Services.Comment
{
    public class CommentService : ICommentService
    {
        private readonly IRepository<Tbl_Comment> _repository_TblComments;
        private readonly IWorkContext _workContext;
        private readonly IRepository<Shipment> _repository_Shipment;
        private readonly IRepository<Order> _repository_Order;
        private readonly IDbContext _dbContext;
        private readonly IRepository<Misc.Ticket.Domain.Tbl_Ticket> _repository_TblTicket;
        private readonly IStoreContext _storeContext;
        private readonly IRepository<Misc.Ticket.Domain.Tbl_Ticket_Detail> _repositoryTbl_TicketDetail;

        public CommentService(IRepository<Tbl_Comment> repository_TblComments,
            IWorkContext workContext,
            IRepository<Shipment> repository_Shipment,
            IRepository<Order> repository_Order,
            IDbContext dbContext,
            IRepository<Misc.Ticket.Domain.Tbl_Ticket> repository_TblTicket,
            IStoreContext storeContext,
            IRepository<Misc.Ticket.Domain.Tbl_Ticket_Detail> repositoryTbl_TicketDetail)
        {
            _repository_TblComments = repository_TblComments;
            _workContext = workContext;
            _repository_Shipment = repository_Shipment;
            _repository_Order = repository_Order;
            _dbContext = dbContext;
            _repository_TblTicket = repository_TblTicket;
            _storeContext = storeContext;
            _repositoryTbl_TicketDetail = repositoryTbl_TicketDetail;
        }




        public bool Insert(CommentModel commentModel, out string msg)
        {
            var validations = commentModel.IsValid();
            if (!string.IsNullOrEmpty(validations))
            {
                msg = validations;
                return false;
            }
            if (!_workContext.CurrentCustomer.IsRegistered())
            {
                msg = "برای ثبت نظر ابتدا باید وارد سامانه شوید کنید";
                return false;
            }
            var customerId = _workContext.CurrentCustomer.Id;
            var shipment = _repository_Shipment.Table.FirstOrDefault(p => p.TrackingNumber == commentModel.TrackingCode);
            if(shipment == null)
            {
                msg = "کد رهگیری وارد شده یافت نشد";
                return false;
            }
            var order = _repository_Order.GetById(shipment.OrderId);
            if(order == null || order.CustomerId != customerId)
            {
                msg = "این سفارش متعلق به شما نیست";
                return false;
            }

            if(_repository_TblComments.Table.Any(p=>p.CustomerId ==customerId && p.OrderId == order.Id && p.TrackingCode == commentModel.TrackingCode))
            {
                msg = "شما قبلا برای این کد رهگیره نظر خود را اعلام کرده اید";
                return false;
            }


            _repository_TblComments.Insert(new Tbl_Comment()
            {
                CustomerId = customerId,
                OrderId = order.Id,
                Description = commentModel.Description,
                Rate = commentModel.Rate,
                TrackingCode = commentModel.TrackingCode,
                IsPublished = false,
                CreateDate = DateTime.Now
            });

            if((int)commentModel.Rate < 3)
            {
                Misc.Ticket.Domain.Tbl_Ticket newticket = new Misc.Ticket.Domain.Tbl_Ticket();
                newticket.DateInsert = DateTime.Now;
                newticket.DepartmentId = 2;
                newticket.IdCategoryTicket = 2;
                newticket.ProrityId = 3;
                newticket.IdCustomer = _workContext.CurrentCustomer.Id;
                newticket.IsActive = true;
                newticket.Issue = "ثبت نظر جدید";
                newticket.StoreId = _storeContext.CurrentStore.Id;
                newticket.ShowCustomer = false;
                newticket.TypeTicket = false;
                _repository_TblTicket.Insert(newticket);


                Misc.Ticket.Domain.Tbl_Ticket_Detail temp = new Misc.Ticket.Domain.Tbl_Ticket_Detail();
                temp.DateInsert = DateTime.Now;
                temp.Description = $"یک نظر جدید با امتیاز {(int)commentModel.Rate} ثبت شده است، لطفا از قسمت نظرات پیگیری فرمایید";
                temp.IdTicket = newticket.Id;
                temp.Type = false;
                

                _repositoryTbl_TicketDetail.Insert(temp);
            }
            msg = "ثبت با موفقیت انجام شد، سپاس از نظر شما";
            return true;
        }


        public List<CommentSearchOutput> SearchComments(CommentSearchInput searchInput)
        {
            SqlParameter[] prms = new SqlParameter[] {
                new SqlParameter() { ParameterName = "@FromCityId", SqlDbType = SqlDbType.Int,Value = searchInput.FromCityId },
                new SqlParameter() { ParameterName = "@ToCityId", SqlDbType = SqlDbType.Int,Value = searchInput.ToCityId },
                new SqlParameter() { ParameterName = "@WeightCategory", SqlDbType = SqlDbType.NVarChar,Value = searchInput.WeightCategory ?? "" }
            };
            var queryResult = _dbContext.SqlQuery<CommentSearchOutput>("EXECUTE [dbo].[Sp_CommentReport] @FromCityId,@ToCityId,@WeightCategory", prms);
            return queryResult.ToList();
        }
    }
}
