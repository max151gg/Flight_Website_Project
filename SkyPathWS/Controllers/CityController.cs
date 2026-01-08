using Microsoft.AspNetCore.Mvc;
using SkyPath_Models.Models;
using SkyPathWS.ORM.Repositories;
using System;
using System.Collections.Generic;

namespace SkyPathWS.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CityController : ControllerBase
    {
        RepositoryUOW repositoryUOW;

        public CityController()
        {
            this.repositoryUOW = new RepositoryUOW();
        }

        [HttpGet]
        public List<City> GetAll()
        {
            this.repositoryUOW.HelperOleDb.OpenConnection();
            try
            {
                // Your repository method name is GetALL (caps)
                return this.repositoryUOW.CityRepository.GetALL();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
            finally
            {
                this.repositoryUOW.HelperOleDb.CloseConnection();
            }
        }

        [HttpGet]
        public City GetById(string id)
        {
            this.repositoryUOW.HelperOleDb.OpenConnection();
            try
            {
                return this.repositoryUOW.CityRepository.GetById(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
            finally
            {
                this.repositoryUOW.HelperOleDb.CloseConnection();
            }
        }
    }
}
