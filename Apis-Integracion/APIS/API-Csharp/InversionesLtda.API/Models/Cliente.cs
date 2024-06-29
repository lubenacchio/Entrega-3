using System.ComponentModel.DataAnnotations;

namespace InversionesLtda.API.Models
{
    public class Cliente
    {
        public int id { get; set; }

        [Required(ErrorMessage = "La razón social es obligatoria.")]
        public string razonSocial { get; set; }

        [Required(ErrorMessage = "El RUT es obligatorio.")]
        [RegularExpression(@"^\d{1,2}\.\d{3}\.\d{3}[-][0-9kK]{1}$", ErrorMessage = "El formato del RUT no es válido.")]
        public string rut { get; set; }

        [Required(ErrorMessage = "La dirección es obligatoria.")]
        public string direccion { get; set; }
    }
}
