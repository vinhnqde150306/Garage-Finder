﻿using DataAccess.DTO;
using DataAccess.DTO.RequestDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IGarageRepository
    {
        GarageDTO Add(AddGarageDTO garage);
        void Update(UpdateGarageDTO garage);
        List<GarageDTO> GetGarages();
        void DeleteGarage(int id);
        public List<GarageDTO> GetGaragesByID(int id);
        public List<GarageDTO> GetByPage(PageDTO p);
        public List<GarageDTO> GetGarageByUser(int? id);
        public List<GarageDTO> GetGarageByProviceId(int? provinceID);
        public List<GarageDTO> GetGarageByDistrictsID(int? id);
        public List<GarageDTO> GetGarageByCategoryId(int? id);
        public List<GarageDTO> GetGarageByBrandId(int? id);
    }
}
