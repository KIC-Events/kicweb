using KiCData.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripts
{
    public class EmailFromDB
    {
        private IConfigurationRoot _config;
        private KiCdbContext _dbContext;

        public EmailFromDB(IConfigurationRoot configuration, KiCdbContext kiCdbContext)
        {
            _config = configuration;
            _dbContext = kiCdbContext;
        }

        public void SendEmailsFromDB()
        {

        }
    }
}
