using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Swashbuckle.AspNetCore.Annotations;

namespace GestionStockApi.Models
{
    public partial class Producto
    {
        public int Id { get; set; }

        public decimal Precio { get; set; }

        [SwaggerIgnore]
        public DateTime FechaCarga { get; set; }

        public int IdCategoria { get; set; }

        [JsonIgnore]
        public virtual Categoria IdCategoriaNavigation { get; set; } = null!;
    }
}
