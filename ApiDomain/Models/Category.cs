using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDomain
{
    public class Category
    {
        public Guid CategoryId { get; set; }
        public string NameCategory { get; set; }=string.Empty;
        //ссылка на связь с блюдом
        public ICollection<Dish>? Dishes { get; set; } = new List<Dish>();
    }
}