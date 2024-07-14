using KiCData.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiCData.Services
{
    public class KICDbService : IKICDbService
    {
        private IConfigurationRoot _config;
        private KiCdbContext _context;

        /// <summary>
        /// Instantiates a new instance of the KICDbService
        /// </summary>
        /// <param name="config">The IConfigurationRoot object from the service provider or build services. This provides the connection string used to connect to the database.</param>
        public KICDbService(IConfigurationRoot config)
        {
            _config = config;
            _context = new KiCdbContext(config);
        }

        /// <summary>
        /// Exposes the existing KiCdbContext contained within the service.
        /// </summary>
        /// <returns>KiCdbContext</returns>
        public KiCdbContext GetDBContext() { return _context; }
    }
}
