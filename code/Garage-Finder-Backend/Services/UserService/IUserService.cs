﻿using DataAccess.DTO.User.RequestDTO;
using DataAccess.DTO.User.ResponeModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.UserService
{
    public interface IUserService
    {
        void UpdateUser(UserUpdateDTO usersDTO, int userID);
        bool SendPhoneCode(string phoneNumber);
        UserInfor Get(int userId);
        void ChangePassword(int userId, string oldPassword, string newPassword);
    }
}
