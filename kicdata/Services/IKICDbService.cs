using KiCData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiCData.Services
{
    public interface IKICDbService
    {
        public KiCdbContext GetDBContext();
    }
}
