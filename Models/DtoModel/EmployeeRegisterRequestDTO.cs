﻿using System.ComponentModel.DataAnnotations;

namespace IotSupplyStore.Models.DtoModel
{
    public class EmployeeRegisterRequestDTO
    {
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string AvatarLink { get; set; }
        public string Address { get; set; }
        public string citizenIdentification { get; set; } //REQUIRED
    }
}
