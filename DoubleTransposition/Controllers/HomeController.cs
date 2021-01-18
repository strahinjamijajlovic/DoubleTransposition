using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DoubleTransposition.Models;
using DoubleTransposition.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using DoubleTransposition.Utils;
using DoubleTransposition.Interfaces;
using System.IO;

namespace DoubleTransposition.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDoubleTranspositionService _doubleTranspositionService;

        public HomeController(ILogger<HomeController> logger, IDoubleTranspositionService doubleTranspositionService)
        {
            _logger = logger;
            _doubleTranspositionService = doubleTranspositionService;
        }

        public IActionResult Index()
        {
            return View(new DoubleTranspositionViewModel
            {
                AlgorithmModes = CreateAlgorithmModesDropdown()
            });
        }

        public IActionResult ProcessFile(DoubleTranspositionViewModel vm)
        {
            if(ModelState.IsValid)
            {
                //prepare model for the encrypt/decrypt service
                DoubleTranspositionModel dataModel = new DoubleTranspositionModel
                {
                    FileName = vm.FileToProcess.FileName, //I didn't use this in the end
                    Stream = vm.FileToProcess.OpenReadStream(),
                    ColumnKeys = vm.ColumnsKey.Split(',').Select(Int32.Parse).ToList(), //split by ',' parse each as Int32 and turn them into a list
                    RowKeys = vm.RowsKey.Split(',').Select(Int32.Parse).ToList()
                };

                string resultString;
                byte[] resultArray;

                //if selected mode is encrypt, encrypt the recieved data, otherwise decrypt the data we recieved
                if (vm.SelectedMode == AlgorithmModes.Encrypt)
                    resultString = _doubleTranspositionService.Encrypt(dataModel);
                else
                    resultString = _doubleTranspositionService.Decrypt(dataModel);

                
                using (var memStream = new MemoryStream())
                using (TextWriter streamWriter = new StreamWriter(memStream))
                {
                    streamWriter.Write(resultString);
                    streamWriter.Flush();
                    resultArray = memStream.ToArray();                    
                }
                return File(resultArray, "text/plain", "resultFile.txt");
            }

            return View("Index", new DoubleTranspositionViewModel
            {
                AlgorithmModes = CreateAlgorithmModesDropdown()
            });
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private ICollection<SelectListItem> CreateAlgorithmModesDropdown()
        {
            return Enum.GetValues(typeof(AlgorithmModes))
                .Cast<AlgorithmModes>()
                .Select(x => new SelectListItem
                {
                    Value = ((int)x).ToString(),
                    Text = x.GetDescription()
                }).ToList();
        }
    }
}
