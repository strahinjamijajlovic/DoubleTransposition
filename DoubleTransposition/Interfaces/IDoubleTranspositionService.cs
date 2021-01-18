using DoubleTransposition.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DoubleTransposition.Interfaces
{
    public interface IDoubleTranspositionService
    {
        public string Encrypt(DoubleTranspositionModel data);
        public string Decrypt(DoubleTranspositionModel data);
    }
}
