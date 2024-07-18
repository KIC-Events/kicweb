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
        /// <summary>
        /// Exposes the KiCdbContext carried through the Dependency Injection. 
        /// </summary>
        /// <returns>KiCdbContext</returns>
        public KiCdbContext GetDBContext();
    }
}
