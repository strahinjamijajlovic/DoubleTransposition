using DoubleTransposition.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DoubleTransposition.Models
{
    public class DoubleTranspositionViewModel
    {
        [RegularExpression(@"((\d{1},)+)?(\d)", ErrorMessage = "The key must be in the format: \"digit,digit,digit\"")]
        [Required]
        [Display(Name = "Rows key")]
        public string RowsKey { get; set; } = string.Empty;
        [RegularExpression(@"((\d{1},)+)?(\d)", ErrorMessage = "The key must be in the format: \"digit,digit,digit\"")]
        [Required]
        [Display(Name = "Columns key")]
        public string ColumnsKey { get; set; } = string.Empty;

        public AlgorithmModes SelectedMode { get; set; }
        [Display(Name = "Alogrithm mode")]
        public IEnumerable<SelectListItem> AlgorithmModes { get; set; }

        [Display(Name = "File to be processed")]
        [Required]
        public IFormFile FileToProcess { get; set; } = default!;
    }
}
