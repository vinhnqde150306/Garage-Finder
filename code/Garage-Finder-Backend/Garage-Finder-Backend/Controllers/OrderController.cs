﻿using AutoMapper;
using DataAccess.DTO;
using DataAccess.DTO.Orders;
using DataAccess.DTO.Orders.RequestDTO;
using DataAccess.DTO.Orders.ResponseDTO;
using DataAccess.DTO.User.ResponeModels;
using GFData.Models.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using Repositories.Interfaces;
using Services.OrderService;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Garage_Finder_Backend.Controllers
{
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository orderRepository;
        private readonly IServiceRepository serviceRepository;
        private readonly IOrderService orderService;
        private readonly IGuestOrderRepository guestOrderRepository;
        private readonly ICarRepository carRepository;
        private readonly IUsersRepository usersRepository;
        private readonly ICategoryGarageRepository categoryGarageRepository;
        private readonly ICategoryRepository categoryRepository;
        private readonly IMapper mapper;

        public OrderController(IOrderRepository orderRepository, IServiceRepository serviceRepository,
            IOrderService orderService, IGuestOrderRepository guestOrderRepository, IMapper mapper,
            ICarRepository carRepository, IUsersRepository usersRepository, 
            ICategoryGarageRepository categoryGarageRepository, ICategoryRepository categoryRepository)
        {
            this.orderRepository = orderRepository;
            this.serviceRepository = serviceRepository;
            this.orderService = orderService;
            this.guestOrderRepository = guestOrderRepository;
            this.carRepository = carRepository;
            this.usersRepository = usersRepository;
            this.categoryGarageRepository = categoryGarageRepository;
            this.categoryRepository = categoryRepository;
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
                var user =User.GetTokenInfor();
                var orders = orderRepository.GetAllOrdersByUserId(user.UserID);
                List<OrderDetailDTO> list = new List<OrderDetailDTO>();
                foreach (var ord in orders)
                {
                    var o = mapper.Map<OrderDetailDTO>(ord);
                    var car = carRepository.GetCarById(ord.CarID);
                    var userDB = usersRepository.GetUserByID(car.UserID);
                    o = mapper.Map<OrdersDTO, OrderDetailDTO>(ord);
                    o = mapper.Map(car, o);
                    o = mapper.Map(userDB, o);
                    o.FileOrders = ord.FileOrders.Select(x => x.FileLink).ToList();
                    o.ImageOrders = ord.ImageOrders.Select(x => x.ImageLink).ToList();
                    o.Name = userDB.Name;

                    o.Category = new List<string>();
                    foreach (var detail in ord.OrderDetails)
                    {
                        var categoryGarage = categoryGarageRepository.GetById(detail.CategoryGarageID);
                        var cate = categoryRepository.GetCategory().Where(x => x.CategoryID == categoryGarage.CategoryID).FirstOrDefault();
                        o.Category.Add(cate.CategoryName);
                    }

                    list.Add(o);
                }
                return Ok(list);
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
                var user =User.GetTokenInfor();
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
                    var o = mapper.Map<OrderDetailDTO>(ord);
                    var car = carRepository.GetCarById(ord.CarID);
                    var user = usersRepository.GetUserByID(car.UserID);
                    o = mapper.Map<OrdersDTO, OrderDetailDTO>(ord);
                    o = mapper.Map(car, o);
                    o = mapper.Map(user, o);
                    o.FileOrders = ord.FileOrders.Select(x => x.FileLink).ToList();
                    o.ImageOrders = ord.ImageOrders.Select(x => x.ImageLink).ToList();
                    o.Name = user.Name;
                  
                    o.Category = new List<string>();
                    foreach (var detail in ord.OrderDetails)
                    {
                        var categoryGarage = categoryGarageRepository.GetById(detail.CategoryGarageID);
                        var cate = categoryRepository.GetCategory().Where(x => x.CategoryID == categoryGarage.CategoryID).FirstOrDefault();
                        o.Category.Add(cate.CategoryName);
                    }

                    list.Add(o);
                }

                foreach (var order in gorders)
                {
                    var o = mapper.Map<GuestOrderDTO, OrderDetailDTO>(order);
                    o.FileOrders = order.FileOrders.Select(x => x.FileLink).ToList();
                    o.ImageOrders = order.ImageOrders.Select(x => x.ImageLink).ToList();
                    o.Name = order.Name;
                    o.Category = new List<string>();
                    foreach (var detail in order.GuestOrderDetails)
                    {
                        var categoryGarage = categoryGarageRepository.GetById(detail.CategoryGarageID);
                        var cate= categoryRepository.GetCategory().Where(x => x.CategoryID == categoryGarage.CategoryID).FirstOrDefault();
                        o.Category.Add(cate.CategoryName);
                    }
                    list.Add(o);
                }
                return Ok(list);
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }

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
                var user =User.GetTokenInfor();
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
                var user =User.GetTokenInfor();
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
                var user =User.GetTokenInfor();
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
                var user =User.GetTokenInfor();
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
                var user =User.GetTokenInfor();
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
                var user =User.GetTokenInfor();
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
