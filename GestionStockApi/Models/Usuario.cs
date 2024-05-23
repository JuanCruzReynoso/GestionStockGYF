using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GestionStockApi.Models;

public partial class Usuario
{
    [JsonIgnore]
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Contraseña { get; set; } = null!;
}
