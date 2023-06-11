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
        void Update(GarageDTO garage);
        List<GarageDTO> GetGarages();
        void DeleteGarage(int id);
        public List<GarageDTO> GetGaragesByID(int id);
        public List<GarageDTO> GetByPage(PageDTO p);
        public List<GarageDTO> GetGarageByUser(int id);
        public List<GarageDTO> FilterByCity(int provinceID);
    }
}
