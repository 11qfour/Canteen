using ApiDomain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDomain
{
public class CartDetails
    {
        public Guid CartDetailsId { get; set; }
        // ������� ���� �� ������� (���� ������� -> ����� �������)
        public Guid CartId { get; set; }
        public Cart Cart { get; set; } = null!;

        // ������� ���� �� ����� (���� ����� -> ����� ���� � ���������� ��������)
        public Guid DishId { get; set; }
        public Dish Dish { get; set; } = null!;
        public int Quantity { get; set; }

        // ���� �� ������� ����� (����� ���������� �� Dish.Price, ��������, �� �������)
        [Column(TypeName = "decimal(18,2)")]
        public decimal PriceUnit { get; set; }
    }
}