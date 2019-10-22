using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace MvcMovie.Models
{
    public class Movie
    {
        public int Id { get; set; }

        [Display(Name = "Título")]
        [MinLength(3, ErrorMessage = "El nombre debe contener más de tres caracteres.")]
        [MaxLength(60, ErrorMessage = "El nombre debe contener, como máximo, sesenta caracteres.")]
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Title { get; set; }

        [Display(Name = "Fecha de estreno")]
        [DataType(DataType.Date, ErrorMessage = "Debes introducir una fecha de estreno válida.")]
        public DateTime ReleaseDate { get; set; }

        [Display(Name = "Género")]
        [RegularExpression(@"^[A-Z]+[a-zA-Z""'\s-]*$")]
        [Required(ErrorMessage = "El género es obligatorio.")]
        [StringLength(30)]
        [MaxLength(30, ErrorMessage = "El nombre debe contener, como máximo, treinta caracteres.")]
        public string Genre { get; set; }

        [Display(Name = "Precio")]
        [Range(1, 200)]
        [DataType(DataType.Currency, ErrorMessage = "El valor introducido no es un valor de moneda válido.")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }

        [Display(Name = "Clasificación")]
        [RegularExpression(@"^[A-Z]+[a-zA-Z0-9""'\s-]*$")]
        [StringLength(5)]
        [Required(ErrorMessage = "La clasificación es obligatoria.")]
        public string Rating { get; set; }
    }
}