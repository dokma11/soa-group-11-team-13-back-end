﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Dtos.TouristEquipment
{
    public class TouristEquipmentCreateDto
    {
        public int TouristId { get; set; }
        public List<int>? EquipmentIds { get; set; }
    }
}
