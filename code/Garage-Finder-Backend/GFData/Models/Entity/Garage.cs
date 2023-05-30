﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GFData.Models.Entity
{
    public class Garage
    {
        [Key]
        public int GarageID { get; set; }
        [ForeignKey("User")]
        public int UserID { get; set; }
        public string GarageName { get; set; }
        public string Address { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string Status { get; set; }
        public string OpenTime { get; set; }
        public string Logo { get; set; }
        public string Imagies { get; set; }
        public string Location { get; set; }        
        public ICollection<Orders> Orders { get; set; }
        public ICollection<Service> Services { get; set; }
        public ICollection<FavoriteList> FavoriteList { get; set; }
        public ICollection<Feedback> Feedbacks { get; set; }
        public ICollection<GarageBrand> GarageBrands { get; set; }
        public ICollection<GarageInfo> GarageInfos { get; set; }
        public Users User { get; set; }
    }
}
