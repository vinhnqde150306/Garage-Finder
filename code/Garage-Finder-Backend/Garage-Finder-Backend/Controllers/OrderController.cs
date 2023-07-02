﻿using AutoMapper;
using DataAccess.DTO;
using DataAccess.DTO.Orders.RequestDTO;
using DataAccess.DTO.Orders.ResponseDTO;
using DataAccess.DTO.User.ResponeModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using Repositories.Implements;
using Repositories.Interfaces;
using Services.OrderService;
using System.Security.Claims;

namespace Garage_Finder_Backend.Controllers
{
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository orderRepository;
        private readonly IServiceRepository serviceRepository;
        private readonly IOrderService orderService;
        private readonly IGuestOrderRepository guestOrderRepository;
        private readonly IMapper mapper;
        private UserInfor GetUserFromToken()
        {
            var jsonUser = User.FindFirstValue("user");
            var user = JsonConvert.DeserializeObject<UserInfor>(jsonUser);
            return user;
        }
        public OrderController(IOrderRepository orderRepository, IServiceRepository serviceRepository,
            IOrderService orderService, IGuestOrderRepository guestOrderRepository, IMapper mapper)
        {
            this.orderRepository = orderRepository;
            this.serviceRepository = serviceRepository;
            this.orderService = orderService;
            this.guestOrderRepository = guestOrderRepository;
            this.mapper = mapper;
        }

        //[HttpGet("GetAllOrder")]
        //public IActionResult GetAll()
        //{
        //    try
        //    {
        //        return Ok(orderRepository.GetAllOrders());
        //    }
        //    catch (Exception e)
        //    {

        //        return BadRequest(e.Message);
        //    }
        //}

        [HttpGet("GetByUser")]
        [Authorize]
        public IActionResult GetUserId()
        {
            try
            {
                var user = GetUserFromToken();
                return Ok(orderRepository.GetAllOrdersByUserId(user.UserID));
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }

        [HttpGet("GetOrderByGFId/{GFOrderID}")]
        [Authorize]
        public IActionResult GetOrder(int GFOrderID)
        {
            try
            {
                var user = GetUserFromToken();
                object order = orderService.GetOrderByGFID(GFOrderID, user.UserID);
                return Ok(order);
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }

        [HttpGet("GetOrderByGarageId/{GarageId}")]
        [Authorize]
        public IActionResult GetOrderByGarageId(int GarageId)
        {
            try
            {
                var orders= orderRepository.GetAllOrdersByGarageId(GarageId);
                var gorders = guestOrderRepository.GetOrdersByGarageId(GarageId);
                List<OrderDetailDTO> list = new List<OrderDetailDTO>();
                foreach (var ord in orders)
                {
                    list.Add(mapper.Map<OrderDetailDTO>(ord));
                }

                foreach (var order in gorders)
                {
                    list.Add(mapper.Map<OrderDetailDTO>(order));
                }
                return Ok(list);
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }

        //[HttpGet("GetGuestOrderByGarageId/{GarageId}")]
        //[Authorize]
        //public IActionResult GetGuestOrderByGarageId(int GarageId)
        //{
        //    try
        //    {
        //        var orders = guestOrderRepository.GetOrdersByGarageId(GarageId);
        //        return Ok(orders);
        //    }
        //    catch (Exception e)
        //    {

        //        return BadRequest(e.Message);
        //    }
        //}
        #region Add order
        [HttpPost("AddOrder")]
        [Authorize]
        public IActionResult AddOrderWithCar([FromBody] AddOrderWithCarDTO newOrder)
        {

            try
            {
                orderService.AddOrderWithCar(newOrder);

                return Ok("SUCCESS");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("AddOrderFromGuest")]
        [AllowAnonymous]
        public IActionResult AddOrderFromGuest([FromBody] AddOrderFromGuestDTO newOrder)
        {
            try
            {
                orderService.AddOrderFromGuest(newOrder);

                return Ok("SUCCESS");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("AddOrderWithoutCar")]
        [Authorize]
        public IActionResult AddOrderWithouCar([FromBody] AddOrderWithoutCarDTO newOrder)
        {
            try
            {
                var user = GetUserFromToken();
                orderService.AddOrderWithoutCar(newOrder, user.UserID);

                return Ok("SUCCESS");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        #endregion
        #region Update order

        [HttpPost("GarageAcceptOrder/{GFOrderID}")]
        [Authorize]
        public IActionResult GarageAcceptOrder(int GFOrderID)
        {
            try
            {
                var user = GetUserFromToken();
                orderService.GarageAcceptOrder(GFOrderID, user.UserID);
                return Ok("SUCCESS");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("GarageRejectOrder/{GFOrderID}")]
        [Authorize]
        public IActionResult GarageRejectOrder(int GFOrderID)
        {
            try
            {
                var user = GetUserFromToken();
                orderService.GarageRejectOrder(GFOrderID, user.UserID);
                return Ok("SUCCESS");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("GarageDoneOrder")]
        [Authorize]
        public IActionResult GarageDoneOrder([FromBody]DoneOrderDTO doneOrder)
        {
            try
            {
                var user = GetUserFromToken();
                orderService.GarageDoneOrder(doneOrder, user.UserID);
                return Ok("SUCCESS");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("GarageCancelOrder/{GFOrderID}")]
        [Authorize]
        public IActionResult GarageCancelOrder(int GFOrderID)
        {
            try
            {
                var user = GetUserFromToken();
                orderService.GarageCancelOrder(GFOrderID, user.UserID);
                return Ok("SUCCESS");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("UserCancelOrder/{GFOrderID}")]
        [Authorize]
        public IActionResult UserCancelOrder(int GFOrderID)
        {
            try
            {
                var user = GetUserFromToken();
                orderService.UserCancelOrder(user.UserID, GFOrderID);
                return Ok("SUCCESS");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        #endregion

        [HttpDelete("Delete/{id}")]
        public IActionResult Delete([FromBody] int id)
        {
            try
            {
                orderRepository.Delete(id);
                return Ok("SUCCESS");
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }
    }
}
